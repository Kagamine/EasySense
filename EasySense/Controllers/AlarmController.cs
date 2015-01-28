using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySense.Controllers
{
    [Authorize]
    public class AlarmController : BaseController
    {
        // GET: Alarm
        public ActionResult Index()
        {
            return View();
        }
    }
}