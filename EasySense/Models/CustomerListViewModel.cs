using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class CustomerListViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        public string Position { get; set; }

        public string Tel { get; set; }

        public string Fax { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string QQ { get; set; }

        public string WeChat { get; set; }

        public string Hint { get; set; }

        public string DepartmentName { get; set; }

        public string ProductCategory { get; set; }

        public string ProductName { get; set; }

        public string OfficeEmail { get; set; }

        public int EnterpriseID { get; set; }

        public string Birthday { get; set; }

        public EnterpriseModel Enterprise { get; set; }

        public ICollection<ProjectListViewModel> Projects { get; set; }

        public static implicit operator CustomerListViewModel(CustomerModel Customer)
        {
            var sex = (Customer.Sex == EasySense.Models.Sex.Male ? "男" : "女");
            
            var data = new List<ProjectListViewModel>();
            foreach (var p in Customer.Projects)
                data.Add((ProjectListViewModel)p);

            return new CustomerListViewModel
            {
                ID = Customer.ID,
                DepartmentName = Customer.DepartmentName,
                ProductCategory = Customer.ProductCategory,
                ProductName = Customer.ProductName,
                OfficeEmail = Customer.OfficeEmail,
                Name = Customer.Name,
                Sex = sex,
                Position = Customer.Position,
                Tel = Customer.Tel,
                Phone = Customer.Phone,
                Email = Customer.Email,
                QQ = Customer.QQ,
                WeChat = Customer.WeChat,
                Hint = Customer.Hint,
                EnterpriseID = Customer.EnterpriseID,
                Birthday = Customer.Birthday == null ? "" : Customer.Birthday.ToString("yyyy-MM-dd"),
                Projects = data
            };
        }
    }
}