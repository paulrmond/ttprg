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
    public class SystemLogsController : Controller
    {
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        // GET: SystemLogs
        private BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();

        SearchReportsCollectionModels srchZG = new SearchReportsCollectionModels();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ViewSystemLogs()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;
            //srchZG.systemlogslist = db.systemlogs.SqlQuery("Select * from systemlogs").ToList();
            srchZG.systemlogslist = db.systemlogs.Where(x => x.ZoneGroupCode == ZoneGroupCode).ToList();
            return View("ViewSystemLogs", srchZG);
        }

        public ActionResult SystemlogsView()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroupCode = context.Users.FirstOrDefault(n => n.Id == userid).ZoneGroup;

            List<systemlogs> qry = new List<systemlogs>();
            qry = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.UserName == s.UserName && s.ZoneGroupCode == ZoneGroupCode).ToList();


            return View("SystemlogsView", qry);
        }


        [HttpPost]
        public PartialViewResult GetLogs(string searchstr, string selsrch)
        {
            List<systemlogs> syslogs = new List<systemlogs>();
            BCS_Context db = new BCS_Context();
            if (!string.IsNullOrEmpty(searchstr))
            {

                //var datedate = Convert.ToDateTime(searchstr.ToString());
                switch (selsrch)
                {
                    case "default":
                        syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.UserName.Contains(searchstr)).ToList();
                        break;
                    case "username":
                        syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.UserName.Contains(searchstr)).ToList();
                        break;

                    case "level":
                        syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.loglevel.Contains(searchstr)).ToList();
                        break;
                    case "message":
                        syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.remarks.Contains(searchstr)).ToList();
                        break;
                    case "":
                        syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.UserName.Contains(searchstr)).ToList();
                        break;
                    case "date":
                        DateTime dt1 = Convert.ToDateTime(searchstr);
                        DateTime dtaam1 = dt1.AddHours(23);
                        DateTime dtbam2 = dt1.AddMinutes(59);

                        syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.timestamp.Year == dt1.Year && s.timestamp.Month == dt1.Month && s.timestamp.Day == dt1.Day).ToList();

                        break;
                }
            }
            else
            {
                //syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).ToList();
                syslogs = db.systemlogs.OrderByDescending(s => s.timestamp).Where(s => s.UserName == s.UserName).ToList();
            }
            return PartialView("_GetLogsPartial", syslogs);
        }

        [HttpPost]
        public ActionResult SystemLogsView(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            var tdStartDate = frm["tdstartDate"];
            var tdEndDate = frm["tdEndDate"];
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "System Logs - View Report - from Terminal: " + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fAdmin%2fUserActivityLogs&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&tdStartDate=" + tdStartDate + "&tdEndDate=" + tdEndDate;

            return Redirect(serverURI.Uri.ToString());
        }
    }
}