using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace CCF.Task
{
    public class Log
    {
        private static string filepath = null;
        static Log()
        {
            filepath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\Log";
        }
        public static void WriteLog(string msg)
        {
            if (!System.IO.Directory.Exists(filepath))
                System.IO.Directory.CreateDirectory(filepath);
            string filename = filepath.TrimEnd('\\') + "\\task" + DateTime.Now.ToString("yyyyMMdd") + ".log";

            string content = string.Format("【时间】{0}\r\n【内容】{1}\r\n\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg ?? "");
 
             System.IO.File.AppendAllText(filename, content, System.Text.Encoding.UTF8);

        }
    }
}
