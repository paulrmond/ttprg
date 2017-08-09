using BCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;

namespace BCS.Controllers
{
    public class HOBatchUpdateController : Controller
    {
        
        Array  dbstr;    //var for db table to convert
        
        private BCS_Context db = new BCS_Context();

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        // GET: HOBatchUpdate
        public ActionResult HObatchupdate()
        {
           

            return View();
        }


     
       
        [HttpPost]
        public ActionResult HOSubsidiaryLedger(string[] HObatchupdate, DateTime datefor)
        {


            //END OF CREDENTIALS
        

            foreach (var its in HObatchupdate)
            {
                //GET CHECKBOX VALUE
                switch (its)
                {
                    case "SubsidiaryLedger":
                        dbstr = db.SubsidiaryLedger.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr1 = new int[dbstr.Length];
                        idstr1 = db.SubsidiaryLedger.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.SubsidiaryLedgerId).ToArray();         
                        fucns(dbstr, idstr1,its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Subsidiary Ledger Updated to ML Server  - from Terminal: " + ipaddress);
                        break;
                    case "GeneralBilling":
                        dbstr = db.GeneralBilling.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr2 = new int[dbstr.Length];
                        idstr2 = db.GeneralBilling.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.GeneralBillingId).ToArray();
                        fucns(dbstr, idstr2, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - General Billing Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    case "OrderOfPayment":
                        dbstr = db.OrderOfPayment.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr3 = new int[dbstr.Length];
                        idstr3 = db.OrderOfPayment.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.OrderOfPaymentId).ToArray();
                        fucns(dbstr, idstr3, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Order Of Payment Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    case "FranchiseFeeInformation":
                        dbstr = db.FranchiseFeeInformation.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr4 = new int[dbstr.Length];
                        idstr4 = db.FranchiseFeeInformation.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.FranchiseFeeInformationId).ToArray();
                        fucns(dbstr, idstr4, its);

                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Franchise Fee Information Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    case "GarbageInformations":
                        dbstr = db.GarbageInformations.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr5 = new int[dbstr.Length];
                        idstr5 = db.GarbageInformations.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.GarbageInformationId).ToArray();
                        fucns(dbstr, idstr5, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Garbage Informations Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    case "PoleInformation":
                        dbstr = db.PoleInformation.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr6 = new int[dbstr.Length];
                        idstr6= db.PoleInformation.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.PoleInformationId).ToArray();
                        fucns(dbstr, idstr6, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Pole Information Updated to ML Server  - from Terminal: " + ipaddress);
                                                break;
                    case "RentalInformation":
                        dbstr = db.RentalInformation.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr7 = new int[dbstr.Length];
                        idstr7 = db.RentalInformation.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.RentalInformationId).ToArray();
                        fucns(dbstr, idstr7, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Rental Information Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    case "SecurityGuardFeeInformation":
                        dbstr = db.PassedOnBillingInformation.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr8 = new int[dbstr.Length];
                        idstr8 = db.PassedOnBillingInformation.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.PassedOnBillingInformationId).ToArray();
                        fucns(dbstr, idstr8, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Security Guard Fee Information Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    case "WaterMeterReading":
                        dbstr = db.WaterMeterReading.Where(sl => sl.UpdateDate == datefor).ToArray();
                        int[] idstr9 = new int[dbstr.Length];
                        idstr8 = db.WaterMeterReading.Where(sl => sl.UpdateDate == datefor).Select(sl => sl.WaterMeterReadingId).ToArray();
                        fucns(dbstr, idstr9, its);
                        SL.LogInfo(User.Identity.Name, Request.RawUrl, "HO Batch Update - Water Meter Reading Updated to ML Server  - from Terminal: " + ipaddress);

                        break;
                    //default:
                    //    TempData["AlertMessage"] = "DATABASE UNKNOWN!!" + DateTime.Now.ToString();
                    //    return RedirectToAction("Index");

                }//END OF SWITCH
            }

        

     





          
            
               

            return RedirectToAction("HObatchupdate");


        }  //END OF HTTPOST HOSubsidiaryLedger

   
        public void fucns(Array dbstr, int[] idstr,string HObatchupdate)
        {
            // Set the credentials
            NetworkCredential credentials = new NetworkCredential("", "", "");
            credentials.Domain = ConfigurationManager.AppSettings["MarklogicDomain"];
            credentials.UserName = ConfigurationManager.AppSettings["MarklogicUsername"];
            credentials.Password = ConfigurationManager.AppSettings["MarklogicPassword"];

            var MLuri = ConfigurationManager.AppSettings["MarklogicURI"];
            int getid = 0;
            foreach (var item in dbstr)
            {

                var url = MLuri + HObatchupdate + "/" + HObatchupdate + idstr[getid].ToString() + ".json";
                // Create a request for the URL. 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = credentials;
                //set content type
                request.ContentType = "application/json";
                request.Method = "PUT";


                //write json to Marklogic



                var writer = new StreamWriter(request.GetRequestStream());
                var arryjson = JsonConvert.SerializeObject(item);
                writer.Write(arryjson);
                writer.Flush();



                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusDescription == "Content Updated")
                {
                    TempData["AlertMessage"] = "Content Updated " + DateTime.Now.ToString();
                }
                else if(response.StatusDescription == "Content Created")
                {
                    TempData["AlertMessage"] = "CREATED!!" + DateTime.Now.ToString();

                }
                else
                {
                    TempData["AlertMessage"] = "DATABASE UNKNOWN!!" + DateTime.Now.ToString();
                }
                getid = getid + 1;

            }
        }


    }

   



}


    




    
