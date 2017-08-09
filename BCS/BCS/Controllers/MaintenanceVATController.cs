using BCS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCS.Controllers
{
    public class MaintenanceVATController : Controller
    {
        // GET: DataEntryVAT
        public ActionResult ViewVAT()
        {
            return View();
        }

        // Generate Report
        public ActionResult VATReport(string reportType)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            return Redirect("/Reports/Report.aspx?reportType=" + reportType + "&zoneGroupCode=" + zoneGroupCode);
        }
    }
}