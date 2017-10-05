using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
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
        public ActionResult DashboardProjects(string parent, int projectid){
            var ProjectName = db.Project.Single(proj => proj.id.Equals(projectid)).name;
           
            // TODO Update the entire part
            var response = JObject.Parse( SideServicesManager.CallService("Xml Comment Drone", "Comments", "GET", "AllComments",
                new Dictionary<string, object>() { { "projectName", ProjectName } }));

            JArray files = response["files"].HasValues ? (JArray)response["files"] : null;
            ViewData["Project"] = projectid;
            //
            var root = parent.Substring(0, parent.LastIndexOf("/"));
            if(root.Any())
                ViewData["Root"] = root.Substring(0, root.LastIndexOf("/")+1);
            //
            return View(MergeModel(files?.ToArray(), parent));
        }

        private List<DashboardItem> MergeModel(JToken[] jsoned, String parent){
            //throw new NotImplementedException();
            //files > [Name, Root > Comments > [Content > [Author, Date,Content]]
            var items = new List<DashboardItem>();
            var childFolders = new List<string>();

            foreach (var item in jsoned) {
               
                var name = item.Value<String>("Name").ToString();
                var root = item.Value<JToken>("Root");
                var comm = root.Value<JArray>("Comments");
                var cont = new List<CommentModel>();

                if (comm != null)
                    cont = JsonConvert.DeserializeObject<List<CommentModel>>(
                        comm[0].Value<JArray>("Content").ToString());

                try
                {
                    if ((name.Contains(parent) && name.Remove(0, parent.LastIndexOf("/") + 1).Contains("/")))
                    {
                        var childFolder = parent + name.Remove(0, parent.LastIndexOf("/")).Substring(1, name.IndexOf("/", 1));
                        if(!childFolders.Contains(childFolder))
                            childFolders.Add(childFolder);
                        continue;
                    }

                    if (!name.Contains(parent))
                        continue;
                }
                catch
                {
                    continue;
                }

                var dItem = new DashboardItem(){
                    Name = name.Remove(0, name.LastIndexOf("/") + 1).Replace("_comtrack", "").Replace(".xml",""),
                    Parent = name.Substring(0, name.LastIndexOf("/") + 1),
                    Comments = cont,
                };

                var changes = new List<ChangeSet>();
                try {
                    var relatedFile = db.File.Single(rec => rec.name.EndsWith(dItem.Absolute));
                    changes = db.ChangeSet.Where(rec => rec.trackerId.Equals(relatedFile.tracker_id)).ToList();
                } catch { }

                dItem.Changes = changes.ToArray();
                items.Add(dItem);
            }

            var searchList = db.File.ToList();
            if (items.Any())
                searchList = searchList.Where(rec => !items.Any(it => it.Absolute.Equals(rec.name.Remove(0, rec.name.IndexOf("/"))))).ToList();

            foreach (var uncommented in searchList)
            {
                var name = uncommented.name.Remove(0, uncommented.name.IndexOf("/"));

                try
                {
                    if ((name.Contains(parent) && name.Remove(0, parent.LastIndexOf("/") + 1).Contains("/"))) { 
                        var childFolder = parent + name.Remove(0, parent.LastIndexOf("/")).Substring(1, name.IndexOf("/", 1));
                        if (!childFolders.Contains(childFolder))
                            childFolders.Add(childFolder);
                        continue;
                    }

                    if (!name.Contains(parent))
                        continue;
                }
                catch
                {
                    continue;
                }

                var dItem = new DashboardItem()
                {
                    Name = name.Remove(0, name.LastIndexOf("/") + 1),
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
            ViewData["Folders"] = childFolders;
            return items;
        }
    }
}