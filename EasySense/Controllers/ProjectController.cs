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
        public ActionResult Edit(int id, ProjectModel Model)
        {
            var project = DB.Projects.Find(id);
            //TODO: 把表单提交的信息存入数据库，注意只有Finance级和Root级可以修改财务信息，如果不是这个级别的直接忽略相关字段，前台只要name属性和Model里的属性对应上就从Model参数里读

            project.Log = string.Format("[{0}] {1}({2}) 修改了项目\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), CurrentUser.Name, CurrentUser.Username);
            DB.SaveChanges();
            return Content("项目信息修改成功！");
        }
    }
}