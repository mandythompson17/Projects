using Microsoft.AspNet.Identity;
using Owin.Security.Providers.OpenID.Infrastructure;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //GET: Contact Page
        public ActionResult Contact()
        {
            return View();
        }

        //POST: Contact Form
        [HttpPost]
        public ActionResult Contact(string contactname, string email, string subject, string comments)
        {
            IdentityMessage message = new IdentityMessage();
            EmailService emails = new EmailService();

            message.Subject = subject;
            message.Destination = ConfigurationManager.AppSettings["ContactEmail"];
            message.Body = "From: " + contactname + "\nEmail: " + email + "\n" + comments;
            emails.SendAsync(message);
            return RedirectToAction("Index");
        }


        public ActionResult Portfolio()
        {
            return View();
        }
    }
}