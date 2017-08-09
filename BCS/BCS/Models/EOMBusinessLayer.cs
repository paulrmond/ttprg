using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    //Get all the balances of a Company
    public class EOMBusinessLayer
    {
        BCS_Context db = new BCS_Context();
        string userid;
        int month;
        int maxBillingPeriodId;
        string[] BillingType = new string[] { "POLE RENTAL", "RENTAL", "GARBAGE", "WATER", "FRANCHISE", "SECURITY GUARD", "SEWERAGE" };

        public EOMBusinessLayer(string userid, int month,int maxBillingPeriodId)
        {
            this.userid = userid;
            //this.month = month; //uncomment if months selection in view is enabled
            this.month = month;
            this.maxBillingPeriodId = maxBillingPeriodId;
        }
        public EOMBusinessLayer(string userid)
        {
            this.userid = userid;
            //this.month = month; //uncomment if months selection in view is enabled
        }

        public bool EOMTransaction()
        {
            bool isSuccess = false;
            ApplicationDbContext context = new ApplicationDbContext();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(ZoneGroup);
            List<Company> Companies = searchCompanyPerGroup.Companies;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int MaxId = db.EOMProcessing.Max(m => (int?)m.EOMNumber) ?? 0;
                    BillingPeriod billingPeriod = new BillingPeriod();
                    billingPeriod = db.BillingPeriod.FirstOrDefault(m => m.BillingPeriodId == maxBillingPeriodId);
                    billingPeriod.EOMStatus = "DONE";
                    db.Entry(billingPeriod).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    foreach (var item in Companies)
                    {
                        decimal[] Balance = new decimal[7];
                        int CompanyId = item.CompanyID;
                        DateTime? CompanyDateCreated = db.Company.SingleOrDefault(m => m.CompanyID == CompanyId).CreateDate;
                        for (int i = 0; i < BillingType.Count(); i++)
                        {
                            string billingtype = BillingType[i].ToString();
                            var Credit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == billingtype).Where(m => m.CompanyId == CompanyId && m.BillingSubType.ToUpper() != "INTEREST").Sum(m => (decimal?)m.CreditAmount) ?? 0;
                            var Debit = db.SubsidiaryLedger.Where(m => m.BillingType.ToUpper() == billingtype).Where(m => m.CompanyId == CompanyId && m.BillingSubType.ToUpper() != "INTEREST").Sum(m => (decimal?)m.DebitAmount) ?? 0;
                            Balance[i] = Debit - Credit;
                        }

                        //Save to database
                        if (!isExist(item.CompanyID))
                        {
                            for (int i = 0; i < BillingType.Count() - 1; i++)
                            {
                                EOMProcessing eOMProcessing = new EOMProcessing();
                                eOMProcessing.CompanyId = CompanyId;
                                eOMProcessing.BillingType = BillingType[i].ToString();
                                eOMProcessing.EOMNumber = ++MaxId;
                                db.EOMProcessing.Add(eOMProcessing);
                                db.SaveChanges();
                            }
                        }

                        for (int i = 0; i < BillingType.Count() - 1; i++)
                        {
                            string billingtype = BillingType[i].ToString();
                            EOMProcessing updateEOM = db.EOMProcessing.Where(m => m.CompanyId == CompanyId).Where(m => m.BillingType == billingtype).SingleOrDefault();
                            switch (month)
                            {
                                case 12:
                                    {
                                        updateEOM.November = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 11:
                                    {
                                        updateEOM.October = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 10:
                                    {
                                        updateEOM.September = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 9:
                                    {
                                        updateEOM.August = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 8:
                                    {
                                        updateEOM.July = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 7:
                                    {
                                        updateEOM.June = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 6:
                                    {
                                        updateEOM.May = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 5:
                                    {
                                        updateEOM.April = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 4:
                                    {
                                        updateEOM.March = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 2:
                                    {
                                        updateEOM.February = Balance[i];
                                        db.SaveChanges();
                                        break;
                                    }
                                case 1:
                                    {
                                        updateEOM.January = 0;
                                        updateEOM.February = 0;
                                        updateEOM.March = 0;
                                        updateEOM.April = 0;
                                        updateEOM.May = 0;
                                        updateEOM.June = 0;
                                        updateEOM.July = 0;
                                        updateEOM.August = 0;
                                        updateEOM.September = 0;
                                        updateEOM.October = 0;
                                        updateEOM.November = 0;
                                        updateEOM.December = 0;

                                        int CompanyYear = CompanyDateCreated.Value.Year - DateTime.Now.Year;
                                        if(CompanyYear == 0)
                                            updateEOM.PrevYr1 = Balance[i];
                                        else if(CompanyYear == 1)
                                            updateEOM.PrevYr2 = Balance[i];
                                        else if (CompanyYear == 2)
                                            updateEOM.PrevYr3 = Balance[i];
                                        else if (CompanyYear == 3)
                                            updateEOM.PrevYr4 = Balance[i];
                                        else if (CompanyYear >= 4)
                                            updateEOM.OtherPrevYr = Balance[i];

                                        //int prevYrToUpdate = 0;

                                        //if (updateEOM.PrevYr1 != null)
                                        //    updateEOM.PrevYr1 = Balance[i];
                                        //else if (updateEOM.PrevYr2 != null)
                                        //    updateEOM.PrevYr2 = Balance[i];
                                        //else if (updateEOM.PrevYr3 != null)
                                        //    updateEOM.PrevYr3 = Balance[i];
                                        //else if (updateEOM.PrevYr4 != null)
                                        //    updateEOM.PrevYr4 = Balance[i];
                                        //else if (updateEOM.OtherPrevYr != null)
                                        //    updateEOM.OtherPrevYr = Balance[i];

                                        ////below code will only use if multiple EOM Process in month of January.
                                        ////Check the latest prev year data.
                                        //if (updateEOM.PrevYr1 != null)
                                        //    prevYrToUpdate++;
                                        //else if (updateEOM.PrevYr2 != null)
                                        //    prevYrToUpdate++;
                                        //else if (updateEOM.PrevYr3 != null)
                                        //    prevYrToUpdate++;
                                        //else if (updateEOM.PrevYr4 != null)
                                        //    prevYrToUpdate++;
                                        //else if(updateEOM.OtherPrevYr != null)
                                        //    prevYrToUpdate++;

                                        ////update the latest prev year data
                                        //if (prevYrToUpdate ==1)
                                        //    updateEOM.PrevYr1 = Balance[i];
                                        //else if (prevYrToUpdate == 2)
                                        //    updateEOM.PrevYr2 = Balance[i];
                                        //else if (prevYrToUpdate == 3)
                                        //    updateEOM.PrevYr3 = Balance[i];
                                        //else if (prevYrToUpdate == 4)
                                        //    updateEOM.PrevYr4 = Balance[i];
                                        //else if (prevYrToUpdate == 5)
                                        //    updateEOM.OtherPrevYr = Balance[i];

                                        db.SaveChanges();
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }                        
                    }//end foreach company
                    transaction.Commit();
                    isSuccess = true;                  
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }//end transaction
            return isSuccess;
        }



        private bool isExist(int CompanyId)
        {
            bool isexist = false;
            isexist = db.EOMProcessing.Any(m => m.CompanyId == CompanyId);
            return isexist;
        }
        //decimal pole = Balance[0];
        //decimal rental = Balance[1];
        //decimal garbage = Balance[2];
        //decimal water = Balance[3];
        //decimal franchise = Balance[4];
        //decimal security = Balance[5];
        //decimal sewerage = Balance[6];
    }
}