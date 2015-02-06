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

        [HttpPost]
        [ValidateSID]
        public ActionResult Create(AlarmModel Model)
        {
            Model.ID = Guid.NewGuid();
            Model.UserID = CurrentUser.ID;
            DB.Alarms.Add(Model);
            DB.SaveChanges();
            return RedirectToAction("Index", "Alarm");
        }

        [HttpPost]
        [ValidateSID]
        [AccessToAlarm]
        public ActionResult Delete(Guid id)
        {
            var alarm = DB.Alarms.Find(id);
            DB.Alarms.Remove(alarm);
            DB.SaveChanges();
            return RedirectToAction("Index", "Alarm");
        }

        [HttpPost]
        [ValidateSID]
        [AccessToAlarm]
        public ActionResult Edit(Guid id, AlarmModel Model)
        {
            var alarm = DB.Alarms.Find(id);
            alarm.Begin = Model.Begin;
            alarm.End = Model.End;
            alarm.Hint = Model.Hint;
            alarm.Remind = Model.Remind;
            DB.SaveChanges();
            return RedirectToAction("Index", "Alarm");
        }
    }
}