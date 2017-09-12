using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using WorkstationManagementServices.Models;
using WorkstationManagementServices.Models.Database;

namespace WorkstationManagementServices.Controllers
{
    public class HomeController : Controller
    {
        private ManagementEntities db = new ManagementEntities();

        public ActionResult Index()
        {
            db.Database.Connection.Open();
            SystemInfoModel model = new SystemInfoModel();
            model.Databases = new List<DbConnectionModel>()
            {
                new DbConnectionModel {
                    Database = db.Database.Connection.Database,
                    DataSource = db.Database.Connection.DataSource,
                    ServerInfo = db.Database.Connection.ServerVersion,
                    State = db.Database.Connection.State.ToString()
                }
            };

            try {
                model.WebSites = null;//IISWebServices();
            } catch { }

            db.Database.Connection.Close();
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private IEnumerable<SiteModel> IISWebServices()
        {
            
            foreach (Microsoft.Web.Administration.Site site in new ServerManager().Sites)
                yield return new SiteModel { 
                    Name = site.Name,
                    State = site.State.ToString(),
                    PhysicalPath = site.Applications["/"].VirtualDirectories[0].PhysicalPath
                };

        }


        public ActionResult Dashboard(){
            return View(db.Project.ToArray());
        }
    }
}