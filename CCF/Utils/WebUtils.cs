using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CCF.Utils
{
    public class WebUtils
    {

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(result) || !IsIP(result))
                return "127.0.0.1";

            return result;
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }


        /// <summary>
        /// 是否为有效域
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static bool IsValidDomain(string host)
        {
            if (host.IndexOf(".") == -1)
                return false;

            return new Regex(@"^\d+$").IsMatch(host.Replace(".", string.Empty)) ? false : true;
        }


        public static void WriteCookie(string cookieName, string strName, string strValue, DateTime? expries = null, string cookieDomain = "")
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
            }
            cookie.Values[strName] = HttpUtility.UrlEncode(strValue);
            if (expries != null)
                cookie.Expires = expries.Value;
            cookie.HttpOnly = true;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 获得cookie值
        /// </summary>
        /// <param name="strName">项</param>
        /// <returns>值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[strName].ToString());
            return "";
        }
        public static string BuildValiCode(int count = 4)
        {
            string chkCode = string.Empty;
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X' };
            Random rnd = new Random(DateTime.Now.Millisecond);
            //生成验证码字符串 
            for (int i = 0; i < count; i++)
            {
                chkCode += character[rnd.Next(0, character.Length - 1)];
            }
            //写入Cookie 
            return chkCode;
        }
        //public class MemoryFileContent
        //{
        //    public string FileName { get; set; }
        //    public byte[] Content { get; set; }
        //}
        //public HttpResponseMessage HttpPost(string api, Dictionary<string, object> parameters)
        //{
        //    if (parameters == null)
        //    {
        //        parameters = new Dictionary<string, object>();
        //    }
        //    var dict = new Dictionary<string, object>(parameters.ToDictionary(k => k.Key, v => v.Value));
        //    HttpContent httpContent = null;
        //    HttpClient http = new HttpClient();
        //    if (dict.Count(p => p.Value is MemoryFileContent) > 0)
        //    {
        //        var content = new MultipartFormDataContent();
        //        foreach (var param in dict)
        //        {
        //            if (param.Value is MemoryFileContent)	//内存文件
        //            {
        //                var mcontent = (MemoryFileContent)param.Value;
        //                content.Add(new ByteArrayContent(mcontent.Content), param.Key, mcontent.FileName);
        //            }
        //            else /*if (dataType.IsValueType || dataType == typeof(string))*/	//其他类型
        //            {
        //                content.Add(new StringContent(string.Format("{0}", param.Value)), param.Key);
        //            }
        //        }
        //        httpContent = content;
        //    }
        //    else
        //    {
        //        var content = new FormUrlEncodedContent(dict.ToDictionary(k => k.Key, v => string.Format("{0}", v.Value)));
        //        httpContent = content;
        //    }
        //    return http.PostAsync(api, httpContent).Result;
        //}

        //public HttpResponseMessage HttpPost(string api, string text,Encoding end,string contentType)
        //{
        //    StringContent httpContent = new StringContent(text, end, contentType);
        //    HttpClient http = new HttpClient();
        //    return http.PostAsync(api, httpContent).Result;
        //}
    }
}
