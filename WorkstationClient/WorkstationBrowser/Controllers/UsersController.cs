using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers
{
    public class UsersController : GenericController
    {
        // GET: Users
        public ActionResult Index()
        {
            ViewData["MyId"] = _Session.CurrentUser.id;

            return View(_Session.GetAllUsers());
        }

        public ActionResult MyProfile(){
            ViewData["CurrentTeam"] = _Session.GetTeamByUser(_Session.CurrentUser);
        
            return View(_Session.CurrentUser);
        }

        public ActionResult FileUpload(HttpPostedFileBase file){
           
            if (file != null){
                //SessionWrapper currentSession = Session["WorkstationConnection"] as SessionWrapper;

                string pic = _Session.CurrentUser.username.Replace(" ", String.Empty) + "_profile" + Path.GetExtension(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/UserContent/Profile/"), pic);
                // file is uploaded
                file.SaveAs(path);
                _Session.CurrentUser.profilepic = pic;
                _Session.EditUser(_Session.CurrentUser);
            }
            // after successfully uploading redirect the user
            return RedirectToAction("MyProfile");
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            //SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UsersModel user = _Session.GetUserById((int)id);
            if (user == null) {
                return HttpNotFound();
            }
            ViewBag.team_id = new SelectList(_Session.GetAllTeams(), "id", "name", user.team_id);
            ViewBag.rank = new SelectList(_Session.GetAllRanks(), "name", "name", user.rank);

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,email,team_id, rank")] UsersModel users) {
            //SessionWrapper wrapper = Session["WorkstationConnection"] as SessionWrapper;

            users.profilepic = _Session.GetUserById((int)users.id).profilepic;
            
            users.rights = _Session.GetRankByName(users.rank).rights;
            
            if (_Session.EditUser(users)) {

                return RedirectToAction("Index");
            }
            else {
                ViewBag.team_id = new SelectList(_Session.GetAllTeams(), "id", "name", users.team_id);
                ViewBag.rank = new SelectList(_Session.GetAllRanks(), "name", "name", users.rank);

      
                return View(users);
            }
        }


    }
}
