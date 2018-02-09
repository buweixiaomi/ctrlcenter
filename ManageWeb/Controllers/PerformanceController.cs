using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class PerformanceController : ManageBaseController
    {
        //
        // GET: /Performance/
        ManageDomain.BLL.ServerWatchBll swbll = new ManageDomain.BLL.ServerWatchBll();
        public ActionResult Index(string keywords = "", int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Performance);
            ViewBag.keywords = keywords;
            const int pagesize = 20;
            ManageDomain.BLL.ServerWatchBll cusbll = new ManageDomain.BLL.ServerWatchBll();
            var model = cusbll.GetServerPage(keywords ?? "", pno, pagesize);
            return View(model);
        }

        public ActionResult Detail(int serverid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Performance);
            var server = new ManageDomain.BLL.ServerMachineBll().GetDetail(serverid);
            if (server == null)
            {
                throw new ManageDomain.MException(MExceptionCode.BusinessError, "服务器不存在！");
            }
            ViewBag.server = server;
            ViewBag.serverid = serverid;
            return View();
        }


        public JsonResult GetChartData(string datatype, int serverid, DateTime? begintime, DateTime? endtime)
        {
            if (begintime == null)
                begintime = DateTime.Now.AddHours(-1);
            if (endtime == null)
                endtime = DateTime.Now;

            ManageDomain.Entity.ChartEntity rdata = null;
            switch (datatype)
            {
                case "cpu":
                    rdata = swbll.GetCpuChartData(serverid, begintime.Value, endtime.Value);
                    break;
                case "memory":
                    rdata = swbll.GetMemoryChartData(serverid, begintime.Value, endtime.Value);
                    break;
                case "diskio":
                    rdata = swbll.GetDiskIOChartData(serverid, begintime.Value, endtime.Value);
                    break;
                case "diskspace":
                    rdata = swbll.GetDiskSpaceChartData(serverid, begintime.Value, endtime.Value);
                    break;
                case "httprequest":
                    rdata = swbll.GetHttpRequestChartData(serverid, begintime.Value, endtime.Value);
                    break;
                case "networkio":
                    rdata = swbll.GetNetworkIOChartData(serverid, begintime.Value, endtime.Value);
                    break;
                default:
                    throw new Exception("无效数据类型");
            }
            return JsonE(rdata);
        }
    }
}
