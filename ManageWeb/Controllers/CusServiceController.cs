using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageDomain;

namespace ManageWeb.Controllers
{
    public class CusServiceController : ManageBaseController
    {

        ManageDomain.BLL.CusServiceBll cusservicebll = new ManageDomain.BLL.CusServiceBll();
        public ActionResult Index(string keywords, int? cusid = null, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_ServiceLog_Show);
            ViewBag.keywords = keywords;
            ViewBag.cusid = cusid;
            const int pagesize = 20;
            var model = cusservicebll.GetPage(pno, pagesize, cusid ?? 0, keywords);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_ServiceLog_Add);
            return View();
        }


        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.CusService model, string Show_CusId = "")
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_ServiceLog_Add);
            ViewBag.Show_CusId = Show_CusId;
            try
            {
                if (model == null)
                    throw new MException(MExceptionCode.BusinessError, "无效参数！");
                if (model.CusId <= 0)
                {
                    throw new MException(MExceptionCode.BusinessError, "请选择客户！");
                }
                model = cusservicebll.Add(model);
                return RedirectToAction("Detail", new { cusserviceid = model.CusServiceId });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(model);
            }
        }

        public ActionResult Detail(int cusserviceid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_ServiceLog_Show);
            var mdoel = cusservicebll.GetDetail(cusserviceid);
            if (mdoel == null)
            {
                throw new MException(MExceptionCode.NotExist, "记录不存在！");
            }
            return View(mdoel);
        }

    }
}
