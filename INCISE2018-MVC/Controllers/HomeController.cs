using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INCISE2018_MVC.Managers;

namespace INCISE2018_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Programme()
        {
            return View();
        }

        public ActionResult Travel_Accommodations()
        {
            return View();
        }

        public ActionResult Venue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactConferenceGroup(FormCollection form)
        {
            string msg = form["SendMessage"];
            string userMail = User.Identity.Name;
            MailManager mail = new MailManager();
            mail.MailTitle = "Contact from Venue Page";
            mail.ReciveUserAddress = "garry@garrylabs.com";
            mail.UseHtml = true;
            mail.MailContent = "<div>You can reply by this address: " +
                "<b style=\"\">"+userMail+"</b></div><div><font __editorwarp__=\"1\" style=\"display: inline; font-size: 14px; " +
                "font-family: Verdana; color: rgb(0, 0, 0); background-color: rgba(0, 0, 0, 0); font-weight: 400; font-style: normal;\">" +
                "<hr></font></div><font __editorwarp__=\"1\" style=\"display: inline; font-size: 14px; font-family: Verdana; " +
                "color: rgb(0, 0, 0); background-color: rgba(0, 0, 0, 0); font-weight: 400; font-style: normal;\"><div style=" +
                "\"display: block; font-size: 14px; font-family: Verdana; color: rgb(0, 0, 0); background-color: rgba(0, 0, 0, 0);" +
                " font-weight: 400; font-style: normal;\">The contact text from Venue page is:</div><blockquote style=\"margin: " +
                "0.8em 0px 0.8em 2em; padding: 0px 0px 0px 0.7em; border-left: 2px solid rgb(221, 221, 221);\" formatblock=\"1\">" +
                msg +
                "</blockquote></font><div></div><div><includetail><!--<![endif]--></includetail></div>";
            if (mail.Send())
            {
                ViewBag.SendSuccessFlag = true;
            }
            return View("Venue");
        }
    }
}