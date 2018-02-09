using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class WorkDaily
    {
        public int WorkDailyId { get; set; }
        public int ManagerId { get; set; }
        public string Summary { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime WorkTime { get; set; }
        public string Content { get; set; }
        public int State { get; set; }
        public int Score { get; set; }

        public string ManagerName { get; set; }
    }
}
