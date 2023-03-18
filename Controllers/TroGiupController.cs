using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Sneaker.Controllers
{
    public class TroGiupController : Controller
    {
        // GET: TroGiup
        [HttpGet]
        public ActionResult SendGmail()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SendGmail(string ToEmailId, string Subject, string Message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(ToEmailId);
                mail.From = new MailAddress("jacknhoxdx3@gmail.com");
                mail.Subject = Subject;
                string Body = Message;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                //SMTP Server Address of gmail
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("jacknhoxdx3@gmail.com", "jtkvzmzwikpchunx");
                // Smtp Email ID and Password For authentication
                smtp.EnableSsl = true;
                smtp.Send(mail);
                ViewBag.Message = "Your Message Send Successfully";
            }
            catch
            {
                ViewBag.Message = "Error............";
            }

            return View();
        }
        
    }
}