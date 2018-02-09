using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands
{
    public abstract class ICommand
    {
        public Exception Exception { get; protected set; }
        public abstract int Execute(string[] args);

        /// <summary>
        /// 代理方法 内部调用 Execute
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Exec(string[] args)
        {
            try
            {
                return Execute(args);
            }
            catch (Exception ex)
            {
                Exception = ex;
                if (string.IsNullOrWhiteSpace(Msg))
                    Msg = ex.Message;
                return -1000;
            }
        }

        public string Msg { get; protected set; }
    }
}
