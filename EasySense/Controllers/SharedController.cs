using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using EasySense.Models;
using EasySense.Schema;

namespace EasySense.Controllers
{
    public class SharedController : BaseController
    {
        // GET: Shared
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public ActionResult Login(string Username, string Password, bool Remember)
        {
            if (User.Identity.IsAuthenticated == true)
            {
                return Redirect("/");
            }
            UserModel user;
            if (Username.IndexOf("@") > 0)
                user = (from u in DB.Users
                        where u.Email == Username
                        select u).SingleOrDefault();
            else
                user = (from u in DB.Users
                        where u.Username == Username
                        select u).SingleOrDefault();
            var pwd = Helpers.Security.SHA256(Password);
            if (user == null || user.Password != pwd)
            {
                ViewBag.Info = "用户名或密码错误！";
                return View();
            }
            else
            {
                FormsAuthentication.SetAuthCookie(user.Username, Remember);
                user.LastLoginTime = DateTime.Now;
                DB.SaveChanges();
                if (Request.UrlReferrer == null)
                    return Redirect("/");
                else
                    return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [Route("Logout")]
        [ValidateSID]
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        [Route("Search")]
        [HttpGet]
        public ActionResult Search(string Text)
        {        
            //TODO: 完成超级搜索逻辑，适用于搜索ID和子串关系的标题
            var result = new SuperSearchViewModel();
            result.Users = new List<SuperSearchUserViewModel>();
            result.Projects = new List<SuperSearchProjectViewModel>();
            result.Enterprises = new List<SuperSearchEnterpriseViewModel>();
            result.Customers = new List<SuperSearchCustomerViewModel>();
            result.Files = new List<SuperSearchFileViewModel>();

            var users = new List<UserModel>();
            var projects = new List<ProjectModel>();
            var enterprises = new List<EnterpriseModel>();
            var customers = new List<CustomerModel>();
            var files = new List<FileModel>();

            users = (from u in DB.Users where  u.Username.Contains(Text) || u.ID.ToString().Equals(Text) select u).ToList();
            projects = (from p in DB.Projects where p.Title.Contains(Text) || p.ID.ToString().Equals(Text) select p).ToList();
            enterprises = (from e in DB.Enterprises where e.Title.Contains(Text) || e.ID.ToString().Equals(Text) select e).ToList();
            customers = (from c in DB.Customers where c.Name.Contains(Text) || c.ID.ToString().Equals(Text) select c).ToList();
            files = (from f in DB.Files where f.Filename.Contains(Text) select f).ToList();

            foreach (var u in users)
            {
                result.Users.Add((SuperSearchUserViewModel)u);
            }
            foreach(var project in projects)
            {
                result.Projects.Add((SuperSearchProjectViewModel)project);
            }
            foreach (var en in enterprises)
            {
                result.Enterprises.Add((SuperSearchEnterpriseViewModel)en);
            }
            foreach (var customer in customers)
            {
                result.Customers.Add((SuperSearchCustomerViewModel)customer);
            }
            foreach(var file in files)
            {
                result.Files.Add((SuperSearchFileViewModel)file);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}