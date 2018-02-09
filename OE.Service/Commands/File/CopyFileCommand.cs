using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.File
{
    public class CopyFileCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override int Execute(string[] args)
        {
            int count = 0;
            for (int i = 0; i < args.Length; i = i + 2)
            {
                if (System.IO.File.Exists(args[i]))
                {
                    Utils.Utils.CopyFile(args[i], args[i + 1]);
                    count++;
                }
                if (System.IO.Directory.Exists(args[i]))
                {
                    Utils.Utils.CopyDir(args[i], args[i + 1]);
                    count++;
                }
            }
            Msg = "复制文件" + count + "个";
            return 1;
        }
    }
}
