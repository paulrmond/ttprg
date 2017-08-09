using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Net;

namespace BCS.Controllers
{
    public class MaintenanceSubsidiaryLedgerController : Controller
    {

        private BCS_Context db = new BCS_Context();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        [HttpGet]
        public ActionResult ViewSubsidiaryLedger()
        {
            SearchSubsidiaryLedgerViewModel searchcompany = new SearchSubsidiaryLedgerViewModel();
            searchcompany.ZoneList = db.Zone.Select(x => x).ToList();
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            SearchSubsidiaryLedgerViewModel temp = TempData["SearchSubsidiaryLedgerViewModels"] as SearchSubsidiaryLedgerViewModel;
            ViewBag.CompanySelected = "OK";
            if (ViewBag.TransactionSuccess == null)
            {
                return View("ViewSubsidiaryLedger", searchcompany);
            }
            else
            {
                var username = User.Identity.GetUserName();
                RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
                ViewBag.IsValidRole = roleAssignmentMatrix.SubsidiaryLedger;
                return View("ViewSubsidiaryLedger", temp);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewSubsidiaryLedger(string SearchInput, FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            SearchSubsidiaryLedgerViewModel searchcompany1 = new SearchSubsidiaryLedgerViewModel();
            searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            List<Company> NewCompanies = new List<Company>();
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                searchcompany1.SearchInput = SearchInput.ToString();
                NewCompanies = db.Company.Where(x => x.CompanyName.Contains(SearchInput)).Take(50).OrderBy(x => x.CompanyName).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                //searchcompany1.SubsidiaryLedgerList = db.SubsidiaryLedger.Select(x => x).ToList();
                searchcompany1.BalanceList = db.Balances.Select(x => x).ToList();
                return View(searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                searchcompany1.BalanceList = db.Balances.Select(x => x).ToList();
                if (CanParse)
                {
                    var username = User.Identity.GetUserName();
                    RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
                    ViewBag.IsValidRole = roleAssignmentMatrix.SubsidiaryLedger;
                    ViewBag.CompanySelected = "OK";

                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    searchcompany1.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == ParsedCompanyID).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                    searchcompany1.SubsidiaryLedgerToList = db.SubsidiaryLedger.Where(m => m.CompanyId == ParsedCompanyID).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                    searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                    var billT = db.SubsidiaryLedger.GroupBy(m => m.BillingType).Select(g => new { BillingType = g.Key }).ToList();
                    foreach (var itemss in billT)
                    {
                        searchcompany1.BillingType.Add(itemss.BillingType.ToUpper().Replace(" ", ""));
                    }
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        if (item.Category == "Garbage Fee")
                        {
                            var newcat = item.Category.Replace("Fee", "");
                            searchcompany1.Category.Add(newcat.Replace(" ", "").ToUpper());
                        }
                        else if (item.Category == "Rental Fee")
                        {
                            var newcat = item.Category.Replace("Fee", "");
                            searchcompany1.Category.Add(newcat.Replace(" ", "").ToUpper());
                        }
                        else
                        {
                            searchcompany1.Category.Add(item.Category.ToUpper());
                        }
                    }
                    foreach (var items in searchcompany1.BillingType)
                    {
                        if (searchcompany1.SubsidiaryLedgerList.Count != 0)
                        {
                            for (int z = 0; z < searchcompany1.Category.Count; z++)
                            {
                                if (items == searchcompany1.Category[z].Replace(" ", ""))
                                {
                                    if (items == "ADMINFEE")
                                    {
                                        TempData["BillType"] = "ADMIN FEE";
                                    }
                                    else if (items == "PASSEDONBILLING")
                                    {
                                        TempData["BillType"] = "PASSED ON BILLING";
                                    }
                                    else if (items == "POLERENTAL")
                                    {
                                        TempData["BillType"] = "POLE RENTAL";
                                    }
                                    else
                                    {
                                        TempData["BillType"] = items;
                                    }
                                    var contItem = TempData["BillType"].ToString();
                                    try
                                    {
                                        var SumCredit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == ParsedCompanyID).Sum(y => y.CreditAmount);
                                        searchcompany1.Credit.Add(SumCredit);
                                        var SumDebit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == ParsedCompanyID).Sum(y => y.DebitAmount);
                                        searchcompany1.Debit.Add(SumDebit);
                                        var CurrentBalance = SumDebit - SumCredit;
                                        searchcompany1.CurrentBalance.Add(CurrentBalance);
                                        searchcompany1.CategoryList.Add(contItem);
                                    }
                                    catch
                                    { }
                                    if (searchcompany1.BalanceList.Count != 0)
                                    {
                                        try
                                        {
                                            var a = db.Balances.Where(d => d.BillingType == contItem && d.CompanyId == ParsedCompanyID).ToList();
                                            var prevbal = a.LastOrDefault(x => x.BillingType == contItem).Amount;
                                            searchcompany1.PreviousBalance.Add(prevbal);
                                        }
                                        catch { searchcompany1.PreviousBalance.Add(0); }

                                    }
                                    else
                                    {
                                        searchcompany1.PreviousBalance.Add(0);
                                    }
                                }
                                //
                            }
                        }
                    }
                }
                return View(searchcompany1);
            }
            //Default value
            else
            {
                return View(searchcompany1);
            }
        }

        public ActionResult AddSubsidiaryLedger(int CompanyId, DateTime AddTransactionDate, DateTime AddDueDate, decimal Debit, decimal Credit, string Remarks, string BillingType, string TransactionType, string BillingSubType, string AddBillRef)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            var username = User.Identity.GetUserName();
            var DebitVal = Convert.ToString(Debit);
            if (DebitVal.Contains("."))
            { }
            else
            {
                DebitVal = DebitVal + ".00";
            }
            var CreditVal = Convert.ToString(Credit);
            if (CreditVal.Contains("."))
            { }
            else
            {
                CreditVal = CreditVal + ".00";
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Maintenance Subsidiary Ledger) - from Terminal:" + ipaddress);
            SearchSubsidiaryLedgerViewModel SearchSubsidiaryLedgerViewModels = new SearchSubsidiaryLedgerViewModel();
            SubsidiaryLedger SubsidiaryAssignment = new SubsidiaryLedger();
            SearchSubsidiaryLedgerViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            SubsidiaryAssignment.CompanyId = CompanyId;
            SubsidiaryAssignment.TransactionDate = AddTransactionDate;
            var BillPeriodLlist = db.BillingPeriod.Where(x => x.Generated == "YES" && x.groupCode == ZoneGroup).ToList().Last();
            SubsidiaryAssignment.BillingPeriod = BillPeriodLlist.BillingPeriodId;
            SubsidiaryAssignment.BillingType = BillingType;
            SubsidiaryAssignment.CreatedBy = username;
            SubsidiaryAssignment.TransactionType = "ADJUSTMENT";
            SubsidiaryAssignment.DebitAmount = Convert.ToDecimal(DebitVal);
            SubsidiaryAssignment.CreditAmount = Convert.ToDecimal(CreditVal);
            SubsidiaryAssignment.Remarks = Remarks;
            SubsidiaryAssignment.BillingDate = DateTime.Now;
            SubsidiaryAssignment.CreateDate = DateTime.Now;
            SubsidiaryAssignment.DueDate = AddDueDate;
            SubsidiaryAssignment.BillingSubType = BillingSubType;
            SubsidiaryAssignment.BillingReference = AddBillRef;
            SubsidiaryAssignment.Currency = "PHP";  



            if (BillingType == "PASSED ON BILLING")
            {
                var zii = Convert.ToInt32(AddBillRef);
                SubsidiaryAssignment.Other = db.PassedOnBillingInformation.FirstOrDefault(x => x.PassedOnBillingInformationId == zii).Type;
            }
            else if (BillingType == "ADMIN FEE")
            {
                SubsidiaryAssignment.Other = db.Company.FirstOrDefault(x => x.CompanyID == CompanyId).ZoneCode;
            }else
            {
                SubsidiaryAssignment.Other = "";
            }

            db.SubsidiaryLedger.Add(SubsidiaryAssignment);
            db.SaveChanges();

            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.SubsidiaryLedger;

            SearchSubsidiaryLedgerViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
            SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == CompanyId).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();

            SearchSubsidiaryLedgerViewModels.BalanceList = db.Balances.Select(x => x).ToList();
            var billT = db.SubsidiaryLedger.GroupBy(m => m.BillingType).Select(g => new { BillingType = g.Key }).ToList();
            foreach (var itemss in billT)
            {
                SearchSubsidiaryLedgerViewModels.BillingType.Add(itemss.BillingType.ToUpper().Replace(" ", ""));
            }
            //SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Select(x => x).ToList();
            var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
            foreach (var item in bill)
            {
                if (item.Category == "Garbage Fee")
                {
                    var newcat = item.Category.Replace("Fee", "");
                    SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                }
                else if (item.Category == "Rental Fee")
                {
                    var newcat = item.Category.Replace("Fee", "");
                    SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                }
                else
                {
                    SearchSubsidiaryLedgerViewModels.Category.Add(item.Category.ToUpper());
                }
            }
            foreach (var items in SearchSubsidiaryLedgerViewModels.BillingType)
            {
                if (SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList.Count != 0)
                {
                    for (int z = 0; z < SearchSubsidiaryLedgerViewModels.Category.Count; z++)
                    {
                        if (items == SearchSubsidiaryLedgerViewModels.Category[z].Replace(" ", ""))
                        {
                            if (items == "ADMINFEE")
                            {
                                TempData["BillType"] = "ADMIN FEE";
                            }
                            else if (items == "PASSEDONBILLING")
                            {
                                TempData["BillType"] = "PASSED ON BILLING";
                            }
                            else if (items == "POLERENTAL")
                            {
                                TempData["BillType"] = "POLE RENTAL";
                            }
                            else
                            {
                                TempData["BillType"] = items;
                            }
                            var contItem = TempData["BillType"].ToString();
                            try
                            {
                                var SumCredit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompanyId).Sum(y => y.CreditAmount);
                                SearchSubsidiaryLedgerViewModels.Credit.Add(SumCredit);
                                var SumDebit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompanyId).Sum(y => y.DebitAmount);
                                SearchSubsidiaryLedgerViewModels.Debit.Add(SumDebit);
                                var CurrentBalance = SumDebit - SumCredit;
                                SearchSubsidiaryLedgerViewModels.CurrentBalance.Add(CurrentBalance);
                                SearchSubsidiaryLedgerViewModels.CategoryList.Add(contItem);
                            }
                            catch { }
                            if (SearchSubsidiaryLedgerViewModels.BalanceList.Count != 0)
                            {
                                try
                                {
                                    var a = db.Balances.Where(d => d.BillingType == contItem && d.CompanyId == CompanyId).ToList();
                                    var prevbal = a.LastOrDefault(x => x.BillingType == contItem).Amount;
                                    SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(prevbal);
                                }
                                catch { SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0); }

                            }
                            else
                            {
                                SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0);
                            }
                        }
                    }
                }
            }
            ViewBag.CompanySelected = "OK";
            TempData["TransactionSuccess"] = "Add";
            TempData["SearchSubsidiaryLedgerViewModels"] = SearchSubsidiaryLedgerViewModels;
            return RedirectToAction("ViewSubsidiaryLedger", "MaintenanceSubsidiaryLedger");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisplaySubsidiaryLedgerList(int? CompanyId, FormCollection frmcollection)
        {
            SearchSubsidiaryLedgerViewModel SearchCompanyViewModels = new SearchSubsidiaryLedgerViewModel();
            SearchCompanyViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            if (frmcollection.Count == 0)
            {
                int compId = Convert.ToInt32(frmcollection["CompanyId"]);
                SearchCompanyViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(x => x.CompanyId == compId).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                foreach (var item in bill)
                {
                    SearchCompanyViewModels.Category.Add(item.Category);
                }
                return View(SearchCompanyViewModels);
            }
            else if (frmcollection.Count >= 1)
            {
                int parsedID = int.Parse(frmcollection["SubLedgerId"]);
                SearchSubsidiaryLedgerViewModel SearchSubsidiaryLedgerViewModels = new SearchSubsidiaryLedgerViewModel();
                SubsidiaryLedger SubsidiaryLedgerAssignment = new SubsidiaryLedger();
                SubsidiaryLedger SubsidiaryLedger = db.SubsidiaryLedger.Find(parsedID);
                db.SubsidiaryLedger.Remove(SubsidiaryLedger);
                db.SaveChanges();
                SearchSubsidiaryLedgerViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompanyId).ToList();
                SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == CompanyId).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                SearchSubsidiaryLedgerViewModels.BalanceList = db.Balances.Select(x => x).ToList();
                var billT = db.SubsidiaryLedger.GroupBy(m => m.BillingType).Select(g => new { BillingType = g.Key }).ToList();
                foreach (var itemss in billT)
                {
                    SearchSubsidiaryLedgerViewModels.BillingType.Add(itemss.BillingType.ToUpper().Replace(" ", ""));
                }
                SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Select(x => x).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                foreach (var item in bill)
                {
                    if (item.Category == "Garbage Fee")
                    {
                        var newcat = item.Category.Replace("Fee", "");
                        SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                    }
                    else if (item.Category == "Rental Fee")
                    {
                        var newcat = item.Category.Replace("Fee", "");
                        SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                    }
                    else
                    {
                        SearchSubsidiaryLedgerViewModels.Category.Add(item.Category.ToUpper());
                    }
                }
                foreach (var items in SearchSubsidiaryLedgerViewModels.BillingType)
                {
                    if (SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList.Count != 0)
                    {
                        for (int z = 0; z < SearchSubsidiaryLedgerViewModels.Category.Count; z++)
                        {
                            if (items == SearchSubsidiaryLedgerViewModels.Category[z].Replace(" ", ""))
                            {
                                if (items == "ADMINFEE")
                                {
                                    TempData["BillType"] = "ADMIN FEE";
                                }
                                else if (items == "PASSEDONBILLING")
                                {
                                    TempData["BillType"] = "PASSED ON BILLING";
                                }
                                else if (items == "POLERENTAL")
                                {
                                    TempData["BillType"] = "POLE RENTAL";
                                }
                                else
                                {
                                    TempData["BillType"] = items;
                                }
                                var contItem = TempData["BillType"].ToString();
                                try
                                {
                                    var SumCredit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompanyId).Sum(y => y.CreditAmount);
                                    SearchSubsidiaryLedgerViewModels.Credit.Add(SumCredit);
                                    var SumDebit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompanyId).Sum(y => y.DebitAmount);
                                    SearchSubsidiaryLedgerViewModels.Debit.Add(SumDebit);
                                    var CurrentBalance = SumDebit - SumCredit;
                                    SearchSubsidiaryLedgerViewModels.CurrentBalance.Add(CurrentBalance);
                                    SearchSubsidiaryLedgerViewModels.CategoryList.Add(contItem);
                                }
                                catch { }
                                if (SearchSubsidiaryLedgerViewModels.BalanceList.Count != 0)
                                {
                                    try
                                    {
                                        var a = db.Balances.Where(d => d.BillingType == contItem && d.CompanyId == CompanyId).ToList();
                                        var prevbal = a.LastOrDefault(x => x.BillingType == contItem).Amount;
                                        SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(prevbal);
                                    }
                                    catch { SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0); }

                                }
                                else
                                {
                                    SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0);
                                }
                            }
                        }
                    }
                }
                var username = User.Identity.GetUserName();
                RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
                ViewBag.IsValidRole = roleAssignmentMatrix.SubsidiaryLedger;
                ViewBag.CompanySelected = "OK";
                return View("ViewSubsidiaryLedger", SearchSubsidiaryLedgerViewModels);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSubsidiaryLedger(int CompId, FormCollection frmcollection)
        {
            var DebitVal = frmcollection["Debit"].ToString();
            //if (DebitVal.Contains("."))
            //{ }
            //else
            //{
            //    DebitVal = DebitVal + ".00";
            //}
            var CreditVal = frmcollection["Credit"].ToString();
            //if (CreditVal.Contains("."))
            //{ }
            //else
            //{
            //    CreditVal = CreditVal + ".00";
            //}
            SubsidiaryLedger SubsidiaryLedgerinfo = null;
            int ParsedIntID = int.Parse(frmcollection["SubsidiaryId"]);
            SubsidiaryLedgerinfo = db.SubsidiaryLedger.Find(ParsedIntID);
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Updated Item! (Maintenance Subsidiary Ledger) - from Terminal:" + ipaddress);
            if (SubsidiaryLedgerinfo != null)
            {
                SubsidiaryLedgerinfo.CompanyId = CompId;
                SubsidiaryLedgerinfo.TransactionDate = Convert.ToDateTime(frmcollection["TransactionDate"].ToString());
                SubsidiaryLedgerinfo.DueDate = Convert.ToDateTime(frmcollection["DueDate"].ToString());
                try
                {
                    SubsidiaryLedgerinfo.BillingType = frmcollection["BillingType"].ToString();
                }
                catch { }
                try
                {
                    SubsidiaryLedgerinfo.BillingReference = frmcollection["EditBillRef"].ToString();
                }
                catch { }
                SubsidiaryLedgerinfo.BillingSubType = frmcollection["BillingSubType"].ToString();
                SubsidiaryLedgerinfo.DebitAmount = Convert.ToDecimal(DebitVal.Replace(",", ""));
                SubsidiaryLedgerinfo.CreditAmount = Convert.ToDecimal(CreditVal.Replace(",", ""));
                SubsidiaryLedgerinfo.Remarks = frmcollection["Remarks"].ToString();
                db.Entry(SubsidiaryLedgerinfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                SearchSubsidiaryLedgerViewModel SearchSubsidiaryLedgerViewModels = new SearchSubsidiaryLedgerViewModel();
                SearchSubsidiaryLedgerViewModels.ZoneList = db.Zone.Select(x => x).ToList();
                SearchSubsidiaryLedgerViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompId).ToList();
                SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == CompId).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                SearchSubsidiaryLedgerViewModels.BalanceList = db.Balances.Select(x => x).ToList();
                var billT = db.SubsidiaryLedger.GroupBy(m => m.BillingType).Select(g => new { BillingType = g.Key }).ToList();
                foreach (var itemss in billT)
                {
                    SearchSubsidiaryLedgerViewModels.BillingType.Add(itemss.BillingType.ToUpper().Replace(" ", ""));
                }
                //SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Select(x => x).ToList();
                var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                foreach (var item in bill)
                {
                    if (item.Category == "Garbage Fee")
                    {
                        var newcat = item.Category.Replace("Fee", "");
                        SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                    }
                    else if (item.Category == "Rental Fee")
                    {
                        var newcat = item.Category.Replace("Fee", "");
                        SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                    }
                    else
                    {
                        SearchSubsidiaryLedgerViewModels.Category.Add(item.Category.ToUpper());
                    }
                }
                foreach (var items in SearchSubsidiaryLedgerViewModels.BillingType)
                {
                    if (SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList.Count != 0)
                    {
                        for (int z = 0; z < SearchSubsidiaryLedgerViewModels.Category.Count; z++)
                        {
                            if (items.ToUpper() == SearchSubsidiaryLedgerViewModels.Category[z].Replace(" ", ""))
                            {
                                if (items == "ADMINFEE")
                                {
                                    TempData["BillType"] = "ADMIN FEE";
                                }
                                else if (items == "PASSEDONBILLING")
                                {
                                    TempData["BillType"] = "PASSED ON BILLING";
                                }
                                else if (items == "POLERENTAL")
                                {
                                    TempData["BillType"] = "POLE RENTAL";
                                }
                                else
                                {
                                    TempData["BillType"] = items;
                                }
                                var contItem = TempData["BillType"].ToString();
                                try
                                {
                                    try
                                    {
                                        var SumCredit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompId).Sum(y => y.CreditAmount);
                                        SearchSubsidiaryLedgerViewModels.Credit.Add(SumCredit);
                                        var SumDebit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompId).Sum(y => y.DebitAmount);
                                        SearchSubsidiaryLedgerViewModels.Debit.Add(SumDebit);
                                        var CurrentBalance = SumDebit - SumCredit;
                                        SearchSubsidiaryLedgerViewModels.CurrentBalance.Add(CurrentBalance);
                                        SearchSubsidiaryLedgerViewModels.CategoryList.Add(contItem);
                                    }
                                    catch { }
                                }
                                catch { }
                                if (SearchSubsidiaryLedgerViewModels.BalanceList.Count != 0)
                                {
                                    try
                                    {
                                        var a = db.Balances.Where(d => d.BillingType == contItem && d.CompanyId == CompId).ToList();
                                        var prevbal = a.LastOrDefault(x => x.BillingType == contItem).Amount;
                                        SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(prevbal);
                                    }
                                    catch { SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0); }

                                }
                                else
                                {
                                    SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0);
                                }
                            }
                        }
                    }
                }
                var username = User.Identity.GetUserName();
                RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
                ViewBag.IsValidRole = roleAssignmentMatrix.SubsidiaryLedger;
                ViewBag.CompanySelected = "OK";
                ViewBag.TransactionSuccess = "Edit";
                return View("ViewSubsidiaryLedger", SearchSubsidiaryLedgerViewModels);
            }
            return View();
        }

        public ActionResult RemoveSubsidiary(FormCollection frm)
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Deleted Item! (Maintenance Subsidiary Ledger) - from Terminal:" + ipaddress);
            SearchSubsidiaryLedgerViewModel SearchSubsidiaryLedgerViewModels = new SearchSubsidiaryLedgerViewModel();
            SearchSubsidiaryLedgerViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            int parsedID = int.Parse(frm["SubLedgerId"]);
            int CompID = int.Parse(frm["RemoveCompId"]);
            SubsidiaryLedger subsidiary = db.SubsidiaryLedger.Find(parsedID);
            db.SubsidiaryLedger.Remove(subsidiary);
            db.SaveChanges();

            SearchSubsidiaryLedgerViewModels.CompanyList = db.Company.Where(m => m.CompanyID == CompID).ToList();
            SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == CompID).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
            SearchSubsidiaryLedgerViewModels.BalanceList = db.Balances.Select(x => x).ToList();
            var billT = db.SubsidiaryLedger.GroupBy(m => m.BillingType).Select(g => new { BillingType = g.Key }).ToList();
            foreach (var itemss in billT)
            {
                SearchSubsidiaryLedgerViewModels.BillingType.Add(itemss.BillingType.ToUpper().Replace(" ", ""));
            }
            SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Select(x => x).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
            var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
            foreach (var item in bill)
            {
                if (item.Category == "Garbage Fee")
                {
                    var newcat = item.Category.Replace("Fee", "");
                    SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                }
                else if (item.Category == "Rental Fee")
                {
                    var newcat = item.Category.Replace("Fee", "");
                    SearchSubsidiaryLedgerViewModels.Category.Add(newcat.Replace(" ", "").ToUpper());
                }
                else
                {
                    SearchSubsidiaryLedgerViewModels.Category.Add(item.Category.Replace(" ", "").ToUpper());
                }
            }
            foreach (var items in SearchSubsidiaryLedgerViewModels.BillingType)
            {
                if (SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList.Count != 0)
                {
                    for (int z = 0; z < SearchSubsidiaryLedgerViewModels.Category.Count; z++)
                    {
                        if (items.ToUpper() == SearchSubsidiaryLedgerViewModels.Category[z].Replace(" ", ""))
                        {
                            if (items == "ADMINFEE")
                            {
                                TempData["BillType"] = "ADMIN FEE";
                            }
                            else if (items == "PASSEDONBILLING")
                            {
                                TempData["BillType"] = "PASSED ON BILLING";
                            }
                            else if (items == "POLERENTAL")
                            {
                                TempData["BillType"] = "POLE RENTAL";
                            }
                            else
                            {
                                TempData["BillType"] = items;
                            }
                            var contItem = TempData["BillType"].ToString();
                            try
                            {
                                var SumCredit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompID).Sum(y => y.CreditAmount);
                                SearchSubsidiaryLedgerViewModels.Credit.Add(SumCredit);
                                var SumDebit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == CompID).Sum(y => y.DebitAmount);
                                SearchSubsidiaryLedgerViewModels.Debit.Add(SumDebit);
                                var CurrentBalance = SumDebit - SumCredit;
                                SearchSubsidiaryLedgerViewModels.CurrentBalance.Add(CurrentBalance);
                                SearchSubsidiaryLedgerViewModels.CategoryList.Add(contItem);
                            }
                            catch { }
                            if (SearchSubsidiaryLedgerViewModels.BalanceList.Count != 0)
                            {
                                try
                                {
                                    var a = db.Balances.Where(d => d.BillingType == contItem && d.CompanyId == CompID).ToList();
                                    var prevbal = a.LastOrDefault(x => x.BillingType == contItem).Amount;
                                    SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(prevbal);
                                }
                                catch { SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0); }
                            }
                            else
                            {
                                SearchSubsidiaryLedgerViewModels.PreviousBalance.Add(0);
                            }
                        }
                    }
                }
            }
            SearchSubsidiaryLedgerViewModels.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == CompID).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
            ViewBag.CompanySelected = "OK";
            TempData["TransactionSuccess"] = "delete";
            TempData["SearchSubsidiaryLedgerViewModels"] = SearchSubsidiaryLedgerViewModels;
            return RedirectToAction("ViewSubsidiaryLedger", "MaintenanceSubsidiaryLedger");
        }

        public ActionResult FilterBillType(string CompyId, string BillType, string BillRef, string Other)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            SearchSubsidiaryLedgerViewModel searchcompany1 = new SearchSubsidiaryLedgerViewModel();
            searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            List<Company> NewCompanies = new List<Company>();
            if (Other == "")
            {
                Other = null;
            }
            if (BillRef == "")
            {
                BillRef = null;
            }
            int OutParseValue;
            bool CanParse = int.TryParse(CompyId.ToString(), out OutParseValue);
            searchcompany1.BalanceList = db.Balances.Select(x => x).ToList();
            if (CanParse)
            {
                var username = User.Identity.GetUserName();
                RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
                ViewBag.IsValidRole = roleAssignmentMatrix.SubsidiaryLedger;
                ViewBag.CompanySelected = "OK";

                int ParsedCompanyID = int.Parse(CompyId.ToString());
                searchcompany1.SubsidiaryLedgerList = db.SubsidiaryLedger.Where(m => m.CompanyId == ParsedCompanyID && m.BillingType == BillType && m.BillingReference == BillRef && m.Other == Other).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                searchcompany1.SubsidiaryLedgerToList = db.SubsidiaryLedger.Where(m => m.CompanyId == ParsedCompanyID).OrderByDescending(x => x.SubsidiaryLedgerId).ToList();
                searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                var billT = db.SubsidiaryLedger.GroupBy(m => m.BillingType).Select(g => new { BillingType = g.Key }).ToList();
                foreach (var itemss in billT)
                {
                    searchcompany1.BillingType.Add(itemss.BillingType.ToUpper().Replace(" ", ""));
                }
                var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                foreach (var item in bill)
                {
                    if (item.Category == "Garbage Fee")
                    {
                        var newcat = item.Category.Replace("Fee", "");
                        searchcompany1.Category.Add(newcat.Replace(" ", "").ToUpper());
                    }
                    else if (item.Category == "Rental Fee")
                    {
                        var newcat = item.Category.Replace("Fee", "");
                        searchcompany1.Category.Add(newcat.Replace(" ", "").ToUpper());
                    }
                    else
                    {
                        searchcompany1.Category.Add(item.Category.ToUpper());
                    }
                }
                foreach (var items in searchcompany1.BillingType)
                {
                    if (searchcompany1.SubsidiaryLedgerList.Count != 0)
                    {
                        for (int z = 0; z < searchcompany1.Category.Count; z++)
                        {
                            if (items == searchcompany1.Category[z].Replace(" ", ""))
                            {
                                if (items == "ADMINFEE")
                                {
                                    TempData["BillType"] = "ADMIN FEE";
                                }
                                else if (items == "PASSEDONBILLING")
                                {
                                    TempData["BillType"] = "PASSED ON BILLING";
                                }
                                else if (items == "POLERENTAL")
                                {
                                    TempData["BillType"] = "POLE RENTAL";
                                }
                                else
                                {
                                    TempData["BillType"] = items;
                                }
                                var contItem = TempData["BillType"].ToString();
                                try
                                {
                                    var SumCredit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == ParsedCompanyID && x.BillingType == BillType && x.BillingReference == BillRef).Sum(y => y.CreditAmount);
                                    searchcompany1.Credit.Add(SumCredit);
                                    var SumDebit = db.SubsidiaryLedger.Where(x => x.BillingType == contItem && x.CompanyId == ParsedCompanyID && x.BillingType == BillType && x.BillingReference == BillRef).Sum(y => y.DebitAmount);
                                    searchcompany1.Debit.Add(SumDebit);
                                    var CurrentBalance = SumDebit - SumCredit;
                                    searchcompany1.CurrentBalance.Add(CurrentBalance);
                                    searchcompany1.CategoryList.Add(contItem);
                                }
                                catch
                                { }
                                if (searchcompany1.BalanceList.Count != 0)
                                {
                                    try
                                    {
                                        var a = db.Balances.Where(d => d.BillingType == contItem && d.CompanyId == ParsedCompanyID).ToList();
                                        var prevbal = a.LastOrDefault(x => x.BillingType == contItem).Amount;
                                        searchcompany1.PreviousBalance.Add(prevbal);
                                    }
                                    catch { searchcompany1.PreviousBalance.Add(0); }

                                }
                                else
                                {
                                    searchcompany1.PreviousBalance.Add(0);
                                }
                            }
                            //
                        }
                    }
                }
            }
            return View("ViewSubsidiaryLedger", searchcompany1);
        }

        public JsonResult SubsiData(string BillType, int CompId)
        {
            List<MaintenanceSubsidiaryLedgerJsonReturnData> MaintenanceSubsidiaryLedgerJsonReturnData = new List<MaintenanceSubsidiaryLedgerJsonReturnData>();
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            try
            {
                if (BillType.ToUpper() == "ADMIN FEE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.Company.Where(x => x.CompanyID == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        //ReturnData.TransactionNo = db.AdminFee.FirstOrDefault(x => x.Dev_Comp_Code == items.CompanyCode).AdminFeeId.ToString();
                        //ReturnData.TransactionDescription = db.AdminFee.FirstOrDefault(x => x.Dev_Comp_Code == items.CompanyCode).Company_Name;
                        //ReturnData.TransactionNo = db.AdminFee.FirstOrDefault(x => x.Dev_Comp_Code == items.CompanyCode).Company_Name;
                        ReturnData.TransactionNo = items.CompanyCode;
                        ReturnData.TransactionDescription = items.CompanyCode;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "FRANCHISE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.FranchiseFeeInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.FranchiseFeeInformationId.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "GARBAGE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.GarbageInformations.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.GarbageInformationId.ToString();
                        ReturnData.TransactionDescription = items.Type;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "PASSED ON BILLING")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.PassedOnBillingInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.PassedOnBillingInformationId.ToString();
                        ReturnData.TransactionDescription = items.Type;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "POLE RENTAL")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.PoleInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.PoleInformationId.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "RENTAL")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.RentalInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.RentalInformationId.ToString();
                        ReturnData.TransactionDescription = items.Type;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "SEWERAGE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.WaterMeterAssignment.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.MeterNumber.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "WATER")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.WaterMeterAssignment.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.MeterNumber.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
            }
            catch { }
            return Json(null);
        }

        public JsonResult EditSubsiData(string BillType, int CompId)
        {
            List<MaintenanceSubsidiaryLedgerJsonReturnData> MaintenanceSubsidiaryLedgerJsonReturnData = new List<MaintenanceSubsidiaryLedgerJsonReturnData>();
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            try
            {
                if (BillType.ToUpper() == "ADMIN FEE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.Company.Where(x => x.CompanyID == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        //ReturnData.TransactionNo = db.AdminFee.FirstOrDefault(x => x.Dev_Comp_Code == items.CompanyCode).AdminFeeId.ToString();
                        //ReturnData.TransactionDescription = db.AdminFee.FirstOrDefault(x => x.Dev_Comp_Code == items.CompanyCode).Company_Name;
                        ReturnData.TransactionNo = items.CompanyCode;
                        ReturnData.TransactionDescription = items.CompanyCode;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "FRANCHISE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.FranchiseFeeInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.FranchiseFeeInformationId.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "GARBAGE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.GarbageInformations.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.GarbageInformationId.ToString();
                        ReturnData.TransactionDescription = items.Type;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "PASSED ON BILLING")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.PassedOnBillingInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.PassedOnBillingInformationId.ToString();
                        ReturnData.TransactionDescription = items.Type;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "POLE RENTAL")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.PoleInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.PoleInformationId.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "RENTAL")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.RentalInformation.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.RentalInformationId.ToString();
                        ReturnData.TransactionDescription = items.Type;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "SEWERAGE")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.WaterMeterAssignment.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.MeterNumber.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
                else if (BillType.ToUpper() == "WATER")
                {
                    MaintenanceSubsidiaryLedgerJsonReturnData ReturnData = new MaintenanceSubsidiaryLedgerJsonReturnData();
                    var ReSubsi = db.WaterMeterAssignment.Where(x => x.CompanyId == CompId).ToList();
                    foreach (var items in ReSubsi)
                    {
                        ReturnData.TransactionNo = items.MeterNumber.ToString();
                        ReturnData.TransactionDescription = items.StartDate + "-" + items.EndDate;
                        MaintenanceSubsidiaryLedgerJsonReturnData.Add(ReturnData);
                    }
                    return Json(MaintenanceSubsidiaryLedgerJsonReturnData);
                }
            }
            catch { }
            return Json(null);
        }

    }
}