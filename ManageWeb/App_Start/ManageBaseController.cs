using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb
{

    [Authorize]
    public class ManageBaseController : Controller
    {
        public ManageDomain.Entity.LoginTokenModel Token;
        protected override void OnException(ExceptionContext filterContext)
        {
            int code = (int)ManageDomain.MExceptionCode.ServerError;
            if (filterContext.Exception is ManageDomain.MException)
            {
                code = (filterContext.Exception as ManageDomain.MException).Code;
            }
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 200;

                var vresult = Json(new JsonEntity() { code = code, data = null, msg = filterContext.Exception.Message }, JsonRequestBehavior.AllowGet);
                vresult.ExecuteResult(filterContext.Controller.ControllerContext);
                filterContext.Controller.ControllerContext.HttpContext.Response.End();
            }
            else
            {
                var vresult = View("Error", filterContext.Exception);
                vresult.ExecuteResult(filterContext.Controller.ControllerContext);
                filterContext.Controller.ControllerContext.HttpContext.Response.End();
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (User != null)
                if (User.Identity.IsAuthenticated)
                {
                    Token = ManageDomain.Pub.GetTokenModel(User.Identity.Name);
                    filterContext.HttpContext.Session["CurrUserId"] = Token.Id;
                }
        }

        public JsonResult JsonE(object data)
        {
            return Json(new JsonEntity() { code = 1, data = data, msg = "" });
        }
    }
}
