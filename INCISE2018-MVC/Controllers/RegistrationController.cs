using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INCISE2018_MVC.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Foreign()
        {
            return View();
        }

        public ActionResult Domestic()
        {
            return View();
        }
    }
}