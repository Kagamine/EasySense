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
            return RedirectToAction("Day", "Report", new { id = CurrentUser.ID });
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
            ViewBag.ThisWeek = Helpers.Time.WeekOfYear(DateTime.Now);
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
            IEnumerable<ReportModel> reports = DB.Reports.Where(x => x.UserID == id && x.Year == year);
            if (month.HasValue)
                reports = reports.Where(x => x.Month == month.Value);
            if (week.HasValue)
                reports = reports.Where(x => x.Week >= week.Value - 2 && x.Week <= week.Value + 2);
            reports = reports.Where(x => x.Type == Type);
            var ret = new List<ReportViewModel>();
            foreach (var r in reports.ToList())
                ret.Add((ReportViewModel)r);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult New()
        {
            ViewBag.ThisWeek = Helpers.Time.WeekOfYear(DateTime.Now);
            ViewBag.ID = CurrentUser.ID;
            return View();
        }

        [HttpPost]
        [ValidateSID]
        [ValidateInput(false)]
        public ActionResult New(ReportModel Model)
        {
            Model.Year = DateTime.Now.Year;
            if (Model.Type == ReportType.Day)
            {
                DateTime date = Helpers.String.ToDateTime(Model.Date0, "yyyy/MM/dd");
                Model.Year = date.Year;
                Model.Month = date.Month;
                Model.Day = date.Day;
            }
            else if (Model.Type == ReportType.Month)
            {
                Model.Month = Model.Month0;
            }
            else
            {
                if (Model.Week0 == 1)
                {
                    Model.Week = Helpers.Time.WeekOfYear(DateTime.Now);
                }
                else if (Model.Week0 == 2)
                {
                    Model.Week = Helpers.Time.WeekOfYear(DateTime.Now) + 1;
                }
                else if (Model.Week0 == 3)
                {
                    Model.Week = Helpers.Time.WeekOfYear(DateTime.Now) + 2;
                }
            }
            Model.Time = DateTime.Now;
            Model.UserID = CurrentUser.ID;
            var cnt = (from r in DB.Reports
                       where r.Type == Model.Type
                       && r.Year == Model.Year
                       && r.Month == Model.Month
                       && r.Week == Model.Week
                       && r.Day == Model.Day
                       && r.UserID == CurrentUser.ID
                       select r).Count();
            if (cnt > 0) return RedirectToAction("Message", "Shared", new { msg = "请勿重复创建报告。" });
            Model.TodoList = Helpers.HtmlFilter.Instance.SanitizeHtml(Model.TodoList);
            Model.QuestionList = Helpers.HtmlFilter.Instance.SanitizeHtml(Model.QuestionList);
            Model.FinishedList = Helpers.HtmlFilter.Instance.SanitizeHtml(Model.FinishedList);
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
        [ValidateInput(false)]
        public ActionResult Edit(int id, string TodoList, string FinishedList, string QuestionList)
        {
            var report = DB.Reports.Find(id);
            if (report.UserID != CurrentUser.ID && CurrentUser.Role != UserRole.Root)
                return RedirectToAction("AccessDenied", "Shared");
            ViewBag.ID = report.ID;
            report.TodoList = Helpers.HtmlFilter.Instance.SanitizeHtml(TodoList);
            report.FinishedList = Helpers.HtmlFilter.Instance.SanitizeHtml(FinishedList);
            report.QuestionList = Helpers.HtmlFilter.Instance.SanitizeHtml(QuestionList);
            DB.SaveChanges();
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