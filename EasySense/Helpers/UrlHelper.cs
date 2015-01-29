using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace EasySense.Helpers
{
    public static class UrlHelper
    {
        public static string MakeGetURL(object tmp)
        {
            var ret = "?";
            var t = tmp.GetType();
            var properties = t.GetProperties();
            foreach (var key in properties)
                ret += key.Name + "=" + key.GetValue(tmp) + "&";
            return ret.TrimEnd('&');
        }
    }
}