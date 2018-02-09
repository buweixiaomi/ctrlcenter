using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain
{

    public class ApiLogHelper
    {
        static CCF.WatchLog.DBLoger dbloger = new CCF.WatchLog.DBLoger();
        public static bool CheckSign(string signkey, string sign)
        {
            string secret = CCF.ConfigHelper.GetAppConfig("WatchApiLogSignSecret", "");
            string thissign = CCF.Utils.Security.MakeMD5(signkey.Substring(0, 41) + secret);
            if (sign.ToLower() == thissign.ToLower())
                return true;
            return false;
        }
        public static void BuildSign(string secret, out string toSignKey, out string sign)
        {
            toSignKey = DateTime.Now.Ticks.ToString() + Guid.NewGuid().ToString();
            var realtohash = toSignKey.Substring(41);
            sign = CCF.Utils.Security.MakeMD5(realtohash + secret);
        }

        public static void WriteLog(List<CCF.WatchLog.LogEntity> logs)
        {
            dbloger.WriteLog(logs);
        }
    }
     

    public class ApiLogReqEntity
    {
        public string signKey { get; set; }
        public string sign { get; set; }
        public List<CCF.WatchLog.LogEntity> logs { get; set; }
    } 
}
