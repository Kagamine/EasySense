﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Helpers
{
    public static class Time
    {
        public static string ToTimeTip(DateTime time)
        {
            var span = DateTime.Now - time;
            if (Math.Abs(span.TotalDays) > 365)
            {
                if (span.TotalDays > 0)
                    return "很久以前";
                else
                    return "很久以后";
            }
            var sec = (long)span.TotalSeconds;
            if (sec == 0)
            {
                return "刚刚";
            }
            else if (Math.Abs(sec) < 60)
            {
                if (sec > 0)
                    return sec + "秒前";
                else
                    return -sec + "秒后";
            }
            else if (Math.Abs(sec) < 60 * 60)
            {
                if (sec > 0)
                    return sec / 60 + "分钟前";
                else
                    return -sec / 60 + "分钟后";
            }
            else if (Math.Abs(sec) < 60 * 60 * 24)
            {
                if (sec > 0)
                    return sec / 60 / 60 + "小时前";
                else
                    return -sec / 60 / 60 + "小时后";
            }
            else if (Math.Abs(sec) < 60 * 60 * 24 * 30)
            {
                if (sec > 0)
                    return sec / 60 / 60 / 24 + "天前";
                else
                    return -sec / 60 / 60 / 24 + "天后";
            }
            return time.ToString("yyyy-MM-dd");
        }

        public static string ToVagueTimeLength(DateTime time1, DateTime time2)
        {
            var sec = (int)Math.Abs((time2 - time1).TotalSeconds);
            if (sec < 60)
            {
                return sec + "秒";
            }
            else if (sec < 60 * 60)
            {
                return sec / 60 + "分";
            }
            else if (sec < 60 * 60 * 24)
            {
                return sec / 60 / 60 + "小时";
            }
            else
            {
                return sec / 60 / 60 / 24 + "天";
            }
        }
        public static string ToTimeLength(DateTime time1, DateTime time2)
        {
            var sec = (int)Math.Abs((time2 - time1).TotalSeconds);
            var ret = "";
            if (sec / 60 / 60 / 24 > 0)
            {
                ret += sec / 60 / 60 / 24 + "天";
            }
            if (sec / 60 / 60 % 24 > 0)
            {
                ret += sec / 60 / 60 % 24 + "小时";
            }
            if (sec / 60 % 60 > 0)
            {
                ret += sec / 60 % 60 + "分钟";
            }
            if (sec % 60 > 0)
            {
                ret += sec % 60 + "秒";
            }
            return ret;
        }
        public static string ToContestStatus(DateTime begin, DateTime? rest_begin, DateTime? rest_end, DateTime end)
        {
            if (DateTime.Now < begin)
                return ToTimeTip(begin) + "开始";
            if (DateTime.Now > end)
                return "已经结束";
            if (rest_begin == null)
            {
                return "正在进行";
            }
            else
            {
                if (DateTime.Now < rest_begin) return "正在进行解题阶段";
                else if (DateTime.Now < rest_end) return "中场休息";
                else return "Hack阶段";
            }
        }
        public static DateTime ToDateTime(string TimeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(TimeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }

        public static DateTime ToDateTime(int TimeStamp)
        {
            return ToDateTime(TimeStamp.ToString());
        }

        public static string ToTimeStamp(System.DateTime Time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return Convert.ToInt64((Time - startTime).TotalSeconds).ToString();
        }

        public static int WeekOfYear(DateTime dateTime)
        {
            int firstdayofweek = System.Convert.ToDateTime(dateTime.Year.ToString() + "- 1-1 ").DayOfWeek.GetHashCode();
            int days = dateTime.DayOfYear;
            int daysOutOneWeek = days - (7 - firstdayofweek);
            if (daysOutOneWeek <= 0)
            {
                return 0;
            }
            else
            {
                int weeks = daysOutOneWeek / 7;
                if (daysOutOneWeek % 7 != 0)
                {
                    weeks++;
                }
                return weeks;
            }
        }

        public static int WeekCountOfYear(int year)
        {
            return WeekOfYear(Convert.ToDateTime(year + "-12-31"));
        }
    }
}