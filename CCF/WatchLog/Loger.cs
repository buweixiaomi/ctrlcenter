using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CCF.WatchLog
{
    public class Loger
    {
        //常量
        public const string CONFIG_WatchLog_BatchSize = "watchlog:BatchSize";
        public const string CONFIG_WatchLog_StackSize = "watchlog:WriteNoBlock";
        public const string CONFIG_WatchLog_TimeOutSeconds = "watchlog:TimeOutSeconds";
        public const string CONFIG_WatchLog_LogerType = "watchlog:LogerType";
        public const string CONFIG_WatchLog_ProjectName = "watchlog:ProjectName";
        public const string CONFIG_WatchLog_TimeWatchType = "watchlog:OpenTimeWatch";
        public const string CONFIG_WatchLog_WriteNoBlock = "watchlog:WriteNoBlock";

        //变量
        private static int BatchSize = 1000;
        private static int MaxStackSize = 1000000;
        private static int TimeOutSeconds = 5;//秒 
        private static string LogerType = "fileloger";
        private static bool OpenTimeWatch = true;
        private static AutoResetEvent are = new AutoResetEvent(false);
        private static List<LogEntity> logs = new List<LogEntity>();
        private static object logs_locker = new object();
        private static ILoger innerlog = null;
        private static bool IsAdding = false;
        private static string projectName = "";
        private static bool writeNoBlock = false;
        private static System.Threading.Thread backAddThread = null;
        static Loger()
        {
            try
            {
                #region size
                string size = ConfigHelper.GetAppConfig(CONFIG_WatchLog_BatchSize);
                if (!string.IsNullOrEmpty(size))
                {
                    int tsize = DB.LibConvert.StrToInt(size);
                    if (tsize > 0)
                        BatchSize = tsize;
                }
                #endregion

                #region size
                string stacksize = ConfigHelper.GetAppConfig(CONFIG_WatchLog_StackSize);
                if (!string.IsNullOrEmpty(stacksize))
                {
                    int tstacksize = DB.LibConvert.StrToInt(stacksize);
                    if (tstacksize > 0)
                        MaxStackSize = tstacksize;
                }
                #endregion
                #region timeout
                string timeout = ConfigHelper.GetAppConfig(CONFIG_WatchLog_TimeOutSeconds);
                if (!string.IsNullOrEmpty(timeout))
                {
                    int ttimeout = DB.LibConvert.StrToInt(timeout);
                    if (ttimeout > 0)
                        TimeOutSeconds = ttimeout;
                }
                #endregion

                #region logertype
                LogerType = ConfigHelper.GetAppConfig(CONFIG_WatchLog_LogerType, LogerType);
                switch (LogerType.ToLower())
                {
                    case "fileloger":
                        innerlog = new FileLoger();
                        break;
                    case "dbloger":
                        innerlog = new DBLoger();
                        break;
                    case "consoleloger":
                        innerlog = new ConsoleLoger();
                        break;
                    case "none":
                        break;
                }
                #endregion

                #region projectname
                projectName = ConfigHelper.GetAppConfig(CONFIG_WatchLog_ProjectName, "未知项目名(请配置" + CONFIG_WatchLog_ProjectName + ")");
                #endregion

                #region OpenTimeWatch
                OpenTimeWatch = ConfigHelper.GetAppConfig(CONFIG_WatchLog_TimeWatchType, OpenTimeWatch.ToString()).ToLower() == "true";
                #endregion


                #region writeNoBlock
                writeNoBlock = ConfigHelper.GetAppConfig(CONFIG_WatchLog_WriteNoBlock, writeNoBlock.ToString()).ToLower() == "true";
                #endregion

                if (innerlog != null && writeNoBlock == false)
                {
                    //后台写线程
                    backAddThread = new Thread(ThreadAdd);
                    backAddThread.IsBackground = true;
                    backAddThread.Start();
                }
            }
            catch (Exception ex)
            {
                innerlog = null;
            }
        }

        #region 私有方法
        private static void _AddLog(LogEntity log)
        {
            try
            {
                if (innerlog == null)
                    return;
                log.ProjectName = projectName;
                if (System.Web.HttpContext.Current != null)
                {
                    log.InnerGroupID = System.Web.HttpContext.Current.GetHashCode();
                }
                else
                {
                    log.InnerGroupID = Guid.NewGuid().ToString().GetHashCode();
                }
                if (writeNoBlock)
                {
                    innerlog.WriteLog(new List<LogEntity>() { log });
                    return;
                }
                lock (logs_locker)
                {
                    if (IsAdding && logs.Count >= MaxStackSize)
                    {
                        //如果正在写日志数据，但新的日志又满了，这里新来的日志不添加进去
                        System.Diagnostics.Trace.WriteLine("out stack");
                        return;
                    }
                    logs.Add(log);
                    if (logs.Count >= BatchSize)
                    {
                        are.Set();
                    }
                }
            }
            catch (Exception ex) { }
        }
        private static void ThreadAdd()
        {
            while (true)
            {
                if (logs.Count < BatchSize)
                {
                    //等待
                    try { are.WaitOne(TimeOutSeconds * 1000); }
                    catch (Exception ex) { Thread.Sleep(TimeSpan.FromSeconds(1)); }
                }
                List<LogEntity> waitwrite = null;
                lock (logs_locker)
                {
                    waitwrite = logs.Take(BatchSize).ToList();
                    logs = logs.Skip(BatchSize).ToList();// new List<LogEntity>();
                }
                try
                {
                    IsAdding = true;
                    innerlog.WriteLog(waitwrite);
                }
                catch (Exception ex) { }
                finally { IsAdding = false; }
            }
        }

        #endregion

        #region 公开方法

        public static void Notify()
        {
            are.Set();
        }

        public static int WaitCount()
        {
            return logs.Count;
        }

        public static void Log(string title, string content)
        {
            Log(title, content, "");
        }
        public static void Log(string title, string content, string addition)
        {
            if (innerlog == null)
                return;
            LogEntity log = new LogEntity();
            log.CreateTime = DateTime.Now;
            log.Title = title ?? "";
            log.Content = content ?? "";
            log.Elapsed = 0;
            log.GroupID = log.Title.GetHashCode();
            log.LogType = 0;
            _AddLog(log);
        }

        public static void Error(string title, string msg)
        {
            Error(title, msg, "");
        }
        public static void Error(string title, string msg, string addition)
        {
            if (innerlog == null)
                return;
            LogEntity log = new LogEntity();
            log.CreateTime = DateTime.Now;
            log.Title = title ?? "";
            log.Content = msg ?? "";
            log.Elapsed = 0;
            log.GroupID = log.Title.GetHashCode();
            log.LogType = 2;
            log.Addition = addition;
            _AddLog(log);
        }

        public static void Error(Exception ex)
        {
            if (innerlog == null)
                return;
            Error(ex.Message, ex);
        }


        public static void Error(string title, Exception ex)
        {
            if (innerlog == null)
                return;
            Error(title, string.Format("【信息】{0} \r\n\t【内部异常】{1} \r\n\t【跟踪】{2}", ex.Message, ex.InnerException, ex.StackTrace), "");
        }
        public static void TimeWatch(string title, string content, string addition, double elapsed)
        {
            if (innerlog == null || OpenTimeWatch == false)
            {
                return;
            }
            LogEntity log = new LogEntity();
            log.CreateTime = DateTime.Now;
            log.Title = title ?? "";
            log.Content = content ?? "";
            log.Elapsed = elapsed;
            log.GroupID = log.Title.GetHashCode();
            log.LogType = 1;
            log.Addition = addition;
            _AddLog(log);
        }

        public static void TimeWatch(string title, string content, string addition, Action act)
        {
            if (innerlog == null || OpenTimeWatch == false)
            {
                act();
                return;
            }
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (act != null)
                    act();
                sw.Stop();
                TimeWatch(title, content, addition, sw.Elapsed.TotalSeconds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void TimeWatchSql(string sql, List<DB.ProcedureParameter> paras, Action act)
        {
            TimeWatchSql(sql, paras, null, act);
        }
        public static void TimeWatchSql(string sql, List<DB.ProcedureParameter> paras, DbConnection dbconn, Action act)
        {
            if (innerlog == null)
            {
                act();
                return;
            }
            string addition = dbconn == null ? "" : dbconn.Database;
            string title = "sql耗时【" + sql.GetHashCode() + "】";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("【sql】:{0};", sql);
            if (paras != null && paras.Count > 0)
            {
                sb.AppendFormat("\r\n\t【参数】:{0};", string.Join(",", paras.Select(x => "@" + x.Name + ":" + DB.LibConvert.NullToStr(x.Value) + "  ")));
            }
            else
            {
                sb.AppendFormat("\r\n\t【参数】:{0};", "[无]");
            }
            try
            {
                TimeWatch(title, sb.ToString(), addition, act);
            }
            catch (Exception ex)
            {
                title = "sql错误[" + sql.GetHashCode() + "]";
                sb.Insert(0, "【错误说明】" + ex.Message + ";\r\n\t");
                Error(title, sb.ToString(), addition);
                throw ex;
            }
        }
        #endregion
    }
}
