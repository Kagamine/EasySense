using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class NotificationViewModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public DateTime? Time { get; set; }

        public static NotificationViewModel BuildFinanceNotification(ProjectModel Project)
        {
            return new NotificationViewModel
            {
                ID = Project.ID.ToString(),
                Title = Project.Title,
                Time = Project.End
            };
        }

        public static implicit operator NotificationViewModel(ProjectModel Project)
        {
            return new NotificationViewModel
            {
                ID = Project.ID.ToString(),
                Title = Project.Title
            };
        }

        public static implicit operator NotificationViewModel(CustomerModel Customer)
        {
            return new NotificationViewModel
            {
                ID = Customer.ID.ToString(),
                Title = Customer.Name,
                Time = Customer.Birthday
            };
        }

        public static implicit operator NotificationViewModel(AlarmModel Alarm)
        {
            return new NotificationViewModel
            {
                ID = Alarm.ID.ToString(),
                Title = Alarm.Title,
                Time = Alarm.Begin
            };
        }
    }
}