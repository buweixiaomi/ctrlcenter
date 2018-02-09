using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Update
{
    public class UpdateConfigCommand : ICommand
    {
        public override int Execute(string[] args)
        {
            CCF.WatchLog.Loger.Log("正在更新配置...", "");
            ApiSdk.ConfigApi configapi = new ApiSdk.ConfigApi();
            var result = configapi.GetUnionConfig();
            if (result.code <= 0)
            {
                Msg = result.msg;
                return -1;
            }
            Configrations.Config.unionConfig = result.data.Item2;
            Configrations.Config.StoreConfig();
            Configrations.Config.LastConfigSign = result.data.Item1;
            CCF.WatchLog.Loger.Log("完成更新配置", "");
            if (TaskCore.TaskContainer.Instance().TaskConfigIsRuning)
            {
                RunConfig();
            }
            return 1;
        }

        private void RunConfig()
        {
            string RunPerformance = Configrations.Config.GetUnionConfig(Configrations.ConfigConst.Performance_Run_Name, "true").ToLower();
            try
            {
                if (RunPerformance == "false")
                {
                    OE.Service.TaskCore.TaskContainer.Instance().Stop(OE.Service.Tasks.PerformanceTask.STATIC_TASK_ID);
                }
                else
                {
                    OE.Service.TaskCore.TaskContainer.Instance().Start(OE.Service.Tasks.PerformanceTask.STATIC_TASK_ID);
                }
            }
            catch (Exception ex)
            {
                CCF.WatchLog.Loger.Error("处理性能任务失败[RunPerformance=" + RunPerformance + "]", ex);
            }
        }
    }
}
