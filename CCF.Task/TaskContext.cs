using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCF.Task
{
    public abstract class TaskContext 
    {
        public abstract string GetSystemConfig(string key);
    }
}
