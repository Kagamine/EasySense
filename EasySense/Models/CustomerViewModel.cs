using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class CustomerViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public Sex Sex { get; set; }

        public string Position { get; set; }

        public string Tel { get; set; }

        public string Fax { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string QQ { get; set; }

        public string WeChat { get; set; }

        public string Hint { get; set; }

        public int EnterpriseID { get; set; }

        public string Birthday { get; set; }

        public static implicit operator CustomerViewModel(CustomerModel Customer)
        {
            return new CustomerViewModel
            {
                ID = Customer.ID,
                Name = Customer.Name,
                Sex = Customer.Sex,
                Position = Customer.Position,
                Tel = Customer.Tel,
                Fax = Customer.Fax,
                Phone = Customer.Phone,
                Email = Customer.Email,
                QQ = Customer.QQ,
                WeChat = Customer.WeChat,
                Hint = Customer.Hint,
                EnterpriseID = Customer.EnterpriseID,
                Birthday = Customer.Birthday.ToString()
            };
        }
    }
}