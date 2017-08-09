using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;

namespace BCS.Controllers
{
    public class SearchCompanyController : Controller
    {

        private BCS_Context db = new BCS_Context();
        // GET: SearchCompany
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(SearchCompany searchcompany)
        {
            Session["SearchInput"] = searchcompany.SearchInput;
            return RedirectToAction("List", "SearchCompany");
            //return View();
        }
        public ActionResult List()
        {
           
            var company = db.Company.ToList();
            if (!string.IsNullOrEmpty(Session["SearchInput"] as string))
                {
                
                company = db.Company.ToList().Where(c => c.CompanyName.Contains(Session["SearchInput"].ToString())).ToList();
            }
           
            return View(company);
        }
            
    }
}