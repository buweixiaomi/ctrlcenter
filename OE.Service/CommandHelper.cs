using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service
{
    public class CommandHelper
    {
        public static int CurrMaxCmdId = 0;

        Dictionary<string, Type> cmds = new Dictionary<string, Type>();

        private List<CommandInfo> WaitingCmds = new List<CommandInfo>();
        private object comdlock = new object();

        public CommandHelper()
        {
            foreach (var t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.BaseType.Equals(typeof(OE.Service.Commands.ICommand)))
                {
                    string name = t.Name.ToLower();
                    if (name.EndsWith("command"))
                        name = name.Substring(0, name.IndexOf("command"));
                    cmds.Add(name, t);
                }
            }
        }
        public void ProcessCommand()
        {
            lock (comdlock)
            {
                ApiSdk.CommandApi cmdapi = new ApiSdk.CommandApi();
                var result = cmdapi.GetCommands(CurrMaxCmdId);
                if (result.code <= 0)
                    return;

                WaitingCmds.AddRange(result.data);
                foreach (var a in WaitingCmds)
                {
                    ProcessCMD(a);
                }
                WaitingCmds.Clear();
            }
        }

        private void ProcessCMD(CommandInfo cmdinfo)
        {
            if (!Notify(cmdinfo.ID))
                return;
            if (!cmds.ContainsKey(cmdinfo.Name.ToLower()))
            {
                SetResult(cmdinfo.ID, -1, "命令不存在！", null);
                return;
            }
            try
            {
                CCF.WatchLog.Loger.Log("开始执行命令" + cmdinfo.Name, cmdinfo.Name);
                List<string> args = new List<string>();
                foreach (var a in cmdinfo.Args)
                {
                    if (a.ContainConfig)
                    {
                        args.Add(Configrations.Config.GetResultConfig(a.OriValue));
                    }
                    else
                    {
                        args.Add(a.OriValue);
                    }
                }
                Commands.ICommand cmd = Activator.CreateInstance(cmds[cmdinfo.Name.ToLower()]) as Commands.ICommand;
                int code = cmd.Exec(args.ToArray());
                CCF.WatchLog.Loger.Log("结束执行命令" + cmdinfo.Name, cmdinfo.Name);
                SetResult(cmdinfo.ID, code, cmd.Msg, cmd.Exception);
            }
            catch (Exception ex)
            {
                CCF.WatchLog.Loger.Error("执行命令出错" + cmdinfo.Name, ex);
                SetResult(cmdinfo.ID, -1000, "", ex);
            }
        }

        private bool Notify(int id)
        {
            try
            {
                ApiSdk.CommandApi cmdapi = new ApiSdk.CommandApi();
                var result = cmdapi.CommandNotify(id);
                if (result.code <= 0)
                    return false;
                return true;
            }
            catch { return false; }
        }

        private void SetResult(int id, int code, string msg, Exception exception)
        {
            try
            {
                ApiSdk.CommandApi cmdapi = new ApiSdk.CommandApi();
                cmdapi.CommandResult(id, code, msg ?? "", exception == null ? "" : exception.Message);
            }
            catch { }
        }
    }
}
