using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using BCS.Models.Cryptography;
using System.Configuration;
using Microsoft.Reporting.WebForms;


namespace BCS.Controllers
{
    public class CodeTestingController : Controller
    {
        BCS_Context db = new BCS_Context();
        // GET: CodeTesting

        public ActionResult Index(FormCollection frm)
        {


            //AegingBLL ag = new AegingBLL();
            //ag.TestAeging();
            string a = "1,5,3,2,10,8,4,9,7,11,6,5";
            string[] ab = a.Split(',');
            int[] num = new int[ab.Length];
            bool isSorted = true;

            for (int i = 0; i < ab.Length; i++)
            {
                num[i] = int.Parse(ab[i]);
            }

            while (isSorted)
            {
                isSorted = false;
                for (int i = 0; i < ab.Length -1; i++)
                {
                    if (num[i] > num[i + 1])
                    {
                        int temp = num[i];
                        num[i] = num[i + 1];
                        num[i + 1] = temp;

                        isSorted = true;
                    }
                }
            }
            string c = string.Join(",", num);
            //throw new Exception("ERROR");
            return View();
        }
        [ValidateInput(true)]
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index_Post(FormCollection frm)
        {

            //TempData["Test"] = "Test message from GET";
            ViewBag.Test = "";
            //return RedirectToAction("Index", "CodeTesting");
            return View();
        }

        public ActionResult Index1()
        {
            List<Zone> zones = new List<Zone>();
            zones = db.Zone.ToList();

            return View();
        }

        public PartialViewResult GetCompanies(string CompanyName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            List<Company> NewCompanies = new List<Company>();
            List<Company> NewCompaniesPerGroup = new List<Company>();
            NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
            SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
            NewCompaniesPerGroup = searchCompanyPerGroup.Companies;
            return PartialView("_BillingSearchCompany", NewCompaniesPerGroup);
        }

        public PartialViewResult GetUsers(string UserName)
        {
            List<RoleAssignmentDetailsViewModel> roleViewModel = new List<RoleAssignmentDetailsViewModel>();
            if (!string.IsNullOrEmpty(UserName))
            {
                List<RoleAssignmentMatrix> roleAssignmentMatrix = db.RoleAssignmentMatrix.ToList();
                foreach (var item in roleAssignmentMatrix)
                {
                    RoleAssignmentDetailsViewModel temprole = new RoleAssignmentDetailsViewModel();
                    temprole.Id = item.RoleAssignmentMatrixId;
                    temprole.UserName = item.UserName;
                    roleViewModel.Add(temprole);
                }
                //return PartialView("_RoleAccessMatrix", roleViewModel);
            }
            return PartialView("_RoleAccessMatrixPartial", roleViewModel);
        }

        [HttpPost]
        [ActionName("Index1")]
        public ActionResult Index1_Post(FormCollection frm)
        {
            Encrypt en = new Encrypt("abcdef");
            string a = en.EncryptedValue;
            string encrypted = "";
            string pass = "abcdef";
            string EncriptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(pass);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncriptionKey, new byte[]
                {
                    0x49,0x76,0x61,0x6e,0x20,0x4d,0x65,
                    0x64,0x76,0x65,0x64,0x65,0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encrypted = Convert.ToBase64String(ms.ToArray());
                }
            }
            //////////////////////////////////////////////////////////////
            byte[] cipherBytes = Convert.FromBase64String(encrypted);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncriptionKey, new byte[]
                {
                    0x49,0x76,0x61,0x6e,0x20,0x4d,0x65,
                    0x64,0x76,0x65,0x64,0x65,0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encrypted = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return View();
            //return Redirect("~/Reports/CompanyReport.aspx");
        }


    }
}