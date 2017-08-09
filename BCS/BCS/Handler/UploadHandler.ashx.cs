using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;
using OfficeOpenXml;
using System.Data.SqlClient;
using NPOI.HSSF.UserModel;
using System.IO;
using BCS.Helper;

namespace BCS.Handler
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                var billPeriodFromContext = context.Request.QueryString["billperiod"].ToString();
                var zoneTypeFromContext = context.Request.QueryString["zonetype"].ToString();
                int billPeriod = int.Parse(billPeriodFromContext);
                BCS_Context db = new BCS_Context();
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile file = files["uploadData"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var excelHelper = new Helper.ExcelHelper();
                        List<AdminFee> adminfees = new List<AdminFee>();
                        db.AdminFee.RemoveRange(db.AdminFee.Where(m => m.BillingPeriodId == billPeriod && m.Upload_Type.ToUpper().Trim() == zoneTypeFromContext.ToUpper().Trim()).ToList());
                        db.SaveChanges();
                        try
                        {
                            var uname = HttpContext.Current.User.Identity.Name;
                            ApplicationDbContext dbcontext = new ApplicationDbContext();
                            var user = dbcontext.Users.Where(m => m.UserName == uname).FirstOrDefault();
                            var zone = user.ZoneGroup;
                            var zoneid = db.ZoneGroup.Where(m => m.ZoneGroupCode == zone).FirstOrDefault().ZoneGroupId;

                            var prop = excelHelper.GetProperties(typeof(AdminFee), new[] { "Ecozone", "Zone_Type", "Company_Name", "Enterprise_Type", "Employment", "Zone_Code", "Month", "Year", "Comp_Code", "Developer", "Dev_Comp_Code", "Total_Locators", "Total_Employment", "BillingPeriodId","Upload_Type" });
                            var data = excelHelper.ReadData<AdminFee>(file.InputStream, file.FileName, prop, billPeriod, "Admin", zoneTypeFromContext);

                            foreach (var item in data)
                            {
                                var groupId = "";
                                try
                                {
                                    string tempZone = "";               
                                    if (zoneTypeFromContext.ToUpper().Trim() != "ALL")
                                    {
                                        if (item.Zone_Type.ToUpper().Trim() == "IT CENTER" || item.Zone_Type.ToUpper().Trim() == "IT PARK")
                                            tempZone = "IT";
                                        else if(item.Zone_Type.ToUpper().Trim() != "IT CENTER" && item.Zone_Type.ToUpper().Trim() != "IT PARK" && item.Zone_Type.ToUpper().Trim() != "MANUFACTURING CEZ")
                                            tempZone = "OTHERS";
                                        else if(item.Zone_Type.ToUpper().Trim() == "MANUFACTURING SEZ")
                                            tempZone = "MANUFACTURING";
                                        else
                                            tempZone = item.Zone_Type.ToUpper().Trim();
                                        //tempZone = item.Zone_Type.ToUpper().Trim() == "IT CENTER" || item.Zone_Type.ToUpper().Trim() == "IT PARK" ?
                                        //    "IT" : item.Zone_Type.ToUpper().Trim();

                                        //tempZone = item.Zone_Type.ToUpper().Trim() != "IT CENTER" && item.Zone_Type.ToUpper().Trim() != "IT PARK" && item.Zone_Type.ToUpper().Trim() != "MANUFACTURING CEZ" ?
                                        //    "OTHERS" : item.Zone_Type.ToUpper().Trim();

                                        //tempZone = item.Zone_Type.ToUpper().Trim() == "MANUFACTURING CEZ" ?
                                        //    "MANUFACTURING" : item.Zone_Type.ToUpper().Trim();
                                    }

                                    if (tempZone == zoneTypeFromContext.ToUpper().Trim())
                                    {
                                        try
                                        {
                                            var zoneCode = db.Company.Where(m => m.CompanyCode == item.Dev_Comp_Code).FirstOrDefault().ZoneCode ?? "";
                                            if (zoneCode != null || zoneCode != "")
                                            {
                                                groupId = db.Zone.Where(m => m.ZoneCode == zoneCode).FirstOrDefault().ZoneGroup ?? "";
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            IOHelper.LogTxt(item.Dev_Comp_Code, billPeriodFromContext, zoneTypeFromContext);
                                            //throw new Exception("Zone code is has no match in current record " + item.Dev_Comp_Code);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Missmatch zone type " + tempZone);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }

                                if (groupId == zoneid.ToString())
                                {
                                    adminfees.Add(item);
                                }
                            }

                            db.AdminFee.AddRange(adminfees);
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            HttpContext.Current.Response.ContentType = "text/plain";
                            HttpContext.Current.Response.Write(ex.Message);
                            throw new Exception(ex.Message);
                        }
                    }
                }
                //if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                //    {
                //        string filename = file.FileName;
                //        string filecontenttype = file.ContentType;

                //        byte[] filebytes = new byte[file.ContentLength];
                //        var data = file.InputStream.Read(filebytes, 0, Convert.ToInt32(file.ContentLength));
                //        //int billingPeriodId = Convert.ToInt32(frm["BillingPeriod"].ToString());

                //        List<AdminFee> adminFee = new List<AdminFee>();
                //        using (var transaction = db.Database.BeginTransaction())
                //        {
                //            db.AdminFee.RemoveRange(db.AdminFee.Where(m => m.BillingPeriodId == billPeriod).ToList());
                //            db.SaveChanges();
                //            try
                //            {
                //                using (var package = new ExcelPackage(file.InputStream))
                //                {
                //                    List<String> notRegisteredCompany = new List<string>();
                //                    var currentSheet = package.Workbook.Worksheets;
                //                    var workSheet = currentSheet.First();
                //                    var noOfCol = workSheet.Dimension.End.Column;
                //                    var noOfRow = workSheet.Dimension.End.Row;

                //                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                //                    {
                //                        AdminFee adminfee = new AdminFee();
                //                        adminfee.Ecozone = workSheet.Cells[rowIterator, 1].Value != null ? workSheet.Cells[rowIterator, 1].Value.ToString() : "";
                //                        adminfee.Zone_Type = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : "";
                //                        adminfee.Company_Name = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString() : "";
                //                        adminfee.Enterprise_Type = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString() : "";
                //                        adminfee.Employment = workSheet.Cells[rowIterator, 5].Value != null ? workSheet.Cells[rowIterator, 5].Value.ToString() : "";
                //                        adminfee.Zone_Code = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "";
                //                        adminfee.Month = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString() : "";
                //                        adminfee.Year = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString() : "";
                //                        adminfee.Comp_Code = workSheet.Cells[rowIterator, 9].Value != null ? workSheet.Cells[rowIterator, 9].Value.ToString() : "";
                //                        adminfee.Developer = workSheet.Cells[rowIterator, 10].Value != null ? workSheet.Cells[rowIterator, 10].Value.ToString() : "";
                //                        adminfee.Dev_Comp_Code = workSheet.Cells[rowIterator, 11].Value != null ? workSheet.Cells[rowIterator, 11].Value.ToString() : "";
                //                        adminfee.Total_Locators = workSheet.Cells[rowIterator, 12].Value != null ? workSheet.Cells[rowIterator, 12].Value.ToString() : "";
                //                        adminfee.Total_Employment = workSheet.Cells[rowIterator, 13].Value != null ? workSheet.Cells[rowIterator, 13].Value.ToString() : "";
                //                        adminfee.BillingPeriodId = billPeriod;
                //                        if (!db.Company.Any(m => m.CompanyCode == adminfee.Comp_Code))
                //                            notRegisteredCompany.Add(adminfee.Company_Name);

                //                        db.AdminFee.Add(adminfee);
                //                        db.SaveChanges();
                //                    }
                //                    //TempData["notRegisteredCompany"] = notRegisteredCompany;
                //                    //TempData["TransactionSuccess"] = "Complete";
                //                    transaction.Commit();
                //                }
                //            }
                //            catch (Exception)
                //            {
                //                transaction.Rollback();
                //                //TempData["TransactionSuccess"] = "Failed";
                //            }
                //        }
                //    }
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