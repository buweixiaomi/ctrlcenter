using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class WorkItem
    {
        public int WorkItemId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? Finaldate { get; set; }
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
        public DateTime? CommitTime { get; set; }

        /// <summary>
        /// 1-5
        /// </summary>
        public int Difficulty { get; set; }
        public decimal EstimateTime { get; set; }

        public decimal? ActualTime { get; set; }
        public int State { get; set; }
        public string Remark { get; set; }
        public double Point { get; set; }
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }
        public int? FeedbackId { get; set; }

        /// <summary>
        /// 1-5
        /// </summary>
        public int Importance { get; set; }

        public string Tag { get; set; }
        public List<WorkDistribute> Distributes { get; set; }
    }
}
