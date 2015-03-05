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
                                 && r.UserID == CurrentUser.ID
                                 orderby r.Time descending
                                 select r).FirstOrDefault());
            ViewBag.LastWeek = ((from r in DB.Reports
                             where r.Type == ReportType.Week
                             && r.UserID == CurrentUser.ID
                                 orderby r.Time descending
                             select r).FirstOrDefault());
            ViewBag.LastMonth = ((from r in DB.Reports
                             where r.Type == ReportType.Month
                             && r.UserID == CurrentUser.ID
                                  orderby r.Time descending
                             select r).FirstOrDefault());
            var ThisWeek = Helpers.Time.WeekOfYear(DateTime.Now);
            ViewBag.Day = (from r in DB.Reports
                           where r.Type == ReportType.Day
                           && r.Year == DateTime.Now.Year
                           && r.Month == DateTime.Now.Month
                           && r.Day == DateTime.Now.Day
                           && r.UserID == CurrentUser.ID
                           select r).Count() > 0;
            ViewBag.Week = (from r in DB.Reports
                           where r.Type == ReportType.Week
                           && r.Year == DateTime.Now.Year
                           && r.Week == ThisWeek
                           && r.UserID == CurrentUser.ID
                            select r).Count() > 0;
            ViewBag.Month = (from r in DB.Reports
                           where r.Type == ReportType.Month
                           && r.Year == DateTime.Now.Year
                           && r.Month == DateTime.Now.Month
                           && r.UserID == CurrentUser.ID
                             select r).Count() > 0;
            ViewBag.Enterprises = (from e in DB.Enterprises
                                   select e).ToList();
            return View();
        }
    }
}