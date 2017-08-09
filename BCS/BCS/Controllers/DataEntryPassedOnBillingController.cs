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
    public class DataEntryPassedOnBillingController : Controller
    {
        private BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        //This Action Method implements the PRG pattern...
        [HttpGet]
        public ActionResult ViewPassedOnBillingPRG()
        {
            PassedOnBillingViewModel PassedList = new PassedOnBillingViewModel();
            PassedList.ZoneList = db.Zone.Select(x => x).ToList();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.PassedOnBilling;
            ViewBag.CompanySelected = "OK";
            PassedOnBillingViewModel temp = TempData["PassedOnBillingData"] as PassedOnBillingViewModel;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("PassedOnBillingData");
            return View("ViewPassedOnBilling", temp);
        }

        [HttpGet]
        public ActionResult ViewPassedOnBilling()
        {
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            PassedOnBillingViewModel searchcompany = new PassedOnBillingViewModel();
            searchcompany.ZoneList = db.Zone.Select(x => x).ToList();
            List<BillingPeriod> billingPeriods = new List<BillingPeriod>();
            billingPeriods = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList();
            var billp = billingPeriods.OrderByDescending(m => m.BillingPeriodId);
            billingPeriods = billp.ToList();
            searchcompany.BillingPeriodList = billingPeriods;
            return View(searchcompany);
        }

        // GET: Searched Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewPassedOnBilling(string SearchInput, FormCollection frm)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.PassedOnBilling;

            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                PassedOnBillingViewModel searchcompany1 = new PassedOnBillingViewModel();
                searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
                ApplicationDbContext context = new ApplicationDbContext();

                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                //searchcompany1.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).Where(m => m.Finalized.ToUpper() != "YES").ToList();
                searchcompany1.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
                var billp = searchcompany1.BillingPeriodList.OrderByDescending(m => m.BillingPeriodId);
                searchcompany1.BillingPeriodList = billp.ToList();
                
                return View(searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                PassedOnBillingViewModel searchcompany1 = new PassedOnBillingViewModel();
                searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    searchcompany1.PassedOnBillingList = db.PassedOnBillingInformation.Where(m => m.CompanyId == ParsedCompanyID).ToList();
                    searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                    if (searchcompany1.PassedOnBillingList.Count >= 1)
                    {
                        searchcompany1.TotalAmount = searchcompany1.PassedOnBillingList.Sum(x => x.Amount);
                    }
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Passed OnBilling - Search Company -from Terminal:" + ipaddress);
                    ViewBag.CompanySelected = "OK";
                }
                //searchcompany1.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).Where(m => m.Finalized.ToUpper() != "YES").ToList();
                searchcompany1.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
                var billp = searchcompany1.BillingPeriodList.OrderByDescending(m => m.BillingPeriodId);
                searchcompany1.BillingPeriodList = billp.ToList();
                return View(searchcompany1);
            }
            //Default value
            else
            {
                PassedOnBillingViewModel searchcompany1 = new PassedOnBillingViewModel();
                searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
                //searchcompany1.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).Where(m => m.Finalized.ToUpper() != "YES").ToList();
                searchcompany1.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
                var billp = searchcompany1.BillingPeriodList.OrderByDescending(m => m.BillingPeriodId);
                searchcompany1.BillingPeriodList = billp.ToList();
                return View(searchcompany1);
            }
        }

        // Date Entry for Security Guard Entry
        [ValidateAntiForgeryToken]
        public ActionResult AddPassedOnBilling(int CompanyId, Decimal Amount, DateTime OriginDateAdd, DateTime BillingDateAdd, int BillingPeriod, string Type, string isEdit, string PassedOnBillingInfoId)
        {
            PassedOnBillingViewModel searchcompany = new PassedOnBillingViewModel();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.PassedOnBilling;
            //SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Data Entry PassOnBilling) - from Terminal:" + ipaddress);
            PassedOnBillingInformation passedonBillingInfo = null;
            passedonBillingInfo = db.PassedOnBillingInformation.Find(CompanyId);

            if (isEdit != "Edit")
            {
                PassedOnBillingViewModel passedonBillingViewModels = new PassedOnBillingViewModel();
                PassedOnBillingInformation passedonBillingAssignment = new PassedOnBillingInformation();
                passedonBillingViewModels.ZoneList = db.Zone.Select(x => x).ToList();
                passedonBillingAssignment.CompanyId = CompanyId;
                passedonBillingAssignment.Amount = Amount;
                passedonBillingAssignment.Type = Type;
                passedonBillingAssignment.OriginDate = OriginDateAdd;
                passedonBillingAssignment.BillingDate = BillingDateAdd;
                passedonBillingAssignment.CreateDate = DateTime.Now;
                passedonBillingAssignment.CreatedBy = userid;
                passedonBillingAssignment.BillingPeriod = BillingPeriod;
                db.PassedOnBillingInformation.Add(passedonBillingAssignment);
                db.SaveChanges();

                passedonBillingViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
                passedonBillingViewModels.PassedOnBillingList = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).ToList();
                passedonBillingViewModels.TotalAmount = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).Sum(x => x.Amount);

                //passedonBillingViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).Where(m => m.Finalized.ToUpper() != "YES").ToList();
                passedonBillingViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
                var billp = passedonBillingViewModels.BillingPeriodList.OrderByDescending(m=>m.BillingPeriodId);
                passedonBillingViewModels.BillingPeriodList = billp.ToList();
                //return View("ViewSecurityGuardFee", SearchSecurityGuardFeeViewModels);
                TempData["TransactionSuccess"] = "Add";
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Passed OnBilling - Add -from Terminal:" + ipaddress);
                TempData["PassedOnBillingData"] = passedonBillingViewModels;
            }
            else
            {
                int passedonbillingid = Convert.ToInt32(PassedOnBillingInfoId);
                passedonBillingInfo = db.PassedOnBillingInformation.FirstOrDefault(m => m.PassedOnBillingInformationId == passedonbillingid);
                PassedOnBillingViewModel passedonBillingViewModels = new PassedOnBillingViewModel();
                passedonBillingViewModels.ZoneList = db.Zone.Select(x => x).ToList();
                passedonBillingInfo.Amount = Amount;
                passedonBillingInfo.OriginDate = OriginDateAdd;
                passedonBillingInfo.BillingDate = BillingDateAdd;
                passedonBillingInfo.BillingPeriod = BillingPeriod;
                passedonBillingInfo.Type = Type;
                passedonBillingInfo.UpdatedBy = userid;
                passedonBillingInfo.UpdateDate = DateTime.Now;
                db.Entry(passedonBillingInfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                passedonBillingViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
                passedonBillingViewModels.PassedOnBillingList = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).ToList();
                passedonBillingViewModels.TotalAmount = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).Sum(x => x.Amount);
                ViewBag.CompanySelected = "OK";

                //passedonBillingViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).Where(m => m.Finalized.ToUpper() != "YES").ToList();
                passedonBillingViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
                var billp = passedonBillingViewModels.BillingPeriodList.OrderByDescending(m => m.BillingPeriodId);
                passedonBillingViewModels.BillingPeriodList = billp.ToList();
                TempData["TransactionSuccess"] = "Edit";
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Passed OnBilling - Edit -from Terminal:" + ipaddress);
                TempData["PassedOnBillingData"] = passedonBillingViewModels;
            }
            return RedirectToAction("ViewPassedOnBillingPRG", "DataEntryPassedOnBilling");
        }

        [HttpPost]
        public ActionResult DeletePassedOnBilling(int idToDelete, FormCollection frm)
        {
            var id = frm[0].ToString();
            int CompanyId = int.Parse(id);
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            PassedOnBillingInformation passedOnBillingInformation = new PassedOnBillingInformation();
            passedOnBillingInformation = db.PassedOnBillingInformation.Find(idToDelete);
            db.PassedOnBillingInformation.Remove(passedOnBillingInformation);
            db.SaveChanges();
            PassedOnBillingViewModel passedonBillingViewModels = new PassedOnBillingViewModel();
            passedonBillingViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
            passedonBillingViewModels.PassedOnBillingList = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).ToList();
            try
            {
                passedonBillingViewModels.TotalAmount = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).Sum(x => x.Amount);
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Passed OnBilling - Delete Record -from Terminal:" + ipaddress);
            }
            catch (Exception)
            {
                passedonBillingViewModels.TotalAmount = 0;
            }
            
            ViewBag.CompanySelected = "OK";

            passedonBillingViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
            var billp = passedonBillingViewModels.BillingPeriodList.OrderByDescending(m => m.BillingPeriodId);
            passedonBillingViewModels.BillingPeriodList = billp.ToList();
            TempData["TransactionSuccess"] = "delete";
            TempData["PassedOnBillingData"] = passedonBillingViewModels;
            return RedirectToAction("ViewPassedOnBillingPRG", "DataEntryPassedOnBilling");
        }

        // Display OR Delete Security Guard Fee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisplayPassedOnBilling(int CompId, FormCollection frmcollection)
        {
            PassedOnBillingViewModel searchcompany = new PassedOnBillingViewModel();
            searchcompany.ZoneList = db.Zone.Select(x => x).ToList();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.PassedOnBilling;
            PassedOnBillingViewModel SearchCompanyViewModels = new PassedOnBillingViewModel();
            if (frmcollection.Count == 0)
            {
                SearchCompanyViewModels.PassedOnBillingList = db.PassedOnBillingInformation.SqlQuery("Select * from SecurityGuardFeeInformations where CompanyId = '" + frmcollection["CompanyId"] + "'").ToList();
                SearchCompanyViewModels.TotalAmount = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompId).Sum(x => x.Amount);
                return View(SearchCompanyViewModels);
            }
            else if (frmcollection.Count >= 1)
            {
                PassedOnBillingViewModel passedonBillingViewModels = new PassedOnBillingViewModel();
                int parsedID = int.Parse(frmcollection["PassedOnBillingInformationId"]);
                PassedOnBillingInformation securityfee = db.PassedOnBillingInformation.Find(parsedID);
                //db.PassedOnBillingInformation.Remove(securityfee);
                //db.SaveChanges();
                passedonBillingViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompId).ToList();
                passedonBillingViewModels.PassedOnBillingList = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompId).ToList();
                passedonBillingViewModels.TotalAmount = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompId).Sum(x => x.Amount);
                return View("ViewPassedOnBilling", passedonBillingViewModels);
            }
            ViewBag.CompanySelected = "OK";
            //SearchCompanyViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).Where(m => m.Finalized.ToUpper() != "YES").ToList();
            SearchCompanyViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
            var billp = SearchCompanyViewModels.BillingPeriodList.OrderByDescending(m => m.BillingPeriodId);
            SearchCompanyViewModels.BillingPeriodList = billp.ToList();
            return View();
        }

        

        //// Update Data of Security Guard Fee
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdatePassedOnBilling(int CompanyId, FormCollection frmcollection)
        //{
        //    var userid = User.Identity.GetUserId();
        //    string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
        //    var username = User.Identity.GetUserName();
        //    RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
        //    ViewBag.IsValidRole = roleAssignmentMatrix.PassedOnBilling;
        //    PassedOnBillingInformation passedonBillingInfo = null;
        //    //int ParsedIntID = int.Parse(frmcollection["ID"]);
        //    passedonBillingInfo = db.PassedOnBillingInformation.Find(CompanyId);

        //    if (passedonBillingInfo != null)
        //    {
        //        PassedOnBillingViewModel passedonBillingViewModels = new PassedOnBillingViewModel();
        //        passedonBillingInfo.Amount = Convert.ToDecimal(frmcollection["Amount"].ToString());
        //        passedonBillingInfo.OriginDate = Convert.ToDateTime(frmcollection["StartDate"].ToString());
        //        passedonBillingInfo.BillingDate = Convert.ToDateTime(frmcollection["EndDate"].ToString());
        //        passedonBillingInfo.BillingPeriod = Convert.ToInt32(frmcollection["BillingPeriod"].ToString());
        //        passedonBillingInfo.Type = frmcollection["Type"].ToString();
        //        passedonBillingInfo.UpdatedBy = userid;
        //        passedonBillingInfo.UpdateDate = DateTime.Now;
        //        db.Entry(passedonBillingInfo).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();

        //        passedonBillingViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
        //        passedonBillingViewModels.PassedOnBillingList = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).ToList();
        //        passedonBillingViewModels.TotalAmount = db.PassedOnBillingInformation.Where(m => m.CompanyId == CompanyId).Sum(x => x.Amount);
        //        ViewBag.CompanySelected = "OK";
        //        //ViewBag.TransactionSuccess = "Edit";
        //        passedonBillingViewModels.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroup).ToList();
        //        //return View("ViewSecurityGuardFee", SearchSecurityGuardFeeViewModels);
        //        TempData["TransactionSuccess"] = "Edit";
        //        TempData["PassedOnBillingData"] = passedonBillingViewModels;
        //        return RedirectToAction("ViewPassedOnBillingPRG", "DataEntryPassedOnBilling");
        //    }
        //    else
        //    {
        //        throw new Exception("Invalid transaction.");
        //    }
        //}

        // Generate Report
        [HttpPost]
        public ActionResult PassedOnBillingReport(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var billingPeriod = frm["billingPeriod"];
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Passed OnBilling - View Report -from Terminal:" + ipaddress);
            //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fPassedOnBilling&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod;

            return Redirect(serverURI.Uri.ToString());
        }

    }
}