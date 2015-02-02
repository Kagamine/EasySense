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
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}