using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class HomeController : GenericController
    {
        public ActionResult Index()
        {
            if (_Session == null && Request.IsAuthenticated)
                return RedirectToAction("Logout", "Login");
            
            return View();
        }

    }
}