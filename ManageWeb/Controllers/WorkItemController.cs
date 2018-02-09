using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class WorkItemController : ManageBaseController
    {
        ManageDomain.BLL.WorkItemBll workitembll = new ManageDomain.BLL.WorkItemBll();
        //
        // GET: /WorkItem/

        public ActionResult Index(int? distributeuserid, int? createuserid, int? workitemstate, string distributetome = "", int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Show);

            ViewBag.distributetome = distributetome == "on";
            ViewBag.distributeuserid = distributeuserid;
            ViewBag.createuserid = createuserid;
            ViewBag.workitemstate = workitemstate;
            if (distributetome == "on")
            {
                distributeuserid = User.CurrUserId();
            }
            const int pagesize = 15;
            var models = workitembll.GetPage(distributeuserid, createuserid, workitemstate, pno, pagesize);
            return View(models);
        }

        [HttpGet]
        public ActionResult Add()
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Add);
            return View("Edit");
        }


        [HttpPost]
        public ActionResult Add(ManageDomain.Models.WorkItem model, int[] distributeuser, string[] tag)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Add);
            string _tag = ManageDomain.Pub.CombineTags(tag);
            model.Tag = _tag;
            model.Content = ManageDomain.Pub.URLDecode(model.Content);
            model.Distributes = workitembll.BuildDistributes(distributeuser);
            model.ManagerId = this.Token.Id;
            model.ManagerName = this.Token.Name;
            try
            {
                model = workitembll.Add(model);
                return RedirectToAction("Edit", new { workitemid = model.WorkItemId });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View("Edit", model);
            }
        }

        [HttpGet]
        public ActionResult Edit(int workitemid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Update);
            var model = workitembll.GetDetail(workitemid);
            if (model == null)
            {
                throw new ManageDomain.MException(ManageDomain.MExceptionCode.NotExist, "工作项不存在！");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.WorkItem model, int[] distributeuser, string[] tag)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Update);
            if (model == null)
            {
                throw new MException(MExceptionCode.BusinessError, "无效参数");
            }
            string _tag = ManageDomain.Pub.CombineTags(tag);
            model.Tag = _tag;
            model.Content = ManageDomain.Pub.URLDecode(model.Content);
            model.Distributes = workitembll.BuildDistributes(distributeuser);
            model.ManagerId = this.Token.Id;
            model.ManagerName = this.Token.Name;
            try
            {
                model = workitembll.Update(model);
                model = workitembll.GetDetail(model.WorkItemId);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(model);
            }
        }

        public ActionResult Detail(int workitemid, bool exec = false)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Show);
            var model = workitembll.GetDetail(workitemid);
            ViewBag.exec = exec ? "" : null;
            if (model == null)
            {
                throw new ManageDomain.MException(ManageDomain.MExceptionCode.NotExist, "工作项不存在！");
            }
            return View(model);
        }

        public JsonResult Delete(int workitemid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_Delete);
            bool delete = workitembll.MakeDelete(workitemid);
            return JsonE("OK");
        }

        public JsonResult DistributeExec(ManageDomain.Models.WorkDistribute model)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_ExecWork);
            if (model == null)
            {
                throw new MException(MExceptionCode.BusinessError, "无效参数");
            }
            if (model.WorkItemId <= 0)
            {
                throw new MException(MExceptionCode.BusinessError, "无效工作项");
            }
            model.ManagerId = User.CurrUserId();
            bool delete = workitembll.DistributeExec(model);
            return JsonE("OK");

        }

        public ActionResult summary(int? distributeuserid, int pno = 1, string BeginTime = "", string EndTime = "")
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkItem_summary);
            ViewBag.distributeuserid = distributeuserid;          
            ViewBag.BeginTime = BeginTime;
            ViewBag.EndTime = EndTime;
            const int pagesize = 20;
            var model = workitembll.GetSummary(BeginTime, EndTime, distributeuserid, pno, pagesize);
            return View(model);
        }
    }
}
