using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
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

        public bool Public { get; set; }

        [Index]
        public DateTime Time { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public virtual UserModel User { get; set; }
    }
}