using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    [Authorize]
    public class HomeController : GenericController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (_Session == null && Request.IsAuthenticated)
                return RedirectToAction("Logout", "Login");

            return View();
        }

        // Temporary Solution
        [AllowAnonymous]
        [HttpGet]
        public JsonResult RandomTest()
        {
            dynamic obj = new { id=10, str = ""};
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult AngularTest(){
            return View();
        }
    }
}