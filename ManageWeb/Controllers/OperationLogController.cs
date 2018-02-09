using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageDomain.Models;
using ManageDomain;

namespace ManageWeb.Controllers
{
    public class OperationLogController : ManageBaseController
    {
        //
        // GET: /OperationLog/
        ManageDomain.BLL.OperationLogBll logbll = new ManageDomain.BLL.OperationLogBll();
        public ActionResult Index(int pno = 1, string BeginTime = "", string EndTime = "", string keywords = "")
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.OperationLog_Show);
            //logbll.AddLog(new OperationLog { 
            // Createtime=DateTime.Now,
            // OperationContent="测试操作客户管理",
            // OperationTitle="编辑客户",
            // Module="客户管理",
            // OperationName="test"
            //});
            ViewBag.keywords = keywords;
            ViewBag.BeginTime = BeginTime;
            ViewBag.EndTime = EndTime;
            const int pagesize = 20;
            var model = logbll.GetLogPage(pno, pagesize, keywords, BeginTime, EndTime);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.OperationLog_Show);
            if (id > 0)
            {
                var model = logbll.GetLogDetail(id);
                return View(model);
            }
            else
            {
                return View();
            }
        }

    }
}
