using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySense.Helpers
{
    public static class MvcHtmlHelperExt
    {

        public static MvcHtmlString ToTimeTip<TModel>(this HtmlHelper<TModel> self, DateTime time)
        {
            return new MvcHtmlString(Time.ToTimeTip(time));
        }

        public static MvcHtmlString ToTimeLength<TModel>(this HtmlHelper<TModel> self, DateTime time1, DateTime time2)
        {
            return new MvcHtmlString(Time.ToTimeLength(time1,time2));
        }

        public static MvcHtmlString ToVagueTimeLength<TModel>(this HtmlHelper<TModel> self, DateTime time1, DateTime time2)
        {
            return new MvcHtmlString(Time.ToVagueTimeLength(time1, time2));
        }

        public static MvcHtmlString Sanitized<TModel>(this HtmlHelper<TModel> self, string html)
        {
            if (html == null) return new MvcHtmlString("");
            return new MvcHtmlString(HtmlFilter.Instance.SanitizeHtml(html));
        }

        public static MvcHtmlString ToTimeStamp<TModel>(this HtmlHelper<TModel> self, DateTime time)
        {
            return new MvcHtmlString(Helpers.Time.ToTimeStamp(time).ToString());
        }

        public static MvcHtmlString AntiForgerySID<TModel>(this HtmlHelper<TModel> self)
        {
            return new MvcHtmlString("<input type=\"hidden\" name=\"sid\" value=\"" + self.ViewBag.SID + "\" />");
        }
        public static MvcHtmlString SID<TModel>(this HtmlHelper<TModel> self)
        {
            return new MvcHtmlString("sid=" + self.ViewBag.SID);
        }
    }
}