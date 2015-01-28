using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySense.Controllers
{
    [Authorize]
    public class AvatarController : BaseController
    {
        // GET: Avatar
        public ActionResult Index()
        {
            return View();
        }
    }
}