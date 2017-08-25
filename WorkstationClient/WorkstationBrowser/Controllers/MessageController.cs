using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class MessageController : GenericController
    {
        // GET: Message
        [ChildActionOnly]
        public ActionResult _Index() {
            ViewData["AllUsers"] = _Session.GetAllUsers().ToArray();
            return View(_Session.WorkstationSession.GetAllMessages(_Session.CurrentUser, false, true, false,true));
        }

     
        public ActionResult Index()
        {
            ViewData["AllUsers"] = _Session.GetAllUsers().ToArray();
            return View(_Session.WorkstationSession.GetAllMessages(_Session.CurrentUser, false, true, false, true));
        }


        public ActionResult _Create(){
            ViewBag.to = new SelectList(
                _Session.GetAllUsers().ToList(),
                "id", "username");

            return PartialView();
        }

        [HttpPost]
        public ActionResult _Create([Bind(Include = "title, content, to")] MessageModel model)
        {
     
            model.read = false;
            model.from = _Session.CurrentUser.id;
            model.direct = false;
            bool result = _Session.WorkstationSession.SendMessage(model);
            ModelState.Clear();

            List<UsersModel> CurrentUsers = _Session.GetAllUsers().ToList();

            ViewBag.to = new SelectList(
                CurrentUsers,
                "id", "username");

            String content;
            content = result ? $"Your message have been sent successfully to {CurrentUsers.Single(user => user.id == model.to).username}" : "An error occured during the mailing, please retry later";

            _Session.WorkstationSession.CreateNotification(new NotificationModel() {
                content = content,
                title = $"Your message at {DateTime.Now}"
            }, new int[] { model.from }, false);

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateMessage([Bind(Include = "id, from, to, read")] MessageModel model, string action)
        {

            switch (action) {
                case "read":
                    model.read = true;
                    _Session.WorkstationSession.MarkAsRead(model);
                    break;
                case "delete":
                    _Session.WorkstationSession.DeleteMessage(model);
                    break;
            }

            return RedirectToAction("Index");
        }

        public void _UpdateDirectMessages(int targetid)
        {
            try
            {

            }
            catch
            {
    
            }
        }

        public ActionResult MarkAllAsRead()
        {

            foreach (var message in
                _Session.WorkstationSession.GetAllMessages(_Session.CurrentUser, false, true, false,true))
            {
                message.read = true;
                _Session.WorkstationSession.MarkAsRead(message);
            }

            return RedirectToAction("Index");
        }


        public ActionResult DeleteAll()
        {

            foreach (var message in
                _Session.WorkstationSession.GetAllMessages(_Session.CurrentUser, false, true, false, true))
            {

                _Session.WorkstationSession.DeleteMessage(message);
            }

            return RedirectToAction("Index");
        }
    }
}
