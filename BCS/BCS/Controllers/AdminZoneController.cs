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

    [ValidateInput(true)]
    public class AdminZoneController : Controller
    {
        // GET: AdminZoneGroupZone
        private BCS_Context db = new BCS_Context();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        public ActionResult ViewZoneGroup(FormCollection frm)
        {
            if (frm.Count == 0)
            {
                SearchZoneGroupViewModel SearchZoneGroupViewModels = new SearchZoneGroupViewModel();
                SearchZoneGroupViewModels.ZoneGroupList = db.ZoneGroup.SqlQuery("Select * from ZoneGroups where ZoneGroupCode != '99'").ToList();
                ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
                return View(SearchZoneGroupViewModels);
            }
            else if (frm.Count >= 1)
            {
                int parsedID = int.Parse(frm["ZoneGroupId"]);
                ZoneGroup zonegroup = db.ZoneGroup.Find(parsedID);
                db.ZoneGroup.Remove(zonegroup);

                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone -View Zone Group - Zone Group Updated  - from Terminal: " + ipaddress);

                db.SaveChanges();
                return RedirectToAction("ViewZoneGroup");
            }
            return View();
        }

        public ActionResult ViewZone(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var zonegroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var zonegroupID = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == zonegroup).ZoneGroupId;

            List<Zone> zones = new List<Zone>();
            if (User.IsInRole("Super User"))
                zones = db.Zone.ToList();
            else
                zones = db.Zone.Where(m => m.ZoneGroup == zonegroupID.ToString()).ToList();

            if (frm.Count == 0)
            {
                SearchZoneViewModel SearchZoneViewModels = new SearchZoneViewModel();
                SearchZoneViewModels.ZoneList = zones;
                SearchZoneViewModels.ZoneGroupList = db.ZoneGroup.SqlQuery("Select * from ZoneGroups").ToList();
                ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
                ViewBag.ZoneGroup = zonegroup;
                return View(SearchZoneViewModels);
            }
            //else if (frm.Count >= 1)
            //{
            //    int parsedID = int.Parse(frm["ZoneId"]);
            //    Zone zone = db.Zone.Find(parsedID);
            //    db.Zone.Remove(zone);
            //    db.SaveChanges();
            //    return RedirectToAction("ViewZone");
            //}  
            ViewBag.ZoneGroup = zonegroup;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddZoneGroup(string ZoneGroupCode, string ZoneGroupName, string ZoneGroupAddress, string ZoneGroupRole, string ZoneGroupInitial)
        {
            if (!db.ZoneGroup.Any(m => m.ZoneGroupCode == ZoneGroupCode))
            {
                SearchZoneGroupViewModel SearchZoneGroup = new SearchZoneGroupViewModel();
                ZoneGroup ZoneGroupAssignment = new ZoneGroup();
                ZoneGroupAssignment.ZoneGroupCode = ZoneGroupCode;
                ZoneGroupAssignment.ZoneGroupName = ZoneGroupName;
                ZoneGroupAssignment.ZoneGroupAddress = ZoneGroupAddress;
                ZoneGroupAssignment.ZoneGroupRole = ZoneGroupRole;
                ZoneGroupAssignment.ZoneGroupInitials = ZoneGroupInitial;
                db.ZoneGroup.Add(ZoneGroupAssignment);
                db.SaveChanges();
                TempData["TransactionSuccess"] = "Add";

                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - Zone Group Added  - from Terminal: " + ipaddress);

                return RedirectToAction("ViewZoneGroup");
            }
            else
            {
                TempData["TransactionSuccess"] = "Duplicate";

                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - Zone Group Duplicate  - from Terminal: " + ipaddress);

                return RedirectToAction("ViewZoneGroup");
            }
        }

        public ActionResult AddZone(string ZoneName, string ZoneGroup, string ZoneCode)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var zonegroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var zonegroupID = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == zonegroup).ZoneGroupId;

            CheckZoneDuplicateEntry checkZoneDuplicateEntry = new CheckZoneDuplicateEntry(ZoneName, zonegroupID.ToString());
            if (!checkZoneDuplicateEntry.hasDuplicateEntry())
            {
                SearchZoneGroupViewModel SearchZoneGroup = new SearchZoneGroupViewModel();
                Zone ZoneAssignment = new Zone();
                ZoneAssignment.ZoneCode = ZoneCode;
                ZoneAssignment.ZoneName = ZoneName;
                ZoneAssignment.ZoneGroup = zonegroup == "99" ? ZoneGroup : zonegroupID.ToString();
                db.Zone.Add(ZoneAssignment);
                db.SaveChanges();

                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - Zone Added  - from Terminal: " + ipaddress);

                TempData["TransactionSuccess"] = "Add";
            }
            else
            {
                TempData["TransactionSuccess"] = "Duplicate";

                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - Zone Duplicate  - from Terminal: " + ipaddress);

            }
            return RedirectToAction("ViewZone");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateZoneGroup(FormCollection frmcollection)
        {
            ZoneGroup zonegroupinfo = null;
            int ParsedIntID = int.Parse(frmcollection["ZoneGId"]);

            string zonegroupcode = frmcollection["ZoneGroupCode"].ToString();
            zonegroupinfo = db.ZoneGroup.Find(ParsedIntID);
            string zoneCodeOrig = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupId == ParsedIntID).ZoneGroupCode;
            int newzonecodeId = 0;

            if (db.ZoneGroup.Any(m => m.ZoneGroupCode == zonegroupcode))
                newzonecodeId = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == zonegroupcode).ZoneGroupId;

            if ((db.ZoneGroup.Any(m => m.ZoneGroupCode == zonegroupcode) && newzonecodeId == ParsedIntID) || (!db.ZoneGroup.Any(m => m.ZoneGroupCode == zonegroupcode)))
            {
                if (zonegroupinfo != null)
                {
                    zonegroupinfo.ZoneGroupCode = frmcollection["ZoneGroupCode"].ToString();
                    zonegroupinfo.ZoneGroupName = frmcollection["ZoneGroupName"].ToString();
                    zonegroupinfo.ZoneGroupAddress = frmcollection["ZoneGroupAddress"].ToString();
                    zonegroupinfo.ZoneGroupRole = frmcollection["ZoneGroupRole"].ToString();
                    zonegroupinfo.ZoneGroupInitials = frmcollection["ZoneGroupInitial"].ToString();
                    db.Entry(zonegroupinfo).State = System.Data.Entity.EntityState.Modified;

                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - Zone Group Edited  - from Terminal: " + ipaddress);

                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                }
            }
            else
            {
                TempData["TransactionSuccess"] = "Duplicate";
            }

            return RedirectToAction("ViewZoneGroup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateZone(FormCollection frmcollection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var zonegroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;
            var zonegroupID = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == zonegroup).ZoneGroupId;
            string ZoneName = frmcollection["ZoneName"].ToString();
            string ZoneNameOrig = frmcollection["ZoneNameOrig"].ToString();
            Zone zoneinfo = null;
            int ParsedIntID = int.Parse(frmcollection["ZoneId2"]);
            zoneinfo = db.Zone.Find(ParsedIntID);

            CheckZoneDuplicateEntry checkZoneDuplicateEntry = new CheckZoneDuplicateEntry(ZoneName, zonegroupID.ToString(), ZoneNameOrig);
            if (!checkZoneDuplicateEntry.hasDuplicateEntry())
            {
                if (zoneinfo != null)
                {
                    zoneinfo.ZoneCode = frmcollection["ZoneCode"].ToString();
                    zoneinfo.ZoneName = ZoneName;

                    if (zonegroup == "99")
                        zoneinfo.ZoneGroup = frmcollection["ZoneGroup"].ToString();

                    db.Entry(zoneinfo).State = System.Data.Entity.EntityState.Modified;

                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - Zone Edited  - from Terminal: " + ipaddress);

                    db.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                }
            }
            else
            {
                TempData["TransactionSuccess"] = "Duplicate";
            }
            return RedirectToAction("ViewZone");
        }

        // Generater Report
        public ActionResult ZoneReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Zone - View Zone Report  - from Terminal: " + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fAdmin%2fZoneAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }

    }
}