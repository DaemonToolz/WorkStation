using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

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

            var members = _Session.GetAllUsers().Where(user => user.team_id == id).ToArray();
            try
            {
                ViewData["Manager"] = members.Single(user => currentTeam.manager_id == user.id);
            }
            catch
            {
                ViewData["Manager"] = null;

            }

            ViewData["ActiveMembers"] = members;
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

                
                var files = Directory.GetFiles(Server.MapPath("~/UserContent/Team/"), CurrentTeam.name.Replace(" ", String.Empty) + "_ico.*");
                foreach (var tmpFile in files)
                    System.IO.File.Delete(tmpFile);


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

            ViewBag.department_id = new SelectList(_Session.GetAllDepartments(), "id", "name");
            ViewBag.project_id = new SelectList(_Session.GetAllProjects(), "id", "name");

            return View();
        }

        // POST: Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="name, project_id, department_id")] TeamModel model)
        {
            model.teampic = "Team_default.png";
            if (_Session.CreateTeam(model))
                return RedirectToAction("Index");
            else {
                ViewBag.department_id = new SelectList(_Session.GetAllDepartments(), "id", "name");
                ViewBag.project_id = new SelectList(_Session.GetAllProjects(), "id", "name");

                return View();
            }
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int id) {
            ViewBag.department_id = new SelectList(_Session.GetAllDepartments(), "id", "name");
            ViewBag.project_id = new SelectList(_Session.GetAllProjects(), "id", "name");

            var members = _Session.GetAllUsers().Where(usr => usr.team_id == id).ToList();
            members.Add(new UsersModel() {id = 0, username = "Not Affected"});
            ViewBag.manager_id = new SelectList(members , "id", "username");

            return View(_Session.GetTeamById(id));
        }

        // POST: Team/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, [Bind(Include ="id,department_id,name,project_id, teampic, manager_id")] TeamModel model)
        {
            if (model.manager_id == 0)
                model.manager_id = null;

            if (_Session.EditTeam(model))
                return RedirectToAction("Index");
            
            ViewBag.department_id = new SelectList(_Session.GetAllDepartments(), "id", "name");
            ViewBag.project_id = new SelectList(_Session.GetAllProjects(), "id", "name");
            return View(model);

        }

  
        // POST: Team/Delete/5
        
        public ActionResult Delete(int id)
        {
            _Session.DeleteTeam(id);
            return RedirectToAction("Index");
          
        }
    }
}
