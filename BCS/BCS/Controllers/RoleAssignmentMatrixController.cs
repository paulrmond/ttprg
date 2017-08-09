using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Net;


namespace BCS.Controllers
{
    public class RoleAssignmentMatrixController : Controller
    {
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        BCS_Context db = new BCS_Context();
        // GET: RoleAssignmentMatrix
        [Authorize]
        [HttpGet]
        [ActionName("Details")]
        public ActionResult Details()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var zoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            List<RoleAssignmentDetailsViewModel> roleViewModel = new List<RoleAssignmentDetailsViewModel>();
            List<RoleAssignmentMatrix> roleAssignmentMatrix = new List<RoleAssignmentMatrix>();

            if (User.IsInRole("Super User"))
                roleAssignmentMatrix = db.RoleAssignmentMatrix.ToList();
            else
                roleAssignmentMatrix = db.RoleAssignmentMatrix.Where(m => m.ZoneGroup == zoneGroup).ToList();

            foreach (var item in roleAssignmentMatrix)
            {
                RoleAssignmentDetailsViewModel temprole = new RoleAssignmentDetailsViewModel();
                temprole.Id = item.RoleAssignmentMatrixId;
                temprole.UserName = item.UserName;
                roleViewModel.Add(temprole);
            }

            ViewBag.RoleModel = roleViewModel;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public ActionResult Details_Post(FormCollection frm)
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit(int id)
        {
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.Find(id);
            return View(roleAssignmentMatrix);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(RoleAssignmentMatrix roleAssignmentMatrix,string All,string user2)
        {
            roleAssignmentMatrix.UserName = user2;
            ApplicationDbContext context = new ApplicationDbContext();
            string zonegroup = context.Users.FirstOrDefault(m => m.UserName == roleAssignmentMatrix.UserName).ZoneGroup;
            roleAssignmentMatrix.ZoneGroup = zonegroup;
            TryUpdateModel(roleAssignmentMatrix);
            roleAssignmentMatrix.ZoneGroup = zonegroup;

            if(All == "All")
            {
                roleAssignmentMatrix.Administrative = true;
                roleAssignmentMatrix.Aging = true;
                roleAssignmentMatrix.Billing = true;
                roleAssignmentMatrix.Collection = true;
                roleAssignmentMatrix.Company = true;
                roleAssignmentMatrix.Franchise = true;
                roleAssignmentMatrix.Garbage = true;
                roleAssignmentMatrix.HO = true;
                roleAssignmentMatrix.JBR = true;
                roleAssignmentMatrix.PassedOnBilling = true;
                roleAssignmentMatrix.Payment = true;
                roleAssignmentMatrix.Period = true;
                roleAssignmentMatrix.Pole = true;
                roleAssignmentMatrix.Rate = true;
                roleAssignmentMatrix.Rentals = true;
                roleAssignmentMatrix.Security = true;
                roleAssignmentMatrix.Sewerage = true;
                roleAssignmentMatrix.SubsidiaryLedger = true;
                roleAssignmentMatrix.Water = true;            
            }

            db.Entry(roleAssignmentMatrix).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["TransactionSuccess"] = "Edit";
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Role Assignment - Role Edited  - from Terminal: " + ipaddress);
            return RedirectToAction("Details");
        }

        [Authorize]
        [HttpGet]
        [ActionName("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        public PartialViewResult GetUsers(string UserName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var zoneGroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            List<RoleAssignmentDetailsViewModel> roleViewModelAll = new List<RoleAssignmentDetailsViewModel>();
            List<RoleAssignmentDetailsViewModel> roleViewModelSingle = new List<RoleAssignmentDetailsViewModel>();

            List<RoleAssignmentMatrix> roleAssignmentMatrix = new List<RoleAssignmentMatrix>();

            if (User.IsInRole("Super User"))
                roleAssignmentMatrix = db.RoleAssignmentMatrix.ToList();
            else
                roleAssignmentMatrix = db.RoleAssignmentMatrix.Where(m => m.ZoneGroup == zoneGroup).ToList();

            foreach (var item in roleAssignmentMatrix)
            {
                RoleAssignmentDetailsViewModel temprole = new RoleAssignmentDetailsViewModel();
                temprole.Id = item.RoleAssignmentMatrixId;
                temprole.UserName = item.UserName;
                roleViewModelAll.Add(temprole);
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Role Assignment - Role Created  - from Terminal: " + ipaddress);
            }

            if (!string.IsNullOrEmpty(UserName))
            {
                RoleAssignmentDetailsViewModel temprole = new RoleAssignmentDetailsViewModel();
                temprole = roleViewModelAll.Where(m => m.UserName.ToUpper() == UserName.ToUpper()).FirstOrDefault();
                roleViewModelSingle.Add(temprole);

                return PartialView("_RoleAccessMatrixPartial", roleViewModelSingle);
            }
            else
            {
                return PartialView("_RoleAccessMatrixPartial", roleViewModelAll);
            }
      

        }
    }
}