using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Alarms")]
    public class AlarmModel
    {
        public Guid ID { get; set; }

        [StringLength(64)]
        [Index]
        public string Title { get; set; }

        [Required]
        public string Hint { get; set; }

        [Index]
        public DateTime Begin { get; set; }

        [Index]
        public DateTime End { get; set; }

        public int? Remind { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual UserModel User { get; set; }
    }
}