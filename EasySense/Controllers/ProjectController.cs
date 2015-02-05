using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;
using EasySense.Schema;
using System.Data.Entity.Validation;

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

        [HttpGet]
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
    }
}