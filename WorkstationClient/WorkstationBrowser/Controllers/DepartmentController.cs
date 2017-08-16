using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;

namespace WorkstationBrowser.Controllers
{
    public class DepartmentController : Controller
    {
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            var AllDepartments = wrapper.GetAllDepartments();
            return View(AllDepartments);
        }

     
    }
}
