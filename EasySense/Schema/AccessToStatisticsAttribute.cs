using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class AccessToStatisticsAttribute : BaseAuthorizeAttribute
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
                    var StatisticsID = Guid.Parse(((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["id"].ToString());
                    var statistics = db.Statistics.Find(StatisticsID);
                    if (statistics.PushTo == null)
                        return true;
                    if (statistics.PushTo == UserRole.Finance && user.Role == UserRole.Finance)
                        return true;
                    if (statistics.PushTo == UserRole.Master && user.Role == UserRole.Master)
                        return true;
                }
            }
            return false;
        }
    }
}