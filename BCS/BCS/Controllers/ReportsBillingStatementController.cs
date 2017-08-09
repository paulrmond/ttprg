using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;

namespace BCS.Controllers
{

    public class ReportsBillingStatementController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        // GET: ReportBillingStatement
        ApplicationDbContext context = new ApplicationDbContext();

        private BCS_Context db = new BCS_Context();

        public ActionResult ViewBillingStatement()
        {
            SearchReportsBillingStatementModels srchBS = new SearchReportsBillingStatementModels();

            srchBS.ZoneGroupList = db.ZoneGroup.SqlQuery("SELECT * FROM ZoneGroups EXCEPT SELECT * FROM ZoneGroups WHERE ZoneGroupName = 'Super User'").ToList();
            //srchBS.ZoneList = db.Zone.SqlQuery("Select * from Zones").ToList();
            var userid = User.Identity.GetUserId();
            string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
      
            srchBS.BillingPeriodList = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).ToList();
            srchBS.EnterpriseTypeList = db.Database.SqlQuery<EnterpriseTypes>("SELECT DISTINCT EnterpriseType FROM Companies ORDER BY EnterpriseType ASC").ToList();
            srchBS.Companylist = db.Company.SqlQuery("Select * from Companies").ToList();
            srchBS.ZoneList = db.Zone.SqlQuery("Select * from Zones").ToList();
            return View("ViewBillingStatement", srchBS);
        }


        
        public JsonResult GetDevelopers()
        {
            var enterpriseList = db.Database.SqlQuery<EnterpriseTypes>("SELECT DISTINCT EnterpriseType FROM Companies ORDER BY EnterpriseType ASC").ToList();
            return Json(enterpriseList);
        }


        public JsonResult getcompany(string comp, string type)
        {
            List<BillingPaymentJsonReturnData> billingPaymentJsonReturnData = new List<BillingPaymentJsonReturnData>();
            SearchMainOrderOfPaymentViewModel searchop = new SearchMainOrderOfPaymentViewModel();

            if (type == "AdminFee")
            {
                if (comp.Length >= 3)
                {
                    var companies = db.Database.SqlQuery<DeveloperDetails>("SELECT DISTINCT Dev_Comp_Code, Developer, Zone_Code, Ecozone from AdminFees left join Zones on AdminFees.Zone_Code = Zones.ZoneCode where AdminFees.Developer like '%" + comp + "%'").ToList();

                    return Json(companies);
                }
            }

            else
            {
                if (comp.Length >= 3)
                {
                    var newcomp = comp.Replace("'", "''");
                    var complist = db.Company.SqlQuery("SELECT * FROM COMPANIES WHERE CompanyName like '" + newcomp + "%'").Take(15).ToList();
                     
                    foreach (var items in complist)
                    {
                        searchop.CompanyID.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyID.ToString());
                        searchop.CompanyName.Add(db.Company.FirstOrDefault(x => x.CompanyID == items.CompanyID).CompanyName);
                        searchop.ZoneName.Add(db.Zone.FirstOrDefault(x => x.ZoneCode == items.ZoneCode).ZoneName);
                    }

                    for (int i = 0; i < searchop.CompanyName.Count; i++)
                    {
                        BillingPaymentJsonReturnData billingPaymentJsonReturnDataNew = new BillingPaymentJsonReturnData();
                        billingPaymentJsonReturnDataNew.CompanyID = Convert.ToInt32(searchop.CompanyID[i]);
                        billingPaymentJsonReturnDataNew.CompanyName = searchop.CompanyName[i].ToString();
                        billingPaymentJsonReturnDataNew.ZoneName = searchop.ZoneName[i].ToString();
                        billingPaymentJsonReturnData.Add(billingPaymentJsonReturnDataNew);
                    }
                    return Json(billingPaymentJsonReturnData);
                }  
            }

            return Json(null);
        }

        [HttpPost]
        public JsonResult returnCompany(string zonename, string zonegroupcode, string enttype, string comp)
        {
            SearchCompany companysearch = new SearchCompany();


            var userid = User.Identity.GetUserId();
            string ZoneGroupx = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            if (comp.Length >= 3)
            {
                if (zonegroupcode == null)
                {

                    if (enttype == "")
                    {
                        if (zonename == "All")
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == ZoneGroupx
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                        else
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == ZoneGroupx && t2.ZoneCode == zonename && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                    }
                    else if (enttype == "All")
                    {
                        if (zonename == "All")
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == ZoneGroupx && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                        else
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == ZoneGroupx && t2.ZoneCode == zonename && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                    }
                    else
                    {
                        var selcom = from t1 in db.Company
                                     join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                     join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                     where t3.ZoneGroupCode == ZoneGroupx && t2.ZoneCode == zonename && t1.EnterpriseType == enttype && t1.CompanyName.StartsWith(comp)
                                     orderby t1.CompanyName
                                     select t1;
                        companysearch.CompanyList = selcom.ToList();

                    }

                }
                else
                {
                    if (enttype == "")
                    {
                        if (zonename == "All")
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == zonegroupcode && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                        else
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == zonegroupcode && t2.ZoneCode == zonename && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                    }
                    else if (enttype == "All")
                    {
                        if (zonename == "All")
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == zonegroupcode && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                        else
                        {
                            var selcom = from t1 in db.Company
                                         join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                         join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                         where t3.ZoneGroupCode == zonegroupcode && t2.ZoneCode == zonename && t1.CompanyName.StartsWith(comp)
                                         orderby t1.CompanyName
                                         select t1;
                            companysearch.CompanyList = selcom.ToList();
                        }
                    }
                    else
                    {
                        var selcom = from t1 in db.Company
                                     join t2 in db.Zone on t1.ZoneCode equals t2.ZoneCode
                                     join t3 in db.ZoneGroup on t2.ZoneGroup equals t3.ZoneGroupId.ToString()
                                     where t3.ZoneGroupCode == zonegroupcode && t2.ZoneCode == zonename && t1.EnterpriseType == enttype && t1.CompanyName.StartsWith(comp)
                                     orderby t1.CompanyName
                                     select t1;
                        companysearch.CompanyList = selcom.ToList();

                    }
                }
            
            }
            return Json(companysearch);

    

            //if (a != "All")
            //{
            //    companysearch.CompanyList = db.Company.OrderBy(x=> x.CompanyName).Where(x => x.ZoneCode == a).ToList();
            //}
            //else
            //{
            //    companysearch.CompanyList = db.Company.SqlQuery("Select * from Companies ORDER BY CompanyName ASC").ToList();
            //}



        }

        [HttpPost]
        public JsonResult returnZone(string a)
        {

            BillingStatementViewModel billingStatementViewModel = new BillingStatementViewModel();
            int zonegroupid = db.ZoneGroup.FirstOrDefault(m => m.ZoneGroupCode == a).ZoneGroupId;
            billingStatementViewModel.zone = db.Zone.Where(m => m.ZoneGroup == zonegroupid.ToString()).ToList();
            billingStatementViewModel.billingPeriod = db.BillingPeriod.OrderByDescending(x=> x.BillingPeriodId).Where(m => m.groupCode == a).ToList();
            return Json(billingStatementViewModel);
        }

        [HttpPost]
        public ActionResult ViewBillingStatement(FormCollection frm)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userid = User.Identity.GetUserId();
            var fname = context.Users.SingleOrDefault(m => m.Id == userid).GivenName;
            var mname = context.Users.SingleOrDefault(m => m.Id == userid).MiddleName;
            var lname = context.Users.SingleOrDefault(m => m.Id == userid).LastName;

            var signedUser = fname + " " + mname + " " + lname;

            string zoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;

            NameValueCollection settings = new NameValueCollection();

                switch (zoneGroup)
            {
                case "01":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("HeadOffice/GENERALBILLING");
                    break;

                case "03":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("CaviteEconomicZone/GENERALBILLING");
                    break;

                case "06":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("BaguioCityEconomicZone/GENERALBILLING");
                    break;

                case "09":
                    settings = (NameValueCollection)ConfigurationManager.GetSection("MactanEconomicZone/GENERALBILLING");
                    break;

                default:
                    break;
            }

            
            string receivedBy = settings["receivedBy"].ToString();
            string approvedBy = settings["approvedBy"].ToString();
            string preparedBy = settings["preparedBy"].ToString();

            string position = settings["position"].ToString();
            string position2 = settings["position2"].ToString();

            var zoneGroupCode = frm["zoneGroupCode"];
            var zoneType = frm["zoneName"];
            zoneType = zoneType.Replace("&", "");

            var billingPeriod = frm["billingPeriod"];
            var generateType = frm["generatedType"];
            var filteredBy = frm["filteredby"];
            filteredBy = filteredBy.Replace(",", "");

            if (generateType == "AdminFee")
            {
                if (filteredBy == "AllCompany")
                {
                    filteredBy = filteredBy.ToString();
                }
                else
                {
                    if (zoneType == "All")
                    {
                        var splitTo = filteredBy.Split('|');
                        var compCode = splitTo[0];
                        var compName = splitTo[1];

                        filteredBy = compCode;
                    }
                    else
                    {
                        var newFilter2 = filteredBy.Split('|');
                        var compCode = newFilter2[0];
                        var compName = newFilter2[1];

                        filteredBy = compCode;
                    }

                    //int compId = Convert.ToInt32(newFilter[0]);
                    //var filteredFilteredBy = newFilter[1];

                    //var newFilter2= filteredFilteredBy.Split('~');
                    //var compName = newFilter2[0]; 
                    //var zoneName = newFilter2[1];

                    //zoneType = db.Zone.SingleOrDefault(x => x.ZoneName.Replace(" ","") == zoneName.Replace(" ","")).ZoneCode;
                    //filteredBy = db.Company.SingleOrDefault(x => x.CompanyName.Replace(" ", "") == compName.Replace(" ","") && x.ZoneCode == zoneType && x.CompanyID == compId).CompanyCode;
                }
            }
            else
            {
                if (filteredBy == "AllCompany")
                {
                    filteredBy = filteredBy.ToString();
                }
                else
                {
                    if (zoneType == "All")
                    {
                        var newFilter = filteredBy.Split('|');
                        int compId = Convert.ToInt32(newFilter[0]);
                        var filteredFilteredBy = newFilter[1];

                        var newFilter2 = filteredFilteredBy.Split('~');
                        var compName = newFilter2[0];
                        var zoneName = newFilter2[1];

                        zoneType = db.Zone.SingleOrDefault(x => x.ZoneName.Replace(" ", "") == zoneName.Replace(" ", "")).ZoneCode;
                        filteredBy = db.Company.SingleOrDefault(x => x.CompanyName.Replace(" ", "").Replace(",", "") == compName.Replace(" ", "") && x.ZoneCode == zoneType && x.CompanyID == compId).CompanyCode;
                    }

                    else
                    {
                        var newFilter2 = filteredBy.Split('|');
                        var compCode = newFilter2[0];
                        var compName = newFilter2[1];

                        filteredBy = compCode;
                    }                    
                }
            }

            filteredBy = filteredBy.Replace("&", "");
            var entrpType = frm["entrpType"];

            string serverURL = ConfigurationManager.AppSettings["serverURL"].ToString();

            UriBuilder serverURI = new UriBuilder(serverURL);

            switch (generateType)
            {

                case "AllBillingType":   
                    serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fBillingStatements&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&entrpType=" + entrpType + "&zoneType=" + zoneType + "&filteredBy=" + filteredBy + "&approvedBy=" + approvedBy + "&receivedBy=" + receivedBy + "&position=" + position + "&preparedBy=" + signedUser;
                    return Redirect(serverURI.Uri.ToString());

                case "PassedOnBilling":
                    serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fPassedOnBillingStatements&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&entrpType=" + entrpType + "&zoneType=" + zoneType + "&filteredBy=" + filteredBy + "&approvedBy=" + approvedBy + "&receivedBy=" + receivedBy + "&position=" + position + "&position2=" + position2 + "&signedUser=" + signedUser + "&preparedBy=" + preparedBy;
                    return Redirect(serverURI.Uri.ToString());

                case "AdminFee":
                    serverURI.Query = serverURI.Query.ToString() + "%2fCollectionReports%2fAdminFeeBillingStatements&rs:Command=Render&zoneGroupCode=" + zoneGroupCode + "&billingPeriod=" + billingPeriod + "&entrpType=" + entrpType + "&zoneType=" + zoneType + "&filteredBy=" + filteredBy + "&approvedBy=" + approvedBy + "&receivedBy=" + receivedBy + "&position=" + position + "&preparedBy=" + position2;
                    return Redirect(serverURI.Uri.ToString());
                default:
                    break;
            }

            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Billing Statement - View Report  - from Terminal: " + ipaddress);
            return View("ViewBillingStatement");
        }

    }


}


