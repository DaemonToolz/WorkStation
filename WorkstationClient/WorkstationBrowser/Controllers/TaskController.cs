using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        [ChildActionOnly]
        public ActionResult _Index(ProjectModel related, SessionWrapper originalSession, int? userid, short? AddSection)
        {
            var allTasks = originalSession.WorkstationSession.GetAllTasks(related?.id, userid);
            Session["ProjectId"] = related.id;
            ViewData["AddSection"] = AddSection;

            return PartialView(allTasks);
        }

        [ChildActionOnly]
        public ActionResult _Create(){
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            var CurrentTeam = wrapper.WorkstationSession.GetAllTeams()
                .Single(team => team.project_id == (long) Session["ProjectId"]);
            var CurrentUsers = wrapper.WorkstationSession.GetAllUsers().Where(user => user.team_id == CurrentTeam.id);
            
            ViewBag.user_id = new SelectList(
                CurrentUsers, 
                "id", "username");

            return PartialView();
        }

        [ChildActionOnly]
        [HttpPost]
        public ActionResult _Create([Bind(Include = "title,description, begin, end, user_id")] TaskModel model){
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            model.project_id = long.Parse(Session["ProjectId"].ToString());
            wrapper.WorkstationSession.CreateTask(model);
            
            return PartialView();
        }

        /*
        [ChildActionOnly]
        [HttpPost]
        public ActionResult _Index([Bind(Include = "id,title,description,begin, end, user_id, project_id")] TaskModel model, string action){

            var currentSession = Session["WorkstationConnection"] as SessionWrapper;
            
            if (action.Equals("Delete"))
                currentSession.WorkstationSession.DeleteTask(model);

            var currentProject = Session["CurrentProject"] as ProjectModel;
            return PartialView();
        }
        */
    }
}
