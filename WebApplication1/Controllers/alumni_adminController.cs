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
    public class alumni_adminController : Controller
    {
        private FUI_POLLEntities db = new FUI_POLLEntities();

        // GET: alumni_admin
        public ActionResult Index()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                return View(db.alumni_admin.Where(x => x.campus == campus).OrderByDescending(m => m.created_at).ToList());
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }


        public ActionResult my_account()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                int adm_id = ((int)Session["adm_id"]);
                alumni_admin alumni_admin = db.alumni_admin
                      .Where(s => s.campus == campus && s.adm_id == adm_id)
                      .FirstOrDefault<alumni_admin>();
                if (alumni_admin == null)
                {
                    return HttpNotFound();
                }
                return View("Details",alumni_admin);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: alumni_admin/Edit/5
        public ActionResult edit_account()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                int adm_id = ((int)Session["adm_id"]);
                alumni_admin alumni_admin = db.alumni_admin
                      .Where(s => s.campus == campus && s.adm_id == adm_id)
                      .FirstOrDefault<alumni_admin>();
                if (alumni_admin == null)
                {
                    return HttpNotFound();
                }
                return View("Edit",alumni_admin);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        // GET: alumni_admin/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                alumni_admin alumni_admin = db.alumni_admin
                      .Where(s => s.campus == campus && s.adm_id == id)
                      .FirstOrDefault<alumni_admin>();
                if (alumni_admin == null)
                {
                    return HttpNotFound();
                }
                return View(alumni_admin);
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }

        // GET: alumni_admin/Create
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

        //POST: alumni_admin/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "adm_id,adm_guid,adm_name,adm_email,adm_password,is_active,campus")] alumni_admin alumni_admin)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {

                if (ModelState.IsValid)
                {
                    alumni_admin.adm_guid = Guid.NewGuid();
                    alumni_admin.created_at = DateTime.Now;
                    alumni_admin.created_by = Session["adm_name"].ToString();
                    alumni_admin.adm_password = EncodePasswordToBase64(alumni_admin.adm_password);
                    db.alumni_admin.Add(alumni_admin);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(alumni_admin);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in EncodePasswordToBase64" + ex.Message);
            }
        }


        //this function decodes your Password
        public string DecodePasswordFromBase64(string encodedData)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(encodedData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in function DecodePasswordFromBase64" + ex.Message);
            }

        }



        // GET: alumni_admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string campus = Session["adm_campus"].ToString();
            alumni_admin alumni_admin = db.alumni_admin
                  .Where(s => s.campus == campus && s.adm_id == id)
                  .FirstOrDefault<alumni_admin>();
            if (alumni_admin == null)
            {
                return HttpNotFound();
            }
            return View(alumni_admin);
        }

        // POST: alumni_admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(alumni_admin alumni_admin)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    alumni_admin.updated_at = DateTime.Now;
                    alumni_admin.updated_by = Session["adm_name"].ToString();
                    db.Entry(alumni_admin).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(alumni_admin);
            }
            else {
                return RedirectToAction("Login","Home");
            }
        }

        // GET: alumni_admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                alumni_admin alumni_admin = db.alumni_admin
                      .Where(s => s.campus == campus && s.adm_id == id)
                      .FirstOrDefault<alumni_admin>();
                if (alumni_admin == null)
                {
                    return HttpNotFound();
                }
                return View(alumni_admin);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: alumni_admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                alumni_admin alumni_admin = db.alumni_admin
                      .Where(s => s.campus == campus && s.adm_id == id)
                      .FirstOrDefault<alumni_admin>();
                db.alumni_admin.Remove(alumni_admin);
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
