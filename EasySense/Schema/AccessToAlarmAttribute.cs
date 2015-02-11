using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class AccessToAlarmAttribute : BaseAuthorizeAttribute
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
                    Guid AlarmID;
                    if (((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["id"] != null)
                        AlarmID = Guid.Parse(((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["id"].ToString());
                    else
                        AlarmID = Guid.Parse(httpContext.Request.Form["id"].ToString());
                    if ((from a in db.Alarms where a.UserID == user.ID && a.ID == AlarmID select a).Count() > 0)
                        return true;
                }
            }
            return false;
        }
    }
}