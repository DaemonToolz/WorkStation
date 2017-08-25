using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;

namespace WorkstationBrowser.Controllers
{
    public class DepartmentController : GenericController
    {
        public ActionResult Index()
        {
         
            return View(_Session.GetAllDepartments());
        }

     
    }
}
