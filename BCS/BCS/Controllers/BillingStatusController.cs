using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;

namespace BCS.Controllers
{
    public class BillingStatusController : Controller
    {
        private BCS_Context db = new BCS_Context();
        // GET: BillingStatus
        public ActionResult ViewDelinquent()
        {
            try
            {
                SearchIndexViewModel searchCompany = new SearchIndexViewModel();
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
                List<Company> NewCompanies = new List<Company>();
                // to be continue
                var CurrentDate = DateTime.Now;
                NewCompanies = db.Company.Select(c => c).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchCompany.CompanyList = searchCompanyPerGroup.Companies;
                foreach (var compItem in searchCompany.CompanyList)
                {
                    // DELINQUENT //
                    // --- BEGIN RENTAL --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "RENTAL" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId9"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId9"] = 0;
                    }
                    int ReSubId9 = Convert.ToInt32(TempData["SubId9"]);
                    if (ReSubId9 > 0)
                    {
                        var DelinRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "RENTAL" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId9).ToList();
                        foreach (var DelinRentlist in DelinRentalList)
                        {
                            if (Convert.ToDecimal(DelinRentlist.DebitAmount) - Convert.ToDecimal(DelinRentlist.CreditAmount) >= 1)
                            {
                                if (DelinRentalList.Select(x => x).Sum(x => x.DebitAmount) - DelinRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinRentlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinRentItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinRentItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Rental");
                                            searchCompany.DelinDueDate.Add(DelinRentlist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinRentlist.DebitAmount - DelinRentlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "RENTAL" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinRentlist in DelinRentalList)
                        {
                            if (Convert.ToDecimal(DelinRentlist.DebitAmount) - Convert.ToDecimal(DelinRentlist.CreditAmount) >= 1)
                            {
                                if (DelinRentalList.Select(x => x).Sum(x => x.DebitAmount) - DelinRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinRentlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinRentItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinRentItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Rental");
                                            searchCompany.DelinDueDate.Add(DelinRentlist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinRentlist.DebitAmount - DelinRentlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END RENTAL --- //
                    // --- BEGIN POLE RENTAL --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "POLE RENTAL" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId10"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId10"] = 0;
                    }
                    int ReSubId10 = Convert.ToInt32(TempData["SubId10"]);
                    if (ReSubId10 > 0)
                    {
                        var DelinPoleRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "POLE RENTAL" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId10).ToList();
                        foreach (var DelinPolelist in DelinPoleRentalList)
                        {
                            if (Convert.ToDecimal(DelinPolelist.DebitAmount) - Convert.ToDecimal(DelinPolelist.CreditAmount) >= 1)
                            {
                                if (DelinPoleRentalList.Select(x => x).Sum(x => x.DebitAmount) - DelinPoleRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinPoleRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinPolelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinPoleItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinPoleItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Pole Rental");
                                            searchCompany.DelinDueDate.Add(DelinPolelist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinPolelist.DebitAmount - DelinPolelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinPoleRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "POLE RENTAL" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinPolelist in DelinPoleRentalList)
                        {
                            if (Convert.ToDecimal(DelinPolelist.DebitAmount) - Convert.ToDecimal(DelinPolelist.CreditAmount) >= 1)
                            {
                                if (DelinPoleRentalList.Select(x => x).Sum(x => x.DebitAmount) - DelinPoleRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinPoleRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinPolelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinPoleItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinPoleItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Pole Rental");
                                            searchCompany.DelinDueDate.Add(DelinPolelist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinPolelist.DebitAmount - DelinPolelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END POLE RENTAL --- //
                    // --- BEGIN GARBAGE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "GARBAGE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId11"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId11"] = 0;
                    }
                    int ReSubId11 = Convert.ToInt32(TempData["SubId11"]);
                    if (ReSubId11 > 0)
                    {
                        var DelinGarbageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "GARBAGE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId11).ToList();
                        foreach (var DelinGarblist in DelinGarbageList)
                        {
                            if (Convert.ToDecimal(DelinGarblist.DebitAmount) - Convert.ToDecimal(DelinGarblist.CreditAmount) >= 1)
                            {
                                if (DelinGarbageList.Select(x => x).Sum(x => x.DebitAmount) - DelinGarbageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinGarbageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinGarblist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinGarbItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinGarbItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Garbage Fee");
                                            searchCompany.DelinDueDate.Add(DelinGarblist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinGarblist.DebitAmount - DelinGarblist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinGarbageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "GARBAGE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinGarblist in DelinGarbageList)
                        {
                            if (Convert.ToDecimal(DelinGarblist.DebitAmount) - Convert.ToDecimal(DelinGarblist.CreditAmount) >= 1)
                            {
                                if (DelinGarbageList.Select(x => x).Sum(x => x.DebitAmount) - DelinGarbageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinGarbageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinGarblist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinGarbItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinGarbItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Garbage Fee");
                                            searchCompany.DelinDueDate.Add(DelinGarblist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinGarblist.DebitAmount - DelinGarblist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END GARBAGE --- //
                    // --- BEGIN WATER --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "WATER" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId12"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId12"] = 0;
                    }
                    int ReSubId12 = Convert.ToInt32(TempData["SubId12"]);
                    if (ReSubId12 > 0)
                    {
                        var DelinWaterList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "WATER" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId12).ToList();
                        foreach (var DelinWatelist in DelinWaterList)
                        {
                            if (Convert.ToDecimal(DelinWatelist.DebitAmount) - Convert.ToDecimal(DelinWatelist.CreditAmount) >= 1)
                            {
                                if (DelinWaterList.Select(x => x).Sum(x => x.DebitAmount) - DelinWaterList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinWaterList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinWatelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinWateItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinWateItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Water");
                                            searchCompany.DelinDueDate.Add(DelinWatelist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinWatelist.DebitAmount - DelinWatelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinWaterList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "WATER" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinWatelist in DelinWaterList)
                        {
                            if (Convert.ToDecimal(DelinWatelist.DebitAmount) - Convert.ToDecimal(DelinWatelist.CreditAmount) >= 1)
                            {
                                if (DelinWaterList.Select(x => x).Sum(x => x.DebitAmount) - DelinWaterList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinWaterList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinWatelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinWateItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinWateItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Water");
                                            searchCompany.DelinDueDate.Add(DelinWatelist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinWatelist.DebitAmount - DelinWatelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END WATER --- //
                    // --- BEGIN FRANCHISE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "FRANCHISE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId13"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId13"] = 0;
                    }
                    int ReSubId13 = Convert.ToInt32(TempData["SubId13"]);
                    if (ReSubId13 > 0)
                    {
                        var DelinFranchiseList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "FRANCHISE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId13).ToList();
                        foreach (var DelinFranlist in DelinFranchiseList)
                        {
                            if (Convert.ToDecimal(DelinFranlist.DebitAmount) - Convert.ToDecimal(DelinFranlist.CreditAmount) >= 1)
                            {
                                if (DelinFranchiseList.Select(x => x).Sum(x => x.DebitAmount) - DelinFranchiseList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinFranchiseList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinFranlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinFranItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinFranItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Franchise Fee");
                                            searchCompany.DelinDueDate.Add(DelinFranlist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinFranlist.DebitAmount - DelinFranlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinFranchiseList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "FRANCHISE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinFranlist in DelinFranchiseList)
                        {
                            if (Convert.ToDecimal(DelinFranlist.DebitAmount) - Convert.ToDecimal(DelinFranlist.CreditAmount) >= 1)
                            {
                                if (DelinFranchiseList.Select(x => x).Sum(x => x.DebitAmount) - DelinFranchiseList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinFranchiseList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinFranlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinFranItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinFranItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Franchise Fee");
                                            searchCompany.DelinDueDate.Add(DelinFranlist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinFranlist.DebitAmount - DelinFranlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END FRANCHISE --- //
                    // --- BEGIN PASSONBILLING --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "PASSED ON BILLING" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId14"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId14"] = 0;
                    }
                    int ReSubId14 = Convert.ToInt32(TempData["SubId14"]);
                    if (ReSubId14 > 0)
                    {
                        var DelinPassedOnBillingList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "PASSED ON BILLING" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId14).ToList();
                        foreach (var DelinPasslist in DelinPassedOnBillingList)
                        {
                            if (Convert.ToDecimal(DelinPasslist.DebitAmount) - Convert.ToDecimal(DelinPasslist.CreditAmount) >= 1)
                            {
                                if (DelinPassedOnBillingList.Select(x => x).Sum(x => x.DebitAmount) - DelinPassedOnBillingList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinPassedOnBillingList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinPasslist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinPassItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinPassItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Pass On Billing");
                                            searchCompany.DelinDueDate.Add(DelinPasslist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinPasslist.DebitAmount - DelinPasslist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinPassedOnBillingList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "PASSED ON BILLING" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinPasslist in DelinPassedOnBillingList)
                        {
                            if (Convert.ToDecimal(DelinPasslist.DebitAmount) - Convert.ToDecimal(DelinPasslist.CreditAmount) >= 1)
                            {
                                if (DelinPassedOnBillingList.Select(x => x).Sum(x => x.DebitAmount) - DelinPassedOnBillingList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinPassedOnBillingList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinPasslist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinPassItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinPassItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Pass On Billing");
                                            searchCompany.DelinDueDate.Add(DelinPasslist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinPasslist.DebitAmount - DelinPasslist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END PASSONBILLING --- //
                    // --- BEGIN SEWERAGE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "SEWERAGE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId15"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId15"] = 0;
                    }
                    int ReSubId15 = Convert.ToInt32(TempData["SubId15"]);
                    if (ReSubId15 > 0)
                    {
                        var DelinSewerageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "SEWERAGE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId15).ToList();
                        foreach (var DelinSewelist in DelinSewerageList)
                        {
                            if (Convert.ToDecimal(DelinSewelist.DebitAmount) - Convert.ToDecimal(DelinSewelist.CreditAmount) >= 1)
                            {
                                if (DelinSewerageList.Select(x => x).Sum(x => x.DebitAmount) - DelinSewerageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinSewerageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinSewelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinSeweItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinSeweItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Sewerage");
                                            searchCompany.DelinDueDate.Add(DelinSewelist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinSewelist.DebitAmount - DelinSewelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinSewerageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "SEWERAGE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinSewelist in DelinSewerageList)
                        {
                            if (Convert.ToDecimal(DelinSewelist.DebitAmount) - Convert.ToDecimal(DelinSewelist.CreditAmount) >= 1)
                            {
                                if (DelinSewerageList.Select(x => x).Sum(x => x.DebitAmount) - DelinSewerageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinSewerageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinSewelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinSeweItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinSeweItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Sewerage");
                                            searchCompany.DelinDueDate.Add(DelinSewelist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinSewelist.DebitAmount - DelinSewelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END SEWERAGE --- //
                    // --- BEGIN ADMIN --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "ADMIN FEE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId16"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId16"] = 0;
                    }
                    int ReSubId16 = Convert.ToInt32(TempData["SubId16"]);
                    if (ReSubId16 > 0)
                    {
                        var DelinAdminFeeList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "ADMIN FEE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate && x.SubsidiaryLedgerId > ReSubId16).ToList();
                        foreach (var DelinAdmilist in DelinAdminFeeList)
                        {
                            if (Convert.ToDecimal(DelinAdmilist.DebitAmount) - Convert.ToDecimal(DelinAdmilist.CreditAmount) >= 1)
                            {
                                if (DelinAdminFeeList.Select(x => x).Sum(x => x.DebitAmount) - DelinAdminFeeList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinAdminFeeList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinAdmilist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinAdmiItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinAdmiItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Admin Fee");
                                            searchCompany.DelinDueDate.Add(DelinAdmilist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinAdmilist.DebitAmount - DelinAdmilist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var DelinAdminFeeList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "ADMIN FEE" && x.CompanyId == compItem.CompanyID && CurrentDate > x.DueDate).ToList();
                        foreach (var DelinAdmilist in DelinAdminFeeList)
                        {
                            if (Convert.ToDecimal(DelinAdmilist.DebitAmount) - Convert.ToDecimal(DelinAdmilist.CreditAmount) >= 1)
                            {
                                if (DelinAdminFeeList.Select(x => x).Sum(x => x.DebitAmount) - DelinAdminFeeList.Select(y => y).Sum(y => y.CreditAmount) > 0 || DelinAdminFeeList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + DelinAdmilist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var DelinAdmiItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.DelinCompanyName.Add(DelinAdmiItems.CompanyName);
                                            searchCompany.DelinBillingType.Add("Admin Fee");
                                            searchCompany.DelinDueDate.Add(DelinAdmilist.DueDate);
                                            searchCompany.DelinAmount.Add(DelinAdmilist.DebitAmount - DelinAdmilist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END ADMIN FEE --- //
                    // ----------- //
                }
                return View("ViewDelinquent", searchCompany);
            }
            catch
            {
                SearchIndexViewModel searchComp = new SearchIndexViewModel();
                searchComp.DelinCompanyName = null;
                return View("ViewDelinquent", searchComp);
            }
        }

        public ActionResult ViewDueOnServices()
        {
            try
            {
                SearchIndexViewModel searchCompany = new SearchIndexViewModel();
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
                List<Company> NewCompanies = new List<Company>();
                // to be continue
                var CurrentDate = DateTime.Now;

                DateTime today = DateTime.Today;
                DateTime thirtyDaysAgo = today.AddDays(-30);
                DateTime thirtyDaysFromNow = today.AddDays(30);
                DateTime ninetyDaysAgo = today.AddDays(-90);
                DateTime ninetyDaysFromNow = today.AddDays(90);

                NewCompanies = db.Company.Select(c => c).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchCompany.CompanyList = searchCompanyPerGroup.Companies;
                foreach (var compItem in searchCompany.CompanyList)
                {
                    // DUE ON SERVICES //
                    var DueRentalList = db.RentalInformation.Where(q => CurrentDate <= q.EndDate && q.EndDate >= thirtyDaysAgo && q.EndDate <= thirtyDaysFromNow && q.CompanyId == compItem.CompanyID).OrderBy(q => q.EndDate).ToList();
                    var DuePoleRental = db.PoleInformation.Where(x => CurrentDate <= x.EndDate && x.EndDate >= thirtyDaysAgo && x.EndDate <= thirtyDaysFromNow && x.CompanyId == compItem.CompanyID).OrderBy(x => x.EndDate).ToList();
                    var DueWater = db.WaterMeterAssignment.Where(y => CurrentDate <= y.EndDate && y.EndDate >= thirtyDaysAgo && y.EndDate <= thirtyDaysFromNow && y.CompanyId == compItem.CompanyID).OrderBy(y => y.EndDate).ToList();
                    var DueFranchise = db.FranchiseFeeInformation.Where(k => CurrentDate <= k.EndDate && k.EndDate >= thirtyDaysAgo && k.EndDate <= thirtyDaysFromNow && k.CompanyId == compItem.CompanyID).OrderBy(k => k.EndDate).ToList();

                    // Rental //
                    if (DueRentalList.Count > 0)
                    {
                        foreach (var RentalItems in DueRentalList)
                        {
                            searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + RentalItems.CompanyId + "'").ToList();
                            if (searchCompany.CompanyList.Count == 1)
                            {
                                foreach (var RentalItemss in searchCompany.CompanyList)
                                {
                                    searchCompany.DueCompanyName.Add(RentalItemss.CompanyName);
                                }
                            }
                            searchCompany.DueBillingType.Add("Rental");
                            searchCompany.DueEndDate.Add(RentalItems.EndDate);
                        }
                    }

                    // -------------------------------- //

                    // Pole Rental //

                    if (DuePoleRental.Count > 0)
                    {
                        foreach (var poleItems in DuePoleRental)
                        {
                            searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + poleItems.CompanyId + "'").ToList();
                            if (searchCompany.CompanyList.Count == 1)
                            {
                                foreach (var poleItemss in searchCompany.CompanyList)
                                {
                                    searchCompany.DueCompanyName.Add(poleItemss.CompanyName);
                                }
                            }
                            searchCompany.DueBillingType.Add("Pole Rental");
                            searchCompany.DueEndDate.Add(poleItems.EndDate);
                        }
                    }

                    // -------------------------------- //

                    // Water //

                    if (DueWater.Count > 0)
                    {
                        foreach (var WaterItems in DueWater)
                        {
                            searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + WaterItems.CompanyId + "'").ToList();
                            if (searchCompany.CompanyList.Count == 1)
                            {
                                foreach (var WaterItemss in searchCompany.CompanyList)
                                {
                                    searchCompany.DueCompanyName.Add(WaterItemss.CompanyName);
                                }
                            }
                            searchCompany.DueBillingType.Add("Water");
                            searchCompany.DueEndDate.Add(WaterItems.EndDate);
                        }
                    }

                    // -------------------------------- //

                    // Franchise //

                    if (DueFranchise.Count > 0)
                    {
                        foreach (var FranchiseItems in DueFranchise)
                        {
                            searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + FranchiseItems.CompanyId + "'").ToList();
                            if (searchCompany.CompanyList.Count == 1)
                            {
                                foreach (var FranchiseItemss in searchCompany.CompanyList)
                                {
                                    searchCompany.DueCompanyName.Add(FranchiseItemss.CompanyName);
                                }
                            }
                            searchCompany.DueBillingType.Add("Franchise");
                            searchCompany.DueEndDate.Add(FranchiseItems.EndDate);
                        }
                    }
                }
                return View("ViewDueOnServices", searchCompany);
            }
            catch
            {
                SearchIndexViewModel searchComp = new SearchIndexViewModel();
                searchComp.DueCompanyName = null;
                return View("ViewDueOnServices", searchComp);
            }
            // --- END DUE ON SERVICES --- //  
        }

        public ActionResult ViewUnpaidCompany()
        {
            try
            {
                SearchIndexViewModel searchCompany = new SearchIndexViewModel();
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
                List<Company> NewCompanies = new List<Company>();

                NewCompanies = db.Company.Select(c => c).ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                searchCompany.CompanyList = searchCompanyPerGroup.Companies;
                foreach (var compItem in searchCompany.CompanyList)
                {
                    // ----- UNPAID COMPANY ----- //
                    // --- RENTAL --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "RENTAL" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId1"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId1"] = 0;
                    }
                    int ReSubId = Convert.ToInt32(TempData["SubId1"]);
                    if (ReSubId > 0)
                    {
                        var UnpaidRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "RENTAL" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId).ToList();
                        foreach (var Rentlist in UnpaidRentalList)
                        {
                            if (Convert.ToDecimal(Rentlist.DebitAmount) - Convert.ToDecimal(Rentlist.CreditAmount) >= 1)
                            {
                                if (UnpaidRentalList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 && UnpaidRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Rentlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var RentItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(RentItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(RentItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(RentItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Rentlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Rental");
                                            searchCompany.UnpaidAmount.Add(Rentlist.DebitAmount - Rentlist.CreditAmount);
                                        }
                                    }
                                }
                                else { }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "RENTAL" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Rentlist in UnpaidRentalList)
                        {
                            if (Convert.ToDecimal(Rentlist.DebitAmount) - Convert.ToDecimal(Rentlist.CreditAmount) >= 1)
                            {
                                if (UnpaidRentalList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 && UnpaidRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Rentlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var RentItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(RentItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(RentItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(RentItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Rentlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Rental");
                                            searchCompany.UnpaidAmount.Add(Rentlist.DebitAmount - Rentlist.CreditAmount);
                                        }
                                    }
                                }
                                else { }
                            }
                        }
                    }
                    // --- END RENTAL --- //
                    // --- POLE RENTAL --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "POLE RENTAL" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId2"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId2"] = 0;
                    }
                    int ReSubId2 = Convert.ToInt32(TempData["SubId2"]);
                    if (ReSubId2 > 0)
                    {
                        var UnpaidPoleRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "POLE RENTAL" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId2).ToList();
                        foreach (var Polelist in UnpaidPoleRentalList)
                        {
                            if (Convert.ToDecimal(Polelist.DebitAmount) - Convert.ToDecimal(Polelist.CreditAmount) >= 1)
                            {
                                if (UnpaidPoleRentalList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidPoleRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidPoleRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Polelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var PoleItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(PoleItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(PoleItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(PoleItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Polelist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Pole Rental");
                                            searchCompany.UnpaidAmount.Add(Polelist.DebitAmount - Polelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidPoleRentalList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "POLE RENTAL" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Polelist in UnpaidPoleRentalList)
                        {
                            if (Convert.ToDecimal(Polelist.DebitAmount) - Convert.ToDecimal(Polelist.CreditAmount) >= 1)
                            {
                                if (UnpaidPoleRentalList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidPoleRentalList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidPoleRentalList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Polelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var PoleItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(PoleItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(PoleItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(PoleItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Polelist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Pole Rental");
                                            searchCompany.UnpaidAmount.Add(Polelist.DebitAmount - Polelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END POLE RENTAL --- //
                    // --- GARBAGE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "GARBAGE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId3"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId3"] = 0;
                    }
                    int ReSubId3 = Convert.ToInt32(TempData["SubId3"]);
                    if (ReSubId3 > 0)
                    {
                        var UnpaidGarbageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "GARBAGE" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId3).ToList();
                        foreach (var Garblist in UnpaidGarbageList)
                        {
                            if (Convert.ToDecimal(Garblist.DebitAmount) - Convert.ToDecimal(Garblist.CreditAmount) >= 1)
                            {
                                if (UnpaidGarbageList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidGarbageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidGarbageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Garblist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var GarbItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(GarbItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(GarbItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(GarbItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Garblist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Garbage Fee");
                                            searchCompany.UnpaidAmount.Add(Garblist.DebitAmount - Garblist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidGarbageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "GARBAGE" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Garblist in UnpaidGarbageList)
                        {
                            if (Convert.ToDecimal(Garblist.DebitAmount) - Convert.ToDecimal(Garblist.CreditAmount) >= 1)
                            {
                                if (UnpaidGarbageList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidGarbageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidGarbageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Garblist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var GarbItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(GarbItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(GarbItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(GarbItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Garblist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Garbage Fee");
                                            searchCompany.UnpaidAmount.Add(Garblist.DebitAmount - Garblist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END GARBAGE --- //
                    // --- BEGIN WATER --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "WATER" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId4"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId4"] = 0;
                    }
                    int ReSubId4 = Convert.ToInt32(TempData["SubId4"]);
                    if (ReSubId4 > 0)
                    {
                        var UnpaidWaterList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "WATER" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId4).ToList();
                        foreach (var Watelist in UnpaidWaterList)
                        {
                            if (Convert.ToDecimal(Watelist.DebitAmount) - Convert.ToDecimal(Watelist.CreditAmount) >= 1)
                            {
                                if (UnpaidWaterList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidWaterList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidWaterList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Watelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var WateItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(WateItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(WateItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(WateItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Watelist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Water");
                                            searchCompany.UnpaidAmount.Add(Watelist.DebitAmount - Watelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidWaterList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "WATER" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Watelist in UnpaidWaterList)
                        {
                            if (Convert.ToDecimal(Watelist.DebitAmount) - Convert.ToDecimal(Watelist.CreditAmount) >= 1)
                            {
                                if (UnpaidWaterList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidWaterList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidWaterList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Watelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var WateItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(WateItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(WateItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(WateItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Watelist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Water");
                                            searchCompany.UnpaidAmount.Add(Watelist.DebitAmount - Watelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END WATER --- //
                    // --- BEGIN FRANCHISE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "FRANCHISE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId5"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId5"] = 0;
                    }
                    int ReSubId5 = Convert.ToInt32(TempData["SubId5"]);
                    if (ReSubId5 > 0)
                    {
                        var UnpaidFranchiseList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "FRANCHISE" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId5).ToList();
                        foreach (var Franlist in UnpaidFranchiseList)
                        {
                            if (Convert.ToDecimal(Franlist.DebitAmount) - Convert.ToDecimal(Franlist.CreditAmount) >= 1)
                            {
                                if (UnpaidFranchiseList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidFranchiseList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidFranchiseList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Franlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var FranItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(FranItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(FranItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(FranItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Franlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Franchise");
                                            searchCompany.UnpaidAmount.Add(Franlist.DebitAmount - Franlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidFranchiseList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "FRANCHISE" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Franlist in UnpaidFranchiseList)
                        {
                            if (Convert.ToDecimal(Franlist.DebitAmount) - Convert.ToDecimal(Franlist.CreditAmount) >= 1)
                            {
                                if (UnpaidFranchiseList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidFranchiseList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidFranchiseList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Franlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var FranItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(FranItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(FranItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(FranItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Franlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Franchise");
                                            searchCompany.UnpaidAmount.Add(Franlist.DebitAmount - Franlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END FRANCHISE --- //
                    // --- BEGIN PASSONBILLING --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "PASSED ON BILLING" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId6"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId6"] = 0;
                    }
                    int ReSubId6 = Convert.ToInt32(TempData["SubId6"]);
                    if (ReSubId6 > 0)
                    {
                        var UnpaidPassedOnBillingList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "PASSED ON BILLING" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId6).ToList();
                        foreach (var Passlist in UnpaidPassedOnBillingList)
                        {
                            if (Convert.ToDecimal(Passlist.DebitAmount) - Convert.ToDecimal(Passlist.CreditAmount) >= 1)
                            {
                                if (UnpaidPassedOnBillingList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidPassedOnBillingList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidPassedOnBillingList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Passlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var PassItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(PassItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(PassItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(PassItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Passlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Pass On Billing");
                                            searchCompany.UnpaidAmount.Add(Passlist.DebitAmount - Passlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidPassedOnBillingList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "PASSED ON BILLING" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Passlist in UnpaidPassedOnBillingList)
                        {
                            if (Convert.ToDecimal(Passlist.DebitAmount) - Convert.ToDecimal(Passlist.CreditAmount) >= 1)
                            {
                                if (UnpaidPassedOnBillingList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidPassedOnBillingList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidPassedOnBillingList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Passlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var PassItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(PassItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(PassItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(PassItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Passlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Pass On Billing");
                                            searchCompany.UnpaidAmount.Add(Passlist.DebitAmount - Passlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END PASSONBILLING --- //
                    // --- BEGIN SEWERAGE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "SEWERAGE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId7"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId7"] = 0;
                    }
                    int ReSubId7 = Convert.ToInt32(TempData["SubId7"]);
                    if (ReSubId7 > 0)
                    {
                        var UnpaidSewerageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "SEWERAGE" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId7).ToList();
                        foreach (var Sewelist in UnpaidSewerageList)
                        {
                            if (Convert.ToDecimal(Sewelist.DebitAmount) - Convert.ToDecimal(Sewelist.CreditAmount) >= 1)
                            {
                                if (UnpaidSewerageList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidSewerageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidSewerageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Sewelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var SeweItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(SeweItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(SeweItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(SeweItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Sewelist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Sewerage");
                                            searchCompany.UnpaidAmount.Add(Sewelist.DebitAmount - Sewelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidSewerageList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "SEWERAGE" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Sewelist in UnpaidSewerageList)
                        {
                            if (Convert.ToDecimal(Sewelist.DebitAmount) - Convert.ToDecimal(Sewelist.CreditAmount) >= 1)
                            {
                                if (UnpaidSewerageList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidSewerageList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidSewerageList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Sewelist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var SeweItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(SeweItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(SeweItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(SeweItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Sewelist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Sewerage");
                                            searchCompany.UnpaidAmount.Add(Sewelist.DebitAmount - Sewelist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END SEWERAGE --- //
                    // --- BEGIN ADMINFEE --- //
                    try
                    {
                        var SubId = db.SubsidiaryLedger.Where(o => o.TransactionType == "PAYMENT" && o.BillingType == "ADMIN FEE" && o.CompanyId == compItem.CompanyID).ToList();
                        TempData["SubId8"] = SubId.LastOrDefault(x => x.TransactionType == "PAYMENT").SubsidiaryLedgerId;
                    }
                    catch
                    {
                        TempData["SubId8"] = 0;
                    }
                    int ReSubId8 = Convert.ToInt32(TempData["SubId8"]);
                    if (ReSubId8 > 0)
                    {
                        var UnpaidAdminFeeList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "ADMIN FEE" && x.CompanyId == compItem.CompanyID && x.SubsidiaryLedgerId > ReSubId8).ToList();
                        foreach (var Adminlist in UnpaidAdminFeeList)
                        {
                            if (Convert.ToDecimal(Adminlist.DebitAmount) - Convert.ToDecimal(Adminlist.CreditAmount) >= 1)
                            {
                                if (UnpaidAdminFeeList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidAdminFeeList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidAdminFeeList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Adminlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var AdmiItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(AdmiItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(AdmiItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(AdmiItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Adminlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Admin Fee");
                                            searchCompany.UnpaidAmount.Add(Adminlist.DebitAmount - Adminlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var UnpaidAdminFeeList = db.SubsidiaryLedger.Where(x => x.BillingType.ToUpper() == "ADMIN FEE" && x.CompanyId == compItem.CompanyID).ToList();
                        foreach (var Adminlist in UnpaidAdminFeeList)
                        {
                            if (Convert.ToDecimal(Adminlist.DebitAmount) - Convert.ToDecimal(Adminlist.CreditAmount) >= 1)
                            {
                                if (UnpaidAdminFeeList.Select(x => x).Sum(x => x.DebitAmount) - UnpaidAdminFeeList.Select(y => y).Sum(y => y.CreditAmount) > 0 || UnpaidAdminFeeList.LastOrDefault().TransactionType.ToUpper().ToString() == "BILLING")
                                {
                                    searchCompany.CompanyList = db.Company.SqlQuery("SELECT * FROM Companies WHERE CompanyID = '" + Adminlist.CompanyId + "'").ToList();
                                    if (searchCompany.CompanyList.Count == 1)
                                    {
                                        foreach (var AdmiItems in searchCompany.CompanyList)
                                        {
                                            searchCompany.UnpaidCompanyName.Add(AdmiItems.CompanyName);
                                            searchCompany.UnpaidCompanyAddress.Add(AdmiItems.Address);
                                            searchCompany.UnpaidEnterpriseType.Add(AdmiItems.EnterpriseType);
                                            searchCompany.UnpaidDueDate.Add(Adminlist.DueDate);
                                            searchCompany.UnpaidBillingType.Add("Admin Fee");
                                            searchCompany.UnpaidAmount.Add(Adminlist.DebitAmount - Adminlist.CreditAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // --- END ADMINFEE --- //
                    // --- END UNPAID COMPANY --- //
                }
                return View("ViewUnpaidCompany", searchCompany);
            }
            catch
            {
                SearchIndexViewModel searchComp = new SearchIndexViewModel();
                searchComp.UnpaidCompanyName = null;
                return View("ViewUnpaidCompany", searchComp);
            }
        }
    }
}