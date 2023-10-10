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
    public class upcoming_eventController : Controller
    {
        private FUI_POLLEntities db = new FUI_POLLEntities();

        // GET: upcoming_event
        public ActionResult Index()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                return View(db.upcoming_event.Where(x => x.campus == campus).ToList());
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }

        // GET: upcoming_event/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null) { 
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                upcoming_event upcoming_event = db.upcoming_event
                 .Where(s => s.campus == campus && s.event_id == id)
                 .FirstOrDefault<upcoming_event>();
                if (upcoming_event == null)
            {
                return HttpNotFound();
            }
            return View(upcoming_event);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: upcoming_event/Create
        public ActionResult Create()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                return View();
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }

        // POST: upcoming_event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(upcoming_event upcoming_event)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                upcoming_event.event_guid = Guid.NewGuid();
                if (ModelState.IsValid)
                {
                    //db.upcoming_event.Add(upcoming_event);
                    //db.SaveChanges();
                    db.insert_upcoming_event(upcoming_event.event_guid, upcoming_event.event_date_time, upcoming_event.event_title, upcoming_event.event_details, upcoming_event.campus, upcoming_event.event_link, upcoming_event.event_expir_date
                        , upcoming_event.created_at, upcoming_event.created_by);
                    return RedirectToAction("Index");
                }

                return View(upcoming_event);
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }

        // GET: upcoming_event/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                upcoming_event upcoming_event = db.upcoming_event
                    .Where(s => s.campus == campus && s.event_id == id)
                    .FirstOrDefault<upcoming_event>();         
                if (upcoming_event == null)
                {
                    return HttpNotFound();
                }
                return View(upcoming_event);
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }

        // POST: upcoming_event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(upcoming_event upcoming_event)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(upcoming_event).State = EntityState.Modified;
                    //db.SaveChanges();

                    db.update_upcoming_event(upcoming_event.event_id, upcoming_event.event_guid, upcoming_event.event_date_time, upcoming_event.event_title, upcoming_event.event_details, upcoming_event.campus, upcoming_event.event_link, upcoming_event.event_expir_date
                    ,upcoming_event.created_at, upcoming_event.created_by, upcoming_event.updated_at, upcoming_event.updated_by);
                    return RedirectToAction("Index");
                }
                return View(upcoming_event);
            }
            else
            {
                return RedirectToAction("Login","Home");
            }
        }

        // GET: upcoming_event/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                upcoming_event upcoming_event = db.upcoming_event
                    .Where(s => s.campus == campus && s.event_id == id)
                    .FirstOrDefault<upcoming_event>();
                if (upcoming_event == null)
                {
                    return HttpNotFound();
                }
                return View(upcoming_event);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: upcoming_event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                upcoming_event upcoming_event = db.upcoming_event
                    .Where(s => s.campus == campus && s.event_id == id)
                    .FirstOrDefault<upcoming_event>();
                db.upcoming_event.Remove(upcoming_event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else {
                return RedirectToAction("Login", "Home");
            }
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
