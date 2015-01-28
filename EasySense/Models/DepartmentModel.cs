using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Departments")]
    public class DepartmentModel
    {
        public int ID { get; set; }

        [StringLength(32)]
        [Required]
        public string Title { get; set; }

        public virtual ICollection<UserModel> Users { get; set; }
    }
}