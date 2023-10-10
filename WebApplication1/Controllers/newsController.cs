using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class newsController : Controller
    {
        private FUI_POLLEntities db = new FUI_POLLEntities();

        // GET: news
        public ActionResult Index()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                return View(db.news.Where(x => x.campus == campus).OrderByDescending(m => m.created_at).ToList()) ;
            }
            else
            {
                return RedirectToAction("Login","Home");
            }
        }

        // GET: news/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                news news = db.news
                    .Where(s => s.campus == campus && s.news_id == id)
                    .FirstOrDefault<news>();
                if (news == null)
                {
                    return HttpNotFound();
                }
                return View(news);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: news/Create
        public ActionResult Create()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                return View();
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: news/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(news news)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = Request.Files["news_image"];
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        news.news_image = "Content/Images/" + _FileName;
                        string _path = Path.Combine(Server.MapPath("~/Content/Images"), _FileName);
                        file.SaveAs(_path);
                    }
                    //db.news.Add(news);
                    //db.SaveChanges();
                    db.insert_news(news.news_guid, news.news_title, news.news_date, news.news_details,news.campus, news.news_image, news.news_link, news.news_expir_date, news.created_at, news.created_by
                        );
                    return RedirectToAction("Index");
                }
                return View(news);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: news/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                news news = db.news
                    .Where(s => s.campus == campus && s.news_id == id)
                    .FirstOrDefault<news>();
                if (news == null)
                {
                    return HttpNotFound();
                }
                TempData["image_path"] = news.news_image;
                return View(news);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: news/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(news newsx)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = Request.Files["news_image"];
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        newsx.news_image = "Content/Images/" + _FileName;
                        string _path = Path.Combine(Server.MapPath("~/Content/Images"), _FileName);
                        file.SaveAs(_path);
                    }
                    else
                    {
                        newsx.news_image = TempData["image_path"] as String;
                    }
                    //db.Entry(newsx).State = EntityState.Modified;
                    //db.SaveChanges();
                    db.update_news(newsx.news_id, newsx.news_title, newsx.news_date, newsx.news_details, newsx.campus, newsx.news_image, newsx.news_link, newsx.news_expir_date, newsx.created_at, newsx.created_by,
                        newsx.updated_at, newsx.updated_by);
                    return RedirectToAction("Index");
                }
                return View(newsx);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: news/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                news news = db.news
                    .Where(s => s.campus == campus && s.news_id == id)
                    .FirstOrDefault<news>();
                if (news == null)
                {
                    return HttpNotFound();
                }
                return View(news);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: news/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                news news = db.news
                    .Where(s => s.campus == campus && s.news_id == id)
                    .FirstOrDefault<news>();
                db.news.Remove(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login","Home");
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
