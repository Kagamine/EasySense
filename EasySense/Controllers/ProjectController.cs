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
using EasySense.Utils;
using EasySense.Helpers;
using System.Data.Entity.Validation;

namespace EasySense.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        // GET: Project
        public ActionResult Index()
        {
            ViewBag.Enterprises = (from e in DB.Enterprises
                                   select e).ToList();
            ViewBag.Users = (from u in DB.Users
                                   select u).ToList();
            ViewBag.Config = Startup.Config;
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
            ViewBag.Config = Startup.Config;
            return View(project);
        }

        [UserOwnedProject]
        [HttpPost]
        [ValidateSID]
        public ActionResult Edit(int id, ProjectModel Model, string EnterpriseName, string CustomerName)
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
            project.InvoiceTime = Model.InvoiceTime;
            project.Ordering = Model.Ordering;
            project.Priority = Model.Priority;
            project.Charge = Model.Charge;
            project.ChargeTime = Model.ChargeTime;
            project.SignTime = Model.SignTime;
            project.InvoicePrice = Model.InvoicePrice;
            project.CustomerID = Model.CustomerID;
            project.Title = Model.Title;
            project.Status = Model.Status;
            project.Brand = Model.Brand;
            project.Log += string.Format("[{0}] {1}({2}) 修改了项目\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Name, CurrentUser.Username);
            if (Model.Status == ProjectStatus.Completed)
                project.Percent = 1;
            project.PayMethod = Model.PayMethod;
            if (project.ProductID != Model.ProductID)
            {
                if (Model.ProductID != null)
                {
                    project.ProductID = Model.ProductID;
                    var product = DB.Products.Find(Model.ProductID);
                    project.AwardAllocRatioCache = product.Category.AwardAllocRatio;
                    project.SaleAllocRatioCache = product.Category.SaleAllocRatio;
                    project.TaxRatioCache = product.Category.TaxRatio;
                }
                else
                {
                    project.AwardAllocRatioCache = null;
                    project.SaleAllocRatioCache = null;
                    project.TaxRatioCache = null;
                }
            }
            if (!string.IsNullOrEmpty(EnterpriseName))
            {
                if (project.Enterprise == null || project.Enterprise != null && !EnterpriseName.Contains(project.Enterprise.Title) && EnterpriseName != project.Enterprise.Title)
                {
                    var tmp = (from e in DB.Enterprises where e.Title == EnterpriseName select e).ToList();
                    if (tmp.Count > 0)
                    {
                        EnterpriseModel enterprise = (EnterpriseModel) tmp[0];
                        enterprise.Title = EnterpriseName;
                        enterprise.Key = Helpers.Pinyin.Convert(EnterpriseName);
                        enterprise.Phone = (Model.Phone == null ? "" : Model.Phone);
                       // enterprise.Brand = Model.Brand;
                        //
                        project.EnterpriseID = enterprise.ID;
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            throw dbEx;
                        }
                    }
                    else
                    {
                        var e = new EnterpriseModel
                        {
                            Level = EnterpriseLevel.D,
                            Key = Helpers.Pinyin.Convert(EnterpriseName),
                            Title = (EnterpriseName == null ? "" : EnterpriseName),
                            Phone = (Model.Phone == null ? "" : Model.Phone),
                            Address = "Null",
                            SalesVolume = "Null",
                            Scale = "Null",
                            Hint = "Null",
                            Fax = "Null",
                            Property = "Null",
                            Type = "Null",
                            Website = "Null",
                            Zip = "Null"
                        };
                        DB.Enterprises.Add(e);
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            throw dbEx;
                        }
                        project.EnterpriseID = e.ID;
                    }
                }
                else
                {
                    if (project.EnterpriseID != null)
                    {
                        EnterpriseModel enterprise = DB.Enterprises.Find(project.EnterpriseID);
                        enterprise.Title = EnterpriseName;
                        enterprise.Key = Helpers.Pinyin.Convert(EnterpriseName);
                        enterprise.Phone = (Model.Phone == null ? "" : Model.Phone);
                        //enterprise.Brand = Model.Brand;
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            throw dbEx;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(CustomerName))
            {
                if (project.CustomerID == null || project.CustomerID !=null && !CustomerName.Contains(project.Customer.Name) && CustomerName!=project.Customer.Name)
                {
                    var tmp = (from e in DB.Customers where e.Name == CustomerName select e).ToList();
                    if (tmp.Count > 0)
                    {
                        CustomerModel customer = (CustomerModel) tmp[0];
                        customer.Name = CustomerName;
                        customer.Email = (Model.Email == null ? "" : Model.Email);
                        customer.Phone = (Model.Phone == null ? "" : Model.Phone);
                        customer.Tel = (Model.Tel == null ? "" : Model.Tel);
                        //
                        project.CustomerID = customer.ID;
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            throw dbEx;
                        }
                    }
                    else
                    {
                        if (project.EnterpriseID.HasValue)
                        {
                            var c = new CustomerModel
                            {
                                Name = CustomerName,
                                Sex = Sex.Male,
                                Email = (Model.Email == null ? "" : Model.Email),
                                Birthday = DateTime.Now,
                                EnterpriseID = project.EnterpriseID.Value,
                                Hint = "Null",
                                Fax = "Null",
                                Phone = (Model.Phone == null ? "" : Model.Phone),
                                QQ = "Null",
                                Tel = (Model.Tel == null ? "" : Model.Tel),
                                WeChat = "Null",
                                Position = "Null"
                            };
                            DB.Customers.Add(c);
                            try
                            {
                                DB.SaveChanges();
                            }
                            catch (DbEntityValidationException dbEx)
                            {
                                throw dbEx;
                            }
                            project.CustomerID = c.ID;
                        }
                    }
                }
                else
                {
                    if (project.CustomerID != null)
                    {
                        CustomerModel customer = DB.Customers.Find(project.CustomerID);
                        customer.Name = CustomerName;
                        customer.Email = (Model.Email == null ? "" : Model.Email);
                        customer.Phone = (Model.Phone == null ? "" : Model.Phone);
                        customer.Tel = (Model.Tel == null ? "" : Model.Tel);
                        try
                        {
                            DB.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            throw dbEx;
                        }
                    }
                }
            }
            try
            {
                DB.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw dbEx;
            }
            return RedirectToAction("Show", "Project", new { id = id });
        }

        [HttpGet]
        public ActionResult Search(string ProjectID, int Page, string Title, ProjectStatus? Status, DateTime? Begin, DateTime? End, string OrderBy, string Order, DateTime? InvoiceBegin, DateTime? InvoiceEnd, int? EnterpriseID, int? gte, int? lte)
        {
            Title = StringUtils.Trim(Title);
            IEnumerable<ProjectModel> projects = (from p in DB.Projects
                                                  where p.Title.Contains(Title)
                                                  select p).ToList();
            if(!string.IsNullOrEmpty(ProjectID))
                projects = projects.Where(x => x.RefNum == ProjectID);
            if (Status.HasValue)
                projects = projects.Where(x => x.Status == Status.Value);
            if (Begin.HasValue)
                projects = projects.Where(x => x.SignTime >= Begin.Value);
            if (End.HasValue)
                projects = projects.Where(x => x.SignTime <= End.Value);
            if (InvoiceBegin.HasValue)
                projects = projects.Where(x => x.InvoiceTime >= InvoiceBegin.Value);
            if (InvoiceEnd.HasValue)
                projects = projects.Where(x => x.InvoiceTime <= InvoiceEnd.Value);
            if (EnterpriseID.HasValue)
                projects = projects.Where(x => x.EnterpriseID == EnterpriseID.Value);
            if (gte.HasValue)
                projects = projects.Where(x => x.Charge >= gte.Value);
            if (lte.HasValue)
                projects = projects.Where(x => x.Charge <= lte.Value);
            if (CurrentUser.Role == UserRole.Employee)
                projects = projects.Where(x => x.UserID == CurrentUser.ID);
            else if (CurrentUser.Role == UserRole.Master)
                projects = projects.Where(x => x.User.DepartmentID != null && x.User.Department.UserID == CurrentUser.ID);
            if (string.IsNullOrEmpty(OrderBy))
            {
                projects = projects.OrderByDescending(x => x.SignTime).ThenByDescending(x => x.Priority);
            }
            else
            {
                if (Order == "asc")
                {
                    if (OrderBy == "RefNum")
                        projects = projects.OrderBy(x => x.RefNum);
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
                        projects = projects.OrderBy(x => x.SignTime);
                }
                else
                {
                    if (OrderBy == "RefNum")
                        projects = projects.OrderByDescending(x => x.RefNum);
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
                        projects = projects.OrderByDescending(x => x.SignTime);
                }
            }
            int countOfRecords = projects.Count();
            projects = projects.Skip(20 * (Page - 1)).Take(20).ToList();
            var data = new List<ProjectListViewModel>();
            foreach (var p in projects)
            {
                try
                {
                    data.Add((ProjectListViewModel)p);
                }
                catch
                {
                    //e.Message;
                }
            }
            //
            Pager pager = Pager.GetInstance(Page, 20, countOfRecords);
            SearchResult searchResult = new SearchResult(data, pager);

            return Json(searchResult, JsonRequestBehavior.AllowGet);
        }

        [MinRole(UserRole.Finance)]
        [HttpPost]
        public ActionResult DeleteSelectedProjects(string ids)
        {
            foreach (string id in ids.Split(','))
            {
                var project = DB.Projects.Find(int.Parse(id));
                DB.Projects.Remove(project);
                DB.SaveChanges();
            }
            return Content("OK");
        }
        
        [HttpGet]
        public ActionResult SearchBills(int ProjectID, int Type, DateTime? Begin, DateTime? End)
        {
            var project = DB.Projects.Find(ProjectID);
            var data = new List<BillListViewModel>();
            foreach (var b in project.Bills)
            {
                if (Type >= 0)
                {
                    if (b.Type != Type)
                    {
                        continue;
                    }
                }
                if (Begin.HasValue)
                {
                    if (b.Time < Begin.Value) {
                        continue;
                    }
                }
                if (End.HasValue)
                {
                    if (b.Time > End.Value.AddDays(1)) {
                        continue;
                    }
                }

                try
                {
                    data.Add((BillListViewModel)b);
                }
                catch
                {
                    //e.Message;
                }
            }
            //

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [ValidateSID]
        public ActionResult Create(ProjectModel Model)
        {
            Model.Log = DateTime.Now + " 被创建.\r\n";
            Model.Hint = "无内容";
            Model.UserID = Model.UserID;
            Model.Ordering = false;
            Model.Percent = 0;
            Model.PayMethod = PayMethod.Unpaid;
            DB.Projects.Add(Model);
            DB.SaveChanges();
            Model.RefNum = "39" + Model.ID.ToString().PadLeft(5, '0');
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
            ViewBag.ProjectID = id;
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
            DB.Bills.Add(Model);
            DB.SaveChanges();
            var Project = DB.Projects.Find(id);
            Project.Log += string.Format("[{0}] {1}添加了支出(-{2})\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username, Model.Plan);
            DB.SaveChanges();
            return Content(Model.ID.ToString());
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult EditBillType(Guid id, BillModel Model)
        {
            var bill = DB.Bills.Find(id);
            bill.Type = Model.Type;
            bill.Project.Log += string.Format("[{0}] {1}修改了支出\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();

            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult EditBillHint(Guid id, BillModel Model)
        {
            var bill = DB.Bills.Find(id);
            bill.Hint = Model.Hint;
            bill.Project.Log += string.Format("[{0}] {1}修改了支出\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();

            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult EditBillPlan(Guid id, BillModel Model)
        {
            var bill = DB.Bills.Find(id);
            bill.Plan = Model.Plan;
            bill.Project.Log += string.Format("[{0}] {1}修改了支出\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();

            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult EditBillTime(Guid id, BillModel Model)
        {
            var bill = DB.Bills.Find(id);
            bill.Time = Model.Time;
            bill.Project.Log += string.Format("[{0}] {1}修改了支出\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();

            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult EditBillActual(Guid id, BillModel Model)
        {
            var bill = DB.Bills.Find(id);
            bill.Actual = Model.Actual;
            bill.Project.Log += string.Format("[{0}] {1}修改了支出\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();

            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
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
            bill.Time = Model.Time;
            bill.Project.Log += string.Format("[{0}] {1}修改了支出\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();

            return RedirectToAction("Bill", "Project", new { id = bill.ProjectID });
        }
        
        [MinRole(UserRole.Finance)]
        [HttpPost]
        public ActionResult DeleteSelectedBills(string ids)
        {
            foreach (string id in ids.Split(','))
            {
                var bill = DB.Bills.Find(new Guid(id));
                DB.Bills.Remove(bill);
                DB.SaveChanges();
            }
            return Content("OK");
        }

        [MinRole(UserRole.Finance)]
        [ValidateSID]
        [HttpPost]
        public ActionResult DeleteBill(Guid id)
        {
            var bill = DB.Bills.Find(id);
            bill.Project.Log += string.Format("[{0}] {1}删除了支出", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Username);
            DB.SaveChanges();
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
            var html = "<table style='border: 1px solid #000;border-collapse:collapse'><tr><td colspan=\"5\" style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>" + project.Title + " ID:" + project.ID + " 支出明细</td></tr><tr><td style='border-bottom:1px solid #000'>支出日期</td><td style='border-bottom:1px solid #000'>支出类型</td><td style='border-bottom:1px solid #000'>说明</td><td style='border-bottom:1px solid #000'>计划经费</td><td style='border-bottom:1px solid #000'>实际经费</td></tr>";
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
            project.Log = "";
            try
            {
                DB.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw dbEx;
            }
            return RedirectToAction("Show", "Project", new { id = id });
        }

        [ValidateSID]
        public ActionResult Export(DateTime? begin, DateTime? end, string type)
        {
            IEnumerable<ProjectModel> projects = DB.Projects;
            if (begin.HasValue)
                projects = projects.Where(x => x.SignTime >= begin.Value);
            if (end.HasValue)
                projects = projects.Where(x => x.SignTime <= end.Value);
            if (CurrentUser.Role == UserRole.Employee)
                projects = projects.Where(x => x.UserID == CurrentUser.ID);
            if (CurrentUser.Role == UserRole.Master)
                projects = projects.Where(x => x.User.DepartmentID != null && x.User.Department.UserID == CurrentUser.ID);
            projects = projects.ToList();
            var html = @"<table style='border: 1px solid #000;border-collapse:collapse'>
<tr>
    <td class='border: 1px solid #000;border-collapse:collapse'>ID</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>项目名称</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>所有者</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>金额</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>签订日期</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>产品类型</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>客户</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>品牌</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>联系人</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>状态</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>开票日期</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>收款日期</td>
</tr>";
            foreach (var p in projects)
            {
                var tmp = (ProjectListViewModel)p;
                html += string.Format(@"<table style='border: 1px solid #000;border-collapse:collapse'>
<tr>
    <td class='border: 1px solid #000;border-collapse:collapse'>{0}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{1}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{2}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{3}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{4}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{5}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{6}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{7}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{8}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{9}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{10}</td>
    <td class='border: 1px solid #000;border-collapse:collapse'>{11}</td>
</tr>", tmp.ID , tmp.Title,tmp.Owner, tmp.Charge, tmp.SignTime, tmp.Product, tmp.Enterprise, tmp.Customer, tmp.Brand, tmp.Status, tmp.InvoiceTime, tmp.ChargeTime);
            }
            html += "</table>";
            if (type == "PDF")
                return File(Helpers.Export.ToPDF(html), "application/pdf", Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
            else
                return File(Helpers.Export.ToExcel(html), "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
        }

        public ActionResult ExportBill(DateTime? begin, DateTime? end, string type)
        {
            if (CurrentUser.Role < UserRole.Finance)
                return Redirect("/Shared/AccessDenied");
            IEnumerable<BillModel> bills = (from b in DB.Bills
                         orderby b.Time descending
                         select b);
            if (begin.HasValue)
                bills = bills.Where(x => x.Time >= begin.Value);
            if (end.HasValue)
                bills = bills.Where(x => x.Time <= end.Value);
            bills = bills.ToList();
            var html = "<table style='border: 1px solid #000;border-collapse:collapse'><tr><td colspan=\"7\" style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>支出明细</td></tr><tr><td style='border-bottom:1px solid #000'>项目ID</td><td style='border-bottom:1px solid #000'>项目名称</td><td style='border-bottom:1px solid #000'>支出日期</td><td style='border-bottom:1px solid #000'>支出类型</td><td style='border-bottom:1px solid #000'>说明</td><td style='border-bottom:1px solid #000'>计划经费</td><td style='border-bottom:1px solid #000'>实际经费</td></tr>";
            foreach (var b in bills)
            {
                html += string.Format(
                    "<tr><td style='border-bottom:1px solid #000'>{5}</td style='border-bottom:1px solid #000'><td style='border-bottom:1px solid #000'>{6}</td style='border-bottom:1px solid #000'><td style='border-bottom:1px solid #000'>{0}</td style='border-bottom:1px solid #000'><td style='border-bottom:1px solid #000'>{1}</td><td style='border-bottom:1px solid #000'>{2}</td><td style='border-bottom:1px solid #000'>￥{3}</td><td style='border-bottom:1px solid #000'>￥{4}</td></tr>",
                    b.Time.ToString("yyyy-MM-dd"),
                    ((BillViewModel)b).Type,
                    b.Hint,
                    b.Plan.ToString("0.00"),
                    b.Actual.ToString("0.00"),
                    b.ProjectID,
                    b.Project.Title
                );
            }
            html += "</table>";
            if (type == "PDF")
                return File(Helpers.Export.ToPDF(html), "application/pdf", Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
            else
                return File(Helpers.Export.ToExcel(html), "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
        }

        [HttpPost]
        public ActionResult Handout(int id, string username)
        {
            var project = DB.Projects.Where(x => x.ID == id).Single();
            var user = DB.Users.Where(x => x.Username == username).Single();
            project.UserID = user.ID;
            DB.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}