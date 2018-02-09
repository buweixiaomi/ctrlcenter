using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Publish
{
    public class PublishAppCommand : ICommand
    {
        /// <summary>
        /// app url version
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override int Execute(string[] args)
        {
            if (args == null || args.Count() != 4)
            {
                Msg = "参数为四个！";
                return -1;
            }
            string appname = args[1];
            string version = args[2];
            string downloadurl = args[3];

            string appdir = Configrations.Config.GetAppDir(appname, "");
            if (string.IsNullOrEmpty(appdir))
            {
                Msg = "应用程序不存在！";
                return -1;
            }
            if (System.IO.Directory.Exists(appdir))
            {
                //先备份
                BackupAppCommand backupcmd = new BackupAppCommand();
                int backupresult = backupcmd.Execute(new string[] { appname });
                if (backupresult <= 0)
                {
                    Msg = backupcmd.Msg;
                    return -1;
                }
            }
            //下载文件
            var downloadfileresult = new ApiSdk.CommApi().DownloadFile(downloadurl);
            if (downloadfileresult.code <= 0)
            {
                Msg = downloadfileresult.msg;
                return -1;
            }
            //保存到暂时文件
            string fillename = Configrations.Config.GetTempFileName();
            using (var filestream = new System.IO.FileStream(fillename, System.IO.FileMode.CreateNew))
            {
                filestream.Write(downloadfileresult.data, 0, downloadfileresult.data.Length);
                filestream.Flush();
            }
            //解压 到app目录
            Utils.Utils.UnZip(fillename, appdir, "", true);
            //删除临时文件
            System.IO.File.Delete(fillename);

            //添加版本文件
            System.IO.File.WriteAllText(appdir.TrimEnd('\\') + "\\" + Configrations.ConfigConst.AppVersionFileName, version, Encoding.UTF8);
            Msg = string.Format("发布完成; 新版本号:{0}", version);
            return 1;
        }
    }
}
