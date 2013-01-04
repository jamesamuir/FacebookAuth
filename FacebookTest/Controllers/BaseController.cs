using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacebookTest.Security;
using System.Web.Mvc;

namespace FacebookTest.Controllers
{
    public class BaseController : Controller
    {
        protected virtual new AccountPrincipal User
        {
            get { return HttpContext.User as AccountPrincipal; }
        }
    }
}