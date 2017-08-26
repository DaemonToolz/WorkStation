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

            if (unreadfirst)
                _UserNotifications.OrderBy(notif => !notif.read);
            return View(_UserNotifications);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "id,title,content,read")] NotificationModel notification, string NotificationUpdater) {

            //var currentSession = Session["WorkstationConnection"] as SessionWrapper;

            if (NotificationUpdater.Equals("Read"))
                _Session.AcknowledgeNotification(notification);
            else
                _Session.DeleteNotification(notification);

            _UserNotifications = _Session.GetNotifications().ToArray();

            return RedirectToAction("Index");
        }
    }
}
