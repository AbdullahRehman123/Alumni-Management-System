using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.Validation;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class membersController : Controller
    {
        private FUI_POLLEntities db = new FUI_POLLEntities();
        //const int keySize = 64;
        //const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;


        public ActionResult upcoming_events()
        {
            if (Session["alum_email"] != null && Session["alum_id"] != null)
            {
                string campus = Session["alum_campus"].ToString();            
                return View(db.upcoming_event.Where(x => x.campus == campus));
            }
            else
            {
                return RedirectToAction("Register");
            }
        }

        public ActionResult latest_news()
        {
            if (Session["alum_email"] != null && Session["alum_id"] != null)
            {
                string campus = Session["alum_campus"].ToString();
                return View(db.news.Where(x => x.campus == campus));
            }
            else
            {
                return RedirectToAction("Register");
            }
        }


        public ActionResult Index()
        {
            if (Session["alum_email"] != null && Session["alum_id"] != null)
            {
                string campus = Session["alum_campus"].ToString();
                List<job_post> jobs_list = db.job_post.Where(x => x.campus == campus).ToList();
                jobs_list.Reverse();
                return View(jobs_list);
            }
            else {
                return RedirectToAction("Register");
            }
        }


        // GET: members/Details/5
        public ActionResult Details()
        {
            if (Session["alum_id"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            member member = db.members.Find(Session["alum_id"]);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: members/Create
        public ActionResult Register()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(member m)
        {
            if(Request.UrlReferrer==null)
            {
                return Json(new { success = false, responseText = "URL referer not present" }, JsonRequestBehavior.AllowGet);
            }
            if (m.program_studied == "Choose Program")
            {
                ModelState.AddModelError("m.program_studied",
                                         "Please Choose Program");
            }
            m.guid = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                var email = db.members
                    .Where(b => b.email.ToLower() == m.email.ToLower())
                    .FirstOrDefault();
                if (email == null)
                {
                    m.headers = Request.Headers.ToString();
                    m.ip_address = Request.UserHostAddress;
                    db.insert_member_sp(m.name, m.email, m.guid, m.mobile_no, m.ip_address, m.headers, m.created_at, m.created_by, m.already_voted, EncodePasswordToBase64(m.password), EncodePasswordToBase64(m.confirm_password), m.registration_no, m.degree_completion_date, m.campus, m.category_studied, m.program_studied, m.org_comp_working_in, m.designation);
                    return Json(new { success = true, responseText = "Registered Successfully. Kindly Wait For Verification from Admin!" }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { success = false, responseText = "Email address already exists" }, JsonRequestBehavior.AllowGet);
                }
            }
            var errors = string.Join(" | ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return Json(new { success = false, responseText = Json(errors) }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Logout()
        {
            if (Session["alum_email"] !=null && Session["alum_id"]!=null)
            {
                Session.Remove("alum_email");
                Session.Remove("alum_id");
                Session.Remove("alum_campus");
                return RedirectToAction("Register");
            }
            else
            {
                return RedirectToAction("Register");
            }
        }


        [HttpPost]
        public ActionResult LoginNew(member member)
        {
            string encodedPassword = "";
            if (member.password != null)
            {
                encodedPassword = EncodePasswordToBase64(member.password);
            }
            member m = db.members.FirstOrDefault(u => u.email == member.email &&  u.password == encodedPassword);
            if (m != null)
            {
                if (m.validation_status == 1)
                {
                    //  Successful Login
                    Session["alum_email"] = m.email;
                    Session["alum_id"] = m.member_id;
                    Session["alum_campus"] = m.campus;
                    return Json(new { redirectToUrl = Url.Action("Index", "members") });
                }
                //  Registration Nor verified
                else {
                    return Json(new { success = false, responseText = "Your registration is pending verification from Admin" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //  Send "Failure"
                if (member.email == null && member.password == null) { return Json(new { success = false, responseText = "Please enter Email and Password" }, JsonRequestBehavior.AllowGet); }
                else { return Json(new { success = false, responseText = "Invalid Email or Password" }, JsonRequestBehavior.AllowGet); }
                
            }
        }

        //GET: members/Edit/5
        public ActionResult Edit()
        {
            if (Session["alum_id"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            member member = db.members.Find(Session["alum_id"]);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //POST: members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Edit(member m)
        {
            if (ModelState.IsValid)
            {
                member member = db.members.Find(Session["alum_id"]);
                m.updated_by = Session["alum_id"].ToString();
                m.updated_at = DateTime.Now;
                db.update_member_sp(m.member_id, m.name, m.email, member.guid, m.mobile_no, member.ip_address, member.headers, member.created_at, member.created_by,m.updated_at,m.updated_by,m.already_voted,
                member.password,member.confirm_password,m.registration_no,m.degree_completion_date,m.campus,m.category_studied,m.program_studied,m.org_comp_working_in,m.designation,member.validation_status);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(m);
        }

        //// GET: members/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    member member = db.members.Find(id);
        //    if (member == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(member);
        //}

        //// POST: members/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    member member = db.members.Find(id);
        //    db.members.Remove(member);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult GetCategoryPrograms(string category, string campus)
        {
            string configvalue3 = "";
            if (campus == "FUSST (formerly known as FURC or FUIEMS)") {
                if (category == "BS") { configvalue3 = ConfigurationManager.AppSettings["fusst_bs_programs_list"]; }
                else if (category == "MS") { configvalue3 = ConfigurationManager.AppSettings["fusst_ms_programs_list"]; }
                else if (category == "PHD") { configvalue3 = ConfigurationManager.AppSettings["fusst_phd_programs_list"]; }
            }
            else if (campus == "FUSH (formerly known as FUIC or FUMC)")
            {
                if (category == "BS") { configvalue3 = ConfigurationManager.AppSettings["fush_bs_programs_list"]; }
                else if (category == "MS") { configvalue3 = ConfigurationManager.AppSettings["fush_ms_programs_list"]; }
                else if (category == "PHD") { configvalue3 = ConfigurationManager.AppSettings["fush_phd_programs_list"]; }
            }

            string[] values3 = configvalue3.Split(',').Select(sValue => sValue.Trim()).ToArray();
            List<string> bs_programs_list = new List<string>();
            for (int i = 0; i < values3.Length; i++)
            {
                    bs_programs_list.Add(values3[i]);     
            }
            return Json(bs_programs_list, JsonRequestBehavior.AllowGet);
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
                throw new Exception("Error in function Password Encoder" + ex.Message);
            }
        }


        //this function Convert to Decord your Password
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
                throw new Exception("Error in function Password decoder" + ex.Message);
            }

        }
    }
}
