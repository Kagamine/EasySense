using System;

namespace EasySense.Utils
{
    public class StringUtils
    {
        public static string Trim(string str)
        {
            if (str != null)
            {
                return str.Trim();
            }
            return str;
        }

        public static bool IsNotBlank(string str)
        {
            str = Trim(str);
            if (str != null && !str.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}