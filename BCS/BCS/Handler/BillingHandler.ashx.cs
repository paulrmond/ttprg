using BCS.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NPOI.HSSF.UserModel;
using System.IO;
using BCS.Helper;

namespace BCS.Handler
{
    /// <summary>
    /// Summary description for BillingHandler
    /// </summary>
    public class BillingHandler : IHttpHandler
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
                        List<AssessmentBilling> assessments = new List<AssessmentBilling>();
                        List<AssessmentBilling> tempAssessments = new List<AssessmentBilling>();
                        db.SaveChanges();
                        try
                        {
                            var prop = excelHelper.GetProperties(typeof(AssessmentBilling), new[] { "REF_NO", "TRANS_NO", "COMP_CODE", "TRANS_DATE", "RATE_CODE", "RATE_TYPE", "TYPE_DESC", "ACCT_CODE", "NGAS_CODE", "DUE_DATE", "ZONE_CODE", "AMOUNT_DUE", "BILL_PERIOD_MONTH", "BILL_PERIOD_YEAR", "BILL_REFERENCE_NO", "BILL_PERIOD_ID", "BILL_DATE", "BILL_GENERATION_DATE", "CURRENT_BILLING_PERIOD" });
                            var data1 = excelHelper.ReadData<AssessmentBilling>(file.InputStream, file.FileName, prop, billPeriod,"Billing","");
                            assessments.AddRange(data1);
                            foreach (var item in assessments)
                            {
                                AssessmentBilling assessmentBilling = new AssessmentBilling();
                                assessmentBilling = item;
                                var compId = db.Company.Where(m => m.CompanyCode == item.COMP_CODE).Select(x => (int?)x.CompanyID).FirstOrDefault() ?? 0;
                                assessmentBilling.COMPANY_ID = compId.ToString();
                                tempAssessments.Add(assessmentBilling);
                            }
                            db.AssessmentBilling.AddRange(tempAssessments);
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

////List<String> notRegisteredCompany = new List<string>();
//var currentSheet = package.Workbook.Worksheets;
//var workSheet = currentSheet.First();
//var noOfCol = workSheet.Dimension.End.Column;
//var noOfRow = workSheet.Dimension.End.Row;

//                                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
//                                {
//                                    Assessment assessment = new Assessment();

//var generationDate = workSheet.Cells[rowIterator, 18].Value;
//var billingPeriod = workSheet.Cells[rowIterator, 16].Value;
//var billingDate = workSheet.Cells[rowIterator, 17].Value;
//var dueDate = workSheet.Cells[rowIterator, 10].Value;
//var debitAmount = workSheet.Cells[rowIterator, 12].Value;
//var transactionDate = workSheet.Cells[rowIterator, 4].Value;

//var compCode = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : "";
//int? compId = db.Company.FirstOrDefault(m => m.CompanyCode == compCode).CompanyID;
//assessment.CompanyId = compId == null ? 0 : (int)compId;

//                                    assessment.TransactionType = "BILLING";
//                                    //assessment.BillingPeriod = billingPeriod == null ? 0 : int.Parse(billingPeriod.ToString());
//                                    assessment.BillingPeriod = maxid;
//                                    assessment.BillingType = workSheet.Cells[rowIterator, 6].Value.ToString() != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "";
//                                    assessment.Currency = "PHP";

//                                    assessment.BillingDate = billingDate == null ? DateTime.Now : DateTime.Parse(billingDate.ToString());
//                                    assessment.DueDate = dueDate == null ? DateTime.Now : DateTime.Parse(dueDate.ToString());
//                                    assessment.BillingReference = workSheet.Cells[rowIterator, 15].Value != null ? workSheet.Cells[rowIterator, 15].Value.ToString() : "";
//                                    assessment.TransactionReference = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : "";
//                                    assessment.DebitAmount = debitAmount == null ? 0 : decimal.Parse(debitAmount.ToString());
//                                    assessment.CreditAmount = 0;
//                                    assessment.TransactionDate = transactionDate == null ? DateTime.Now : DateTime.Parse(transactionDate.ToString());
//                                    assessment.TypeDesc = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString() : "";

//                                    assessment.CompCode = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString() : "";

//                                    assessment.EngineeringApplicationRefNo = workSheet.Cells[rowIterator, 1].Value != null ? workSheet.Cells[rowIterator, 1].Value.ToString() : "";

//                                    assessment.GenerationDate = generationDate != null ? DateTime.Parse(generationDate.ToString()) : DateTime.Now;

//                                    assessment.BillingMonth = workSheet.Cells[rowIterator, 13].Value != null ? workSheet.Cells[rowIterator, 13].Value.ToString() : "";
//                                    assessment.BillingYear = workSheet.Cells[rowIterator, 14].Value != null ? workSheet.Cells[rowIterator, 14].Value.ToString() : "";
//                                    assessment.NGASCode = workSheet.Cells[rowIterator, 9].Value != null ? workSheet.Cells[rowIterator, 9].Value.ToString() : "";


//                                    assessment.RateCode = workSheet.Cells[rowIterator, 5].Value != null ? workSheet.Cells[rowIterator, 5].Value.ToString() : "";
//                                    assessment.RateType = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "";
//                                    assessment.ZoneCode = workSheet.Cells[rowIterator, 11].Value != null ? workSheet.Cells[rowIterator, 11].Value.ToString() : "";
//                                    assessment.Status = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString() : "";


//                                    //adminfee.BillingPeriodId = billPeriod;
//                                    //if (!db.Company.Any(m => m.CompanyCode == adminfee.Comp_Code))
//                                    //    notRegisteredCompany.Add(adminfee.Company_Name);