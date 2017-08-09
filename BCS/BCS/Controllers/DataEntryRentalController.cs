using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.SqlClient;
using System.Net;
using System.Configuration;

namespace BCS.Controllers
{
    [Authorize]
    public class DataEntryRentalController : Controller
    {
        private BCS_Context db = new BCS_Context();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        public ActionResult ViewRentalsRPG()
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rentals;
            ViewBag.CompanySelected = "OK";
            SearchCompany temp = TempData["SearchCompany"] as SearchCompany;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("SearchCompany");
            return View("ViewRentals", temp);
        }

        [HttpGet]
        public ActionResult ViewRentals()
        {
            SearchCompany searchcompany = new SearchCompany();
            return View(searchcompany);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewRentals(string SearchInput, FormCollection frm, RentalInformation rentalinfo, string isEdit)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.FirstOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rentals;
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                SearchCompany searchcompany1 = new SearchCompany();
                searchcompany1.SearchInput = SearchInput.ToString();
                List<Company> NewCompanies = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                //searchcompany1.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%' and ZoneGroup").ToList();

                ViewBag.CompanySelected = "";
                return View(searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                SearchCompany searchcompany1 = new SearchCompany();

                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    searchcompany1.RentalInformationList = db.RentalInformation.Where(m => m.CompanyId == ParsedCompanyID).ToList();
                    searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                    searchcompany1.BillingRate = db.BillingRates.Where(m => m.Category == "Rental Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();

                    var bill = searchcompany1.BillingRate.GroupBy(m => m.SubCategory).ToList();
                    foreach (var item in bill)
                    {
                        searchcompany1.SubCategory.Add(item.Key.ToString());
                    }
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Rental - Search Company  - from Terminal: " + ipaddress);
                    ViewBag.CompanySelected = "OK";
                }
                else
                {
                    ViewBag.CompanySelected = "";
                }


                return View(searchcompany1);
            }
            else if (isEdit == "True") //Edit transaction
            {
                var area = frm["Area"].Split(',');
                var newArea = "";
                foreach (var item in area)
                {
                    newArea = newArea + item;
                }
                RentalInformation rent = new RentalInformation();
                int CompanyID = int.Parse(frm["CompanyID"].ToString());
                //TryUpdateModel(rent);
                //if (ModelState.IsValid)
                //{
                RentalInformation rentinfo = null;
                int ParsedIntRentID = int.Parse(frm["rentID"]);
                rentinfo = db.RentalInformation.Find(ParsedIntRentID);

                if (rentinfo != null)
                {
                    rentinfo.Type = frm["Type"].ToString();
                    rentinfo.Rate = decimal.Parse(frm["Rate"]);
                    rentinfo.Area = Math.Round(decimal.Parse(newArea), 2);
                    rentinfo.Amount = Math.Round(decimal.Parse(frm["Rate"]) * decimal.Parse(frm["Area"]), 2);
                    rentinfo.StartDate = DateTime.Parse(frm["StartDate"].ToString());
                    rentinfo.EndDate = DateTime.Parse(frm["EndDate"].ToString());
                    rentinfo.BillMode = frm["BillMode"].ToString();
                    rentinfo.DueOn = int.Parse(frm["DueOn"]);
                    rentinfo.BillingMonths = frm["billingMonths"];
                    //rentinfo.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
                    rentinfo.Currency = frm["Currency"].ToString();
                    rentinfo.UpdateDate = DateTime.Now;
                    rentinfo.UpdatedBy = userid;
                    db.Entry(rentinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.ValidatePostback = "True";
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Rental - Edit  - from Terminal: " + ipaddress);
                    TempData["TransactionSuccess"] = "Edit";
                    //ViewBag.TransactionSuccess = "Edit";
                }
                //}
                else
                {
                    ViewBag.ValidatePostback = "False";
                }

                SearchCompany searchcompany = new SearchCompany();
                searchcompany.RentalInformationList = db.RentalInformation.Where(m => m.CompanyId == CompanyID).ToList();
                searchcompany.CompanyList = db.Company.Where(m => m.CompanyID == CompanyID).ToList();
                searchcompany.BillingRate = db.BillingRates.Where(m => m.Category == "Rental Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();

                var bill = searchcompany.BillingRate.GroupBy(m => m.SubCategory).ToList();

                foreach (var item in bill)
                {
                    searchcompany.SubCategory.Add(item.Key);
                }
                ViewBag.CompanySelected = "OK";

                TempData["SearchCompany"] = searchcompany;
                return RedirectToAction("ViewRentalsRPG", "DataEntryRental");
                //return View(searchcompany);
            }
            //If returned key/value pairs is > 3. "Add transaction is invoked
            else if (frm.Count > 3)
            {
                int OutIntParse;
                decimal OutDecimalParse;
                bool canParse = int.TryParse(frm["CompanyID"].ToString(), out OutIntParse);
                int parsedCompanyID = (canParse) ? int.Parse(frm["CompanyID"].ToString()) : 0;
                //int ParsedIntRentID = int.Parse(frm["RentalInformationId"]);
                RentalInformation rent = new RentalInformation();
                TryUpdateModel(rent);
                //var errors = ModelState.Values.SelectMany(v => v.Errors);
                //UpdateModel<IRentalInformation>(rent);
                //if (ModelState.IsValid)
                //{
                try
                {
                    var r = frm["Rate"].ToString();
                    var s = r.Split(',');
                    var newRate = "";
                    foreach (var item in s)
                    {
                        newRate += item;
                    }
                    decimal rate = Math.Round((decimal.TryParse(newRate, out OutDecimalParse)) ? decimal.Parse(newRate) : 0, 2);
                    decimal area = Math.Round((decimal.TryParse(frm["Area"].ToString(), out OutDecimalParse)) ? decimal.Parse(frm["Area"].ToString()) : 0, 2);
                    decimal amt = Math.Round(rate * area, 2);

                    int RentalBillMode = (int.TryParse(frm["BillMode"].ToString(), out OutIntParse)) ? int.Parse(frm["BillMode"].ToString()) : 0;

                    RentalInformation rentalinformation = new RentalInformation();

                    rentalinformation.CompanyId = parsedCompanyID;
                    rentalinformation.Type = frm["Type"].ToString();
                    rentalinformation.Rate = rate;
                    rentalinformation.Area = area;

                    string str = frm["StartDate"].ToString();

                    rentalinformation.StartDate = Convert.ToDateTime(frm["StartDate"].ToString());
                    rentalinformation.EndDate = Convert.ToDateTime(frm["EndDate"].ToString());
                    //rentalinformation.BillMode = (int.TryParse(frm["BillMode"].ToString(), out OutIntParse)) ? int.Parse(frm["BillMode"].ToString()) : 0;
                    rentalinformation.BillMode = frm["BillMode"].ToString();
                    rentalinformation.DueOn = (int.TryParse(frm["DueOn"].ToString(), out OutIntParse)) ? int.Parse(frm["DueOn"].ToString()) : 0;

                    rentalinformation.BillingMonths = frm["billingMonths"];
                    //rentalinformation.BillingMonths = "1,2,3,4,5,6,7,8,9,10,11,12";
                    rentalinformation.CreatedDate = DateTime.Now;
                    rentalinformation.Amount = amt;
                    rentalinformation.Currency = frm["Currency"].ToString();
                    rentalinformation.CreatedBy = userid;

                    db.RentalInformation.Add(rentalinformation);
                    db.SaveChanges();
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Rental - Add - from Terminal: " + ipaddress);
                    TempData["TransactionSuccess"] = "Add";
                    //ViewBag.TransactionSuccess = "Add";
                    ViewBag.ValidatePostback = "True";
                }
                catch (Exception)
                {
                    ViewBag.ValidatePostback = "False";
                }
                //}
                //else
                //{
                //    ViewBag.ValidatePostback = "False";
                //}


                ViewBag.CompanySelected = "OK";
                SearchCompany searchcompany = new SearchCompany();
                searchcompany.RentalInformationList = db.RentalInformation.Where(m => m.CompanyId == parsedCompanyID).ToList();
                searchcompany.CompanyList = db.Company.Where(m => m.CompanyID == parsedCompanyID).ToList();
                searchcompany.BillingRate = db.BillingRates.Where(m => m.Category == "Rental Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();

                var bill = searchcompany.BillingRate.GroupBy(m => m.SubCategory).ToList();

                foreach (var item in bill)
                {
                    searchcompany.SubCategory.Add(item.Key);
                }

                TempData["SearchCompany"] = searchcompany;
                return RedirectToAction("ViewRentalsRPG", "DataEntryRental");
                //return View(searchcompany);
            }
            //Default value
            else
            {
                SearchCompany searchcompany1 = new SearchCompany();
                return View(searchcompany1);
            }
        }


        // Generate Report
        public ActionResult RentalReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Rental - View Report  - from Terminal: " + ipaddress);
            //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);

            //Uri serverURI = new Uri(serverURL);
            //string reportPath = "%2fDataEntries%2fRentalAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            //System.UriBuilder uriBuilder = new System.UriBuilder(serverURI);
            //uriBuilder.Path += reportPath;
            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fRentalAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }
    }
}

//public ActionResult DeleteRental()
//{
//    ApplicationDbContext context = new ApplicationDbContext();
//    var userid = User.Identity.GetUserId();
//    string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
//    RentalInformation rentinfo = null;
//    rentinfo = db.RentalInformation.Find(2);
//    if (rentinfo != null)
//    {
//        db.RentalInformation.Remove(rentinfo);
//        db.SaveChanges();
//    }
//    else
//    {
//        throw new Exception("Invalid transaction.");
//    }

//    //int OutIntParse;
//    //bool canParse = int.TryParse(frmcollection["CompanyID"].ToString(), out OutIntParse);
//    //int parsedCompanyID = (canParse) ? int.Parse(frmcollection["CompanyID"].ToString()) : 0;
//    int parsedCompanyID = 1;
//    SearchCompany searchcompany = new SearchCompany();
//    searchcompany.RentalInformationList = db.RentalInformation.Where(m => m.CompanyId == parsedCompanyID).ToList();
//    searchcompany.CompanyList = db.Company.Where(m => m.CompanyID == parsedCompanyID).ToList();
//    searchcompany.BillingRate = db.BillingRates.Where(m => m.Category == "Rental Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();
//    var bill = searchcompany.BillingRate.GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

//    foreach (var item in bill)
//    {
//        searchcompany.SubCategory.Add(item.SubCategory);
//    }
//    return RedirectToAction("ViewRentals", "DataEntryRental", new { searchcompany });
//}

//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult UpdateRental(FormCollection frmcollection)
//{
//    ApplicationDbContext context = new ApplicationDbContext();
//    var userid = User.Identity.GetUserId();
//    string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

//    var username = User.Identity.GetUserName();
//    RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
//    ViewBag.IsValidRole = roleAssignmentMatrix.Rentals;

//    RentalInformation rent = new RentalInformation();
//    TryUpdateModel(rent);
//    if (ModelState.IsValid)
//    {
//        RentalInformation rentinfo = null;
//        int ParsedIntRentID = int.Parse(frmcollection["RentalInformationID"]);
//        rentinfo = db.RentalInformation.Find(ParsedIntRentID);

//        if (rentinfo != null)
//        {
//            rentinfo.Type = frmcollection["Type"].ToString();
//            rentinfo.Rate = decimal.Parse(frmcollection["Rate"]);
//            rentinfo.Area = decimal.Parse(frmcollection["Area"]);
//            rentinfo.StartDate = DateTime.Parse(frmcollection["StartDate"].ToString());
//            rentinfo.EndDate = DateTime.Parse(frmcollection["EndDate"].ToString());
//            rentinfo.BillMode = frmcollection["BillMode"];
//            rentinfo.DueOn = int.Parse(frmcollection["DueOn"]);
//            rentinfo.BillingMonths = frmcollection["BillingMonths"];
//            rentinfo.Currency = frmcollection["Currency"].ToString();
//            rentinfo.UpdatedBy = userid;
//            rentinfo.UpdateDate = DateTime.Now;
//            db.Entry(rentinfo).State = System.Data.Entity.EntityState.Modified;
//            db.SaveChanges();
//        }
//    }            

//    //int OutIntParse;
//    //bool canParse = int.TryParse(frmcollection["CompanyID"].ToString(), out OutIntParse);
//    //int parsedCompanyID = (canParse) ? int.Parse(frmcollection["CompanyID"].ToString()) : 0;
//    int parsedCompanyID = int.Parse(frmcollection["CompanyID"]);
//    SearchCompany searchcompany = new SearchCompany();
//    searchcompany.RentalInformationList = db.RentalInformation.Where(m => m.CompanyId == parsedCompanyID).ToList();
//    searchcompany.CompanyList = db.Company.Where(m => m.CompanyID == parsedCompanyID).ToList();
//    searchcompany.BillingRate = db.BillingRates.Where(m => m.Category == "Rental Fee").Where(m => m.ZoneGroup == ZoneGroup).ToList();

//    var bill = searchcompany.BillingRate.GroupBy(m => m.SubCategory).ToList();

//    foreach (var item in bill)
//    {
//        searchcompany.SubCategory.Add(item.Key);
//    }

//    return View("ViewRentals", searchcompany);
//}