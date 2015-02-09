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
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index(int? id)
        {
            ViewBag.ID = id.HasValue ? id.Value : CurrentUser.ID;
            if (CurrentUser.Role < UserRole.Root)
                return RedirectToAction("Day", "Report", new { id = CurrentUser.ID });
            else
                return View();
        }

        [AccessToReport]
        public ActionResult Day(int id)
        {
            ViewBag.ID = CurrentUser.ID;
            return View();
        }

        [AccessToReport]
        public ActionResult Week(int id)
        {
            ViewBag.ID = CurrentUser.ID;
            return View();
        }

        [AccessToReport]
        public ActionResult Month(int id)
        {
            ViewBag.ID = CurrentUser.ID;
            return View();
        }

        [HttpGet]
        [AccessToReport]
        public ActionResult GetReports(int id, ReportType Type,int year, int? month, int? week)
        {
            IEnumerable<ReportModel> reports = DB.Reports.Where(x => x.ID == id && x.Year == year);
            if (month.HasValue)
                reports = reports.Where(x => x.Month == month.Value);
            if (week.HasValue)
                reports = reports.Where(x => x.Week == week.Value);
            reports = reports.Where(x => x.Type == Type);
            var ret = new List<ReportViewModel>();
            foreach (var r in reports.ToList())
                ret.Add((ReportViewModel)r);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult New()
        {
            ViewBag.ID = CurrentUser.ID;
            return View();
        }

        [HttpPost]
        [ValidateSID]
        public ActionResult New(ReportModel Model)
        {
            Model.Year = DateTime.Now.Year;
            if (Model.Type == ReportType.Day)
            {
                Model.Month = DateTime.Now.Month;
                Model.Day = DateTime.Now.Day;
            }
            else if (Model.Type == ReportType.Month)
            {
                Model.Month = DateTime.Now.Month;
            }
            else
            {
                Model.Week = Helpers.Time.WeekOfYear(DateTime.Now);
            }
            Model.Time = DateTime.Now;
            Model.UserID = CurrentUser.ID;
            var cnt = (from r in DB.Reports
                       where r.Type == Model.Type
                       && r.Year == Model.Year
                       && r.Month == Model.Month
                       && r.Week == Model.Week
                       && r.Day == Model.Day
                       select r).Count();
            if (cnt > 0) return RedirectToAction("Message", "Shared", new { msg = "请勿重复创建报告。" });
            DB.Reports.Add(Model);
            DB.SaveChanges();
            if (Model.Type == ReportType.Day)
                return RedirectToAction("Day", "Report", new { id = CurrentUser.ID });
            else if (Model.Type == ReportType.Month)
                return RedirectToAction("Month", "Report", new { id = CurrentUser.ID });
            else
                return RedirectToAction("Week", "Report", new { id = CurrentUser.ID });
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ID = CurrentUser.ID;
            var report = DB.Reports.Find(id);
            if (report.UserID != CurrentUser.ID && CurrentUser.Role != UserRole.Root)
                return RedirectToAction("AccessDenied", "Shared");
            return View(report);
        }

        [HttpPost]
        [ValidateSID]
        public ActionResult Edit(int id, string TodoList, string FinishedList, string QuestionList)
        {
            var report = DB.Reports.Find(id);
            if (report.UserID != CurrentUser.ID && CurrentUser.Role != UserRole.Root)
                return RedirectToAction("AccessDenied", "Shared");
            report.TodoList = TodoList;
            report.FinishedList = FinishedList;
            report.QuestionList = QuestionList;
            if (report.Type == ReportType.Day)
                return RedirectToAction("Day", "Report", new { id = report.UserID });
            else if (report.Type == ReportType.Month)
                return RedirectToAction("Month", "Report", new { id = report.UserID });
            else
                return RedirectToAction("Week", "Report", new { id = report.UserID });
        }

        [HttpGet]
        public ActionResult GetWeeks(int id)
        {
            return Content(Helpers.Time.WeekCountOfYear(id).ToString());
        }
    }
}