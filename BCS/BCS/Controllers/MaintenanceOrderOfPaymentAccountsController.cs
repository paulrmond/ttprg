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

namespace BCS.Controllers
{
    public class MaintenanceOrderOfPaymentAccountsController : Controller
    {
        // GET: MaintenanceOrderOfPaymentAccounts
        private BCS_Context db = new BCS_Context();
        private BCS_Context dc = new BCS_Context();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();


        [HttpGet]
        public ActionResult ViewOrderOfPaymentAccounts()
        {
            SearchOPAccountsViewModel SearchOPAccountViewModels = new SearchOPAccountsViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            //SearchOPAccountViewModels.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts Where ZoneGroupCode = '" + ZoneGroup + "'").ToList();
            //SearchOPAccountViewModels.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            SearchOPAccountViewModels.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            SearchOPAccountViewModels.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).Take(10).OrderBy(x => x.OPAccountDescription).ToList();
            SearchOPAccountsViewModel temp = TempData["SearchOPAccountViewModels"] as SearchOPAccountsViewModel;
            if (ViewBag.TransactionSuccess == null)
            {
                return View(SearchOPAccountViewModels);
            }
            else
            {
                ViewBag.CompanySelected = "OK";
                if (temp != null){
                    return View("ViewOrderOfPaymentAccounts", temp);
                } else {
                    return View("ViewOrderOfPaymentAccounts", SearchOPAccountViewModels);
                }
            }
        }

        // Search OPAccount or Initiate Add Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewOrderOfPaymentAccounts(string SearchInput, string Rate, FormCollection frm)
        {
            SearchOPAccountsViewModel searchopaccount = new SearchOPAccountsViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                searchopaccount.SearchInput = SearchInput.ToString();
                //searchopaccount.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountDescription like '%" + SearchInput + "%' AND ZoneGroupCode = '" + ZoneGroup + "'").ToList();
                searchopaccount.OPAccountList = db.OPAccount.Where(x => x.OPAccountDescription.Contains(SearchInput)).Where(z => z.ZoneGroupCode == ZoneGroup).OrderBy(d => d.OPAccountDescription).ToList();
                searchopaccount.DivisionList = db.Division.Select(x => x).ToList();
                return View(searchopaccount);
            }
            //Result of selected OPAccount shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    searchopaccount.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts Where ZoneGroupCode = '" + ZoneGroup + "' AND OPAccountId = '" + ParsedCompanyID + "'").ToList();
                    searchopaccount.DivisionList = db.Division.Select(x => x).ToList();
                }
                ViewBag.CompanySelected = "OK";
            }
            //Default value
            searchopaccount.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            return View(searchopaccount);
        }

        // Update OPAccount Data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOPAccount(FormCollection frmcollection)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Updated Item! (Maintenance Nature of Payments) - from Terminal:" + ipaddress);
            OPAccount opaccountinfo = null;
            ApplicationDbContext context = new ApplicationDbContext();
            var OPAFee = frmcollection["OPAccountFee"].ToString().Replace(",", "");
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
            int ParsedIntCompanyID = int.Parse(frmcollection["OPAccountId1"]);
            opaccountinfo = db.OPAccount.Find(ParsedIntCompanyID);
            if (opaccountinfo != null)
            {
                opaccountinfo.OPAccountCode = frmcollection["OPAccountCode"].ToString();
                opaccountinfo.NGASCode = frmcollection["OPAccountNGASCode"].ToString();
                opaccountinfo.OPAccountDescription = frmcollection["OPAccountDescription"].ToString();
                opaccountinfo.OPAccountFee = Convert.ToDecimal(OPAFee);
                opaccountinfo.DivisionCode = frmcollection["DivisionCode"];
                opaccountinfo.ZoneGroupCode = ZoneGroup;
                opaccountinfo.OPAccountDescription = frmcollection["OPAccountDescription"].ToString();
                opaccountinfo.OPAccountValidity = frmcollection["OPAccountValidity"].ToString();
                opaccountinfo.AccountTag = frmcollection["AccountTag"].ToString();
                db.Entry(opaccountinfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            foreach (var results in db.OrderOfPaymentDetail.Where(b => b.OPAccountId == ParsedIntCompanyID))
            {
                results.AccountTag = frmcollection["AccountTag"].ToString();
            }
            db.SaveChanges();
            int parsedCompanyID = int.Parse(frmcollection["OPAccountId1"]);
            SearchOPAccountsViewModel searchopaccount = new SearchOPAccountsViewModel();
            //searchopaccount.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).Take(10).ToList();
            searchopaccount.OPAccountList = db.OPAccount.Where(x => x.OPAccountId == parsedCompanyID).ToList();
            searchopaccount.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            ViewBag.TransactionSuccess = "Edit";
            ViewBag.CompanySelected = "OK";
            return View("ViewOrderOfPaymentAccounts", searchopaccount);
        }

        public ActionResult AddOPAccount(FormCollection frmcollection)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Maintenance Nature of Payments) - from Terminal:" + ipaddress);
            SearchOPAccountsViewModel SearchOPAccountViewModels = new SearchOPAccountsViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var FeeVal = frmcollection["OPAccountFee"];
            if (FeeVal.Contains("."))
            { }
            else
            {
                FeeVal = FeeVal + ".00";
            }
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
            OPAccount OPAccountAssignment = new OPAccount();
            //CONDITION
            OPAccountAssignment.OPAccountCode = frmcollection["OPAccountCode"].ToString();
            OPAccountAssignment.NGASCode = frmcollection["OPAccountNGASCode"].ToString();
            OPAccountAssignment.OPAccountDescription = frmcollection["OPAccountDescription"].ToString();
            OPAccountAssignment.OPAccountFee = Convert.ToDecimal(FeeVal);
            OPAccountAssignment.OPAccountValidity = frmcollection["OPAccountValidity"].ToString();
            OPAccountAssignment.DivisionCode = frmcollection["DivisionCode"];
            OPAccountAssignment.ZoneGroupCode = ZoneGroup;
            OPAccountAssignment.AccountTag = frmcollection["AddAccountTag"];
            db.OPAccount.Add(OPAccountAssignment);
            db.SaveChanges();
            //SearchOPAccountViewModels.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).ToList();
            SearchOPAccountViewModels.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).Take(50).ToList();
            SearchOPAccountViewModels.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            TempData["TransactionSuccess"] = "Add";
            TempData["SearchOPAccountViewModels"] = SearchOPAccountViewModels;
            return RedirectToAction("ViewOrderOfPaymentAccounts", "MaintenanceOrderOfPaymentAccounts");
        }

        public ActionResult RemoveOPAccounts(FormCollection frm)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Deleted Item! (Maintenance Nature of Payments) - from Terminal:" + ipaddress);
            SearchOPAccountsViewModel SearchOPAccountViewModels = new SearchOPAccountsViewModel();
            int parsedID = int.Parse(frm["OPAccountId"]);
            OPAccount opaccount = db.OPAccount.Find(parsedID);
            db.OPAccount.Remove(opaccount);
            db.SaveChanges();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
            //SearchOPAccountViewModels.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts Where ZoneGroupCode = '" + ZoneGroup + "'").ToList();
            SearchOPAccountViewModels.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).ToList();
            SearchOPAccountViewModels.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            TempData["TransactionSuccess"] = "delete";
            TempData["SearchOPAccountViewModels"] = SearchOPAccountViewModels;
            return RedirectToAction("ViewOrderOfPaymentAccounts", "MaintenanceOrderOfPaymentAccounts");
        }

        public ActionResult ViewSelectFilter(FormCollection frm)
        {
            SearchOPAccountsViewModel OPAccounts = new SearchOPAccountsViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            //string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
            string FilDivision = frm["SelectFilter"].ToString();
            if (frm["SelectFilter"] == "All")
            {
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by All (Maintenance Nature of Payments) - from Terminal:" + ipaddress);
                //OPAccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts Where ZoneGroupCode = '" + ZoneGroup + "'").ToList();
                OPAccounts.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).ToList();
            }
            else
            {
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Division (Maintenance Nature of Payments) - from Terminal:" + ipaddress);
                OPAccounts.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup && x.DivisionCode == FilDivision).OrderBy(x => x.OPAccountDescription).ToList();
            }
            ViewBag.CompanySelected = "OK";
            OPAccounts.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            return View("ViewOrderOfPaymentAccounts", OPAccounts);
        }

        // Display Sub Items 
        public ActionResult ViewOPASubItems(int OPAId)
        {
            SearchOPAccountsViewModel SearchOrderOfPaymentDetailyViewModels = new SearchOPAccountsViewModel();
            SearchOrderOfPaymentDetailyViewModels.OPASubItemsList = db.OPASubItems.Where(x => x.OPAccountId == OPAId).ToList();
            SearchOPAccountsViewModel temp = TempData["SearchOPASubItemsViewModels"] as SearchOPAccountsViewModel;
            TempData["OPAId"] = OPAId;
            if (ViewBag.TransactionSuccess == null)
            {
                return View("ViewOPASubItems", SearchOrderOfPaymentDetailyViewModels);
            }
            else
            {
                ViewBag.CompanySelected = "OK";
                return View("ViewOPASubItems", temp);
            }           
        }

        // Add Sub Items
        public ActionResult AddOPASubItems(FormCollection frmcollection)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Maintenance Nature of Payments Sub Items) - from Terminal:" + ipaddress);
            SearchOPAccountsViewModel SearchOPAccountViewModels = new SearchOPAccountsViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var FeeVal = frmcollection["OPAccountFee"];
            int OPAId = Convert.ToInt32(frmcollection["OPAId"]);
            if (FeeVal.Contains("."))
            { }
            else
            {
                FeeVal = FeeVal + ".00";
            }
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
            OPASubItems OPASubItemsAssignment = new OPASubItems();
            //CONDITION
            OPASubItemsAssignment.Code = frmcollection["OPAccountCode"].ToString();
            OPASubItemsAssignment.NGASCode = frmcollection["OPAccountNGASCode"].ToString();
            OPASubItemsAssignment.Description = frmcollection["OPAccountDescription"].ToString();
            OPASubItemsAssignment.OPAccountId = OPAId;
            OPASubItemsAssignment.Fee = Convert.ToDecimal(FeeVal);
            db.OPASubItems.Add(OPASubItemsAssignment);
            db.SaveChanges();
            //SearchOPAccountViewModels.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).ToList();
            SearchOPAccountViewModels.OPASubItemsList = db.OPASubItems.Where(x => x.OPAccountId == OPAId).ToList();
            SearchOPAccountViewModels.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            foreach (var results2 in dc.OPAccount.Where(b => b.OPAccountId == OPAId))
            {
                results2.OPAccountFee = SearchOPAccountViewModels.OPASubItemsList.Sum(x => x.Fee);
            }
            dc.SaveChanges();
            foreach (var results in db.OrderOfPaymentDetail.Where(b => b.OPAccountId == OPAId))
            {
                results.Amount = SearchOPAccountViewModels.OPASubItemsList.Sum(x => x.Fee);
                results.TotalAmount = SearchOPAccountViewModels.OPASubItemsList.Sum(x => x.Fee);
            }
            db.SaveChanges();
            TempData["TransactionSuccess"] = "Add";
            TempData["SearchOPASubItemsViewModels"] = SearchOPAccountViewModels;
            return RedirectToAction("ViewOPASubItems", "MaintenanceOrderOfPaymentAccounts", new { OPAId = OPAId });
        }

        // Update OPAccount Data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOPASubItems(FormCollection frmcollection)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Updated Item! (Maintenance Nature of Payments Sub Items) - from Terminal:" + ipaddress);
            OPASubItems opaccountinfo = null;
            var OPAFee = frmcollection["EditFee"].ToString().Replace(",", "");
            int ParsedIntCompanyID = int.Parse(frmcollection["EditSubItemsId"]);
            int OPAId = Convert.ToInt32(frmcollection["EditOPAId"]);
            opaccountinfo = db.OPASubItems.Find(ParsedIntCompanyID);
            if (opaccountinfo != null)
            {
                opaccountinfo.Code = frmcollection["EditCode"].ToString();
                opaccountinfo.NGASCode = frmcollection["EditNGASCode"].ToString();
                opaccountinfo.Description = frmcollection["EditDescription"].ToString();
                opaccountinfo.OPAccountId = Convert.ToInt32(frmcollection["EditOPAId"]);
                opaccountinfo.Fee = Convert.ToDecimal(frmcollection["EditFee"]);
                db.Entry(opaccountinfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            db.SaveChanges();
            int parsedOPAId = Convert.ToInt32(frmcollection["EditOPAId"]);
            SearchOPAccountsViewModel searchopaccount = new SearchOPAccountsViewModel();
            //searchopaccount.OPAccountList = db.OPAccount.Where(x => x.ZoneGroupCode == ZoneGroup).OrderBy(x => x.OPAccountDescription).Take(10).ToList();
            searchopaccount.OPASubItemsList = db.OPASubItems.Where(x => x.OPAccountId == parsedOPAId).ToList();
            searchopaccount.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            foreach (var results in db.OrderOfPaymentDetail.Where(b => b.OPAccountId == OPAId))
            {
                results.Amount = searchopaccount.OPASubItemsList.Sum(x => x.Fee);
                results.TotalAmount = searchopaccount.OPASubItemsList.Sum(x => x.Fee);
            }
            db.SaveChanges();
            foreach (var results2 in dc.OPAccount.Where(b => b.OPAccountId == OPAId))
            {
                results2.OPAccountFee = searchopaccount.OPASubItemsList.Sum(x => x.Fee);
            }
            dc.SaveChanges();
            ViewBag.TransactionSuccess = "Edit";
            ViewBag.CompanySelected = "OK";
            TempData["OPAId"] = parsedOPAId;
            return View("ViewOPASubItems", searchopaccount);
        }

        public ActionResult RemoveOPASubItems(FormCollection frm)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Deleted Item! (Maintenance Nature of Payments Sub Items) - from Terminal:" + ipaddress);
            SearchOPAccountsViewModel SearchOPASubItemsViewModels = new SearchOPAccountsViewModel();
            int parsedID = Convert.ToInt32(frm["RemSubItemsId"]);
            int OPAId = Convert.ToInt32(frm["RemOPAId"]);
            OPASubItems opaccount = db.OPASubItems.Find(parsedID);
            db.OPASubItems.Remove(opaccount);
            db.SaveChanges();
            SearchOPASubItemsViewModels.OPASubItemsList = db.OPASubItems.Where(x => x.OPAccountId == OPAId).ToList();
            SearchOPASubItemsViewModels.DivisionList = db.Division.SqlQuery("Select * from Divisions").ToList();
            foreach (var results in db.OrderOfPaymentDetail.Where(b => b.OPAccountId == OPAId))
            {
                results.Amount = SearchOPASubItemsViewModels.OPASubItemsList.Sum(x => x.Fee);
                results.TotalAmount = SearchOPASubItemsViewModels.OPASubItemsList.Sum(x => x.Fee);
            }
            db.SaveChanges();
            foreach (var results2 in dc.OPAccount.Where(b => b.OPAccountId == OPAId))
            {
                results2.OPAccountFee = SearchOPASubItemsViewModels.OPASubItemsList.Sum(x => x.Fee);
            }
            dc.SaveChanges();
            TempData["TransactionSuccess"] = "delete";
            TempData["SearchOPASubItemsViewModels"] = SearchOPASubItemsViewModels;
            return RedirectToAction("ViewOPASubItems", "MaintenanceOrderOfPaymentAccounts", new { OPAId = OPAId });
        }
    }
}