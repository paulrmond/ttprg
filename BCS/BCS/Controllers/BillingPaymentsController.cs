using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BCS.Controllers
{
    public class BillingPaymentsController : Controller
    {
        // GET: BillingPayment
        private BCS_Context db = new BCS_Context();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        public object SqlMethods { get; private set; }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public PartialViewResult ViewPaymentsType(string type)
        {

            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            SearchOrderOfPaymentViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails").ToList();
            SearchOrderOfPaymentViewModels.ZoneList = db.Zone.Select(x => x).ToList();


            if (type == "ALL")
            {
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus != "DELETED").Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();

            }
            else if (type == "PAID")
            {
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus == "PAID").Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();

            }
            else if (type == "UNPAID")
            {
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus == "UNPAID" || x.PaymentStatus == "").Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();

            }
            else if (type == "EXPIRED")
            {
                DateTime cutoffPoint = DateTime.Now.AddDays(-60);
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus == "UNPAID" && x.OPDate <= cutoffPoint).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();

            }
            else
            {
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();

            }
            foreach (var item in SearchOrderOfPaymentViewModels.OrderOfPaymentList)
            {
                SearchOrderOfPaymentViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                if (SearchOrderOfPaymentViewModels.CompanyList.Count == 1)
                {
                    foreach (var items in SearchOrderOfPaymentViewModels.CompanyList)
                    {
                        SearchOrderOfPaymentViewModels.CompanyName.Add(items.CompanyName);
                        SearchOrderOfPaymentViewModels.Address.Add(items.Address);
                        SearchOrderOfPaymentViewModels.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                    }
                }
            }
            // --- END --- //
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            //return View(SearchOrderOfPaymentViewModels);

            return PartialView("~/Views/Shared/OoPayment/All.cshtml", SearchOrderOfPaymentViewModels);

            // return PartialView("")
        }



        // Initial Display Value for Order Of Payment
        public ActionResult ViewPayments(string dexist)
        {

            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            SearchOrderOfPaymentViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails").ToList();
            SearchOrderOfPaymentViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus != "DELETED").Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();



            DateTime cutoffPoint = DateTime.Now.AddDays(-60);
            double xx = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus == "UNPAID" && x.OPDate <= cutoffPoint).Take(200).OrderByDescending(z => z.OrderOfPaymentId).Count();


            foreach (var item in SearchOrderOfPaymentViewModels.OrderOfPaymentList)
            {
                SearchOrderOfPaymentViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                if (SearchOrderOfPaymentViewModels.CompanyList.Count == 1)
                {
                    foreach (var items in SearchOrderOfPaymentViewModels.CompanyList)
                    {
                        SearchOrderOfPaymentViewModels.CompanyName.Add(items.CompanyName);
                        SearchOrderOfPaymentViewModels.Address.Add(items.Address);
                        SearchOrderOfPaymentViewModels.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                    }
                }
            }
            // --- END --- //
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            if (xx > 0)
            {
                ViewBag.ExpiredItems = xx;

            }
            return View(SearchOrderOfPaymentViewModels);
        }

        public ActionResult ViewPaymentsexpired(string dexist)
        {

            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            SearchOrderOfPaymentViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails").ToList();
            SearchOrderOfPaymentViewModels.ZoneList = db.Zone.Select(x => x).ToList();

            DateTime cutoffPoint = DateTime.Now.AddDays(-60);
            SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode && x.PaymentStatus == "UNPAID" && x.OPDate <= cutoffPoint).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();

            foreach (var item in SearchOrderOfPaymentViewModels.OrderOfPaymentList)
            {
                SearchOrderOfPaymentViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                if (SearchOrderOfPaymentViewModels.CompanyList.Count == 1)
                {
                    foreach (var items in SearchOrderOfPaymentViewModels.CompanyList)
                    {
                        SearchOrderOfPaymentViewModels.CompanyName.Add(items.CompanyName);
                        SearchOrderOfPaymentViewModels.Address.Add(items.Address);
                        SearchOrderOfPaymentViewModels.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                    }
                }
            }
            // --- END --- //
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            return View(SearchOrderOfPaymentViewModels);
        }


        [HttpPost]
        public ActionResult DeleteAllOldOP()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            //x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode
            DateTime cutoffPoint = DateTime.Now.AddDays(-60);
            var query = from ord in db.OrderOfPayment
                        where ord.DivisionCode == DivisionCode && ord.ZoneGroupcode == ZoneGroupCode && ord.PaymentStatus == "UNPAID" && ord.OPDate <= cutoffPoint
                        select ord;

            foreach (OrderOfPayment ord in query)
            {
                ord.PaymentStatus = "DELETED";

                // Insert any additional changes to column values.
            }

            // Submit the changes to the database.
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

                //Console.WriteLine(e);
                // Provide for exceptions.
            }

            return RedirectToAction("ViewPayments");
        }

        // GET: Searched Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewPayments(string SearchInput, FormCollection frm)
        {
            List<Company> NewCompanies = new List<Company>();
            ApplicationDbContext context = new ApplicationDbContext();
            SearchMainOrderOfPaymentViewModel searchcompany1 = new SearchMainOrderOfPaymentViewModel();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                searchcompany1.SearchInput = SearchInput.ToString();
                // --- Append Company Name / Address to Main List --- //
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "' ORDER BY OrderOfPaymentId DESC").Take(200).ToList();
                foreach (var item in searchcompany1.OrderOfPaymentList)
                {
                    searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchcompany1.CompanyListTwo.Count == 1)
                    {
                        foreach (var items in searchcompany1.CompanyListTwo)
                        {
                            searchcompany1.CompanyName.Add(items.CompanyName);
                            searchcompany1.Address.Add(items.Address);
                            searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        }
                    }
                }
                // --- END --- //
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;


                return View(searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                // --- Append Company Name / Address to Main List --- //
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "' ORDER BY OrderOfPaymentId DESC").Take(200).ToList();
                foreach (var item in searchcompany1.OrderOfPaymentList)
                {
                    searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchcompany1.CompanyListTwo.Count == 1)
                    {
                        foreach (var items in searchcompany1.CompanyListTwo)
                        {
                            searchcompany1.CompanyName.Add(items.CompanyName);
                            searchcompany1.Address.Add(items.Address);
                            searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        }
                    }
                }
                // --- END --- //
                // --- Append Company Name / Address to Main List --- //
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "' ORDER BY OrderOfPaymentId DESC").Take(200).ToList();
                foreach (var item in searchcompany1.OrderOfPaymentList)
                {
                    searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchcompany1.CompanyListTwo.Count == 1)
                    {
                        foreach (var items in searchcompany1.CompanyListTwo)
                        {
                            searchcompany1.CompanyName.Add(items.CompanyName);
                            searchcompany1.Address.Add(items.Address);
                            searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        }
                    }
                }
                // --- END --- //    
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Payments - View Companies Searched  - from Terminal: " + ipaddress);
                return View(searchcompany1);
            }
            //Default value
            else
            {
                SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
                // ---- Generate 9 Characters for Reference Number ----
                if (SearchOrderOfPaymentViewModels.OrderOfPaymentList.Count == 0)
                {
                    SearchOrderOfPaymentViewModels.AutoOPNumber = 00000001;
                    string year = Convert.ToString(DateTime.Now.Year);
                    string latestyear = year.Substring(2);
                    SearchOrderOfPaymentViewModels.AutoReferenceNumber = "9" + ZoneGroupCode + latestyear + "0001";
                }
                // ----
                // --- Append Company Name / Address to Main List --- //
                //SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "' ORDER BY OrderOfPaymentId DESC").ToList();
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();
                foreach (var item in SearchOrderOfPaymentViewModels.OrderOfPaymentList)
                {
                    SearchOrderOfPaymentViewModels.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (SearchOrderOfPaymentViewModels.CompanyListTwo.Count == 1)
                    {
                        foreach (var items in SearchOrderOfPaymentViewModels.CompanyListTwo)
                        {
                            SearchOrderOfPaymentViewModels.CompanyName.Add(items.CompanyName);
                            SearchOrderOfPaymentViewModels.Address.Add(items.Address);
                            SearchOrderOfPaymentViewModels.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        }
                    }
                }
                // --- END --- //
                return View(SearchOrderOfPaymentViewModels);
            }
        }

        // Display Order of Payment
        public ActionResult DisplayOrderOfPayment(FormCollection frmcollection)
        {
            if (frmcollection.Count == 0)
            {
                SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
                string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
                SearchOrderOfPaymentViewModels.ZoneList = db.Zone.Select(x => x).ToList();
                //SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "' ORDER BY OrderOfPaymentId DESC").ToList();
                SearchOrderOfPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();
                return View(SearchOrderOfPaymentViewModels);
            }
            return View();
        }

        // Delete Order of Payment
        public ActionResult RemoveOrderOfPayment(FormCollection frm)
        {
            int parsedID = int.Parse(frm["OrderOfPaymentId"]);
            int parsedDetail = Convert.ToInt32(frm["RemoveCompanyId"]);
            OrderOfPayment removeop = db.OrderOfPayment.Find(parsedID);
            db.OrderOfPaymentDetail.RemoveRange(db.OrderOfPaymentDetail.Where(x => x.CompanyId == parsedDetail));
            db.OrderOfPayment.Remove(removeop);
            db.SaveChanges();
            TempData["TransactionSuccess"] = "delete";
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Deleted Item! (Billing Payments Order of Payments) - from Terminal:" + ipaddress);
            return RedirectToAction("ViewPayments");
        }

        // Display Order Of Payment Detail
        public ActionResult DisplayOrderOfPaymentDetail(int CompanyId, int? opid1, FormCollection frmcollection)
        {
            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            SearchOrderOfPaymentDetailyViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            if (frmcollection.Count <= 2)
            {
                if (CompanyId != 0)
                {
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentId ='" + opid1 + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompanyId + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments Where OrderOfPaymentId ='" + opid1 + "' AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();


                    //SearchOrderOfPaymentDetailyViewModels.UserName = 


                    foreach (var item in SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList)
                    {
                        SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "'").ToList();
                        if (SearchOrderOfPaymentDetailyViewModels.OPAccountList.Count == 1)
                        {
                            foreach (var items in SearchOrderOfPaymentDetailyViewModels.OPAccountList)
                            {
                                SearchOrderOfPaymentDetailyViewModels.OPAccountDesc.Add(items.OPAccountDescription);
                            }
                        }
                    }
                    foreach (var item in SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList)
                    {
                        SearchOrderOfPaymentDetailyViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (SearchOrderOfPaymentDetailyViewModels.CompanyList.Count == 1)
                        {
                            foreach (var items in SearchOrderOfPaymentDetailyViewModels.CompanyList)
                            {
                                SearchOrderOfPaymentDetailyViewModels.CompanyName.Add(items.CompanyName);
                                SearchOrderOfPaymentDetailyViewModels.Address.Add(items.Address);
                            }
                        }
                    }
                    if (SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList.Count >= 1)
                    {
                        SearchOrderOfPaymentDetailyViewModels.TotalQuantity = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == opid1).Sum(c => c.Quantity);
                        SearchOrderOfPaymentDetailyViewModels.GrandTotal = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == opid1).Sum(c => c.TotalAmount);
                    }
                    SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).ToList();

                    var usern = SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList.Select(x => x.CreatedBy).ToList();
                    string fname = "";
                    string mname = "";
                    string lname = "";
                    foreach (var item in usern)
                    {
                        fname = context.Users.SingleOrDefault(m => m.Id == item).GivenName;
                        mname = context.Users.SingleOrDefault(m => m.Id == item).MiddleName;
                        lname = context.Users.SingleOrDefault(m => m.Id == item).LastName;

                    }

                    SearchOrderOfPaymentDetailyViewModels.UserName = fname + " " + mname + " " + lname;


                    return View("ViewPaymentDetails", SearchOrderOfPaymentDetailyViewModels);
                }
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Payments - Display Order of payment details  - from Terminal: " + ipaddress);
            return RedirectToAction("ViewPayments");
        }

        // Delete Order Of Payment Detail
        public ActionResult RemoveOrderOfPaymentDetail(int CompanyIds, FormCollection frm)
        {
            int parsedID = int.Parse(frm["OrderOfPaymentDetailId"]);
            OrderOfPaymentDetail removeopd = db.OrderOfPaymentDetail.Find(parsedID);
            db.Detail.RemoveRange(db.Detail.Where(x => x.OrderOfPaymentDetailId == parsedID));
            db.OrderOfPaymentDetail.Remove(removeopd);
            db.SaveChanges();
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Deleted Item! (Billing Payments Order of Payment Details) - from Terminal:" + ipaddress);
            SearchMainOrderOfPaymentViewModel SearchPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
            SearchPaymentViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails").ToList();
            OrderOfPayment opdinfo1 = null;
            int ParsedIntID1 = int.Parse(frm["RemoveOPId"]);
            opdinfo1 = db.OrderOfPayment.Find(ParsedIntID1);
            if (opdinfo1 != null)
            {
                if (SearchPaymentViewModels.OrderOfPaymentDetailList.Count >= 1)
                {
                    try
                    {
                        SearchPaymentViewModels.TotalQuantity = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == ParsedIntID1).Sum(c => c.Quantity);
                        SearchPaymentViewModels.GrandTotal = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == ParsedIntID1).Sum(y => y.TotalAmount);
                        opdinfo1.TotalAmount = SearchPaymentViewModels.GrandTotal;
                        db.Entry(opdinfo1).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch
                    {
                        opdinfo1.TotalAmount = decimal.Zero;
                        db.Entry(opdinfo1).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (SearchPaymentViewModels.OrderOfPaymentDetailList.Count == 0)
                {
                    opdinfo1.TotalAmount = decimal.Zero;
                    db.Entry(opdinfo1).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            TempData["TransactionSuccess"] = "delete";

            return RedirectToAction("ViewPaymentDetails", new { CompanyId = CompanyIds, opid1 = ParsedIntID1 });
        }

        public ActionResult AddSearchOPAccount(string SearchInput, FormCollection frmcollection)
        {
            SearchMainOrderOfPaymentViewModel searchOPAccounts = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                int CompId = Convert.ToInt32(frmcollection["CompanyName1"].ToString());
                int OPId = Convert.ToInt32(frmcollection["OPId1"].ToString());
                searchOPAccounts.SearchInput = SearchInput.ToString();
                searchOPAccounts.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId).ToList();
                searchOPAccounts.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OPId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
                foreach (var item in searchOPAccounts.OrderOfPaymentDetailList)
                {
                    // --- Append OP Description to OP Search Main List --- //
                    searchOPAccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroup + "' AND DivisionCode = '" + Division + "'").ToList();
                    if (searchOPAccounts.OPAccountList.Count == 1)
                    {
                        foreach (var items in searchOPAccounts.OPAccountList)
                        {
                            searchOPAccounts.OPAccountDesc.Add(items.OPAccountDescription);
                        }
                    }
                    // --- END --- //
                }
                foreach (var item in searchOPAccounts.OrderOfPaymentList)
                {
                    // --- Append Company Name / Address to Search Main List --- //
                    searchOPAccounts.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchOPAccounts.CompanyList.Count == 1)
                    {
                        foreach (var items in searchOPAccounts.CompanyList)
                        {
                            searchOPAccounts.CompanyName.Add(items.CompanyName);
                            searchOPAccounts.Address.Add(items.Address);
                        }
                    }
                    // --- END --- //
                }
                // -- Sum displayed TotalQuantity and GrandTotal to Main Listing -- //
                if (searchOPAccounts.OrderOfPaymentDetailList.Count >= 1)
                {
                    searchOPAccounts.TotalQuantity = db.OrderOfPaymentDetail.Where(c => c.OrderOfPaymentId == OPId).Sum(b => b.Quantity);
                    searchOPAccounts.GrandTotal = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId).Sum(x => x.TotalAmount);
                }
                // --- END --- //
                searchOPAccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountDescription like '%" + SearchInput + "%' AND ZoneGroupCode = '" + ZoneGroup + "' AND DivisionCode = '" + Division + "'").ToList();
                searchOPAccounts.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompId + "'").ToList();
                ViewBag.SelectOPAccount = "ADDSHOW";
                return View("ViewPaymentDetails", searchOPAccounts);
            }
            //Result of selected opaccount shown by "Search button"
            else if (frmcollection.Count >= 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frmcollection[3].ToString(), out OutParseValue);
                int CompName = Convert.ToInt32(frmcollection["CompanyName1"].ToString());
                int OPId = Convert.ToInt32(frmcollection["OPId1"].ToString());
                SearchMainOrderOfPaymentViewModel searchopaccounts = new SearchMainOrderOfPaymentViewModel();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
                if (CanParse)
                {
                    int ParsedOPAccountID = int.Parse(frmcollection[3].ToString());
                    searchopaccounts.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId).ToList();
                    searchopaccounts.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OPId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
                    // --- Append OP Description to Search Main List --- //
                    foreach (var item in searchopaccounts.OrderOfPaymentDetailList)
                    {
                        searchopaccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroup + "' AND DivisionCode = '" + Division + "'").ToList();
                        if (searchopaccounts.OPAccountList.Count == 1)
                        {
                            foreach (var items in searchopaccounts.OPAccountList)
                            {
                                searchopaccounts.OPAccountDesc.Add(items.OPAccountDescription);
                            }
                        }
                    }
                    // --- END --- //
                    // --- Append Company Name / Address to Main List --- //
                    foreach (var item in searchopaccounts.OrderOfPaymentList)
                    {
                        searchopaccounts.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchopaccounts.CompanyList.Count == 1)
                        {
                            foreach (var items in searchopaccounts.CompanyList)
                            {
                                searchopaccounts.CompanyName.Add(items.CompanyName);
                                searchopaccounts.Address.Add(items.Address);
                            }
                        }
                    }
                    // --- END --- //
                    // --- Sum displayed TotalQuantity / GrandTotal to Main Listing --- //
                    if (searchopaccounts.OrderOfPaymentDetailList.Count >= 1)
                    {
                        searchopaccounts.TotalQuantity = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == OPId).Sum(k => k.Quantity);
                        searchopaccounts.GrandTotal = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == OPId).Sum(n => n.TotalAmount);
                    }
                    // --- END --- //
                    searchopaccounts.OPAccountList = db.OPAccount.Where(m => m.OPAccountId == ParsedOPAccountID).ToList();
                    searchopaccounts.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompName + "'").ToList();
                }
                else if (CanParse == false)
                {
                    searchopaccounts.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId).ToList();
                    searchopaccounts.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OPId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
                    // --- Append OP Description to Main List --- //
                    foreach (var item in searchopaccounts.OrderOfPaymentDetailList)
                    {
                        searchopaccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroup + "' AND DivisionCode = '" + Division + "'").ToList();
                        if (searchopaccounts.OPAccountList.Count == 1)
                        {
                            foreach (var items in searchopaccounts.OPAccountList)
                            {
                                searchopaccounts.OPAccountDesc.Add(items.OPAccountDescription);
                            }
                        }
                    }
                    // --- END --- //
                    // --- Append Company Name / Address to Main List --- //
                    foreach (var item in searchopaccounts.OrderOfPaymentList)
                    {
                        searchopaccounts.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchopaccounts.CompanyList.Count == 1)
                        {
                            foreach (var items in searchopaccounts.CompanyList)
                            {
                                searchopaccounts.CompanyName.Add(items.CompanyName);
                                searchopaccounts.Address.Add(items.Address);
                            }
                        }
                    }
                    // --- END --- //
                    if (searchopaccounts.OrderOfPaymentDetailList.Count >= 1)
                    {
                        searchopaccounts.TotalQuantity = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == OPId).Sum(k => k.Quantity);
                        searchopaccounts.GrandTotal = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == OPId).Sum(n => n.TotalAmount);
                    }
                    searchopaccounts.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompName + "'").ToList();
                }
                ViewBag.SelectOPAccount = "ADDHIDE";
                return View("ViewPaymentDetails", searchopaccounts);
            }
            //Default value
            else
            {
                SearchMainOrderOfPaymentViewModel searchcompany1 = new SearchMainOrderOfPaymentViewModel();
                return View("ViewPaymentDetails", searchcompany1);
            }
        }

        public ActionResult EditSearchOPAccount(string SearchInput, string AddOPDId, string EditOPDId, FormCollection frmcollection)
        {
            SearchMainOrderOfPaymentViewModel searchOPAccounts = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
            int AddOPDIds = Convert.ToInt32(AddOPDId);
            int EditOPDIds = Convert.ToInt32(EditOPDId);
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                int CompName = Convert.ToInt32(frmcollection["CompanyName1"].ToString());
                int OPId = Convert.ToInt32(frmcollection["OPId1"].ToString());
                searchOPAccounts.SearchInput = SearchInput.ToString();
                searchOPAccounts.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId && z.OrderOfPaymentDetailId == AddOPDIds || z.OrderOfPaymentDetailId == EditOPDIds).ToList();
                searchOPAccounts.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OPId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
                foreach (var item in searchOPAccounts.OrderOfPaymentDetailList)
                {
                    searchOPAccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroupCode + "' AND DivisionCode = '" + Division + "'").ToList();
                    if (searchOPAccounts.OPAccountList.Count == 1)
                    {
                        foreach (var items in searchOPAccounts.OPAccountList)
                        {
                            searchOPAccounts.OPAccountDesc.Add(items.OPAccountDescription);
                        }
                    }
                }
                foreach (var item in searchOPAccounts.OrderOfPaymentList)
                {
                    searchOPAccounts.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchOPAccounts.CompanyList.Count == 1)
                    {
                        foreach (var items in searchOPAccounts.CompanyList)
                        {
                            searchOPAccounts.CompanyName.Add(items.CompanyName);
                            searchOPAccounts.Address.Add(items.Address);
                        }
                    }
                }
                if (searchOPAccounts.OrderOfPaymentDetailList.Count >= 1)
                {
                    searchOPAccounts.TotalQuantity = db.OrderOfPaymentDetail.Where(c => c.OrderOfPaymentId == OPId).Sum(b => b.Quantity);
                    searchOPAccounts.GrandTotal = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId).Sum(x => x.TotalAmount);
                }
                searchOPAccounts.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompName + "'").ToList();
                searchOPAccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountDescription like '%" + SearchInput + "%' AND ZoneGroupCode = '" + ZoneGroupCode + "' AND DivisionCode = '" + Division + "'").ToList();
                ViewBag.SelectOPAccount = "EDITSHOW";
                return View("ViewPaymentDetails", searchOPAccounts);
            }
            //Result of selected opaccount shown by "Search button"
            else if (frmcollection.Count >= 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frmcollection[4].ToString(), out OutParseValue);
                int CompName = Convert.ToInt32(frmcollection["CompanyName1"].ToString());
                int OPId = Convert.ToInt32(frmcollection["OPId1"].ToString());
                SearchMainOrderOfPaymentViewModel searchopaccounts = new SearchMainOrderOfPaymentViewModel();
                if (CanParse)
                {
                    int ParsedOPAccountID = int.Parse(frmcollection[4].ToString());
                    searchopaccounts.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId && z.OrderOfPaymentDetailId == AddOPDIds || z.OrderOfPaymentDetailId == EditOPDIds).ToList();
                    searchopaccounts.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OPId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
                    foreach (var item in searchopaccounts.OrderOfPaymentDetailList)
                    {
                        searchopaccounts.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroupCode + "' AND DivisionCode = '" + Division + "'").ToList();
                        if (searchopaccounts.OPAccountList.Count == 1)
                        {
                            foreach (var items in searchopaccounts.OPAccountList)
                            {
                                searchopaccounts.OPAccountDesc.Add(items.OPAccountDescription);
                            }
                        }
                    }
                    foreach (var item in searchopaccounts.OrderOfPaymentList)
                    {
                        searchopaccounts.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchopaccounts.CompanyList.Count == 1)
                        {
                            foreach (var items in searchopaccounts.CompanyList)
                            {
                                searchopaccounts.CompanyName.Add(items.CompanyName);
                                searchopaccounts.Address.Add(items.Address);
                            }
                        }
                    }
                    if (searchopaccounts.OrderOfPaymentDetailList.Count >= 1)
                    {
                        searchopaccounts.TotalQuantity = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == OPId).Sum(k => k.Quantity);
                        searchopaccounts.GrandTotal = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == OPId).Sum(n => n.TotalAmount);
                    }
                    searchopaccounts.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompName + "'").ToList();
                    searchopaccounts.OPAccountList = db.OPAccount.Where(m => m.OPAccountId == ParsedOPAccountID).ToList();
                }
                ViewBag.SelectOPAccount = "EDITHIDE";

                return View("ViewPaymentDetails", searchopaccounts);
            }
            //Default value
            else
            {
                SearchMainOrderOfPaymentViewModel searchcompany1 = new SearchMainOrderOfPaymentViewModel();
                return View("ViewPaymentDetails", searchcompany1);
            }
        }

        // Display OR Delete Visa Detail
        public ActionResult DisplayDiplomatDetails(int? OPDetailId, int? OPId, FormCollection frmcollection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
            SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Select(x => x).ToList();
            if (frmcollection.Count == 0)
            {
                SearchOrderOfPaymentDetailyViewModels.DetailList = db.Detail.SqlQuery("Select * from OPDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE OrderOfPaymentId ='" + OPId + "' AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();
                SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                return View("ViewDiplomatDetails", SearchOrderOfPaymentDetailyViewModels);
            }
            else if (frmcollection.Count >= 2)
            {
                int parsedID = int.Parse(frmcollection["DeleteDetail"]);
                int parsedOPDId = int.Parse(frmcollection["DeleteOPD"]);
                OPDetail removediplomat = db.Detail.Find(parsedID);
                db.Detail.Remove(removediplomat);
                db.SaveChanges();
                TempData["TransactionSuccess"] = "delete";
                return RedirectToAction("ViewDiplomatDetails", new { OrderOfPaymentDetailId = parsedOPDId, OrderOfPaymentId = OPId });
            }
            return View();
        }

        // Data Entry For Order Of Payment
        public ActionResult AddOrderOfPayment(string AddCompCode, DateTime OPDate)
        {
            SearchMainOrderOfPaymentViewModel SearchPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
            List<Company> NewCompanies = new List<Company>();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SearchPaymentViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            //NewCompanies = db.Company.Select(x => x).ToList();
            NewCompanies = db.Company.Where(x => x.CompanyCode == AddCompCode).ToList();
            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
            //SearchPaymentViewModels.CompanyList = searchCompanyPerGroup.Companies;
            SearchPaymentViewModels.CompanyList = NewCompanies;
            OrderOfPayment OrderOfPaymentAssignment = new OrderOfPayment();
            try
            {
                if (AddCompCode != null)
                {
                    //SearchPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();
                    SearchPaymentViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.ZoneGroupcode == ZoneGroupCode).OrderByDescending(x => x.OrderOfPaymentId).Take(1).ToList();
                    // ---- Generate 9 Characters Reference Number ---- 
                    foreach (var items in SearchPaymentViewModels.OrderOfPaymentList)
                    {
                        if (items.OPNumber != null)
                        {
                            SearchPaymentViewModels.AutoOPNumber = Convert.ToInt32(items.OPNumber) + 1;
                        }
                        if (items.ReferenceNo != null)
                        {
                            string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                            string year = Convert.ToString(DateTime.Now.Year);
                            string latestyear = year.Substring(2);
                            int compyear = Convert.ToInt32(year);
                            int subYear = Convert.ToInt32(20 + items.ReferenceNo.Substring(3, 2));
                            if (subYear < compyear)
                            {
                                SearchPaymentViewModels.AutoReferenceNumber = "9" + ZoneGroup + latestyear + "0001";
                            }
                            else
                            {
                                string subAutoRef = items.ReferenceNo.Substring(5);
                                int incAutoRef = Convert.ToInt32(subAutoRef) + 1;
                                string paddedAutoRef = incAutoRef.ToString().PadLeft(4, '0');
                                SearchPaymentViewModels.AutoReferenceNumber = "9" + ZoneGroup + latestyear + paddedAutoRef;
                            }
                        }
                    }
                    // ----
                    // ---- Generate 9 Default Characters of Reference Number ----
                    if (SearchPaymentViewModels.OrderOfPaymentList.Count == 0)
                    {
                        string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                        SearchPaymentViewModels.AutoOPNumber = 00000001;
                        string year = Convert.ToString(DateTime.Now.Year);
                        string latestyear = year.Substring(2);
                        SearchPaymentViewModels.AutoReferenceNumber = "9" + ZoneGroup + latestyear + "0001";
                    }
                    // ----
                    OrderOfPaymentAssignment.OPDate = OPDate;
                    OrderOfPaymentAssignment.CompanyCode = SearchPaymentViewModels.CompanyList.FirstOrDefault(g => g.CompanyCode == AddCompCode).CompanyCode;
                    OrderOfPaymentAssignment.CompanyId = SearchPaymentViewModels.CompanyList.FirstOrDefault(g => g.CompanyCode == AddCompCode).CompanyID;
                    OrderOfPaymentAssignment.CompanyName = SearchPaymentViewModels.CompanyList.FirstOrDefault(x => x.CompanyCode == AddCompCode).CompanyName;
                    OrderOfPaymentAssignment.PaymentStatus = "UNPAID";
                    OrderOfPaymentAssignment.DivisionCode = DivisionCode;
                    OrderOfPaymentAssignment.ZoneGroupcode = ZoneGroupCode;
                    OrderOfPaymentAssignment.CreatedBy = userid;

                    do
                    {
                        BCS_Context dc = new BCS_Context();
                        SearchPaymentViewModels.OrderOfPaymentList = dc.OrderOfPayment.Where(x => x.ZoneGroupcode == ZoneGroupCode && x.ReferenceNo == SearchPaymentViewModels.AutoReferenceNumber).OrderByDescending(x => x.OrderOfPaymentId).Take(1).ToList();
                        // ---- Generate 9 Characters Reference Number ---- 
                        foreach (var items in SearchPaymentViewModels.OrderOfPaymentList)
                        {
                            if (items.OPNumber != null)
                            {
                                SearchPaymentViewModels.AutoOPNumber = Convert.ToInt32(items.OPNumber) + 1;
                            }
                            if (items.ReferenceNo != null)
                            {
                                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                                string year = Convert.ToString(DateTime.Now.Year);
                                string latestyear = year.Substring(2);
                                int compyear = Convert.ToInt32(year);
                                int subYear = Convert.ToInt32(20 + items.ReferenceNo.Substring(3, 2));
                                if (subYear < compyear)
                                {
                                    SearchPaymentViewModels.AutoReferenceNumber = "9" + ZoneGroup + latestyear + "0001";
                                }
                                else
                                {
                                    string subAutoRef = items.ReferenceNo.Substring(5);
                                    int incAutoRef = Convert.ToInt32(subAutoRef) + 1;
                                    string paddedAutoRef = incAutoRef.ToString().PadLeft(4, '0');
                                    SearchPaymentViewModels.AutoReferenceNumber = "9" + ZoneGroup + latestyear + paddedAutoRef;
                                }
                            }
                        }
                    }
                    while (SearchPaymentViewModels.OrderOfPaymentList.Count > 0);
                    {
                        OrderOfPaymentAssignment.ReferenceNo = SearchPaymentViewModels.AutoReferenceNumber;
                    }
                    db.OrderOfPayment.Add(OrderOfPaymentAssignment);
                    db.SaveChanges();
                    ////////////  FOR REDIRECT PAGE TO OP ITEMS  ///////////
                    var OPAccountTag = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).ToList();
                    var OPCompanyId = OPAccountTag.Select(c => c.CompanyId).Last();
                    var OPIds = OPAccountTag.Select(f => f.OrderOfPaymentId).Last();
                    return RedirectToAction("DisplayOrderOfPaymentDetail", new { CompanyId = OPCompanyId, opid1 = OPIds });
                }
                else
                {
                    TempData["TransactionSuccess"] = "Failed";
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script> alert('Company doesn't exist!') </script>");
                TempData["TransactionSuccess"] = "CompanyNotExist";
                return RedirectToAction("ViewPayments");
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Billing Payments Order of Payments) - from Terminal:" + ipaddress);
            return View();
        }

        // Data Entry For Order Of Payment Detail
        public ActionResult AddOrderOfPaymentDetail(int OPId, int CompanyId, string Type, string AccountTag, int Quantity, string Amount, string Representative, string Remarks, string Withholding, FormCollection frmcollection)
        {
            try
            {
                SearchMainOrderOfPaymentViewModel SearchPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
                string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Billing Payments Order of Payment Details) - from Terminal:" + ipaddress);
                OrderOfPaymentDetail OrderOfPaymentDetailAssignment = new OrderOfPaymentDetail();
                OrderOfPayment OrderOfPaymentAssignment = new OrderOfPayment();
                OrderOfPaymentDetailAssignment.OrderOfPaymentId = OPId;
                OrderOfPaymentDetailAssignment.CompanyId = CompanyId;
                OrderOfPaymentDetailAssignment.OPAccountId = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == Type && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountId;
                OrderOfPaymentDetailAssignment.AccountTag = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == Type && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).AccountTag;
                OrderOfPaymentDetailAssignment.OPAccountCode = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == Type && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountCode;
                if (Withholding == "YES")
                {
                    var TAmnt = Quantity * Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                    var WTax = 0.01;
                    OrderOfPaymentDetailAssignment.TotalAmount = TAmnt - TAmnt * Convert.ToDecimal(WTax);
                    OrderOfPaymentDetailAssignment.Withholding = "YES";
                    OrderOfPaymentDetailAssignment.WithHoldingTaxAmount = TAmnt * Convert.ToDecimal(WTax);
                }
                else
                {
                    OrderOfPaymentDetailAssignment.TotalAmount = Quantity * Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                    OrderOfPaymentDetailAssignment.Withholding = "NO";
                }
                OrderOfPaymentDetailAssignment.Quantity = Quantity;
                if (Withholding == "YES")
                {
                    OrderOfPaymentDetailAssignment.Amount = Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                }
                else
                {
                    OrderOfPaymentDetailAssignment.Amount = Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                }
                OrderOfPaymentDetailAssignment.Representative = Representative;
                OrderOfPaymentDetailAssignment.Remarks = Remarks;

                if (db.Company.FirstOrDefault(x => x.CompanyID == CompanyId).OwnershipType == "Individual" && Withholding == "YES")
                {
                    OrderOfPaymentDetailAssignment.ATC = "WI140";
                    OrderOfPaymentDetailAssignment.ATCRates = "10%";
                }
                else if (db.Company.FirstOrDefault(x => x.CompanyID == CompanyId).OwnershipType == "Corporate" || db.Company.FirstOrDefault(x => x.CompanyID == CompanyId).OwnershipType == "Government" && Withholding == "YES")
                {
                    OrderOfPaymentDetailAssignment.ATC = "WC140";
                    OrderOfPaymentDetailAssignment.ATCRates = "10%";
                }

                db.OrderOfPaymentDetail.Add(OrderOfPaymentDetailAssignment);
                db.SaveChanges();
                SearchPaymentViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails").ToList();
                OrderOfPayment opdinfo = null;
                int ParsedIntID = OPId;
                opdinfo = db.OrderOfPayment.Find(ParsedIntID);
                if (opdinfo != null)
                {
                    if (SearchPaymentViewModels.OrderOfPaymentDetailList.Count >= 1)
                    {

                        SearchPaymentViewModels.TotalQuantity = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == OPId).Sum(k => k.Quantity);
                        SearchPaymentViewModels.GrandTotal = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == OPId).Sum(k => k.TotalAmount);
                        opdinfo.TotalAmount = SearchPaymentViewModels.GrandTotal;
                        db.Entry(opdinfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (SearchPaymentViewModels.OrderOfPaymentDetailList.Count == 0)
                    {
                        if (Withholding == "YES")
                        {
                            var TAmnt = Quantity * Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                            var WTax = 0.01;
                            OrderOfPaymentDetailAssignment.TotalAmount = TAmnt - TAmnt * Convert.ToDecimal(WTax);
                            OrderOfPaymentDetailAssignment.WithHoldingTaxAmount = TAmnt * Convert.ToDecimal(WTax);
                            OrderOfPaymentDetailAssignment.Withholding = "YES";
                        }
                        else
                        {
                            OrderOfPaymentDetailAssignment.TotalAmount = Quantity * Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                        }
                        if (Withholding == "YES")
                        {
                            OrderOfPaymentDetailAssignment.Amount = Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                        }
                        else
                        {
                            OrderOfPaymentDetailAssignment.Amount = Convert.ToDecimal(Amount.ToString().Replace(",", ""));
                        }
                        db.Entry(opdinfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                SearchMainOrderOfPaymentViewModel searchnew = new SearchMainOrderOfPaymentViewModel();
                searchnew.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentId == OPId).ToList();
                searchnew.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OPId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
                string ZoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
                string Division = context.Users.FirstOrDefault(x => x.Id == userid).Division;
                foreach (var item in searchnew.OrderOfPaymentDetailList)
                {
                    searchnew.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroup + "'").ToList();
                    if (searchnew.OPAccountList.Count == 1)
                    {
                        foreach (var items in searchnew.OPAccountList)
                        {
                            searchnew.OPAccountDesc.Add(items.OPAccountDescription);
                        }
                    }
                }
                ////////////  FOR ACCOUNTABLE FORMS / VISA / PROSC FEE  ////////////
                var OPAccountTag = searchnew.OrderOfPaymentDetailList;
                if (OPAccountTag.Select(g => g.AccountTag).Last() == "AF")
                {
                    var OPDetailId = OPAccountTag.Select(c => c.OrderOfPaymentDetailId).Last();
                    var OPDetailQty = OPAccountTag.Select(f => f.Quantity).Last();
                    SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
                    SearchOrderOfPaymentDetailyViewModels.DetailList = db.Detail.SqlQuery("Select * from OPDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                    var OrderId = SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList[0].OrderOfPaymentId;
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(f => f.OrderOfPaymentId == OrderId && f.DivisionCode == DivisionCode && f.ZoneGroupcode == ZoneGroupCode).ToList();
                    SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Select(x => x).ToList();
                    TempData["Accountable"] = "AF";
                    ViewBag.Accountable = TempData["Accountable"] as string;
                    return View("ViewDiplomatDetails", SearchOrderOfPaymentDetailyViewModels);
                }
                else if (OPAccountTag.Select(g => g.AccountTag).Last() == "VISA")
                {
                    var OPDetailId = OPAccountTag.Select(c => c.OrderOfPaymentDetailId).Last();
                    var OPDetailQty = OPAccountTag.Select(d => d.Quantity).Last();
                    SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
                    SearchOrderOfPaymentDetailyViewModels.DetailList = db.Detail.SqlQuery("Select * from OPDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                    var OrderId = SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList[0].OrderOfPaymentId;
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(f => f.OrderOfPaymentId == OrderId && f.DivisionCode == DivisionCode && f.ZoneGroupcode == ZoneGroupCode).ToList();
                    SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Select(x => x).ToList();
                    //Response.Write("<script> alert('Required: ' + '" + OPDetailQty + "' + ' Other Details') </script>");
                    TempData["Accountable"] = "VISA";
                    ViewBag.Accountable = TempData["Accountable"] as string;
                    return View("ViewDiplomatDetails", SearchOrderOfPaymentDetailyViewModels);
                }
                else if (OPAccountTag.Select(g => g.AccountTag).Last() == "PF")
                {
                    var OPDetailId = OPAccountTag.Select(c => c.OrderOfPaymentDetailId).Last();
                    var OPDetailQty = OPAccountTag.Select(d => d.Quantity).Last();
                    SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
                    SearchOrderOfPaymentDetailyViewModels.DetailList = db.Detail.SqlQuery("Select * from OPDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentDetailId ='" + OPDetailId + "'").ToList();
                    var OrderId = SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList[0].OrderOfPaymentId;
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.Where(f => f.OrderOfPaymentId == OrderId && f.DivisionCode == DivisionCode && f.ZoneGroupcode == ZoneGroupCode).ToList();
                    SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Select(x => x).ToList();
                    //Response.Write("<script> alert('Required: ' + '" + OPDetailQty + "' + ' Other Details') </script>");
                    TempData["Accountable"] = "VISA";
                    ViewBag.Accountable = TempData["Accountable"] as string;
                    return View("ViewDiplomatDetails", SearchOrderOfPaymentDetailyViewModels);
                }
                ///////////////////////
                else
                {
                    TempData["TransactionSuccess"] = "Add";
                    return RedirectToAction("ViewPaymentDetails", new { CompanyId = CompanyId, opid1 = OPId });
                }
            }
            catch
            {
                TempData["TransactionSuccess"] = "NoDesc";
            }
            return RedirectToAction("ViewPaymentDetails", new { CompanyId = CompanyId, opid1 = OPId });
        }

        // Data Entry For Visa Detail
        public ActionResult AddDiplomat(int OrderOfPaymentDetailId, int OrderOfPaymentId, string GivenName, string MiddleName, string SurName, string VisaName, string Nationality, string Description, int? Start, int? End, string Pending, string SN1, string SN2, string SN3, string SN4, string SN5, string SN6, string SN7, string SN8, string SN9, string SN10, string SN11, string SN12, string SN13, string SN14, string SN15)
        {
            SearchMainOrderOfPaymentViewModel SearchPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
            SearchPaymentViewModels.OPAccountList = db.OPAccount.Select(x => x).ToList();
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Added Item! (Billing Payments Other Details) - from Terminal:" + ipaddress);
            OPDetail DiplomatAssignment = new OPDetail();
            if (GivenName != string.Empty || MiddleName != string.Empty || SurName != string.Empty || VisaName != string.Empty || Description != string.Empty || Start >= 1 || End >= 1 || Pending != string.Empty || SN1 != string.Empty || SN5 != string.Empty || SN10 != string.Empty || SN15 != string.Empty)
            {
                DiplomatAssignment.OrderOfPaymentId = OrderOfPaymentId;
                DiplomatAssignment.OrderOfPaymentDetailId = OrderOfPaymentDetailId;
                DiplomatAssignment.GivenName = GivenName;
                DiplomatAssignment.MiddleName = MiddleName;
                DiplomatAssignment.SurName = SurName;
                DiplomatAssignment.VisaName = VisaName;
                DiplomatAssignment.Nationality = Nationality;
                DiplomatAssignment.Description = Description;
                DiplomatAssignment.OPStart = Start;
                DiplomatAssignment.OPEnd = End;
                DiplomatAssignment.Pending = Pending;
                DiplomatAssignment.SN1 = SN1;
                DiplomatAssignment.SN2 = SN2;
                DiplomatAssignment.SN3 = SN3;
                DiplomatAssignment.SN4 = SN4;
                DiplomatAssignment.SN5 = SN5;
                DiplomatAssignment.SN6 = SN6;
                DiplomatAssignment.SN7 = SN7;
                DiplomatAssignment.SN8 = SN8;
                DiplomatAssignment.SN9 = SN9;
                DiplomatAssignment.SN10 = SN10;
                DiplomatAssignment.SN11 = SN11;
                DiplomatAssignment.SN12 = SN12;
                DiplomatAssignment.SN13 = SN13;
                DiplomatAssignment.SN14 = SN14;
                DiplomatAssignment.SN15 = SN15;
                db.Detail.Add(DiplomatAssignment);
                db.SaveChanges();
                TempData["TransactionSuccess"] = "Add";
            }
            else
            {
                TempData["TransactionSuccess"] = "Failed";
            }
            return RedirectToAction("ViewDiplomatDetails", new { OrderOfPaymentDetailId = OrderOfPaymentDetailId, OrderOfPaymentId = OrderOfPaymentId });
        }

        // Update Data for Order Of Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderOfPayment(string OPNumber, string EditcompName, DateTime OPDate, FormCollection frmcollection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            SearchMainOrderOfPaymentViewModel searchcomp = new SearchMainOrderOfPaymentViewModel();
            List<Company> NewCompanies = new List<Company>();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            NewCompanies = db.Company.Select(x => x).OrderBy(x => x.CompanyName).Take(10).ToList();
            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
            searchcomp.CompanyList = searchCompanyPerGroup.Companies;
            OrderOfPayment opinfo = null;
            int ParsedIntID = int.Parse(frmcollection["OPId"]);
            opinfo = db.OrderOfPayment.Find(ParsedIntID);
            if (opinfo != null)
            {
                try
                {
                    opinfo.OPNumber = OPNumber;
                    opinfo.CompanyCode = searchcomp.CompanyList.FirstOrDefault(z => z.CompanyName == EditcompName).CompanyCode;
                    opinfo.CompanyId = searchcomp.CompanyList.FirstOrDefault(z => z.CompanyName == EditcompName).CompanyID;
                    opinfo.CompanyName = EditcompName;
                    opinfo.OPDate = OPDate;
                    opinfo.DivisionCode = DivisionCode;
                    opinfo.ZoneGroupcode = ZoneGroupCode;
                    opinfo.CreatedBy = userid;
                    db.Entry(opinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                }
                catch
                {
                    TempData["TransactionSuccess"] = "CompanyNotExist";
                }
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Updated Item! (Billing Payments Order of Payments) - from Terminal:" + ipaddress);
            return RedirectToAction("ViewPayments");
        }

        // Update Data for Order Of Payment Detail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderOfPaymentDetail(int CompId, string Type, int Quantity, string Amount, string Withholding, string Representative, string Remarks, FormCollection frmcollection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            OrderOfPaymentDetail opdinfo = null;
            int ParsedIntID = int.Parse(frmcollection["OPDId"]);
            int ParsedIntID1 = int.Parse(frmcollection["OPId"]);
            try
            {
                opdinfo = db.OrderOfPaymentDetail.Find(ParsedIntID);
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Updated Item! (Billing Payments Order of Payment Details) - from Terminal:" + ipaddress);
                if (opdinfo != null)
                {
                    opdinfo.CompanyId = CompId;
                    opdinfo.Quantity = Quantity;
                    opdinfo.OPAccountId = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == Type && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountId;
                    opdinfo.AccountTag = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == Type && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).AccountTag;
                    opdinfo.OPAccountCode = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == Type && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountCode;
                    if (Withholding == "YES")
                    {
                        var TAmnt = Quantity * Convert.ToDecimal(Amount.Replace(",", ""));
                        var WTax = 0.01;
                        opdinfo.TotalAmount = TAmnt - TAmnt * Convert.ToDecimal(WTax);
                        opdinfo.WithHoldingTaxAmount = TAmnt * Convert.ToDecimal(WTax);
                        opdinfo.Withholding = "YES";
                    }
                    else
                    {
                        opdinfo.TotalAmount = Quantity * Convert.ToDecimal(Amount.Replace(",", ""));
                        opdinfo.Withholding = "NO";
                        opdinfo.WithHoldingTaxAmount = 0;
                    }
                    if (Withholding == "YES")
                    {
                        opdinfo.Amount = Convert.ToDecimal(Amount.Replace(",", ""));
                    }
                    else
                    {
                        opdinfo.Amount = Convert.ToDecimal(Amount.Replace(",", ""));
                    }
                    opdinfo.Representative = Representative;
                    opdinfo.Remarks = Remarks;
                    db.Entry(opdinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                SearchMainOrderOfPaymentViewModel SearchPaymentViewModels = new SearchMainOrderOfPaymentViewModel();
                SearchPaymentViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails").ToList();
                OrderOfPayment opdinfo1 = null;

                opdinfo1 = db.OrderOfPayment.Find(ParsedIntID1);
                if (opdinfo1 != null)
                {
                    if (SearchPaymentViewModels.OrderOfPaymentDetailList.Count >= 1)
                    {
                        SearchPaymentViewModels.TotalQuantity = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == ParsedIntID1).Sum(k => k.Quantity);
                        SearchPaymentViewModels.GrandTotal = db.OrderOfPaymentDetail.Where(y => y.OrderOfPaymentId == ParsedIntID1).Sum(k => k.TotalAmount);
                        opdinfo1.TotalAmount = SearchPaymentViewModels.GrandTotal;
                        db.Entry(opdinfo1).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                ////////////////////////
                foreach (var item in SearchPaymentViewModels.OrderOfPaymentDetailList)
                {
                    SearchPaymentViewModels.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "' AND ZoneGroupCode = '" + ZoneGroupCode + "'").ToList();
                    if (SearchPaymentViewModels.OPAccountList.Count == 1)
                    {
                        foreach (var items in SearchPaymentViewModels.OPAccountList)
                        {
                            SearchPaymentViewModels.OPAccountDesc.Add(items.OPAccountDescription);
                        }
                    }
                }
                ///////////////////////
                TempData["TransactionSuccess"] = "Edit";
            }
            catch
            {
                TempData["TransactionSuccess"] = "NoDesc";
            }
            return RedirectToAction("ViewPaymentDetails", new { CompanyId = CompId, opid1 = ParsedIntID1 });
        }

        // Update Data for Visa Detail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDiplomatDetail(int EditDetail, int EditOrderOfPaymentDetailId, int EditOrderOfPaymentId, FormCollection frmcollection)
        {
            OPDetail detailinfo = null;
            int ParsedIntID = EditDetail;
            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
            SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Select(x => x).ToList();
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "User Updated Item! (Billing Payments Other Details) - from Terminal:" + ipaddress);
            detailinfo = db.Detail.Find(ParsedIntID);
            if (detailinfo != null)
            {
                detailinfo.OrderOfPaymentId = EditOrderOfPaymentId;
                detailinfo.OrderOfPaymentDetailId = EditOrderOfPaymentDetailId;
                detailinfo.GivenName = frmcollection["EditGivenName"];
                detailinfo.MiddleName = frmcollection["EditMiddleName"];
                detailinfo.SurName = frmcollection["EditSurName"];
                detailinfo.VisaName = frmcollection["EditVisaName"];
                detailinfo.Nationality = frmcollection["EditNationality"];
                detailinfo.Description = frmcollection["EditDescription"];
                try
                {
                    detailinfo.OPStart = Convert.ToInt32(frmcollection["EditStart"]);
                }
                catch { }
                try
                {
                    detailinfo.OPEnd = Convert.ToInt32(frmcollection["EditEnd"]);
                }
                catch { }
                detailinfo.Pending = frmcollection["EditPending"];
                detailinfo.SN1 = frmcollection["EditSN1"];
                detailinfo.SN2 = frmcollection["EditSN2"];
                detailinfo.SN3 = frmcollection["EditSN3"];
                detailinfo.SN4 = frmcollection["EditSN4"];
                detailinfo.SN5 = frmcollection["EditSN5"];
                detailinfo.SN6 = frmcollection["EditSN6"];
                detailinfo.SN7 = frmcollection["EditSN7"];
                detailinfo.SN8 = frmcollection["EditSN8"];
                detailinfo.SN9 = frmcollection["EditSN9"];
                detailinfo.SN10 = frmcollection["EditSN10"];
                detailinfo.SN11 = frmcollection["EditSN11"];
                detailinfo.SN12 = frmcollection["EditSN12"];
                detailinfo.SN13 = frmcollection["EditSN13"];
                detailinfo.SN14 = frmcollection["EditSN14"];
                detailinfo.SN15 = frmcollection["EditSN15"];
                db.Entry(detailinfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            TempData["TransactionSuccess"] = "Edit";
            return RedirectToAction("ViewDiplomatDetails", new { OrderOfPaymentDetailId = EditOrderOfPaymentDetailId, OrderOfPaymentId = EditOrderOfPaymentId });
        }
        public ActionResult ViewPaymentDetails(int CompanyId, int? opid1)
        {
            SearchMainOrderOfPaymentViewModel searchOPAccounts = new SearchMainOrderOfPaymentViewModel();
            SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentId ='" + opid1 + "'").ToList();
            SearchOrderOfPaymentDetailyViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompanyId + "'").ToList();
            SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments Where OrderOfPaymentId ='" + opid1 + "' AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();
            SearchOrderOfPaymentDetailyViewModels.ZoneList = db.Zone.Select(x => x).ToList();
            foreach (var item in SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList)
            {
                SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "'").ToList();
                if (SearchOrderOfPaymentDetailyViewModels.OPAccountList.Count == 1)
                {
                    foreach (var items in SearchOrderOfPaymentDetailyViewModels.OPAccountList)
                    {
                        SearchOrderOfPaymentDetailyViewModels.OPAccountDesc.Add(items.OPAccountDescription);
                    }
                }
            }
            foreach (var item in SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList)
            {
                SearchOrderOfPaymentDetailyViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                if (SearchOrderOfPaymentDetailyViewModels.CompanyList.Count == 1)
                {
                    foreach (var items in SearchOrderOfPaymentDetailyViewModels.CompanyList)
                    {
                        SearchOrderOfPaymentDetailyViewModels.CompanyName.Add(items.CompanyName);
                        SearchOrderOfPaymentDetailyViewModels.Address.Add(items.Address);
                    }
                }
            }
            if (SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList.Count >= 1)
            {
                SearchOrderOfPaymentDetailyViewModels.TotalQuantity = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == opid1).Sum(c => c.Quantity);
                SearchOrderOfPaymentDetailyViewModels.GrandTotal = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == opid1).Sum(c => c.TotalAmount);
            }
            SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).ToList();
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            return View("ViewPaymentDetails", SearchOrderOfPaymentDetailyViewModels);
        }

        public ActionResult ViewDiplomatDetails(int OrderOfPaymentDetailId, int OrderOfPaymentId)
        {
            SearchMainOrderOfPaymentViewModel searchnew = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            searchnew.DetailList = db.Detail.SqlQuery("Select * from OPDetails Where OrderOfPaymentDetailId ='" + OrderOfPaymentDetailId + "'").ToList();
            searchnew.OrderOfPaymentList = db.OrderOfPayment.Where(z => z.OrderOfPaymentId == OrderOfPaymentId && z.DivisionCode == DivisionCode && z.ZoneGroupcode == ZoneGroupCode).ToList();
            searchnew.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.Where(z => z.OrderOfPaymentDetailId == OrderOfPaymentDetailId).ToList();
            searchnew.OPAccountList = db.OPAccount.Select(x => x).ToList();
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            return View("ViewDiplomatDetails", searchnew);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchCompany(string SearchInput, FormCollection frm)
        {
            List<Company> NewCompanies = new List<Company>();
            ApplicationDbContext context = new ApplicationDbContext();
            SearchMainOrderOfPaymentViewModel searchcompany1 = new SearchMainOrderOfPaymentViewModel();
            searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                searchcompany1.SearchInput = SearchInput.ToString();
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();
                foreach (var item in searchcompany1.OrderOfPaymentList)
                {
                    searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchcompany1.CompanyListTwo.Count == 1)
                    {
                        foreach (var items in searchcompany1.CompanyListTwo)
                        {
                            searchcompany1.CompanyName.Add(items.CompanyName);
                            searchcompany1.Address.Add(items.Address);
                            searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        }
                    }
                }
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                ViewBag.OPCompanySelected = "OPSHOW";
                return View("ViewPayments", searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[1].ToString());
                    searchcompany1.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).Take(200).OrderByDescending(z => z.OrderOfPaymentId).ToList();
                    foreach (var item in searchcompany1.OrderOfPaymentList)
                    {
                        searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchcompany1.CompanyListTwo.Count == 1)
                        {
                            foreach (var items in searchcompany1.CompanyListTwo)
                            {
                                searchcompany1.CompanyName.Add(items.CompanyName);
                                searchcompany1.Address.Add(items.Address);
                                searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }
                    }
                    searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                }
                ViewBag.OPCompanySelected = "OPHIDE";
                return View("ViewPayments", searchcompany1);
            }
            //Default value
            else
            {
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).ToList();
                return View("ViewPayments", searchcompany1);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSearchCompany(string SearchInput, int? OPId2, int? OPId3, FormCollection frm)
        {
            List<Company> NewCompanies = new List<Company>();
            ApplicationDbContext context = new ApplicationDbContext();
            SearchMainOrderOfPaymentViewModel searchcompany1 = new SearchMainOrderOfPaymentViewModel();
            searchcompany1.ZoneList = db.Zone.Select(x => x).ToList();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            //Result of initial search button
            if (!string.IsNullOrEmpty(SearchInput))
            {
                searchcompany1.SearchInput = SearchInput.ToString();
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE OrderOfPaymentId = '" + OPId2 + "' AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();
                foreach (var item in searchcompany1.OrderOfPaymentList)
                {
                    searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                    if (searchcompany1.CompanyListTwo.Count == 1)
                    {
                        foreach (var items in searchcompany1.CompanyListTwo)
                        {
                            searchcompany1.CompanyName.Add(items.CompanyName);
                            searchcompany1.Address.Add(items.Address);
                            searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        }
                    }
                }
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + SearchInput + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
                searchcompany1.CompanyList = searchCompanyPerGroup.Companies;
                ViewBag.OPCompanySelected = "EDITOPSHOW";
                return View("ViewPayments", searchcompany1);
            }
            //Result of selected company shown by "Search button"
            else if (frm.Count == 3)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[2].ToString(), out OutParseValue);
                if (CanParse)
                {
                    int ParsedCompanyID = int.Parse(frm[2].ToString());
                    searchcompany1.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE OrderOfPaymentId = '" + OPId3 + "' AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();
                    foreach (var item in searchcompany1.OrderOfPaymentList)
                    {
                        searchcompany1.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchcompany1.CompanyListTwo.Count == 1)
                        {
                            foreach (var items in searchcompany1.CompanyListTwo)
                            {
                                searchcompany1.CompanyName.Add(items.CompanyName);
                                searchcompany1.Address.Add(items.Address);
                                searchcompany1.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }
                    }
                    searchcompany1.CompanyList = db.Company.Where(m => m.CompanyID == ParsedCompanyID).ToList();
                }
                ViewBag.OPCompanySelected = "EDITOPHIDE";
                return View("ViewPayments", searchcompany1);
            }
            //Default value
            else
            {
                searchcompany1.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.OrderOfPaymentId == OPId2 && x.DivisionCode == DivisionCode && x.ZoneGroupcode == ZoneGroupCode).ToList();
                return View("ViewPayments", searchcompany1);
            }
        }

        public ActionResult selectFilter(string opfilter, string Fildate, FormCollection frm)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            List<Company> NewCompanies = new List<Company>();
            NewCompanies = db.Company.Select(x => x).OrderBy(x => x.CompanyName).ToList();
            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
            searchop.CompanyList = searchCompanyPerGroup.Companies;
            searchop.ZoneList = db.Zone.Select(x => x).ToList();
            var inputSearch = opfilter;
            var inputDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            if (Fildate != string.Empty)
            {
                inputDate = Convert.ToDateTime(Fildate).ToString("yyyy-MM-dd");
            }
            if (frm.Count > 0)
            {
                if (frm["selectFilter"] == "CompanyName")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Company Name (Billing Payments Order of Payments) - from Terminal:" + ipaddress);
                    searchop.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.CompanyName.Contains(inputSearch) && x.DivisionCode == DivisionCode).ToList().OrderByDescending(x => x.OrderOfPaymentId).ToList();
                    foreach (var item in searchop.OrderOfPaymentList)
                    {
                        searchop.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchop.CompanyListTwo.Count == 1)
                        {
                            foreach (var items in searchop.CompanyListTwo)
                            {
                                searchop.CompanyName.Add(items.CompanyName);
                                searchop.Address.Add(items.Address);
                                searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }

                    }
                }
                else if (frm["selectFilter"] == "OP/Ref No")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by OP/Ref No (Billing Payments Order of Payments) - from Terminal:" + ipaddress);
                    searchop.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.ReferenceNo.Contains(inputSearch) && x.DivisionCode == DivisionCode).ToList().OrderByDescending(x => x.OrderOfPaymentId).ToList();
                    foreach (var item in searchop.OrderOfPaymentList)
                    {
                        searchop.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchop.CompanyListTwo.Count == 1)
                        {
                            foreach (var items in searchop.CompanyListTwo)
                            {
                                searchop.CompanyName.Add(items.CompanyName);
                                searchop.Address.Add(items.Address);
                                searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }
                    }
                    ViewBag.OPCompanySelected = "OK";
                }
                else if (frm["selectFilter"] == "Date")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by Date (Billing Payments Order of Payments) - from Terminal:" + ipaddress);
                    searchop.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("SELECT * from OrderOfPayments where OPDate between '" + inputDate + "' AND '" + inputDate + "' AND DivisionCode = '" + DivisionCode + "'").ToList();
                    foreach (var item in searchop.OrderOfPaymentList)
                    {
                        searchop.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchop.CompanyListTwo.Count == 1)
                        {
                            foreach (var items in searchop.CompanyListTwo)
                            {
                                searchop.CompanyName.Add(items.CompanyName);
                                searchop.Address.Add(items.Address);
                                searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }
                    }
                    ViewBag.OPCompanySelected = "OK";
                }
                else if (frm["selectFilter"] == "OR No.")
                {
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "User searched items filtered by OR No. (Billing Payments Order of Payments) - from  Terminal:" + ipaddress);
                    searchop.OrderOfPaymentList = db.OrderOfPayment.Where(x => x.ORNumber.ToString().Contains(inputSearch) && x.DivisionCode == DivisionCode).ToList().OrderByDescending(x => x.OrderOfPaymentId).ToList();
                    foreach (var item in searchop.OrderOfPaymentList)
                    {
                        searchop.CompanyListTwo = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchop.CompanyListTwo.Count == 1)
                        {
                            foreach (var items in searchop.CompanyListTwo)
                            {
                                searchop.CompanyName.Add(items.CompanyName);
                                searchop.Address.Add(items.Address);
                                searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }
                    }
                    ViewBag.OPCompanySelected = "OK";
                }
            }
            //
            //Result of selected company shown by "Search button"
            if (frm.Count == 2)
            {
                int OutParseValue;
                bool CanParse = int.TryParse(frm[1].ToString(), out OutParseValue);
                if (CanParse)
                {
                    int ParsedOPID = int.Parse(frm[1].ToString());

                    searchop.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments WHERE DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "' AND OrderOfPaymentId = '" + ParsedOPID + "'").ToList();
                    foreach (var item in searchop.OrderOfPaymentList)
                    {
                        searchop.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (searchop.CompanyList.Count == 1)
                        {
                            foreach (var items in searchop.CompanyList)
                            {
                                searchop.CompanyName.Add(items.CompanyName);
                                searchop.Address.Add(items.Address);
                                searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                            }
                        }
                    }
                }
            }
            return View("ViewPayments", searchop);

        }


        // Display Order Of Payment Detail
        public ActionResult BackOrderOfPaymentDetail(int CompanyId, int? opid1, FormCollection frmcollection)
        {
            SearchMainOrderOfPaymentViewModel searchOPAccounts = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            if (frmcollection.Count <= 2)
            {
                if (CompanyId != 0)
                {
                    SearchMainOrderOfPaymentViewModel SearchOrderOfPaymentDetailyViewModels = new SearchMainOrderOfPaymentViewModel();
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList = db.OrderOfPaymentDetail.SqlQuery("Select * from OrderOfPaymentDetails Where OrderOfPaymentId ='" + opid1 + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies Where CompanyId ='" + CompanyId + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList = db.OrderOfPayment.SqlQuery("Select * from OrderOfPayments Where OrderOfPaymentId ='" + opid1 + "' AND DivisionCode = '" + DivisionCode + "' AND ZoneGroupcode = '" + ZoneGroupCode + "'").ToList();
                    SearchOrderOfPaymentDetailyViewModels.ZoneList = db.Zone.Select(x => x).ToList();
                    foreach (var item in SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList)
                    {
                        SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.SqlQuery("Select * from OPAccounts where OPAccountId = '" + item.OPAccountId + "'").ToList();
                        if (SearchOrderOfPaymentDetailyViewModels.OPAccountList.Count == 1)
                        {
                            foreach (var items in SearchOrderOfPaymentDetailyViewModels.OPAccountList)
                            {
                                SearchOrderOfPaymentDetailyViewModels.OPAccountDesc.Add(items.OPAccountDescription);
                            }
                        }
                    }
                    foreach (var item in SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentList)
                    {
                        SearchOrderOfPaymentDetailyViewModels.CompanyList = db.Company.SqlQuery("Select * from Companies where CompanyId = '" + item.CompanyId + "'").ToList();
                        if (SearchOrderOfPaymentDetailyViewModels.CompanyList.Count == 1)
                        {
                            foreach (var items in SearchOrderOfPaymentDetailyViewModels.CompanyList)
                            {
                                SearchOrderOfPaymentDetailyViewModels.CompanyName.Add(items.CompanyName);
                                SearchOrderOfPaymentDetailyViewModels.Address.Add(items.Address);
                            }
                        }
                    }
                    if (SearchOrderOfPaymentDetailyViewModels.OrderOfPaymentDetailList.Count >= 1)
                    {
                        SearchOrderOfPaymentDetailyViewModels.TotalQuantity = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == opid1).Sum(c => c.Quantity);
                        SearchOrderOfPaymentDetailyViewModels.GrandTotal = db.OrderOfPaymentDetail.Where(x => x.OrderOfPaymentId == opid1).Sum(c => c.TotalAmount);
                    }
                    SearchOrderOfPaymentDetailyViewModels.OPAccountList = db.OPAccount.Where(x => x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OrderBy(x => x.OPAccountDescription).ToList();
                    return View("ViewPaymentDetails", SearchOrderOfPaymentDetailyViewModels);
                }
            }
            return RedirectToAction("ViewPayments");
        }

        public JsonResult Feedata(string comp)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            try
            {
                var Fees = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == comp && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountFee;
                return Json(Fees);
            }
            catch { }
            return Json(null);
        }

        public JsonResult Feedata2(string comp2)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            try
            {
                var Fees = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == comp2 && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountFee;
                return Json(Fees);
            }
            catch { }
            return Json(null);
        }

        public JsonResult SubItemsData(string comp2)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string DivisionCode = context.Users.FirstOrDefault(m => m.Id == userid).Division;
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            try
            {
                var Fees = db.OPAccount.FirstOrDefault(x => x.OPAccountDescription == comp2 && x.DivisionCode == DivisionCode && x.ZoneGroupCode == ZoneGroupCode).OPAccountFee;
                return Json(Fees);
            }
            catch { }
            return Json(null);
        }

        public JsonResult CompData(string comp)
        {
      
            List<BillingPaymentJsonReturnData> billingPaymentJsonReturnData = new List<BillingPaymentJsonReturnData>();
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            List<Company> NewCompanies = new List<Company>();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            if (comp.Length >= 3)
            {

                var newcomp = comp.Replace("'", "''");
                //NewCompanies = db.Company.Where(x => x.CompanyName.Contains(comp)).ToList();
                NewCompanies = db.Company.SqlQuery("SELECT * FROM COMPANIES WHERE CompanyName like '" + newcomp + "%'").ToList();
                
                 var ZoneList = db.Zone.Select(x => x).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
                //searchop.CompanyList = searchCompanyPerGroup.Companies;
                searchop.CompanyList = NewCompanies;
                foreach (var items in searchop.CompanyList)
                {
                    searchop.CompanyID.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyID.ToString());
                    searchop.CompanyName.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyName);
                    searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                }
                for (int i = 0; i < searchop.CompanyName.Count; i++)
                {
                    BillingPaymentJsonReturnData billingPaymentJsonReturnDataNew = new BillingPaymentJsonReturnData();
                    billingPaymentJsonReturnDataNew.CompanyID = Convert.ToInt32(searchop.CompanyID[i]);
                   // billingPaymentJsonReturnDataNew.Companyid = Convert.ToString(searchop.CompanyID[i]);
                   //billingPaymentJsonReturnDataNew.Companyid = searchop.CompanyID[i].ToString();
                    billingPaymentJsonReturnDataNew.CompanyName = searchop.CompanyName[i].ToString();
                    billingPaymentJsonReturnDataNew.ZoneName = searchop.ZoneName[i].ToString();
                   // billingPaymentJsonReturnDataNew.companycount = Convert.ToString(NewCompanies.Count());
                    billingPaymentJsonReturnData.Add(billingPaymentJsonReturnDataNew);
                    
                }

               return Json(billingPaymentJsonReturnData);
            }
            return Json(null);
        }

        public JsonResult GetAddress(string comp)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            try
            {
                var Addr = db.Company.FirstOrDefault(x => x.CompanyName == comp).Address;
                return Json(Addr);
            }
            catch { }
            return Json(null);
        }

        public JsonResult CompData2(string comp2)
        {
            List<BillingPaymentJsonReturnData> billingPaymentJsonReturnData = new List<BillingPaymentJsonReturnData>();
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            List<Company> NewCompanies = new List<Company>();
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            if (comp2.Length >= 3)
            {
                NewCompanies = db.Company.Where(x => x.CompanyName.Contains(comp2)).Take(15).ToList();
                var ZoneList = db.Zone.Select(x => x).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroupCode);
                searchop.CompanyList = searchCompanyPerGroup.Companies;
                foreach (var items in searchop.CompanyList)
                {
                    searchop.CompanyName.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyName);
                    searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                }
                for (int i = 0; i < searchop.CompanyName.Count; i++)
                {
                    BillingPaymentJsonReturnData billingPaymentJsonReturnDataNew = new BillingPaymentJsonReturnData();
                    billingPaymentJsonReturnDataNew.CompanyName = searchop.CompanyName[i].ToString();
                    billingPaymentJsonReturnDataNew.ZoneName = searchop.ZoneName[i].ToString();
                    billingPaymentJsonReturnData.Add(billingPaymentJsonReturnDataNew);
                }
                return Json(billingPaymentJsonReturnData);
            }
            return Json(null);
        }

        public JsonResult GetAddress2(string comp)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            try
            {
                var Addr = db.Company.FirstOrDefault(x => x.CompanyName == comp).Address;
                return Json(Addr);
            }
            catch { }
            return Json(null);
        }

        public JsonResult GetCompCode(string comp)
        {
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();
            try
            {
      

                var CompZone = comp.Split('|');
                var Comp = CompZone[0].TrimStart();
                var Zone = CompZone[1].TrimStart();
                var ZoneSplit = Zone.Split('~');
                var finalid = ZoneSplit[1].Replace("(", "").Replace(")", "");
                int compid = Convert.ToInt32(finalid);
                var compName = Comp;
                var zoneName = ZoneSplit[0];

                var zoneCode = db.Zone.SingleOrDefault(x => x.ZoneName.Replace(" ", "") == zoneName.Replace(" ", "")).ZoneCode;
                var compCode = db.Company.SingleOrDefault(x => x.CompanyName.Replace(",", "") == compName.Replace(",", "") && x.ZoneCode == zoneCode && x.CompanyID == compid).CompanyCode;

                var code = compCode;

                return Json(code);
            }
            catch (Exception ex) {


            }
            return Json(null);
        }

        // Generate Report
        public ActionResult OPSlipPayments(string OPRefNumber, string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var fname = context.Users.SingleOrDefault(m => m.Id == userid).GivenName;
            var mname = context.Users.SingleOrDefault(m => m.Id == userid).MiddleName;
            var lname = context.Users.SingleOrDefault(m => m.Id == userid).LastName;

            var signedUser = fname + " " + mname + " " + lname;

            NameValueCollection settings = new NameValueCollection();

            switch (zoneGroupCode)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/OPSlip");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/OPSlip");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/OPSlip");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/OPSlip");
                    break;

                default:
                    break;
            }

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();
            string preparedBy = settings["preparedBy"].ToString();
            string refNumber = OPRefNumber;

            UriBuilder serverURI = new UriBuilder(serverURL);

            // return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fOrderOfPaymentReports%2fOrderofPaymentSlip&rs:Command=Render&refNumber=" + refNumber + "&zoneGroupCode=" + zoneGroupCode + "&Preparedby=" + Preparedby);
            //return Redirect("http://52.76.199.113/ReportServer/Pages/ReportViewer.aspx?%2fOrderOfPaymentReports%2fOrderofPaymentSlip&rs:Command=Render&refNumber=" + refNumber + "&zoneGroupCode=" + zoneGroupCode + "&preparedBy=" + preparedBy);

            serverURI.Query = serverURI.Query.ToString() + "%2fOrderOfPaymentReports%2fOrderofPaymentSlip&rs:Command=Render&refNumber=" + refNumber + "&zoneGroupCode=" + zoneGroupCode + "&preparedBy=" + signedUser;

            return Redirect(serverURI.Uri.ToString());

        }
        //public ActionResult OPSummaryReport(string reportType)
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string zoneGroupCode = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

        //    return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fBilling%2fOrderofPaymentSummaryReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode);
        //}
    }
}