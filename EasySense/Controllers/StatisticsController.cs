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
            IEnumerable<StatisticsModel> statistics = DB.Statistics;
            if (CurrentUser.Role == UserRole.Finance)
                statistics = statistics.Where(x => x.PushTo == UserRole.Finance || x.PushTo == null);
            else if (CurrentUser.Role == UserRole.Finance)
                statistics = statistics.Where(x => x.PushTo == UserRole.Master || x.PushTo == null);
            else if (CurrentUser.Role == UserRole.Employee)
                statistics = statistics.Where(x => x.PushTo == null);
            statistics = statistics.OrderByDescending(x => x.Time).ToList();
            ViewBag.Users = DB.Users.ToList();
            return View(statistics);
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
            IEnumerable<ProjectModel> Projects = DB.Projects;
            if (Model.Begin != null)
                Projects = Projects.Where(x => x.SignTime >= Model.Begin.Value);
            if (Model.End != null)
                Projects = Projects.Where(x => x.SignTime <= Model.End.Value);
            if (Model.Status != null)
                Projects = Projects.Where(x => x.Status == Model.Status.Value);
            if (Model.UserID != null)
                Projects = Projects.Where(x => x.UserID == Model.UserID.Value);
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

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Delete(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            DB.Statistics.Remove(statistics);
            DB.SaveChanges();
            return Content("OK");
        }

        [AccessToStatistics]
        public ActionResult Show(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return View(statistics);
        }
    }
}