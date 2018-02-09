using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models.WatchLog
{
    public class TimeWatchAna
    {
        public int _groupId { get; set; }
        public string _dbname { get; set; }
        public decimal _sum { get; set; }
        public decimal _avg { get; set; }
        public decimal _max { get; set; }
        public decimal _min { get; set; }
        public int _count { get; set; }
        public int egId { get; set; }
        public string egContent { get; set; }
    }
}
