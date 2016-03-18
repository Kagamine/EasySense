using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;
using System.Configuration;

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
            DB.SaveChanges();
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

            ViewBag.products = (from e in DB.Products select e).ToList();

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
            try
            {
                DB.Categories.Remove(category);
                DB.SaveChanges();
                return Content("OK");
            }
            catch
            {
                return Content("cannot delete");
            }
            //return RedirectToAction("Category", "System");
        }

        [HttpGet]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult DeleteDepartment(int id)
        {
            var department = DB.Departments.Find(id);
            DB.Departments.Remove(department);
            DB.SaveChanges();
            return RedirectToAction("Department", "System");
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult DeleteProduct(int id)
        {
            var product = DB.Products.Find(id);
            try
            {
                DB.Products.Remove(product);
                DB.SaveChanges();
                return Content("OK");
            }
            catch
            {
                return Content("cannot delete");
            }
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

        [MinRole(UserRole.Root)]
        public ActionResult Department()
        {
            var departments = (from d in DB.Departments
                               orderby d.ID descending
                               select d).ToList();
            ViewBag.Users = DB.Users.OrderBy(x => x.Role).ToList();
            return View(departments);
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult CreateDepartment(DepartmentModel Model)
        {
            if (Model.UserID != null)
            {
                var user = DB.Users.Find(Model.UserID);
                if (user.Role < UserRole.Master)
                    user.Role = UserRole.Master;
            }
            DB.Departments.Add(Model);
            DB.SaveChanges();
            return RedirectToAction("Department", "System");
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult EditDepartment(int id, string Title, int? UserID)
        {
            var department = DB.Departments.Find(id);
            department.Title = Title;
            department.UserID = UserID;
            if (UserID.HasValue)
            {
                var user = DB.Users.Find(UserID.Value);
                if (user.Role < UserRole.Master)
                    user.Role = UserRole.Master;
            }
            DB.SaveChanges();
            return RedirectToAction("Department", "System");
        }

        [HttpGet]
        [MinRole(UserRole.Root)]
        public ActionResult GetDepartment(int id)
        {
            var department = DB.Departments.Find(id);
            return Json((DepartmentViewModel)department, JsonRequestBehavior.AllowGet);
        }

        [MinRole(UserRole.Root)]
        public ActionResult Field()
        {
            ViewBag.Config = Startup.Config;
            return View();
        }

        [HttpPost]
        [MinRole(UserRole.Root)]
        public ActionResult Field(bool Title, bool Description, bool Begin,
            bool End, bool SignTime, bool Charge, bool InvoicePrice,
            bool InvoiceSN, bool Hint, bool ChargeTime, bool ActualPayments,
            bool InvoiceTime, bool Priority, bool Status, bool EnterpriseName,
            bool CustomerName, bool Tel, bool Phone, bool Email, bool Brand,
            bool Ordering, bool CategoryID, bool ProductID, bool ZoneID,
            bool PayMethod)
        {



            Startup.Config["Title"] = Title.ToString();

            Startup.Config["Description"] = Description.ToString();

            Startup.Config["Begin"] = Begin.ToString();

            Startup.Config["End"] = End.ToString();

            Startup.Config["SignTime"] = SignTime.ToString();

            Startup.Config["Charge"] = Charge.ToString();

            Startup.Config["InvoicePrice"] = InvoicePrice.ToString();

            Startup.Config["InvoiceSN"] = InvoiceSN.ToString();

            Startup.Config["Hint"] = Hint.ToString();

            Startup.Config["ChargeTime"] = ChargeTime.ToString();

            Startup.Config["ActualPayments"] = ActualPayments.ToString();

            Startup.Config["InvoiceTime"] = InvoiceTime.ToString();

            Startup.Config["Priority"] = Priority.ToString();

            Startup.Config["Status"] = Status.ToString();

            Startup.Config["EnterpriseName"] = EnterpriseName.ToString();

            Startup.Config["CustomerName"] = CustomerName.ToString();

            Startup.Config["Tel"] = Tel.ToString();

            Startup.Config["Phone"] = Phone.ToString();

            Startup.Config["Email"] = Email.ToString();

            Startup.Config["Brand"] = Brand.ToString();

            Startup.Config["Ordering"] = Ordering.ToString();

            Startup.Config["CategoryID"] = CategoryID.ToString();

            Startup.Config["ProductID"] = ProductID.ToString();

            Startup.Config["ZoneID"] = ZoneID.ToString();

            Startup.Config["PayMethod"] = PayMethod.ToString();

            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/config.json", Newtonsoft.Json.JsonConvert.SerializeObject(Startup.Config));

            return RedirectToAction("Field", "System");
        }
    }
}