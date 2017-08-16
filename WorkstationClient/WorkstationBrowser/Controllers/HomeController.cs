using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["WorkstationConnection"] == null && Request.IsAuthenticated)
                return RedirectToAction("Logout", "Login");

            if (Request.IsAuthenticated) {
                if (Session["SystemNotifications"] != null)
                {
                    NotificationModel[] notifications = Session["SystemNotifications"] as NotificationModel[];
                    ViewData["CurrentSession"] = Session["WorkstationConnection"];
                    ViewData["UnreadNotifications"] = notifications.Count(notif => !notif.read);
                    ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
                }
            }
            return View();
        }

     
    }
}