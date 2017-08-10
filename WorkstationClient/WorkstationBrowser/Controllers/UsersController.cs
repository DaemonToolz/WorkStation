using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            
            return View(wrapper.GetAllUsers());
        }

        public ActionResult MyProfile(){
            SessionWrapper currentSession = Session["WorkstationConnection"] as SessionWrapper;
            return View(currentSession.CurrentUser);
        }
    }
}
