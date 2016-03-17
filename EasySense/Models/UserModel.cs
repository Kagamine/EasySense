using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    public enum UserRole
    {
        Employee,
        Master,
        Finance,
        Root
    }

    [Table("Users")]
    public class UserModel
    {
        public int ID { get; set; }

        [StringLength(20)]
        [Index(IsUnique = true)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [StringLength(16)]
        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string DepartmentTitle
        {
            get
            {
                if (Department != null)
                {
                    return Department.Title;
                }
                else
                {
                    return null;
                }
            }
        }

        [StringLength(8)]
        [Index]
        [Required]
        public string Key { get; set; }

        [Index]
        public UserRole Role { get; set; }

        public byte[] Avatar { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentID { get; set; }

        public virtual DepartmentModel Department { get; set; }

        [StringLength(64)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        public DateTime InsertTime { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public virtual ICollection<ProjectModel> Projects { get; set; }

        public virtual ICollection<AlarmModel> Alarms { get; set; }

        public virtual ICollection<ReportModel> Reports { get; set; }

        public virtual ICollection<FileModel> Files { get; set; }
    }
}