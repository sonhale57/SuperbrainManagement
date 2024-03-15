using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class BranchGroupsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: BranchGroups
        public ActionResult Index()
        {
            return View(db.BranchGroups.ToList());
        }

        // GET: BranchGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BranchGroup branchGroup = db.BranchGroups.Find(id);
            if (branchGroup == null)
            {
                return HttpNotFound();
            }
            return View(branchGroup);
        }

        // GET: BranchGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BranchGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Enable,DateCreate,NameInvestor,PhoneInvestor,EmailInvestor")] BranchGroup branchGroup)
        {
            if (ModelState.IsValid)
            {
                db.BranchGroups.Add(branchGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(branchGroup);
        }

        // GET: BranchGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BranchGroup branchGroup = db.BranchGroups.Find(id);
            if (branchGroup == null)
            {
                return HttpNotFound();
            }
            return View(branchGroup);
        }

        // POST: BranchGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Enable,DateCreate,NameInvestor,PhoneInvestor,EmailInvestor")] BranchGroup branchGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branchGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(branchGroup);
        }

        // GET: BranchGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BranchGroup branchGroup = db.BranchGroups.Find(id);
            if (branchGroup == null)
            {
                return HttpNotFound();
            }
            return View(branchGroup);
        }

        // POST: BranchGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BranchGroup branchGroup = db.BranchGroups.Find(id);
            db.BranchGroups.Remove(branchGroup);
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
