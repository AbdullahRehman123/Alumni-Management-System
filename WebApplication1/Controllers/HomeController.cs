using Antlr.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        FUI_POLLEntities db = new FUI_POLLEntities();
        dynamic dy = new ExpandoObject();

        // GET: alumni_admin/Details/5

        [HttpPost]
        [ValidateAntiForgeryToken]    
        public ActionResult add_upcoming_event([Bind(Include = "event_date_time,event_title,event_details,event_link")] upcoming_event upcoming_event)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                upcoming_event.event_guid = Guid.NewGuid();
                if (ModelState.IsValid)
                {
                    db.upcoming_event.Add(upcoming_event);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(upcoming_event);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Index()
        {
            try
            {
                List<upcoming_event> events_list = db.upcoming_event.ToList();
                List<news> news_list = db.news.ToList();
                List<job_post> jobs_list = db.job_post.ToList();
                events_list.Reverse();
                news_list.Reverse();
                jobs_list.Reverse();
                ViewBag.events_list = events_list;
                ViewBag.news_list = news_list;
                ViewBag.jobs_list = jobs_list;
                return View();
            }
            catch (Exception e)
            {
                ViewBag.message = e.Message;
                return View();
            }
        }

        // GET: members/Details/5
        public ActionResult Details(int id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                member member = db.members
                      .Where(s => s.campus == campus && s.member_id == id)
                      .FirstOrDefault<member>();
                if (member == null)
                {
                    return HttpNotFound();
                }
                return View(member);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var validation_status_options = db.validation_status.ToList();
                if (validation_status_options != null)
                {
                    ViewBag.data = validation_status_options;
                }
                string campus = Session["adm_campus"].ToString();
                member member = db.members
                      .Where(s => s.campus == campus && s.member_id == id)
                      .FirstOrDefault<member>();
                if (member == null)
                {
                    return HttpNotFound();
                }
                return View(member);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult validate(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                member member = db.members
                      .Where(s => s.campus == campus && s.member_id == id)
                      .FirstOrDefault<member>();
                if (member == null)
                {
                    return HttpNotFound();
                }
                else if (member != null)
                {
                    member.validation_status = 1;
                    member.updated_at = DateTime.Now;
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult reject(int? id)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string campus = Session["adm_campus"].ToString();
                member member = db.members
                      .Where(s => s.campus == campus && s.member_id == id)
                      .FirstOrDefault<member>();
                if (member == null)
                {
                    return HttpNotFound();
                }
                else if (member != null)
                {
                    member.validation_status = 3;
                    member.updated_at = DateTime.Now;
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        //POST: home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(member m)
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(m).State = EntityState.Modified;
                    db.update_member_sp(m.member_id, m.name, m.email, m.guid, m.mobile_no, m.ip_address, m.headers, m.created_at, m.created_by, m.updated_at, m.updated_by, m.already_voted,
                    m.password, m.confirm_password, m.registration_no, m.degree_completion_date, m.campus, m.category_studied, m.program_studied, m.org_comp_working_in, m.designation,m.validation_status);
                    //db.SaveChanges();
                    return RedirectToAction("Dashboard", "Home");
                }
                var validation_status_options = db.validation_status.ToList();
                if (validation_status_options != null)
                {
                    ViewBag.data = validation_status_options;
                }
                return View(m);
            }
            else { 
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

        [HttpPost]
        public ActionResult redirectDashboard()
        {
            try
            {
                var email = Request["email"];
                var password = Request["password"];
                var campus = Request["campus"];
                string encodedPassword = "";

                if (password != null)
                {
                    encodedPassword = EncodePasswordToBase64(password);
                }
                alumni_admin adm = db.alumni_admin.FirstOrDefault(u => u.adm_email == email && u.adm_password == encodedPassword && u.campus == campus && u.is_active == true);
                if (adm != null)
                {
                    ViewBag.email = email;
                    Session["adm_email"] = email;
                    Session["adm_id"] = adm.adm_id;
                    Session["adm_campus"] = adm.campus;
                    Session["adm_name"] = adm.adm_name;
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception e) {
                ViewBag.exception_message = e.ToString();
                ViewBag.stack_trace = e.StackTrace;
                return View("About");
            }
        }

        [HttpGet]   
        public ActionResult view_job_posts()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                string campus = Session["adm_campus"].ToString();
                List<job_post> jobs_list = db.job_post.Where(x=>x.campus==campus).ToList();
                jobs_list.Reverse();
                ViewBag.jobs_list = jobs_list;
                return View("view_job_posts");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [Route("home/view_job_posts/{id}")]
        public ActionResult Details(int? id)
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
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult Login()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                ViewBag.email = Session["email"];
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                return View();
            }          
        }

        [HttpGet]
        public ActionResult logout()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {
                Session.Remove("adm_email");
                Session.Remove("adm_id");
                Session.Remove("adm_name");
                return RedirectToAction("Login","Home");
            }
            else
            {
                return RedirectToAction("Login","Home");
            }
        }

        public ActionResult RejectedAlumni()
        {
            if (Session["adm_email"] != null && Session["adm_id"] != null)
            {             
                ViewBag.Message = "Your Rejected Alumni page.";
                dy.rejectedMembersCount = getRejectedMembers().Count();
                dy.rejected_members_list = getRejectedMembers();
                return View(dy);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }        
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            try
            {
                if (Session["adm_email"] != null && Session["adm_id"] != null)
                {
                    dy.validatedMembersCount = getValidatedMembers().Count();
                    dy.unValidatedMembersCount = getUnValidatedMembers().Count();
                    dy.rejectedMembersCount = getRejectedMembers().Count();
                    dy.validatedMembersList = getValidatedMembers();
                    dy.un_validated_members_list = getUnValidatedMembers();
                    dy.rejected_members_list = getRejectedMembers();
                    return View(dy);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
             catch (Exception e) {
                ViewBag.exception_message = e.ToString();
                ViewBag.stack_trace = e.StackTrace;
                return View("About");
            }
                
        }

        public List<member> getMembers()
        {
            List<member> list_members = db.members.OrderByDescending(m => m.created_at).ToList();
            return list_members;
        }

        public List<member> getValidatedMembers()
        {
            string campus = Session["adm_campus"].ToString();
            List<member> validated_members_list = db.members.Where(x => x.validation_status == 1 && x.campus == campus ).OrderByDescending(m => m.updated_at).ToList();
            return validated_members_list;
        }

        public List<member> getUnValidatedMembers()
        {
            string campus = Session["adm_campus"].ToString();
            List<member> un_validated_members_list = db.members.Where(x => x.validation_status == 2 && x.campus == campus ).OrderByDescending(m => m.created_at).ToList();
            return un_validated_members_list;
        }

        public List<member> getRejectedMembers()
        {
            string campus = Session["adm_campus"].ToString();
            List<member> rejected_members_list = db.members.Where(x => x.validation_status == 3 && x.campus == campus ).OrderByDescending(m => m.updated_at).ToList();
            return rejected_members_list;
        }

        public List<upcoming_event> getEventsList()
        {
            List<upcoming_event> eventsList = db.upcoming_event.OrderByDescending(m => m.created_at).ToList();
            return eventsList;
        }

    }

}