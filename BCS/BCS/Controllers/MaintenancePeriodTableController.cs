using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net;

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class MaintenancePeriodTableController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        private BCS_Context db = new BCS_Context();

        public ActionResult ViewPeriodTablePRG(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Period;
            //ViewBag.Groups = new SelectList(db.ZoneGroup.ToList(), "ZoneGroupCode", "ZoneGroupName");

            SearchBillingPeriodViewModel SearchPeriodViewModels = new SearchBillingPeriodViewModel();
            //SearchPeriodViewModels.BillPeriodList = db.BillingPeriod.ToList();
            SearchPeriodViewModels.BillPeriodList = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).OrderByDescending(n=>n.DateFrom).ToList();
            //return View(SearchPeriodViewModels);
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            return View("ViewPeriodTable", SearchPeriodViewModels);
        }

        // GET:
        public ActionResult ViewPeriodTable(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Period;

            if (frm.Count == 0)
            {
                SearchBillingPeriodViewModel SearchPeriodViewModels = new SearchBillingPeriodViewModel();
                SearchPeriodViewModels.BillPeriodList = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).OrderByDescending(n => n.DateFrom).ToList();
                return View(SearchPeriodViewModels);
            }
            else if (frm.Count >= 1)
            {
                int parsedID = int.Parse(frm["ID"]);
                BillingPeriod period = db.BillingPeriod.Find(parsedID);
                db.BillingPeriod.Remove(period);
                db.SaveChanges();
                return RedirectToAction("ViewPeriodTable");
            }
            return View();
        }

        public ActionResult RemovePeriodTable(FormCollection frm)
        {
            int parsedID = int.Parse(frm["ID"]);
            BillingPeriod period = db.BillingPeriod.Find(parsedID);
            db.BillingPeriod.Remove(period);
            db.SaveChanges();
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Maintenance Period Table - Period Table Removed  - from Terminal: " + ipaddress);
            return RedirectToAction("ViewPeriodTable");
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddPeriod(DateTime DateFrom, DateTime DateTo, string PeriodText, string Groups)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var username = User.Identity.GetUserName();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            var maxBillingPeriodId = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).Max(m => m.BillingPeriodId);
            var EOMStatus = db.BillingPeriod.FirstOrDefault(m => m.BillingPeriodId == maxBillingPeriodId).EOMStatus ?? "NOT DONE";
            
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Period;
            string previousStatus = "YES";
            DateTime? previousDateTo = null;

            EOMStatus = "DONE"; //Temporary code. To bypass EOM process before new billing period.
            if (EOMStatus.ToUpper() == "DONE")
            {
                if (db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).Count() > 0)
                {
                    var maxBillingId = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).Max(m => m.BillingPeriodId);
                    previousStatus = db.BillingPeriod.Where(m => m.BillingPeriodId == maxBillingId).Where(m => m.groupCode == ZoneGroup).FirstOrDefault().Finalized;
                    previousDateTo = db.BillingPeriod.Where(m => m.BillingPeriodId == maxBillingId).Where(m => m.groupCode == ZoneGroup).FirstOrDefault().DateTo;
                }
                else
                {
                    previousDateTo = DateFrom.AddDays(-1);
                }

                if (previousStatus.ToUpper() == "YES")
                {
                    if (DateFrom > previousDateTo)
                    {
                        BillingPeriod PeriodAssignment = new BillingPeriod();
                        PeriodAssignment.PeriodText = PeriodText;
                        PeriodAssignment.DateFrom = DateFrom;
                        PeriodAssignment.DateTo = DateTo;
                        PeriodAssignment.groupCode = ZoneGroup;
                        PeriodAssignment.Finalized = "NO";
                        PeriodAssignment.Generated = "NO";
                        db.BillingPeriod.Add(PeriodAssignment);
                        db.SaveChanges();
                    }
                    else
                    {
                        TempData["TransactionSuccess"] = "DatePeriodFailed";
                    }
                }
                else
                {
                    TempData["TransactionSuccess"] = "PeriodFailed";
                }
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Maintenance Period Table - Period Table Added  - from Terminal: " + ipaddress);
            }
            else
            {
                TempData["TransactionSuccess"] = "GenerateEOMFirst";
            }
            return RedirectToAction("ViewPeriodTablePRG", "MaintenancePeriodTable");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePeriod(FormCollection frmcollection)
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Period;
            BillingPeriod Periodinfo = null;
            int ParsedIntID = int.Parse(frmcollection["BillingPeriodId"]);

            string BillingStatus = db.BillingPeriod.Where(m => m.BillingPeriodId == ParsedIntID).SingleOrDefault().Finalized;

            if (BillingStatus.ToUpper() == "NO")
            {
                Periodinfo = db.BillingPeriod.Find(ParsedIntID);
                if (Periodinfo != null)
                {
                    Periodinfo.PeriodText = frmcollection["PeriodText"].ToString();
                    Periodinfo.DateFrom = Convert.ToDateTime(frmcollection["DateFrom"].ToString());
                    Periodinfo.DateTo = Convert.ToDateTime(frmcollection["DateTo"].ToString());
                    //Periodinfo.groupCode = Group;
                    db.Entry(Periodinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Invalid transaction.");
                }
            }
            else
            {
                Response.Write("<script>alert('Unable to edit finalized billing period.')</script>");
            }
            int parsedID = int.Parse(frmcollection["BillingPeriodId"]);
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Maintenance Period Table - Period Table Updated  - from Terminal: " + ipaddress);

            return RedirectToAction("ViewPeriodTablePRG", "MaintenancePeriodTable");
        }
    }
}