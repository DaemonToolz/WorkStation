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

            return View(wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true));
        }

     
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

            ViewData["AllUsers"] = wrapper.GetAllUsers().ToArray();

            return View(wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true));
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
            
            wrapper.WorkstationSession.SendMessage(model);
            List<UsersModel> CurrentUsers = wrapper.WorkstationSession.GetAllUsers().ToList();
      

            ViewBag.to = new SelectList(
                CurrentUsers,
                "id", "username");

            return PartialView();
        }


    }
}
