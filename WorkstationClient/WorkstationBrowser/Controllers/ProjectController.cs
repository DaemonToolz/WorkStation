using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class ProjectController : GenericController
    {
        private static Regex TitleVerifier = new Regex("[^a-zA-Z0-9]");

        // GET: Project
        public ActionResult Index(int limit = 25, int offset = 0)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            IEnumerable<ProjectModel> projects = _Session.GetAllProjects();
            if (limit > 0) {
                if (offset >= 0) {
                    ViewData["Offsets"] = projects.Count() / limit;
                    projects = projects.Skip(limit * offset);
                    ViewData["CurrentOffset"] = offset;
                }
                projects = projects.Take(limit);

            }

            ViewData["MyId"] = _Session.CurrentUser.id;
            ViewData["MyTeam"] = _Session.CurrentUser.team_id;
            var AllProjects = _Session.GetAllProjects();
            var MyTeam = _Session.GetTeamById(_Session.CurrentUser.team_id ?? 0);
            ViewData["MyProject"] = _Session.GetProject(MyTeam.project_id ?? 0);


            return View(projects);
        }

        // GET: Project/Details/5
        public ActionResult Details(int id){
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var currentProject = _Session.WorkstationSession.GetProject((long) id);
            ViewData["CurrentProject"] = currentProject;
            if (currentProject.admin_id != null)
            {
                ViewData["Manager"] = _Session.GetUserById((int)_Session.GetProject(id).admin_id);
            }

            if (_Session.GetAllTeams().Any(team => team.project_id == currentProject.id))
            {
                ViewData["Team"] = _Session.GetAllTeams().Where(team => team.project_id == currentProject.id).ToArray();
            }
            
            return View(currentProject);
        }


        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file, int id)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

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
        public ActionResult Create([Bind(Include="name, precedence")]ProjectModel model){
            model.root = $@"{Server.MapPath("~/")}\UserContent\ProjectFiles\{model.name}\";
            model.projpic = "Default_Project.png";
            String fileTracker = $@"{Server.MapPath("~/")}\UserContent\FileTracker\{model.name}\";

            if (_Session.CreateProject(model))
            {
                if (!Directory.Exists(model.root))
                    Directory.CreateDirectory(model.root);

                if (!Directory.Exists((fileTracker) + @"\Comments\"))
                    Directory.CreateDirectory(fileTracker + @"\Comments\");

                if (!Directory.Exists((fileTracker) + @"\Changes\"))
                    Directory.CreateDirectory(fileTracker + @"\Changes\");
            }

            return PartialView(model);
        }


        public ActionResult ProjectDocuments(String project, int projectid)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");


            string root =  $@"{Server.MapPath("~/")}\UserContent\ProjectFiles\{project}\";
            ViewData["ProjectRoot"] = root;
            if (project.Contains("/"))
            {
                ViewData["RootName"] = project.Substring(0, project.LastIndexOf("/"));
                ViewData["RealRoot"] = project.Substring(0, project.IndexOf("/"));
            }

            ViewData["Projectid"] = projectid;
            TempData["FileUploadRoot"] = root;
            List<DocumentModel> files = new List<DocumentModel>();

            foreach (var dir in Directory.GetDirectories(root))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                files.Add(new DocumentModel()
                {
                    Name = dirInfo.Name,
                    Path = dirInfo.FullName,
                    Extension = dirInfo.Extension,
                    Parent = dirInfo.Parent.FullName,
                    Directory = true
                });
            }

            foreach (string file in Directory.EnumerateFiles(root))
            {
                FileInfo fileinfo = new FileInfo(file);
                files.Add(new DocumentModel()
                {
                    Name = fileinfo.Name,
                    Path = fileinfo.FullName,
                    Extension = fileinfo.Extension,
                    Parent = "",
                    Directory = false
                });        
            }

            ViewData["ProjectName"] = project;
            return View(files);
        }

        public ActionResult _DocumentCreator(String project, int projectid, String filename, String extension, String filepath) {
            TempData["Project"] = project;
            TempData["Projectid"] = projectid;

            TempData["extension"] = extension;
            TempData["filename"] = filename;
            TempData["filepath"] = filepath;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Ugly way, should be replaced by a built-in engine sending XML and or JSON
        public ActionResult _DocumentCreator(String title, String extension, String content, String project, int projectid)
        {
            /*
            if (!TitleVerifier.IsMatch(title)){
                TempData["Project"] = project;
                TempData["Projectid"] = projectid;

                return View();
            }*/
            //String projectPath = $@"C:\inetpub\ftproot\{(String) TempData["Project"]}\";
            System.IO.File.WriteAllText($@"{Server.MapPath("~/")}\UserContent\ProjectFiles\{project}\{title}.{extension}", content);
        
            return RedirectToAction("ProjectDocuments", new{project = project, projectid = projectid });
        }

        [HttpPost]
        public ActionResult _FileContent(String path) {
            //String[] filecontent = 
            ViewData["Path"] = path;
            return PartialView();

        }
      
        public FileResult Download(String path) {
            if (!Request.IsAuthenticated)
                return null;

            FileInfo file = new FileInfo(path);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = file.Name;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DownloadAsZip(String project){
            if (!Request.IsAuthenticated || project == null || project.Trim() == "")
                return null;

            string root = $@"{Server.MapPath("~/")}\UserContent\ProjectFiles\{project}\";
            var memoryStream = new MemoryStream();

            using (ZipFile zip = new ZipFile()){
                zip.AddFiles(Directory.GetFiles(root), project);
                zip.Save(memoryStream);
            }
            memoryStream.Seek(0, 0);

            return File(memoryStream, "application/zip", $"{project}.zip");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void UploadFiles(IEnumerable<HttpPostedFileBase> files, String directoryPath){
            if (files != null){
                foreach (var file in files)
                {
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        // extract only the fielname
                        var fileName = Path.GetFileName(file.FileName);
                        // TODO: need to define destination
                        var path = Path.Combine(directoryPath, fileName);
                        file.SaveAs(path);
                    }
                }
            }
        }

        public ActionResult DeleteFile(String path, String project, int id, bool isDir){
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (isDir)
                Directory.Delete(path);
            else 
                System.IO.File.Delete(path);
            return RedirectToAction("ProjectDocuments", "Project", new {project = project, projectid = id});
        }


        public ActionResult Edit(long id)
        {
            var admins = _Session.GetAllUsers().ToList();
            admins.Add(new UsersModel() {id = 0, username = "Not Affected"});
            ViewBag.admin_id = new SelectList(admins, "id", "username");

            return View(_Session.GetProject(id));
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, precedence, admin_id")] ProjectModel project)
        {


            var totalProject = _Session.GetProject(project.id);
            totalProject.precedence = project.precedence;
            totalProject.admin_id = project.admin_id == 0 ? null : project.admin_id;
            var oldManagerId = totalProject.admin_id;
            if (_Session.EditProject(totalProject)){

                if (oldManagerId != null)
                {
                    var oldUser = _Session.GetUserById((int)oldManagerId);
                    _Session.CreateNotification($"{totalProject.name} / Project Management",
                        $"{oldUser.username} is no longer manager of the project {totalProject.name}",
                        false, _Session.GetUsersByProject(project.id).Select(usr => (int)usr.id).ToArray());
                }

                if (project.admin_id != null)
                {
                    var newUser = _Session.GetUserById((int)project.admin_id);

                    _Session.CreateNotification($"{totalProject.name} / Project Management",
                        $"{newUser.username} is now in charge of the project {totalProject.name}",
                        false, _Session.GetUsersByProject(project.id).Select(usr => (int)usr.id).ToArray());
                }


            }

            return RedirectToAction("Details", "Project", new { id = totalProject.id});
        }

        [HttpPost]
        public ActionResult Delete(long id) {
            _Session.DeleteProject(id);
            return RedirectToAction("Index");
        }
    }
}
