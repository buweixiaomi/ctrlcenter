using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int State { get; set; }
        public DateTime CreateTime { get; set; }
        public int? cusId { get; set; }
        public string CusName { get; set; }
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }
        public int WorkItemId { get; set; }
        public DateTime? LastProcessTime { get; set; }
        public string Remark { get; set; }
        public int CheckManagerId { get; set; }
        public string CheckManagerName { get; set; }
        public string CheckRemark { get; set; }
        public DateTime? CheckTime { get; set; }
        public int FeedbackType { get; set; }
        public int FromSource { get; set; }
    }
}

