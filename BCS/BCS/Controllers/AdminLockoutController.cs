using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;

namespace BCS.Controllers
{

   
    [Authorize]
    public class AdminLockoutController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        // GET: AdminLockout

        public ActionResult UnlockAdmin()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> AppUsers = new List<ApplicationUser>();
            List<ApplicationUser> LockedUsers = new List<ApplicationUser>();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            AppUsers = context.Users.ToList();
            foreach (var item in AppUsers)
            {
                if (UserManager.IsLockedOut(item.Id)) //check all locked user
                    LockedUsers.Add(item);
            }

            //ViewBag.Users = LockedAppUsers;            
            ViewBag.Users = LockedUsers;
            return View();
        }

        public ActionResult UnlockAdminPost(string userid)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //unlock the USER using Identity framework/            
            var a = UserManager.IsLockedOut(userid); //check the status if locked            
            var juan = UserManager.FindById(userid); //get the id of locked user
            var dt = juan.LockoutEndDateUtc; //get the lockout date
            juan.LockoutEndDateUtc = dt.Value.AddDays(-1); //modify the lockout date. must be less than the current date time

            var b = UserManager.IsLockedOut(userid); //check if lockout status changed
            context.SaveChanges(); //save changes


            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Admin Lockout - Lockout Status Updated  - from Terminal: " + ipaddress);

            return RedirectToAction("UnlockAdmin");
        }
    }
}