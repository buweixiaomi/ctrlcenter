using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Task
{
    public class StopTaskCommand : ICommand
    {
        public override int Execute(string[] args)
        {
            if (args == null || args.Count() == 0)
            {
                this.Msg = "参数不正确";
                return -1;
            }
            bool isok = TaskCore.TaskContainer.Instance().Stop(Convert.ToInt32(args[0]));
            if (!isok)
            {
                this.Msg = "停止失败！";
                return -1;
            }
            return 1;
        }

    }
}
