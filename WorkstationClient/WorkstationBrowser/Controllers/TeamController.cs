using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;

namespace WorkstationBrowser.Controllers
{
    public class TeamController : GenericController
    {
        // GET: Team
        public ActionResult Index()
        {
    
            ViewData["TeamId"] = _Session.CurrentUser.team_id;
            ViewData["AllDepartments"] = _Session.GetAllDepartments().ToArray();
            ViewData["AllProjects"] = _Session.GetAllProjects().ToArray();

            return View(_Session.GetAllTeams());
        }

        // GET: Team/Details/5
        public ActionResult Details(int id){
            
            var currentTeam = _Session.GetAllTeams().First(team => team.id == id);
            
            ViewBag.id = id;
            ViewData["Department"] = _Session.GetAllDepartments()
                .Single(dept => dept.id == currentTeam.department_id);
            if(currentTeam.project_id != null)
                ViewData["Project"] = _Session.GetProject((long)currentTeam.project_id);

            ViewData["ActiveMembers"] = _Session.GetAllUsers().Where(user => user.team_id == id).ToArray();
            return View(currentTeam);
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file, int id)
        {

            if (file != null)
            {
               
                var CurrentTeam = _Session.GetTeamById(id);
                string pic = CurrentTeam.name.Replace(" ", String.Empty) + "_ico" + Path.GetExtension(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/UserContent/Team/"), pic);
                // file is uploaded
                file.SaveAs(path);
                CurrentTeam.teampic = pic;
                _Session.EditTeam(CurrentTeam);
            }
            // after successfully uploading redirect the user
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Team/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Team/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Team/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Team/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Team/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
