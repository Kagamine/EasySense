using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class StatisticsViewModel
    {
        public Guid ID { get; set; }

        public string Title { get; set; }

        public string PushTo { get; set; }

        public string Hint { get; set; }

        public string Begin { get; set; }

        public string End { get; set; }

        public Decimal? ChargeBegin { get; set; }

        public Decimal? ChargeEnd { get; set; }

        public string ProductIDs { get; set; }

        public string CustomerIDs { get; set; }

        public string Brands { get; set; }

        public string EnterpriseIDs { get; set; }

        public int? UserID { get; set; }

        public string Status { get; set; }

        public string Time { get; set; }
                
        public string EmployeeGraphics { get; set; }

        public string EnterpriseGraphics { get; set; }

        public string ExportedFields { get; set; }

        public static implicit operator StatisticsViewModel(StatisticsModel statistics)
        {
            string pushTo = null;
            if (statistics.PushTo == null)
                pushTo = "";
            else if (statistics.PushTo == UserRole.Finance)
                pushTo = "Finance";
            else
                pushTo = "Master";

            string status = null;
            if (statistics.Status == null)
                status = "";
            else if (statistics.Status == ProjectStatus.Current)
                status = "Current";
            else if (statistics.Status == ProjectStatus.Completed)
                status = "Completed";
            else if (statistics.Status == ProjectStatus.Bidding)
                status = "Bidding";
            else
                status = "Dumped";
        
            return new StatisticsViewModel
            {
                ID = statistics.ID,
                Title = statistics.Title,
                PushTo = pushTo,
                Hint = statistics.Hint,
                Begin = statistics.Begin == null ? "" : statistics.Begin.Value.ToString("yyyy-MM-dd"),
                End = statistics.End == null ? "" : statistics.End.Value.ToString("yyyy-MM-dd"),
                ChargeBegin = statistics.ChargeBegin,
                ChargeEnd = statistics.ChargeEnd,
                ProductIDs = statistics.ProductIDs,
                CustomerIDs = statistics.CustomerIDs,
                Brands = statistics.Brands,
                EnterpriseIDs = statistics.EnterpriseIDs,
                UserID = statistics.UserID,
                Status = status,
                Time = statistics.Time == null ? "" : statistics.Time.ToString("yyyy-MM-dd"),
                EmployeeGraphics = statistics.EnterpriseGraphics,
                EnterpriseGraphics = statistics.EnterpriseGraphics,
                ExportedFields = statistics.ExportedFields

            };
        }
    }
}