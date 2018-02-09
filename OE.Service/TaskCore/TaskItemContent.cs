using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OE.Service.TaskCore
{

    public class TaskItemContent
    {
        public int TaskID { get; set; }
        public CCF.Task.TaskBase Task { get; set; }
        public AppDomain TaskDomain { get; set; }

        public DateTime? lastRunTime { get; set; }
        public string BaseDir { get; set; }

        public TaskItem TaskConfig { get; set; }
    }
}
