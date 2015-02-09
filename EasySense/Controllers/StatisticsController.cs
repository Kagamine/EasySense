using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;
using Newtonsoft.Json;

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
            return View(statistics);
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Create(StatisticsModel Model)
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
            var html = "<table style='border: 1px solid #000'><tr><td colspan='{TOTALCOL}' style='text-align: center; font-weight: bold; border: 1px solid #000'>" + Model.Title + "</td></tr>";
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
                        html += "<td style='border: 1px solid #000'>￥" + price.ToString("0.00") + "<td>";
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
                    employeeitem.PushData(u.Name, Convert.ToInt32(Projects.Where(x => x.UserID == u.ID).Sum(x => x.Charge.Value)));
                }
                var enterpriseitem = new JQChartViewModel
                {
                    type = "column",
                    title = p.Title,
                    data = new List<dynamic[]>()
                };
                foreach (var e in Enterprises)
                {
                    enterpriseitem.PushData(e.Title, Convert.ToInt32(Projects.Where(x => x.EnterpriseID != null && x.EnterpriseID == e.ID).Sum(x => x.Charge.Value)));
                }
                CustomerChart.Add(enterpriseitem);
                EmployeeChart.Add(employeeitem);
            }
            Model.EnterpriseGraphics = JsonConvert.SerializeObject(CustomerChart);
            Model.EmployeeGraphics = JsonConvert.SerializeObject(EmployeeChart);
            #endregion
            DB.SaveChanges();
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
        public ActionResult ExportExcel(Guid id)
        {
            var statistics = DB.Statistics.Find(id);
            return File(statistics.ExcelBlob, "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
        }

        [AccessToStatistics]
        public ActionResult ExportPDF(Guid id)
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