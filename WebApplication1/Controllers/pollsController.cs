using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Dynamic;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Xml.Linq;
using System.Web.Caching;
using System.Web.Services.Description;

namespace WebApplication1.Controllers
{
    public class pollsController : Controller
    {
        FUI_POLLEntities ent = new FUI_POLLEntities();
        dynamic dy = new ExpandoObject();
        // GET: Multiple
        public ActionResult Index(Guid? id)
        {
            try {
                dy.candidates = getCandidates();
                DateTime current_date_time = DateTime.Now;
                if (id != null && ent.members.Count(e => e.guid == id )>0)
                {
                    dy.member = getMember((Guid)id);
                    if ((ent.members.Count(e => e.guid == id && (e.already_voted == null)) > 0))
                    {
                        if ((ent.elections.Count(e => e.start_date_time > current_date_time) > 0)) {
                          
                            TempData["start_date_time"] = ent.elections.Select(u => u.start_date_time).SingleOrDefault();
                            return RedirectToAction("NotStarted");
                        }
                        else if ((ent.elections.Count(e => e.end_date_time < current_date_time) > 0))
                            {
                                return RedirectToAction("Closed");
                            }
                            else {
                            return View(dy);
                        }
                    }
                        
                 
                    else
                        return RedirectToAction("AlreadyVoted", new { id = id });
                }
                else
                {
                    return RedirectToAction("InvalidUrl");
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("InvalidUrl");
            }         
        }

        public ActionResult InvalidUrl()
        {
            return View();
        }

        public ActionResult Closed()
        {
            return View();
        }

        public ActionResult NotStarted()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult SuccessfullyVoted()
        {
            return View();
        }

        public ActionResult AlreadyVoted(Guid id)
        {
            dy.member = getMember(id);
            return View(dy);
        }


        public List<candidate> getCandidates()
        {
            List<candidate> list_candidates = ent.candidates.ToList();
            return list_candidates;
        }

        public List<member> getMember(Guid id)
        {
            var result = ent.members.SqlQuery("EXEC get_members_sp @guid",
             new SqlParameter("@guid", id)).ToList();
             return result;
        }

        [ResponseType(typeof(void))]
        public ActionResult PutMember(Guid id, FormCollection formCollection)
        {
            try
            {
                if ((ent.members.Count(e => e.guid == id) > 0))
                {
                    if ((ent.members.Count(e => e.guid == id && (e.already_voted == null )) > 0))
                    {
                        var headers = Request.Headers.ToString();
                        string ip_address = Request.UserHostAddress;
                        for (int i = 1; i < (formCollection.Count); i++)
                        {
                            string category_name = formCollection.GetKey(i);
                            int candidate_id = Int16.Parse(formCollection[category_name]);
                            ent.update_member_candidate_sp(id, candidate_id, ip_address, headers);
                        }

                        return RedirectToAction("SuccessfullyVoted");
                    }
                    return RedirectToAction("AlreadyVoted");
                }
                else
                {
                    return RedirectToAction("InvalidUrl");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("InvalidUrl");
            }
        }
    }
}