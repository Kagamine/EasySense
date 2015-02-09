using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class AccessToReportAttribute : BaseAuthorizeAttribute
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
                    if (user.Role >= UserRole.Root) return true;
                    var UserID = Convert.ToInt32(((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["id"].ToString());
                    if(UserID == user.ID)
                        return true;
                }
            }
            return false;
        }
    }
}