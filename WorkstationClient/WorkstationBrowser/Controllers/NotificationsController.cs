using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class NotificationsController : GenericController
    {
        // GET: Notifications
        public ActionResult Index(bool unreadfirst = false, bool hideread = false, bool hideunread = false)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var UserNotifications = _UserNotifications.OrderByDescending(notif => notif.stamp).ToArray();
            if (unreadfirst)
                UserNotifications = UserNotifications.OrderBy(notif => notif.read).ToArray();

            if (hideread) {
                var notifs = UserNotifications.ToList();
                notifs.RemoveAll(notif => notif.read);
                UserNotifications = notifs.ToArray();
            }

            if (hideunread){
                var notifs = UserNotifications.ToList();
                notifs.RemoveAll(notif => !notif.read);
                UserNotifications = notifs.ToArray();
            }

            TempData["hideread"] = hideread;
            TempData["hideunread"] = hideunread;
            TempData["unreadfirst"] = unreadfirst;

            return View(UserNotifications);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "id,title,content,read, stamp")] NotificationModel notification, string NotificationUpdater) {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            //var currentSession = Session["WorkstationConnection"] as SessionWrapper;

            if (NotificationUpdater.Equals("Read"))
                _Session.AcknowledgeNotification(notification);
            else
                _Session.DeleteNotification(notification);

            _UserNotifications = _Session.GetNotifications().ToArray();

            return RedirectToAction("Index", new{unreadfirst = (bool)TempData["unreadfirst"] , hideread = (bool)TempData["hideread"], hideunread = (bool)TempData["hideunread"] });
        }
    }
}
