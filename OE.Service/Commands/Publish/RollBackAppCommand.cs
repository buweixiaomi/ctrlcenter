using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Publish
{
    public class RollBackAppCommand : ICommand
    {
        /// <summary>
        /// App version
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override int Execute(string[] args)
        {
            if (args == null || args.Count() != 3)
            {
                Msg = "为三个参数！";
                return -1;
            }
            string appname = args[1];
            string toappversion = args[2];
            string appdir = Configrations.Config.GetAppDir(appname, "");
            string appbackupdir = Configrations.Config.GetAppBackup(appname, Configrations.Config.GetAppDefaultBackupDir(appname));
            if (string.IsNullOrEmpty(appdir))
            {
                Msg = "应用不存在！";
                return -1;
            }
            if (!System.IO.Directory.Exists(appbackupdir))
            {
                Msg = "备份文件不存在！";
                return -1;
            }
            string toversiondir = "";
            if (toappversion == "-1")
            {
                //回退到上一个备份版本
                toversiondir = GetAppAllBackupTag(appbackupdir, appname).LastOrDefault();
            }
            else
            {
                //回退到特定版本 需要有版本号
                var v_vs = GetAppAllBackupVersion(appbackupdir, appname).Where(x => x.Item1 == toappversion).ToList().OrderBy(x => x.Item2).FirstOrDefault();
                if (v_vs != null)
                {
                    toversiondir = v_vs.Item3;
                }
            }
            if (string.IsNullOrEmpty(toversiondir))
            {
                Msg = "回退版本不存在！";
                return -1;
            }
            int copyfiles = Utils.Utils.CopyDir(toversiondir, appdir);
            string versionfilename = System.IO.Path.Combine(toversiondir, Configrations.ConfigConst.AppVersionFileName);
            string newtoversion = "";
            if (System.IO.File.Exists(versionfilename))
            {
                newtoversion = System.IO.File.ReadAllText(versionfilename);
            }
            Msg = "回退成功，回退到备份" + toversiondir +"；版本号："+newtoversion+ ";复制文件数：" + copyfiles;
            return 1;
        }

        private List<string> GetAppAllBackupTag(string backupdir, string appname)
        {
            List<string> vs = new List<string>();
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(backupdir);
            if (dirinfo.Exists)
            {
                foreach (var a in dirinfo.GetDirectories(appname + "_*"))
                {
                    vs.Add(a.FullName);
                }
            }
            vs.Sort();
            return vs;
        }

        private List<Tuple<string, string, string>> GetAppAllBackupVersion(string backupdir, string appname)
        {
            List<Tuple<string, string, string>> vs = new List<Tuple<string, string, string>>();
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(backupdir);
            if (dirinfo.Exists)
            {
                foreach (var a in dirinfo.GetDirectories(appname + "_*"))
                {
                    string versionfile = a.FullName.TrimEnd('\\') + "\\" + Configrations.ConfigConst.AppVersionFileName;
                    if (!System.IO.File.Exists(versionfile))
                        continue;
                    string version = System.IO.File.ReadAllText(a.FullName.TrimEnd('\\') + "\\" + Configrations.ConfigConst.AppVersionFileName, Encoding.UTF8);
                    if (string.IsNullOrEmpty(version))
                        continue;
                    vs.Add(new Tuple<string, string, string>(version, a.Name.Replace(appname + "_", ""), a.FullName));
                }
            }
            return vs;
        }


    }
}
