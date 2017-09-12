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
    public class RanksController : Controller
    {
        private ManagementEntities db = new ManagementEntities();

        // GET: Ranks
        public ActionResult Index()
        {
            return View(db.Rank.ToList());
        }

        // GET: Ranks/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = db.Rank.Find(id);
            
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // GET: Ranks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ranks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

            // This model should evolve by using a IEnumerable<> instead of bools (yet it is fixed right now)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name")] Rank rank, 
            bool IsAdmin = false, bool CanReadDept = true, bool CanEditDept = false, 
            bool CanReadProj = true, bool CanEditProj = false, bool CanReadTeam = true,
            bool CanEditTeam = false, bool CanReadUser = true, bool CanEditUser = false,
            bool CanReadTask = true, bool CanEditTask = true, bool CanReadMesg = true,
            bool CanEditMesg = true, bool CanReadNoti = true, bool CanEditNoti = true)
        {
            try
            {

       
                //
                rank.rights = (IsAdmin ? "1" : "0") +
                              (CanReadDept ? "1" : "0") +
                              (CanEditDept ? "1" : "0") +
                              (CanReadProj ? "1" : "0") +
                              (CanEditProj ? "1" : "0") +
                              (CanReadTeam ? "1" : "0") +
                              (CanEditTeam ? "1" : "0") +
                              (CanReadUser ? "1" : "0") +
                              (CanEditUser ? "1" : "0") +
                              (CanReadTask ? "1" : "0") +
                              (CanEditTask ? "1" : "0") +
                              (CanReadMesg ? "1" : "0") +
                              (CanEditMesg ? "1" : "0") +
                              (CanReadNoti ? "1" : "0") +
                              (CanEditNoti ? "1" : "0");

                    db.Rank.Add(rank);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                
            }
            catch{
                return View(rank);
            }
            
        }

        // GET: Ranks/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = db.Rank.Find(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // POST: Ranks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "name,rights")] Rank rank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rank);
        }

        // GET: Ranks/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = db.Rank.Find(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // POST: Ranks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Rank rank = db.Rank.Find(id);
            db.Rank.Remove(rank);
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
