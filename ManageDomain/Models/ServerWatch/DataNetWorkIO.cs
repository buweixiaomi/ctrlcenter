using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models.ServerWatch
{
   public class DataNetWorkIO
    {

        public int id { get; set; }
        public int serverid { get; set; }
        public DateTime timestamp { get; set; }

        public string subkey { get; set; }
        public decimal sent { get; set; }
        public decimal received { get; set; }
        public DateTime createtime { get; set; }
    }
}
