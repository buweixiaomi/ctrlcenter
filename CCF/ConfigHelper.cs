using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF
{
    public class ConfigHelper
    {
        public static string GetAppConfig(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[name];
        }


        public static string GetAppConfig(string name, string defaultval)
        {
            string v = GetAppConfig(name);
            if (string.IsNullOrWhiteSpace(v))
                v = defaultval;
            return v;
        }

        public static string MapPath(string oripath)
        {
            string v = oripath;
            try
            {
                if (oripath.StartsWith("~"))
                {
                    if (System.Web.HttpContext.Current != null)
                    {
                        v = System.Web.HttpContext.Current.Server.MapPath(oripath);
                    }
                    else
                    {
                        v = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, v.Substring(1).Replace("/", "\\").TrimStart('\\'));
                    }
                }
            }
            catch { }
            return v;
        }

    }
}
