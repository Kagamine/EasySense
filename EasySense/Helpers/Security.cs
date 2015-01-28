using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Security.Cryptography;

namespace EasySense.Helpers
{
    public static class Security
    {
        /// <summary> 
        /// SHA256加密字符串 
        /// </summary> 
        /// <param name="source">源字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public static string SHA256(string source)
        {
            SHA256 sha256 = new System.Security.Cryptography.SHA256Managed();
            byte[] sha256Bytes = System.Text.Encoding.Default.GetBytes(source);
            byte[] cryString = sha256.ComputeHash(sha256Bytes);
            string sha256Str = string.Empty;
            for (int i = 0; i < cryString.Length; i++)
            {
                sha256Str += cryString[i].ToString("X");
            }
            return sha256Str;
        }

        /// <summary> 
        /// SHA1加密字符串 
        /// </summary> 
        /// <param name="source">源字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public static string SHA1(string source)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "SHA1");
        }
        /// <summary> 
        /// MD5加密字符串 
        /// </summary> 
        /// <param name="source">源字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public static string MD5(string source)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5"); ;
        } 
    }
}
