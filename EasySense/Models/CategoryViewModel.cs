using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class CategoryViewModel
    {
        public int ID { get; set; }
         
        public string Title { get; set; }

        public float SaleAllocRatio { get; set; }

        public float AwardAllocRatio { get; set; }

        public float TaxRatio { get; set; }

        public static implicit operator CategoryViewModel(CategoryModel Category)
        {
            return new CategoryViewModel
            {
                ID = Category.ID,
                Title = Category.Title,
                SaleAllocRatio = Category.SaleAllocRatio,
                AwardAllocRatio = Category.AwardAllocRatio,
                TaxRatio = Category.TaxRatio
            };
        }
    }
}