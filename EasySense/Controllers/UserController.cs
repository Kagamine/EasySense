using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySense.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 86400)]
        public ActionResult Avatar(int id)
        {
            var user = DB.Users.Find(id);
            if (user.Avatar == null)
                return File(System.IO.File.ReadAllBytes(Server.MapPath("~/Images/Avatar.png")), "image/png");
            return File(user.Avatar, "image/png");
        }
    }
}