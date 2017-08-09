using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    public class AegingBLL
    {
        BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();
        int month;
        int year;
        public AegingBLL(int month, int year)
        {
            this.month = month;
            this.year = year;
        }

        private class Da
        {
            public int CompanyId { get; set; }
            public string ZoneCode { get; set; }
            public string ZoneGroupCode { get; set; }
            public string CompanyCode { get; set; }
        }
        public bool TestAeging()
        {
            bool isSuccess = false;
            IEnumerable<Da> a = db.Database.SqlQuery<Da>("SELECT Distinct(SubsidiaryLedgers.CompanyId), Companies.ZoneCode, ZoneGroups.ZoneGroupCode,Companies.CompanyCode " +
                            "FROM SubsidiaryLedgers INNER JOIN " +
                            "Companies ON SubsidiaryLedgers.CompanyId = Companies.CompanyID INNER JOIN " +
                            "Zones ON Companies.ZoneCode = Zones.ZoneCode INNER JOIN " +
                            "ZoneGroups ON Zones.ZoneGroup = ZoneGroups.ZoneGroupId").ToList();

            var userName = HttpContext.Current.User.Identity.Name;
            var zonegroup = context.Users.Where(m => m.UserName == userName).FirstOrDefault().ZoneGroup;
            a = a.Where(m => m.ZoneGroupCode == zonegroup).ToList();
            string[] billingTypes = new string[] { "POLE RENTAL", "GARBAGE", "WATER", "FRANCHISE", "PASSED ON BILLING", "SEWERAGE", "ADMIN FEE", "RENTAL" };
            using (var dbtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Database.ExecuteSqlCommand("Delete from BCSAgingOutputs where BillOriginYear = '"+ year + "' and CollectionMonth = '"+ month +"'");
                    foreach (var item in a)
                    {
                        BCSAgingOutput bCSAgingOutput = new BCSAgingOutput();
                        bCSAgingOutput.CompanyId = item.CompanyId;
                        bCSAgingOutput.CompCode = item.CompanyCode;
                        bCSAgingOutput.CreditAdjustment = 0;
                        bCSAgingOutput.DebitAdjustment = 0;
                        bCSAgingOutput.BillOriginMonth = month;
                        bCSAgingOutput.BillOriginYear = year;
                        bCSAgingOutput.CollectionMonth = month;
                        bCSAgingOutput.CollectionYear = year;
                        bCSAgingOutput.CollectionPeriod = month + "-" + year;
                        foreach (var billingType in billingTypes)
                        {
                            AegingDAL aegingDAL = new AegingDAL(item.CompanyId, billingType);
                            bCSAgingOutput.BillingType = billingType;
                            bCSAgingOutput.PreviousBalance = aegingDAL.Dr;
                            bCSAgingOutput.PresentBalance = (aegingDAL.Dr - aegingDAL.Cr);
                            bCSAgingOutput.CollectionAmount = aegingDAL.Cr;
                            db.BCSAgingOutput.Add(bCSAgingOutput);
                            db.SaveChanges();
                        }
                    }
                    AssessmentAging();
                    dbtransaction.Commit();
                    isSuccess = true;
                }
                catch (Exception)
                {
                    dbtransaction.Rollback();
                }

            }
            return isSuccess;
        }

        public void AssessmentAging()
        {
            //Get list of Company in two tables
            IEnumerable<String> listOfCompCode = db.Database.SqlQuery<String>("select distinct(a.COMP_CODE) from (select COMP_CODE from AssessmentBillings " +
                                                                "UNION ALL " +
                                                                "Select COMP_CODE from AssessmentPayments) as a").ToList();
            //using (var dbtransaction = db.Database.BeginTransaction())
            //{
                try
                {
                    foreach (var item in listOfCompCode)
                    {
                        //For each company get the RATE_TYPE in two table and compute the Dr and Cr
                        IEnumerable<String> listOfRateType = db.Database.SqlQuery<String>("select distinct(a.RATE_TYPE) " +
                                                                                      "from(select RATE_TYPE, COMP_CODE from AssessmentBillings " +
                                                                                      "UNION ALL " +
                                                                                      "Select RATE_TYPE, COMP_CODE from AssessmentPayments) as a " +
                                                                                      "Where a.COMP_CODE = '" + item + "'").ToList();
                        //For each RATE_TYPE loop thru each Reference number
                        foreach (var ratetype in listOfRateType)
                        {
                            //var paymentRefNo = db.AssessmentPayment.Where(m => m.COMP_CODE == item.ToString()).Where(m => m.RATE_TYPE == ratetype.ToString()).Select(m => m.REF_NO).ToList();
                            //var billingRefNo = db.AssessmentBilling.Where(m => m.COMP_CODE == item.ToString()).Where(m => m.RATE_TYPE == ratetype.ToString()).Select(m => m.REF_NO).ToList();
                            //paymentRefNo.AddRange(billingRefNo);
                            //IEnumerable<string> distinctRefNo = paymentRefNo.Distinct();

                            var CompanyId = db.Company.Where(m => m.CompanyCode.ToUpper() == item.ToUpper()).FirstOrDefault().CompanyID;
                            BCSAgingOutput bCSAgingOutput = new BCSAgingOutput();
                            bCSAgingOutput.CompanyId = CompanyId;
                            bCSAgingOutput.CompCode = item;
                            bCSAgingOutput.CreditAdjustment = 0;
                            bCSAgingOutput.DebitAdjustment = 0;
                            bCSAgingOutput.BillOriginMonth = month;
                            bCSAgingOutput.BillOriginYear = year;
                            bCSAgingOutput.CollectionMonth = month;
                            bCSAgingOutput.CollectionYear = year;
                            bCSAgingOutput.CollectionPeriod = month + "-" + year;

                            var listdr = db.AssessmentPayment.Where(m => m.COMP_CODE == item.ToString()).Where(m => m.RATE_TYPE == ratetype.ToString()).Select(m => m.PRINCIPAL_PAYMENT).ToList(); ;
                            var listcr = db.AssessmentBilling.Where(m => m.COMP_CODE == item.ToString()).Where(m => m.RATE_TYPE == ratetype.ToString()).Select(m => m.AMOUNT_DUE).ToList(); ;
                            var ab = listdr.Select(m => double.Parse(m)).ToList();
                            var cd = listcr.Select(m => double.Parse(m)).ToList();
                            var dr = ab.Sum();
                            var cr = cd.Sum();
                            
                            bCSAgingOutput.BillingType = ratetype;
                            bCSAgingOutput.PreviousBalance = Math.Round(dr,2);
                            bCSAgingOutput.PresentBalance = Math.Round((dr - cr),2);
                            bCSAgingOutput.CollectionAmount = Math.Round(cr,2);
                            db.BCSAgingOutput.Add(bCSAgingOutput);
                            db.SaveChanges();

                        }
                    }
                    //dbtransaction.Commit();
                }
                catch (Exception ex)
                {
                    //dbtransaction.Rollback();
                }
            //}
        }
    }
}