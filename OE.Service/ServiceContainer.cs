using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OE.Service
{
    public class ServiceContainer
    {
        object processlocker = new object();
        bool isprocess = false;
        CommandHelper cmdhelper = new CommandHelper();
        private TaskCore.PingStateTask pingtask;
        public void Start()
        {
            CCF.WatchLog.Loger.Log("正在配置ClientId...", "");
            SetClientId();
            CCF.WatchLog.Loger.Log("ClientId:" + Configrations.Config.ClientID, "");

            CCF.WatchLog.Loger.Log("任务线程启动中...", "");

            Configrations.Config.ResumeConfig();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    CCF.WatchLog.Loger.Log("更新配置...", "");
                    Commands.Update.UpdateConfigCommand u1 = new Commands.Update.UpdateConfigCommand();
                    int updateconfig = u1.Exec(null);
                    CCF.WatchLog.Loger.Log("更新配置完成.", "");
                    if (updateconfig <= 0)
                    {
                        CCF.WatchLog.Loger.Error("启动时更新配置失败," + u1.Msg, "");
                        Configrations.Config.ResumeConfig();
                        CCF.WatchLog.Loger.Log("使用原来配置启动", "");
                    }
                    CCF.WatchLog.Loger.Log("初始化任务容器...", "");
                    TaskCore.TaskContainer.Instance().Init();
                }
                catch (Exception ex)
                {
                    CCF.WatchLog.Loger.Error("初始化任务容器失败", ex);
                }
            });
            CCF.WatchLog.Loger.Log("任务线程启动中完成。", "");


            CCF.WatchLog.Loger.Log("启动心跳...", "");
            if (pingtask != null)
                pingtask.Stop();
            pingtask = new TaskCore.PingStateTask();
            pingtask.OnCommand = OnCommand;
            pingtask.Run();
            CCF.WatchLog.Loger.Log("启动心跳完成.", "");

            CCF.WatchLog.Loger.Log("服务线程启动完成！", "");
        }


        /// <summary>
        /// 创建唯一ID
        /// </summary>
        private void SetClientId()
        {
            string[] macAddress = null;
            string[] ips = null;
            Utils.Utils.GetIpsAndMacs(out ips, out macAddress);
            string s = string.Join(",", macAddress) + System.Environment.MachineName + AppDomain.CurrentDomain.BaseDirectory.ToLower();
            string clientid = CCF.DB.Utility.MakeMD5(s);
            string filefullname = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Configrations.ConfigConst.ClientIdFileName);

            System.IO.File.WriteAllText(filefullname, clientid);
            Configrations.Config.ClientID = clientid;
        }
        private void OnCommand()
        {
            if (isprocess)
                return;
            lock (processlocker)
            {
                if (isprocess)
                    return;
                isprocess = true;
            }
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    cmdhelper.ProcessCommand();
                }
                finally
                {
                    isprocess = false;
                }
            });

        }


        public void Stop()
        {
            if (pingtask != null)
                pingtask.Stop();
            TaskCore.TaskContainer.Instance().ExistService();
        }
    }
}
