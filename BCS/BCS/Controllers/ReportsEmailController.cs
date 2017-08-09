using BCS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;



namespace BCS.Controllers
{
    public class ReportsEmailController : Controller
    {

        //LOGS
        systemlogger SL = new systemlogger();
        //GET IP ADDRESS
        string ipaddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();

        private BCS_Context db = new BCS_Context();
        // GET: ReportsEmail
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ViewReportsEmail()
        {

            SearchCompanyForEmail srch = new SearchCompanyForEmail();

            srch.companylist = db.Company.Where(c => c.SendEmail == "Yes").ToList();

            return View(srch);

        }

        [HttpPost]
        public ActionResult ViewReportsEmail(string atats, string foremail, string forsubject, string forbody, string[] langOpt3, HttpPostedFileBase fileUploader)
        {
            var emailvar = new List<string>();
            SearchCompanyForEmail srch = new SearchCompanyForEmail();

            srch.companylist = db.Company.Where(c => c.SendEmail == "Yes").ToList();

            //string path = "C:/Users/dev2/Documents/IAN/PROJECT FILES/Email/09072016/PBCS 9-6-16 5PM(CONSO)/BCS/BCS/PDF/GeneralBillingStatements.pdf";
            string path2 = HostingEnvironment.ApplicationPhysicalPath + atats;

            bool boleen = System.IO.File.Exists(path2);

            int x = langOpt3.Count() - 1;

            for (int i = 0; i <= x; i++)
            {

                var y = langOpt3[i];

                var ev = db.Company.SingleOrDefault(co => co.CompanyName == y).PrimaryEmailAddress;
                var ev2 = db.Company.SingleOrDefault(co => co.CompanyName == y).SecondaryEmailAddress;

                emailvar.Add(ev.ToString());
                emailvar.Add(ev2.ToString());
                if (ModelState.IsValid)
                {




                    var message = new MailMessage();
                    message.To.Add(new MailAddress(ev)); //replace with valid value
                    message.Subject = forsubject;
                    message.Body = forbody;
                    message.IsBodyHtml = true;
                    message.Attachments.Add(new Attachment(path2));





                    var message2 = new MailMessage();
                    message2.To.Add(new MailAddress(ev2)); //replace with valid value
                    message2.Subject = forsubject;
                    message2.Body = forbody;
                    message2.IsBodyHtml = true;
                    message2.Attachments.Add(new Attachment(path2));




                    using (var smtp = new SmtpClient())
                    {
                        try
                        {
                            smtp.Send(message);
                            smtp.Send(message2);
                            ViewBag.Message = "Sent";
                            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Email - Email Sent  - from Terminal: " + ipaddress);

                        }
                        catch
                        {
                            ViewBag.Message = "Not Sent";
                            SL.LogInfo(User.Identity.Name, Request.RawUrl, "Reports Email - Email Not Sent  - from Terminal: " + ipaddress);

                        }




                    }


                }
            }


            return View("ViewReportsEmail", srch);
        }
    }
}