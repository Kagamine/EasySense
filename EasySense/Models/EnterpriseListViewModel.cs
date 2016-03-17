using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class EnterpriseListViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Title0 { get; set; }
        public string Level { get; set; }
        public static implicit operator EnterpriseListViewModel(EnterpriseModel Enterprise)
        {
            string title = null;
            if (Enterprise.Key != null && Enterprise.Key.Trim().Length > 0)
            {
                title = Enterprise.Key.Trim()[0] + "-" + Enterprise.Title;
            }
            else
            {
                title = Enterprise.Title;
                if (title != null && title.Trim().Length > 0)
                {
                    title = title.Trim()[0] + "-" + title.Trim();
                }
            }

            return new EnterpriseListViewModel
            {
                ID = Enterprise.ID,
                Level = Enterprise.Level.ToString(),
                Title = title,
                Title0 = Enterprise.Title
            };
        }
    }
}