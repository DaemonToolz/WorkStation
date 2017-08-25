﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.Models;

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
           
            var MyTeam = _Session.GetAllTeams().First(team => team.id == _Session.CurrentUser.team_id);
      
            ViewData["MyProject"] = AllProjects.First(project => project.id == MyTeam.project_id);
        
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
                SessionWrapper currentSession = Session["WorkstationConnection"] as SessionWrapper;
                var CurrentProject = currentSession.WorkstationSession.GetProject(id);
                string pic = CurrentProject.name.Replace(" ", String.Empty) + "_ico" + Path.GetExtension(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/UserContent/Project/"), pic);
                // file is uploaded
                file.SaveAs(path);
                CurrentProject.projpic= pic;
                currentSession.WorkstationSession.EditProject(CurrentProject);
            }
            // after successfully uploading redirect the user
            return RedirectToAction("Details", new { id = id });
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
