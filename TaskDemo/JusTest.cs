using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDemo
{
    public class JusTest : CCF.Task.TaskBase
    {
        public override void Run()
        {
            Log("运行了一次！");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}
