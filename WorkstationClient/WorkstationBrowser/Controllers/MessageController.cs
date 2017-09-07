using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class MessageController : GenericController
    {
        // GET: Message
        [ChildActionOnly]
        public ActionResult _Index() {
            if (!Request.IsAuthenticated)
                return View(new MessageModel[]{});

            ViewData["AllUsers"] = _Session.GetAllUsers().ToArray();

            return View(_Session.MyMessages());
        }

     
        public ActionResult Index(params int[] to){
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["AllUsers"] = _Session.GetAllUsers().ToArray();

            if (to == null || !to.Any())
                to = new int[] {0};
            ViewData["to"] = to;
            return View(_Session.MyMessages());
        }

        public ActionResult Str_Index(string to)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

           
            ViewData["AllUsers"] = _Session.GetAllUsers().ToArray();
            int[] result;
            if (to == null || !to.Any())
                result = new int[] { 0 };
             else
                result = System.Web.Helpers.Json.Decode<int[]>(to);
            ViewData["to"] = result;
            return View("Index",_Session.MyMessages());
        }

        public ActionResult _Create(params int[] to){
            if (!Request.IsAuthenticated)
                return PartialView();

    
            var selected = (to != null && to.Any()) ? 
                _Session.GetAllUsers().ToList().Where(usr => to.Contains((int)(usr.id))) :
                new List<UsersModel>(){_Session.GetAllUsers().ToList().First()};

            ViewBag.to = new MultiSelectList(
                _Session.GetAllUsers().ToList(),
                "id", "username",selected);

      
            return PartialView();
        }

        [HttpPost]
        public ActionResult _Create([Bind(Include = "title, content, to")] MessageHeavyModel heavyModel)
        {
            if (!Request.IsAuthenticated)
                return PartialView();

            List<UsersModel> CurrentUsers = _Session.GetAllUsers().ToList();


            foreach (var to_id in heavyModel.to)
            {
                MessageModel model = new MessageModel
                {
                    read = false,
                    @from = (int) _Session.CurrentUser.id,
                    direct = false,
                    title = heavyModel.title,
                    content = heavyModel.content,
                    to = to_id
                };
                bool result = _Session.SendMessage(model);
                ModelState.Clear();

               
                //
                String content;
                content = result
                    ? $"Your message have been sent successfully to {CurrentUsers.Single(user => user.id == model.to).username}"
                    : "An error occured during the mailing, please retry later";

                _Session.CreateNotification($"Your message at {DateTime.Now}", content, false, model.from);
            }

            ViewBag.to = new MultiSelectList(
                CurrentUsers,
                "id", "username");

            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateMessage([Bind(Include = "id, from, to, read, stamp")] MessageModel model, string action)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

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
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            _Session.MarkAsRead(_Session.MyMessages().ToArray());
            return RedirectToAction("Index");
        }


        public ActionResult DeleteAll() {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            _Session.DeleteMessage(_Session.MyMessages().ToArray());
            return RedirectToAction("Index");
        }
    }
}
