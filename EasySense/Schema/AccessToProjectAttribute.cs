using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class AccessToProjectAttribute : BaseAuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                using (var db = new EasySenseContext())
                {
                    var user = (from u in db.Users
                                where u.Username == httpContext.User.Identity.Name
                                select u).Single();
                    if (user.Role >= UserRole.Finance) return true;
                    var id = 0;
                    if (((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["id"] != null)
                        id = Convert.ToInt32(((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["id"]);
                    else
                        id = Convert.ToInt32(httpContext.Request.Form["id"]);
                    var project = db.Projects.Find(id);
                    if (user.Role == UserRole.Master && project.User.Department.UserID == user.ID)
                        return true;
                    if (project.UserID == user.ID)
                        return true;
                }
            }
            return false;
        }
    }
}