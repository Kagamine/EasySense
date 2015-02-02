using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class SuperSearchViewModel
    {
        public List<SuperSearchProjectViewModel> Projects { get; set; }
        public List<SuperSearchEnterpriseViewModel> Enterprises { get; set; }
        public List<SuperSearchCustomerViewModel> Customers { get; set; }
        public List<SuperSearchUserViewModel> Users { get; set; }
        public List<SuperSearchFileViewModel> Files { get; set; }
    }
    public class SuperSearchProjectViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public static implicit operator SuperSearchProjectViewModel(ProjectModel Project)
        {
            return new SuperSearchProjectViewModel
            {
                ID = Project.ID,
                Title = Project.Title,
            };
        }
    }
    public class SuperSearchEnterpriseViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }

        public static implicit operator SuperSearchEnterpriseViewModel(EnterpriseModel Enterprise)
        {
            return new SuperSearchEnterpriseViewModel
            {
                ID = Enterprise.ID,
                Title = Enterprise.Title,
                Level = Enterprise.Level.ToString(),
            };
        }

    }
    public class SuperSearchUserViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public static implicit operator SuperSearchUserViewModel(UserModel User)
        {
            return new SuperSearchUserViewModel
            {
                ID = User.ID,
                Name = User.Username,
            };
        }
    }
    public class SuperSearchCustomerViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Enterprise { get; set; }

        public static implicit operator SuperSearchCustomerViewModel(CustomerModel Customer)
        {
            return new SuperSearchCustomerViewModel
            {
                ID = Customer.ID,
                Name = Customer.Name,
                Enterprise = Customer.Enterprise.Title,
            };
        }
    }
    public class SuperSearchFileViewModel
    {
        public string ID { get; set; }
        public string Filename { get; set; }//without extension, e.g. "HelloWorld"
        public string Extension { get; set; }//with dot, e.g. ".exe"
        public DateTime Time { get; set; }

        public static implicit operator SuperSearchFileViewModel(FileModel File)
        {
            return new SuperSearchFileViewModel
            {
                ID = File.ID.ToString(),
                Filename = File.Filename,
                Extension = File.Extension,
                Time=File.Time,
            };
        }
    }
}