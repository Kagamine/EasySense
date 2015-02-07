using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Statistics")]
    public class StatisticsModel
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public UserRole? PushTo { get; set; }

        public string Hint { get; set; }

        public DateTime? Begin { get; set; }
        
        public DateTime? End { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual UserModel User { get; set; }

        public ProjectStatus? Status { get; set; }

        [Index]
        public DateTime Time { get; set; }
    }
}