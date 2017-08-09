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

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class DataEntryPoleRentalController : Controller
    {
        private BCS_Context db = new BCS_Context();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        public ActionResult ViewPoleRentalsRPG()
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Pole;
            ViewBag.CompanySelected = "OK";
            SearchPoleInformationViewModel temp = TempData["SearchPoleRentalViewModels"] as SearchPoleInformationViewModel;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("SearchPoleRentalViewModels");
            return View("ViewPoleRentals", temp);
        }

        // GET: DataEntryPoleRental
        [HttpGet]
        public ActionResult ViewPoleRentals()
        {
            SearchPoleInformationViewModel searchcompany = new SearchPoleInformationViewModel();
            return View(searchcompany);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewPoleRentals(string SearchInput, FormCollection frm)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Pole;

            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                SearchPoleInformationViewModel searchcompany1 = new SearchPoleInformationViewModel();
                ApplicationDbContext context = new ApplicationDbContext();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;

                //searchcompany1.SearchInput = SearchInput.ToString();
                //searchcompany1.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                return View(searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                SearchPoleInformationViewModel searchcompany1 = new SearchPoleInformationViewModel();

                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    searchcompany1.PoleInformationList = db.PoleInformation.Where(m => m.CompanyId == ParsedCompanyID).ToList();
                    searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                    ViewBag.CompanySelected = "OK";
                }
                return View(searchcompany1);
            }
            //Default value
            else
            {
                SearchPoleInformationViewModel searchcompany1 = new SearchPoleInformationViewModel();
                return View(searchcompany1);
            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddPoleRental(FormCollection frm)
        {
            var origamt = frm["Amount"];
            var amt = frm["Amount"].Split(',');

            var newAmount = "";
            foreach (var item in amt)
            {
                newAmount += item;
            }           
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Pole;
            //SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Data Entry Pole Rental) - from Terminal:" + ipaddress);
            string isEdit = frm["isEdit"].ToString();
            SearchPoleInformationViewModel SearchPoleRentalViewModels = new SearchPoleInformationViewModel();

            PoleInformation PoleRentalAssignment = new PoleInformation();
            int CompanyId = int.Parse(frm["CompanyId"].ToString());
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            TryUpdateModel(PoleRentalAssignment);
            //if (ModelState.IsValid)
            //{
                if (isEdit != "True")
                {
                    PoleRentalAssignment.Amount = Convert.ToDecimal(origamt);
                    PoleRentalAssignment.CreatedDate = DateTime.Now;
                    PoleRentalAssignment.CreatedBy = userid;
                    PoleRentalAssignment.BillMode = frm["BillMode"];
                    PoleRentalAssignment.BillingMonths = frm["billingMonths"];
                    //PoleRentalAssignment.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
                    db.PoleInformation.Add(PoleRentalAssignment);
                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Add";
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Pole Rental- Add  - from Terminal:" + ipaddress);
            }
                else
                {
                    int ParsedIntID = int.Parse(frm["ID"]);
                    PoleRentalAssignment = db.PoleInformation.Find(ParsedIntID);
                    //compinfo.CompanyCode = frmcollection["CompanyCode"].ToString();
                    PoleRentalAssignment.Amount = Convert.ToDecimal(newAmount);
                    PoleRentalAssignment.StartDate = Convert.ToDateTime(frm["StartDate"].ToString());
                    PoleRentalAssignment.EndDate = Convert.ToDateTime(frm["EndDate"].ToString());
                    PoleRentalAssignment.UpdateDate = DateTime.Now;
                    PoleRentalAssignment.UpdatedBy = userid;
                    PoleRentalAssignment.BillingMonths = frm["billingMonths"];
                    //PoleRentalAssignment.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
                    PoleRentalAssignment.BillMode = frm["BillMode"];
                    db.Entry(PoleRentalAssignment).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Pole Rental- Edit  - from Terminal:" + ipaddress);
                //ViewBag.TransactionSuccess = "Add";
            }
                ViewBag.ValidatePostback = "True";
            //}
            //else
            //{
            //    ViewBag.ValidatePostback = "False";
            //}
            
            ViewBag.CompanySelected = "OK";
            SearchPoleRentalViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
            SearchPoleRentalViewModels.PoleInformationList = db.PoleInformation.Where(m => m.CompanyId == CompanyId).ToList();

            TempData["SearchPoleRentalViewModels"] = SearchPoleRentalViewModels;
            return RedirectToAction("ViewPoleRentalsRPG", "DataEntryPoleRental");
            //return View("ViewPoleRentals", SearchPoleRentalViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisplayPoleRentalList(int CompanyId, FormCollection frmcollection)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Pole;

            ViewBag.CompanySelected = "OK";
            if (frmcollection.Count == 0)
            {
                SearchPoleInformationViewModel SearchCompanyViewModels = new SearchPoleInformationViewModel();
                SearchCompanyViewModels.PoleInformationList = db.PoleInformation.SqlQuery("Select * from PoleInformations where CompanyId = '" + frmcollection["CompanyId"] + "'").ToList();
                return View(SearchCompanyViewModels);
            }
            else if (frmcollection.Count >= 1)
            {
                int parsedID = int.Parse(frmcollection["PoleInformationId"]);
                SearchPoleInformationViewModel SearchPoleRentalViewModels = new SearchPoleInformationViewModel();
                PoleInformation PoleRentalAssignment = new PoleInformation();
                PoleInformation pole = db.PoleInformation.Find(parsedID);
                db.PoleInformation.Remove(pole);
                db.SaveChanges();
                SearchPoleRentalViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
                SearchPoleRentalViewModels.PoleInformationList = db.PoleInformation.Where(m => m.CompanyId == CompanyId).ToList();
                return View("ViewPoleRentals", SearchPoleRentalViewModels);
            }
            return View();
        }

        // Generate Report
        public ActionResult PoleRentalReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Pole Rental- View Report  - from Terminal:" + ipaddress);
            //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fPoleRentalAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdatePole(int CompanyId, FormCollection frmcollection)
        //{
        //    var userid = User.Identity.GetUserId();
        //    var username = User.Identity.GetUserName();
        //    RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
        //    ViewBag.IsValidRole = roleAssignmentMatrix.Pole;

        //    ViewBag.CompanySelected = "OK";
        //    PoleInformation poleinfo = new PoleInformation();
        //    int ParsedIntID = int.Parse(frmcollection["PoleInformationId"]);
        //    TryUpdateModel(poleinfo);
        //    if (ModelState.IsValid)
        //    {
        //        if (poleinfo != null)
        //        {
        //            poleinfo = db.PoleInformation.Find(ParsedIntID);
        //            //compinfo.CompanyCode = frmcollection["CompanyCode"].ToString();
        //            poleinfo.Amount = Convert.ToDecimal(frmcollection["Amount"].ToString());
        //            //poleinfo.BillingMonths = frmcollection["BillMode"].ToString();
        //            poleinfo.StartDate = Convert.ToDateTime(frmcollection["StartDate"].ToString());
        //            poleinfo.EndDate = Convert.ToDateTime(frmcollection["EndDate"].ToString());
        //            poleinfo.UpdateDate = DateTime.Now;
        //            poleinfo.UpdatedBy = userid;
        //            poleinfo.BillingMonths = frmcollection["BillingMonths"];
        //            poleinfo.BillMode = frmcollection["BillMode"];
        //            db.Entry(poleinfo).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            TempData["TransactionSuccess"] = "Edit";
        //            //ViewBag.TransactionSuccess = "Edit";
        //        }
        //        ViewBag.ValidatePostback = "True";
        //    }
        //    else
        //    {
        //        ViewBag.ValidatePostback = "False";
        //    }

        //    int parsedID = int.Parse(frmcollection["PoleInformationId"]);
        //    SearchPoleInformationViewModel SearchPoleRentalViewModels = new SearchPoleInformationViewModel();
        //    //searchpole.PoleInformationList = db.PoleInformation.SqlQuery("Select * from PoleInformations where CompanyId = '" + frmcollection["CompanyId"] + "'").ToList();
        //    SearchPoleRentalViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
        //    SearchPoleRentalViewModels.PoleInformationList = db.PoleInformation.Where(m => m.CompanyId == CompanyId).ToList();

        //    TempData["SearchPoleRentalViewModels"] = SearchPoleRentalViewModels;
        //    return RedirectToAction("ViewPoleRentalsRPG", "DataEntryPoleRental");
        //    //return View("ViewPoleRentals", SearchPoleRentalViewModels);
        //}
    }
}