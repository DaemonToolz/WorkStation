using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;

namespace WorkstationBrowser.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        [ChildActionOnly]
        public ActionResult _Index() {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            
            return View(wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true));
        }

     
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

            return View(wrapper.WorkstationSession.GetAllMessages(wrapper.CurrentUser, false, true));
        }


    }
}
