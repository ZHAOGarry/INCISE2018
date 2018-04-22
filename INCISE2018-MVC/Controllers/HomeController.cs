using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            //向老师邮箱发送内容
            return RedirectToAction("Venue");
        }
    }
}