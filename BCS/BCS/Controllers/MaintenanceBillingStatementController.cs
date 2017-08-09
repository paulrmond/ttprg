using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCS.Controllers
{
    public class MaintenanceBillingStatementController : Controller
    {
        // GET: MaintenanceBillingStatement
        public ActionResult ViewBillingStatement()
        {
            return View();
        }
    }
}