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
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Yesterday = ((from r in DB.Reports
                                 where r.Type == ReportType.Day
                                 orderby r.Time descending
                                 select r).FirstOrDefault());
            ViewBag.LastWeek = ((from r in DB.Reports
                             where r.Type == ReportType.Week
                             orderby r.Time descending
                             select r).FirstOrDefault());
            ViewBag.LastMonth = ((from r in DB.Reports
                             where r.Type == ReportType.Month
                             orderby r.Time descending
                             select r).FirstOrDefault());
            var ThisWeek = Helpers.Time.WeekOfYear(DateTime.Now);
            ViewBag.Day = (from r in DB.Reports
                           where r.Type == ReportType.Day
                           && r.Year == DateTime.Now.Year
                           && r.Month == DateTime.Now.Month
                           && r.Day == DateTime.Now.Day
                           select r).Count() > 0;
            ViewBag.Week = (from r in DB.Reports
                           where r.Type == ReportType.Week
                           && r.Year == DateTime.Now.Year
                           && r.Week == ThisWeek
                           select r).Count() > 0;
            ViewBag.Month = (from r in DB.Reports
                           where r.Type == ReportType.Month
                           && r.Year == DateTime.Now.Year
                           && r.Month == DateTime.Now.Month
                           select r).Count() > 0;
            return View();
        }
    }
}