using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageDomain.Models;

namespace ManageWeb.Controllers
{
    public class ServerProjectController : ManageBaseController
    {
        //
        // GET: /CusProject/

        ManageDomain.BLL.ServerProjectBll bll = new ManageDomain.BLL.ServerProjectBll();
        public ActionResult Index(string tag, string projectinfo, string serverinfo, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.ServerProject_Show);
            ViewBag.projectinfo = projectinfo;
            ViewBag.serverinfo = serverinfo;
            const int pagesize = 20;
            var rmdoel = bll.SearchPage(tag, serverinfo, projectinfo, pno, pagesize);
            return View(rmdoel);
        }

        [HttpGet]
        public ActionResult Edit(int serverprojectid = 0)
        {
            Tuple<ManageDomain.Models.ServerProject, ManageDomain.Models.Project, ManageDomain.Models.ServerMachine, List<ManageDomain.Models.ServerProjectConfig>> model = null;
            if (serverprojectid > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.ServerProject_Show);
                var mxmodel = bll.GetDetailWith(serverprojectid);
                if (mxmodel == null)
                {
                    throw new MException("项目不存在！");
                }
                model = mxmodel;
                return View(model);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.ServerProject_Add);
                return View(model);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.ServerProject model, string[] tag, string[] configkey, string[] configvalue, string[] configremark)
        {
            if (model == null)
            {
                ViewBag.msg = "无效参数";
                return View();
            }
            if (model.ServerProjectId > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.ServerProject_Update);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.ServerProject_Add);
            }

            #region config and tags
            List<ManageDomain.Models.ServerProjectConfig> configs = new List<ManageDomain.Models.ServerProjectConfig>();
            if (configkey != null && configvalue != null && configremark != null)
            {
                if (configkey.Length == configvalue.Length && configvalue.Length == configremark.Length)
                {
                    for (int i = 0; i < configkey.Length; i++)
                    {
                        string key = (configkey[i] ?? "").Trim();
                        if (string.IsNullOrEmpty(key))
                            continue;
                        configs.Add(new ManageDomain.Models.ServerProjectConfig()
                        {
                            CanDelete = 1,
                            ServerProjectId = model.ServerProjectId,
                            ProjectId = model.ProjectId,
                            ConfigKey = key,
                            ConfigValue = CCF.DB.LibConvert.NullToStr(configvalue[i]).Trim(),
                            Remark = CCF.DB.LibConvert.NullToStr(configremark[i]).Trim(),
                        });
                    }
                }
            }
            string _tag = ManageDomain.Pub.CombineTags(tag);
            model.Tag = _tag;
            #endregion

            var mxmodel =  new Tuple<ServerProject, Project, ServerMachine, List<ManageDomain.Models.ServerProjectConfig>>(model, null, null, configs);

            if (model.ProjectId <= 0)
            {
                ViewBag.msg = "请选择有效的项目";
                return View(mxmodel);
            }
            if (model.ServerId < 0)
            {
                ViewBag.msg = "请选择有效的服务器";
                return View(mxmodel);
            }

            if (model.ServerProjectId <= 0)
            {
                model = bll.Add(model, configs);
            }
            else
            {
                bll.Update(model, configs);
            }
            mxmodel = bll.GetDetailWith(model.ServerProjectId);
            return View(mxmodel);
        }


        public JsonResult Delete(int serverprojectid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.ServerProject_Delete); 
            int r = bll.Delete(serverprojectid);
            if (r > 0)
            {
                return Json(new { code = 1, msg = "删除成功" });
            }
            else
            {
                return Json(new { code = -1, msg = "删除失败" });
            }
        }
    }
}
