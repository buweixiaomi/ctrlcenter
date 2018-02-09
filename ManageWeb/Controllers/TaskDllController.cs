using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class TaskDllController : ManageBaseController
    {
        //
        // GET: /Task/

        public ActionResult Index(string keywords, int? serverid, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Show);
            ViewBag.keywords = keywords;
            ViewBag.serverid = serverid;
            const int pagesize = 20;
            var model = new ManageDomain.BLL.TaskBll().GetPage(keywords ?? "", serverid ?? 0, pno, pagesize);
            return View(model);
        }
        [HttpGet]
        public ActionResult Edit(int taskid = 0)
        {
            ManageDomain.Models.Task model = null;
            if (taskid > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Show);
                model = new ManageDomain.BLL.TaskBll().GetDetail(taskid);
                if (model == null)
                {
                    throw new MException("项目不存在！");
                }
                ViewBag.servername = new ManageDomain.BLL.ServerMachineBll().GetDetail(model.ServerID).ServerName;
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Add);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.Task model)
        {

            if (model == null)
            {
                ViewBag.msg = "无效参数！";
                return View(model);
            }
            if (model.TaskId > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Update);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Add);
            }
            if (string.IsNullOrWhiteSpace(model.CodeName))
            {
                ViewBag.msg = "项目代码不能为空！";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ViewBag.msg = "项目名称不能为空！";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.ServerID.ToString()) || model.ServerID <= 0)
            {
                ViewBag.msg = "请选择服务器ID！";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.ClassFullName))
            {
                ViewBag.msg = "该版本类名不能为空！";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.RunCron))
            {
                ViewBag.msg = "运行方案不能为空！";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Dll))
            {
                ViewBag.msg = "入口DLL不能为空！";
                return View(model);
            }
            var bll = new ManageDomain.BLL.TaskBll();
            if (model.TaskId > 0)
            {
                bll.EditTask(model);
                ViewBag.msg = "修改成功";
            }
            else
            {
                model = bll.AddTask(model);
                ViewBag.msg = "新增成功";
                return RedirectToAction("edit", new { taskid = model.TaskId });
            }
            model = bll.GetDetail(model.TaskId);
            return View(model);
        }
        [HttpPost]
        public JsonResult Delete(int taskid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Delete);
            var bll = new ManageDomain.BLL.TaskBll();
            int r = bll.DeleteTask(taskid);
            if (r > 0)
            {
                return Json(new { code = 1 });
            }
            else
            {
                return Json(new { code = -1, msg = "删除失败" });
            }

        }
        [HttpPost]
        public JsonResult SetTaskState(int taskid, int newstate)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Update);
            var bll = new ManageDomain.BLL.TaskBll();
            var cmdbll = new ManageDomain.BLL.CommandBll();
            switch (newstate)
            {
                case 1:
                    bll.UpdateTaskState(taskid, 1);//运行
                    cmdbll.SetStartTaskCmd(taskid);
                    return Json(new JsonEntity() { code = 1, msg = "任务运行命令已发出，请关注命令结果。" });
                case 0:
                    bll.UpdateTaskState(taskid, 0);//停止
                    cmdbll.SetStopTaskCmd(taskid);
                    return Json(new JsonEntity() { code = 1, msg = "任务运行命令已发出，请关注命令结果。" });
                case -1:
                    bll.UpdateTaskState(taskid, 0);//停止
                    cmdbll.SetDeleteTaskCmd(taskid);
                    return Json(new JsonEntity() { code = 1, msg = "任务卸载命令已发出，请关注命令结果。" });
                default:
                    throw new MException(MExceptionCode.BusinessError, "无效命令！");
            }
        }
        [HttpPost]
        public JsonResult Settaskversionid(int taskid, int versionid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Update);
            var bll = new ManageDomain.BLL.TaskBll();
            int r = bll.UpdateTaskVersionID(taskid, versionid);
            if (r > 0)
            {
                return Json(new { code = 1 });
            }
            else
            {
                return Json(new { code = -1, msg = "更新版本号失败" });
            }
        }

        public ActionResult Version(int taskid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Show);
            var bll = new ManageDomain.BLL.TaskBll();
            var taskversions = bll.GetTaskVersions(taskid);
            var taskmodel = bll.GetDetail(taskid);
            ViewBag.versions = taskversions;
            return View(taskmodel);
        }
        [HttpPost]
        public ActionResult Version(ManageDomain.Models.TaskVersion model, HttpPostedFileBase downloadfile)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Update);
            var bll = new ManageDomain.BLL.TaskBll();
            model.DownloadUrl = model.DownloadUrl ?? "";
            if (downloadfile != null)
            {
                string filename = DateTime.Now.ToString("yyMMddHHmmss") + ".zip";
                string pathname = Pub.GetConfig(SystemConst.Task_File_Config_Name, "TaskDllFile");
                string path = Server.MapPath(Pub.GetConfig(SystemConst.Task_File_Config_Name, "~/TaskDllFile"));
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                string filefullname = System.IO.Path.Combine(path, filename);
                downloadfile.SaveAs(filefullname);
                model.DownloadUrl = System.IO.Path.Combine(pathname, filename);
            }
            model = bll.AddTaskVersion(model);
            var taskversions = bll.GetTaskVersions(model.TaskId);
            var taskmodel = bll.GetDetail(model.TaskId);
            ViewBag.versions = taskversions;
            return RedirectToAction("version", new { taskid = taskmodel.TaskId });

        }
        [HttpPost]
        public JsonResult deletekversion(int versionid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Task_Update);
            var bll = new ManageDomain.BLL.TaskBll();
            int r = bll.DeleteTaskVersion(versionid);
            if (r > 0)
            {
                return Json(new { code = 1 });
            }
            else
            {
                return Json(new { code = -1, msg = "删除版本号失败" });
            }

        }
    }
}
