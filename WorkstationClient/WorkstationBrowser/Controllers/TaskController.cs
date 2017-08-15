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
        public ActionResult _Index(ProjectModel related, SessionWrapper originalSession)
        {
            var allTasks = originalSession.WorkstationSession.GetAllTasks(related.id, null);
            var MyList = new List<TaskModel>{
                new TaskModel()
                  {
                    id = 1,
                    title = "Making everything!",
                    description = null,
                    begin = DateTime.Now,
                    end = null,
                    project_id = related.id,
                    user_id = originalSession.CurrentUser.id
                }
                };
            return PartialView(MyList);
        }
        
    }
}
