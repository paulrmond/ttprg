using BCS.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Handler
{
    /// <summary>
    /// Summary description for PaymentHandler
    /// </summary>
    public class PaymentHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                BCS_Context db = new BCS_Context();
                ApplicationDbContext appdb = new ApplicationDbContext();
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile file = files["uploadData"];

                var logUSer = HttpContext.Current.User.Identity.Name;
                string ZoneGroup = appdb.Users.FirstOrDefault(m => m.UserName == logUSer).ZoneGroup;
                var billingPeriod1 = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList();
                var billPeriod = billingPeriod1.Max(m => m.BillingPeriodId);

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string filename = file.FileName;
                    string filecontenttype = file.ContentType;
                    byte[] filebytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(filebytes, 0, Convert.ToInt32(file.ContentLength));
                    
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var excelHelper = new Helper.ExcelHelper();
                        List<AssessmentPayment> assessments = new List<AssessmentPayment>();
                        List<AssessmentPayment> tempAssessments = new List<AssessmentPayment>();
                        db.SaveChanges();
                        try
                        {
                            var prop = excelHelper.GetProperties(typeof(AssessmentPayment), new[] {
                                "REF_NO", "COMP_CODE", "RATE_TYPE", "ZONE_CODE",
                                "BILL_PERIOD_MONTH", "BILL_PERIOD_YEAR", "BILL_GENERATION_DATE",
                                "OR_NUM", "OR_DATE", "CHECK_AMOUNT", "CASH_AMOUNT", "CHECK_NO",
                                "CHECK_BRANCH", "CHECK_DATE", "TOTAL_PAYMENT", "PRINCIPAL_PAYMENT",
                                "INTEREST_PAYMENT", "ASSESSMENT_TYPE", "BILL_COVERAGE_DATE_FROM",
                                "BILL_COVERAGE_DATE_TO","RATE_CODE","ACCT_CODE", "CURRENT_BILLING_PERIOD" });

                            var data1 = excelHelper.ReadData<AssessmentPayment>(file.InputStream, file.FileName, prop, billPeriod,"Payment","");
                            assessments.AddRange(data1);
                            foreach (var item in assessments)
                            {
                                AssessmentPayment assessmentPayment = new AssessmentPayment();
                                assessmentPayment = item;
                                var compId = db.Company.Where(m => m.CompanyCode == item.COMP_CODE).Select(x=> (int?)x.CompanyID).FirstOrDefault() ?? 0;
                                assessmentPayment.COMPANY_ID = compId.ToString();
                                tempAssessments.Add(assessmentPayment);
                            }
                            db.AssessmentPayment.AddRange(tempAssessments);
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

//using (var package = new ExcelPackage(file.InputStream))
//{
//    //List<String> notRegisteredCompany = new List<string>();
//    var currentSheet = package.Workbook.Worksheets;
//    var workSheet = currentSheet.First();
//    var noOfCol = workSheet.Dimension.End.Column;
//    var noOfRow = workSheet.Dimension.End.Row;

//    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
//    {
//        Assessment assessment = new Assessment();
//        var totalPayment = workSheet.Cells[rowIterator, 15].Value;
//        var coverageFrom = workSheet.Cells[rowIterator, 19].Value;
//        var coverageTo = workSheet.Cells[rowIterator, 20].Value;
//        var generationDate = workSheet.Cells[rowIterator, 7].Value;
//        var cashPayment = workSheet.Cells[rowIterator, 11].Value;
//        var checkPayment = workSheet.Cells[rowIterator, 10].Value;
//        var checkDate = workSheet.Cells[rowIterator, 14].Value;
//        var interestPayment = workSheet.Cells[rowIterator, 17].Value;
//        var orDate = workSheet.Cells[rowIterator, 9].Value;
//        var principalPayment = workSheet.Cells[rowIterator, 16].Value;

//        var compCode = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : "";
//        int? compId = db.Company.FirstOrDefault(m => m.CompanyCode == compCode).CompanyID;
//        assessment.CompanyId = compId == null ? 0 : (int)compId;
//        assessment.TransactionType = "PAYMENT";

//        assessment.BillingPeriod = maxid;
//        assessment.BillingType = workSheet.Cells[rowIterator, 3].Value.ToString() != null ? workSheet.Cells[rowIterator, 3].Value.ToString() : "";
//        assessment.Currency = "PHP";

//        assessment.BillingDate = DateTime.Now;
//        assessment.DueDate = DateTime.Now;
//        assessment.DebitAmount = 0;
//        assessment.CreditAmount = totalPayment == null ? 0 : decimal.Parse(totalPayment.ToString());
//        assessment.TransactionDate = DateTime.Now;
//        assessment.CompCode = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : "";
//        assessment.EngineeringApplicationRefNo = workSheet.Cells[rowIterator, 1].Value != null ? workSheet.Cells[rowIterator, 1].Value.ToString() : "";
//        assessment.CoverageFrom = coverageFrom != null ? DateTime.Parse(coverageFrom.ToString()) : DateTime.Now;
//        assessment.CoverageTo = coverageTo != null ? DateTime.Parse(coverageTo.ToString()) : DateTime.Now;
//        assessment.GenerationDate = generationDate != null ? DateTime.Parse(generationDate.ToString()) : DateTime.Now;
//        assessment.BillingMonth = workSheet.Cells[rowIterator, 5].Value != null ? workSheet.Cells[rowIterator, 5].Value.ToString() : "";
//        assessment.BillingYear = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "";
//        assessment.CashPayment = cashPayment == null ? 0 : decimal.Parse(cashPayment.ToString());
//        assessment.CheckPayment = checkPayment == null ? 0 : decimal.Parse(checkPayment.ToString());
//        assessment.CheckBranch = workSheet.Cells[rowIterator, 13].Value != null ? workSheet.Cells[rowIterator, 13].Value.ToString() : "";
//        assessment.CheckDate = checkDate == null ? DateTime.Now : DateTime.Parse(checkDate.ToString());
//        assessment.CheckNo = workSheet.Cells[rowIterator, 12].Value != null ? workSheet.Cells[rowIterator, 12].Value.ToString() : "";
//        assessment.InterestPayment = interestPayment == null ? 0 : decimal.Parse(interestPayment.ToString());
//        assessment.ORDate = orDate == null ? DateTime.Now : DateTime.Parse(orDate.ToString());
//        assessment.ORNumber = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString() : "";
//        assessment.PrincipalPayment = principalPayment == null ? 0 : decimal.Parse(principalPayment.ToString());
//        assessment.RateCode = workSheet.Cells[rowIterator, 21].Value != null ? workSheet.Cells[rowIterator, 21].Value.ToString() : "";
//        assessment.RateType = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString() : "";
//        assessment.ZoneCode = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString() : "";
//        assessment.Status = workSheet.Cells[rowIterator, 22].Value != null ? workSheet.Cells[rowIterator, 22].Value.ToString() : "";


//        //adminfee.BillingPeriodId = billPeriod;
//        //if (!db.Company.Any(m => m.CompanyCode == adminfee.Comp_Code))
//        //    notRegisteredCompany.Add(adminfee.Company_Name);

//        db.Assessment.Add(assessment);
//        db.SaveChanges();
//    }
//    //TempData["notRegisteredCompany"] = notRegisteredCompany;
//    //TempData["TransactionSuccess"] = "Complete";
//    transaction.Commit();
//}