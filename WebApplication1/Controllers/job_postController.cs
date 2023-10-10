using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class job_postController : Controller
    {
        private FUI_POLLEntities db = new FUI_POLLEntities();

        // GET: job_post
        public ActionResult Index()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                return View(db.job_post.Where(x => x.campus == campus));
            }
            else {
                return RedirectToAction("Register","members");
            }
        }

        // GET: job_post/Details/5
        [Route("members/job_post/details/{id}")]
        public ActionResult Details(int? id)
        {
            if (Session["alum_email"] != null && Session["alum_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["alum_campus"].ToString();
                job_post job_post = db.job_post
                 .Where(s => s.campus == campus && s.job_post_id == id)
                 .FirstOrDefault<job_post>();
                if (job_post == null)
                {
                    return HttpNotFound();
                }
                return View(job_post);
            }
            else
            {
                return RedirectToAction("Register", "members");
            }
        }

        // GET: job_post/Create
        [Route("home/job_post/create")]
        public ActionResult Create()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }        
        }

        // POST: job_post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(job_post job_post)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = Request.Files["image_path"];
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        job_post.image_path = "Content/Images/" + _FileName;
                        string _path = Path.Combine(Server.MapPath("~/Content/Images"), _FileName);
                        file.SaveAs(_path);
                    }
                    else {
                        job_post.image_path = "Content/Images/no-image-icon.png"; 
                    }
                    //db.job_post.Add(job_post);
                    //db.SaveChanges();
                    job_post.created_at = DateTime.Now;
                    job_post.created_by = Session["adm_name"].ToString();
                    db.insert_job_post(job_post.title, job_post.category, job_post.post_type, job_post.location, job_post.description, job_post.salary, job_post.tags, job_post.image_path,job_post.campus,job_post.created_at,job_post.created_by);
                    return RedirectToAction("view_job_posts", "Home");
                }
                return View(job_post);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }      
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create_job_post()
        //{
        //    if (Session["adm_email"] != null && Session["adm_id"] != null)
        //    {
        //        var httpRequest = HttpContext.Request;
        //        var xyz = httpRequest.Params["xyz"];
        //        var image = httpRequest.Params["image_path"];
        //        HttpPostedFileBase file = Request.Files["image_path"];
        //        if (httpRequest.Files.Count > 0)
        //        {
        //            var myFile = httpRequest.Files[0];
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            return RedirectToAction("Index", "members");
        //        }

        //        return RedirectToAction("Index", "members");
        //    }
        //    else {
        //        return RedirectToAction("Register", "members");
        //    }
        //}

        // GET: job_post/Edit/5
        [Route("home/job_post/Edit/{id}")]
        public ActionResult Edit(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                job_post job_post = db.job_post
                 .Where(s => s.campus == campus && s.job_post_id == id)
                 .FirstOrDefault<job_post>();
                if (job_post == null)
                {
                    return HttpNotFound();
                }
                TempData["image_path"] = job_post.image_path;
                return View(job_post);
            }
            else {
                return RedirectToAction("Register", "members");
            }
        }

        // POST: job_post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("home/job_post/Edit/{id}")]
        public ActionResult Edit(job_post job_post)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(job_post).State = EntityState.Modified;
                    //db.SaveChanges();
                    HttpPostedFileBase file = Request.Files["image_path"];
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        job_post.image_path = "Content/Images/" + _FileName;
                        string _path = Path.Combine(Server.MapPath("~/Content/Images"), _FileName);
                        file.SaveAs(_path);
                    }
                    else
                    {
                        job_post.image_path = TempData["image_path"] as String;
                    }
                    job_post.updated_at = DateTime.Now;
                    job_post.updated_by = Session["adm_name"].ToString();
                    db.update_job_post(job_post.job_post_id, job_post.title, job_post.category, job_post.post_type, job_post.location, job_post.description, job_post.salary, job_post.tags, job_post.image_path,job_post.campus,
                        job_post.created_at,job_post.created_by,job_post.updated_at,job_post.updated_by);
                    return RedirectToAction("view_job_posts","Home");
                }
                return View(job_post);
            }
            else {
                return RedirectToAction("Home", "Login");
            }
        }

        // GET: job_post/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                job_post job_post = db.job_post
                 .Where(s => s.campus == campus && s.job_post_id == id)
                 .FirstOrDefault<job_post>();
                if (job_post == null)
                {
                    return HttpNotFound();
                }
                return View(job_post);
            }
            else {
                return RedirectToAction("Register", "members");
            }
        }

        // POST: job_post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                job_post job_post = db.job_post
                 .Where(s => s.campus == campus && s.job_post_id == id)
                 .FirstOrDefault<job_post>();
                db.job_post.Remove(job_post);
                db.SaveChanges();
                return RedirectToAction("view_job_posts", "Home");
            }
            else {
                return RedirectToAction("Register", "members");
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
