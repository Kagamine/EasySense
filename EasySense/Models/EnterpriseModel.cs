using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public enum EnterpriseLevel
    {
        A,
        B,
        C,
        D
    }

    [Table("Enterprises")]
    public class EnterpriseModel
    {
        public int ID { get; set; }

        [StringLength(128)]
        [Required]
        [Index]
        public string Title { get; set; }

        [StringLength(64)]
        [Index]
        public string Key { get; set; }

        [Index]
        public EnterpriseLevel Level { get; set; }

        [StringLength(32)]
        [Required]
        public string Phone { get; set; }

        [StringLength(32)]
        [Required]
        public string Fax { get; set; }

        [StringLength(16)]
        [Required]
        public string Zip { get; set; }

        [StringLength(64)]
        [Required]
        public string Website { get; set; }

        [Required]
        public string Hint { get; set; }

        [StringLength(32)]
        [Required]
        public string Property { get; set; }

        [StringLength(32)]
        [Required]
        public string Type { get; set; }

        [StringLength(32)]
        [Required]
        public string Scale { get; set; }

        [StringLength(32)]
        [Required]
        public string SalesVolume { get; set; }

        public virtual ICollection<CustomerModel> Customers { get; set; }
    }
}