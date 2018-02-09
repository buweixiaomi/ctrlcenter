using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Web.Mvc.Html;

namespace ManageWeb
{
    public static class Extentions
    {
        public static HtmlString ToBootPager<T>(this ManageDomain.Models.PageModel<T> model)
        {
            string s = "  <script type=\"text/javascript\">         " +
           " var options = {                                                     " +
          "  bootstrapMajorVersion: 2, //版本                         " +
          "  currentPage: @currentPage., //当前页数                 " +
           " totalPages: @pageCount, //总页数                        " +
           " itemTexts: function (type, page, current) {            " +
           "     switch (type) {                                               " +
           "         case 'first':                                                  " +
           "             return '首页';                                        " +
           "         case 'prev':                                            " +
           "             return '上一页';                                      " +
           "         case 'next':                                        " +
           "             return '下一页';                                 " +
           "         case 'last':                                           " +
           "             return '末页';                                   " +
           "         case 'page':                                           " +
           "             return page;                                    " +
           "     }                                                              " +
           " },//点击事件，用于通过Ajax来刷新整个list列表 " +
         " $('#example').bootstrapPaginator(options);     " +
            "</script>";
            int pagecount = 0;
            if (model.PageSize == 0)
            {
                if (model.list.Count > 0)
                {
                    pagecount = 1;
                }
            }
            else
            {
                pagecount = model.TotalCount / model.PageSize + (model.TotalCount % model.TotalCount == 0 ? 0 : 1);
            }
            s.Replace("@currentPage", model.PageNo.ToString())
                .Replace("@pageCount", pagecount.ToString());
            return new HtmlString(s);
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="wvp"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static HtmlString ToDateString(this HtmlHelper wvp, DateTime? dt)
        {
            return ToDateString(wvp, dt, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        ///  format:  
        ///  date: yyyy-MM-dd
        ///  datetime: yyyy-MM-dd HH:mm:ss
        ///  time: HH:mm:ss
        ///  "":  yyyy-MM-dd
        /// </summary>
        /// <param name="wvp"></param>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static HtmlString ToDateString(this HtmlHelper wvp, DateTime? dt, string format)
        {
            if (dt == null)
                return new HtmlString("");
            if (string.IsNullOrEmpty(format))
                format = "yyyy-MM-dd";
            switch (format.ToLower())
            {
                case "date":
                    format = "yyyy-MM-dd";
                    break;
                case "datetime":
                    format = "yyyy-MM-dd HH:mm:ss";
                    break;
                case "time":
                    format = "HH:mm:ss";
                    break;
                default:
                    break;
            }
            return new HtmlString(dt.Value.ToString(format));
        }

        public static HtmlString ZeroToEmpty(this HtmlHelper hh, double? v, string format = "")
        {
            string ts = "";
            if (v == null || v.Value == 0d)
                ts = "";
            else
                ts = v.Value.ToString(format);
            return new HtmlString(ts);
        }

        public static HtmlString ZeroToEmpty(this HtmlHelper hh, decimal? v, string format = "")
        {
            string ts = "";
            if (v == null || v.Value == 0m)
                ts = "";
            else
                ts = v.Value.ToString(format);
            return new HtmlString(ts);
        }

        public static HtmlString ZeroToEmpty(this HtmlHelper hh, int? v)
        {
            string ts = "";
            if (v == null || v.Value == 0)
                ts = "";
            else
                ts = v.Value.ToString();
            return new HtmlString(ts);
        }

        public static HtmlString RouteActionLink(this HtmlHelper hh, string text, string url, string @class, string class2, string key, string currkey)
        {
            var currkeys = currkey.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string r = string.Format("<a class=\"{2} {3}\" href=\"{1}\">{0}</a>", text, url, @class, currkeys.Contains(key) ? class2 : "");
            return new HtmlString(r);
        }

        public static string UrlKey(this WebViewPage wvp)
        {
            var rd = wvp.ViewContext.RouteData;
            List<string> urlkeys = new List<string>();
            if (rd.Values["area"] != null)
            {
                urlkeys.Add(rd.Values["area"].ToString());
            }

            if (rd.Values["controller"] != null)
            {
                urlkeys.Add(rd.Values["controller"].ToString());
            }
            else
            {
                urlkeys.Add("home");
            }

            if (rd.Values["action"] != null)
            {
                urlkeys.Add(rd.Values["action"].ToString());
            }
            else
            {
                urlkeys.Add("index");
            }
            return string.Join(".", urlkeys).ToLower();
        }


        public static int CurrUserId(this System.Security.Principal.IPrincipal t)
        {
            if (System.Web.HttpContext.Current == null)
                return 0;
            return CCF.DB.LibConvert.ObjToInt(System.Web.HttpContext.Current.Session["CurrUserId"]);
        }

    }
}