using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net;
using System.Configuration;
using System.Collections.Specialized;

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class DataEntryGarbageController : Controller
    {
        private BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        public ActionResult ViewGarbagePRG()
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Garbage;
            ViewBag.CompanySelected = "OK";
            SearchGarbageInformation temp = TempData["GarbageData"] as SearchGarbageInformation;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("GarbageData");
            return View("ViewGarbage", temp);
        }
        // GET: DataEntryGarbage
        public ActionResult ViewGarbage(int? CompanyId)
        {
            SearchGarbageInformation searchGarbageInformation = new SearchGarbageInformation();
            if (CompanyId != null)
            {
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                //searchGarbageInformation.BillingPeriods = db.BillingPeriod.Where(m => m.Finalized == "No").Where(m => m.groupCode == ZoneGroup).ToList();
                var username = User.Identity.GetUserName();
                RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
                ViewBag.IsValidRole = roleAssignmentMatrix.Garbage;

                searchGarbageInformation.Companies = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
                //searchGarbageInformation.BillingPeriods = db.BillingPeriod.Where(m => m.Finalized == "No").Where(m => m.groupCode == ZoneGroup).ToList();
                IEnumerable<BillingPeriod> IBillingPeriods = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList();
                searchGarbageInformation.BillingPeriods = IBillingPeriods.OrderByDescending(m => m.BillingPeriodId).ToList();
                searchGarbageInformation.BillingRates = db.BillingRates.Where(m => m.Category == "Garbage Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();
                searchGarbageInformation.GarbageInformations = db.GarbageInformations.Where(m => m.CompanyId == CompanyId).ToList();
                //var subCategory = searchGarbageInformation.BillingRates.GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();
                var subcat1 = searchGarbageInformation.BillingRates.GroupBy(m => m.SubCategory).ToList();

                foreach (var item in subcat1)
                {
                    searchGarbageInformation.SubCategory.Add(item.Key.ToString());
                }
                ViewBag.CompanySelected = "OK";

            }

            return View(searchGarbageInformation);
        }
        public ActionResult SearchEnterprise(string CompanyName)
        {
            SearchGarbageInformation searchGarbageInformation = new SearchGarbageInformation();
            if (!string.IsNullOrEmpty(CompanyName))
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchGarbageInformation.Companies = searchCompanyPerGroup.Companies;
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Garbage - Search Company - from Terminal:" + ipaddress);
                //searchGarbageInformation.Companies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
            }

            return View("ViewGarbage", searchGarbageInformation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGarbage(FormCollection frm)
        {
            string isEdit = frm["isEdit"].ToString();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Garbage;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Data Entry Garbage) - from Terminal:" + ipaddress);
            GarbageInformation garbageInformation = new GarbageInformation();
            int CompanyId = int.Parse(frm["CompanyId"].ToString());
            TryUpdateModel(garbageInformation);
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                DateTime CollectionDate = DateTime.Parse(frm["CollectionDate"].ToString());
                int BillingPeriod = int.Parse(frm["BillingPeriod"].ToString());
                string Type = frm["Type"].ToString();
                decimal Rate = decimal.Parse(frm["Rate"].ToString());
                decimal Weight = decimal.Parse(frm["Weight"].ToString());

                if (isEdit != "Edit")
                {
                    VerifyDuplicateEntries verifyDuplicateEntries = new VerifyDuplicateEntries(CompanyId, BillingPeriod, "GARBAGE");
                    if (!verifyDuplicateEntries.hasDuplicateByBillingPeriod())
                    {
                        //garbageInformation.CompanyId = CompanyId;
                        //garbageInformation.BillingPeriod = BillingPeriod;
                        //garbageInformation.Type = Type;
                        //garbageInformation.Weight = Weight;
                        //garbageInformation.Rate = Rate;
                        //garbageInformation.CreatedBy = userid;
                        //garbageInformation.CreateDate = DateTime.Now;
                        //garbageInformation.CollectionDate = CollectionDate;

                        //db.GarbageInformations.Add(garbageInformation);
                        //db.SaveChanges();

                        db.Database.ExecuteSqlCommand("Insert into GarbageInformations(CompanyId,BillingPeriod,Type,Weight,Rate,CreatedBy,CreateDate,CollectionDate) values('" + CompanyId + "','" + BillingPeriod + "','" + Type + "','" + Weight + "','" + Rate + "','" + userid + "','" + DateTime.Now + "','" + CollectionDate + "')");
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Garbage - Add  - from Terminal:" + ipaddress);
                        TempData["TransactionSuccess"] = "Add";
                    }
                    else
                    {
                        TempData["TransactionSuccess"] = "DuplicateBillingPeriod";
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Garbage Billing - Period already used  - from Terminal:" + ipaddress);
                    }
                        
                }
                else
                {
                    int GarbageInformationId = int.Parse(frm["GarbageInfoId"].ToString());
                    //garbageInformation = db.GarbageInformations.Where(m => m.GarbageInformationId == GarbageInformationId).Single();
                    //garbageInformation.BillingPeriod = BillingPeriod;
                    //garbageInformation.Type = Type;
                    //garbageInformation.Rate = Rate;
                    //garbageInformation.Weight = Weight;
                    //garbageInformation.CollectionDate = CollectionDate;
                    //garbageInformation.UpdatedBy = userid;
                    //garbageInformation.UpdateDate = DateTime.Now;

                    //db.Entry(garbageInformation).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();

                    db.Database.ExecuteSqlCommand("Update GarbageInformations set CompanyId = '" + CompanyId + "',BillingPeriod ='" + BillingPeriod + "',Type ='" + Type + "',Weight='" + Weight + "',Rate='" + Rate + "',UpdatedBy='" + userid + "',UpdateDate='" + DateTime.Now + "',CollectionDate='" + CollectionDate + "' where GarbageInformationId = '" + GarbageInformationId + "'");

                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Garbage - Edit - from Terminal:" + ipaddress);
                    TempData["TransactionSuccess"] = "Edit";
                }
            }

            SearchGarbageInformation searchGarbageInformation = new SearchGarbageInformation();
            searchGarbageInformation.Companies = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
            //searchGarbageInformation.BillingPeriods = db.BillingPeriod.Where(m => m.Finalized == "No").Where(m => m.groupCode == ZoneGroup).ToList();
            IEnumerable<BillingPeriod> IBillingPeriods = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList();
            searchGarbageInformation.BillingPeriods = IBillingPeriods.OrderByDescending(m => m.BillingPeriodId).ToList();
            searchGarbageInformation.BillingRates = db.BillingRates.Where(m => m.Category == "Garbage Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();
            searchGarbageInformation.GarbageInformations = db.GarbageInformations.Where(m => m.CompanyId == CompanyId).ToList();

            var subcat1 = searchGarbageInformation.BillingRates.GroupBy(m => m.SubCategory).ToList();

            foreach (var item in subcat1)
            {
                searchGarbageInformation.SubCategory.Add(item.Key.ToString());
            }
            //return View("ViewGarbage", searchGarbageInformation);
            TempData["GarbageData"] = searchGarbageInformation;
            return RedirectToAction("ViewGarbagePRG", "DataEntryGarbage");

        }

        // Generate Report
        public ActionResult GarbageCollectionReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var fname = context.Users.SingleOrDefault(m => m.Id == userid).GivenName;
            var mname = context.Users.SingleOrDefault(m => m.Id == userid).MiddleName;
            var lname = context.Users.SingleOrDefault(m => m.Id == userid).LastName;

            var signedUser = fname + " " + mname + " " + lname;

            string zoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            NameValueCollection settings = new NameValueCollection();

            switch (zoneGroup)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/GARBAGE");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/GARBAGE");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/GARBAGE");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/GARBAGE");
                    break;

                default:
                    break;
            }
            
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Garbage - View Report  - from Terminal:" + ipaddress);
            //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            string preparedBy = settings["preparedBy"].ToString();

            string prepPos = settings["prepPos"].ToString();
            string appPos = settings["appPos"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fGarbageCollectionAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroup + "&preparedBy=" + preparedBy + "&ReceivedBy=" + signedUser + "&preparedByPOS=" + prepPos + "&ReceivedByPOS=" + appPos;

            return Redirect(serverURI.Uri.ToString());
        }
    }
}