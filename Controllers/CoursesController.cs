using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class CoursesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Courses
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var courses = db.Courses.Include(b => b.Program);
            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Code.ToLower().Contains(searchString.ToLower()) || x.Program.Name.ToLower().Contains(searchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    courses = courses.OrderByDescending(s => s.Name);
                    break;
                case "date":
                    courses = courses.OrderBy(s => s.Id);
                    break;
                case "date_desc":
                    courses = courses.OrderByDescending(s => s.Id);
                    break;
                case "name":
                    courses = courses.OrderBy(s => s.Name);
                    break;
                default:
                    courses = courses.OrderBy(s => s.Program.Name);
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var pagedData = courses.ToPagedList(pageNumber, pageSize);
            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };
            ViewBag.PagedListRenderOptions = pagedListRenderOptions;
            return View(pagedData);
        }
        /*
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var program = db.Programs;
            if (!string.IsNullOrEmpty(searchString))
            {
                program = program.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Code.ToLower().Contains(searchString.ToLower()) || x.Program.Name.ToLower().Contains(searchString.ToLower()));
            }/*
            switch (sortOrder)
            {
                case "name_desc":
                    courses = courses.OrderByDescending(s => s.Name);
                    break;
                case "date":
                    courses = courses.OrderBy(s => s.Id);
                    break;
                case "date_desc":
                    courses = courses.OrderByDescending(s => s.Id);
                    break;
                case "name":
                    courses = courses.OrderBy(s => s.Name);
                    break;
                default:
                    courses = courses.OrderBy(s => s.Program.Name);
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var pagedData = program.ToPagedList(pageNumber, pageSize);
            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };
            ViewBag.PagedListRenderOptions = pagedListRenderOptions;
            return View(pagedData);
        }*/
        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Hour,Levels,DisplayOrder,Price,IdProgram,Description,FormulaMidterm,SpeedMidterm,FormulaEndterm,SpeedEndterm,DevelopRoute,ScorePass,DateCreate,IdUser,Sessions,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Hour,Levels,DisplayOrder,Price,IdProgram,Description,FormulaMidterm,SpeedMidterm,FormulaEndterm,SpeedEndterm,DevelopRoute,ScorePass,DateCreate,IdUser,Sessions,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
