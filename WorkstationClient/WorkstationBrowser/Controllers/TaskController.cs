using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class TaskController : GenericController
    {
        // GET: Task
     
        public ActionResult _Index(ProjectModel related, int? userid, short? AddSection)
        {
   
            IEnumerable<TaskModel> allTasks  = new List<TaskModel>();
            if ((related == null || related.id <= 0) && (userid != null && userid > 0))
                allTasks = _Session.GetTasksByUser((int) userid);
            else if ((related != null && related.id > 0) && (userid == null || userid <= 0))
                allTasks = _Session.GetTasks((int)related?.id);
            else if ((related != null && related.id > 0) && (userid != null && userid > 0))
                allTasks = _Session.GetTasksByUser((int)userid).Where(task => task.project_id == related.id);
            Session["ProjectId"] = related?.id;
            ViewData["AddSection"] = AddSection;
            if (userid == null && related != null) {

                try
                {
                    var CurrentTeam = _Session.GetAllTeams()
                        .Single(team => team.project_id == related.id);

                    ViewData["TeamMembers"] = _Session.GetAllUsers()
                        .Where(user => user.team_id == CurrentTeam.id).ToArray();
                }
                catch
                {
                    ViewData["TeamMembers"] = new TeamModel[]{};
                }
            }

            return PartialView(allTasks);
        }


        public ActionResult _Create()
        {
            List<UsersModel> CurrentUsers;
            try
            {
                var CurrentTeam = _Session.GetAllTeams()
                    .Single(team => team.project_id == (long) Session["ProjectId"]);

                CurrentUsers = _Session.GetAllUsers().Where(user => user.team_id == CurrentTeam.id)
                    .ToList();
               
            }
            catch
            {
                CurrentUsers = new List<UsersModel>();
            }

            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });

            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");

            return PartialView();
        }

        [HttpPost]
        
        public ActionResult _Create([Bind(Include = "title,description, begin, end, user_id, precedence")] TaskModel model){
            //SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            model.progress = 0;
            model.project_id = long.Parse(Session["ProjectId"].ToString());
            if (model.user_id == 0)
                model.user_id = null;

            _Session.CreateTask(model);

            var project = _Session.GetProject(model.project_id);
            var CurrentTeam = _Session.GetAllTeams()
                .Single(team => team.project_id == (long)Session["ProjectId"]);
            List<UsersModel> CurrentUsers = _Session.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });

            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");

            return RedirectToAction("Details", "Project", new { id = model.project_id });
        }

        
        [HttpPost]
        public ActionResult _Index([Bind(Include = "id,title,description,begin, end, user_id, project_id, progress, precedence")] TaskModel model, string action){

            //var currentSession = Session["WorkstationConnection"] as SessionWrapper;

            switch (action) {
                case "Delete":
                    _Session.DeleteTask(model);
                    break;
                case "Edit":
                    return RedirectToAction("Edit", "Task", new {id = model.id});
            }

            var allTasks = _Session.GetTasks((int)model.project_id);
            
            Session["ProjectId"] = model.project_id;
            ViewData["AddSection"] = true;

            return RedirectToAction("Details", "Project", new {id = model.project_id});
        }


        public ActionResult Edit(long id)
        {
          
            var CurrentTeam = _Session.GetAllTeams()
                .Single(team => team.project_id == (long)Session["ProjectId"]);

            List<UsersModel> CurrentUsers = 
                _Session.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });

            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");
            return View(_Session.GetTaskById(id, (long)Session["ProjectId"]));
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,description,begin,end,user_id,project_id, progress, precedence")] TaskModel task, String action)
        {
            if(action.Equals("Cancel"))
                return RedirectToAction("Details", "Project", new { id = task.project_id });
            
            var CurrentTeam = _Session.GetAllTeams()
                .Single(team => team.project_id == (long)Session["ProjectId"]);
            List<UsersModel> CurrentUsers = _Session.GetAllUsers().Where(user => user.team_id == CurrentTeam.id).ToList();
            CurrentUsers.Add(new UsersModel() { id = 0, username = "Not affected" });
            if (task.user_id == 0)
            {
                task.user_id = null;
            }
            _Session.EditTask(task);
            ViewBag.user_id = new SelectList(
                CurrentUsers,
                "id", "username");
            return RedirectToAction("Details","Project",new{id = task.project_id});
        }
    }
}
