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
            if (user == null || user.Password != pwd )
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
            //TODO: 
        }
    }
}