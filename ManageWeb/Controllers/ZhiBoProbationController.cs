using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class ZhiBoProbationController : Controller
    {
        //
        // GET: /ZhiBoProbation/
        ManageDomain.BLL.ZhiBoProbationBll bll = new ManageDomain.BLL.ZhiBoProbationBll();
        public ActionResult Index(string keywords, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Probation_Show);
            ViewBag.keywords = keywords;
            const int pagesize = 20;
            var model = bll.GetProbation(keywords, pno, pagesize);
            return View(model);
        }
        public JsonResult AddRemark(int id, string remark)
        {
            var r = bll.AddRemark(id, remark);
            if (r > 0)
                return Json(new JsonEntity { code = 1, msg = "" });
            else
                return Json(new JsonEntity { code = -1, msg = "添加失败" });
        }
        public JsonResult GetDetail(int id)
        {
            var model = bll.GetDetail(id);
            if (model != null)
                return Json(new JsonEntity { code = 1, data = model.Remark });
            else
                return Json(new JsonEntity { code = -1, msg = "失败" });
        }

    }
}
