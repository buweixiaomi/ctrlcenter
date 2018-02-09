using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Areas.Api.Controllers
{
    public class ApiLogController : Controller
    {
        //
        // GET: /Api/ApiLog/

        public JsonResult Index(ManageDomain.ApiLogReqEntity req)
        {
            if (string.IsNullOrWhiteSpace(req.signKey) || req.signKey.Length < 41)
            {
                return Json(new { code = -1, msg = "非法请求！" });
            }
            if (!ManageDomain.ApiLogHelper.CheckSign(req.signKey ?? "", req.sign ?? ""))
            {
                return Json(new { code = -1, msg = "非法请求！" });
            }
            ManageDomain.ApiLogHelper.WriteLog(req.logs);
            return Json(new
            {
                code = 1,
                msg = ""
            });
        }
    }

}
