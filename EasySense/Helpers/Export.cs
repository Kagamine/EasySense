using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using EO.Pdf;

namespace EasySense.Helpers
{
    public static class Export
    {
        public static byte[] ToExcel(string Html)
        {
            return Encoding.UTF8.GetBytes(String.Excel(Html));
        }

        public static byte[] ToPDF(string Html)
        {
            var filename = System.Web.HttpContext.Current.Server.MapPath("~/Temp/" + Helpers.Time.ToTimeStamp(DateTime.Now) + ".pdf");
            HtmlToPdf.ConvertHtml(Html, filename);
            var pdfBytes = System.IO.File.ReadAllBytes(filename);
            System.IO.File.Delete(filename);
            return pdfBytes;
        }
    }
}