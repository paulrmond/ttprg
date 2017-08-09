using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace BCS.Controllers
{
    [Authorize]
    public class DataEntryAdminFeeController : Controller
    {
        BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();



        // GET: DataEntryAdminFee
        public ActionResult ViewAdminFee()
        {

            //SearchAdminFeeModels srchafm = new SearchAdminFeeModels(); 
            var userid = User.Identity.GetUserId();
            var ZoneGroup1 = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            //srchafm.BillingPeriodList = db.BillingPeriod.Where(m => m.Finalized.ToUpper() == "NO" && m.groupCode == ZoneGroup1).ToList();
            //srchafm.AdminFeeList =    db.AdminFee.SqlQuery("Select * from AdminFees").ToList();
            ViewBag.ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            //ViewBag.BillingPeriod = db.BillingPeriod.Where(m => m.Finalized.ToUpper() == "NO" && m.groupCode == ZoneGroup1).ToList();
            var billp = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup1).ToList().OrderByDescending(m => m.BillingPeriodId);
            ViewBag.BillingPeriod = billp.ToList();

            //ViewBag.AdminFee = db.AdminFee.SqlQuery("SELECT DISTINCT Developer FROM AdminFees").ToList();
            //ViewBag.AdminFee = db.AdminFee.Where(m => m.Zone_Code == ZoneGroup1).ToList();
            ViewBag.AdminFee = db.AdminFee.Select(m => m.Developer).Distinct().ToList();
            return View("ViewAdminFee");
        }

        //JSON RESULT FOR DEVELOPER
        public JsonResult GetDevelopers(string Dev)
        {
            //var Devlist = db.AdminFee.Select(m => m.Developer).Distinct().ToList();
            //try
            //{
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ApplicationDbContext context = new ApplicationDbContext();
            //DeveloperDetailsViewModel samp = new DeveloperDetailsViewModel();
            ReportTable DeveloperList = new ReportTable();
            List<devdetails> samp = new List<devdetails>();
            DeveloperDetails1 devd = new DeveloperDetails1();
            var userid = User.Identity.GetUserId();
            string zoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            var ZoneGroupId = db.ZoneGroup.FirstOrDefault(x => x.ZoneGroupCode == zoneGroup).ZoneGroupId.ToString();
            var ZoneCode = db.Zone.Where(x => x.ZoneGroup == ZoneGroupId).Select(x => x.ZoneCode).ToString();

            if (Dev.Length >= 4)
            {
                var dev = db.Database.SqlQuery<DeveloperDetails>("SELECT DISTINCT Dev_Comp_Code, Developer, Zone_Code, Ecozone from AdminFees left join Zones on AdminFees.Zone_Code = Zones.ZoneCode where AdminFees.Developer like '%" + Dev + "%'").ToList();
                //foreach (var items in dev)
                //{
                //    devdetails temp = new devdetails();
                //    //samp.DeveloperId1.Add(items.DeveloperId);
                //    temp.Dev_Comp_Code=items.Dev_Comp_Code;
                //    temp.Developer=items.Developer;
                //    temp.Ecozone=items.Ecozone;
                //    temp.Zone_Code=items.Zone_Code;
                //    samp.Add(temp);
                //}
                devd.dev = samp;
                //db.Database.SqlQuery<DeveloperDetails>("SELECT DISTINCT Dev_Comp_Code, Developer, Zone_Code, Ecozone from AdminFees left join Zones on AdminFees.Zone_Code = Zones.ZoneCode where AdminFees.Developer like '%" + Dev + "%'").ToList();
                //var devs = db.AdminFee.Select(o => o.Zone_Code == ZoneCode && o.Developer.Contains(Dev)).ToList();

                //var devs = from af in db.AdminFee
                //           join z in db.Zone on af.Zone_Code equals z.ZoneCode
                //           where z.ZoneGroup == ZoneGroupId && af.Developer.Contains(Dev)
                //           select af;

                //devs.ToList();

                //var devs1 = devs.Contains(Dev).ToString().Take(15);

                //foreach (var item in devs1)
                //{
                //        //var devs = db.Database.SqlQuery<AdminFee>("select AdminFees.* from AdminFees left join Zones on AdminFees.Zone_Code = Zones.ZoneCode where Zones.ZoneGroup = '" + ZoneGroupId + "'").ToList();
                //        DeveloperList.DeveloperName.Add(item.ToString());
                //        //DeveloperList.DeveloperName.Add(db.AdminFee.Where(x => x.Zone_Code == item.ZoneCode).FirstOrDefault().Developer); 
                //        //catch (Exception ex) { }                       

                //}

                ////var a = DeveloperList;
                //try
                //{
                //    //var Devlist1 = DeveloperList.DeveloperName.Contains(Dev).ToString().Take(15);
                //    return Json(devs);
                //}
                //catch { }

                return Json(dev);
            }
            //}
            //catch { }
            return Json(null);
        }


        public JsonResult getEcozone(string Dev)
        {
            try
            {
                var code = Dev.Split('~');
                var DevName = code[0];
                var EcoName = code[1];
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ApplicationDbContext context = new ApplicationDbContext();
                //DeveloperDetailsViewModel samp = new DeveloperDetailsViewModel();
                ReportTable DeveloperList = new ReportTable();
                List<devdetails> samp = new List<devdetails>();
                DeveloperDetails1 devd = new DeveloperDetails1();
                var userid = User.Identity.GetUserId();
                string zoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
                var ZoneGroupId = db.ZoneGroup.FirstOrDefault(x => x.ZoneGroupCode == zoneGroup).ZoneGroupId.ToString();
                var ZoneCode = db.Zone.Where(x => x.ZoneGroup == ZoneGroupId).Select(x => x.ZoneCode).ToString();

                if (Dev.Length >= 4)
                {
                    var dev = db.Database.SqlQuery<DeveloperDetails>("SELECT DISTINCT Dev_Comp_Code, Zone_Code from AdminFees left join Zones on AdminFees.Zone_Code = Zones.ZoneCode where AdminFees.Developer = '" + DevName + "'  AND AdminFees.Ecozone = '" + EcoName + "'").ToList().Take(1);
                    return Json(dev);
                }
            }
            catch { }
            return Json(null);

        }


        public JsonResult uploaddata(string dat)
        {
            foreach (var item in Request.Files)
            {
                var a = Request.Files[item.ToString()];
                HttpPostedFileBase file = Request.Files["a"];
            }
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Admin Fee - Upload data  - from Terminal: " + ipaddress);
            return Json("OK");
        }

        // Generate Report
        [HttpPost]
        public ActionResult AdminFeeListReport(FormCollection frm)
        {
            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);

            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            var zoneType = frm["zoneType"];
            var devCode = frm["devname1"];
            var splitTo = devCode.Split('|');
            var compCode = splitTo[0];
            var compName = splitTo[1];

            compCode = compCode.Replace(" ","");

            //var zoneCode = frm["zoneCode"];
            //var compCode = frm["compCode"];
            var generatedType = frm["generatedType"];
            var billingPeriodID = frm["BillingPeriod"];

            switch (generatedType)
            {

                case "AdminFeeAlphaList":
                    serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fAdminFeeAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&compCode=" + compCode + "&billingPeriodID=" + billingPeriodID + "&zoneType=" + zoneType;
                    return Redirect(serverURI.Uri.ToString());

                case "AdminFeeMonthly":
                    serverURI.Query = serverURI.Query.ToString() + "%2fDataEntries%2fAdminFeeMonthlyReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&compCode=" + compCode + "&billingPeriodID=" + billingPeriodID + "&zoneType=" + zoneType;
                    return Redirect(serverURI.Uri.ToString());

                default:
                    break;
            }


            //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);

            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Data Entry Admin Fee - View Report  - from Terminal: " + ipaddress);
            return Redirect("ViewAdminFee");

        }

        //public ActionResult AdminFeeMonthlyReport(string reportType, int billingPeriodID)
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var userid = User.Identity.GetUserId();
        //    string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

        //    //return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);
        //    return Redirect("http://bcs.dci.ph/ReportServer/Pages/ReportViewer.aspx?%2fDataEntries%2fAdminFeeMonthlyReport&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriodID=" + billingPeriodID);
        //}
    }

    public class DeveloperDetails1
    {
        public List<devdetails> dev { get; set; }
    }

    public class devdetails
    {
        public string Dev_Comp_Code { get; set; }
        public string Developer { get; set; }
        public string Zone_Code { get; set; }
        public string Ecozone { get; set; }
    }
}