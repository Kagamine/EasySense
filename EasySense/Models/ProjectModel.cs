using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public enum ProjectStatus {
        Current,
        Completed,
        Bidding,
        Dumped
    }

    public enum ProjectPriority
    {
        Normal,
        Medium,
        High
    }

    public enum PayMethod
    {
        Unpaid,
        Cash,
        Transfer
    }

    [Table("Projects")]
    public class ProjectModel
    {
        //Overview begin
        public int ID { get; set; }

        [StringLength(128)]
        [Index]
        public string Title { get; set; }

        public string Description { get; set; }

        [Index]
        public DateTime Begin { get; set; }

        [Index]
        public DateTime End { get; set; }

        [Index]
        public ProjectPriority Priority { get; set; }

        [Index]
        public ProjectStatus Status { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual UserModel User { get; set; }

        [Index]
        public float Percent { get; set; }

        //Customer begin
        [ForeignKey("Enterprise")]
        public int? EnterpriseID { get; set; }

        public virtual EnterpriseModel Enterprise { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerID { get; set; }

        public virtual CustomerModel Customer { get; set; }

        //Project settings begin
        [Index]
        public bool Ordering { get; set; }

        public DateTime? SignTime { get; set; }

        [ForeignKey("Product")]
        public int? ProductID { get; set; }

        public virtual ProductModel Product { get; set; }

        [ForeignKey("Zone")]
        public int? ZoneID { get; set; }

        public virtual ZoneModel Zone { get; set; }

        //Finance begin
        public decimal? Charge { get; set; }

        public float? SaleAllocRatioCache { get; set; }

        public float? AwardAllocRatioCache { get; set; }

        public float? TaxRatioCache { get; set; }

        [NotMapped]
        public string Tel { get; set; }

        [NotMapped]
        public string Phone { get; set; }

        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        public string Brand { get; set; }

        [NotMapped]
        public decimal SellingCommisson
        {
            get
            {
                if (Charge != null && SaleAllocRatioCache != null)
                    return Charge.Value * (decimal)SaleAllocRatioCache.Value;
                else
                    return 0;
            }
        }

        [NotMapped]
        public decimal Award
        {
            get
            {
                if (Charge != null && AwardAllocRatioCache != null)
                    return Charge.Value * (decimal)AwardAllocRatioCache.Value;
                else
                    return 0;
            }
        }

        [NotMapped]
        public decimal Profit
        {
            get
            {
                if (Charge.HasValue)
                    return (decimal)ProfitRatio * Charge.Value;
                else
                    return 0;
            }
        }

        [NotMapped]
        public decimal Tax
        {
            get
            {
                if (Charge != null && TaxRatioCache != null)
                    return Charge.Value * (decimal)TaxRatioCache.Value;
                else
                    return 0;
            }
        }

        [NotMapped]
        public float ProfitRatio
        {
            get
            {
                if (Charge == 0)
                    return 0;
                var ret = 1f;
                if (AwardAllocRatioCache.HasValue)
                    ret -= AwardAllocRatioCache.Value;
                if (SaleAllocRatioCache.HasValue)
                    ret -= SaleAllocRatioCache.Value;
                if (TaxRatioCache.HasValue)
                    ret -= TaxRatioCache.Value;
                if (Bills != null)
                {
                    var bills = Bills.Sum(x => x.Actual);
                    if (Charge.HasValue)
                        ret -= (float)(bills / Charge.Value);
                }
                return ret;
            }
        }

        //Invoice begin
        public decimal? InvoicePrice { get; set; }

        public DateTime? InvoiceTime { get; set; }

        [StringLength(256)]
        public string InvoiceSN { get; set; }

        public string Hint { get; set; }

        public DateTime? ChargeTime { get; set; }

        public decimal? ActualPayments { get; set; }

        [Index]
        public PayMethod PayMethod { get; set; }

        public string Log { get; set; }

        public string RefNum { get; set; }

        public virtual ICollection<BillModel> Bills { get; set; }

        public override bool Equals(object obj)
        {
            var data = obj as ProjectModel;
            if (data.ID == this.ID) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}