using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;
using EasySense.Schema;

namespace EasySense.Controllers
{
    [Authorize]
    public class EnterpriseController : BaseController
    {
        // GET: Enterprise
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search(string Text)
        {
            var result = new List<EnterpriseListViewModel>();
            //TODO: 完成根据Key字段Contains搜索结果与Title字段Contains搜索结果
            var _result = new List<EnterpriseModel>();
            _result = (from e in DB.Enterprises where e.Key.Contains(Text) || e.Title.Contains(Text) select e).ToList();
            foreach (var e in _result)
            {
                result.Add((EnterpriseModel)e);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400)]
        public ActionResult Icon(int id)
        {
            var enterprise = DB.Enterprises.Find(id);
            if (enterprise.Icon == null)
                return File(System.IO.File.ReadAllBytes(Server.MapPath("~/Images/Avatar.png")), "image/png");
            return File(enterprise.Icon, "image/png");
        }

        public ActionResult Show(int id)
        {
            var enterprise = DB.Enterprises.Find(id);
            return View(enterprise);
        }

        [HttpPost]
        [ValidateSID]
        public ActionResult Create(EnterpriseModel Model)
        {
            Model.Key = Helpers.Pinyin.Convert(Model.Title);
            DB.Enterprises.Add(Model);
            DB.SaveChanges();
            return Content(Model.ID.ToString());
        }

        public ActionResult Edit(
            int id, 
            string Title, 
            EnterpriseLevel Level,
            string Type,
            string Brand,
            string Scale,
            string SalesVolume,
            string Phone,
            string Fax,
            string Address,
            string Zip,
            string Website,
            string Hint
            )
        {
            var enterprise = DB.Enterprises.Find(id);
            enterprise.Title = Title;
            enterprise.Key = Helpers.Pinyin.Convert(Title);
            enterprise.Level = Level;
            enterprise.Type = Type;
            enterprise.Brand = Brand;
            enterprise.Scale = Scale;
            enterprise.SalesVolume = SalesVolume;
            enterprise.Phone = Phone;
            enterprise.Fax = Fax;
            enterprise.Address = Address;
            enterprise.Zip = Zip;
            enterprise.Website = Website;
            enterprise.Hint = Hint;
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                var timestamp = Helpers.String.ToTimeStamp(DateTime.Now);
                var filename = timestamp + ".tmp";
                var dir = Server.MapPath("~") + @"\Temp\";
                file.SaveAs(dir+filename);
                enterprise.Icon = System.IO.File.ReadAllBytes(dir+filename);
                System.IO.File.Delete(dir + filename);
            }
            DB.SaveChanges();
            return RedirectToAction("Show", "Enterprise", new { id = id });
        }

        [ValidateSID]
        [HttpPost]
        public ActionResult CreateCustomer(int id, CustomerModel Model)
        {
            Model.EnterpriseID = id;
            DB.Customers.Add(Model);
            DB.SaveChanges();
            return Content(Model.ID.ToString());
        }

        [ValidateSID]
        [HttpGet]
        public ActionResult DeleteCustomer(int id)
        {
            var customer = DB.Customers.Find(id);
            var eid = customer.EnterpriseID;
            DB.Customers.Remove(customer);
            DB.SaveChanges();
            return RedirectToAction("Show", "Enterprise", new { id = eid });
        }

        [ValidateSID]
        [HttpPost]
        public ActionResult EditCustomer(int id, CustomerModel Model)
        {
            var customer = DB.Customers.Find(id);
            var eid = customer.EnterpriseID;
            customer.Birthday = Model.Birthday;
            customer.Email = Model.Email;
            customer.Fax = Model.Fax;
            customer.Hint = Model.Hint;
            customer.Name = Model.Name;
            customer.Phone = Model.Phone;
            customer.Position = Model.Position;
            customer.QQ = Model.QQ;
            customer.Sex = Model.Sex;
            customer.Tel = Model.Tel;
            customer.WeChat = Model.WeChat;
            DB.SaveChanges();
            return RedirectToAction("Show", "Enterprise", new { id = eid });
        }

        [HttpGet]
        public ActionResult Customer(int id)
        {
            var customer = DB.Customers.Find(id);
            return Json((CustomerViewModel)customer, JsonRequestBehavior.AllowGet);
        }

        [ValidateSID]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var enterprise = DB.Enterprises.Find(id);
            DB.Enterprises.Remove(enterprise);
            DB.SaveChanges();
            return RedirectToAction("Index", "Enterprise");
        }

        private string BuildHtmlTable(int id)
        {
            var enterprise = DB.Enterprises.Find(id);
            var html = "<table style='border: 1px solid #000'><tr><td colspan=\"10\" style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>" + enterprise.Title + " 联系人</td></tr><tr><td style='border-bottom:1px solid #000'>姓名</td><td style='border-bottom:1px solid #000'>性别</td><td style='border-bottom:1px solid #000'>固定电话</td><td style='border-bottom:1px solid #000'>传真</td><td style='border-bottom:1px solid #000'>手机</td><td style='border-bottom:1px solid #000'>电子邮箱</td><td style='border-bottom:1px solid #000'>QQ</td><td style='border-bottom:1px solid #000'>微信</td><td style='border-bottom:1px solid #000'>生日</td><td style='border-bottom:1px solid #000'>备注</td></tr>";
            foreach (var c in enterprise.Customers)
            {
                html += string.Format(
                    "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td></tr>",
                    c.Name,
                    c.Sex == Sex.Male?"男" :"女",
                    c.Tel,
                    c.Fax,
                    c.Phone,
                    c.Email,
                    c.QQ,
                    c.WeChat,
                    c.Birthday.ToString("MM月dd日"),
                    c.Hint
                );
            }
            html += "</table>";
            return html;
        }

        [HttpGet]
        public ActionResult ExportExcel(int id)
        {
            return File(Helpers.Export.ToExcel(BuildHtmlTable(id)), "application/vnd.ms-excel", Helpers.Time.ToTimeStamp(DateTime.Now) + ".xls");
        }

        [HttpGet]
        public ActionResult ExportPDF(int id)
        {
            return File(Helpers.Export.ToPDF(BuildHtmlTable(id)), "application/pdf", Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
        }

        [HttpGet]
        public ActionResult Customers(int id, string Text)
        {
            var enterprise = DB.Enterprises.Find(id);
            var ret = new List<CustomerViewModel>();
            foreach (var c in enterprise.Customers.Where(x=>x.Name.Contains(Text)))
                ret.Add((CustomerViewModel)c);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}