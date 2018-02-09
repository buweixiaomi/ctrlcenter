using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace ManageWeb.Areas.Api.Controllers
{
    public class TaskController : ApiBaseController
    {
        [ClientAuth(ClientAuthType.Auth)]
        public ActionResult Summary()
        {
            var model = new ManageDomain.BLL.TaskBll().GetTaskSummary(ServerId);
            return ApiResult(model);
        }

        [ClientAuth(ClientAuthType.Auth)]
        public ActionResult Detail(int taskid)
        {
            var model = new ManageDomain.BLL.TaskBll().GetCurrTaskDetail(taskid);
            if (model == null || model.Item1.State == -1)
                throw new ManageDomain.MException(ManageDomain.MExceptionCode.NotExist, "任务不存在！");
            return ApiResult(new
            {
                TaskID = model.Item1.TaskId,
                ClassFullName = model.Item1.ClassFullName,
                Name = model.Item1.Title,
                Remark = model.Item1.Remark,
                Url = ManageDomain.Pub.DirPathGetDowloadUrl(model.Item2.DownloadUrl),
                DownloadFileName = "任务" + model.Item2.TaskId,
                Dll = model.Item1.Dll,
                LastServerState = model.Item1.SeverState,
                RunCron = model.Item1.RunCron,
                ConfigJson = model.Item1.TaskConfig
            });
        }
    }
}
