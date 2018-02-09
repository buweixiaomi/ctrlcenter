using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageWeb
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Dapper.SqlMapper.AddTypeHandler(typeof(Nullable<DateTime>), new ManageDomain.DapperNullableHandler());
            //Dapper.SqlMapper.AddTypeHandlerImpl(typeof(Nullable<DateTime>), new ManageDomain.DapperNullableHandler(), false);
        }

        void Application_BeginRequest(Object sender, EventArgs e)
        {
            var rq = System.Web.HttpContext.Current;
            CCF.WatchLog.Loger.Log(rq.Request.Url.ToString(), string.Format("【url】{0};\r\n\t【IP】{1};\r\n\t【UserHostName】{2};", rq.Request.Url.ToString(), rq.Request.UserHostAddress, rq.Request.UserHostName));
        }


        void Application_EndRequest(Object sender, EventArgs e)
        {
            var rq = System.Web.HttpContext.Current;
            CCF.WatchLog.Loger.Log(rq.Request.Url.ToString(), "结束请求");
        }

    }
}