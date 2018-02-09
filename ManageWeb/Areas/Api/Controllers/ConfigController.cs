using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace ManageWeb.Areas.Api.Controllers
{
    public class ConfigController : ApiBaseController
    {
        [ClientAuth(ClientAuthType.TryAuth)]
        public ActionResult Ping()
        {
            if (ServerId > 0)
            {
                var serverbll = new ManageDomain.BLL.ServerMachineBll();
                var model = serverbll.PingTask(ServerId);
                return JsonE(model);
            }
            return Json(new { code = -1001, msg = "Ping成功,但未绑定！", data = new { MaxCmdID = 0 } });
        }

        [ClientAuth(ClientAuthType.Auth)]
        public ActionResult uploaddata(string uploadtype, string data)
        {
            uploadtype = (uploadtype ?? "").ToLower();
            switch (uploadtype)
            {
                case "watchdata":
                    new ManageDomain.BLL.ServerWatchBll().SaveWatchData(this.ServerId, data);
                    break;
                case "tasksummary":
                    new ManageDomain.BLL.TaskBll().SaveTaskSummary(this.ServerId, data);
                    break;
                default:
                    break;
            }

            return JsonE(1);
        }

        [ClientAuth(ClientAuthType.Auth)]
        public ActionResult GetConfig()
        {
            var bll = new ManageDomain.BLL.ServerMachineBll();

            var configs = bll.GetServerUnionConfig(ServerId);
            return JsonE(configs);
        }

        public ActionResult DownloadFile(string filename)
        {
            filename = filename.Replace("\\", "/").Replace("..", "").Replace("~", "").TrimStart('/');
            string file = Server.MapPath("~/" + filename);
            return File(file, "application/octet-stream");
        }

        [ClientAuth(ClientAuthType.Auth)]
        public ActionResult PostData(int typecode, string data)
        {
            return JsonE("接收到了数据！" + data);
        }
    }
}
