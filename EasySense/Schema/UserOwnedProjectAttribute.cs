using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class UserOwnedProjectAttribute : BaseAuthorizeAttribute
    {
        private int ProjectID { get; set; }

        public UserOwnedProjectAttribute(int ProjectID)
        {
            this.ProjectID = ProjectID;
        }

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
                    var project = db.Projects.Find(ProjectID);
                    if (project.UserID == user.ID)
                        return true;
                }
            }
            return base.AuthorizeCore(httpContext);
        }
    }
}