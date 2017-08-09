using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;

namespace BCS.Controllers
{
    public class ReportsCollectionController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        ApplicationDbContext context = new ApplicationDbContext();

        private BCS_Context db = new BCS_Context();
        // GET: ReportsCollection
        public ActionResult ViewCollection()
        {
            SearchReportsCollectionModels srchZG = new SearchReportsCollectionModels();

            var userId = User.Identity.GetUserId();

            srchZG.ZoneGroupList = db.ZoneGroup.SqlQuery("SELECT * FROM ZoneGroups EXCEPT SELECT * FROM ZoneGroups WHERE ZoneGroupName = 'Super User'").ToList();
            srchZG.ZoneList = db.Zone.SqlQuery("SELECT DISTINCT * FROM Zones").ToList();
            srchZG.Accounts = db.Database.SqlQuery<AccountList>("SELECT DISTINCT OPAccountCode, OPAccountDescription FROM OPAccounts ORDER BY OPAccountDescription").ToList();
            srchZG.OPAccountList = db.OPAccount.OrderBy(x => x.OPAccountDescription).ToList();
            srchZG.DivisionList = db.Division.Select(x => x).ToList();

            ReportTableBLL rpt = new ReportTableBLL();

            srchZG.RTlist = rpt.RTTables();

            return View("ViewCollection", srchZG);
        }

        [HttpPost]
        public JsonResult returnZone(string a)
        {
            BillingStatementViewModel billingStatementViewModel = new BillingStatementViewModel();
            int zonegroupid = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == a).ZoneGroupId;
            var user = context.Roles.ToList();
            var cashiers = user.Where(m => m.Name == "Cashier").ToList();
            List<String> applicationUsers = new List<String>();

            var userid = User.Identity.GetUserId();
            string zoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            var ZoneList = db.Database.SqlQuery<DeveloperDetails>("SELECT DISTINCT Zones.ZoneName, Zones.ZoneCode FROM AspNetUserRoles INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId INNER JOIN AspNetUsers ON AspNetUserRoles.UserId = AspNetUsers.Id INNER JOIN Zones ON Zones.ZoneId = AspNetUsers.Zone WHERE AspNetRoles.RoleNumber = 7 AND AspNetUsers.ZoneGroup = '" + zoneGroup + "'").ToList();
            //foreach (var item in cashiers)
            //{
            //    var ab = item.Users;
            //    foreach (var item1 in item.Users)
            //    {
            //        try
            //        {
            //            var cd = item1.UserId;
            //            var ef = context.Users.Where(m => m.Id == cd).FirstOrDefault();
            //            if (ef.ZoneGroup == a)
            //            {
            //                int zoneid = 0;
            //                if (int.TryParse(ef.Zone, out zoneid))
            //                    zoneid = int.Parse(ef.Zone);

            //                var userZone = db.Zone.Any(m => m.ZoneId == zoneid) ? db.Zone.Where(m => m.ZoneId == zoneid).FirstOrDefault().ZoneName : "Zone name not found.";
            //                applicationUsers.Add(userZone);
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //            throw;
            //        }
                    
            //    }
            //}
            ////var asd = applicationUsers.ToList();
            //billingStatementViewModel.zoneName = applicationUsers.Distinct().ToList();
            //billingStatementViewModel.billingPeriod = db.BillingPeriod.Where(m => m.groupCode == a).ToList();
            return Json(ZoneList);
        }
        


        [HttpPost]
        public ActionResult ViewCollection(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            NameValueCollection settings = new NameValueCollection();

            switch (zoneGroup)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/COLLECTIONS");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/COLLECTIONS");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/COLLECTIONS");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/COLLECTIONS");
                    break;

                default:
                    break;
            }


            string collectedBy = settings["collectedBy"].ToString();
            string approvedBy = settings["approvedBy"].ToString();

            string accntOff = settings["accntOff"].ToString();
            string officialDest = settings["officialDest"].ToString();
            string officeDest = settings["officeDest"].ToString();

            var zoneName = "";
            var zoneGroupCode = frm["zoneGroupCode"];            

            var zone = frm["zoneName"];

            if (zone == "All")
            {
                 zoneName = zone;
            }
            else
            {
                var zoneList = zone.Split('|');
                zoneName = zoneList[0];
                var zoneCode = zoneList[1];
            }

            var reportType = frm["ReportType"];
            //var reportName = frm["ReportName"];
            var reportname = Regex.Replace(frm["ReportName"], @"\s+", "");

            string input = frm["ReportName"];
            string patt = ",";

            string[] substrings = Regex.Split(input, patt);



            var groupedStartDate = frm["groupedStartDate"];
            var groupedEndDate = frm["groupedEndDate"];
            var groupedARType = frm["groupedARType"];

            groupedARType = groupedARType.Replace(",", "");
            groupedEndDate = groupedEndDate.Replace(",", "");
            groupedStartDate = groupedStartDate.Replace(",", "");

            var groupedCollectionStartDate = frm["groupedCollectionStartDate"];
            var groupedCollectionEndDate = frm["groupedCollectionEndDate"];
            var groupedCollectionType = frm["groupedCollectionType"];

            var tdStartDate = frm["tdStartDate"];
            var tdEndDate = frm["tdEndDate"];
            var DivisionCode = frm["DivisionCode"];

            var currencyType = frm["currency"];

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);

            if (substrings.Length == 2)
            {

                string reportnamex = substrings[1];
                var reportnamexx = Regex.Replace(reportnamex, @"\s+", "");
                switch (reportType)
                {

                    // Collection Reports
                    case "Collection":

                        switch (reportnamexx)
                        {
                            case "MonthlyReportCollection":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlyReportCollection&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "MonthlySummaryCollectionPerAccount":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlySummaryCollectionperAccount&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&collectedBy=" + collectedBy + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "FinalValueAddedTaxWithheldByPayor(Form2306)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fFinalValueAddedTaxWithHeldByPayor(Form2306)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "CollectionReportGrossNet":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCollectionReportGrossNet&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedCollectionStartDate=" + groupedCollectionStartDate + "&groupedCollectionEndDate=" + groupedCollectionEndDate + "&groupedCollectionType=" + groupedCollectionType + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&collectedBy=" + collectedBy;
                                return Redirect(serverURI.Uri.ToString());

                            case "CollectionReportInterest":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCollectionReportInterest&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedCollectionStartDate=" + groupedCollectionStartDate + "&groupedCollectionEndDate=" + groupedCollectionEndDate + "&groupedCollectionType=" + groupedCollectionType + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&collectedBy=" + collectedBy;
                                return Redirect(serverURI.Uri.ToString());

                            case "ExpandedWithHoldingTaxWithHeldByPayor":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fExpandedWithHoldingTaxWithHeldByPayor(Form2307)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "CollectionReportVAT":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCollectionReportVAT&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&approvedBy=" + approvedBy;
                                return Redirect(serverURI.Uri.ToString());

                            case "MonthlySummaryofDeposits":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlySummaryofDeposits&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "MonthlySummaryReportCollectionPerRevenueItem":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlySummaryReportofCollectionsperRevenueItem&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ComparativeSummaryReportPerRevenueItem":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fComparativeSummaryReportPerRevenueItem&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "DollarPaymentsCollection":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fDollarPaymentsCollection&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionPerUnitResponsibilityCenter":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListOfCollectionPerUnitResponsibilityCenter&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&divisionCode=" + DivisionCode;
                                return Redirect(serverURI.Uri.ToString());

                            case "BillingCollectionReportMonthlyDaily(USD)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fBillingCollectionReportMonthlyDaily(USD)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionDuetoHeadOffice(421)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCollectionDuetoHeadOffice(421)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionDuefromOtherZones(142)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCollectionDuefromOtherZones(142)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            default:
                                break;
                        }
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Collection - View Collection Reports  - from Terminal: " + ipaddress);
                        break;

                    // Official Receipt Reports
                    case "OfficialReceipt":

                        switch (reportnamexx)
                        {
                            case "ListofCheckReceived":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCheckReceived&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCancelledOfficialReceipt":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCancelledOfficialReceipt&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "PaymentDetails":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fPaymentDetails(ORDetails)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;
                                return Redirect(serverURI.Uri.ToString());

                            case "CashReceiptsAndDepositsRecord":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCashReceiptsAndDepositsRecords&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&collectedBy=" + collectedBy + "&accntOff=" + accntOff + "&officialDest=" + officialDest + "&officeDest=" + officeDest;
                                return Redirect(serverURI.Uri.ToString());

                            case "IssuedOR":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fIssuedOR&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ORswith121":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fOR%27swith121&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            default:
                                break;
                        }
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Collection - View Official Receipt Reports  - from Terminal: " + ipaddress);

                        break;

                    // Other Reports
                    case "Others":

                        switch (reportnamexx)
                        {
                            case "AccountableForms":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAccountableForms&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionPerAccountCode":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCollectionPerAccountCode&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "SerialNumber":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fSerialNumber&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ReportofPayment-EngineeringFees":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fReportofPayment-EngineeringFees&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&zoneName=" + zoneName + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate;
                                return Redirect(serverURI.Uri.ToString());

                            case "AdvancesAlphaList":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAdvancesAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "VISAApplicationReport":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fVisaApplicationReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "OrderofPaymentSummaryReport":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fOrderofPaymentSummaryReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "AccountabilityforAccountableFormsReport":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAccountabilityforAccountableFormsReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "StatementOfAccount(Payment)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fReports%2fStatementOfAccount(Payment)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "AccountDescription":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAccountDescription&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "VATAlphalist":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fVATAlphalist&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            default:
                                break;
                        }

                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Collection - View Others Reports  - from Terminal: " + ipaddress);

                        break;

                    default:
                        break;
                }

            }
            else
            {
                switch (reportType)
                {

                    // Collection Reports
                    case "Collection":

                        switch (reportname)
                        {
                            case "MonthlyReportCollection":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlyReportCollection&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "MonthlySummaryCollectionPerAccount":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlySummaryCollectionperAccount&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&collectedBy=" + collectedBy + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "FinalValueAddedTaxWithheldByPayor(Form2306)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fFinalValueAddedTaxWithHeldByPayor(Form2306)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "CollectionReportGrossNet":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCollectionReportGrossNet&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedCollectionStartDate=" + groupedCollectionStartDate + "&groupedCollectionEndDate=" + groupedCollectionEndDate + "&groupedCollectionType=" + groupedCollectionType + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&collectedBy=" + collectedBy;
                                return Redirect(serverURI.Uri.ToString());

                            case "CollectionReportInterest":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCollectionReportInterest&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedCollectionStartDate=" + groupedCollectionStartDate + "&groupedCollectionEndDate=" + groupedCollectionEndDate + "&groupedCollectionType=" + groupedCollectionType + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&collectedBy=" + collectedBy;
                                return Redirect(serverURI.Uri.ToString());

                            case "ExpandedWithHoldingTaxWithHeldByPayor":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fExpandedWithHoldingTaxWithHeldByPayor(Form2307)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "CollectionReportVAT":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCollectionReportVAT&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&approvedBy=" + approvedBy;
                                return Redirect(serverURI.Uri.ToString());

                            case "MonthlySummaryofDeposits":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlySummaryofDeposits&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "MonthlySummaryReportCollectionPerRevenueItem":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fMonthlySummaryReportofCollectionsperRevenueItem&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ComparativeSummaryReportPerRevenueItem":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fComparativeSummaryReportPerRevenueItem&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "DollarPaymentsCollection":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fDollarPaymentsCollection&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionPerUnitResponsibilityCenter":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListOfCollectionPerUnitResponsibilityCenter&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&divisionCode=" + DivisionCode;
                                return Redirect(serverURI.Uri.ToString());

                            case "BillingCollectionReportMonthlyDaily(USD)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fBillingCollectionReportMonthlyDaily(USD)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionDuetoHeadOffice(421)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCollectionDuetoHeadOffice(421)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionDuefromOtherZones(142)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCollectionDuefromOtherZones(142)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            default:
                                break;
                        }
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Collection - View Collection Reports  - from Terminal: " + ipaddress);
                        break;

                    // Official Receipt Reports
                    case "OfficialReceipt":

                        switch (reportname)
                        {
                            case "ListofCheckReceived":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCheckReceived&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCancelledOfficialReceipt":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCancelledOfficialReceipt&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "PaymentDetails":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fPaymentDetails(ORDetails)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;
                                return Redirect(serverURI.Uri.ToString());

                            case "CashReceiptsAndDepositsRecord":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fCashReceiptsAndDepositsRecords&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType + "&collectedBy=" + collectedBy + "&accntOff=" + accntOff + "&officialDest=" + officialDest + "&officeDest=" + officeDest;
                                return Redirect(serverURI.Uri.ToString());

                            case "IssuedOR":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fIssuedOR&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ORswith121":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fOR%27swith121&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            default:
                                break;
                        }
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Collection - View Official Receipt Reports  - from Terminal: " + ipaddress);

                        break;

                    // Other Reports
                    case "Others":

                        switch (reportname)
                        {
                            case "AccountableForms":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAccountableForms&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ListofCollectionPerAccountCode":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fListofCollectionPerAccountCode&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "SerialNumber":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fSerialNumber&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "ReportofPayment-EngineeringFees":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fReportofPayment-EngineeringFees&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&zoneName=" + zoneName + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate;
                                return Redirect(serverURI.Uri.ToString());

                            case "AdvancesAlphaList":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAdvancesAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "VISAApplicationReport":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fVisaApplicationReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "OrderofPaymentSummaryReport":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fOrderofPaymentSummaryReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "AccountabilityforAccountableFormsReport":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAccountabilityforAccountableFormsReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "StatementOfAccount(Payment)":
                                serverURI.Query = serverURI.Query.ToString() + "%2fReports%2fStatementOfAccount(Payment)&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "AccountDescription":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAccountDescription&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            case "VATAlphalist":
                                serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fVATAlphalist&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate + "&zoneName=" + zoneName + "&currencyType=" + currencyType;
                                return Redirect(serverURI.Uri.ToString());

                            default:
                                break;
                        }

                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Collection - View Others Reports  - from Terminal: " + ipaddress);

                        break;

                    default:
                        break;
                }
            }

            return Redirect("ViewCollection");
        }

    }
}