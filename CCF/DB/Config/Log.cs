using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.Config
{
    public class Log
    {
        public static Action<string> loghandler;

        public static void Write(string msg)
        {
            InnertWrite("[Log]" + msg);
        }

        private static void InnertWrite(string msg)
        {
            System.Diagnostics.Trace.WriteLine(msg);
            if (loghandler != null)
            {
                loghandler(msg);
            }
        }

        public static void Write(string temp, params string[] paras)
        {
            Write(string.Format(temp, paras));
        }

        public static void Alert(string msg)
        {
            InnertWrite("[Alert]" + msg);
        }
        public static void Alert(string temp, params string[] paras)
        {
            Alert(string.Format(temp, paras));
        }

        public static void Error(string msg)
        {
            InnertWrite("[Error]" + msg);
        }
        public static void Error(string temp, params string[] paras)
        {
            Error(string.Format(temp, paras));
        }
    }
}
