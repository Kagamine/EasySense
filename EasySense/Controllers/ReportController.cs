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
            return View();
        }

        [AccessToReport]
        public ActionResult Week(int id)
        {
            return View();
        }

        [AccessToReport]
        public ActionResult Month(int id)
        {
            return View();
        }

        public ActionResult New()
        {
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
            DB.Reports.Add(Model);
            DB.SaveChanges();
            if (Model.Type == ReportType.Day)
                return RedirectToAction("Day", "Report", new { id = CurrentUser.ID });
            else if (Model.Type == ReportType.Month)
                return RedirectToAction("Month", "Report", new { id = CurrentUser.ID });
            else
                return RedirectToAction("Week", "Report", new { id = CurrentUser.ID });
        }

        [HttpGet]
        public ActionResult GetWeeks(int id)
        {
            return Content(Helpers.Time.WeekCountOfYear(id).ToString());
        }
    }
}