using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public enum Sex
    {
        Male,
        Female
    }

    [Table("Customers")]
    public class CustomerModel
    {
        public int ID { get; set; }

        [StringLength(16)]
        [Required]
        public string Name { get; set; }

        public Sex Sex { get; set; }

        [StringLength(32)]
        [Required]
        public string Position { get; set; }

        [StringLength(32)]
        [Required]
        public string Tel { get; set; }

        [StringLength(32)]
        [Required]
        public string Fax { get; set; }

        [StringLength(32)]
        [Required]
        public string Phone { get; set; }

        [StringLength(32)]
        [Required]
        public string Email { get; set; }

        [StringLength(32)]
        [Required]
        public string QQ { get; set; }

        [StringLength(32)]
        [Required]
        public string WeChat { get; set; }

        [StringLength(32)]
        [Required]
        public string Hint { get; set; }

        [ForeignKey("Enterprise")]
        public int EnterpriseID { get; set; }

        public DateTime Birthday { get; set; }

        public virtual EnterpriseModel Enterprise { get; set; }

        public virtual ICollection<ProjectModel> Projects { get; set; }
    }
}