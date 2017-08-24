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
    public class NotificationsController : Controller {
        // GET: Notifications
        public ActionResult Index(bool unreadfirst = false, bool hideread = false) {

            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            NotificationModel[] notifications = Session["SystemNotifications"] as NotificationModel[];
           
            Session["SystemNotifications"] = notifications;
            if (unreadfirst)
                notifications.OrderBy(notif => !notif.read);
            return View(notifications);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "id,title,content,read")] NotificationModel notification, string NotificationUpdater) {

            var currentSession = Session["WorkstationConnection"] as SessionWrapper;

            if (NotificationUpdater.Equals("Read")){
                notification.read = true;
                currentSession.WorkstationSession.AcknowledgeNotification(notification, currentSession.CurrentUser.id);
            }
            else
                currentSession.WorkstationSession.DeleteNotification(notification.id, currentSession.CurrentUser.id);
            Session["SystemNotifications"] =
                currentSession.WorkstationSession.GetAllNotifications(currentSession.CurrentUser.id);

            return RedirectToAction("Index");
        }
    }
}
