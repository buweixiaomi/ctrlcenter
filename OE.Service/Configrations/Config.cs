using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Configrations
{
    public class Config
    {
        public static List<ConfigKeyValue> unionConfig = new List<ConfigKeyValue>();

        public static string ClientID = "";
        public static object _unionConfigLocker = new object();

        public static string LastConfigSign = "";
        public static string GetSystemConfig(string key, string defaultv)
        {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                return defaultv;
            return value;
        }

        public static void StoreConfig()
        {
            lock (_unionConfigLocker)
            {
                string json = Utils.Utils.SerializeObject(unionConfig);
                string filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigConst.UnionConfigFileName);
                System.IO.File.WriteAllText(filepath, json, Encoding.UTF8);
            }
        }

        public static void ResumeConfig()
        {
            lock (_unionConfigLocker)
            {
                string filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigConst.UnionConfigFileName);
                if (System.IO.File.Exists(filepath))
                {
                    try
                    {
                        unionConfig = Utils.Utils.DeserializeObject<List<ConfigKeyValue>>(System.IO.File.ReadAllText(filepath, Encoding.UTF8));
                    }
                    catch { }
                    if (unionConfig == null)
                        unionConfig = new List<ConfigKeyValue>();
                }
            }
        }

        public static string GetUnionConfig(string key, string defaultv)
        {
            var item = unionConfig.FirstOrDefault(x => x.Key.ToLower() == key.ToLower());
            if (item == null || string.IsNullOrEmpty(item.Value))
                return defaultv;
            return item.Value;
        }


        public static string GetResultConfig(string orvalue)
        {
            foreach (var a in unionConfig)
            {
                orvalue = orvalue.Replace("{@" + a.Key + "}", a.Value);
            }
            return orvalue;
        }

        public static string GetAppDir(string appname, string defaultv)
        {
            return GetUnionConfig(appname + "_DIR", defaultv);
        }

        public static string GetAppBackup(string appname, string defaultv)
        {
            return GetUnionConfig(appname + "_BACKUP", defaultv);
        }

        public static string GetAppDefaultBackupDir(string appname)
        {
            return "D:\\" + appname + "_BACKUP";
        }

        public static string GetTempFileName()
        {
            string tempdir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\temp";
            if (!System.IO.Directory.Exists(tempdir))
                System.IO.Directory.CreateDirectory(tempdir);
            string fillename = tempdir + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".tmp";
            return fillename;
        }

    }
}
