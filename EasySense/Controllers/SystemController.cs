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
    public class SystemController : BaseController
    {
        // GET: System
        [MinRole(UserRole.Root)]
        public ActionResult Index()
        {
            var zones = (from z in DB.Zones
                         orderby z.ID descending
                         select z).ToList();
            return View(zones);
        }

        [HttpGet]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult DeleteZone(int id)
        {
            var zone = DB.Zones.Find(id);
            DB.Zones.Remove(zone);
            return RedirectToAction("Index", "System");
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult RenameZone(int id, string Title)
        {
            var zone = DB.Zones.Find(id);
            zone.Title = Title;
            DB.SaveChanges();
            return RedirectToAction("Index", "System");
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult CreateZone(string Title)
        {
            DB.Zones.Add(new ZoneModel { Title = Title });
            DB.SaveChanges();
            return RedirectToAction("Index", "System");
        }

        [MinRole(UserRole.Root)]
        public ActionResult Category()
        {
            var categories = (from c in DB.Categories
                              orderby c.ID descending
                              select c).ToList();
            return View(categories);
        }
    }
}