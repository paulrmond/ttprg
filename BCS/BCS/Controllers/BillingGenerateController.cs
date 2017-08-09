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
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace BCS.Controllers
{
    [Authorize]
    [ValidateInput(true)]
    public class BillingGenerateController : Controller
    {
        private BCS_Context db = new BCS_Context();
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        public ActionResult ViewGeneratePRG()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Billing;

            List<BillingPeriod> billingperiod = new List<BillingPeriod>();
            billingperiod = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).OrderByDescending(m => m.BillingPeriodId).ToList();

            //if(TempData["BillingStatus"] != null)
            //    Response.Write("<script>alert('" + TempData["BillingStatus"].ToString() + "')</script>");
            ViewBag.BillingStatus = TempData["BillingStatus"] as string;
            ViewBag.Toggled = TempData["Toggled"] as string;
            return View("ViewGenerate", billingperiod);
        }

        // GET: BillingGenerate
        public ActionResult ViewGenerate()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Billing;

            List<BillingPeriod> billingperiod = new List<BillingPeriod>();
            billingperiod = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).OrderByDescending(m => m.BillingPeriodId).ToList();

            return View(billingperiod);
        }

        public ActionResult Finalize(int id)
        {
            var username = User.Identity.GetUserName();
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Billing;

            BillingRegenerateBLL billingRegenerateBLL = new BillingRegenerateBLL(id);

            if (billingRegenerateBLL.FinalizeBilling())
            {
                Response.Write("Billing period has been finalized.");
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Generate- Billing period has been finalized.  - from Terminal: " + ipaddress);
            } else { 
                Response.Write("Failed to finalize billing period. Please contact your system administrator.");
                SL.LogError(User.Identity.Name, Request.RawUrl, "Billing Generate -Failed to finalize billing period  - from Terminal: " + ipaddress);
            }
            var userid = User.Identity.GetUserId();
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); //initialize usermanager
            var rolename = UserManager.GetRoles(userid).FirstOrDefault(); //get the role name of current user
            UserAccessControl uac = new UserAccessControl("billing", rolename);
            ViewBag.IsValidRole = uac.userCanAccess();

            List<BillingPeriod> billingperiodlist = new List<BillingPeriod>();
            billingperiodlist = db.BillingPeriod.ToList();
            //return View("ViewGenerate", billingperiodlist);
            return RedirectToAction("ViewGeneratePRG", "BillingGenerate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewGenerate(FormCollection frm)
        {
            int generatePerCompanyId = 0;
            string BillingGenerateValue = "";
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            string ZoneGroup1 = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            RoleAssignmentMatrix roleAssignmentMatrix = db.RoleAssignmentMatrix.SingleOrDefault(m => m.UserName == username);
            ViewBag.IsValidRole = roleAssignmentMatrix.Billing;

            //CoverageFrom CoverageTo
            int billingPeriodId = int.Parse(frm["PeriodId"].ToString());

            VerifyBillingPeriod verify = new VerifyBillingPeriod(billingPeriodId, ZoneGroup1);
            if (verify.canGenerate()) //Verify if previous billing already generated a billing.
            {
                string FxRate = frm["FxRate"].ToString();
                DateTime dtCoverageFrom = Convert.ToDateTime(frm["CoverageFrom"].ToString());
                DateTime dtCoverageTo = Convert.ToDateTime(frm["CoverageTo"].ToString());
                DateTime dtBillingDate = Convert.ToDateTime(frm["BillDate"].ToString());
                DateTime dtBillingDue = Convert.ToDateTime(frm["DueDate"].ToString());
                Company company = new Company();

                string TypeOfBillingGenerate = frm["TypeOfBillingGenerate"].ToString(); 

                if (TypeOfBillingGenerate == "PerCompany")
                { 
                    generatePerCompanyId = Convert.ToInt32(frm["generatePerCompanyId"].ToString());
                    BillingGenerateValue = frm["TypeOfBillingGeneratePerCompany"].ToString();
                }
                else if(TypeOfBillingGenerate == "PerPOB")
                {
                    BillingGenerateValue = frm["TypeOfBillingGeneratePerPOB"].ToString();
                }                    
                else if (TypeOfBillingGenerate == "PerBillingType")
                {
                    BillingGenerateValue = frm["TypeOfBillingGeneratePerBillingType"].ToString();
                }                    
                else if (TypeOfBillingGenerate == "PerAdminFee")
                {
                    BillingGenerateValue = frm["TypeOfBillingGeneratePerAdminFee"].ToString();
                }
                    

                //Get the current user
                //ApplicationDbContext context = new ApplicationDbContext();
                string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
                BillingPeriod billingPeriod = db.BillingPeriod.Find(billingPeriodId);
                string isgenerated = billingPeriod.Generated;
                bool isGenerated = true; // delete this line of need to delete first the records in regenerate
                //bool isGenerated = false; //The role is Delete first the data then return as true.
                #region delete_records
                //08-01-2017 do not delete records in regenerate.. update only the changes
                //if (isgenerated.ToLower() == "yes")
                //{
                //    if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERCOMPANY") //Delete base on type of billing generate
                //    {
                //        int companyId = Convert.ToInt32(generatePerCompanyId);
                //        BillingRegenerateBLL billingRegenerateBLL = new BillingRegenerateBLL(billingPeriodId, companyId);
                //        isGenerated = billingRegenerateBLL.RegenerateBillingPerCompany();
                //        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Regenerate- Billing generate percompany.  - from Terminal: " + ipaddress);
                //    }
                //    else if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERENTERPRISE")
                //    {
                //        BillingRegenerateBLL billingRegenerateBLL = new BillingRegenerateBLL(billingPeriodId, BillingGenerateValue);
                //        isGenerated = billingRegenerateBLL.RegenerateBillingPerEnterprise();
                //        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Regenerate- Billing generate perenterprise.  - from Terminal: " + ipaddress);
                //    }
                //    else if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERBILLINGTYPE")
                //    {
                //        BillingRegenerateBLL billingRegenerateBLL = new BillingRegenerateBLL(billingPeriodId, BillingGenerateValue);
                //        isGenerated = billingRegenerateBLL.RegenerateBillingPerBillingType();
                //        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Regenerate- Billing generate perbillingtype.  - from Terminal: " + ipaddress);
                //    }
                //    else if (!string.IsNullOrEmpty(TypeOfBillingGenerate) && TypeOfBillingGenerate.ToUpper() == "PERADMINFEE")
                //    {
                //        BillingRegenerateBLL billingRegenerateBLL = new BillingRegenerateBLL(billingPeriodId, BillingGenerateValue);
                //        isGenerated = billingRegenerateBLL.RegenerateBillingPerAdminFee();
                //        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Regenerate- Billing generate peradminfee.  - from Terminal: " + ipaddress);
                //    }
                //    else
                //    {
                //        BillingRegenerateBLL billingRegenerateBLL = new BillingRegenerateBLL(billingPeriodId);
                //        isGenerated = billingRegenerateBLL.RegenerateBilling();
                //        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Regenerate- Billing generate all company.  - from Terminal: " + ipaddress);
                //    }                    
                //}
                #endregion delete_records
                //BillingBusinessLayer billingBusinessLayer = new BillingBusinessLayer(ZoneGroup, dtCoverageFrom, dtCoverageTo, dtBillingDate, dtBillingDue, billingPeriodId, userid, FxRate, generatePerCompanyId, TypeOfBillingGenerate);
                BillingBusinessLayerNewAlgo billingBusinessLayerNewAlgoTemp = new BillingBusinessLayerNewAlgo(ZoneGroup, dtCoverageFrom, dtCoverageTo, dtBillingDate, dtBillingDue, billingPeriodId, userid, FxRate, generatePerCompanyId, TypeOfBillingGenerate, BillingGenerateValue);

                //bool isCurrencyValid = true;
                bool isCurrencyValid = billingBusinessLayerNewAlgoTemp.isValidFxRate(FxRate);
                if (dtBillingDue >= dtCoverageFrom && dtBillingDate >= dtCoverageFrom)
                {
                    if (db.BillingRates.Any(m => m.Category == "Sewerage" && m.ZoneGroup == ZoneGroup))
                    {
                        if (isCurrencyValid)
                        {
                            BillingPeriod CloseBillingPeriod = db.BillingPeriod.Find(billingPeriodId);
                            CloseBillingPeriod.IsPaymentOpen = false;
                            db.Entry(CloseBillingPeriod).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            using (var dbtransaction = db.Database.BeginTransaction())
                            {
                                try //If Custom error is Enabled. Delete this try catch block.. "Using" statement above will automatically rollback transaction when error occur.
                                {
                                    if (isgenerated.ToLower() == "yes" && isGenerated)
                                    {
                                        //BillingBusinessLayerNewAlgo billingBusinessLayerNewAlgo = new BillingBusinessLayerNewAlgo(ZoneGroup, dtCoverageFrom, dtCoverageTo, dtBillingDate, dtBillingDue, billingPeriodId, userid, FxRate, generatePerCompanyId, TypeOfBillingGenerate, BillingGenerateValue);
                                        //billingBusinessLayer.generateBilling();
                                        billingBusinessLayerNewAlgoTemp.generateBilling();
                                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Regenerate - Billing generate complete.  - from Terminal: " + ipaddress);
                                        TempData["BillingStatus"] = "Billing generate complete";//Response.Write("<script>alert('Billing generate complete.')</script>");
                                    }
                                    else
                                    {
                                        //BillingBusinessLayerNewAlgo billingBusinessLayerNewAlgo = new BillingBusinessLayerNewAlgo(ZoneGroup, dtCoverageFrom, dtCoverageTo, dtBillingDate, dtBillingDue, billingPeriodId, userid, FxRate, generatePerCompanyId, TypeOfBillingGenerate, BillingGenerateValue);
                                        //billingBusinessLayer.generateBilling();
                                        billingBusinessLayerNewAlgoTemp.generateBilling();
                                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Generate- Billing generate complete.  - from Terminal: " + ipaddress);
                                        TempData["BillingStatus"] = "Billing generate complete"; //Response.Write("<script>alert('Billing generate complete.')</script>");
                                    }
                                }
                                catch (DbEntityValidationException ex)
                                {
                                    //    catch (DbEntityValidationException dbEx)
                                    //{
                                    foreach (var validationErrors in ex.EntityValidationErrors)
                                    {
                                        foreach (var validationError in validationErrors.ValidationErrors)
                                        {
                                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                                    validationError.PropertyName,
                                                                    validationError.ErrorMessage);
                                        }
                                        //    }
                                        }

                                        string ex1 = ex.Message;
                                    TempData["BillingStatus"] = ex1;  //Response.Write("<script>alert('Billing Generation failed.')</script>");
                                    dbtransaction.Rollback();
                                }
                            }//End of using
                             //BillingPeriod OpenBillingPeriod = db.BillingPeriod.Find(billingPeriodId);
                             //OpenBillingPeriod.IsPaymentOpen = true;
                             //db.Entry(OpenBillingPeriod).State = System.Data.Entity.EntityState.Modified;
                             //db.SaveChanges();
                        }//End of is not USD
                        else
                        {
                            TempData["BillingStatus"] = "Billing Generation failed. The system has detected USD type of billing. Please input data in FX Rate field.";
                            SL.LogWarning (User.Identity.Name, Request.RawUrl, "Billing Generate- Billing Generation failed.  - from Terminal: " + ipaddress);
                            //Response.Write("<script>alert('Billing Generation failed. The system has detected USD type of billing. Please input data in FX Rate field.')</script>");
                        }
                    }
                    else
                    {
                        TempData["BillingStatus"] = "Unable to proceed. No Sewerage rate detected.";
                        SL.LogError(User.Identity.Name, Request.RawUrl, "Billing Generate- Unable to proceed.No Sewerage rate detected.  - from Terminal: " + ipaddress);


                        //Response.Write("<script>alert('Unable to proceed. No Sewerage rate detected.')</script>");
                    }
                }
                else
                {
                    TempData["BillingStatus"] = "Unable to proceed. Billing Date/Due Date must be greater than Coverage Date.";
                }
            }
            else
            {
                TempData["BillingStatus"] = "Unable to proceed. Previous billing period already generated a billing.";
                //Response.Write("<script>alert('Unable to proceed. Previous billing period already generated a billing.')</script>");
            }
            //
            List<BillingPeriod> billingperiodlist = new List<BillingPeriod>();
            billingperiodlist = db.BillingPeriod.ToList();
            //return View("ViewGenerate", billingperiodlist);
            TempData["Toggled"] = "Toggled";
            return RedirectToAction("ViewGeneratePRG", "BillingGenerate");
        }

        public PartialViewResult GetCompanies(string CompanyName)
        {
            if (!string.IsNullOrEmpty(CompanyName))
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var userid = User.Identity.GetUserId();
                string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

                List<Company> NewCompanies = new List<Company>();
                List<Company> NewCompaniesPerGroup = new List<Company>();
                NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
                SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
                NewCompaniesPerGroup = searchCompanyPerGroup.Companies;
                SL.LogInfo(User.Identity.Name, Request.RawUrl, "Billing Generate- View/Get Companies  - from Terminal: " + ipaddress);

                return PartialView("_BillingSearchCompany", NewCompaniesPerGroup);
            }
            else
            {
                List<Company> NewCompaniesPerGroup = new List<Company>();
                return PartialView("_BillingSearchCompany", NewCompaniesPerGroup);
            }
        }

    }
}