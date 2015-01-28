using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySense.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }
    }
}