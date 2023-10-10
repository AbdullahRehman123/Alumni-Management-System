using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class Error : Controller
    {
        public ActionResult Index()
        {
            ViewBag["exception_message"] = TempData["exception_message"];
            return View();
        }
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View();
        }
    }

}