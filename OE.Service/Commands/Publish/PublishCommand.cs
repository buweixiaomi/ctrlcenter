using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Publish
{
    public class PublishCommand : ICommand
    {
        /// <summary>
        /// app url version
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override int Execute(string[] args)
        {
            if (args == null || args.Count() != 3)
            {
                Msg = "参数为三个！";
                return -1;
            }

           
        }
    }
}
