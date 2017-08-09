using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace BCS.Controllers
{
    
    [ValidateInput(true)]
    public class RoleController : Controller
    {
        ApplicationDbContext context;

        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;
            return View(Roles);
        }

        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole Role)
        {
            var RoleManager1 = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));
            ApplicationRole approle = new ApplicationRole();
            approle.Name = Role.Name;
            var roles = context.Roles.ToList();
            approle.RoleNumber = roles.Count() + 1;


            if (!string.IsNullOrEmpty(Role.Name))
            {
                
                if (!RoleManager1.RoleExists(Role.Name))
                {
                    var saveRole = RoleManager1.Create(approle);
                    if (saveRole.Succeeded)
                    {
                        TempData["TransactionSuccess"] = "Add";
                        return RedirectToAction("Index");
                    }
                    //context.Roles.Add(Role);
                    //context.SaveChanges();
                    TempData["TransactionSuccess"] = "Add";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["TransactionSuccess"] = "Duplicate";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.TransactionSuccess = "Failed";
                return View("Create");
            }
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit(string roleId,string roleName)
        {
            ViewBag.RoleId = roleId;
            ViewBag.RoleName = roleName;
            //IdentityRole newRole = new IdentityRole();
            //newRole = context.Roles.SingleOrDefault(m => m.Id == roleId);
            //newRole.Name = "Supervisor";

            //context.Entry(newRole).State = System.Data.Entity.EntityState.Modified;
            //context.SaveChanges();

            return View("Edit");
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(string RoleName,string RoleId)
        {
            if (!string.IsNullOrEmpty(RoleName))
            {
                var RoleManager1 = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));
                if (!RoleManager1.RoleExists(RoleName))
                {
                    IdentityRole newRole = new IdentityRole();
                    newRole = context.Roles.SingleOrDefault(m => m.Id == RoleId);
                    newRole.Name = RoleName;

                    context.Entry(newRole).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    TempData["TransactionSuccess"] = "Edit";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["TransactionSuccess"] = "Duplicate";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.RoleId = RoleId;
                ViewBag.TransactionSuccess = "Failed";
                return View("Edit");
            }
        }
    }
}