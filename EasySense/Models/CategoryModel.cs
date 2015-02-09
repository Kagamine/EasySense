using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Categories")]
    public class CategoryModel
    {
        public int ID { get; set; }

        [StringLength(64)]
        [Index]
        public string Title { get; set; }

        public float SaleAllocRatio { get; set; }

        public float AwardAllocRatio { get; set; }

        public float ProfitAllocRatio { get; set; }

        public float TaxRatio { get; set; }

        public virtual ICollection<ProductModel> Products { get; set; }
    }
}