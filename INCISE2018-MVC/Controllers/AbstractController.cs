using INCISE2018_MVC.Models;
using INCISE2018_MVC.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INCISE2018_MVC.Controllers
{
    public class AbstractController : Controller
    {
        private static string[] Themes = new string[] {
            "Session 1: Canyon processes in space and time (formation, evolution, circulation)",
            "Session 2: New ways to study submarine canyons: integrated programmes, new technologies and coordinated monitoring efforts",
            "Session 3: Patterns and heterogeneity in submarine canyons",
            "Session 4: Submarine canyon conservation",
            "Special Session: Canyons and trenches in the South China Sea and West Pacific"
        };

        private static string[] Pre = new string[] {
            "Poster Presentation",
            "Oral Presentation"
        };

        private AbstractManager amger;

        [HttpGet]
        public ActionResult Index()
        {
            AbstractViewModel model = new AbstractViewModel();
            amger = new AbstractManager();
            model.AbstarctList = amger.GetListOnShow();
            return View(model);
        }

        [HttpGet]
        public ActionResult MyAbstract()
        {
            ViewBag.AlertType = TempData["AlertType"];
            ViewBag.AlertContent = TempData["AlertContent"];
            MyAbstractViewModel model = GetMyAbstract();
            return View(model);
        }

        [HttpGet]
        public ActionResult Console()
        {
            ViewBag.AlertType = TempData["AlertType"];
            ViewBag.AlertContent = TempData["AlertContent"];
            AbstractManager manager = new AbstractManager();
            ConsoleViewModel model = new ConsoleViewModel();
            model.ShowList = manager.GetListOnShow();
            model.WaitList = manager.GetWaitingList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Detail(int Id)
        {
            amger = new AbstractManager();
            AbstractModel model = amger.GetItemById(Id, User.Identity.IsAuthenticated ? User.Identity.Name : " ");
            if (model == null)
            {
                return View("Blocking");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult New()
        {
            var INCISETheme = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Text = "-" },
                new SelectListItem() { Value = Themes[0], Text = Themes[0]},
                new SelectListItem() { Value = Themes[1], Text = Themes[1]},
                new SelectListItem() { Value = Themes[2], Text = Themes[2]},
                new SelectListItem() { Value = Themes[3], Text = Themes[3]},
                new SelectListItem() { Value = Themes[4] ,Text = Themes[4]}
            };
            ViewBag.ThemeOptions = INCISETheme;

            var PreType = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Text = "-" },
                new SelectListItem() { Value = Pre[0], Text = Pre[0]},
                new SelectListItem() { Value = Pre[1], Text = Pre[1] }
            };
            ViewBag.PreOptions = PreType;
            return View();
        }

        [HttpPost]
        public ActionResult New(AbstractModel model)
        {
            AbstractManager manager = new AbstractManager();
            if (manager.NewAbstract(model, User.Identity.Name))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "A new abstract has been created.";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }
            return RedirectToAction("MyAbstract");
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            AbstractManager manager = new AbstractManager();
            var model = manager.GetItemById(Id, User.Identity.Name);
            var INCISETheme = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Text = "-" },
                new SelectListItem() { Value = Themes[0], Text = Themes[0]},
                new SelectListItem() { Value = Themes[1], Text = Themes[1]},
                new SelectListItem() { Value = Themes[2], Text = Themes[2]},
                new SelectListItem() { Value = Themes[3], Text = Themes[3]},
                new SelectListItem() { Value = Themes[4] ,Text = Themes[4]}
            };
            ViewBag.ThemeOptions = INCISETheme;

            var PreType = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Text = "-" },
                new SelectListItem() { Value = Pre[0], Text = Pre[0]},
                new SelectListItem() { Value = Pre[1], Text = Pre[1] }
            };
            ViewBag.PreOptions = PreType;

            List<EditBrief> edit = manager.GetPreviousVersions(model.GroupId);
            ViewBag.List = edit;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AbstractModel model)
        {
            AbstractManager manager = new AbstractManager();
            if (manager.Edit(model))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "Edit successfully.";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }
            return RedirectToAction("MyAbstract");
        }

        public ActionResult Submit(int Id)
        {
            AbstractManager manager = new AbstractManager();
            if (manager.Submit(Id))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "Abstract(ID:" + Id + ") has been submitted.";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }

            MailBrief brief = manager.GetMailBrief(Id);
            MailManager mail = new MailManager();
            string html = "<h2>The abstract you submitted has been <strong>submitted</strong>.</h2><hr />" +
                "Title:"+brief.Title+"<br />" +
                "Id:"+brief.GroupId+"-"+brief.Id+"<br />" +
                "Creation Time:"+brief.SubmitTime + "<br />" +
                "Operation Time:"+DateTime.Now +"<hr />" +
                "Any requests please get in touch:<a href=\"mailto: incise2018 @sustc.edu.cn\">incise2018@sustc.edu.cn</a>";
            mail.ReciveUserAddress = brief.MailAddress;
            mail.MailTitle = "Abstract‘s Status Update";
            mail.MailContent = html;
            mail.UseHtml = true;
            mail.Send();
            return RedirectToAction("MyAbstract");
        }

        public ActionResult Pass(int Id,int GroupId)
        {
            if (!PermissionCheck())
            {
                return View("Blocking");
            }

            AbstractManager manager = new AbstractManager();
            if (manager.Pass(Id, GroupId))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "Set Abstract ("+GroupId+"-"+Id+ ") Statement to [Approved].";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }

            MailBrief brief = manager.GetMailBrief(Id);
            MailManager mail = new MailManager();
            string html = "<h2>The abstract you submitted has been <strong>validated</strong>.</h2><hr />" +
                "Title:" + brief.Title + "<br />" +
                "Id:" + brief.GroupId + "-" + brief.Id + "<br />" +
                "Creation Time:" + brief.SubmitTime + "<br />" +
                "Operation Time:" + DateTime.Now + "<hr />" +
                "Any requests please get in touch:<a href=\"mailto: incise2018 @sustc.edu.cn\">incise2018@sustc.edu.cn</a>";
            mail.ReciveUserAddress = brief.MailAddress;
            mail.MailTitle = "Abstract‘s Status Update";
            mail.MailContent = html;
            mail.UseHtml = true;
            mail.Send();
            return RedirectToAction("Console");
        }

        public ActionResult Reject(int Id,int GroupId)
        {
            if (!PermissionCheck())
            {
                return View("Blocking");
            }

            AbstractManager manager = new AbstractManager();
            if (manager.Reject(Id))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "Set Abstract (" + GroupId + "-" + Id + ") Statement to [Fail].";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }

            MailBrief brief = manager.GetMailBrief(Id);
            MailManager mail = new MailManager();
            string html = "<h2>The abstract you submitted has been <strong>rejected</strong>.</h2>" +
                "<p>Please revise and resubmit.</p><hr />" +
                "Title:" + brief.Title + "<br />" +
                "Id:" + brief.GroupId + "-" + brief.Id + "<br />" +
                "Creation Time:" + brief.SubmitTime + "<br />" +
                "Operation Time:" + DateTime.Now + "<hr />" +
                "Any requests please get in touch:<a href=\"mailto: incise2018 @sustc.edu.cn\">incise2018@sustc.edu.cn</a>";
            mail.ReciveUserAddress = brief.MailAddress;
            mail.MailTitle = "Abstract‘s Status Update";
            mail.MailContent = html;
            mail.UseHtml = true;
            mail.Send();
            return RedirectToAction("Console");
        }

        public ActionResult Delete(int Id,int GroupId)
        {
            if (!PermissionCheck())
            {
                return View("Blocking");
            }

            AbstractManager manager = new AbstractManager();
            if (manager.Delete(Id))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "Set Abstract (" + GroupId + "-" + Id + ") Statement to [Fail].";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }

            MailBrief brief = manager.GetMailBrief(Id);
            MailManager mail = new MailManager();
            string html = "<h2>The abstract you submitted has been <strong>withdraw by administrator</strong>.</h2>" +
                 "<p>Please revise and resubmit or contact us via email..</p><hr />" +
                "Title:" + brief.Title + "<br />" +
                "Id:" + brief.GroupId + "-" + brief.Id + "<br />" +
                "Creation Time:" + brief.SubmitTime + "<br />" +
                "Operation Time:" + DateTime.Now + "<hr />" +
                "Any requests please get in touch:<a href=\"mailto: incise2018 @sustc.edu.cn\">incise2018@sustc.edu.cn</a>";
            mail.ReciveUserAddress = brief.MailAddress;
            mail.MailTitle = "Abstract‘s Status Update";
            mail.MailContent = html;
            mail.UseHtml = true;
            mail.Send();
            return RedirectToAction("Console");

        }

        private bool PermissionCheck()
        {
            PermissionManager manager = new PermissionManager();
            if (manager.IsAdmin(User.Identity.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Withdraw(int Id)
        {
            AbstractManager manager = new AbstractManager();
            if (manager.Withdraw(Id))
            {
                TempData["AlertType"] = "SUCCESS";
                TempData["AlertContent"] = "Your abstract has been successfully withdrawn.";
            }
            else
            {
                TempData["AlertType"] = "DANGER";
                TempData["AlertContent"] = "Oops! Something Wrong! Please Try Again!";
            }
            MailBrief brief = manager.GetMailBrief(Id);
            MailManager mail = new MailManager();
            string html = "<h2>The abstract you submitted has been <strong>withdraw by yourself</strong>.</h2><hr />" +
                "Title:" + brief.Title + "<br />" +
                "Id:" + brief.GroupId + "-" + brief.Id + "<br />" +
                "Creation Time:" + brief.SubmitTime + "<br />" +
                "Operation Time:" + DateTime.Now + "<hr />" +
                "Operation IP Address:" + Request.UserHostAddress + "<hr />"+
                "Any requests please get in touch:<a href=\"mailto: incise2018 @sustc.edu.cn\">incise2018@sustc.edu.cn</a>";
            mail.ReciveUserAddress = brief.MailAddress;
            mail.MailTitle = "Abstract‘s Status Update";
            mail.MailContent = html;
            mail.UseHtml = true;
            mail.Send();
            return View("MyAbstract", GetMyAbstract());
        }

        private MyAbstractViewModel GetMyAbstract()
        {
            MyAbstractViewModel model = new MyAbstractViewModel();
            model.AbstractDictionary = new Dictionary<AbstractModel, List<AbstractModel>>();
            AbstractManager manager = new AbstractManager();
            List<AbstractModel> list = manager.GetMyList(User.Identity.Name);
            int i = int.MaxValue;
            AbstractModel temp = null;
            foreach (AbstractModel item in list)
            {
                if (i > item.GroupId)
                {
                    model.AbstractDictionary.Add(item, new List<AbstractModel>());
                    temp = item;
                    i = item.GroupId;
                }
                else
                {
                    model.AbstractDictionary[temp].Add(item);
                }
            }
            return model;
        }
    }
}