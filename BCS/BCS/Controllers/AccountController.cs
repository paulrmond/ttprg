// Author: E.A.Cabatan
// Date Created: 5/18/16
// Date Modified:
// Consultant: DCI

using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BCS.Models;
using System.Collections;
using System.Collections.Generic;
using BCS.Models.Cryptography;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;

namespace BCS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationDbContext context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        BCS_Context db = new BCS_Context();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        public AccountController()
        {
            context = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            context = new ApplicationDbContext();
            ApplicationUser appuser = new ApplicationUser();

            appuser = context.Users.Where(m => m.UserName == model.UserName).FirstOrDefault();
            var status1 = appuser != null ? appuser.Status : "";
            //ApplicationUser appuser = new ApplicationUser();
            //appuser = context.Users.Where(m => m.UserName == model.UserName).Single();

            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            if (status1.ToUpper() != "INACTIVE")
            {
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
                switch (result)
                {
                    case SignInStatus.Success:
                        {


                            SL.LogInfo(model.UserName, Request.RawUrl, "Login Succesful  - from Terminal: " + ipaddress);
                            return RedirectToLocal(returnUrl);
                        }
                    case SignInStatus.LockedOut:
                        {
                            SL.LogError(model.UserName, Request.RawUrl, "Account Locked out! - from Terminal:" + ipaddress);
                            return View("Lockout");
                        }
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        {
                            ModelState.AddModelError("", "Invalid login attempt.");
                            SL.LogWarning(model.UserName, Request.RawUrl, "Invalid login attempt. - from Terminal:" + ipaddress);
                            return View(model);
                        }
                }
            }
            else
            {
                return View("CustomLockout");
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [Authorize]
        public ActionResult Register()
        {
            BCS_Context db = new BCS_Context();
            ViewBag.ZoneGroupCode = new SelectList(db.ZoneGroup.Where(m => m.ZoneGroupCode != "99"), "ZoneGroupCode", "ZoneGroupName"); //Dynamic name of viewbag will be passed in view.
            ViewBag.Name = new SelectList(context.Roles.Where(m => m.Name != "System Administrator" && m.Name != "Super User"), "Name", "Name");
            ViewBag.Division = new SelectList(db.Division.ToList(), "Code", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, String Name)
        {
            BCS_Context db = new BCS_Context();
            if (ModelState.IsValid)
            {
                Encrypt encrypt = new Encrypt(model.Password);
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, LastName = model.LastName, MiddleName = model.MiddleName, GivenName = model.GivenName, ZoneGroup = model.ZoneGroupCode, Status = model.Status, Division = model.Division, LocalPass = encrypt.EncryptedValue, Zone = model.Zone };
                var result = await UserManager.CreateAsync(user, model.Password);
                
                    if (result.Succeeded)
                    {
                        //Assign Role to user Here 
                        await this.UserManager.AddToRoleAsync(user.Id, Name);
                        RoleAssignmentMatrix roleAssignmentMatrix = new RoleAssignmentMatrix();
                        roleAssignmentMatrix.UserName = model.UserName;
                        roleAssignmentMatrix.ZoneGroup = model.ZoneGroupCode;
                        db.RoleAssignmentMatrix.Add(roleAssignmentMatrix);
                        await db.SaveChangesAsync();
                        //Ends Here
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        //return RedirectToAction("Index", "Home");
                        TempData["TransactionSuccess"] = "Add";

                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account Registered - from Terminal: " + ipaddress);

                        return RedirectToAction("ViewUser", "Account");
                    }
               
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form            
            ViewBag.ZoneGroupCode = new SelectList(db.ZoneGroup.ToList(), "ZoneGroupCode", "ZoneGroupName");
            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");
            ViewBag.Division = new SelectList(db.Division.ToList(), "Code", "Name");
            TempData["TransactionSuccess"] = "Failed";
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ViewUser(FormCollection frm, string SearchCompany)
        {
            List<ApplicationUser> user = new List<ApplicationUser>();
            var userid = User.Identity.GetUserId();
            var zonegroup = context.Users.FirstOrDefault(m => m.Id == userid).ZoneGroup;

            if (zonegroup != "99") //if superuser. show all users
            {
                if (string.IsNullOrEmpty(SearchCompany))
                    user = context.Users.Where(m => m.ZoneGroup == zonegroup).ToList();
                else
                {
                    IEnumerable<ApplicationUser> Appuser = context.Users.Where(m => m.ZoneGroup == zonegroup).ToList();
                    user = Appuser.Where(m => m.UserName.ToUpper() == SearchCompany.ToUpper()).ToList();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SearchCompany))
                    user = context.Users.ToList();
                else
                {
                    user = context.Users.Where(m => m.UserName.ToUpper() == SearchCompany.ToUpper()).ToList();
                }

            }

            for (int i = 0; i < user.Count; i++)
            {
                var userId = user[i].Id;
                var userRoleId = user[i].Roles.FirstOrDefault().RoleId;
                var roleName = context.Roles.SingleOrDefault(m => m.Id == userRoleId).Name;
                user[i].RoleName = roleName;
            }
            IEnumerable iuser = user.OrderBy(m => m.UserName);

            ViewBag.Users = iuser;
            ViewBag.TransactionSuccess = TempData["TransactionSuccess"] as string;

            //SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account - View User - from Terminal: " + ipaddress);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditUser(string id)
        {
            var user = context.Users.Find(id);
            var userRoleId = user.Roles.SingleOrDefault().RoleId;
            var roleName = context.Roles.SingleOrDefault(m => m.Id == userRoleId).Name;
            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name", roleName);

            return View(user);
        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteUser(string userid)
        {
            ApplicationUser appuser = new ApplicationUser(); //create new app user
            appuser = context.Users.Find(userid); //load the PK then assign updated values
            appuser.Status = "Inactive";
            context.Entry(appuser).State = System.Data.Entity.EntityState.Modified; //set the entity state to "updated"
            context.SaveChanges(); //save changes

            //var roleid = appuser.Roles.SingleOrDefault().RoleId; //get role id of user
            //var rolename = context.Roles.SingleOrDefault(m => m.Id == roleid).Name; // get role name of user
            //UserManager.RemoveFromRoles(userid, rolename); //remove from ASPNETUSERROLE
            //var updateuser = UserManager.UpdateAsync(appuser); //update the user data


            List<ApplicationUser> user = new List<ApplicationUser>();

            var loggedinId = User.Identity.GetUserId();
            var zonegroup = context.Users.FirstOrDefault(m => m.Id == loggedinId).ZoneGroup;

            if (zonegroup != "99") //if superuser. show all users
                user = context.Users.Where(m => m.ZoneGroup == zonegroup).ToList();
            else
                user = context.Users.ToList();

            for (int i = 0; i < user.Count; i++)
            {
                var userId = user[i].Id;
                var userRoleId = user[i].Roles.SingleOrDefault().RoleId;
                var roleName = context.Roles.SingleOrDefault(m => m.Id == userRoleId).Name;
                user[i].RoleName = roleName;
            }

            ViewBag.Users = user;
            return View("ViewUser");
        }

        [Authorize]
        [HttpPost]
        [ActionName("EditUser")]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser_Post(string id, FormCollection frm)
        {
            ApplicationUser appuser = new ApplicationUser(); //create new app user

            appuser = context.Users.Find(id); //load the PK then assign updated values

            appuser.GivenName = frm["GivenName"].ToString();
            //appuser.Email = frm["Email"].ToString();
            appuser.LastName = frm["LastName"].ToString();
            appuser.MiddleName = frm["MiddleName"].ToString();
            //appuser.UserName = frm["UserName"].ToString();

            var newrolename = frm["Name"].ToString();


            using (var transaction = context.Database.BeginTransaction()) //begin transaction
            {
                if (appuser != null)
                {
                    var roleid = appuser.Roles.SingleOrDefault().RoleId; //get role id of user
                    var rolename = context.Roles.SingleOrDefault(m => m.Id == roleid).Name; // get role name of user
                    UserManager.RemoveFromRoles(id, rolename); //remove from ASPNETUSERROLE
                    UserManager.AddToRole(id, newrolename); //replace with new one

                    var updateuser = UserManager.UpdateAsync(appuser); //update the user data
                    context.Entry(appuser).State = System.Data.Entity.EntityState.Modified; //set the entity state to "updated"

                    SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account - Edit User to " + appuser.GivenName + " " + appuser.LastName + " " + appuser.MiddleName + "Role:" + newrolename + " from Terminal:" + ipaddress);
                    context.SaveChanges(); //save changes
                }
                //Not necessary. Auto commit in using statement if no error occur.
                transaction.Commit();
            }

            //ViewBag.Users = context.Users.ToList();
            //ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");
            //return View("ViewUser");
            TempData["TransactionSuccess"] = "Edit";

            return RedirectToAction("ViewUser", "Account");
        }

        //
        // GET: /Account/ConfirmEmail
        [Authorize]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Confirm Email");
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [Authorize]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Forgot Password - something failed, redisplay form - from Terminal:" + ipaddress);
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [Authorize]
        public ActionResult ForgotPasswordConfirmation()
        {
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account - View Forgot Password - from Terminal:" + ipaddress);
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [Authorize]
        public ActionResult ResetPassword(string userid)
        {
            var code = UserManager.GeneratePasswordResetToken(userid);
            ResetPasswordViewModel res = new ResetPasswordViewModel();
            res.UserId = userid;
            res.Code = code;
            return View(res);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                Encrypt encrypt = new Encrypt(model.Password); //Update the local encrypted password
                user.LocalPass = encrypt.EncryptedValue;
                await UserManager.UpdateAsync(user);
                //context.Entry(user).State = System.Data.Entity.EntityState.Modified; <= will throw an error 
                //An entity object cannot be referenced by multiple instances of IEntityChangeTracker.

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            //var pass = model.ConfirmPassword;
            //var user = context.Users.Where(m => m.Id == model.UserId).Single();
            //// validate password using PasswordValidator.Validate
            //user.PasswordHash = PasswordHasher.HashPassword(pass);
            //Update(user);

            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account - Reset Password - from Terminal:" + ipaddress);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [Authorize]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account - User Logout -from Terminal:" + ipaddress);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        // Generate Report
        // View Users Info
        public ActionResult UserReport()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroupCode = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Account - View User Report - from Terminal:" + ipaddress);

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);
            serverURI.Query = serverURI.Query.ToString() + "%2fAdmin%2fUserAlphaList&rs:Command=Render&zoneGroupCode=" + zoneGroupCode;

            return Redirect(serverURI.Uri.ToString());
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public JsonResult SelectZone(string zonecode)
        {
            BCS_Context db = new BCS_Context();
            int i = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == zonecode).ZoneGroupId;
            List<Zone> zones = new List<Zone>();
            zones = db.Zone.Where(m => m.ZoneGroup == i.ToString()).ToList();

            return Json(zones);
        }

        public ActionResult UnlockUserPost(string userid)
        {
            ApplicationUser appuser = new ApplicationUser(); //create new app user
            appuser = context.Users.Find(userid); //load the PK then assign updated values
            appuser.Status = "Active";
            context.Entry(appuser).State = System.Data.Entity.EntityState.Modified; //set the entity state to "updated"
            context.SaveChanges(); //save changes

            //var roleid = appuser.Roles.SingleOrDefault().RoleId; //get role id of user
            //var rolename = context.Roles.SingleOrDefault(m => m.Id == roleid).Name; // get role name of user
            //UserManager.RemoveFromRoles(userid, rolename); //remove from ASPNETUSERROLE
            //var updateuser = UserManager.UpdateAsync(appuser); //update the user data


            List<ApplicationUser> user = new List<ApplicationUser>();

            var loggedinId = User.Identity.GetUserId();
            var zonegroup = context.Users.FirstOrDefault(m => m.Id == loggedinId).ZoneGroup;

            if (zonegroup != "99") //if superuser. show all users
                user = context.Users.Where(m => m.ZoneGroup == zonegroup).ToList();
            else
                user = context.Users.ToList();

            for (int i = 0; i < user.Count; i++)
            {
                var userId = user[i].Id;
                var userRoleId = user[i].Roles.SingleOrDefault().RoleId;
                var roleName = context.Roles.SingleOrDefault(m => m.Id == userRoleId).Name;
                user[i].RoleName = roleName;
            }

            ViewBag.Users = user;
            return View("ViewUser");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}