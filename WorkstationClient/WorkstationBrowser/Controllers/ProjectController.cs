using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;

namespace WorkstationBrowser.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["MyId"] = wrapper.CurrentUser.id;
            ViewData["MyTeam"] = wrapper.CurrentUser.team_id;
            var AllProjects = wrapper.WorkstationSession.GetAllProjects();
           
            var MyTeam = wrapper.WorkstationSession.GetAllTeams().First(team => team.id == wrapper.CurrentUser.team_id);
      
            ViewData["MyProject"] = AllProjects.First(project => project.id == MyTeam.project_id);
          
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

            return View(wrapper.WorkstationSession.GetAllProjects());
        }

        // GET: Project/Details/5
        public ActionResult Details(int id){
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentSession"] = wrapper;

            return View(wrapper.WorkstationSession.GetProject((long)id));
        }

       
    }
}
