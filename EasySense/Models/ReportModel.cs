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

    [Table("reports")]
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

        public string TodoList { get; set; }

        public string FinishedList { get; set; }

        public string QuestionList { get; set; }

        [Index]
        public DateTime Time { get; set; }

        [NotMapped]
        public int Month0 { get; set; }

        [NotMapped]
        public int Week0 { get; set; }

        [NotMapped]
        public string Date0 { get; set; }

        public override bool Equals(object obj)
        {
            var data = obj as ReportModel;
            if (data.ID == this.ID) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}