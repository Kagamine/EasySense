using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Zones")]
    public class ZoneModel
    {
        public int ID { get; set; }

        [StringLength(64)]
        [Index(IsUnique = true)]
        public string Title { get; set; }

        public override bool Equals(object obj)
        {
            var data = obj as ZoneModel;
            if (data.ID == this.ID) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }
    }
}