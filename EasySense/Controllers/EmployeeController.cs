using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Schema;
using EasySense.Models;

namespace EasySense.Controllers
{
    [Authorize]
    public class EmployeeController : BaseController
    {
        // GET: Employee
        [MinRole(UserRole.Root)]
        public ActionResult Index()
        {
            var employee = (from u in DB.Users
                            orderby u.ID descending
                            select u).ToList();
            return View(employee);
        }

        [HttpPost]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Edit(int id, UserModel Model)
        {
            var user = DB.Users.Find(id);
            user.Name = Model.Name;
            user.Role = Model.Role;
            user.Email = Model.Email;
            if (!string.IsNullOrEmpty(Model.Password))
                user.Password = Helpers.Security.SHA256(Model.Password);
            user.Key = Helpers.Pinyin.Convert(Model.Name);
            DB.SaveChanges();
            return RedirectToAction("Index", "Employee");
        }

        [HttpGet]
        [ValidateSID]
        [MinRole(UserRole.Root)]
        public ActionResult Delete(int id)
        {
            var user = DB.Users.Find(id);
            DB.Users.Remove(user);
            DB.SaveChanges();
            return RedirectToAction("Index", "Employee");
        }

        [HttpGet]
        [MinRole(UserRole.Root)]
        public ActionResult Detail(int id)
        {
            var user = DB.Users.Find(id);
            return Json((EmployeeViewModel)user, JsonRequestBehavior.AllowGet);
        }
    }
}