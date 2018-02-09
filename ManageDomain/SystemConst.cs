using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain
{
    public class SystemConst
    {
        public const string MYSQL_CONFIG_NAME = "MySqlConn";
        public const string MYSQL_WATCH_CONFIG_NAME = "PerformanceDBConn";
        public const string MySQL_WATCHLOG_CONFIG_NAME = "WatchLogDBConn";
        public const int Max_Heart_SpanSeconds = 120;//s
        public const int Max_Server_Days = 15;//days
        public const string Project_File_Config_Name = "ProjectFileDir";
        public const string Task_File_Config_Name = "TaskFileDir";//添加任务DLL路径

    }

    public enum SystemKeys
    {
        Permission_Key_Customer_Delete,
        Permission_Key_Customer_Add
    }
}
