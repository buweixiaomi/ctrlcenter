using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace ManageWeb
{
    public class ApiBaseController : Controller
    {
        public int ServerId { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("on");
            object[] attr = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ClientAuthAttribute), false);
            ClientAuthAttribute tt = new ClientAuthAttribute();
            if (attr == null || attr.Length == 0)
            {
                attr = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(ClientAuthAttribute), false);
            }
            if (attr != null && attr.Length > 0)
                tt = attr[0] as ClientAuthAttribute;
            if (tt.AuthType != ClientAuthType.None)
            {
                ManageDomain.BLL.ServerMachineBll serverbll = new ManageDomain.BLL.ServerMachineBll();

                string macs = filterContext.RequestContext.HttpContext.Request.Headers["Client_Macs"] ?? "";
                string ips = filterContext.RequestContext.HttpContext.Request.Headers["Client_IPs"] ?? "";
                string clientid = filterContext.RequestContext.HttpContext.Request.Headers["Client_ID"] ?? "";
                ips += "," + filterContext.RequestContext.HttpContext.Request.UserHostAddress;
                string[] arrmac = macs.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string[] arrip = ips.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();

                var model = serverbll.GetServerByClientId(clientid);// serverbll.GetUnionServer(arrmac, arrip, clientid);
                if (model == null)
                {
                    ManageDomain.ClientsCache.AddClientInfo(clientid, string.Format("ClientId:{2} MAC:{0} IP:{1}", macs, ips, clientid));
                }
                else
                {
                    ManageDomain.ClientsCache.Remove(clientid);
                }


                if (model == null && tt.AuthType == ClientAuthType.Auth)
                {
                    throw new ManageDomain.MException(ManageDomain.MExceptionCode.NoPermission, "无权限");
                    //filterContext.HttpContext.Response.StatusCode = 200;
                    //var vresult = Json(new JsonEntity() { code = (int)ManageDomain.MExceptionCode.NoPermission, data = null, msg = "无权限" }, JsonRequestBehavior.AllowGet);
                    //vresult.ExecuteResult(filterContext.Controller.ControllerContext);
                    //filterContext.Controller.ControllerContext.HttpContext.Response.End();
                    //filterContext.HttpContext.Response.Close();
                    //throw new System.Web.HttpUnhandledException("无权限");
                }
                if (model != null)
                    ServerId = model.ServerId;
            }
            base.OnActionExecuting(filterContext);

        }
        public JsonResult ApiResult(object obj)
        {
            return JsonE(obj);
        }

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

        public JsonResult JsonE(object data)
        {
            return Json(new JsonEntity() { code = 1, data = data, msg = "" });
        }

    }
}
