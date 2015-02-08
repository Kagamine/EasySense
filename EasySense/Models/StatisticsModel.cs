using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Statistics")]
    public class StatisticsModel
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public UserRole? PushTo { get; set; }

        public string Hint { get; set; }

        public DateTime? Begin { get; set; }
        
        public DateTime? End { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public virtual UserModel User { get; set; }

        public ProjectStatus? Status { get; set; }

        [Index]
        public DateTime Time { get; set; }

        [NotMapped]
        public string PushToDisplay
        {
            get
            {
                if (PushTo == null)
                    return "所有人";
                else if (PushTo == UserRole.Finance)
                    return "财务专员";
                else
                    return "部门主任";
            }
        }

        public byte[] ExcelBlob { get; set; }

        public byte[] PDFBlob { get; set; }

        public byte[] GraphicsBlob { get; set; }

        public string HtmlPreview { get; set; }
    }
}