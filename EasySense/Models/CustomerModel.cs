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
        //[Required]
        public string Position { get; set; }

        [StringLength(32)]
        //[Required]
        public string Tel { get; set; }

        [StringLength(32)]
        //[Required]
        public string Fax { get; set; }

        [StringLength(32)]
        //[Required]
        public string Phone { get; set; }

        [StringLength(32)]
        //[Required]
        public string Email { get; set; }

        [StringLength(32)]
        //[Required]
        public string QQ { get; set; }

        [StringLength(32)]
        //[Required]
        public string WeChat { get; set; }

        [StringLength(32)]
        //[Required]
        public string Hint { get; set; }

        [StringLength(50)]
        public string DepartmentName { get; set; }

        [StringLength(50)]
        public string ProductCategory { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string OfficeEmail { get; set; }

        [ForeignKey("Enterprise")]
        public int EnterpriseID { get; set; }

        public DateTime Birthday { get; set; }

        public virtual EnterpriseModel Enterprise { get; set; }

        public virtual ICollection<ProjectModel> Projects { get; set; }

        public override bool Equals(object obj)
        {
            var data = obj as CustomerModel;
            if (data.ID == this.ID) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}