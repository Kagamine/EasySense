using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class DepartmentViewModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int? UserID { get; set; }

        public static implicit operator DepartmentViewModel(DepartmentModel Model)
        {
            return new DepartmentViewModel
            {
                ID = Model.ID,
                Title = Model.Title,
                UserID = Model.UserID
            };
        }
    }
}