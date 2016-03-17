using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class BillListViewModel
    {

        public Guid ID { get; set; }

        public decimal Plan { get; set; }

        public decimal Actual { get; set; }

        public string Time { get; set; }

        public string Hint { get; set; }

        public int TypeAsInt { get; set; }

        public string Type { get; set; }

        public string RefNum { get; set; }

        public static implicit operator BillListViewModel(BillModel Bill)
        {
            string refNum = null;
            if (Bill.Project != null)
            {
                refNum = Bill.Project.RefNum;
            }
            return new BillListViewModel
            {
                ID = Bill.ID,
                Hint = Bill.Hint,
                Actual = Bill.Actual,
                Time = Bill.Time.ToString("yyyy-MM-dd"),
                Plan = Bill.Plan,
                TypeAsInt = Bill.Type,
                Type = BillModel.BillTypes[Bill.Type],
                RefNum = refNum
            };
        }
    }
}