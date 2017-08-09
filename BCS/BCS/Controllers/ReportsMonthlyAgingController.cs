using BCS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Collections.Specialized;

namespace BCS.Controllers
{
    public class ReportsMonthlyAgingController : Controller
    {
        // GET: BillingPeriod
        private BCS_Context db = new BCS_Context();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        // GET: ReportsMonthlyAging

        public ActionResult ViewMonthlyAging(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            SearchSubsidiaryLedgerViewModel ForBilling = new SearchSubsidiaryLedgerViewModel();
            //ForBillPeriod.BillingPeriodList = db.BillingPeriod.Where(x => x.groupCode == ZoneGroupCode).ToList();
            ForBilling.BCSAgingOutput = db.BCSAgingOutput.Select(x => x.BillOriginYear).Distinct().OrderBy(x => x).ToList();
            return View("ViewMonthlyAging", ForBilling);
        }

        // local
        //[HttpPost]
        //public ActionResult MonthlyAging(FormCollection frm)
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string ZoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

        //    var ModeSelect = frm["Mode"];
        //    var ScopeSelect = frm["Scope"];

        //    NameValueCollection settings = new NameValueCollection();

        //    switch (ZoneGroupCode)
        //    {
        //        case "01":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/AGING");
        //            break;

        //        case "03":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/AGING");
        //            break;

        //        case "06":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/AGING");
        //            break;

        //        case "09":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/AGING");
        //            break;

        //        default:
        //            break;
        //    }

        //    // AGING Prior Years
        //    string preparedByAY = settings["preparedByAY"].ToString();
        //    string preparedByPositionAY = settings["preparedByPositionAY"].ToString();
        //    string notedByAY = settings["notedByAY"].ToString();
        //    string notedByPositionAY = settings["notedByPositionAY"].ToString();

        //    // AGING By Month
        //    string preparedByAM = settings["preparedByAM"].ToString();
        //    string preparedByPositionAM = settings["preparedByPositionAM"].ToString();
        //    string notedByAM = settings["notedByAM"].ToString();
        //    string notedByPositionAM = settings["notedByPositionAM"].ToString();

        //    // AGING Summary
        //    string certifiedByAS = settings["certifiedbyAS"].ToString();
        //    string certifiedByPositionAS = settings["certifiedByPositionAS"].ToString();
        //    string preparedByAS = settings["preparedbyAS"].ToString();
        //    string preparedByPositionAS = settings["preparedByPositionAS"].ToString();
        //    string notedByAS = settings["notedbyAS"].ToString();
        //    string notedByPositionAS = settings["notedByPositionAS"].ToString();

        //    // AGING Receivables
        //    string certifiedByAR = settings["certifiedbyAR"].ToString();
        //    string certifiedByPositionAR = settings["certifiedByPositionAR"].ToString();
        //    string preparedByAR = settings["preparedbyAR"].ToString();
        //    string preparedByPositionAR = settings["preparedByPositionAR"].ToString();
        //    string notedByAR = settings["notedbyAR"].ToString();
        //    string notedByPositionAR = settings["notedByPositionAR"].ToString();

        //    switch (ModeSelect)
        //    {
        //        case "AgingOfAccountsReceivable":

        //            // return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fAgingOfAccountsReceivable&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&certifiedByAR=" + certifiedByAR + "&preparedByAR=" + preparedByAR + "&notedByAR=" + notedByAR + "&certifiedByPositionAR=" + certifiedByPositionAR + "&preparedByPositionAR=" + preparedByPositionAR + "&notedByPositionAR=" + notedByPositionAR);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fAgingOfAccountsReceivable&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&certifiedByAR=" + certifiedByAR + "&preparedByAR=" + preparedByAR + "&notedByAR=" + notedByAR + "&certifiedByPositionAR=" + certifiedByPositionAR + "&preparedByPositionAR=" + preparedByPositionAR + "&notedByPositionAR=" + notedByPositionAR);

        //        case "MonthlyAgingSummary":

        //            // return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fMonthlyAgingSummary&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&certifiedByAS=" + certifiedByAS + "&preparedByAS=" + preparedByAS + "&NotedByAS=" + notedByAS + "&certifiedByPositionAS=" + certifiedByPositionAS + "&preparedByPositionAS=" + preparedByPositionAS + "&NotedByPositionAS=" + notedByPositionAS);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fMonthlyAgingSummary&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&certifiedByAS=" + certifiedByAS + "&preparedByAS=" + preparedByAS + "&notedByAS=" + notedByAS + "&certifiedByPositionAS=" + certifiedByPositionAS + "&preparedByPositionAS=" + preparedByPositionAS + "&NotedByPositionAS=" + notedByPositionAS);

        //        case "MonthlyAgingByMonth":

        //            // return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fMonthlyAgingByMonth&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&preparedByAM=" + preparedByAM + "&notedByAM=" + notedByAM + "&preparedByPositionAM=" + preparedByPositionAM + "&notedByPositionAM=" + notedByPositionAM);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fMonthlyAgingByMonth&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&preparedByAM=" + preparedByAM + "&notedByAM=" + notedByAM + "&preparedByPositionAM=" + preparedByPositionAM + "&notedByPositionAM=" + notedByPositionAM);

        //        case "MonthlyAgingPriorWithYears":

        //            // return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fMonthlyAgingPriorwithYears&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&prepareByAY=" + prepareByAY + "&notedByAY=" + notedByAY + "&prepareByPositionAY=" + prepareByPositionAY + "&notedByPositionAY=" + notedByPositionAY);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fMonthlyAgingReports%2fMonthlyAgingPriorwithYears&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&preparedByAY=" + preparedByAY + "&notedByAY=" + notedByAY + "&preparedByPositionAY=" + preparedByPositionAY + "&notedByPositionAY=" + notedByPositionAY);

        //        default:
        //            break;
        //    }


        //    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Monthly Aging - View Reports - from Terminal: " + ipaddress);
        //    return View("ViewMonthlyAging");
        //}

        [HttpPost]
        // aws
        public ActionResult MonthlyAging(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            int billingPeriod = Convert.ToInt32(frm["billingPeriod"]);
            var ModeSelect = frm["Mode"];
            var ScopeSelect = frm["Scope"];
            var billingMonth = frm["billingMonth"];
            var billingYear = frm["billingYear"];

            NameValueCollection settings = new NameValueCollection();

            switch (ZoneGroupCode)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/AGING");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/AGING");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/AGING");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/AGING");
                    break;

                default:
                    break;
            }

            // AGING Prior Years
            string preparedByAY = settings["preparedByAY"].ToString();
            string preparedByPositionAY = settings["preparedByPositionAY"].ToString();
            string notedByAY = settings["notedByAY"].ToString();
            string notedByPositionAY = settings["notedByPositionAY"].ToString();

            // AGING By Month
            string preparedByAM = settings["preparedByAM"].ToString();
            string preparedByPositionAM = settings["preparedByPositionAM"].ToString();
            string notedByAM = settings["notedByAM"].ToString();
            string notedByPositionAM = settings["notedByPositionAM"].ToString();

            // AGING Summary
            string certifiedByAS = settings["certifiedbyAS"].ToString();
            string certifiedByPositionAS = settings["certifiedByPositionAS"].ToString();
            string preparedByAS = settings["preparedbyAS"].ToString();
            string preparedByPositionAS = settings["preparedByPositionAS"].ToString();
            string notedByAS = settings["notedbyAS"].ToString();
            string notedByPositionAS = settings["notedByPositionAS"].ToString();

            // AGING Receivables
            string certifiedByAR = settings["certifiedbyAR"].ToString();
            string certifiedByPositionAR = settings["certifiedByPositionAR"].ToString();
            string preparedByAR = settings["preparedbyAR"].ToString();
            string preparedByPositionAR = settings["preparedByPositionAR"].ToString();
            string notedByAR = settings["notedbyAR"].ToString();
            string notedByPositionAR = settings["notedByPositionAR"].ToString();

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);

            switch (ModeSelect)
            {
                case "AgingOfAccountsReceivable":
                    serverURI.Query = serverURI.Query.ToString() + "%2fMonthlyAgingReports%2fAgingOfAccountsReceivable&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&certifiedByAR=" + certifiedByAR + "&preparedByAR=" + preparedByAR + "&notedByAR=" + notedByAR + "&certifiedByPositionAR=" + certifiedByPositionAR + "&preparedByPositionAR=" + preparedByPositionAR + "&notedByPositionAR=" + notedByPositionAR + "&billingMonth=" + billingMonth + "&billingYear=" + billingYear;
                    return Redirect(serverURI.Uri.ToString());

                case "MonthlyAgingSummary":
                    serverURI.Query = serverURI.Query.ToString() + "%2fMonthlyAgingReports%2fMonthlyAgingSummary&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&certifiedByAS=" + certifiedByAS + "&preparedByAS=" + preparedByAS + "&notedByAS=" + notedByAS + "&certifiedByPositionAS=" + certifiedByPositionAS + "&preparedByPositionAS=" + preparedByPositionAS + "&notedByPositionAS=" + notedByPositionAS + "&billingMonth=" + billingMonth + "&billingYear=" + billingYear;
                    return Redirect(serverURI.Uri.ToString());

                case "MonthlyAgingByMonth":
                    serverURI.Query = serverURI.Query.ToString() + "%2fMonthlyAgingReports%2fMonthlyAgingByMonth&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&preparedByAM=" + preparedByAM + "&notedByAM=" + notedByAM + "&preparedByPositionAM=" + preparedByPositionAM + "&notedByPositionAM=" + notedByPositionAM + "&billingMonth=" + billingMonth + "&billingYear=" + billingYear;
                    return Redirect(serverURI.Uri.ToString());

                case "MonthlyAgingPriorWithYears":
                    serverURI.Query = serverURI.Query.ToString() + "%2fMonthlyAgingReports%2fMonthlyAgingPriorwithYears&rs:Command=Render&zoneGroupCode=" + ZoneGroupCode + "&statusType=" + ScopeSelect + "&preparedByAY=" + preparedByAY + "&notedByAY=" + notedByAY + "&preparedByPositionAY=" + preparedByPositionAY + "&notedByPositionAY=" + notedByPositionAY + "&billingMonth=" + billingMonth + "&billingYear=" + billingYear;
                    return Redirect(serverURI.Uri.ToString());

                default:
                    break;
            }


            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Monthly Aging - View Reports - from Terminal: " + ipaddress);
            return View("ViewMonthlyAging");
        }
    }
}