using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models.ServerWatch
{
    public class DataDiskIO
    {
        public int id { get; set; }
        public int serverid { get; set; }
        public DateTime timestamp { get; set; }

        public string subkey { get; set; }
        public decimal iovalue { get; set; }
        public DateTime createtime { get; set; }
    }
}
