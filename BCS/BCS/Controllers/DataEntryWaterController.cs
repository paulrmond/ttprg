using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class DataEntryWaterController : Controller
    {
        private BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        public List<BillingPeriod> BillingPeriods()
        {
            List<BillingPeriod> billP = new List<BillingPeriod>();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            IEnumerable<BillingPeriod> billingPeriod = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList();
            billP = billingPeriod.OrderByDescending(m => m.BillingPeriodId).ToList();
            return billP;
        }

        //private ApplicationUserManager appmanager;
        public ActionResult ViewWaterPRG()
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Water;
            ViewBag.CompanySelected = "OK";

            ViewBag.ValidatePostback = "True"; //This line of code use for serverside validation.
            SearchWaterViewModel temp = TempData["WaterAssignmentData"] as SearchWaterViewModel;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("WaterAssignmentData");
            return View("ViewWater", temp);
        }
        public ActionResult ViewWaterReadingPRG()
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Water;
            ViewBag.CompanySelected = "OK";
            ViewBag.ValidatePostback = "True"; //This line of code use for serverside validation.
            SearchWaterViewModel temp = TempData["WaterReadingData"] as SearchWaterViewModel;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("WaterReadingData");

            ViewBag.SewerageRate = TempData["SewerageRate"] as decimal?;
            TempData.Keep("SewerageRate");
            return View("ViewWaterReading", temp);
        }
        public ActionResult ViewWater(string ReturnedId)
        {
            SearchWaterViewModel SearchWaterViewModels = new SearchWaterViewModel();
            if (!string.IsNullOrEmpty(ReturnedId))
            {
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + ReturnedId + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                SearchWaterViewModels.BillingPeriods = BillingPeriods();
                SearchWaterViewModels.Companies = searchCompanyPerGroup.Companies;

                //SearchWaterViewModels.Companies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + ReturnedId + "%'").ToList();
            }
            SearchWaterViewModels.BillingPeriods = BillingPeriods();
            return View(SearchWaterViewModels);
        }
        public ActionResult ViewWaterAssignment(int? CompanyID)
        {
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Water;

            SearchWaterViewModel SearchWaterViewModels = new SearchWaterViewModel();
            if (CompanyID != null)
            {
                SearchWaterViewModels.Companies = db.Company.Where(m => m.CompanyID == CompanyID).ToList();
                SearchWaterViewModels.BillingPeriods = BillingPeriods();
                SearchWaterViewModels.WaterMeterAssignments = db.WaterMeterAssignment.Where(m => m.CompanyId == CompanyID).ToList();
                ViewBag.CompanySelected = "OK";
            }
            SearchWaterViewModels.BillingPeriods = BillingPeriods();
            return View("ViewWater", SearchWaterViewModels);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ViewWaterReading(int? CompanyID, string MeterNum)
        {
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Water;

            SearchWaterViewModel SearchWaterViewModels = new SearchWaterViewModel();
            if (CompanyID != null)
            {
                SearchWaterViewModels.Companies = db.Company.Where(m => m.CompanyID == CompanyID).ToList();
                SearchWaterViewModels.WaterMeterAssignments = db.WaterMeterAssignment.Where(m => m.MeterNumber == MeterNum).ToList();
                SearchWaterViewModels.WaterMeterReadings = db.WaterMeterReading.Where(m => m.MeterNumber == MeterNum).OrderByDescending(m => m.PresentReading).ToList();
                SearchWaterViewModels.BillingPeriods = BillingPeriods();
            }

            SearchWaterViewModels.BillingPeriods = BillingPeriods();
            decimal? sewerageRate = db.BillingRates.FirstOrDefault(m => m.ZoneGroup == ZoneGroup && m.Category.ToUpper() == "SEWERAGE").Rate;
            ViewBag.SewerageRate = sewerageRate;
            return View(SearchWaterViewModels);
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddMeter(FormCollection frm)
        {
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Water;

            string isEdit = frm["isEdit"].ToString();
            int CompanyID = int.Parse(frm["CompanyID"].ToString());
            SearchWaterViewModel SearchWaterViewModels = new SearchWaterViewModel();

            WaterMeterAssignment waterMeterAssignment = new WaterMeterAssignment();
            TryUpdateModel(waterMeterAssignment);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(isEdit) || isEdit != "True")
                {
                    string MeterNumber = frm["MeterNumber"].ToString();
                    DateTime StartDate = Convert.ToDateTime(frm["StartDate"].ToString());
                    DateTime EndDate = Convert.ToDateTime(frm["EndDate"].ToString());
                    string Size = frm["Size"].ToString();
                    string Phase = frm["Phase"].ToString();
                    int IncludeInBilling = frm["IncludeInBilling"].ToString() == "YES" ? 1 : 0;

                    DateTime MeterNumberEndDate = new DateTime();
                    WaterMeterAssignment tempMeterAssignment = new WaterMeterAssignment();
                    tempMeterAssignment = db.WaterMeterAssignment.Where(m => m.MeterNumber == MeterNumber).FirstOrDefault();
                    if (tempMeterAssignment != null)
                    {
                        MeterNumberEndDate = tempMeterAssignment.EndDate;
                    }

                    if (MeterNumberEndDate < StartDate)
                    {
                        waterMeterAssignment.CompanyId = CompanyID;
                        waterMeterAssignment.MeterNumber = MeterNumber;
                        waterMeterAssignment.StartDate = Convert.ToDateTime(StartDate);
                        waterMeterAssignment.EndDate = Convert.ToDateTime(EndDate);
                        waterMeterAssignment.Size = Size;
                        waterMeterAssignment.Phase = Phase;
                        waterMeterAssignment.Createdby = userid;
                        waterMeterAssignment.CreateDate = DateTime.Now;
                        waterMeterAssignment.IncludeBilling = IncludeInBilling;

                        db.WaterMeterAssignment.Add(waterMeterAssignment);
                        db.SaveChanges();
                        //ViewBag.TransactionSuccess = "Add";
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - Add Meter  - from Terminal: " + ipaddress);
                        TempData["TransactionSuccess"] = "Add";
                        ViewBag.ValidatePostback = "True";
                    }
                    else
                    {
                        TempData["TransactionSuccess"] = "MeterNumberExist";
                        ViewBag.ValidatePostback = "False";
                    }
                }
                else
                {
                    int waterAssignmentId = int.Parse(frm["wid"].ToString());
                    WaterMeterAssignment waterMeterAssignmentEdit = new WaterMeterAssignment();
                    string MeterNumber = frm["MeterNumber"].ToString();
                    DateTime StartDate = Convert.ToDateTime(frm["StartDate"].ToString());
                    DateTime EndDate = Convert.ToDateTime(frm["EndDate"].ToString());
                    string Size = frm["Size"].ToString();
                    string Phase = frm["Phase"].ToString();
                    int IncludeInBilling = frm["IncludeInBilling"].ToString() == "YES" ? 1 : 0;
                    waterMeterAssignmentEdit = db.WaterMeterAssignment.Where(m => m.WaterMeterAssignmentId == waterAssignmentId).Single();

                    waterMeterAssignmentEdit.MeterNumber = MeterNumber;
                    waterMeterAssignmentEdit.StartDate = Convert.ToDateTime(StartDate);
                    waterMeterAssignmentEdit.EndDate = Convert.ToDateTime(EndDate);
                    waterMeterAssignmentEdit.Size = Size;
                    waterMeterAssignmentEdit.Phase = Phase;

                    waterMeterAssignmentEdit.UpdatedBy = userid;
                    waterMeterAssignmentEdit.UpdateDate = DateTime.Now;
                    waterMeterAssignmentEdit.IncludeBilling = IncludeInBilling;

                    db.Entry(waterMeterAssignmentEdit).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.ValidatePostback = "True";
                    //ViewBag.TransactionSuccess = "Edit";
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - Edit Meter  - from Terminal: " + ipaddress);
                    TempData["TransactionSuccess"] = "Edit";
                }
            }
            else
            {
                ViewBag.ValidatePostback = "False";
            }

            ViewBag.CompanySelected = "OK";
            SearchWaterViewModels.Companies = db.Company.Where(m => m.CompanyID == CompanyID).ToList();
            SearchWaterViewModels.WaterMeterAssignments = db.WaterMeterAssignment.Where(m => m.CompanyId == CompanyID).ToList();

            SearchWaterViewModels.BillingPeriods = BillingPeriods();

            TempData["WaterAssignmentData"] = SearchWaterViewModels;

            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - Meter Added  - from Terminal: " + ipaddress);

            return RedirectToAction("ViewWaterPRG", "DataEntryWater");
            //return View("ViewWater", SearchWaterViewModels);
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddMeterReading(FormCollection frm, string PreviousReading)
        {
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Water;

            SearchWaterViewModel SearchWaterViewModels = new SearchWaterViewModel();
            WaterMeterReading waterMeterReading = new WaterMeterReading();

            TryUpdateModel(waterMeterReading);
            int CompanyID = int.Parse(frm["CompanyID"].ToString());
            string MeterNumber = frm["MeterNumber"].ToString();

            waterMeterReading.CompanyId = CompanyID;
            waterMeterReading.MeterNumber = MeterNumber;
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                int BillingPeriod = int.Parse(frm["BillingPeriod"].ToString());
                decimal PreviousReading1 = decimal.Parse(frm["PreviousReading"].ToString());
                decimal PresentReading = decimal.Parse(frm["PresentReading"].ToString());
                string remarks = frm["remarks"].ToString();
                string isEdit = frm["isEdit"].ToString();
                bool threeMonthsAvg = false;
                if(frm["MonthAverage"].ToString() != "")
                {
                    threeMonthsAvg = true;
                    decimal avg = decimal.Parse(frm["MonthAverage"].ToString());
                    PresentReading += avg;
                }
                if (isEdit.ToLower() == "add")
                {
                    var recordExist = db.WaterMeterReading.Any(m => m.BillingPeriod == BillingPeriod && m.MeterNumber == MeterNumber);
                    if (!recordExist)
                    {
                        //waterMeterReading.BillingPeriod = BillingPeriod;//ok
                        //waterMeterReading.PresentReading = PresentReading;//ok
                        //waterMeterReading.PreviousReading = PreviousReading1;//OK
                        //waterMeterReading.CreatedBy = userid;//ok
                        //waterMeterReading.CreateDate = DateTime.Now;//ok
                        //waterMeterReading.remarks = remarks;

                        db.Database.ExecuteSqlCommand("insert into WaterMeterReadings(CompanyId,BillingPeriod,MeterNumber,PreviousReading,PresentReading,CreateDate,CreatedBy,remarks,UseThreeMonthsAverage) values('" + CompanyID + "','" + BillingPeriod + "','" + MeterNumber + "','" + PreviousReading1 + "','" + PresentReading + "','" + DateTime.Now + "','" + userid + "','" + remarks + "','"+ threeMonthsAvg + "')");
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - Add meter reading  - from Terminal: " + ipaddress);
                        TempData["TransactionSuccess"] = "Add";
                        //db.WaterMeterReading.Add(waterMeterReading);
                        //db.SaveChanges();


                    }
                    else
                    {
                        TempData["TransactionSuccess"] = "WaterReadingFailed";
                    }
                }
                else if (isEdit.ToLower() == "edit")
                {
                    int waterMeterId = int.Parse(frm["meterid"].ToString());
                    //WaterMeterReading waterreading = new WaterMeterReading();
                    //waterreading = db.WaterMeterReading.Find(waterMeterId);
                    //waterreading.PresentReading = PresentReading;
                    //waterreading.PreviousReading = PreviousReading1;
                    //waterreading.remarks = remarks;

                    db.Database.ExecuteSqlCommand("Update WaterMeterReadings set UpdatedBy='" + userid + "',BillingPeriod = '" + BillingPeriod + "',UpdateDate='" + DateTime.Now + "', PresentReading = '" + PresentReading + "',PreviousReading = '" + PreviousReading1 + "',remarks='" + remarks + "',UseThreeMonthsAverage='"+ threeMonthsAvg + "' where WaterMeterReadingId='" + waterMeterId + "'");
                    //Use execute sql command coz ef use only 2 decimal places
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - Edit meter reading  - from Terminal: " + ipaddress);
                    TempData["TransactionSuccess"] = "Edit";
                    //db.Entry(waterreading).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                }

                ViewBag.ValidatePostback = "True";
            }
            else
            {
                ViewBag.ValidatePostback = "False";
            }


            ViewBag.CompanySelected = "OK";
            SearchWaterViewModels.BillingPeriods = BillingPeriods();
            SearchWaterViewModels.Companies = db.Company.Where(m => m.CompanyID == CompanyID).ToList();
            SearchWaterViewModels.WaterMeterAssignments = db.WaterMeterAssignment.Where(m => m.MeterNumber == MeterNumber).ToList();
            SearchWaterViewModels.WaterMeterReadings = db.WaterMeterReading.Where(m => m.MeterNumber == MeterNumber).OrderByDescending(x=>x.PresentReading).ToList();
            SearchWaterViewModels.BillingPeriods = BillingPeriods();
            decimal? sewerageRate = db.BillingRates.FirstOrDefault(m => m.ZoneGroup == ZoneGroup && m.Category.ToUpper() == "SEWERAGE").Rate;

            TempData["SewerageRate"] = sewerageRate;
            TempData["WaterReadingData"] = SearchWaterViewModels;

            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - Meter Reading Added  - from Terminal: " + ipaddress);

            return RedirectToAction("ViewWaterReadingPRG", "DataEntryWater");
            //return View("ViewWaterReading", SearchWaterViewModels);
        }



        // Generate Report
        //public ActionResult WaterMeterAlphaListReport()
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
        //    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Water Meter Alphalist Report  - from Terminal: " + ipaddress);

        //    string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

        //    UriBuilder serverURI = new UriBuilder(serverURL);
        //    serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterMeterAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

        //    return Redirect(serverURI.Uri.ToString());
        //}

        //public ActionResult WaterReadingAlphaListReport()
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
        //    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Water Reading Alphalist Report  - from Terminal: " + ipaddress);

        //    string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

        //    UriBuilder serverURI = new UriBuilder(serverURL);
        //    serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterReadingAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

        //    return Redirect(serverURI.Uri.ToString());
        //}

        public JsonResult PeriodicConsumptionReport(string MeterNumber, int CompanyID)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int companyID = CompanyID;
            string meterNumber = MeterNumber;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Periodic Consumption Report  - from Terminal: " + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterReadingPeriodicConsumption&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&companyID=" + companyID + "&meterNumber=" + meterNumber;

            return Json(serverURI.Uri.ToString());
        }

        //public ActionResult WaterReadingReport(int billingPeriod)
        //{

        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
        //    var billingPeriodID = billingPeriod;

        //    NameValueCollection settings = new NameValueCollection();

        //    switch (zoneGroupCode)
        //    {
        //        case "01":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/WaterReading");
        //            break;

        //        case "03":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/WaterReading");
        //            break;

        //        case "06":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/WaterReading");
        //            break;

        //        case "09":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/WaterReading");
        //            break;

        //        default:
        //            break;
        //    }

        //    string accountOfficer = settings["accountOfficer"].ToString();
        //    string enterpriseRep = settings["enterpriseRep"].ToString();


        //    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Water Reading Report  - from Terminal: " + ipaddress);

        //    string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

        //    UriBuilder serverURI = new UriBuilder(serverURL);
        //    serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterReadingReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriodID=" + billingPeriodID + "&accountOfficer=" + accountOfficer + "&enterpriseRep=" + enterpriseRep;

        //    return Redirect(serverURI.Uri.ToString());
        //}

        public JsonResult WaterMeterAlphaListReport(int BillingPeriod)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int billingPeriod = BillingPeriod;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Water Meter Alphalist Report  - from Terminal: " + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterMeterAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod;

            return Json(serverURI.Uri.ToString());
        }

        public JsonResult WaterReadingAlphaListReport(int BillingPeriod)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int billingPeriod = BillingPeriod;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Water Reading Alphalist Report  - from Terminal: " + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterReadingAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod;

            return Json(serverURI.Uri.ToString());
        }

        public JsonResult WaterReadingReport(int BillingPeriod)
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var billingPeriod = BillingPeriod;

            NameValueCollection settings = new NameValueCollection();

            switch (zoneGroupCode)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/WaterReading");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/WaterReading");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/WaterReading");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/WaterReading");
                    break;

                default:
                    break;
            }

            string accountOfficer = settings["accountOfficer"].ToString();
            string enterpriseRep = settings["enterpriseRep"].ToString();


            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Water - View Water Reading Report  - from Terminal: " + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fWaterReadingReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&accountOfficer=" + accountOfficer + "&enterpriseRep=" + enterpriseRep;

            return Json(serverURI.Uri.ToString());
        }
    }
}