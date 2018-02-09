using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class Customer
    {
        public Customer() { this.LinkManagers = new List<CustomerLinkManager>(); }
        public int CusId { get; set; }
        public string CustomerName { get; set; }
        public int State { get; set; }
        public string Remark { get; set; }
        public string WebDomains { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public string CusNo { get; set; }
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// 0 未定  1公司 2自动 3复合
        /// </summary>
        public int ServerOfType { get; set; }

        public string ContractNo { get; set; }
        public DateTime? ContractBeginTime { get; set; }
        public DateTime? ContractEndTime { get; set; }

        public string ContractRemark { get; set; }

        public string CustomFunction { get; set; }

        public string ServerRemark { get; set; }

        public string Tag { get; set; }

        public List<CustomerLinkManager> LinkManagers { get; set; }

    }
}
