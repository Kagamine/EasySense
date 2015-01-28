using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EasySense.Models;

namespace EasySense.Controllers
{
    public class BaseController : Controller
    {
        public readonly EasySenseContext DB = new EasySenseContext();
        public UserModel CurrentUser = null;
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = (from u in DB.Users
                               where u.Username == requestContext.HttpContext.User.Identity.Name
                               select u).Single();
            }
            ViewBag.CurrentUser = CurrentUser;
        }
    }
}