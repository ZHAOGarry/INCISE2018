using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INCISE2018_MVC.Controllers
{
    public class AbstractController : Controller
    {
        // GET: Abstract
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyAbstarct()
        {
            return View();
        }

        public ActionResult Console()
        {
            return View();
        }
    }
}