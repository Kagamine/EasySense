using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

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

        public ActionResult Create(EnterpriseModel Model)
        {
            Model.Key = Helpers.Pinyin.Convert(Model.Title);
            DB.Enterprises.Add(Model);
            DB.SaveChanges();
            return Content(Model.ID.ToString());
        }
    }
}