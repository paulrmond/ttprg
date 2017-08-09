using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Configuration;

namespace BCS.Controllers
{
    public class MaintenanceCompanyController : Controller
    {

        private BCS_Context db = new BCS_Context();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        // Initial GET BillingRates
        [HttpGet]
        public ActionResult ViewCompany()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<Company> NewCompanies = new List<Company>();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int SingleZoneGroupId = db.ZoneGroup.FirstOrDefault(d => d.ZoneGroupCode == ZoneGroup).ZoneGroupId;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            ViewBag.SelectedCompany = TempData["SelectedCompany"] as string;
            SearchCompany temp = TempData["searchcompany"] as SearchCompany;
            SearchCompany SearchCompanyViewModels = new SearchCompany();
            NewCompanies = db.Company.Select(x => x).OrderBy(x => x.CompanyName).Take(10).ToList();
            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
            SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;

            var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
            foreach (var item in bill)
            {
                SearchCompanyViewModels.Category.Add(item.Category);
            }
            SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
            if (ViewBag.SelectedCompany == null)
            {
                return View(SearchCompanyViewModels);
            }
            else
            {
                return View("ViewCompany", temp);
            }
        }

        // Search Company or Initiate Add Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCompany(string SearchInput, string Rate, FormCollection frm)
        {
            SearchCompany searchcompany = new SearchCompany();
            SearchCompany searchcompany1 = new SearchCompany();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int SingleZoneGroupId = db.ZoneGroup.FirstOrDefault(d => d.ZoneGroupCode == ZoneGroup).ZoneGroupId;

            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                searchcompany1.SearchInput = SearchInput.ToString();
                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.Where(x => x.CompanyName.Contains(SearchInput)).ToList().OrderBy(x => x.CompanyName).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                searchcompany1.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                ViewBag.SelectedCompany = "COMPSHOW";
                return View(searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    List<Company> NewCompanies = new List<Company>();
                    NewCompanies = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                }
                var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                foreach (var item in bill)
                {
                    searchcompany1.Category.Add(item.Category);
                }
                searchcompany1.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                ViewBag.SelectedCompany = "COMPHIDE";
                return View(searchcompany1);
            }
            //If returned key/value pairs is > 3. "Add transaction is invoked
            else if (frm.Count > 3)
            {
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Maintenance Company) - from Terminal:" + ipaddress);
                Company companyinfo = new Company();
                var CompCode = frm["CompanyCode"].ToString();
                companyinfo.CompanyCode = frm["CompanyCode"].ToString();
                companyinfo.CompanyName = frm["CompanyName"].ToString();
                companyinfo.Phase = frm["Phase"].ToString();
                companyinfo.Address = frm["Address"].ToString();
                companyinfo.CreateDate = DateTime.Now.Date;
                companyinfo.Status = frm["Status"].ToString();
                companyinfo.OwnershipType = frm["OwnershipType"].ToString();
                companyinfo.EnterpriseType = frm["EnterpriseType"].ToString();
                searchcompany1.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                searchcompany1.BillingRateList = db.BillingRates.Select(z => z).ToList();

                var vat = frm["Vatable"].ToString();
                if (vat == "YES")
                {
                    companyinfo.VatableItems = "YES";
                    companyinfo.Vat = 12;
                }
                else
                {
                    companyinfo.VatableItems = "NO";
                }
                try
                {
                    if (searchcompany1.ZoneList.Count != 0)
                    {
                        companyinfo.ZoneCode = frm["ZoneCode"].ToString();
                    }
                }
                catch
                {
                }
                try
                {
                    if (searchcompany1.BillingRateList.Count != 0)
                    {
                        companyinfo.WithHolding = frm["WithHolding"].ToString();
                    }
                }
                catch
                {
                }
                companyinfo.SendEmail = frm["SendEmail"].ToString();
                companyinfo.PrimaryEmailAddress = frm["AddEmailAddress"].ToString();
                companyinfo.SecondaryEmailAddress = frm["AddSecondaryEmailAddress"].ToString();
                try
                {
                    companyinfo.DateOfRegistration = Convert.ToDateTime(frm["DateOfRegistration"].ToString());
                }
                catch { }
                db.Company.Add(companyinfo);
                db.SaveChanges();

                var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                foreach (var item in bill)
                {
                    searchcompany.Category.Add(item.Category);
                }
                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.Select(x => x).OrderByDescending(x => x.CompanyID).Take(50).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchcompany.CompanyList = searchCompanyPerGroup.Companies;
                searchcompany.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                TempData["TransactionSuccess"] = "Add";
                TempData["searchcompany"] = searchcompany;
                TempData["SelectedCompany"] = "COMPHIDE";
                return RedirectToAction("ViewCompany", "MaintenanceCompany");
            }
            else
            {
                Response.Write("<script> alert('Invalid Transaction!') </script>");
            }
            //Default value        
            //searchcompany.CompanyList = searchcompany.CompanyList.Select(x => x).ToList();
            return View("ViewCompany");
            //return View();
        }

        // Search / Filter Company
        public ActionResult ViewSelectFilter(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int SingleZoneGroupId = db.ZoneGroup.FirstOrDefault(d => d.ZoneGroupCode == ZoneGroup).ZoneGroupId;
            SearchCompany SearchCompanyViewModels = new SearchCompany();
            List<Company> NewCompanies = new List<Company>();
            if (frm.Count >= 1)
            {
                if (frm["SelectFilter"] == "All")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched all items! (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Select(x => x).OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Service Enterprise")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Service Enterprise (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Service Enterprise").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Logistics")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Logistics (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Logistics").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Utilities")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Utilities (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Utilities").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Regional Warehouse")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Warehouse (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Regional Warehouse").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Export Enterprise")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Export Enterprise (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Export Enterprise").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Domestic Market")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Domestic Market (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Domestic Market").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "IT Enterprise")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by IT Enterprise (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "IT Enterprise").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Facilities Enterprise")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Facilities Enterprise (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Facilities Enterprise").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Tourism Enterprise")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Tourism Enterprise (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Tourism Enterprise").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Agro-Industrial")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Agro-Industrial (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Agro-Industrial").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Developer Enterprise")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Developer Enterprise (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Developer Enterprise").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Free Trade")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Free Trade (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Free Trade").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Medical Tourism")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Medical Tourism (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Medical Tourism").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "PEZA Employees/Tenants")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by PEZA Employees/Tenants (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "PEZA Employees/Tenants").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else if (frm["SelectFilter"] == "Unknown")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Unknown (Maintenance Company) - from Terminal:" + ipaddress);
                    NewCompanies = db.Company.Where(x => x.EnterpriseType == "Unknown").OrderBy(x => x.CompanyName).ToList();
                    SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                    SearchCompanyViewModels.CompanyList = searchCompanyPerGroup.Companies;
                    SearchCompanyViewModels.BillingRateList = db.BillingRates.SqlQuery("Select Distinct * from BillingRates").ToList();
                    var bill = db.BillingRates.GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                    foreach (var item in bill)
                    {
                        SearchCompanyViewModels.Category.Add(item.Category);
                    }
                    ViewBag.SelectedCompany = "COMPSHOW";
                    SearchCompanyViewModels.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                }
                else
                {
                    return View();
                }
            }
            return View("ViewCompany", SearchCompanyViewModels);
        }

        // Update Company Data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCompany(FormCollection frmcollection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            int SingleZoneGroupId = db.ZoneGroup.FirstOrDefault(d => d.ZoneGroupCode == ZoneGroup).ZoneGroupId;
            Company compinfo = null;
            int ParsedIntCompanyID = int.Parse(frmcollection["CompanyID"]);
            compinfo = db.Company.Find(ParsedIntCompanyID);
            SearchCompany searchcompany1 = new SearchCompany();
            List<Company> NewCompanies = new List<Company>();
            if (compinfo != null)
            {
                //compinfo.CompanyCode = frmcollection["CompanyCode"].ToString();
                compinfo.CompanyName = frmcollection["CompanyName"].ToString();
                compinfo.Phase = frmcollection["Phase"].ToString();
                compinfo.Address = frmcollection["Address"].ToString();
                searchcompany1.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
                searchcompany1.BillingRateList = db.BillingRates.Select(z => z).ToList();
                compinfo.UpdateDate = DateTime.Now.Date;
                compinfo.Status = frmcollection["Status"].ToString();
                compinfo.OwnershipType = frmcollection["EditOwnershipType"].ToString();
                compinfo.EnterpriseType = frmcollection["EnterpriseType"].ToString();
                compinfo.SendEmail = frmcollection["SendEmail"].ToString();
                compinfo.PrimaryEmailAddress = frmcollection["EmailAddress"].ToString();
                compinfo.SecondaryEmailAddress = frmcollection["SecondaryEmailAddress"].ToString();
                if (frmcollection["Vatable"].ToString() == "YES")
                {
                    compinfo.Vat = 12;
                    compinfo.VatableItems = frmcollection["Vatable"].ToString();
                }
                else if (frmcollection["Vatable"].ToString() == "NO")
                {
                    compinfo.VatableItems = "NO";
                }
                else
                {
                    compinfo.VatableItems = "";
                }
                try
                {
                    compinfo.DateOfRegistration = Convert.ToDateTime(frmcollection["EditDateOfRegistration"].ToString());
                }
                catch { }
                if (searchcompany1.ZoneList.Count != 0)
                {
                    compinfo.ZoneCode = frmcollection["ZoneCode"].ToString();
                }
                try
                {
                    if (searchcompany1.BillingRateList.Count != 0)
                    {
                        compinfo.WithHolding = frmcollection["WithHolding"].ToString();
                    }
                }
                catch (Exception)
                {
                    //compinfo.VatableItems = null;
                    compinfo.WithHolding = null;
                    db.Entry(compinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                finally
                {
                    db.Entry(compinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                Response.Write("<script> alert('Update Failed!') </script>");
                TempData["IsSuccess"] = false;
            }
            int parsedCompanyID = int.Parse(frmcollection["CompanyID"]);
            SearchCompany searchcompany = new SearchCompany();
            int CompId = Convert.ToInt32(frmcollection["CompanyID"]);
            //NewCompanies = db.Company.Select(x => x).OrderBy(x => x.CompanyName).Take(10).ToList();
            NewCompanies = db.Company.Where(x => x.CompanyID == CompId).ToList();
            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
            searchcompany.CompanyList = searchCompanyPerGroup.Companies;
            searchcompany.ZoneList = db.Zone.Where(x => x.ZoneGroup == SingleZoneGroupId.ToString()).ToList();
            TempData["IsSuccess"] = true;
            TempData["TransactionSuccess"] = "Edit";
            TempData["searchcompany"] = searchcompany;
            TempData["SelectedCompany"] = "COMPHIDE";
            return RedirectToAction("ViewCompany", "MaintenanceCompany");
        }

        // Generate Report
        public ActionResult CompanyReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Maintenance Company- View Report  - from Terminal: " + ipaddress);
            //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fMaintenance%2fCompanyAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }

    }
}