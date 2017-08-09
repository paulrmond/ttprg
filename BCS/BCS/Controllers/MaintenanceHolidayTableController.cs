 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;

namespace BCS.Controllers
{
    public class MaintenanceHolidayTableController : Controller
    {
        private BCS_Context db = new BCS_Context();
        // GET: MaintenanceHolidayTable

        public ActionResult ViewHolidayTable(FormCollection frm)
        {
            if (frm.Count == 0)
            {
                SearchHolidayTableViewModel SearchHolidayViewModels = new SearchHolidayTableViewModel();
                SearchHolidayViewModels.HolidayList = db.HolidayTable.SqlQuery("Select * from HolidayTables").ToList();
                return View(SearchHolidayViewModels);
            }
            else if (frm.Count >= 1)
            {
                int parsedID = int.Parse(frm["HolidayTableId"]);
                HolidayTable holiday = db.HolidayTable.Find(parsedID);
                db.HolidayTable.Remove(holiday);
                db.SaveChanges();
                return RedirectToAction("ViewHolidayTable");
            }
            return View();
        }

        public ActionResult AddHoliday(string Month, string Day, string Year , string Description)
        {
            SearchHolidayTableViewModel SearchHolidayViewModels = new SearchHolidayTableViewModel();

            HolidayTable HolidayAssignment = new HolidayTable();
            //CONDITION
            string Date = Month + "/" + Day + "/" + Year;
            HolidayAssignment.HolidayTableDate = Convert.ToDateTime(Date).Date;
            HolidayAssignment.HolidayTableDescription = Description;
            db.HolidayTable.Add(HolidayAssignment);
            db.SaveChanges();

            SearchHolidayViewModels.HolidayList = db.HolidayTable.SqlQuery("Select * from HolidayTables").ToList();

            return View("ViewHolidayTable", SearchHolidayViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateHoliday(FormCollection frmcollection)
        {
            HolidayTable holiinfo = null;
            int ParsedIntID = int.Parse(frmcollection["ID"]);
            holiinfo = db.HolidayTable.Find(ParsedIntID);

            if (holiinfo != null)
            {
                //compinfo.CompanyCode = frmcollection["CompanyCode"].ToString();
                holiinfo.HolidayTableDate = Convert.ToDateTime(frmcollection["Date"].ToString()).Date;
                holiinfo.HolidayTableDescription = frmcollection["Description"].ToString();
                db.Entry(holiinfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                throw new Exception("Invalid transaction.");
            }
            int parsedID = int.Parse(frmcollection["ID"]);
            SearchHolidayTableViewModel searchholiday = new SearchHolidayTableViewModel();
            searchholiday.HolidayList = db.HolidayTable.SqlQuery("Select * from HolidayTables").ToList();
            return View("ViewHolidayTable", searchholiday);
        }

    }
}