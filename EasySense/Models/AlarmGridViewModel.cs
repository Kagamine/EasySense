using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class AlarmGridViewModel
    {
        public string title { get; set; }

        public string start { get; set; }

        public string end { get; set; }

        public bool allDay { get; set; }

        public string url { get; set; }

        public static implicit operator AlarmGridViewModel(AlarmModel Alarm)
        {
            return new AlarmGridViewModel
            {
                title = Alarm.Title,
                start = Helpers.Time.ToTimeStamp(Alarm.Begin),
                end = Helpers.Time.ToTimeStamp(Alarm.End),
                allDay = false,
                url = "/Alarm/Edit/" + Alarm.ID
            };
        }
    }
}