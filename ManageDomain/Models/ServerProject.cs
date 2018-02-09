using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class ServerProject
    {
        public int ServerProjectId { get; set; }
        public string Title { get; set; } 
        public int ProjectId { get; set; }

        public int ServerId { get; set; }
        /// <summary>
        /// 0 正常    1关闭  -1已删除
        /// </summary>
        public int State { get; set; }
        public string CopyRightConfig { get; set; }
        public string Tag { get; set; }

        public string Remark { get; set; }

        public string FunctionRemark { get; set; }

        public string ServerVersion { get; set; }
        public DateTime? LastUpdateTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }


    public class ServerProjectConfig
    {
        public int ServerProjectId { get; set; } 
        public string ConfigKey { get; set; }
        public int ProjectId { get; set; }

        public string ConfigValue { get; set; }

        public int CanDelete { get; set; }

        public string Remark { get; set; }
    }
}
