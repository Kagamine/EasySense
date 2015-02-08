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
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            return View(CurrentUser);
        }

        [OutputCache(Duration = 86400)]
        public ActionResult Avatar(int id)
        {
            var user = DB.Users.Find(id);
            if (user.Avatar == null)
                return File(System.IO.File.ReadAllBytes(Server.MapPath("~/Images/Avatar.png")), "image/png");
            return File(user.Avatar, "image/png");
        }

        public ActionResult Edit(string Name, string Email, string OldPassword, string Password, string ConfirmPassword)
        {
            CurrentUser.Email = Email;
            CurrentUser.Name = Name;
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                var timestamp = Helpers.String.ToTimeStamp(DateTime.Now);
                var filename = timestamp + ".tmp";
                var dir = Server.MapPath("~") + @"\Temp\";
                file.SaveAs(dir + filename);
                CurrentUser.Avatar = System.IO.File.ReadAllBytes(dir + filename);
                System.IO.File.Delete(dir + filename);
            }
            if (!string.IsNullOrEmpty(OldPassword))
            {
                if (CurrentUser.Password != Helpers.Security.SHA256(OldPassword))
                    return RedirectToAction("Message", "Shared", new { msg = "旧密码输入不正确" });
                if(ConfirmPassword != Password)
                    return RedirectToAction("Message", "Shared", new { msg = "两次密码输入不一致" });
                CurrentUser.Password = Helpers.Security.SHA256(Password);
            }
            DB.SaveChanges();
            return RedirectToAction("Index", "User");
        }
    }
}