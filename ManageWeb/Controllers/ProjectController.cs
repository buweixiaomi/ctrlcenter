using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageDomain;

namespace ManageWeb.Controllers
{
    public class ProjectController : ManageBaseController
    {
        //
        // GET: /Project/
        public ActionResult Index(string keywords, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Project_Show);
            ViewBag.keywords = keywords;
            const int pagesize = 20;
            ManageDomain.BLL.ProjectBll cusbll = new ManageDomain.BLL.ProjectBll();
            var model = cusbll.GetPage(keywords ?? "", pno, pagesize);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int projectid = 0)
        {
            ManageDomain.Models.Project model = null;
            if (projectid > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Project_Show);
                var bll = new ManageDomain.BLL.ProjectBll();
                var mxmodel = bll.GetDetailWidthConfigs(projectid);
                if (mxmodel == null)
                {
                    throw new MException("项目不存在！");
                }
                model = mxmodel.Item1;
                ViewBag.configs = mxmodel.Item2;
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Project_Add);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.Project model, string[] configkey, string[] configvalue, string[] configremark)
        {
            if (model == null)
            {
                ViewBag.msg = "无效参数！";
                return View(model);
            }
            if (model.ProjectId > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Project_Update);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Project_Add);
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
            var bll = new ManageDomain.BLL.ProjectBll();
            List<ManageDomain.Models.ProjectConfig> configs = new List<ManageDomain.Models.ProjectConfig>();
            if (configkey != null && configvalue != null && configremark != null)
            {
                if (configkey.Length == configvalue.Length && configvalue.Length == configremark.Length)
                {
                    for (int i = 0; i < configkey.Length; i++)
                    {
                        string key = (configkey[i] ?? "").Trim();
                        if (string.IsNullOrEmpty(key))
                            continue;
                        configs.Add(new ManageDomain.Models.ProjectConfig()
                        {
                            ProjectId = model.ProjectId,
                            ConfigKey = key,
                            ConfigValue = CCF.DB.LibConvert.NullToStr(configvalue[i]).Trim(),
                            Remark = CCF.DB.LibConvert.NullToStr(configremark[i]).Trim(),
                        });
                    }
                }
            }
            if (model.ProjectId > 0)
            {
                bll.Update(model, configs);
                ViewBag.msg = "修改成功";
            }
            else
            {
                model = bll.Add(model, configs);
                ViewBag.msg = "新增成功";
            }
            var mxmodel = bll.GetDetailWidthConfigs(model.ProjectId);
            model = mxmodel.Item1;
            ViewBag.configs = mxmodel.Item2;
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(int projectid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Project_Delete);
            var bll = new ManageDomain.BLL.ProjectBll();
            int r = bll.Delete(projectid);
            if (r > 0)
            {
                return Json(new { code = 1 });
            }
            else
            {
                return Json(new { code = -1, msg = "删除失败" });
            }
        }

        public ActionResult Version(int projectid)
        {
            var bll = new ManageDomain.BLL.ProjectBll();
            var projectversions = bll.GetProjectVersions(projectid);
            var projectmodel = bll.GetDetail(projectid);
            ViewBag.versions = projectversions;
            return View(projectmodel);
        }

        [HttpPost]
        public ActionResult Version(ManageDomain.Models.ProjectVersion model, HttpPostedFileBase downloadfile)
        {
            var bll = new ManageDomain.BLL.ProjectBll();
            model.DownloadUrl = model.DownloadUrl ?? "";
            if (downloadfile != null)
            {
                string filename = DateTime.Now.ToString("yyMMddHHmmss") + ".zip";
                string pathname = Pub.GetConfig(SystemConst.Project_File_Config_Name, "ProjectFile");
                string path = Server.MapPath(Pub.GetConfig(SystemConst.Project_File_Config_Name, "~/ProjectFile"));
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                string filefullname = System.IO.Path.Combine(path, filename);
                downloadfile.SaveAs(filefullname);
                model.DownloadUrl = System.IO.Path.Combine(pathname, filename);
            }
            if (model.VersionId > 0)
            {
                bll.UpdateProjectVersion(model);
            }
            else
            {
                model = bll.AddProjectVersion(model);
            }
            var projectversions = bll.GetProjectVersions(model.ProjectId);
            var projectmodel = bll.GetDetail(model.ProjectId);
            ViewBag.versions = projectversions;
            return View(projectmodel);
        }


        [HttpPost]
        public JsonResult SynConfig(string configkey)
        {
            var bll = new ManageDomain.BLL.ProjectBll();
            int r = bll.SynConfigToCusProject(configkey);
            return Json(new { code = 1, data = r });
        }
    }
}
