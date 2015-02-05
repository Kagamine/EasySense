using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;

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

        [HttpGet]
        public ActionResult Load()
        {
            var alarms = (from a in DB.Alarms
                          where a.UserID == CurrentUser.ID
                          select a).ToList();
            var ret = new List<AlarmGridViewModel>();
            foreach (var a in alarms)
                ret.Add((AlarmGridViewModel)a);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}