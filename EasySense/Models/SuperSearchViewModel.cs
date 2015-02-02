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
    }
    public class SuperSearchEnterpriseViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
    }
    public class SuperSearchUserViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class SuperSearchCustomerViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Enterprise { get; set; }
    }
    public class SuperSearchFileViewModel
    {
        public string ID { get; set; }
        public string Filename { get; set; }//without extension, e.g. "HelloWorld"
        public string Extension { get; set; }//with dot, e.g. ".exe"
        public DateTime Time { get; set; }
    }
}