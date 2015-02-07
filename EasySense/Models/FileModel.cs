using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public enum FileCategory
    {
        Training,
        Institution,
        Plan,
        Document
    }

    [Table("Files")]
    public class FileModel
    {
        public Guid ID { get; set; }

        public byte[] FileBlob { get; set; }

        [StringLength(64)]
        [Required]
        public string ContentType { get; set; }

        [StringLength(64)]
        [Required]
        public string Filename { get; set; }

        [StringLength(16)]
        [Required]
        public string Extension { get; set; }

        [Index]
        public DateTime Time { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual UserModel User { get; set; }

        [Index]
        public FileCategory FileCategory { get; set; }

        [NotMapped]
        public string FileCategoryAsString
        {
            get
            {
                switch (FileCategory)
                {
                    case FileCategory.Training:
                        return "销售培训";
                    case FileCategory.Institution:
                        return "销售制度";
                    case FileCategory.Plan:
                        return "销售计划";
                    case FileCategory.Document:
                        return "销售文档";
                    default: return "";
                }
            }
        }
    }
}