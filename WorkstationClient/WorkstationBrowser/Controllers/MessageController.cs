using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        [ChildActionOnly]
        public ActionResult _Index() {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;

            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            ViewData["AllUsers"] = wrapper.GetAllUsers().ToArray();

            return View(wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true, false,true));
        }

     
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

            ViewData["AllUsers"] = wrapper.GetAllUsers().ToArray();

            return View(wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true, false, true));
        }


        public ActionResult _Create()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
       
            List<UsersModel> CurrentUsers = wrapper.WorkstationSession.GetAllUsers().ToList();
           
            ViewBag.to = new SelectList(
                CurrentUsers,
                "id", "username");

            return PartialView();
        }

        [HttpPost]
        public ActionResult _Create([Bind(Include = "title, content, to")] MessageModel model)
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            model.read = false;
            model.from = wrapper.CurrentUser.id;
            model.direct = false;
            bool result = wrapper.WorkstationSession.SendMessage(model);
            ModelState.Clear();
            List<UsersModel> CurrentUsers = wrapper.WorkstationSession.GetAllUsers().ToList();
      

            ViewBag.to = new SelectList(
                CurrentUsers,
                "id", "username");

            String content;
            content = result ? $"Your message have been sent successfully to {CurrentUsers.Single(user => user.id == model.to).username}" : "An error occured during the mailing, please retry later";

            wrapper.WorkstationSession.CreateNotification(new NotificationModel() {
                content = content,
                title = $"Your message at {DateTime.Now}"
            }, new int[] { model.from }, false);

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateMessage([Bind(Include = "id, from, to, read")] MessageModel model, string action)
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            switch (action) {
                case "read":
                    model.read = true;
                    wrapper.WorkstationSession.MarkAsRead(model);
                    break;
                case "delete":
                    wrapper.WorkstationSession.DeleteMessage(model);
                    break;
            }

            return RedirectToAction("Index");
        }

        public void _UpdateDirectMessages(int targetid)
        {
            try
            {
                //SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
                //wrapper.UpdateMessages(targetid);

            }
            catch
            {
    
            }
        }

        public ActionResult MarkAllAsRead()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            foreach (var message in 
                wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true, false,true))
            {
                message.read = true;
                wrapper.WorkstationSession.MarkAsRead(message);
            }

            return RedirectToAction("Index");
        }


        public ActionResult DeleteAll()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            foreach (var message in
                wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true, false, true))
            {
            
                wrapper.WorkstationSession.DeleteMessage(message);
            }

            return RedirectToAction("Index");
        }
    }
}
