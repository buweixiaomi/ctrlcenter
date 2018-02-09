using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class CusService
    {
        public int CusServiceId { get; set; }
        public int CusId { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// 服务类型 0:其它  1Bug修改 2功能调整  3新需求
        /// </summary>
        public int ServiceType { get; set; }
        public string ServiceDesc { get; set; }
        public DateTime? ServiceTime { get; set; }
        public string ServiceMan { get; set; }
        public int WorkItemId { get; set; }
        public decimal? ServiceCharge { get; set; }
        public int State { get; set; }
        public int CreateManagerId { get; set; }
        public string CreateManagerName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remark { get; set; }

        public Customer Customer { get; set; }
    }
}
