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
     
        public ActionResult _Index(ProjectModel related, SessionWrapper originalSession, int? userid, short? AddSection)
        {
            var allTasks = originalSession.WorkstationSession.GetAllTasks(related?.id, userid);
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            
            Session["ProjectId"] = related.id;
            ViewData["AddSection"] = AddSection;
            if (userid == null && related != null)
            {
                SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
                var CurrentTeam = wrapper.WorkstationSession.GetAllTeams()
                    .Single(team => team.project_id == related.id);

                ViewData["TeamMembers"] = wrapper.WorkstationSession.GetAllUsers()
                    .Where(user => user.team_id == CurrentTeam.id).ToArray();
            }

            return PartialView(allTasks);
        }


        public ActionResult _Create(){
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            var CurrentTeam = wrapper.WorkstationSession.GetAllTeams()
                .Single(team => team.project_id == (long) Session["ProjectId"]);

            List<UsersModel> CurrentUsers = wrapper.WorkstationSession.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel(){ id = 0, username = "Not affected"});
           
            ViewBag.user_id = new SelectList(
                CurrentUsers, 
                "id", "username");

            return PartialView();
        }

        [HttpPost]
        
        public ActionResult _Create([Bind(Include = "title,description, begin, end, user_id")] TaskModel model){
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;

            model.project_id = long.Parse(Session["ProjectId"].ToString());
            if (model.user_id == 0)
                model.user_id = null;

            wrapper.WorkstationSession.CreateTask(model);

            var project = wrapper.WorkstationSession.GetProject(model.project_id);
            var CurrentTeam = wrapper.WorkstationSession.GetAllTeams()
                .Single(team => team.project_id == (long)Session["ProjectId"]);
            List<UsersModel> CurrentUsers = wrapper.WorkstationSession.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });

            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");

            return RedirectToAction("Details", "Project", new { id = model.project_id });
        }

        
        [HttpPost]
        public ActionResult _Index([Bind(Include = "id,title,description,begin, end, user_id, project_id")] TaskModel model, string action){

            var currentSession = Session["WorkstationConnection"] as SessionWrapper;

            switch (action) {
                case "Delete":
                    currentSession.WorkstationSession.DeleteTask(model);
                    break;
                case "Edit":
                    return RedirectToAction("Edit", "Task", new {id = model.id});
                    break;
            }

            var allTasks = currentSession.WorkstationSession.GetAllTasks(model.project_id, null);
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

            Session["ProjectId"] = model.project_id;
            ViewData["AddSection"] = true;

            return RedirectToAction("Details", "Project", new {id = model.project_id});
        }


        public ActionResult Edit(long id)
        {
          
            var currentSession = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            var CurrentTeam = currentSession.WorkstationSession.GetAllTeams()
                .Single(team => team.project_id == (long)Session["ProjectId"]);
            List<UsersModel> CurrentUsers = currentSession.WorkstationSession.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });

            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");
            return View(currentSession.WorkstationSession.GetTaskId(id));
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,description,begin,end,user_id,project_id")] TaskModel task, String action)
        {
            if(action.Equals("Cancel"))
                return RedirectToAction("Details", "Project", new { id = task.project_id });

            var currentSession = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            var CurrentTeam = currentSession.WorkstationSession.GetAllTeams()
                .Single(team => team.project_id == (long)Session["ProjectId"]);
            List<UsersModel> CurrentUsers = currentSession.WorkstationSession.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });
            if (task.user_id == 0)
            {
                task.user_id = null;
            }
            currentSession.WorkstationSession.EditTask(task);
            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");
            return RedirectToAction("Details","Project",new{id = task.project_id});
        }
    }
}
