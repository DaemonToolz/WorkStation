using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WorkstationManagementServices.Models.Database;

namespace WorkstationManagementServices.Controllers
{
    public class UsersController : Controller
    {
        private ManagementEntities db = new ManagementEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Team);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {

            ViewBag.team_id = new SelectList(db.Team, "id", "name");

            ViewBag.rank = new SelectList(db.Rank, "name", "name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,username,password,encrypted,email,creationdate,team_id,rank")] Users users){
            if (ModelState.IsValid){
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.team_id = new SelectList(db.Team, "id", "name", users.team_id);
            ViewBag.rank = new SelectList(db.Rank, "name", "name", users.rank);

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.team_id = new SelectList(db.Team, "id", "name", users.team_id);

            ViewBag.rank = new SelectList(db.Rank, "name", "name", users.rank);
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,password,encrypted,email,creationdate,team_id, rank")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.team_id = new SelectList(db.Team, "id", "name", users.team_id);
            ViewBag.rank = new SelectList(db.Rank, "name", "name", users.rank);
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
