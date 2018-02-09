using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace OE.Service.TaskCore
{
    public class TaskJob : Quartz.IJob
    {
        private CCF.Task.TaskBase thisTask;
        public void Execute(Quartz.IJobExecutionContext context)
        {
            int id = Convert.ToInt32(context.JobDetail.Description);
            var t = TaskContainer.Instance().Tasks.FirstOrDefault(x => x.TaskID == id);
            if (t == null)
                return;
            if (t.Task.IsRunning)
                return;
            try
            {
                t.lastRunTime = DateTime.Now;
                t.Task.Run();
            }
            catch (Exception ex)
            {
                t.Task.IsRunning = false;
                try { TaskContainer.Instance().Stop(t.TaskID); }
                catch { }
                CCF.WatchLog.Loger.Error("运行任务出错", "任务ID【" + t.TaskID + "】:" + ex.Message);
            }
        }
    }
}
