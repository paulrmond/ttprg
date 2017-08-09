using BCS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Collections.Specialized;

namespace BCS.Controllers
{
    public class ReportsSubsidiaryLedgerController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        private BCS_Context db = new BCS_Context();
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: SubsidiaryLedger
        public ActionResult ViewSubsidiaryLedger()
        {
            SearchSubsidiaryLedgerViewModel SrchSubLedge = new SearchSubsidiaryLedgerViewModel();


            SrchSubLedge.ZoneGroupList = db.ZoneGroup.SqlQuery("SELECT * FROM ZoneGroups EXCEPT SELECT * FROM ZoneGroups WHERE ZoneGroupName = 'Super User'").ToList();


            return View("ViewSubsidiaryLedger", SrchSubLedge);
        }


        public JsonResult CompData(string comp ,string zone)
       {
            
            List<BillingPaymentJsonReturnData> billingPaymentJsonReturnData = new List<BillingPaymentJsonReturnData>();
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();



            if (zone == null)
            {
                var currentUserId = User.Identity.GetUserId();
                string zoneGroups = context.Users.FirstOrDefault(m => m.Id == currentUserId).ZoneGroup;
                var xx = from t1 in db.Company
                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                         where t3.ZoneGroupCode == zoneGroups
                         select t1;


                if (comp.Length >= 3)
                {

                    var Fees = xx.Where(x => x.CompanyName.Contains(comp)).Take(50).ToList();
                   // return Json(Fees);

                    foreach (var items in Fees)
                    {
                        searchop.CompanyCode.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyCode);
                        searchop.CompanyName.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyName);
                        searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        searchop.CompanyID.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyID.ToString());
                       // searchop.Address.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).Address);
                    }

                    for (int i = 0; i < searchop.CompanyName.Count; i++)
                    {
                        BillingPaymentJsonReturnData billingPaymentJsonReturnDataNew = new BillingPaymentJsonReturnData();
                        billingPaymentJsonReturnDataNew.CompanyCode = searchop.CompanyCode[i].ToString();
                        billingPaymentJsonReturnDataNew.CompanyName = searchop.CompanyName[i].ToString();
                        billingPaymentJsonReturnDataNew.ZoneName = searchop.ZoneName[i].ToString();
                        billingPaymentJsonReturnDataNew.Companyid = searchop.CompanyID[i].ToString();
                       // billingPaymentJsonReturnDataNew.CompAdd = searchop.Address[i].ToString();
                        billingPaymentJsonReturnData.Add(billingPaymentJsonReturnDataNew);
                    }
                    return Json(billingPaymentJsonReturnData);
                }

            }
            else
            {
                var xx = from t1 in db.Company
                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                         where t3.ZoneGroupCode == zone
                         select t1;

                if (comp.Length >= 3)
                {

                    var Fees = xx.Where(x => x.CompanyName.Contains(comp)).Take(10).ToList();
                    // return Json(Fees);

                    foreach (var items in Fees) 
                    {
                        searchop.CompanyCode.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyCode);
                        searchop.CompanyName.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyName);
                        searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                        searchop.CompanyID.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyID.ToString());
                      //  searchop.Address.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).Address);
                    }

                    for (int i = 0; i < searchop.CompanyName.Count; i++)
                    {
                        BillingPaymentJsonReturnData billingPaymentJsonReturnDataNew = new BillingPaymentJsonReturnData();
                        billingPaymentJsonReturnDataNew.CompanyCode = searchop.CompanyCode[i].ToString();
                        billingPaymentJsonReturnDataNew.CompanyName = searchop.CompanyName[i].ToString();
                        billingPaymentJsonReturnDataNew.ZoneName = searchop.ZoneName[i].ToString();
                        billingPaymentJsonReturnDataNew.Companyid = searchop.CompanyID[i].ToString();
                     //   billingPaymentJsonReturnDataNew.CompAdd = searchop.Address[i].ToString();

                        billingPaymentJsonReturnData.Add(billingPaymentJsonReturnDataNew);
                    }
                    return Json(billingPaymentJsonReturnData);
                }
            }

        
           return Json(null);
        }


        public JsonResult GetAddress(string comp2)
        {
            

                try
                {

                SearchSubsidiaryLedgerViewModel searchadd = new SearchSubsidiaryLedgerViewModel();

                var code = comp2.Split('-');
                var DevName = code[0];
                var EcoName = code[1];

                double compid = Convert.ToDouble(DevName);

                //  var zonez = db.Zone.FirstOrDefault(x => x.ZoneName == EcoName).ZoneCode;
                var address = db.Company.FirstOrDefault(x => x.CompanyID == compid).Address;
                return Json(address);


            }
                catch (Exception ex) { }
      
           
            return Json(null);

        }

      

        //// GET: DataEntryFranchise 
        //public ActionResult ViewCompanySubsidiary(string CompanyName)
        //{
        //    SearchSubsidiaryLedgerViewModel SrchSubLedge = new SearchSubsidiaryLedgerViewModel();
        //    if (!string.IsNullOrEmpty(CompanyName))
        //    {
        //        ApplicationDbContext context = new ApplicationDbContext();
        //        var userid = User.Identity.GetUserId();
        //        string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

        //        List<Company> NewCompanies = new List<Company>();
        //        NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();

        //        SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
        //        SrchSubLedge.CompanyList = searchCompanyPerGroup.Companies;
        //        //SearchFranchiseViewModels.Companies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
        //    }

        //    return View("ViewSubsidiaryLedger", SrchSubLedge);
        //}

        //[HttpPost]
        //[ActionName("ViewCompanySubsidiary")]
        //public ActionResult ViewCompanySubsidiary_post(string compid, string compName)
        //{
        //    SearchSubsidiaryLedgerViewModel SrchSubLedge = new SearchSubsidiaryLedgerViewModel();
        //    if (!string.IsNullOrEmpty(compName))
        //    {
        //        ApplicationDbContext context = new ApplicationDbContext();
        //        var userid = User.Identity.GetUserId();
        //        string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

        //        List<Company> NewCompanies = new List<Company>();
        //        NewCompanies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + compName + "%'").ToList();

        //        SearchCompanyPerGroup searchCompanyPerGroup = new SearchCompanyPerGroup(NewCompanies, ZoneGroup);
        //        SrchSubLedge.CompanyList = searchCompanyPerGroup.Companies;
        //        ViewBag.CompanySelected = "OK";
        //        //SearchFranchiseViewModels.Companies = db.Company.SqlQuery("Select * from Companies where CompanyName like '%" + CompanyName + "%'").ToList();
        //    }

        //    Company company = new Company();
        //    int id = int.Parse(compid);
        //    company = db.Company.FirstOrDefault(m => m.CompanyID == id);
        //    ViewBag.CompanyName = company.CompanyName;
        //    ViewBag.CompanyAddress = company.Address;
        //    ViewBag.CompanyID = id;

        //    return View("ViewSubsidiaryLedger", SrchSubLedge);
        //}


        // Generate Report
        [HttpPost]
        public ActionResult SubsidiaryLedger(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            string zoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            NameValueCollection settings = new NameValueCollection();

            switch (zoneGroup)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/SUBSIDIARY");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/SUBSIDIARY");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/SUBSIDIARY");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/SUBSIDIARY");
                    break;

                default:
                    break;
            }

            var zoneGroupCode = frm["zoneGroupCode"];
            string ceoName = settings["ceoName"].ToString();
            string representative = settings["representative"].ToString();
            var CompName = frm["companyID"];
            CompName = CompName.Replace("&","");

            var splitcomp = CompName.Split('-');
            var compid = splitcomp[0];
            var compname = splitcomp[1];

            //var CompID = db.Company.FirstOrDefault(x => x.CompanyName == CompName).CompanyID;
            var companyID = compid;
            var groupedStartDate = frm["tdStartDate"];
            var groupedEndDate = frm["tdEndDate"];
            var groupedARType = frm["billingtype"];

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);

            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Subsidiary Ledger - View Report   - from Terminal: " + ipaddress);

            serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fSubsidiaryLedgers&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&groupedStartDate=" + groupedStartDate + "&groupedEndDate=" + groupedEndDate + "&groupedARType=" + groupedARType + "&companyID=" + companyID + "&ceoName=" + ceoName + "&representative=" + representative;
            return Redirect(serverURI.Uri.ToString());
        }
    }
}