using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class JQChartViewModel
    {
        public string type { get; set; }

        public string title { get; set; }

        public List<dynamic[]> data { get; set; }

        public void PushData(string Key, int Value)
        {
            dynamic[] a = new dynamic[2];
            a[0] = Key;
            a[1] = Value;
            data.Add(a);
        }
    }
}