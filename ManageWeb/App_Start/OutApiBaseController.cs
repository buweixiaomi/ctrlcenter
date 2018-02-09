using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace ManageWeb
{
    public class OutApiBaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            int code = (int)ManageDomain.MExceptionCode.ServerError;
            if (filterContext.Exception is ManageDomain.MException)
            {
                code = (filterContext.Exception as ManageDomain.MException).Code;
            }

            filterContext.HttpContext.Response.StatusCode = 200;
            var vresult = Json(new JsonEntity() { code = code, data = null, msg = filterContext.Exception.Message }, JsonRequestBehavior.AllowGet);
            vresult.ExecuteResult(filterContext.Controller.ControllerContext);
            filterContext.Controller.ControllerContext.HttpContext.Response.End();
        }

        public JsonResult JsonError(string msg)
        {
            return Json(new JsonEntity() { code = -1, msg = msg });
        }

        public JsonResult JsonE(object data)
        {
            return Json(new JsonEntity() { code = 1, data = data, msg = "" });
        }

    }
}
