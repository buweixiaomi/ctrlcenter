using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageDomain.Models;

namespace ManageWeb.Controllers
{
    public class CmdController : ManageBaseController
    {
        //
        // GET: /Command/
        ManageDomain.BLL.CommandBll cmdbll = new ManageDomain.BLL.CommandBll();
        public ActionResult Index(string groupid, int? serverid, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Command_Show);
            ViewBag.groupid = groupid;
            ViewBag.serverid = serverid;

            var data = cmdbll.GetPage(groupid ?? "", serverid ?? 0, pno, 20);
            return View(data);
        }

        public ActionResult Operate()
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Command_Add);
            ViewBag.alltag = new ManageDomain.BLL.ServerProjectBll().GetAllTag();
            return View();
        }


        public JsonResult getservers()
        {
            var serverbll = new ManageDomain.BLL.ServerMachineBll();
            return JsonE(serverbll.GetMiniServers(1000));
        }

        public ActionResult getserverproject(int projectid, string tags)
        {
            var tagarr = Pub.SplitTags(tags ?? "");
            var bll = new ManageDomain.BLL.ServerProjectBll();
            var model = bll.GetServerProjectsByProjectid(projectid);
            return Json(new { code = 1, data = model });
        }

        public JsonResult recommitcmd(int cmdid)
        {
            var model = cmdbll.ReCommit(cmdid);
            return JsonE(model.CmdId);
        }


        public JsonResult deletecmd(int cmdid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Command_Delete);
            var model = cmdbll.DeleteCmd(cmdid);
            return JsonE(model);
        }


        public ActionResult Detail(int cmdid)
        {
            ManageDomain.Models.ServerMachine serverinfo = null;
            var model = cmdbll.GetDetailWidthArg(cmdid, out serverinfo);
            ViewBag.serverinfo = serverinfo;
            return View(model);
        }

        #region 提交命令

        [HttpPost]
        public JsonResult SubmitPublish(int projectid, int versionid, string serverprojectids)
        {
            List<int> ids = new List<int>();
            foreach (var a in (serverprojectids ?? "").Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                ids.Add(CCF.DB.LibConvert.StrToInt(a));
            }
            int r = cmdbll.SetPublishApp(projectid, versionid, ids.ToArray());
            return Json(new JsonEntity() { code = 1, data = r, msg = "总发布数" + r });
        }

        [HttpPost]
        public JsonResult SubmitBackupProject(int projectid, string serverprojectids)
        {
            List<int> ids = new List<int>();
            foreach (var a in (serverprojectids ?? "").Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                ids.Add(CCF.DB.LibConvert.StrToInt(a));
            }
            int r = cmdbll.SetBackupApp(projectid, ids.ToArray());
            return Json(new JsonEntity() { code = 1, data = r, msg = "总备份数" + r });
        }


        [HttpPost]
        public JsonResult SubmitRollbackProject(int projectid, string serverprojectids)
        {
            List<int> ids = new List<int>();
            foreach (var a in (serverprojectids ?? "").Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                ids.Add(CCF.DB.LibConvert.StrToInt(a));
            }
            int r = cmdbll.SetRollbackApp(projectid, ids.ToArray());
            return Json(new JsonEntity() { code = 1, data = r, msg = "总回退数" + r });
        }


        public JsonResult submitupdateconfig(string serverids)
        {
            List<int> ids = new List<int>();
            foreach (var a in (serverids ?? "").Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                ids.Add(CCF.DB.LibConvert.StrToInt(a));
            }
            int r = cmdbll.SetUpdateConfig(ids.ToArray());
            return Json(new JsonEntity() { code = 1, data = r, msg = "总更新数" + r });
        }

        public JsonResult submitexeccmd(string cmdline, string serverids)
        {
            List<int> ids = new List<int>();
            foreach (var a in (serverids ?? "").Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                ids.Add(CCF.DB.LibConvert.StrToInt(a));
            }
            int r = cmdbll.SetExecCmd(cmdline, ids.ToArray());
            return Json(new JsonEntity() { code = 1, data = r, msg = "总更新数" + r });
        }
        #endregion

    }
}
