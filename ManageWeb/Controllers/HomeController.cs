using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class HomeController : ManageBaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (ManageDomain.PermissionProvider.ExistWidthCache(ManageDomain.SystemPermissionKey.WorkItem_ExecWork))
            {
                ManageDomain.BLL.WorkItemBll workitembll = new ManageDomain.BLL.WorkItemBll();
                ViewBag.waitworks = workitembll.GetPage(Token.Id, null, 2, 1, 5).list;
            }

            if (ManageDomain.PermissionProvider.ExistWidthCache(ManageDomain.SystemPermissionKey.WorkDaily_Add))
            {
                ViewBag.addworkdaily = "";
            }


            return View();
        }

    }
}
