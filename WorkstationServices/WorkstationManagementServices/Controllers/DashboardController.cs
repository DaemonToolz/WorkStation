using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WorkstationManagementServices.Controllers.Background;
using WorkstationManagementServices.Models;
using WorkstationManagementServices.Models.Database;

namespace WorkstationManagementServices.Controllers
{
    public class DashboardController : Controller
    {

        private ManagementEntities db = new ManagementEntities();

        // GET: Dashboard
        public ActionResult DashboardProjects(int projectid)
        {
           
            List<DetailedFileMode> Detailed = new List<DetailedFileMode>();
            if(db.File.Any(rec => rec.project_id == projectid))
                foreach (var file in db.File.Where(rec => rec.project_id == projectid)){
             
                    var ChangeSetsFor = db.ChangeSet.Where(rec => rec.trackerId.Equals(file.tracker_id));

                    int totalChanges = 0, totalNew = 0, totalRem = 0;

                    foreach (var item in ChangeSetsFor){
                        totalChanges += Math.Abs(item.edition);
                        totalNew += Math.Abs(item.addition);
                        totalRem += Math.Abs(item.deletion);
                    }

                    Detailed.Add(new DetailedFileMode{
                        Filename = file.name,
                        Changes = totalChanges,
                        NewContent = totalNew,
                        RemovedContent = totalRem
                    });

                }

            // Test
            var response = JObject.Parse( SideServicesManager.CallService("Xml Comment Drone", "Comments", "GET", "AllComments",
                new Dictionary<string, object>() { { "projectName", "Workstation" } }));
            JArray files = (JArray)response["files"];

            ViewData["XmlActivity"] = files.Select(c => c).ToArray(); ;
            return View(new ProjectHeavyModel() { FileModels = Detailed.ToArray() });
        }
    }
}