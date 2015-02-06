using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class AlarmViewModel
    {
        public Guid ID { get; set; }

        public string Title { get; set; }

        public string Hint { get; set; }

        public string Begin { get; set; }

        public string End { get; set; }

        public int? Remind { get; set; }

        public static implicit operator AlarmViewModel(AlarmModel Alarm)
        {
            return new AlarmViewModel
            {
                ID = Alarm.ID,
                Begin = Alarm.Begin.ToString(),
                End = Alarm.End.ToString(),
                Hint = Alarm.Hint,
                Remind = Alarm.Remind,
                Title = Alarm.Title
            };
        }
    }
}