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

            var ProjectName = db.Project.Single(proj => proj.id.Equals(projectid)).name;

            // Test
            var response = JObject.Parse( SideServicesManager.CallService("Xml Comment Drone", "Comments", "GET", "AllComments",
                new Dictionary<string, object>() { { "projectName", ProjectName } }));

            JArray files = response["files"].HasValues ? (JArray)response["files"] : null;

            ViewData["XmlActivity"] = files?.ToArray();

            var dict = new Dictionary<string, Dictionary<string, int>>();

            foreach (var item in db.File)
            {
                if (!db.ChangeSet.Any(rec => rec.trackerId.Equals(item.tracker_id)))
                    continue;
                var tracks = db.ChangeSet.Where(rec => rec.trackerId.Equals(item.tracker_id)).ToList();
                var temp = new Dictionary<string, int>();

                foreach (var activity in tracks)
                {
                    if (!temp.ContainsKey(activity.stamp.ToShortDateString()))
                        temp.Add(activity.stamp.ToShortDateString(), 1);
                    else
                        temp[activity.stamp.ToShortDateString()]++;
                }

                dict.Add(item.name, temp);
            }

            ViewData["CSActivity"] = dict;

            //var result = MergeModel(files?.ToArray());
            
            return View(new ProjectHeavyModel() { FileModels = Detailed.ToArray() });
        }

        private List<DashboardItem> MergeModel(JToken[] jsoned)
        {
            throw new NotImplementedException();
            //files > [Name, Root > Comments > [Content > [Author, Date,Content]]
            var items = new List<DashboardItem>();

            foreach (var item in jsoned){
                var name = item["Name"].ToString();
                var dItem = new DashboardItem(){
                    Name = name.Remove(name.LastIndexOf("/") + 1).Replace("_comtrack", "").Replace(".xml",""),
                    Parent = name.Substring(0, name.LastIndexOf("/") + 1),
                    Comments = item["Root"]["Comments"][0]["Content"].ToObject<CommentModel[]>(),
                };

                var changes = new List<ChangeSet>();
                try {
                    var relatedFile = db.File.Single(rec => rec.name.Equals(dItem.Absolute));
                    changes = db.ChangeSet.Where(rec => rec.trackerId.Equals(relatedFile.tracker_id)).ToList();
                } catch { }

                dItem.Changes = changes.ToArray();
                items.Add(dItem);
            }

            foreach (var uncommented in db.File.Where(rec => items.Select(it => it.Absolute).Contains(rec.name)))
            {
                var name = uncommented.name;
                var dItem = new DashboardItem()
                {
                    Name = name.Remove(name.LastIndexOf("/") + 1).Replace("_comtrack", "").Replace(".xml", ""),
                    Parent = name.Substring(0, name.LastIndexOf("/") + 1),
                    Comments = new List<CommentModel>().ToArray()
                };

                var changes = new List<ChangeSet>();
                try {
                    changes = db.ChangeSet.Where(rec => rec.trackerId.Equals(uncommented.tracker_id)).ToList();
                } catch { }

                dItem.Changes = changes.ToArray();
                items.Add(dItem);
            }

            return items;
        }
    }
}