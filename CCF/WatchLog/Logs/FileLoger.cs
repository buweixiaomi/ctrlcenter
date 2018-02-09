using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CCF.WatchLog
{
    public class FileLoger : ILoger
    {
        public const string CONFIG_WatchLog_FilePath = "watchlog:FileLogPath";
        public const string CONFIG_WatchLog_FileUnion = "watchlog:FileUnion";
        public const string CONFIG_WatchLog_DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        private object obj = new object();
        public void WriteLog(List<LogEntity> logs)
        {
            lock (obj)
            {
                try
                {
                    string filepath = ConfigHelper.GetAppConfig(CONFIG_WatchLog_FilePath, "~/WatchLog");
                    bool unionfile = ConfigHelper.GetAppConfig(CONFIG_WatchLog_FileUnion, "false") == "true";

                    filepath = CCF.ConfigHelper.MapPath(filepath);
                    filepath.TrimEnd('\\');
                    if (!System.IO.Directory.Exists(filepath))
                        System.IO.Directory.CreateDirectory(filepath);
                    string commnfilename = "commnwatch" + DateTime.Now.ToString("yyyyMMdd") + ".log";//普通日志
                    string timefilename = "timewatch" + DateTime.Now.ToString("yyyyMMdd") + ".log";//耗时日志
                    string errofilename = "errorwatch" + DateTime.Now.ToString("yyyyMMdd") + ".log";//错误日志
                    StringBuilder commncontents = null; //普通日志
                    StringBuilder timecontents = null;//耗时日志
                    StringBuilder errorcontents = null;//错误日志
                    if (unionfile)
                    {
                        commncontents = new StringBuilder();
                        errorcontents = timecontents = commncontents;
                        commnfilename = "watchlog" + DateTime.Now.ToString("yyyyMMdd") + ".log";//普通日志
                    }
                    else
                    {
                        commncontents = new StringBuilder();
                        errorcontents = new StringBuilder();
                        timecontents = new StringBuilder();
                    }
                    foreach (var a in logs)
                    {
                        if (a.LogType == 0)
                        {
                            commncontents.AppendFormat("【项目】:{0};\r\n【大分组】:{1};\t【小分组】:{2};\r\n【日志类型】:{3};\r\n【标题】:{4};\r\n【内容】:{5};\r\n【createtime】:{6};\r\n\r\n",
                               a.ProjectName ?? "", a.GroupID, a.InnerGroupID, "普通日志", a.Title, a.Content ?? "", a.CreateTime.ToString(CONFIG_WatchLog_DateTimeFormat));
                        }
                        if (a.LogType == 1)
                        {
                            timecontents.AppendFormat("【项目】:{0};\r\n【大分组】:{1};\t【小分组】:{2};\r\n【日志类型】:{3};\r\n【标题】:{4};\r\n【内容】:{5};\r\n【createtime】:{6};\r\n【耗时】:{7}s\r\n\r\n",
                                  a.ProjectName ?? "", a.GroupID, a.InnerGroupID, "耗时日志", a.Title, a.Content ?? "", a.CreateTime.ToString(CONFIG_WatchLog_DateTimeFormat), a.Elapsed);
                        }
                        if (a.LogType == 2)
                        {
                            errorcontents.AppendFormat("【项目】:{0};\r\n【大分组】:{1};\t【小分组】:{2};\r\n【日志类型】:{3};\r\n【标题】:{4};\r\n【内容】:{5};\r\n【createtime】:{6};\r\n\r\n",
                                  a.ProjectName ?? "", a.GroupID, a.InnerGroupID, "错误日志", a.Title, a.Content ?? "", a.CreateTime.ToString(CONFIG_WatchLog_DateTimeFormat));
                        }
                    }
                    if (unionfile)
                    {
                        if (commncontents.Length > 0)
                            System.IO.File.AppendAllText(filepath + "\\" + commnfilename, commncontents.ToString(), System.Text.Encoding.UTF8);
                    }
                    else
                    {
                        if (commncontents.Length > 0)
                            System.IO.File.AppendAllText(filepath + "\\" + commnfilename, commncontents.ToString(), System.Text.Encoding.UTF8);
                        if (timecontents.Length > 0)
                            System.IO.File.AppendAllText(filepath + "\\" + timefilename, timecontents.ToString(), System.Text.Encoding.UTF8);
                        if (errorcontents.Length > 0)
                            System.IO.File.AppendAllText(filepath + "\\" + errofilename, errorcontents.ToString(), System.Text.Encoding.UTF8);
                    }
                }
                catch (Exception)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
