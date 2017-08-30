using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class ProjectController : GenericController
    {
        // GET: Project
        public ActionResult Index()
        {
     
            ViewData["MyId"] = _Session.CurrentUser.id;
            ViewData["MyTeam"] = _Session.CurrentUser.team_id;
            var AllProjects = _Session.GetAllProjects();
            var MyTeam = _Session.GetTeamById(_Session.CurrentUser.team_id ?? 0);
            ViewData["MyProject"] = _Session.GetProject(MyTeam.project_id ?? 0);


            return View(_Session.GetAllProjects());
        }

        // GET: Project/Details/5
        public ActionResult Details(int id){
            
            var currentProject = _Session.WorkstationSession.GetProject((long) id);
            ViewData["CurrentProject"] = currentProject;
            return View(currentProject);
        }


        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file, int id)
        {

            if (file != null)
            {
              
                var CurrentProject = _Session.GetProject(id);
                string pic = CurrentProject.name.Replace(" ", String.Empty) + "_ico" + Path.GetExtension(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/UserContent/Project/"), pic);
                // file is uploaded

                var files = Directory.GetFiles(Server.MapPath("~/UserContent/Project/"), CurrentProject.name.Replace(" ", String.Empty) + "_ico.*");
                foreach (var tmpFile in files)
                    System.IO.File.Delete(tmpFile);
                

                file.SaveAs(path);
                CurrentProject.projpic = pic;
                _Session.EditProject(CurrentProject);
            }
            // after successfully uploading redirect the user
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="name")]ProjectModel model){
            model.root = $@"C:\inetpub\ftproot\{model.name}\";
            model.projpic = "Default_Project.png";
            _Session.CreateProject(model);
            
            /* AJAX CALL
            if (_Session.CreateProject(model))
                return RedirectToAction("Index", "Project"); 
            */
            return PartialView(model);
        }


        public ActionResult ProjectDocuments(String root)
        {
            ViewData["ProjectRoot"] = root;
            List<DocumentModel> files = new List<DocumentModel>();
            foreach (string file in Directory.EnumerateFiles(root))
            {
                FileInfo fileinfo = new FileInfo(file);
                files.Add(new DocumentModel()
                {
                    Name = fileinfo.Name,
                    Path = fileinfo.FullName,
                    Extension = fileinfo.Extension
                });
        
            }
            return View(files);
        }

        public ActionResult _DocumentCreator()
        {
   
            return View();
        }

        public ActionResult _CreateDocument(String title, String content)
        {
     
            return RedirectToAction("_DocumentCreator");
        }

        [HttpPost]
        public ActionResult _FileContent(String path) {
            //String[] filecontent = 
            ViewData["Path"] = path;
            return PartialView();

        }
      
        public FileResult Download(String path) {
            FileInfo file = new FileInfo(path);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = file.Name;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }



        public ActionResult DeleteFile(String path, String root){
            System.IO.File.Delete(path);
            return RedirectToAction("ProjectDocuments", "Project", new {root = root});
        }

    }
}
