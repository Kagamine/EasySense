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
    public class StatisticsController : BaseController
    {
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Create(StatisticsModel Model)
        {
            Model.ID = Guid.NewGuid();
            Model.Time = DateTime.Now;
            DB.Statistics.Add(Model);
            DB.SaveChanges();
            return RedirectToAction("Show", "Statistics", new { id = Model.ID });
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Edit(Guid id, StatisticsModel Model)
        {
            var statistics = DB.Statistics.Find(id);
            statistics.Begin = Model.Begin;
            statistics.End = Model.End;
            statistics.Title = Model.Title;
            statistics.Hint = Model.Hint;
            statistics.UserID = Model.UserID;
            statistics.PushTo = Model.PushTo;
            DB.SaveChanges();
            return RedirectToAction("Show", "Statistics", new { id = Model.ID });
        }

        [HttpGet]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Delete(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            DB.Statistics.Remove(statistics);
            DB.SaveChanges();
            return RedirectToAction("Index", "Statistics");
        }
    }
}