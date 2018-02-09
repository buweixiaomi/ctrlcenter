using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.File
{
    public class DeleteFileCommand : ICommand
    {
        /// <summary>
        /// d:\\Data\\meinv.png
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override int Execute(string[] args)
        {
            int count = 0;
            foreach (var a in args)
            {
                if (System.IO.File.Exists(a))
                {
                    System.IO.File.Delete(a);
                    count++;
                }
                if (System.IO.Directory.Exists(a))
                {
                    System.IO.Directory.Delete(a, true);
                    count++;
                }
            }
            return count;
        }
    }
}
