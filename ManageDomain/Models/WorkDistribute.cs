using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class WorkDistribute
    {
        public int WorkDistributeId { get; set; }
        public int WorkItemId { get; set; }
        public int ManagerId { get; set; }
        public int State { get; set; }
        public string WorkRemark { get; set; }
        //实际用时
        public decimal? ActualTime { get; set; }
        public DateTime? CommitTime { get; set; }

        //public Manager Manager { get; set; }

        public string ManagerName { get; set; }
    }
}
