using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.TaskCore
{
    public class ServiceTaskContext : CCF.Task.TaskContext
    {
        public override string GetSystemConfig(string key)
        {
            return Configrations.Config.GetSystemConfig(key, null);
        }
    }
}
