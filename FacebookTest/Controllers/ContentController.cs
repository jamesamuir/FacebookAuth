using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacebookTest.Controllers
{
    public class ContentController : Controller
    {
        //
        // GET: /Content/EmailSent
        public ActionResult EmailSent()
        {
            return View();
        }
        //
        // GET: /Content/InvalidUrl
        public ActionResult InvalidUrl()
        {
            return View();
        }
        //
        // GET: /Content/Error
        public ActionResult Error()
        {
            return View();
        }
    }
}
