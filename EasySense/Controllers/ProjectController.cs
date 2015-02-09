using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using EasySense.Models;
using EasySense.Schema;
using EO.Pdf;

namespace EasySense.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        [AccessToProject]
        public ActionResult Show(int id)
        {
            var project = DB.Projects.Find(id);
            ViewBag.Zones = (from z in DB.Zones
                             orderby z.ID descending
                             select z).ToList();
            ViewBag.Categories = (from c in DB.Categories
                                  orderby c.ID descending
                                  select c).ToList();
            var enterprises = (from e in DB.Enterprises
                               orderby e.ID descending
                               select e).Take(5).ToList();
            ViewBag.Enterprises = new List<EnterpriseListViewModel>();
            foreach (var e in enterprises)
                ViewBag.Enterprises.Add((EnterpriseListViewModel)e);
            return View(project);
        }

        [UserOwnedProject]
        [HttpPost]
        [ValidateSID]
        public ActionResult Edit(int id, ProjectModel Model)
        {
            var project = DB.Projects.Find(id);
            project.ZoneID = Model.ZoneID;
            project.ActualPayments = Model.ActualPayments;
            project.Begin = Model.Begin;
            project.End = Model.End;
            project.EnterpriseID = Model.EnterpriseID;
            project.Hint = Model.Hint;
            project.Description = Model.Description;
            project.InvoicePrice = Model.InvoicePrice;
            project.InvoiceSN = Model.InvoiceSN;
            project.Ordering = Model.Ordering;
            project.Priority = Model.Priority;
            project.Charge = Model.Charge;
            project.ChargeTime = Model.ChargeTime;
            project.SignTime = Model.SignTime;
            project.InvoicePrice = Model.InvoicePrice;
            project.CustomerID = Model.CustomerID;
            project.Title = Model.Title;
            project.Status = Model.Status;
            project.PayMethod = Model.PayMethod;
            if (project.ProductID != Model.ProductID)
            {
                if (Model.ProductID != null)
                {
                    project.ProductID = Model.ProductID;
                    var product = DB.Products.Find(Model.ProductID);
                    project.AwardAllocRatioCache = product.Category.AwardAllocRatio;
                    project.ProfitAllocRatioCache = product.Category.ProfitAllocRatio;
                    project.SaleAllocRatioCache = product.Category.SaleAllocRatio;
                    project.TaxRatioCache = product.Category.TaxRatio;
                }
                else
                {
                    project.AwardAllocRatioCache = null;
                    project.ProfitAllocRatioCache = null;
                    project.SaleAllocRatioCache = null;
                    project.TaxRatioCache = null;
                }
            }
            project.Log = string.Format("[{0}] {1}({2}) 修改了项目\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Name, CurrentUser.Username);
            DB.SaveChanges();
            return RedirectToAction("Show", "Project", new { id = id });
        }

        [HttpGet]
        public ActionResult Search(int Page, string Title, ProjectStatus? Status, DateTime? Begin, DateTime? End, string OrderBy, string Order)
        {
            IEnumerable<ProjectModel> projects = (from p in DB.Projects
                                                  where p.Title.Contains(Title)
                                                  select p);
            if (Status.HasValue)
                projects = projects.Where(x => x.Status == Status.Value);
            if (Begin.HasValue)
                projects = projects.Where(x => x.Begin <= Begin.Value);
            if (End.HasValue)
                projects = projects.Where(x => x.End <= End.Value);
            if (CurrentUser.Role == UserRole.Employee)
                projects = projects.Where(x => x.UserID == CurrentUser.ID);
            else if (CurrentUser.Role == UserRole.Master)
                projects = projects.Where(x => x.User.Department.UserID == CurrentUser.ID);
            if (string.IsNullOrEmpty(OrderBy))
            {
                projects = projects.OrderByDescending(x => x.Status).ThenByDescending(x=>x.Priority);
            }
            else
            {
                if (Order == "asc")
                {
                    if (OrderBy == "ID")
                        projects = projects.OrderBy(x => x.ID);
                    else if (OrderBy == "UserID")
                        projects = projects.OrderBy(x => x.UserID);
                    else if (OrderBy == "Title")
                        projects = projects.OrderBy(x => x.Title);
                    else if (OrderBy == "Charge")
                        projects = projects.OrderBy(x => x.Charge);
                    else if (OrderBy == "SignTime")
                        projects = projects.OrderBy(x => x.SignTime);
                    else if (OrderBy == "ProductID")
                        projects = projects.OrderBy(x => x.ProductID);
                    else if (OrderBy == "EnterpriseID")
                        projects = projects.OrderBy(x => x.EnterpriseID);
                    else if (OrderBy == "Enterprise.Brand")
                        projects = projects.OrderBy(x => x.EnterpriseID);
                    else if (OrderBy == "CustomerID")
                        projects = projects.OrderBy(x => x.CustomerID);
                    else if (OrderBy == "Status")
                        projects = projects.OrderBy(x => x.Status);
                    else if (OrderBy == "InvoiceTime")
                        projects = projects.OrderBy(x => x.InvoiceTime);
                    else
                        projects = projects.OrderBy(x => x.ChargeTime);
                }
                else
                {
                    if (OrderBy == "ID")
                        projects = projects.OrderByDescending(x => x.ID);
                    else if (OrderBy == "UserID")
                        projects = projects.OrderByDescending(x => x.UserID);
                    else if (OrderBy == "Title")
                        projects = projects.OrderByDescending(x => x.Title);
                    else if (OrderBy == "Charge")
                        projects = projects.OrderByDescending(x => x.Charge);
                    else if (OrderBy == "SignTime")
                        projects = projects.OrderByDescending(x => x.SignTime);
                    else if (OrderBy == "ProductID")
                        projects = projects.OrderByDescending(x => x.ProductID);
                    else if (OrderBy == "EnterpriseID")
                        projects = projects.OrderByDescending(x => x.EnterpriseID);
                    else if (OrderBy == "Enterprise.Brand")
                        projects = projects.OrderByDescending(x => x.EnterpriseID);
                    else if (OrderBy == "CustomerID")
                        projects = projects.OrderByDescending(x => x.CustomerID);
                    else if (OrderBy == "Status")
                        projects = projects.OrderByDescending(x => x.Status);
                    else if (OrderBy == "InvoiceTime")
                        projects = projects.OrderByDescending(x => x.InvoiceTime);
                    else
                        projects = projects.OrderByDescending(x => x.ChargeTime);
                }
            }
            projects = projects.Skip(20 * Page).Take(20).ToList();
            var ret = new List<ProjectListViewModel>();
            foreach (var p in projects)
                ret.Add((ProjectListViewModel)p);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateSID]
        public ActionResult Create(ProjectModel Model)
        {
            Model.Log = DateTime.Now + " 被创建.\r\n";
            Model.Hint = "无内容";
            Model.UserID = CurrentUser.ID;
            Model.Ordering = false;
            Model.Percent = 0;
            Model.PayMethod = PayMethod.Unpaid;
            DB.Projects.Add(Model);
            DB.SaveChanges();
            return Content(Model.ID.ToString());
        }

        [HttpGet]
        [ValidateSID]
        public ActionResult Delete(int id)
        {
            var project = DB.Projects.Find(id);
            DB.Projects.Remove(project);
            DB.SaveChanges();
            return RedirectToAction("Index", "Project");
        }

        [MinRole(UserRole.Finance)]
        public ActionResult Bill(int id)
        {
            var project = DB.Projects.Find(id);
            var bills = new List<BillViewModel>();
            foreach (var b in project.Bills)
                bills.Add((BillViewModel)b);
            ViewBag.Bills = bills;
            return View(project);
        }

        [AccessToProject]
        public ActionResult Log(int id)
        {
            var project = DB.Projects.Find(id);
            return View(project);
        }

        [MinRole(UserRole.Finance)]
        [HttpPost]
        [ValidateSID]
        public ActionResult AddBill(int id, BillModel Model)
        {
            Model.ProjectID = id;
            Model.ID = Guid.NewGuid();
            Model.Time = DateTime.Now;
            DB.Bills.Add(Model);
            DB.SaveChanges();
            return Content(Model.ID.ToString());
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult EditBill(Guid id, BillModel Model)
        {
            var bill = DB.Bills.Find(id);
            bill.Hint = Model.Hint;
            bill.Type = Model.Type;
            bill.Plan = Model.Plan;
            bill.Actual = Model.Actual;
            DB.SaveChanges();
            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult DeleteBill(Guid id)
        {
            var bill = DB.Bills.Find(id);
            DB.Bills.Remove(bill);
            DB.SaveChanges();
            return Content("OK");
        }

        [MinRole(UserRole.Finance)]
        [HttpGet]
        public ActionResult GetBill(Guid id)
        {
            var bill = DB.Bills.Find(id);
            return Json((BillViewModel)bill, JsonRequestBehavior.AllowGet);
        }

        private string BuildHtmlTable(int id)
        {
            var project = DB.Projects.Find(id);
            var bills = (from b in DB.Bills
                         where b.ProjectID == id
                         orderby b.Time descending
                         select b).ToList();
            var html = "<table style='border: 1px solid #000'><tr><td colspan=\"5\" style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>" +project.Title+ " 支出明细</td></tr><tr><td style='border-bottom:1px solid #000'>支出日期</td><td style='border-bottom:1px solid #000'>支出类型</td><td style='border-bottom:1px solid #000'>说明</td><td style='border-bottom:1px solid #000'>计划经费</td><td style='border-bottom:1px solid #000'>实际经费</td></tr>";
            foreach (var b in bills)
            {
                html += string.Format(
                    "<tr><td style='border-bottom:1px solid #000'>{0}</td style='border-bottom:1px solid #000'><td style='border-bottom:1px solid #000'>{1}</td><td style='border-bottom:1px solid #000'>{2}</td><td style='border-bottom:1px solid #000'>￥{3}</td><td style='border-bottom:1px solid #000'>￥{4}</td></tr>", 
                    b.Time.ToString("yyyy-MM-dd"),
                    ((BillViewModel)b).Type,
                    b.Hint,
                    b.Plan.ToString("0.00"),
                    b.Actual.ToString("0.00")
                );
            }
            html += "</table>";
            return html;
        }

        [MinRole(UserRole.Finance)]
        [HttpGet]
        public ActionResult ExportExcel(int id)
        {
            return File(Helpers.Export.ToExcel(BuildHtmlTable(id)), "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
        }

        [MinRole(UserRole.Finance)]
        [HttpGet]
        public ActionResult ExportPDF(int id)
        {
            return File(Helpers.Export.ToPDF(BuildHtmlTable(id)), "application/pdf", Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
        }

        [HttpGet]
        public ActionResult GetProducts(int id)
        {
            var products = (from p in DB.Products
                            where p.CategoryID == id
                            select p).ToList();
            var ret = new List<ProductViewModel>();
            foreach (var p in products)
                ret.Add((ProductViewModel)p);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessToProject]
        [ValidateSID]
        public ActionResult ChangePercent(int id, float percent)
        {
            var project = DB.Projects.Find(id);
            project.Percent = percent / 100;
            DB.SaveChanges();
            return RedirectToAction("Show", "Project", new { id = id });
        }
    }
}