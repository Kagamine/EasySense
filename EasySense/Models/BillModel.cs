using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Bills")]
    public class BillModel
    {
        public Guid ID { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public virtual ProjectModel Project { get; set; }

        public decimal Plan { get; set; }

        public decimal Actual { get; set; }

        public DateTime Time { get; set; }

        public string Hint { get; set; }

        public int Type { get; set; }
    }
}