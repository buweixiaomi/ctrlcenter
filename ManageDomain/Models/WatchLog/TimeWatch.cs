using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models.WatchLog
{
    public class TimeWatch
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public long GroupID { get; set; }
        public long InnerGroupID { get; set; }
        public int LogType { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string Addition { get; set; }
        public DateTime CreateTime { get; set; }

        public DateTime DbCreateTime { get; set; }

        public int CreateTimeMs { get; set; }

        public double Elapsed { get; set; }
    }
}
