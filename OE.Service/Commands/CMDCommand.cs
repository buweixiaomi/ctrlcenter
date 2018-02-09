using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands
{
    public class CMDCommand : ICommand
    {
        public override int Execute(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Msg = "无效参数";
                return -1;
            }
            string cmdstring = args[0];
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            Process process = Process.Start(psi);
            process.StandardInput.WriteLine(cmdstring);
            process.StandardInput.WriteLine("exit");
            string msg = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.Close();
            Msg = "执行完成！\r\n";
            Msg += "结果：" + msg + "\r\n";
            Msg += "错误：" + error + "\r\n";
            return 1;
        }
    }
}
