using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class EmployeeViewModel
    {
        public int ID { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public int? DepartmentID { get; set; }

        public UserRole Role { get; set; }

        public static implicit operator EmployeeViewModel(UserModel User)
        {
            return new EmployeeViewModel
            {
                ID = User.ID,
                Username = User.Username,
                Email = User.Email,
                Name = User.Name,
                Role = User.Role,
                DepartmentID = User.DepartmentID
            };
        }
    }
}