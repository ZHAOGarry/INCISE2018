using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INCISE2018_MVC.Models;
using INCISE2018_MVC.Managers;

namespace INCISE2018_MVC.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            RegistrationManager manager = new RegistrationManager();
            RegistrationViewModel model = new RegistrationViewModel();
            if (User.Identity.IsAuthenticated)
            {
                model.IsRegisted = manager.IsRegisted(User.Identity.Name);
            }
            return View(model);
        }

        public ActionResult Result()
        {
            PermissionManager permission = new PermissionManager();
            if (!permission.IsAdmin(User.Identity.Name))
            {
                return View("Blocking");
            }

            RegistrationManager manager = new RegistrationManager();
            ResultViewModel model = new ResultViewModel();
            model.List = manager.GetResultList();
            return View(model);

        }

        public ActionResult SignUp()
        {
            RegistrationManager manager = new RegistrationManager();
            manager.DoRegist(User.Identity.Name);
            return RedirectToAction("Index");
        }

        public ActionResult Cancel()
        {
            RegistrationManager manager = new RegistrationManager();
            manager.CancelRegist(User.Identity.Name);
            return RedirectToAction("Index");
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