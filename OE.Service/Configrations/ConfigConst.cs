using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Configrations
{
    public class ConfigConst
    {
        public const string UnionConfigFileName = "unionConfig.json";
        //public const string PrivalueConfigFileName = "privateConfig.json";
        public const string TaskConfigFileName = "taskConfig.json";
        public const string AppVersionFileName = "appversion.info";
        public const string ClientIdFileName = "clientId.info";
        public const string TaskDllDirName = "tasksDLL";
        public const string ServerUrlKeyName = "ServerUrl";

        public const string ServiceName = "OE_Client_Service";

        public const string RunInServiceMode_Name = "RunInServiceMode";
        public const int PING_TIMESPAN_SECONDS = 10;


        public const string Performance_Run_Name = "Performance_Run";
        public const string Performance_RunCorn_Name = "Performance_RunCorn";
        public const string Performance_FullCountSend_Name = "Performance_FullCountSend";
        public const int Performance_FullCountSend_Default = 4;
        public const int MAX_CACHE_WAITSEND = 500;
        public const string Performance_RunCorn_Default = "0/10 * * * * ?";


        public const string PerformanceUploadType = "watchdata";

        //public const string API_CONFIG_GLOBAL = "/api/config/getglobal";
        //public const string API_CONFIG_PRIVATE = "/api/config/getprivate";
        public const string API_CONFIG_UNION = "/api/config/getconfig";
        public const string API_SYSTEM_PING = "/api/config/ping";
        public const string API_TASK_SUMMARY = "/api/task/summary";
        public const string API_TASK_DETAIL = "/api/task/detail";
        public const string API_TASK_DOWNLOADFILE = "/api/task/downloadfile";

        public const string API_COMMAND_GETNEWS = "/api/command/getnews";
        public const string API_COMMAND_PROCESSNOTIFY = "/api/command/processnotify";
        public const string API_COMMAND_PROCESSRESULT = "/api/command/processresult";
        public const string API_UPLOAD_DATA = "/api/config/uploaddata";
    }
}
