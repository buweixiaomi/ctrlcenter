using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk.Model
{
    public class TaskDetail
    {
        public int TaskID { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }
        public string Url { get; set; }

        public string DownloadFileName { get; set; }

        public string Dll { get; set; }

        /// <summary>
        /// 服务器任务状态  0:停止 1运行中 -1:已删除
        /// </summary>
        public int LastServerState { get; set; }

        public string ClassFullName { get; set; }

        public string RunCron { get; set; }

        public string ConfigJson { get; set; }
    }
}
