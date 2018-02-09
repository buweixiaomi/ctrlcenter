using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    [AllowAnonymous]
    public class ManagerController : ManageBaseController
    {
        //
        // GET: /Manager/
        ManageDomain.BLL.ManagerBll managerbll = new ManageDomain.BLL.ManagerBll();
        public ActionResult Index(int pno = 1, string keywords = "")
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Manager_Show);
            const int pagesize = 20;
            var model = managerbll.GetManagerPage(pno, pagesize, keywords);
            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(int managerid = 0)
        {
            ViewBag.alltags = managerbll.GetAllManagerTag();
            if (managerid > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Manager_Show);
                var model = managerbll.GetManagerDetail(managerid);
                return View(model);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Manager_Add);
                return View();
            }
        }


        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.Manager model, int[] tag, string AllowLogin = "")
        {
            try
            {
                ViewBag.alltags = managerbll.GetAllManagerTag();
                if (model == null)
                {
                    throw new Exception("无效参数！");
                }
                model.AllowLogin = (AllowLogin ?? "").ToLower() == "on" ? 1 : 0;
                if (model.ManagerId > 0)
                {
                    //权限
                    ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Manager_Update);
                    managerbll.UpdateManager(model, tag);

                    ManageDomain.PermissionProvider.RemoveManagerKeys(model.ManagerId);

                    ViewBag.msg = "修改成功";
                }
                else
                {
                    //权限
                    ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Manager_Add);
                    model = managerbll.AddManager(model, tag);
                    ViewBag.msg = "新增成功";
                }
                model = managerbll.GetManagerDetail(model.ManagerId);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(model);
            }
        }

        public JsonResult ResetPwd(int managerid)
        {
            if (managerbll.ResetPwd(managerid))
            {
                return Json(new JsonEntity() { code = 1, msg = "重置成功" });
            }
            else
            {
                return Json(new JsonEntity() { code = 1, msg = "重置失败" });
            }
        }

        public ActionResult Tag()
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.UserTag_Show);
            ViewBag.alltags = managerbll.GetAllManagerTag();
            return View();
        }

        public JsonResult AddUserTag(string tag, string remark)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.UserTag_Add);
            var result = managerbll.AddUserTag(tag, remark);
            return JsonE("添加成功！");
        }

        public JsonResult DeleteUserTag(int usertagid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.UseTag_Delete);
            var result = managerbll.DeleteUserTag(usertagid);
            return JsonE("删除成功！");
        }

        public ActionResult TagPermission(int usertagid)
        {
            ManageDomain.BLL.PermissionBll pbll = new ManageDomain.BLL.PermissionBll();
            var result = pbll.GetTagPermission(usertagid);
            ViewBag.tagpermissions = result;
            return PartialView();
        }

        public JsonResult SaveTagPermission(int usertagid, string[] keys)
        {
            if (keys == null)
                keys = new string[0];
            var newkeys = keys.Distinct().ToList();
            ManageDomain.BLL.PermissionBll pbll = new ManageDomain.BLL.PermissionBll();
            var result = pbll.SaveTagPermission(usertagid, newkeys);

            ManageDomain.PermissionProvider.ClearCache();

            return JsonE("OK");
        }

        [HttpPost]
        public JsonResult DeleteManager(int managerid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Manager_Delete);
            bool deleteok = managerbll.DeleteManager(managerid);
            if (deleteok)
                return JsonE("OK");
            else
            {
                return Json(new JsonEntity() { code = -1, msg = "删除失败" });
            }
        }
    }
}
