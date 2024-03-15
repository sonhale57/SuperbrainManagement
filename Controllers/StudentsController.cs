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
    public class StudentsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Branch).Include(s => s.MKTCampaign);
            return View(students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
