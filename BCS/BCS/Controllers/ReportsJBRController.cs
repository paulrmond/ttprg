using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Collections.Specialized;
using System.Configuration;

namespace BCS.Controllers
{
    public class ReportsJBRController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        // GET: ReportsJBR
        ApplicationDbContext context = new ApplicationDbContext();
        private BCS_Context db = new BCS_Context();
        public ActionResult ViewJBR()
        {
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            SearchReportsBillingStatementModels srchBS = new SearchReportsBillingStatementModels();
            //srchBS.BillingPeriodList = db.BillingPeriod.SqlQuery("Select * from BillingPeriods").ToList();
            var billp = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList().OrderByDescending(m=>m.BillingPeriodId);
            srchBS.BillingPeriodList = billp.ToList();
            return View("ViewJBR", srchBS);
        }

        // AWS
        [HttpPost]
        public ActionResult ViewJBRReports(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            NameValueCollection settings = new NameValueCollection();

            switch (zoneGroupCode)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/JBR");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/JBR");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/JBR");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/JBR");
                    break;

                default:
                    break;
            }

            // JBR ALL
            string notedByAll = settings["notedByAll"].ToString();
            string notedByPositionAll = settings["notedByPositionAll"].ToString();
            string preparedByAll = settings["preparedByAll"].ToString();
            string preparedByPositionAll = settings["preparedByPositionAll"].ToString();

            // JBR VAT
            string notedByVAT = settings["notedByVAT"].ToString();
            string notedByPositionVAT = settings["notedByPositionVAT"].ToString();
            string preparedByVAT = settings["preparedByVAT"].ToString();
            string preparedByPositionVAT = settings["preparedByPositionVAT"].ToString();

            // JBR WATER/SEWERAGE
            string notedByWS = settings["notedByWS"].ToString();
            string notedByPositionWS = settings["notedByPositionWS"].ToString();
            string preparedByWS = settings["preparedByWS"].ToString();
            string preparedByPositionWS = settings["preparedByPositionWS"].ToString();

            var billingPeriod = frm["BillingPeriod"];
            var JBRCategory = frm["JBRCategory"];

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);

            switch (JBRCategory)
            {
                case "VAT":
                    serverURI.Query = serverURI.Query.ToString() + "%2fJBRReports%2fJBRVAT&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByVAT=" + notedByVAT + "&notedByPositionVAT=" + notedByPositionVAT + "&preparedByVAT=" + preparedByVAT + "&preparedByPositionVAT=" + preparedByPositionVAT;
                    return Redirect(serverURI.Uri.ToString());

                case "All":
                    serverURI.Query = serverURI.Query.ToString() + "%2fJBRReports%2fJBRAll&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByAll=" + notedByAll + "&notedByPositionAll=" + notedByPositionAll + "&preparedByAll=" + preparedByAll + "&preparedByPositionAll=" + preparedByPositionAll;
                    return Redirect(serverURI.Uri.ToString());

                case "watersewarage":
                    serverURI.Query = serverURI.Query.ToString() + "%2fJBRReports%2fJBRWaterSewerage&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByWS=" + notedByWS + "&notedByPositionWS=" + notedByPositionWS + "&preparedByWS=" + preparedByWS + "&preparedByPositionWS=" + preparedByPositionWS;
                    return Redirect(serverURI.Uri.ToString());

                default:
                    break;
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports JBR - View Reports  - from Terminal: " + ipaddress);
            return View("ViewJBR");
        }


        // local
        //[HttpPost]
        //public ActionResult ViewJBRReports(FormCollection frm)
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

        //    NameValueCollection settings = new NameValueCollection();

        //    switch (zoneGroupCode)
        //    {
        //        case "01":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/JBR");
        //            break;

        //        case "03":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/JBR");
        //            break;

        //        case "06":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/JBR");
        //            break;

        //        case "09":
        //            settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/JBR");
        //            break;

        //        default:
        //            break;
        //    }

        //    // JBR ALL
        //    string notedByAll = settings["notedByAll"].ToString();
        //    string notedByPositionAll = settings["notedByPositionAll"].ToString();
        //    string preparedByAll = settings["preparedByAll"].ToString();
        //    string preparedByPositionAll = settings["preparedByPositionAll"].ToString();

        //    // JBR VAT
        //    string notedByVAT = settings["notedByVAT"].ToString();
        //    string notedByPositionVAT = settings["notedByPositionVAT"].ToString();
        //    string preparedByVAT = settings["preparedByVAT"].ToString();
        //    string preparedByPositionVAT = settings["preparedByPositionVAT"].ToString();

        //    // JBR WATER/SEWERAGE
        //    string notedByWS = settings["notedByWS"].ToString();
        //    string notedByPositionWS = settings["notedByPositionWS"].ToString();
        //    string preparedByWS = settings["preparedByWS"].ToString();
        //    string preparedByPositionWS = settings["preparedByPositionWS"].ToString();

        //    var billingPeriod = frm["BillingPeriod"];
        //    var JBRCategory = frm["JBRCategory"];

        //    switch (JBRCategory)
        //    {
        //        case "VAT":
        //            //  return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fJBRReports%2fJBRVAT&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByVAT=" + notedByVAT + "&notedByPositionVAT=" + notedByPositionVAT + "&preparedByVAT=" + preparedByVAT + "&preparedByPositionVAT=" + preparedByPositionVAT);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fJBRReports%2fJBRVAT&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByVAT=" + notedByVAT + "&notedByPositionVAT=" + notedByPositionVAT + "&preparedByVAT=" + preparedByVAT + "&preparedByPositionVAT=" + preparedByPositionVAT);

        //        case "All":
        //            //   return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fJBRReports%2fJBRAll&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByVAT=" + notedByVAT + "&notedByPositionVAT=" + notedByPositionVAT + "&preparedByVAT=" + preparedByVAT + "&preparedByPositionVAT=" + preparedByPositionVAT);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fJBRReports%2fJBRAll&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByAll=" + notedByAll + "&notedByPositionAll=" + notedByPositionAll + "&preparedByAll=" + preparedByAll + "&preparedByPositionAll=" + preparedByPositionAll);

        //        case "watersewarage":
        //            //   return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fJBRReports%2fJBRWaterSewerage&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByVAT=" + notedByVAT + "&notedByPositionVAT=" + notedByPositionVAT + "&preparedByVAT=" + preparedByVAT + "&preparedByPositionVAT=" + preparedByPositionVAT);
        //            return Redirect("http://dev4-pc/ReportServer_DCIDEVDB/Pages/ReportViewer.aspx?%2fJBRReports%2fJBRWaterSewerage&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&notedByWS=" + notedByWS + "&notedByPositionWS=" + notedByPositionWS + "&preparedByWS=" + preparedByWS + "&preparedByPositionWS=" + preparedByPositionWS);

        //        default:
        //            break;
        //    }
        //    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports JBR - View Reports  - from Terminal: " + ipaddress);
        //    return View("ViewJBR");
        //}
    }
}