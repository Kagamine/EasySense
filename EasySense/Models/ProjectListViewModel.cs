﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySense.Models
{
    public class ProjectListViewModel
    {
        public int ID { get; set; }

        public string RefNum { get; set; }

        public string Owner { get; set; }

        public string Title { get; set; }

        public string Charge { get; set; }

        public string SignTime { get; set; }

        public string Product { get; set; }
        public string Enterprise { get; set; }

        public string Customer { get; set; }

        public string Brand { get; set; }

        public string Status { get; set; }

        public string InvoiceTime { get; set; }

        public string ChargeTime { get; set; }

        public static implicit operator ProjectListViewModel(ProjectModel Project)
        {
            var status = "";
            switch (Project.Status)
            {
                case ProjectStatus.Current:
                    status = "当前";
                    break;
                case ProjectStatus.Completed:
                    status = "完成";
                    break;
                case ProjectStatus.Bidding:
                    status = "竞标";
                    break;
                case ProjectStatus.Dumped:
                    status = "废弃";
                    break;
                default:
                    break;
            }
            return new ProjectListViewModel
            {
                ID = Project.ID,
                RefNum = Project.RefNum,
                Owner = Project.User.Name,
                Title = Project.Title,
                Charge = Project.Charge == null ? "未填写" : Project.Charge.Value.ToString("0.00"),
                SignTime = Project.SignTime == null ? "未签订" : Project.SignTime.Value.ToString("yyyy-MM-dd"),
                Product = Project.Product == null ? "未指定" : Project.Product.Title,
                Enterprise = Project.Enterprise == null ? "未指定" : Project.Enterprise.Title,
                Customer = Project.Customer == null ? "未指定" : Project.Customer.Name,
                Brand = Project.Brand,
                Status = status,
                InvoiceTime = Project.InvoiceTime == null ? "未开票" : Project.InvoiceTime.Value.ToString("yyyy-MM-dd"),
                ChargeTime = Project.ChargeTime == null ? "未付款" : Project.ChargeTime.Value.ToString("yyyy-MM-dd")
            };
        }
    }
}