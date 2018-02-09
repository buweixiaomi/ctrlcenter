using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Configrations.Config.GetSystemConfig(Configrations.ConfigConst.RunInServiceMode_Name, "false").ToLower() == "true")
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new OEWINService() };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                Console.Write("服务管理 1:安装， 2:卸载  ，其它进入调试模式：");
                string s = Console.ReadLine();
                switch (s)
                {
                    case "1":
                        Utils.ServiceHelper.Install(Configrations.ConfigConst.ServiceName, Assembly.GetExecutingAssembly().Location);
                        Console.ReadKey();
                        break;
                    case "2":
                        if (Utils.ServiceHelper.ServiceIsExisted(Configrations.ConfigConst.ServiceName))
                            Utils.ServiceHelper.Uninstall(Configrations.ConfigConst.ServiceName, Assembly.GetExecutingAssembly().Location);
                        Console.ReadKey();
                        break;
                    default:
                        CCF.WatchLog.Loger.Log("正在启动...", "");
                        ServiceContainer sc = new ServiceContainer();
                        sc.Start();
                        CCF.WatchLog.Loger.Log("启动成功！", "");
                        Console.Read();
                        break;
                }

            }

        }
    }
}
