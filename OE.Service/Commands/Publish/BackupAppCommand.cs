using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Publish
{
    public class BackupAppCommand : ICommand
    {
        public override int Execute(string[] args)
        {
            if (args == null || args.Count() != 2)
            {
                Msg = "无效参数，为二个参数！";
                return -1;
            }
            string appname = args[1];
            string appdir = Configrations.Config.GetAppDir(appname, "");
            if (string.IsNullOrEmpty(appdir))
            {
                Msg = "应用程序目录未配置！";
                return -1;
            }
            string backdir = Configrations.Config.GetAppBackup(appname, Configrations.Config.GetAppDefaultBackupDir(appname));
            if (!System.IO.Directory.Exists(appdir))
            {
                Msg = "应用程序目录不存在！";
                return -1;
            }
            if (!System.IO.Directory.Exists(backdir))
            {
                System.IO.Directory.CreateDirectory(backdir);
            }
            backdir = backdir.TrimEnd('\\') + "\\" + appname + "_" + DateTime.Now.ToString("yyMMddHHmmss");
            int copyfilecount = Utils.Utils.CopyDir(appdir, backdir);
            Msg = string.Format("从{0} 复制到 {1} 移动文件数{2}", appdir, backdir, copyfilecount);
            return 1;
        }
    }
}
