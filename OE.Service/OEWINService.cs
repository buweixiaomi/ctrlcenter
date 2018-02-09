using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace OE.Service
{
    partial class OEWINService : ServiceBase
    {
        public OEWINService()
        {
            InitializeComponent();
            this.ServiceName = Configrations.ConfigConst.ServiceName;
        }

        ServiceContainer sc = null;
        protected override void OnStart(string[] args)
        {

            CCF.WatchLog.Loger.Log("正在启动...", "");
            sc = new ServiceContainer();
            sc.Start();
            CCF.WatchLog.Loger.Log("启动成功！", "");
            Console.Read();
            // TODO:  在此处添加代码以启动服务。
        }

        protected override void OnStop()
        {
            if (sc != null)
            {
                sc.Stop();
            }
            // TODO:  在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
