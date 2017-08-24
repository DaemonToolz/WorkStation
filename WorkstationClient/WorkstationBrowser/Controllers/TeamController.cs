using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;

namespace WorkstationBrowser.Controllers
{
    public class TeamController : Controller
    {
        // GET: Team
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            ViewData["TeamId"] = wrapper.CurrentUser.team_id;
            ViewData["AllDepartments"] = wrapper.GetAllDepartments();
            ViewData["AllProjects"] = wrapper.GetAllProjects();

            return View(wrapper.WorkstationSession.GetAllTeams());
        }

        // GET: Team/Details/5
        public ActionResult Details(int id){
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            var currentTeam = wrapper.WorkstationSession.GetAllTeams().First(team => team.id == id);
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            ViewBag.id = id;
            ViewData["Department"] = wrapper.WorkstationSession.GetAllDepartments()
                .Single(dept => dept.id == currentTeam.department_id);
            if(currentTeam.project_id != null)
                ViewData["Project"] = wrapper.WorkstationSession.GetProject((long)currentTeam.project_id);

            ViewData["ActiveMembers"] = wrapper.WorkstationSession.GetAllUsers().Where(user => user.team_id == id).ToArray();
            return View(currentTeam);
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file, int id)
        {

            if (file != null)
            {
                SessionWrapper currentSession = Session["WorkstationConnection"] as SessionWrapper;
                var CurrentTeam = currentSession.WorkstationSession.GetTeamPerId(id);
                string pic = CurrentTeam.name.Replace(" ", String.Empty) + "_ico" + Path.GetExtension(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/UserContent/Team/"), pic);
                // file is uploaded
                file.SaveAs(path);
                CurrentTeam.teampic = pic;
                currentSession.WorkstationSession.EditTeam(CurrentTeam);
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
