using OE.Service.TaskCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OE.Service.TaskCore
{
    public class PingStateTask
    {
        private Thread t = null;
        public Action OnCommand;
        public void Run()
        {
            if (t != null)
                Stop();
            t = new Thread(ThreadRun);
            t.IsBackground = true;
            t.Start();
        }

        public void Stop()
        {
            try
            {
                t.Abort();
            }
            finally { }
        }

        private void ThreadRun()
        {
            while (true)
            {
                try
                {
                    var api = new ApiSdk.SystemApi().Ping();
                    if (api.code > 0)
                    {
                        CCF.WatchLog.Loger.Log("完成一次心跳.", "");
                        MaintanceCommand(api.data.MaxCmdID);
                        MaintanceConfig(api.data.ConfigSign);
                    }
                    else
                    {

                        CCF.WatchLog.Loger.Error("心跳失败.", api.msg);
                    }
                }
                catch (Exception ex)
                {
                    CCF.WatchLog.Loger.Error("心跳失败.", ex);
                    //log
                }
                Thread.Sleep(1000 * Configrations.ConfigConst.PING_TIMESPAN_SECONDS);
            }
        }

        private void MaintanceCommand(int serverMaxCmdID)
        {
            if (serverMaxCmdID <= CommandHelper.CurrMaxCmdId)
            {
                return;
            }
            if (OnCommand != null)
                OnCommand();
        }

        private void MaintanceConfig(string configsign)
        {
            if (string.IsNullOrEmpty(configsign))
                return;
            if (Configrations.Config.LastConfigSign != configsign)
            {
                Task.Factory.StartNew(() =>
                {
                    new Commands.Update.UpdateConfigCommand().Exec(null);
                });
            }
        }



    }
}
