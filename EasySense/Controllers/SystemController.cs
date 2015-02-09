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

        [MinRole(UserRole.Root)]
        [HttpGet]
        public ActionResult GetProducts(int id)
        {
            var category = DB.Categories.Find(id);
            var ret = new List<ProductViewModel>();
            foreach (var c in category.Products)
                ret.Add((ProductViewModel)c);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult CreateCategory(CategoryModel Model)
        {
            DB.Categories.Add(Model);
            DB.SaveChanges();
            return RedirectToAction("Category", "System");
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult CreateProduct(ProductModel Model)
        {
            DB.Products.Add(Model);
            DB.SaveChanges();
            return RedirectToAction("Category", "System");
        }

        [HttpGet]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult DeleteCategory(int id)
        {
            var category = DB.Categories.Find(id);
            DB.Categories.Remove(category);
            DB.SaveChanges();
            return RedirectToAction("Category", "System");
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult DeleteProduct(int id)
        {
            var product = DB.Products.Find(id);
            DB.Products.Remove(product);
            DB.SaveChanges();
            return Content("OK");
        }


        [HttpGet]
        [MinRole(UserRole.Root)]
        public ActionResult GetCategory(int id)
        {
            var category = DB.Categories.Find(id);
            return Json((CategoryViewModel)category, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult EditCategory(int id, CategoryModel Model)
        {
            var category = DB.Categories.Find(id);
            category.Title = Model.Title;
            category.AwardAllocRatio = Model.AwardAllocRatio;
            category.SaleAllocRatio = Model.SaleAllocRatio;
            category.TaxRatio = Model.TaxRatio;
            DB.SaveChanges();
            return RedirectToAction("Category", "System");
        }
    }
}