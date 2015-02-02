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
        public string Level { get; set; }
        public static implicit operator EnterpriseListViewModel(EnterpriseModel Enterprise)
        {
            return new EnterpriseListViewModel
            {
                ID = Enterprise.ID,
                Level = Enterprise.Level.ToString(),
                Title = Enterprise.Key[0] + "-" + Enterprise.Title
            };
        }
    }
}