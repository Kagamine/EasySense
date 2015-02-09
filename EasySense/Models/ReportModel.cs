using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public enum ReportType
    {
        Day,
        Week,
        Month
    }

    public class ReportModel
    {
        public int ID { get; set; }

        public ReportType Type { get; set; }

        [Index]
        public int Year { get; set; }

        [Index]
        public int? Month { get; set; }

        [Index]
        public int? Week { get; set; }

        [Index]
        public int? Day { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual UserModel User { get; set; }

        [Required]
        public string TodoList { get; set; }

        [Required]
        public string FinishedList { get; set; }

        [Required]
        public string QuestionList { get; set; }

        [Index]
        public DateTime Time { get; set; }
    }
}