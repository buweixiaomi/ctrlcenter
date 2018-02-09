using OE.Service.ApiSdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk
{
    public class TaskApi
    {

        public ApiResult<List<TasksSummaryResult>> GetTasksSummary()
        {
            return SdkCore.InvokeApi<List<TasksSummaryResult>>(Configrations.ConfigConst.API_TASK_SUMMARY, new { });
        }

        public ApiResult<TaskDetail> GetTaskDetail(int taskid)
        {
            return SdkCore.InvokeApi<TaskDetail>(Configrations.ConfigConst.API_TASK_DETAIL, new { taskid = taskid });
        }

    }
}
