using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class ProductViewModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public static implicit operator ProductViewModel(ProductModel Model)
        {
            return new ProductViewModel
            {
                ID = Model.ID,
                Title = Model.Title
            };
        }
    }
}