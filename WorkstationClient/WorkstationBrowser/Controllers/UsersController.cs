using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            ViewData["MyId"] = wrapper.CurrentUser.id;

            return View(wrapper.GetAllUsers());
        }

        public ActionResult MyProfile(){

            SessionWrapper currentSession = Session["WorkstationConnection"] as SessionWrapper;
            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;
            ViewData["CurrentTeam"] = currentSession.WorkstationSession.GetTeamPerUser(currentSession.CurrentUser.id);
        
            return View(currentSession.CurrentUser);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersModel user = wrapper.WorkstationSession.GetUserId((int)id);
            if (user == null) {
                return HttpNotFound();
            }
            ViewBag.team_id = new SelectList(wrapper.WorkstationSession.GetAllTeams(), "id", "name", user.team_id);
            ViewBag.rank = new SelectList(wrapper.WorkstationSession.GetAllRanks(), "name", "name", user.rank);

            ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

            
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,email,team_id, rank")] UsersModel users) {
            SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;

            users.rights = wrapper.WorkstationSession.GetAllRanks().First(rank => rank.name.Equals(users.rank)).rights;
            
            if (wrapper.WorkstationSession.EditUser(users)) {
                if (wrapper.CurrentUser.id.Equals(users.id))
                {
                    wrapper.WorkstationSession.CreateNotification(new NotificationModel(){
                        content = $"Your profile has been updated at {DateTime.Now}",
                        title = $"Profile updated by {users.username}",
                        read = false
                    }, new int[]{users.id}, false);
                }
                return RedirectToAction("Index");
            }
            else {
                ViewBag.team_id = new SelectList(wrapper.WorkstationSession.GetAllTeams(), "id", "name", users.team_id);
                ViewBag.rank = new SelectList(wrapper.WorkstationSession.GetAllRanks(), "name", "name", users.rank);

                ViewData["CurrentUserRights"] = Session["CurrentUserRights"] as Dictionary<String, bool>;

                return View(users);
            }
        }


    }
}
