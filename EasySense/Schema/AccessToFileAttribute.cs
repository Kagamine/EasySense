using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasySense.Models;

namespace EasySense.Schema
{
    public class AccessToFileAttribute : BaseAuthorizeAttribute
    {
        private Guid FileID { get; set; }

        public AccessToFileAttribute(Guid FileID)
        {
            this.FileID = FileID;
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
                    if (user.Role >= UserRole.Root) return true;
                    if ((from f in db.Files where f.ID == FileID && f.Public select f).Count() > 0)
                        return true;
                }
            }
            return false;
        }
    }
}