using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class ReportViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ReportType Type { get; set; }

        public int Year { get; set; }

        public int? Month { get; set; }

        public int? Week { get; set; }

        public int? Day { get; set; }

        public string TodoList { get; set; }

        public string FinishedList { get; set; }

        public string QuestionList { get; set; }

        public static implicit operator ReportViewModel(ReportModel Report)
        {
            return new ReportViewModel
            {
                ID = Report.ID,
                Day = Report.Day,
                Month = Report.Month,
                Year = Report.Year,
                Week = Report.Week,
                Name = Report.User.Name,
                Type = Report.Type,
                FinishedList = Report.FinishedList,
                QuestionList = Report.QuestionList,
                TodoList = Report.TodoList
            };
        }
    }
}