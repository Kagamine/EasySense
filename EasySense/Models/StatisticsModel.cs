using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public class StatisticsModel
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public UserRole? PushTo { get; set; }

        public string Hint { get; set; }

        public 
    }
}