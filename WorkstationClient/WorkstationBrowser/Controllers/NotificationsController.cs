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
        public ActionResult Index(bool unreadfirst = false, bool hideread = false) {

            
            NotificationModel[] notifications = Session["SystemNotifications"] as NotificationModel[];
           
            Session["SystemNotifications"] = notifications;
            if (unreadfirst)
                notifications.OrderBy(notif => !notif.read);
            return View(notifications);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "id,title,content,read")] NotificationModel notification, string NotificationUpdater) {

            //var currentSession = Session["WorkstationConnection"] as SessionWrapper;

            if (NotificationUpdater.Equals("Read")){
                notification.read = true;
                _Session.WorkstationSession.AcknowledgeNotification(notification, _Session.CurrentUser.id);
            }
            else
                _Session.WorkstationSession.DeleteNotification(notification.id, _Session.CurrentUser.id);
            Session["SystemNotifications"] =
                _Session.WorkstationSession.GetAllNotifications(_Session.CurrentUser.id);

            return RedirectToAction("Index");
        }
    }
}
