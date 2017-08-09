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

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class MaintenanceBillingAndCollectionRatesController : Controller
    {
        private BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult ViewBillingAndCollectionRatesRPG()
        {
            var username = User.Identity.GetUserName();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            ViewBag.ZoneGroup = ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.NGASCode = db.NGAS.ToList();
            SearchBillingAndCollectionRates temp = TempData["searchBillingAndCollectionRates"] as SearchBillingAndCollectionRates;
            ViewBag.Category = TempData["Category"] as string;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("searchBillingAndCollectionRates");
            return View("ViewBillingAndCollectionRates", temp);
        }

        // GET: MaintenanceBillingAndCollectionRates
        public ActionResult ViewBillingAndCollectionRates()
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.NGASCode = db.NGAS.ToList();
            ViewBag.ShowAdd = false;
            ViewBag.ZoneGroup = ZoneGroup;
            SearchBillingAndCollectionRates searchBillingAndCollectionRates = new SearchBillingAndCollectionRates();
            var bill = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
            var subcat = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

            foreach (var item in bill)
            {
                searchBillingAndCollectionRates.Category.Add(item.Category);
            }

            foreach (var item in subcat)
            {
                searchBillingAndCollectionRates.SubCategory.Add(item.SubCategory);
            }
            return View(searchBillingAndCollectionRates);
        }

        public ActionResult FillSubCategory(string Category)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.NGASCode = db.NGAS.ToList();
            SearchBillingAndCollectionRates searchBillingAndCollectionRates = new SearchBillingAndCollectionRates();
            if (!string.IsNullOrEmpty(Category))
            {
                searchBillingAndCollectionRates.BillingRate = db.BillingRates.Where(m => m.Category == Category).ToList();
                var bill = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                var subcat = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

                foreach (var item in bill)
                {
                    searchBillingAndCollectionRates.Category.Add(item.Category);
                }

                foreach (var item in subcat)
                {
                    searchBillingAndCollectionRates.SubCategory.Add(item.SubCategory);
                }
            }
            ViewBag.ZoneGroup = ZoneGroup;
            ViewBag.Category = Category;
            return View("ViewBillingAndCollectionRates", searchBillingAndCollectionRates);
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddSubCategory(string SubCategory, string Category, string Currency, string SubCat, string TransactionType, string NGAS)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.Groups = db.ZoneGroup.ToList();
            SearchBillingAndCollectionRates searchBillingAndCollectionRates = new SearchBillingAndCollectionRates();

            if (TransactionType.ToUpper() == "ADD") //Add sub category
            {
                if (!string.IsNullOrEmpty(SubCategory))
                {
                    List<BillingRate> billrate = db.BillingRates.Where(m => m.Category.ToUpper() == Category.ToUpper()
                        && m.SubCategory.ToUpper() == SubCategory.ToUpper() && m.ZoneGroup == ZoneGroup).ToList();

                    if (Category.ToUpper() == "SEWERAGE")
                    {
                        TempData["TransactionSuccess"] = "ErrorSewerage";
                    }
                    else
                    {
                        BillingRate billingRate = new BillingRate();
                        billingRate.Category = Category;
                        billingRate.ZoneGroup = ZoneGroup;
                        billingRate.NGASCode = NGAS;

                        if (Category != "Rental Fee") //If billing rate. add prefix currency.
                            billingRate.SubCategory = SubCategory;
                        else
                            billingRate.SubCategory = Currency + " " + SubCategory;

                        billingRate.Rate = 0;

                        db.BillingRates.Add(billingRate);
                        db.SaveChanges();
                        TempData["TransactionSuccess"] = "Add";
                    }
                }

                searchBillingAndCollectionRates.BillingRate = db.BillingRates.Where(m => m.Category == Category).ToList();
                var bill = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                var subcat = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

                foreach (var item in bill)
                {
                    searchBillingAndCollectionRates.Category.Add(item.Category);
                }

                foreach (var item in subcat)
                {
                    searchBillingAndCollectionRates.SubCategory.Add(item.SubCategory);
                }
            }
            else if (TransactionType.ToUpper() == "EDIT") // Modify subcategory
            {
                if (!string.IsNullOrEmpty(SubCategory))
                {
                    BillingRate billingRate = new BillingRate();
                    //billingRate = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).Where(m => m.SubCategory == SubCat).Single();
                    billingRate = db.BillingRates.FirstOrDefault(x => x.ZoneGroup == ZoneGroup && x.Category == Category && x.SubCategory == SubCat);
                    if (Category != "Rental Fee") //If billing rate. add prefix currency.
                    {
                        billingRate.SubCategory = SubCategory;
                        billingRate.NGASCode = NGAS;
                    }
                    else
                    {
                        billingRate.SubCategory = Currency + " " + SubCategory;
                        billingRate.NGASCode = NGAS;
                    }
                       

                    db.Entry(billingRate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                }
                else
                    Response.Write("<script>alert('Unable to edit SubCategory. Please check blank field.')</script>");

                searchBillingAndCollectionRates.BillingRate = db.BillingRates.Where(m => m.Category == Category).ToList();
                var bill = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
                var subcat = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

                foreach (var item in bill)
                {
                    searchBillingAndCollectionRates.Category.Add(item.Category);
                }

                foreach (var item in subcat)
                {
                    searchBillingAndCollectionRates.SubCategory.Add(item.SubCategory);
                }
            }
            ViewBag.Category = Category;
            TempData["searchBillingAndCollectionRates"] = searchBillingAndCollectionRates;
            TempData["Category"] = Category;
            return RedirectToAction("ViewBillingAndCollectionRatesRPG", "MaintenanceBillingAndCollectionRates");
            //return View("ViewBillingAndCollectionRates", searchBillingAndCollectionRates);
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(string Category)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.Groups = db.ZoneGroup.ToList();
            //ViewBag.Groups = new SelectList(db.ZoneGroup.ToList(), "ZoneGroupCode", "ZoneGroupName");
            SearchBillingAndCollectionRates searchBillingAndCollectionRates = new SearchBillingAndCollectionRates();

            if (!string.IsNullOrEmpty(Category))
            {
                if (Category.ToUpper() == "SEWERAGE")
                {
                    TempData["TransactionSuccess"] = "ErrorSewerage";
                }
                else
                {
                    BillingRate billingRate = new BillingRate();
                    billingRate.Category = Category;
                    billingRate.ZoneGroup = ZoneGroup;

                    if (Category == "Rental Fee")
                        billingRate.SubCategory = "PHP Rental";
                    else
                        billingRate.SubCategory = Category;

                    billingRate.Rate = 0;

                    db.BillingRates.Add(billingRate);
                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Add";
                }
                searchBillingAndCollectionRates.BillingRate = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).ToList();
                ViewBag.Category = Category;
            }
            else
            {
                ViewBag.ShowAdd = false;
                Response.Write("<script>alert('Unable to add Category. Please check blank field.')</script>");
            }

            var bill = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
            var subcat = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

            foreach (var item in bill)
            {
                searchBillingAndCollectionRates.Category.Add(item.Category);
            }

            foreach (var item in subcat)
            {
                searchBillingAndCollectionRates.SubCategory.Add(item.SubCategory);
            }

            TempData["searchBillingAndCollectionRates"] = searchBillingAndCollectionRates;
            TempData["Category"] = Category;
            return RedirectToAction("ViewBillingAndCollectionRatesRPG", "MaintenanceBillingAndCollectionRates");
            //return View("ViewBillingAndCollectionRates", searchBillingAndCollectionRates);
        }

        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(string EditCategory, string Category)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            SearchBillingAndCollectionRates searchBillingAndCollectionRates = new SearchBillingAndCollectionRates();
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.Groups = db.ZoneGroup.ToList();

            if (!string.IsNullOrEmpty(Category))
            {
                db.Database.ExecuteSqlCommand("Update BillingRates set Category = '" + Category + "' where Category = '" + EditCategory + "'");
                searchBillingAndCollectionRates.BillingRate = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).ToList();
                ViewBag.Category = Category;
                TempData["TransactionSuccess"] = "Edit";
            }
            else
            {
                ViewBag.ShowAdd = false;
                Response.Write("<script>alert('Unable to edit Category. Please check blank field.')</script>");
            }

            var bill = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).GroupBy(m => m.Category).Select(g => new { Category = g.Key }).ToList();
            var subcat = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.Category == Category).GroupBy(m => m.SubCategory).Select(g => new { SubCategory = g.Key }).ToList();

            foreach (var item in bill)
            {
                searchBillingAndCollectionRates.Category.Add(item.Category);
            }

            foreach (var item in subcat)
            {
                searchBillingAndCollectionRates.SubCategory.Add(item.SubCategory);
            }

            TempData["searchBillingAndCollectionRates"] = searchBillingAndCollectionRates;
            TempData["Category"] = Category;
            return RedirectToAction("ViewBillingAndCollectionRatesRPG", "MaintenanceBillingAndCollectionRates");
            //return View("ViewBillingAndCollectionRates", searchBillingAndCollectionRates);
        }

        public ActionResult ViewRatesPRG()
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.NGASCode = TempData["NGASCode"] as string;
            List<BillingRate> temp = TempData["BillingRateList"] as List<BillingRate>;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            TempData.Keep("BillingRateList");
            ViewBag.Category = TempData["Category"] as string;
            return View("ViewRates", temp);
        }

        public ActionResult ViewRates(string SubCategory,string NGASCode)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            ViewBag.NGASCode = NGASCode;
            List<BillingRate> BillingRates = new List<BillingRate>();
            BillingRates = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.SubCategory == SubCategory).ToList();
            ViewBag.Category = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.SubCategory == SubCategory).FirstOrDefault().Category;
            return View(BillingRates);
        }

        [ValidateAntiForgeryToken]
        public ActionResult EditRates(int? BillRateIdTo, decimal? Rate, string SubCategory, string Category, string TransactionType,string NGAS)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Rate;
            TempData["Category"] = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.SubCategory == SubCategory).FirstOrDefault().Category;
            BillingRate BillingRates = new BillingRate();

            if (Rate != null)
            {
                if (TransactionType == "Add")
                {
                    List<BillingRate> billrate = db.BillingRates.Where(m => m.Category.ToUpper() == Category.ToUpper()
                        && m.SubCategory.ToUpper() == SubCategory.ToUpper() && m.ZoneGroup == ZoneGroup).ToList();

                    if (SubCategory.ToUpper() == "SEWERAGE")
                    {
                        TempData["TransactionSuccess"] = "ErrorSewerage";
                    }
                    else
                    {
                        BillingRates.Category = Category;
                        BillingRates.SubCategory = SubCategory;
                        BillingRates.Rate = Rate;
                        BillingRates.ZoneGroup = ZoneGroup;
                        BillingRates.NGASCode = NGAS;

                        db.BillingRates.Add(BillingRates);
                        db.SaveChanges();
                        TempData["TransactionSuccess"] = "Add";
                    }
                }
                else if (TransactionType == "Edit")
                {
                    BillingRates = db.BillingRates.Where(m => m.BillingRateId == BillRateIdTo).Single();
                    BillingRates.Rate = Rate;

                    db.Entry(BillingRates).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                }
                else if (TransactionType == "Delete")
                {
                    BillingRates = db.BillingRates.Where(m => m.BillingRateId == BillRateIdTo).Single();
                    var rates = db.BillingRates.Where(m => m.Category.ToUpper() == Category.ToUpper() && m.ZoneGroup == ZoneGroup).ToList();

                    if(rates.Count > 1)
                    {
                        db.Entry(BillingRates).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                        TempData["TransactionSuccess"] = "DeleteRate";
                    }
                    else
                    {
                        TempData["TransactionSuccess"] = "LessThanoneRate";
                    }
                   
                }
            }
            List<BillingRate> BillingRateList = new List<BillingRate>();
            BillingRateList = db.BillingRates.Where(x => x.ZoneGroup == ZoneGroup).Where(m => m.SubCategory == SubCategory).ToList();
            TempData["BillingRateList"] = BillingRateList;
            TempData["NGASCode"] = NGAS;
            return RedirectToAction("ViewRatesPRG", "MaintenanceBillingAndCollectionRates");
            //return View("ViewRates", BillingRateList);
        }

        // Generate Report
        public ActionResult BillingRatesReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fMaintenance%2fBillingRatesAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }
    }
}