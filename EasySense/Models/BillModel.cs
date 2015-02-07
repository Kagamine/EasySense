using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySense.Models
{
    [Table("Bills")]
    public class BillModel
    {
        public Guid ID { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public virtual ProjectModel Project { get; set; }

        public decimal Plan { get; set; }

        public decimal Actual { get; set; }

        public DateTime Time { get; set; }

        public string Hint { get; set; }

        public int Type { get; set; }

        public static string[] BillTypes =
        {
            "平面印刷费",
            "数码印刷费",
            "光盘印刷费",
            "展览喷绘费",
            "快递费",
            "差旅费",
            "招待费",
            "咨询费",
            "速记费",
            "配音费",
            "场地费",
            "摄影费",
            "摄像费",
            "设计费",
            "技术费",
            "材料费",
            "代垫费",
            "常规",
            "外贸",
            "其他"
        };
    }
}