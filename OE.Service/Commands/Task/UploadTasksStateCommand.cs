using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OE.Service.Commands.Task
{
    public class UploadTasksStateCommand : ICommand
    {
        public override int Execute(string[] args)
        {
            List<object> rdata = new List<object>();
            foreach (var a in OE.Service.TaskCore.TaskContainer.Instance().Tasks)
            {
                double memory = (a.TaskDomain.MonitoringSurvivedMemorySize / 1014 / 1024d);
                rdata.Add(new
                {
                    taskid = a.TaskID,
                    lastruntime = a.lastRunTime == null ? "" : a.lastRunTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    memory = memory
                });
            }
            new ApiSdk.CommApi().UploadData("tasksummary", Utils.Utils.SerializeObject(rdata));
            return 1;
        }
    }
}
