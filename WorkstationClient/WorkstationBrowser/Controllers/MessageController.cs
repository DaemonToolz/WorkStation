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

            return View(_Session.MyMessages());
        }

     
        public ActionResult Index(int to = 0){
            ViewData["AllUsers"] = _Session.GetAllUsers().ToArray();
            ViewData["to"] = to;
            return View(_Session.MyMessages());
        }


        public ActionResult _Create(int to = 0){
           var selected = to != 0 ? _Session.GetAllUsers().ToList().Single(usr => usr.id == to) : _Session.GetAllUsers().ToList().First();

            ViewBag.to = new SelectList(
                _Session.GetAllUsers().ToList(),
                "id", "username",selected );

      
            return PartialView();
        }

        [HttpPost]
        public ActionResult _Create([Bind(Include = "title, content, to")] MessageModel model)
        {
     
            model.read = false;
            model.from = (int)_Session.CurrentUser.id;
            model.direct = false;
            bool result = _Session.SendMessage(model);
            ModelState.Clear();

            List<UsersModel> CurrentUsers = _Session.GetAllUsers().ToList();

            ViewBag.to = new SelectList(
                CurrentUsers,
                "id", "username");

            String content;
            content = result ? $"Your message have been sent successfully to {CurrentUsers.Single(user => user.id == model.to).username}" : "An error occured during the mailing, please retry later";

            _Session.CreateNotification($"Your message at {DateTime.Now}", content, false, model.from);
   
            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateMessage([Bind(Include = "id, from, to, read")] MessageModel model, string action)
        {

            switch (action) {
                case "read":
                    _Session.MarkAsRead(model);
                    break;
                case "delete":
                    _Session.DeleteMessage(model);
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
            _Session.MarkAsRead(_Session.MyMessages().ToArray());
            return RedirectToAction("Index");
        }


        public ActionResult DeleteAll() {
            _Session.DeleteMessage(_Session.MyMessages().ToArray());
            return RedirectToAction("Index");
        }
    }
}
