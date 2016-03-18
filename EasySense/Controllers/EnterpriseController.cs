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

        [HttpGet]
        public ActionResult SearchCustomer(string OrderBy, string Order, int EnterpriseID)
        {
            IEnumerable<CustomerModel> customers = (from c in DB.Customers
                                                  where c.EnterpriseID == EnterpriseID
                                                  select c).ToList();
            if (Order == "asc")
            {
                if (OrderBy == "DepartmentName")
                    customers = customers.OrderBy(x => x.DepartmentName);
                else if (OrderBy == "ProductCategory")
                    customers = customers.OrderBy(x => x.ProductCategory);
                else if (OrderBy == "ProductName")
                    customers = customers.OrderBy(x => x.ProductName);
                else if (OrderBy == "OfficeEmail")
                    customers = customers.OrderBy(x => x.OfficeEmail);
                else if (OrderBy == "Name")
                    customers = customers.OrderBy(x => x.Name);
                else if (OrderBy == "Sex")
                    customers = customers.OrderBy(x => x.Sex);
                else if (OrderBy == "Position")
                    customers = customers.OrderBy(x => x.Position);
                else if (OrderBy == "Tel")
                    customers = customers.OrderBy(x => x.Tel);
                else if (OrderBy == "Phone")
                    customers = customers.OrderBy(x => x.Phone);
                else if (OrderBy == "Fax")
                    customers = customers.OrderBy(x => x.Fax);
                else if (OrderBy == "WeChat")
                    customers = customers.OrderBy(x => x.WeChat);
                else if (OrderBy == "QQ")
                    customers = customers.OrderBy(x => x.QQ);
                else if (OrderBy == "Email")
                    customers = customers.OrderBy(x => x.Email);
                else if (OrderBy == "Birthday")
                    customers = customers.OrderBy(x => x.Birthday);
                else if (OrderBy == "Hint")
                    customers = customers.OrderBy(x => x.Hint);
                else
                    customers = customers.OrderBy(x => x.ID);
            }
            else
            {
                if (OrderBy == "DepartmentName")
                    customers = customers.OrderByDescending(x => x.DepartmentName);
                else if (OrderBy == "ProductCategory")
                    customers = customers.OrderByDescending(x => x.ProductCategory);
                else if (OrderBy == "ProductName")
                    customers = customers.OrderByDescending(x => x.ProductName);
                else if (OrderBy == "OfficeEmail")
                    customers = customers.OrderByDescending(x => x.OfficeEmail);
                else if (OrderBy == "Name")
                    customers = customers.OrderByDescending(x => x.Name);
                else if (OrderBy == "Sex")
                    customers = customers.OrderByDescending(x => x.Sex);
                else if (OrderBy == "Position")
                    customers = customers.OrderByDescending(x => x.Position);
                else if (OrderBy == "Tel")
                    customers = customers.OrderByDescending(x => x.Tel);
                else if (OrderBy == "Phone")
                    customers = customers.OrderByDescending(x => x.Phone);
                else if (OrderBy == "Fax")
                    customers = customers.OrderByDescending(x => x.Fax);
                else if (OrderBy == "WeChat")
                    customers = customers.OrderByDescending(x => x.WeChat);
                else if (OrderBy == "QQ")
                    customers = customers.OrderByDescending(x => x.QQ);
                else if (OrderBy == "Email")
                    customers = customers.OrderByDescending(x => x.Email);
                else if (OrderBy == "Birthday")
                    customers = customers.OrderByDescending(x => x.Birthday);
                else if (OrderBy == "Hint")
                    customers = customers.OrderByDescending(x => x.Hint);
                else
                    customers = customers.OrderByDescending(x => x.ID);
            }

            int countOfRecords = customers.Count();
            customers = customers.ToList();
            var data = new List<CustomerListViewModel>();
            foreach (var c in customers)
                data.Add((CustomerListViewModel)c);

            return Json(data, JsonRequestBehavior.AllowGet);
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
            //enterprise.Brand = Brand;
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
            try
            {
                DB.Customers.Remove(customer);
                DB.SaveChanges();
                return Content(eid.ToString());
            }
            catch
            {
                return Content("cannot delete");
            }
        }

        [ValidateSID]
        [HttpPost]
        public ActionResult EditCustomer(int id, CustomerModel Model)
        {
            var customer = DB.Customers.Find(id);
            var eid = customer.EnterpriseID;
            customer.DepartmentName = Model.DepartmentName;
            customer.ProductCategory = Model.ProductCategory;
            customer.ProductName = Model.ProductName;
            customer.OfficeEmail = Model.OfficeEmail;
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

            try
            {
                DB.SaveChanges();
                return RedirectToAction("Index", "Enterprise");
            }
            catch
            {
                return Content("cannot delete");
            }
        }

        private string BuildHtmlTable(int id)
        {
            var enterprise = DB.Enterprises.Find(id);
            var html = "<table style='border: 1px solid #000;border-collapse:collapse'><tr><td colspan=\"14\" style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>" + enterprise.Title + " 联系人</td></tr><tr><td style='border-bottom:1px solid #000'>部门</td><td style='border-bottom:1px solid #000'>产品类别</td><td style='border-bottom:1px solid #000'>产品名称</td><td style='border-bottom:1px solid #000'>联系人</td><td style='border-bottom:1px solid #000'>性别</td><td style='border-bottom:1px solid #000'>电话</td><td style='border-bottom:1px solid #000'>传真</td><td style='border-bottom:1px solid #000'>手机</td><td style='border-bottom:1px solid #000'>办公邮箱</td><td style='border-bottom:1px solid #000'>私人邮箱</td><td style='border-bottom:1px solid #000'>QQ</td><td style='border-bottom:1px solid #000'>微信</td><td style='border-bottom:1px solid #000'>生日</td><td style='border-bottom:1px solid #000'>备注</td></tr>";
            foreach (var c in enterprise.Customers)
            {
                html += string.Format(
                    "<tr><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{0}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{1}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{2}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{3}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{4}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{5}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{6}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{7}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{8}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{9}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{10}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{11}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{12}</td><td style='font-weight: bold; border-bottom:1px solid #000; text-align: center'>{13}</td></tr>",
                    c.DepartmentName,
                    c.ProductCategory,
                    c.ProductName,
                    c.Name,
                    c.Sex == Sex.Male?"男" :"女",
                    c.Tel,
                    c.Fax,
                    c.Phone,
                    c.OfficeEmail,
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