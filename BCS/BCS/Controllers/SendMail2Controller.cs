using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
//using Postal;
namespace BCS.Controllers
{
    public class SendMail2Controller : Controller
    {
        // GET: SendMail2
        public ActionResult Index()
        {
           // dynamic email = new Email("rafael.christianpaul@gmail.com");
           // email.To = "cprafael@dci.ph";
           //// email.FunnyLink = DB.GetRandomLolcatLink();
           // email.Send();
           return View();
        }


       
    }
}