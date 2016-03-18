using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity.Validation;

namespace EasySense.Controllers
{
    [Authorize]
    public class StatisticsController : BaseController
    {
        // GET: Statistics
        public ActionResult Index()
        {
            IEnumerable<StatisticsModel> statistics = DB.Statistics;
            if (CurrentUser.Role == UserRole.Finance)
                statistics = statistics.Where(x => x.PushTo == UserRole.Finance || x.PushTo == null);
            else if (CurrentUser.Role == UserRole.Finance)
                statistics = statistics.Where(x => x.PushTo == UserRole.Master || x.PushTo == null);
            else if (CurrentUser.Role == UserRole.Employee)
                statistics = statistics.Where(x => x.PushTo == null);
            statistics = statistics.OrderByDescending(x => x.Time).ToList();
            ViewBag.Users = DB.Users.ToList();
            ViewBag.Enterprises = DB.Enterprises.ToList();
            ViewBag.Customers = DB.Customers.ToList();
            //ViewBag.Brands = (from e in DB.Enterprises
            //                  select e.Brand).Distinct().ToList();
            ViewBag.Products = DB.Products.ToList();
            //
            /*
            List<ExportedFieldModel> ExportedFields = new List<ExportedFieldModel>();
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            ExportedFields.Add(new ExportedFieldModel("ID", ""));
            */
            //
            return View(statistics);
        }

        [HttpGet]
        [MinRole(UserRole.Root)]
        public ActionResult GetStatistic(Guid id)
        {
            return Json((StatisticsViewModel) DB.Statistics.Find(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Create(StatisticsModel Model, string[] xExportedFields, int[] xEnterpriseIDs, int[] xCustomerIDs, int[] xProductIDs, string[] xBrands)
        {
            Model.ID = Guid.NewGuid();
            Model.Time = DateTime.Now;
            DB.Statistics.Add(Model);
            IEnumerable<ProjectModel> Projects = DB.Projects;
            if (Model.Begin != null)
                Projects = Projects.Where(x => x.SignTime >= Model.Begin.Value);
            if (Model.End != null)
                Projects = Projects.Where(x => x.SignTime <= Model.End.Value);
            if (Model.Status != null)
                Projects = Projects.Where(x => x.Status == Model.Status.Value);
            if (Model.UserID != null)
                Projects = Projects.Where(x => x.UserID == Model.UserID.Value);
            if (xEnterpriseIDs!= null && xEnterpriseIDs.Count() > 0)
            {
                var eids = xEnterpriseIDs.ToList();
                Projects = Projects.Where(x => x.EnterpriseID!= null && eids.Contains(x.EnterpriseID.Value));
            }
            if (xCustomerIDs != null && xCustomerIDs.Count() > 0)
            {
                var cids = xCustomerIDs.ToList();
                Projects = Projects.Where(x => x.CustomerID != null && cids.Contains(x.CustomerID.Value));
            }
            if (xProductIDs != null && xProductIDs.Count() > 0)
            {
                var pids = xProductIDs.ToList();
                Projects = Projects.Where(x => x.ProductID != null && pids.Contains(x.ProductID.Value));
            }
            if (xBrands != null && xBrands.Count() > 0)
            {
                var tmp = xBrands.ToList();
                Projects = Projects.Where(x => x.Enterprise != null && tmp.Contains(x.Brand));
            }
            if (Model.ChargeBegin.HasValue)
                Projects = Projects.Where(x => x.Charge >= Model.ChargeBegin.Value);
            if (Model.ChargeEnd.HasValue)
                Projects = Projects.Where(x => x.Charge <= Model.ChargeEnd.Value);
            Projects = Projects.Where(x => x.UserID != null && x.ProductID != null && x.SignTime != null);
            Projects = Projects.ToList();
            var CustomerChart = new List<JQChartViewModel>();
            var EmployeeChart = new List<JQChartViewModel>();
            var ProductIDs = (from p in Projects
                              group p by p.ProductID into g
                              select g.Key).ToList();
            var Products = (from p in DB.Products
                            where ProductIDs.Contains(p.ID)
                            select p).ToList();
            var EnterpriseIDs = (from p in Projects
                                 group p by p.EnterpriseID into g
                                 select g.Key).ToList();
            var Enterprises = (from e in DB.Enterprises
                               where EnterpriseIDs.Contains(e.ID)
                               select e).ToList();
            var UserIDs = (from p in Projects
                           where p.UserID != null
                           group p by p.UserID into g
                           select g.Key).ToList();
            var Users = (from u in DB.Users
                         where UserIDs.Contains(u.ID)
                         select u).ToList();
            #region 构建表格
            var DynamicCol = 0;
            var html = "<table style='border: 1px solid #000;border-collapse:collapse'><tr><td colspan='{TOTALCOL}' style='text-align: center; font-weight: bold; border: 1px solid #000'>" + Model.Title + "</td></tr>";
            html += "<tr><td colspan='2' style='border: 1px solid #000'></td><td colspan='{DYNAMICCOL}' style='border: 1px solid #000'>所有者</td></tr>";
            html += "<tr><td colspan='2' style='border: 1px solid #000'></td>";
            foreach (var u in Users)
            {
                html += "<td colspan='{USER" + u.ID + "}' style='border: 1px solid #000'>" + u.Name + "</td>";
            }
            html += "</tr>";
            html += "<tr><td style='border: 1px solid #000'>项目类别</td><td style='border: 1px solid #000'>产品类型</td>";
            foreach (var u in Users)
            {
                var day = (from p in Projects
                           where p.UserID == u.ID
                           group p by new { Year = p.SignTime.Value.Year, Month = p.SignTime.Value.Month } into g
                           select g.Key).ToList();
                DynamicCol += day.Count;
                html = html.Replace("{USER" + u.ID + "}", day.Count.ToString());
                foreach (var d in day)
                    html += "<td style='border: 1px solid #000'>" + d.Year + "年" + d.Month + "月</td>";
            }
            html += "</tr>";
            foreach (var p in Products)
            {
                html += "<tr><td style='border: 1px solid #000'>" + p.Category.Title + "</td><td style='border: 1px solid #000'>" + p.Title + "</td>";
                foreach (var u in Users)
                {
                    var day = (from pr in Projects
                               where pr.UserID == u.ID
                               group pr by new { Year = pr.SignTime.Value.Year, Month = pr.SignTime.Value.Month } into g
                               select g.Key).ToList();
                    foreach (var d in day)
                    {
                        var begin = new DateTime(d.Year, d.Month, 1);
                        var end = begin.AddMonths(1);
                        var price = (from pr in Projects
                                     where pr.UserID == u.ID
                                     && pr.ProductID == p.ID
                                     && pr.SignTime >= begin
                                     && pr.SignTime < end
                                     && pr.Charge != null
                                     select pr).Sum(x => x.Charge.Value);
                        html += "<td style='border: 1px solid #000'>￥" + price.ToString("0.00") + "</td>";
                    }
                }
                html += "</tr>";
            }
            html = html.Replace("{TOTALCOL}", (DynamicCol + 2).ToString()).Replace("{DYNAMICCOL}", DynamicCol.ToString()) + "</table>";
            Model.HtmlPreview = html;
            try
            {
                Model.ExcelBlob = Helpers.Export.ToExcel(html);
            }
            catch { }
            try
            {
                Model.PDFBlob = Helpers.Export.ToPDF(html);
            }
            catch { }
            #endregion
            #region 构建簇状图
            foreach (var p in Products)
            {
                var employeeitem = new JQChartViewModel
                {
                    type = "column",
                    title = p.Title,
                    data = new List<dynamic[]>()
                };
                foreach (var u in Users)
                {
                    employeeitem.PushData(u.Name, Convert.ToInt32(Projects.Where(x => x.UserID == u.ID && x.Charge != null).Sum(x => x.Charge.Value)));
                }
                var enterpriseitem = new JQChartViewModel
                {
                    type = "column",
                    title = p.Title,
                    data = new List<dynamic[]>()
                };
                foreach (var e in Enterprises)
                {
                    enterpriseitem.PushData(e.Title, Convert.ToInt32(Projects.Where(x => x.EnterpriseID != null && x.EnterpriseID == e.ID &&x.Charge !=null).Sum(x => x.Charge.Value)));
                }
                CustomerChart.Add(enterpriseitem);
                EmployeeChart.Add(employeeitem);
            }
            Model.EnterpriseGraphics = JsonConvert.SerializeObject(CustomerChart);
            Model.EmployeeGraphics = JsonConvert.SerializeObject(EmployeeChart);
            Model.ExportedFields = xExportedFields != null ? string.Join(",", xExportedFields) : null;
            Model.EnterpriseIDs = xEnterpriseIDs != null ? string.Join(",", xEnterpriseIDs) : null;
            Model.CustomerIDs = xCustomerIDs != null ? string.Join(",", xCustomerIDs) : null;
            Model.ProductIDs = xProductIDs != null ? string.Join(",", xProductIDs) : null;
            Model.Brands = xBrands != null ? string.Join(",", xBrands) : null;
            #endregion
            DB.SaveChanges();
            return RedirectToAction("Show", "Statistics", new { id = Model.ID });
        }


        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult EditStatistics(StatisticsModel Model, string[] aXExportedFields, int[] aXEnterpriseIDs, int[] aXCustomerIDs, int[] aXProductIDs, string[] aXBrands)
        {
            StatisticsModel statistics = DB.Statistics.Find(Model.ID);
            statistics.Begin = Model.Begin;
            statistics.End = Model.End;
            statistics.Status = Model.Status;
            statistics.UserID = Model.UserID;
            statistics.ChargeBegin = Model.ChargeBegin;
            statistics.ChargeEnd = Model.ChargeEnd;
            statistics.Title = Model.Title;
            statistics.PushTo = Model.PushTo;

            IEnumerable<ProjectModel> Projects = DB.Projects;
            if (Model.Begin != null)
                Projects = Projects.Where(x => x.SignTime >= Model.Begin.Value);
            if (Model.End != null)
                Projects = Projects.Where(x => x.SignTime <= Model.End.Value);
            if (Model.Status != null)
                Projects = Projects.Where(x => x.Status == Model.Status.Value);
            if (Model.UserID != null)
                Projects = Projects.Where(x => x.UserID == Model.UserID.Value);
            if (aXEnterpriseIDs != null && aXEnterpriseIDs.Count() > 0)
            {
                var eids = aXEnterpriseIDs.ToList();
                Projects = Projects.Where(x => x.EnterpriseID != null && eids.Contains(x.EnterpriseID.Value));
            }
            if (aXCustomerIDs != null && aXCustomerIDs.Count() > 0)
            {
                var cids = aXCustomerIDs.ToList();
                Projects = Projects.Where(x => x.CustomerID != null && cids.Contains(x.CustomerID.Value));
            }
            if (aXProductIDs != null && aXProductIDs.Count() > 0)
            {
                var pids = aXProductIDs.ToList();
                Projects = Projects.Where(x => x.ProductID != null && pids.Contains(x.ProductID.Value));
            }
            if (aXBrands != null && aXBrands.Count() > 0)
            {
                var tmp = aXBrands.ToList();
                Projects = Projects.Where(x => x.Enterprise != null && tmp.Contains(x.Brand));
            }
            if (Model.ChargeBegin.HasValue)
                Projects = Projects.Where(x => x.Charge >= Model.ChargeBegin.Value);
            if (Model.ChargeEnd.HasValue)
                Projects = Projects.Where(x => x.Charge <= Model.ChargeEnd.Value);
            Projects = Projects.Where(x => x.UserID != null && x.ProductID != null && x.SignTime != null);
            Projects = Projects.ToList();
            var CustomerChart = new List<JQChartViewModel>();
            var EmployeeChart = new List<JQChartViewModel>();
            var ProductIDs = (from p in Projects
                              group p by p.ProductID into g
                              select g.Key).ToList();
            var Products = (from p in DB.Products
                            where ProductIDs.Contains(p.ID)
                            select p).ToList();
            var EnterpriseIDs = (from p in Projects
                                 group p by p.EnterpriseID into g
                                 select g.Key).ToList();
            var Enterprises = (from e in DB.Enterprises
                               where EnterpriseIDs.Contains(e.ID)
                               select e).ToList();
            var UserIDs = (from p in Projects
                           where p.UserID != null
                           group p by p.UserID into g
                           select g.Key).ToList();
            var Users = (from u in DB.Users
                         where UserIDs.Contains(u.ID)
                         select u).ToList();
            #region 构建表格
            var DynamicCol = 0;
            var html = "<table style='border: 1px solid #000;border-collapse:collapse'><tr><td colspan='{TOTALCOL}' style='text-align: center; font-weight: bold; border: 1px solid #000'>" + Model.Title + "</td></tr>";
            html += "<tr><td colspan='2' style='border: 1px solid #000'></td><td colspan='{DYNAMICCOL}' style='border: 1px solid #000'>所有者</td></tr>";
            html += "<tr><td colspan='2' style='border: 1px solid #000'></td>";
            foreach (var u in Users)
            {
                html += "<td colspan='{USER" + u.ID + "}' style='border: 1px solid #000'>" + u.Name + "</td>";
            }
            html += "</tr>";
            html += "<tr><td style='border: 1px solid #000'>项目类别</td><td style='border: 1px solid #000'>产品类型</td>";
            foreach (var u in Users)
            {
                var day = (from p in Projects
                           where p.UserID == u.ID
                           group p by new { Year = p.SignTime.Value.Year, Month = p.SignTime.Value.Month } into g
                           select g.Key).ToList();
                DynamicCol += day.Count;
                html = html.Replace("{USER" + u.ID + "}", day.Count.ToString());
                foreach (var d in day)
                    html += "<td style='border: 1px solid #000'>" + d.Year + "年" + d.Month + "月</td>";
            }
            html += "</tr>";
            foreach (var p in Products)
            {
                html += "<tr><td style='border: 1px solid #000'>" + p.Category.Title + "</td><td style='border: 1px solid #000'>" + p.Title + "</td>";
                foreach (var u in Users)
                {
                    var day = (from pr in Projects
                               where pr.UserID == u.ID
                               group pr by new { Year = pr.SignTime.Value.Year, Month = pr.SignTime.Value.Month } into g
                               select g.Key).ToList();
                    foreach (var d in day)
                    {
                        var begin = new DateTime(d.Year, d.Month, 1);
                        var end = begin.AddMonths(1);
                        var price = (from pr in Projects
                                     where pr.UserID == u.ID
                                     && pr.ProductID == p.ID
                                     && pr.SignTime >= begin
                                     && pr.SignTime < end
                                     && pr.Charge != null
                                     select pr).Sum(x => x.Charge.Value);
                        html += "<td style='border: 1px solid #000'>￥" + price.ToString("0.00") + "</td>";
                    }
                }
                html += "</tr>";
            }
            html = html.Replace("{TOTALCOL}", (DynamicCol + 2).ToString()).Replace("{DYNAMICCOL}", DynamicCol.ToString()) + "</table>";
            statistics.HtmlPreview = html;
            try
            {
                statistics.ExcelBlob = Helpers.Export.ToExcel(html);
            }
            catch { }
            try
            {
                statistics.PDFBlob = Helpers.Export.ToPDF(html);
            }
            catch { }
            #endregion
            #region 构建簇状图
            foreach (var p in Products)
            {
                var employeeitem = new JQChartViewModel
                {
                    type = "column",
                    title = p.Title,
                    data = new List<dynamic[]>()
                };
                foreach (var u in Users)
                {
                    employeeitem.PushData(u.Name, Convert.ToInt32(Projects.Where(x => x.UserID == u.ID && x.Charge != null).Sum(x => x.Charge.Value)));
                }
                var enterpriseitem = new JQChartViewModel
                {
                    type = "column",
                    title = p.Title,
                    data = new List<dynamic[]>()
                };
                foreach (var e in Enterprises)
                {
                    enterpriseitem.PushData(e.Title, Convert.ToInt32(Projects.Where(x => x.EnterpriseID != null && x.EnterpriseID == e.ID && x.Charge != null).Sum(x => x.Charge.Value)));
                }
                CustomerChart.Add(enterpriseitem);
                EmployeeChart.Add(employeeitem);
            }
            statistics.EnterpriseGraphics = JsonConvert.SerializeObject(CustomerChart);
            statistics.EmployeeGraphics = JsonConvert.SerializeObject(EmployeeChart);
            statistics.ExportedFields = aXExportedFields != null ? string.Join(",", aXExportedFields) : null;
            statistics.EnterpriseIDs = aXEnterpriseIDs != null ? string.Join(",", aXEnterpriseIDs) : null;
            statistics.CustomerIDs = aXCustomerIDs != null ? string.Join(",", aXCustomerIDs) : null;
            statistics.ProductIDs = aXProductIDs != null ? string.Join(",", aXProductIDs) : null;
            statistics.Brands = aXBrands != null ? string.Join(",", aXBrands) : null;
            #endregion

            try
            {
                DB.SaveChanges();
            }
            catch (DbEntityValidationException xe)
            {
                StringBuilder errors = new StringBuilder();
                IEnumerable<DbEntityValidationResult> validationResult = xe.EntityValidationErrors;
                foreach (DbEntityValidationResult result in validationResult)
                {
                    ICollection<DbValidationError> validationError = result.ValidationErrors;
                    foreach (DbValidationError err in validationError)
                    {
                        errors.Append(err.PropertyName + ":" + err.ErrorMessage + "\r\n");
                    }
                }
                return RedirectToAction("Message", "Shared", new { msg = errors.ToString() });
            }

            return RedirectToAction("Show", "Statistics", new { id = Model.ID });
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Edit(Guid id, StatisticsModel Model)
        {
            var statistics = DB.Statistics.Find(id);
            statistics.Begin = Model.Begin;
            statistics.End = Model.End;
            statistics.Title = Model.Title;
            statistics.Hint = Model.Hint;
            statistics.UserID = Model.UserID;
            statistics.PushTo = Model.PushTo;
            DB.SaveChanges();
            return RedirectToAction("Show", "Statistics", new { id = Model.ID });
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Delete(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            DB.Statistics.Remove(statistics);
            DB.SaveChanges();
            return Content("OK");
        }

        [AccessToStatistics]
        public ActionResult Show(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return View(statistics);
        }

        [AccessToStatistics]
        public ActionResult ExportExcel_old(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return File(statistics.ExcelBlob, "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
        }
        
        [AccessToStatistics]
        public ActionResult ExportExcel(Guid id)
        {
            return ExportExcelOrPdf(id, true);
        }

        [AccessToStatistics]
        public ActionResult ExportPDF(Guid id)
        {
            return ExportExcelOrPdf(id, false);
        }

        private ActionResult ExportExcelOrPdf(Guid id, bool toExcel)
        {
            StatisticsModel statistics = DB.Statistics.Find(id);

            IEnumerable<ProjectModel> Projects = DB.Projects;
            if (statistics.Begin != null)
                Projects = Projects.Where(x => x.SignTime >= statistics.Begin.Value);
            if (statistics.End != null)
                Projects = Projects.Where(x => x.SignTime <= statistics.End.Value);
            if (statistics.Status != null)
                Projects = Projects.Where(x => x.Status == statistics.Status.Value);
            if (statistics.UserID != null)
                Projects = Projects.Where(x => x.UserID == statistics.UserID.Value);
            if (statistics.EnterpriseIDs != null && statistics.EnterpriseIDs.Count() > 0)
            {
                var eids = statistics.EnterpriseIDs.Split(',').ToList();
                Projects = Projects.Where(x => x.EnterpriseID != null && eids.Contains(x.EnterpriseID.Value.ToString()));
            }
            if (statistics.CustomerIDs != null && statistics.CustomerIDs.Count() > 0)
            {
                var cids = statistics.CustomerIDs.Split(',').ToList();
                Projects = Projects.Where(x => x.CustomerID != null && cids.Contains(x.CustomerID.Value.ToString()));
            }
            if (statistics.ProductIDs != null && statistics.ProductIDs.Count() > 0)
            {
                var pids = statistics.ProductIDs.Split(',').ToList();
                Projects = Projects.Where(x => x.ProductID != null && pids.Contains(x.ProductID.Value.ToString()));
            }
            if (statistics.Brands != null && statistics.Brands.Count() > 0)
            {
                var tmp = statistics.Brands.Split(',').ToList();
                Projects = Projects.Where(x => x.Enterprise != null && tmp.Contains(x.Brand));
            }
            if (statistics.ChargeBegin.HasValue)
                Projects = Projects.Where(x => x.Charge >= statistics.ChargeBegin.Value);
            if (statistics.ChargeEnd.HasValue)
                Projects = Projects.Where(x => x.Charge <= statistics.ChargeEnd.Value);
            Projects = Projects.Where(x => x.UserID != null && x.ProductID != null && x.SignTime != null);
            Projects = Projects.ToList();

            Projects = Projects.OrderByDescending(x => x.ChargeTime);
            Projects = Projects.ToList();

            var html = "<table style='border: 1px solid #000;border-collapse:collapse'><tr>";
            string exportedFields = statistics.ExportedFields;
            if (string.IsNullOrEmpty(exportedFields)) {
                html += "<td style='border: 1px solid #000'>ID</td>";
                html += "<td style='border: 1px solid #000'>所有者</td>";
                html += "<td style='border: 1px solid #000'>项目名称</td>";
                html += "<td style='border: 1px solid #000'>项目金额</td>";
                html += "<td style='border: 1px solid #000'>签订日期</td>";
                html += "<td style='border: 1px solid #000'>产品类型</td>";
                html += "<td style='border: 1px solid #000'>客户</td>";
                html += "<td style='border: 1px solid #000'>品牌</td>";
                html += "<td style='border: 1px solid #000'>联系人</td>";
                html += "<td style='border: 1px solid #000'>状态</td>";
                html += "<td style='border: 1px solid #000'>开票日期</td>";
                html += "<td style='border: 1px solid #000'>收款日期</td>";
            }
            else
            {
                if (exportedFields.Contains("ID"))
                {
                    html += "<td style='border: 1px solid #000'>ID</td>";
                }
                if (exportedFields.Contains("所有者"))
                {
                    html += "<td style='border: 1px solid #000'>所有者</td>";
                }
                if (exportedFields.Contains("项目名称"))
                {
                    html += "<td style='border: 1px solid #000'>项目名称</td>";
                }
                if (exportedFields.Contains("项目金额"))
                {
                    html += "<td style='border: 1px solid #000'>项目金额</td>";
                }
                if (exportedFields.Contains("签订日期"))
                {
                    html += "<td style='border: 1px solid #000'>签订日期</td>";
                }
                if (exportedFields.Contains("产品类型"))
                {
                    html += "<td style='border: 1px solid #000'>产品类型</td>";
                }
                if (exportedFields.Contains("客户"))
                {
                    html += "<td style='border: 1px solid #000'>客户</td>";
                }
                if (exportedFields.Contains("品牌"))
                {
                    html += "<td style='border: 1px solid #000'>品牌</td>";
                }
                if (exportedFields.Contains("联系人"))
                {
                    html += "<td style='border: 1px solid #000'>联系人</td>";
                }
                if (exportedFields.Contains("状态"))
                {
                    html += "<td style='border: 1px solid #000'>状态</td>";
                }
                if (exportedFields.Contains("开票日期"))
                {
                    html += "<td style='border: 1px solid #000'>开票日期</td>";
                }
                if (exportedFields.Contains("收款日期"))
                {
                    html += "<td style='border: 1px solid #000'>收款日期</td>";
                }
            }
            html += "</tr>";

            //////////////////////////////////////////////////////////
            foreach (ProjectModel Project in Projects)
            {
                ProjectListViewModel p = (ProjectListViewModel)Project;

                html += "<tr>";
                if (string.IsNullOrEmpty(exportedFields))
                {
                    html += "<td style='border: 1px solid #000'>" + p.RefNum + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Owner + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Title + "</td>";
                    html += "<td style='border: 1px solid #000'>￥" + p.Charge + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.SignTime + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Product + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Enterprise + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Brand + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Customer + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.Status + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.InvoiceTime + "</td>";
                    html += "<td style='border: 1px solid #000'>" + p.ChargeTime + "</td>";
                }
                else
                {
                    if (exportedFields.Contains("ID"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.RefNum + "</td>";
                    }
                    if (exportedFields.Contains("所有者"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Owner + "</td>";
                    }
                    if (exportedFields.Contains("项目名称"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Title + "</td>";
                    }
                    if (exportedFields.Contains("项目金额"))
                    {
                        html += "<td style='border: 1px solid #000'>￥" + p.Charge + "</td>";
                    }
                    if (exportedFields.Contains("签订日期"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.SignTime + "</td>";
                    }
                    if (exportedFields.Contains("产品类型"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Product + "</td>";
                    }
                    if (exportedFields.Contains("客户"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Enterprise + "</td>";
                    }
                    if (exportedFields.Contains("品牌"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Brand + "</td>";
                    }
                    if (exportedFields.Contains("联系人"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Customer + "</td>";
                    }
                    if (exportedFields.Contains("状态"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.Status + "</td>";
                    }
                    if (exportedFields.Contains("开票日期"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.InvoiceTime + "</td>";
                    }
                    if (exportedFields.Contains("收款日期"))
                    {
                        html += "<td style='border: 1px solid #000'>" + p.ChargeTime + "</td>";
                    }
                }
                html += "</tr>";
            }
            //////////////////////////////////////////////////////////

            html += "</table>";

            byte[] bytes = new byte[0];
            try
            {
                if (toExcel)
                {
                    bytes = Helpers.Export.ToExcel(html);
                }
                else
                {
                    bytes = Helpers.Export.ToPDF(html);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Message", "Shared", new { msg = e.Message });
            }

            if (toExcel)
            {
                return File(bytes, "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
            }
            else
            {
                return File(bytes, "application/pdf", Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
            }
        }

        [AccessToStatistics]
        public ActionResult ExportPDF_old(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return File(statistics.PDFBlob, "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
        }

        [AccessToStatistics]
        [HttpGet]
        public ActionResult EnterpriseChart(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return Content(statistics.EnterpriseGraphics, "application/json");
        }

        [AccessToStatistics]
        [HttpGet]
        public ActionResult EmployeeChart(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return Content(statistics.EmployeeGraphics, "application/json");
        }

        [AccessToStatistics]
        public ActionResult Graphics(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return View(statistics);
        }
    }
}