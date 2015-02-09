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
                RedirectToAction("Day", "Report", new { id = CurrentUser.ID });
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
        public ActionResult New()
        {
            
        }

        [HttpGet]
        public ActionResult GetWeeks(int id)
        {
            return Content(Helpers.Time.WeekCountOfYear(id));
        }
    }
}