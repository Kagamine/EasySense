using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class RoleAttribute : BaseAuthorizeAttribute
    {
        private UserRole Role { get; set; }

        public RoleAttribute(UserRole Role)
        {
            this.Role = Role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                using (var db = new EasySenseContext())
                {
                    var user = (from u in db.Users
                                where u.Username == httpContext.User.Identity.Name
                                && u.Role == Role
                                select u).SingleOrDefault();
                    if (user != null)
                        return true;
                }
            }
            return base.AuthorizeCore(httpContext);
        }
    }
}