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
            ViewBag.Users = DB.Users.OrderBy(x=>x.Role).ToList();
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
            ViewBag.Config = ConfigurationManager.AppSettings;
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
            ConfigurationManager.AppSettings["Title"] = Title.ToString();
            ConfigurationManager.AppSettings["Description"] = Description.ToString();
            ConfigurationManager.AppSettings["Begin"] = Begin.ToString();
            ConfigurationManager.AppSettings["End"] = End.ToString();
            ConfigurationManager.AppSettings["SignTime"] = SignTime.ToString();
            ConfigurationManager.AppSettings["Charge"] = Charge.ToString();
            ConfigurationManager.AppSettings["InvoicePrice"] = InvoicePrice.ToString();
            ConfigurationManager.AppSettings["InvoiceSN"] = InvoiceSN.ToString();
            ConfigurationManager.AppSettings["Hint"] = Hint.ToString();
            ConfigurationManager.AppSettings["ChargeTime"] = ChargeTime.ToString();
            ConfigurationManager.AppSettings["ActualPayments"] = ActualPayments.ToString();
            ConfigurationManager.AppSettings["InvoiceTime"] = InvoiceTime.ToString();
            ConfigurationManager.AppSettings["Priority"] = Priority.ToString();
            ConfigurationManager.AppSettings["Status"] = Status.ToString();
            ConfigurationManager.AppSettings["EnterpriseName"] = EnterpriseName.ToString();
            ConfigurationManager.AppSettings["CustomerName"] = CustomerName.ToString();
            ConfigurationManager.AppSettings["Tel"] = Tel.ToString();
            ConfigurationManager.AppSettings["Phone"] = Phone.ToString();
            ConfigurationManager.AppSettings["Email"] = Email.ToString();
            ConfigurationManager.AppSettings["Brand"] = Brand.ToString();
            ConfigurationManager.AppSettings["Ordering"] = Ordering.ToString();
            ConfigurationManager.AppSettings["CategoryID"] = CategoryID.ToString();
            ConfigurationManager.AppSettings["ProductID"] = ProductID.ToString();
            ConfigurationManager.AppSettings["ZoneID"] = ZoneID.ToString();
            ConfigurationManager.AppSettings["PayMethod"] = PayMethod.ToString();
            return RedirectToAction("Field", "System");
        }
    }
}