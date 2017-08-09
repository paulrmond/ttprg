using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;

namespace BCS.Handler
{
    /// <summary>
    /// Summary description for CopyPassedOnBillingHandler
    /// </summary>    
    public class CopyPassedOnBillingHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var a = context.Request.QueryString["Copy"].ToString();
            var b = context.Request.QueryString["Type"].ToString();
            var origin = context.Request.QueryString["Origin"].ToString();
            var billing = context.Request.QueryString["Billing"].ToString();
            DateTime originNew = Convert.ToDateTime(origin);
            DateTime billingNew = Convert.ToDateTime(billing);
            HttpFileCollection files = context.Request.Files;
            HttpPostedFile file = files["uploadData"];

            int copy = int.Parse(a);

            BCS_Context db = new BCS_Context();
            if ((file != null) && (file.ContentLength > 0) && (!string.IsNullOrEmpty(file.FileName)))
            {
                using (var dbtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        ApplicationDbContext con = new ApplicationDbContext();
                        //db.PassedOnBillingInformation.RemoveRange(db.PassedOnBillingInformation)
                        var userid = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        var zonegroup = con.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                        SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(zonegroup);
                        IEnumerable<Company> company = searchCompanyPerGroup.Companies;
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 12; rowIterator <= noOfRow; rowIterator++)
                            {
                                var compid = workSheet.Cells[rowIterator, 3].Value;
                                var newCompId = int.Parse(compid.ToString());
                                Company comp = company.Where(m => m.CompanyID == newCompId).FirstOrDefault();
                                var type = workSheet.Cells[rowIterator, 12].Value.ToString() != null ? workSheet.Cells[rowIterator, 12].Value.ToString() : "";

                                if (type == b)
                                {
                                    PassedOnBillingInformation passedOnBillingInformation = new PassedOnBillingInformation();
                                    passedOnBillingInformation.Type = type;
                                    var cid = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value : 0;
                                    passedOnBillingInformation.CompanyId = int.Parse(cid.ToString());
                                    passedOnBillingInformation.BillingPeriod = int.Parse(a);
                                    var amt = workSheet.Cells[rowIterator, 13].Value != null ? workSheet.Cells[rowIterator, 13].Value : 0;
                                    passedOnBillingInformation.Amount = decimal.Parse(amt.ToString());
                                    passedOnBillingInformation.CreatedBy = userid;
                                    passedOnBillingInformation.CreateDate = DateTime.Now;
                                    passedOnBillingInformation.OriginDate = originNew;
                                    passedOnBillingInformation.BillingDate = billingNew;
                                    passedOnBillingInformation.Remarks = workSheet.Cells[rowIterator, 14].ToString();

                                    db.PassedOnBillingInformation.Add(passedOnBillingInformation);
                                    db.SaveChanges();
                                }
                            }
                        }
                        dbtransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                        dbtransaction.Rollback();
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