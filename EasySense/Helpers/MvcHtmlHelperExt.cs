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
        public static MvcHtmlString LinkWithSID(this System.Web.Mvc.UrlHelper Url, string Title, string ActionName, string ControllerName, object RouteValues, object HtmlAttributes)
        {
            string url = Url.Action(ActionName, ControllerName) + "?";
            if (RouteValues != null)
            {
                var RouteValuesProperties = RouteValues.GetType().GetProperties();
                foreach (var p in RouteValuesProperties)
                    url += p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(RouteValues).ToString()) + "&";
            }
            url += "sid=" + Url.RequestContext.HttpContext.Session["sid"];
            var attributes = " ";
            if (HtmlAttributes != null)
            {
                var HtmlAttributesProperties = HtmlAttributes.GetType().GetProperties();
                foreach (var p in HtmlAttributesProperties)
                    attributes += p.Name + "=\"" + p.GetValue(HtmlAttributes) + "\" ";
            }
            return new MvcHtmlString("<a href=\"javascript:window.location='" + url + "'\"" + attributes + "/>" + HttpUtility.HtmlEncode(Title) + "</a>");
        }
        public static MvcHtmlString LinkWithSID(this System.Web.Mvc.UrlHelper Url, string Title, string ActionName, string ControllerName, object RouteValues)
        {
            return LinkWithSID(Url, Title, ActionName, ControllerName, RouteValues, null);
        }
        public static MvcHtmlString LinkWithSID(this System.Web.Mvc.UrlHelper Url, string Title, string ActionName, string ControllerName)
        {
            return LinkWithSID(Url, Title, ActionName, ControllerName, null, null);
        }
    }
}