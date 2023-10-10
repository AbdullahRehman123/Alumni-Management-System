using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class candidatesController : Controller
    {
        private FUI_POLLEntities db = new FUI_POLLEntities();

        // GET: candidates
        public ActionResult Index()
        {
            var candidates = db.candidates.Include(c => c.category).Include(c => c.election);
            return View(candidates.ToList());
        }

        // GET: candidates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            candidate candidate = db.candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // GET: candidates/Create
        public ActionResult Create()
        {
            ViewBag.category_name = new SelectList(db.categories, "category_name", "category_name");
            ViewBag.election_title = new SelectList(db.elections, "election_title", "election_title");
            return View();
        }

        // POST: candidates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "election_title,category_name,name,image,grad_dept,grad_year,indust_org_working,exper_years,town_city_province,message_to_voter,education,experience_details,candidate_id")] candidate candidate)
        {
            if (ModelState.IsValid)
            {
                db.candidates.Add(candidate);
                db.SaveChanges();
                return RedirectToAction("dashboard","home");
            }

            ViewBag.category_name = new SelectList(db.categories, "category_name", "category_name", candidate.category_name);
            ViewBag.election_title = new SelectList(db.elections, "election_title", "election_title", candidate.election_title);
            return View(candidate);
        }

        // GET: candidates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            candidate candidate = db.candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_name = new SelectList(db.categories, "category_name", "category_name", candidate.category_name);
            ViewBag.election_title = new SelectList(db.elections, "election_title", "election_title", candidate.election_title);
            return View(candidate);
        }

        // POST: candidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "election_title,category_name,name,image,grad_dept,grad_year,indust_org_working,exper_years,town_city_province,message_to_voter,education,experience_details,candidate_id")] candidate candidate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard","Home");
            }
            ViewBag.category_name = new SelectList(db.categories, "category_name", "category_name", candidate.category_name);
            ViewBag.election_title = new SelectList(db.elections, "election_title", "election_title", candidate.election_title);
            return View(candidate);
        }

        // GET: candidates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            candidate candidate = db.candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // POST: candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            candidate candidate = db.candidates.Find(id);
            db.candidates.Remove(candidate);
            db.SaveChanges();
            return RedirectToAction("Dashboard","Home");
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
