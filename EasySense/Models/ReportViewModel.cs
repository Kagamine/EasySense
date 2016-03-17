using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

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

        public string TodoListPart
        {
            get
            {
                string list = TodoList;
                if (list != null)
                {
                    list = list.Trim();
                    string[] lines = list.Split('\r');
                    if (lines.Length > 0)
                    {
                        list = lines[0].Trim();
                        list = ClearHtml(list);
                        if (list.Length > 20)
                        {
                            list = list.Substring(0, 20);
                        }
                    }
                }
                if (list == null)
                {
                    list = "";
                }
                return list;
            }
        }

        /// <summary>
        /// 清除文本中Html的标签
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        protected string ClearHtml(string Content)
        {
            Content = ReplaceHtml("&#[^>]*;", "", Content);
            Content = ReplaceHtml("</?marquee[^>]*>", "", Content);
            Content = ReplaceHtml("</?object[^>]*>", "", Content);
            Content = ReplaceHtml("</?param[^>]*>", "", Content);
            Content = ReplaceHtml("</?embed[^>]*>", "", Content);
            Content = ReplaceHtml("</?table[^>]*>", "", Content);
            //Content = ReplaceHtml(" ", "", Content);
            Content = ReplaceHtml("</?tr[^>]*>", "", Content);
            Content = ReplaceHtml("</?th[^>]*>", "", Content);
            Content = ReplaceHtml("</?p[^>]*>", "", Content);
            Content = ReplaceHtml("</?a[^>]*>", "", Content);
            Content = ReplaceHtml("</?img[^>]*>", "", Content);
            Content = ReplaceHtml("</?tbody[^>]*>", "", Content);
            Content = ReplaceHtml("</?li[^>]*>", "", Content);
            Content = ReplaceHtml("</?span[^>]*>", "", Content);
            Content = ReplaceHtml("</?div[^>]*>", "", Content);
            Content = ReplaceHtml("</?th[^>]*>", "", Content);
            Content = ReplaceHtml("</?td[^>]*>", "", Content);
            Content = ReplaceHtml("</?script[^>]*>", "", Content);
            Content = ReplaceHtml("(javascript|jscript|vbscript|vbs):", "", Content);
            Content = ReplaceHtml("on(mouse|exit|error|click|key)", "", Content);
            Content = ReplaceHtml("<\\?xml[^>]*>", "", Content);
            Content = ReplaceHtml("<\\/?[a-z]+:[^>]*>", "", Content);
            Content = ReplaceHtml("</?font[^>]*>", "", Content);
            Content = ReplaceHtml("</?b[^>]*>", "", Content);
            Content = ReplaceHtml("</?u[^>]*>", "", Content);
            Content = ReplaceHtml("</?i[^>]*>", "", Content);
            Content = ReplaceHtml("</?strong[^>]*>", "", Content);
            string clearHtml = Content;
            return clearHtml;
        }

        /// <summary>
        /// 清除文本中的Html标签
        /// </summary>
        /// <param name="patrn">要替换的标签正则表达式</param>
        /// <param name="strRep">替换为的内容</param>
        /// <param name="content">要替换的内容</param>
        /// <returns></returns>
        private string ReplaceHtml(string patrn, string strRep, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                content = "";
            }
            Regex rgEx = new Regex(patrn, RegexOptions.IgnoreCase);
            string strTxt = rgEx.Replace(content, strRep);
            return strTxt;
        }

        public string FinishedListPart
        {
            get
            {
                string list = FinishedList;
                if (list != null)
                {
                    list = list.Trim();
                    string[] lines = list.Split('\r');
                    if (lines.Length > 0)
                    {
                        list = lines[0].Trim();
                        list = ClearHtml(list);
                        if (list.Length > 20)
                        {
                            list = list.Substring(0, 20);
                        }
                    }
                }
                if (list == null)
                {
                    list = "";
                }
                return list;
            }
        }

        public string FinishedList { get; set; }

        public string QuestionList { get; set; }

        public string QuestionListPart
        {
            get
            {
                string list = QuestionList;
                if (list != null)
                {
                    list = list.Trim();
                    string[] lines = list.Split('\r');
                    if (lines.Length > 0)
                    {
                        list = lines[0].Trim();
                        list = ClearHtml(list);
                        if (list.Length > 20)
                        {
                            list = list.Substring(0, 20);
                        }
                    }
                }
                if (list == null)
                {
                    list = "";
                }
                return list;
            }
        }

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