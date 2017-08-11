using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;

namespace WorkstationBrowser.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        public ActionResult Index()
        {

            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            NotificationModel[] notifications = Session["SystemNotifications"] as NotificationModel[];
            notifications.ToList().ForEach(notif => notif.Read = true);
            Session["SystemNotifications"] = notifications;

            return View(notifications);
        }
        
    }
}
