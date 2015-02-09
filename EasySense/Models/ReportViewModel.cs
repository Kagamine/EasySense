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

        public string Time { get; set; }

        public string Start
        {
            get
            {
                if (Type == ReportType.Day)
                {
                    string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
                    return Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();
                }
                return "";
            }
        }

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
                TodoList = Report.TodoList,
                Time = Report.Time.ToString("yyyy-MM-dd HH:mm")
            };
        }
    }
}