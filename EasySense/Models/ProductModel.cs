using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Products")]
    public class ProductModel
    {
        public int ID { get; set; }

        [StringLength(64)]
        [Index]
        public string Title { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public virtual CategoryModel Category { get; set; }
    }
}