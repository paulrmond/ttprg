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
    public class DataEntryFranchiseController : Controller
    {
        private BCS_Context db = new BCS_Context();
    
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        public ActionResult ViewFranchiseRPG()
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Franchise;
            ViewBag.CompanySelected = "OK";
            SearchFranchiseViewModel temp = TempData["SearchFranchiseViewModels"] as SearchFranchiseViewModel;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("SearchFranchiseViewModels");
        
            return View("ViewFranchise", temp);

        }

        // GET: DataEntryFranchise
        public ActionResult ViewFranchise(string CompanyName)
        {
            SearchFranchiseViewModel SearchFranchiseViewModels = new SearchFranchiseViewModel();
            if (!string.IsNullOrEmpty(CompanyName))
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                SearchFranchiseViewModels.Companies = searchCompanyPerGroup.Companies;
                //SearchFranchiseViewModels.Companies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Franchise - Search Company  - from Terminal: " + ipaddress);
            return View(SearchFranchiseViewModels);
        }

        public ActionResult LoadFranchiseInformation(int? CompanyId)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Franchise;

            SearchFranchiseViewModel SearchFranchiseViewModels = new SearchFranchiseViewModel();
            if (CompanyId != null)
            {
                SearchFranchiseViewModels.Companies = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
                SearchFranchiseViewModels.FranchiseFeeInformations = db.FranchiseFeeInformation.Where(m => m.CompanyId == CompanyId).ToList();
                ViewBag.CompanySelected = "OK";
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Franchise - Search Franchise Information  - from Terminal: " + ipaddress);
            return View("ViewFranchise", SearchFranchiseViewModels);
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddFranchiseInformation(FormCollection formcollection)
        {
            string isEdit = formcollection["isEdit"].ToString();
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Franchise;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Data Entry Franchise Information) - from Terminal:" + ipaddress);
            int CompanyId = Convert.ToInt32(formcollection["CompanyId"].ToString());

            //FranchiseFeeInformation fran = new FranchiseFeeInformation();
            //TryUpdateModel(fran);
            //if (ModelState.IsValid)
            //{
            DateTime DateStart = Convert.ToDateTime(formcollection["StartDate"].ToString());
            DateTime EndDate = Convert.ToDateTime(formcollection["EndDate"].ToString());
            string BillMode = formcollection["BillMode"];
            int DueOn = Convert.ToInt32(formcollection["DueOn"].ToString());
            decimal Amount = Convert.ToDecimal(formcollection["Amount"].ToString());

            if (isEdit != "True")
            {
                FranchiseFeeInformation franchiseFeeInformation = new FranchiseFeeInformation();
                franchiseFeeInformation.CompanyId = CompanyId;
                franchiseFeeInformation.Amount = Amount;
                franchiseFeeInformation.BillMode = BillMode;
                franchiseFeeInformation.DueOn = DueOn;
                franchiseFeeInformation.StartDate = DateStart;
                franchiseFeeInformation.EndDate = EndDate;
                franchiseFeeInformation.BillingMonths = formcollection["billingMonths"];
                //franchiseFeeInformation.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
                franchiseFeeInformation.CreatedBy = userid;
                franchiseFeeInformation.CreateDate = DateTime.Now;

                db.FranchiseFeeInformation.Add(franchiseFeeInformation);
                db.SaveChanges();
                TempData["TransactionSuccess"] = "Add";
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Franchise - Franchise Information Added  - from Terminal: " + ipaddress);
                ViewBag.ValidatePostback = "True";
            }
            else
            {
                int FranchiseFeeInformationId = Convert.ToInt32(formcollection["FranchiseFeeInformationId"].ToString());
                FranchiseFeeInformation franchiseFeeInformation = new FranchiseFeeInformation();
                franchiseFeeInformation = db.FranchiseFeeInformation.Where(m => m.FranchiseFeeInformationId == FranchiseFeeInformationId).FirstOrDefault();

                franchiseFeeInformation.Amount = Amount;
                franchiseFeeInformation.BillMode = BillMode;
                franchiseFeeInformation.DueOn = DueOn;
                franchiseFeeInformation.StartDate = DateStart;
                franchiseFeeInformation.EndDate = EndDate;
                franchiseFeeInformation.BillingMonths = formcollection["billingMonths"];
                //franchiseFeeInformation.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
                franchiseFeeInformation.UpdatedBy = userid;
                franchiseFeeInformation.UpdateDate = DateTime.Now;

                db.Entry(franchiseFeeInformation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["TransactionSuccess"] = "Edit";
                ViewBag.ValidatePostback = "True";
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Franchise - Franchise Information Edit  - from Terminal: " + ipaddress);
            }
            //}
            //else
            //{
            //    ViewBag.ValidatePostback = "False";
            //}
            ViewBag.CompanySelected = "OK";
            SearchFranchiseViewModel searchFranchiseViewModel = new SearchFranchiseViewModel();
            searchFranchiseViewModel.Companies = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
            searchFranchiseViewModel.FranchiseFeeInformations = db.FranchiseFeeInformation.Where(m => m.CompanyId == CompanyId).ToList();

            TempData["SearchFranchiseViewModels"] = searchFranchiseViewModel;

            

            return RedirectToAction("ViewFranchiseRPG", "DataEntryFranchise");
            //return View("ViewFranchise", searchFranchiseViewModel);
        }

        //[ValidateAntiForgeryToken]
        //public ActionResult EditFranchise(FormCollection formcollection)
        //{
        //    var userid = User.Identity.GetUserId();
        //    var username = User.Identity.GetUserName();
        //    RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
        //    ViewBag.IsValidRole = roleAssignmentMatrix.Franchise;

        //    int CompanyId = Convert.ToInt32(formcollection["CompanyId"].ToString());

        //    FranchiseFeeInformation fran = new FranchiseFeeInformation();
        //    TryUpdateModel(fran);
        //    if (ModelState.IsValid)
        //    {
        //        int FranchiseFeeInformationId = Convert.ToInt32(formcollection["FranchiseFeeInformationId"].ToString());
        //        DateTime DateStart = Convert.ToDateTime(formcollection["StartDate"].ToString());
        //        DateTime EndDate = Convert.ToDateTime(formcollection["EndDate"].ToString());
        //        string BillMode = formcollection["BillMode"];
        //        int DueOn = Convert.ToInt32(formcollection["DueOn"].ToString());
        //        decimal Amount = Convert.ToDecimal(formcollection["Amount"].ToString());

        //        FranchiseFeeInformation franchiseFeeInformation = new FranchiseFeeInformation();
        //        franchiseFeeInformation = db.FranchiseFeeInformation.Where(m => m.FranchiseFeeInformationId == FranchiseFeeInformationId).Single();

        //        franchiseFeeInformation.Amount = Amount;
        //        franchiseFeeInformation.BillMode = BillMode;
        //        franchiseFeeInformation.DueOn = DueOn;
        //        franchiseFeeInformation.StartDate = DateStart;
        //        franchiseFeeInformation.EndDate = EndDate;
        //        //franchiseFeeInformation.BillingMonths = formcollection["BillingMonths"];
        //        franchiseFeeInformation.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
        //        franchiseFeeInformation.UpdatedBy = userid;
        //        franchiseFeeInformation.UpdateDate = DateTime.Now;

        //        db.Entry(franchiseFeeInformation).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["TransactionSuccess"] = "Edit";
        //        //ViewBag.TransactionSuccess = "Edit";
        //        ViewBag.ValidatePostback = "True";
        //    }
        //    else
        //    {
        //        ViewBag.ValidatePostback = "False";
        //    }

        //    ViewBag.CompanySelected = "OK";
        //    SearchFranchiseViewModel SearchFranchiseViewModels = new SearchFranchiseViewModel();
        //    SearchFranchiseViewModels.Companies = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
        //    SearchFranchiseViewModels.FranchiseFeeInformations = db.FranchiseFeeInformation.Where(m => m.CompanyId == CompanyId).ToList();

        //    TempData["SearchFranchiseViewModels"] = SearchFranchiseViewModels;
        //    return RedirectToAction("ViewFranchiseRPG", "DataEntryFranchise");
        //    //return View("ViewFranchise", SearchFranchiseViewModels);
        //}

        // Generate Report
        public ActionResult FranchiseReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Franchise - View Report  - from Terminal: " + ipaddress);
            // return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);
            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fFranchiseAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }
    }
}