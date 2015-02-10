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

                #region 项目提醒
                ViewBag.ProjectNotifications = new List<NotificationViewModel>();
                IEnumerable<ProjectModel> projectNotifications = from p in DB.Projects
                                                                 where p.Percent < 1
                                                                 && DateTime.Now > p.End
                                                                 orderby p.End ascending
                                                                 select p;
                if (CurrentUser.Role == UserRole.Master)
                {
                    projectNotifications = projectNotifications.Where(x => x.User.Department.UserID == CurrentUser.ID);
                }
                else if(CurrentUser.Role == UserRole.Employee)
                {
                    projectNotifications = projectNotifications.Where(x => x.UserID == CurrentUser.ID);
                }
                projectNotifications = projectNotifications.ToList();
                foreach (var p in projectNotifications)
                    ViewBag.ProjectNotifications.Add((NotificationViewModel)p);
                #endregion
                #region 财龄提醒
                ViewBag.FinanceNotifications = new List<NotificationViewModel>();
                    ViewBag.FinanceNotifications = new List<NotificationViewModel>();
                    var financeNotifications = (from p in DB.Projects
                                                where p.Percent == 1
                                                && p.ChargeTime == null
                                                orderby p.End ascending
                                                select p).ToList();
                    foreach (var p in financeNotifications)
                        ViewBag.FinanceNotifications.Add(NotificationViewModel.BuildFinanceNotification(p));
                #endregion
                #region 日程提醒
                ViewBag.AlarmNotifications = new List<NotificationViewModel>();
                var alarmNotifications = (from a in DB.Alarms
                                          where a.UserID == CurrentUser.ID
                                          && a.Remind.HasValue
                                          && a.End >= DateTime.Now
                                          select a).ToList();
                foreach (var a in alarmNotifications)
                    ViewBag.AlarmNotifications.Add((NotificationViewModel)a);
                #endregion
                #region 生日提醒
                ViewBag.BirthdayNotifications = new List<NotificationViewModel>();
                IEnumerable<CustomerModel> birthdayNotifications = (from c in DB.Customers
                                                                    where DateTime.Now.Month == c.Birthday.Month
                                                                    && DateTime.Now.Day == c.Birthday.Day
                                                                    select c).ToList();
                if (CurrentUser.Role == UserRole.Master)
                {
                    birthdayNotifications = from c in birthdayNotifications
                                            where (from p in DB.Projects
                                                   where p.User.Department.UserID == CurrentUser.ID
                                                   select p.CustomerID).Contains(c.ID)
                                            select c;
                }
                else if (CurrentUser.Role == UserRole.Employee)
                {
                    birthdayNotifications = from c in birthdayNotifications
                                            where (from p in DB.Projects
                                                   where p.UserID == CurrentUser.ID
                                                   select p.CustomerID).Contains(c.ID)
                                            select c;
                }
                birthdayNotifications = birthdayNotifications.ToList();
                foreach (var b in birthdayNotifications)
                    ViewBag.BirthdayNotifications.Add((NotificationViewModel)b);
                #endregion
            }
            ViewBag.SID = requestContext.HttpContext.Session["SID"].ToString();
            ViewBag.CurrentUser = CurrentUser;
        }
    }
}