using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCS.Models;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Net;
using System.Web.Script.Serialization;

namespace BCS.Controllers
{
    [Authorize]
    public class EndOfMonthProcessingController : Controller
    {
        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();


        // GET: EndOfMonthProcessing
        public ActionResult GenerateEOMProcessing()
        {
            //ApplicationDbContext context = new ApplicationDbContext();
            //var userid = User.Identity.GetUserId();
            //string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
            //int month = EOMGetProcessMonth.GetMonth(ZoneGroup);
            //ViewBag.CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            return View();
        }        
        private class DeserializeEOMData
        {
            public int month1 { get; set; }
            public int year1 { get; set; }
        }
        [HttpPost]
        public JsonResult GenerateEOM(string newData)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var a = serializer.Deserialize<DeserializeEOMData>(newData);
            var TransactionStatus = "";
            AegingBLL aeging = new AegingBLL(a.month1, a.year1);
            var isSuccess = aeging.TestAeging();

            if(isSuccess)
                TransactionStatus = "End Of Month Processing Complete";
            else
                TransactionStatus = "End Of Month Processing Failed";
            SL.LogInfo(User.Identity.Name, Request.RawUrl, "EOM Processing - End of Month Processing Generated - from Terminal: " + ipaddress);
            return Json(TransactionStatus);
        }        
    }
}

//public ActionResult GenerateEOMProcessingRPG()
//{
//    ViewBag.CurrentMonth = TempData["CurrentMonth"] as string;
//    TempData.Keep("CurrentMonth");
//    ViewBag.TransactionSuccess = TempData["TransactionStatus"] as string;
//    return View("GenerateEOMProcessing");
//}

//[HttpPost]
//[ActionName("GenerateEOMProcessing")]
//public ActionResult GenerateEOMProcessingPost()
//{
//    string TransactionStatus = "";
//ApplicationDbContext context = new ApplicationDbContext();
//var userid = User.Identity.GetUserId();
//string ZoneGroup = context.Users.SingleOrDefault(m => m.Id == userid).ZoneGroup;
//var maxBillingPeriodId = db.BillingPeriod.Where(m => m.groupCode == ZoneGroup).Max(m => m.BillingPeriodId);
//var mo = db.BillingPeriod.FirstOrDefault(m => m.BillingPeriodId == maxBillingPeriodId).DateFrom;
//var billingPeriodStatus = db.BillingPeriod.FirstOrDefault(m => m.BillingPeriodId == maxBillingPeriodId).Finalized;
//int month = mo.Month;
////int month = EOMGetProcessMonth.GetMonth(ZoneGroup);
////ViewBag.CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
////TempData["CurrentMonth"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
//ViewBag.CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
//    TempData["CurrentMonth"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
//    if (billingPeriodStatus.ToUpper() == "YES")
//    {
//        if (month > 0)
//        {
//            EOMBusinessLayer eOMBusinessLayer = new EOMBusinessLayer(userid, month, maxBillingPeriodId);

//            if (eOMBusinessLayer.EOMTransaction())
//                TransactionStatus = "End Of Month Processing Complete";
//            else
//                TransactionStatus = "End Of Month Processing Failed";
//        }
//        else
//        {
//            TransactionStatus = "NoBillMonths";
//        }
//    }
//    else
//    {
//        TransactionStatus = "Billing period for the month of " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " must be finalized.";
//    }
//    SL.LogInfo(User.Identity.Name, Request.RawUrl, "EOM Processing - End of Month Processing Generated - from Terminal: " + ipaddress);
//    return Json(TransactionStatus);