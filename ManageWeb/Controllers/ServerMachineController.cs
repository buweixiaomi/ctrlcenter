using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageDomain;

namespace ManageWeb.Controllers
{
    public class ServerMachineController : ManageBaseController
    {
        //
        // GET: /ServerMachine/

        public ActionResult Index(string keywords, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Server_Show);
            ViewBag.keywords = keywords;
            const int pagesize = 20;
            ManageDomain.BLL.ServerMachineBll cusbll = new ManageDomain.BLL.ServerMachineBll();
            var model = cusbll.GetPage(keywords ?? "", pno, pagesize);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int serverid = 0)
        {
            ManageDomain.Models.ServerMachine model = null;
            if (serverid > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Server_Show);
                var bll = new ManageDomain.BLL.ServerMachineBll();
                var mxmodel = bll.MxGetDetail(serverid);
                if (mxmodel == null)
                {
                    throw new MException("服务器不存在！");
                }
                model = mxmodel.Item1;
                ViewBag.configs = mxmodel.Item2;
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Server_Add);
            }
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.ServerMachine model, string[] configkey, string[] configvalue, string[] configremark)
        {
            if (model == null)
            {
                ViewBag.msg = "无效参数！";
                return View(model);
            }
            if (model.ServerId > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Server_Update);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Server_Add);
            }

            model.ServerOS = model.ServerOS ?? "";
            if (string.IsNullOrWhiteSpace(model.ServerName))
            {
                ViewBag.msg = "服务器名称不能为空！";
                return View(model);
            }
            List<ManageDomain.Models.ServerConfig> configs = new List<ManageDomain.Models.ServerConfig>();
            if (configkey != null && configvalue != null && configremark != null)
            {
                if (configkey.Length == configvalue.Length && configvalue.Length == configremark.Length)
                {
                    for (int i = 0; i < configkey.Length; i++)
                    {
                        string key = (configkey[i] ?? "").Trim();
                        if (string.IsNullOrEmpty(key))
                            continue;
                        configs.Add(new ManageDomain.Models.ServerConfig()
                        {
                            ServerId = model.ServerId,
                            ConfigKey = key,
                            ConfigValue = CCF.DB.LibConvert.NullToStr(configvalue[i]).Trim(),
                            Remark = CCF.DB.LibConvert.NullToStr(configremark[i]).Trim(),
                        });
                    }
                }
            }
            var bll = new ManageDomain.BLL.ServerMachineBll();
            if (model.ServerId > 0)
            {

                bll.Update(model, configs);
                ViewBag.msg = "修改成功";
            }
            else
            {
                model = bll.Add(model, configs);
                ViewBag.msg = "新增成功";
            }
            var mxmodel = bll.MxGetDetail(model.ServerId);
            model = mxmodel.Item1;
            ViewBag.configs = mxmodel.Item2;
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(int serverid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Server_Delete);
            var bll = new ManageDomain.BLL.ServerMachineBll();
            int r = bll.Delete(serverid);
            if (r > 0)
            {
                return Json(new { code = 1 });
            }
            else
            {
                return Json(new { code = -1, msg = "删除失败" });
            }
        }

        public JsonResult GetCacheClients()
        {
            return JsonE(ManageDomain.ClientsCache.GetAll());
        }
    }
}
