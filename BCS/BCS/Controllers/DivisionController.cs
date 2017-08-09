using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using System.Net;

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class DivisionController : Controller
    {
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        // GET: Division
        public ActionResult Index()
        {
            BCS_Context db = new BCS_Context();
            List<Division> division = new List<Division>();
            division = db.Division.ToList();
            ViewBag.Division = division;
            return View();
        }

        [HttpPost]
        public PartialViewResult GetList(string divisionCode)
        {
            IEnumerable<Division> division;
            BCS_Context db = new BCS_Context();
            if (!string.IsNullOrEmpty(divisionCode))
            {
                division = db.Division.Where(m => m.Code == divisionCode).ToList().OrderBy(m => m.Name);
            }
            else
            {
                division = db.Division.ToList().OrderBy(m => m.Name);
            }
            return PartialView("_DivisionPartial", division);
        }

        [HttpPost]
        public PartialViewResult Create(string DivisionCode, string DivisionName)
        {
            IEnumerable<Division> divisions = new List<Division>();
            BCS_Context db = new BCS_Context();
            try
            {
                using (var dbtransaction = db.Database.BeginTransaction())
                {
                    Division division = new Division();
                    division.Code = DivisionCode;
                    division.Name = DivisionName;
                    db.Division.Add(division);
                    db.SaveChanges();
                    dbtransaction.Commit();
                    divisions = db.Division.ToList().OrderBy(m => m.Name);

                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Division - Division Created  - from Terminal: " + ipaddress);

                    return PartialView("_DivisionPartial", divisions);
                }
            }
            catch (Exception)
            {
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Division - Create Division failed  - from Terminal: " + ipaddress);
                divisions = db.Division.ToList().OrderBy(m => m.Name);
                return PartialView("_DivisionPartial", divisions);
            }
        }

        [HttpPost]
        public PartialViewResult Edit(int DivisionIdEdit, string DivisionCodeEdit, string DivisionNameEdit)
        {
            IEnumerable<Division> divisions = new List<Division>();
            BCS_Context db = new BCS_Context();
            try
            {
                using (var dbtransaction = db.Database.BeginTransaction())
                {
                    Division division = new Division();
                    division = db.Division.FirstOrDefault(m => m.DivisionId == DivisionIdEdit);
                    division.Code = DivisionCodeEdit;
                    division.Name = DivisionNameEdit;
                    db.Entry(division).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    dbtransaction.Commit();
                    divisions = db.Division.ToList().OrderBy(m => m.Name);
                    //divisions.Add(division);
                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Division - Division Edited  - from Terminal: " + ipaddress);

                    return PartialView("_DivisionPartial", divisions);
                }
            }
            catch (Exception)
            {
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Division - Edit division failed  - from Terminal: " + ipaddress);
                divisions = db.Division.ToList().OrderBy(m => m.Name);
                return PartialView("_DivisionPartial", divisions);
            }
        }
    }
}