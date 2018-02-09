using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models.ServerWatch
{
    public class ServerStateInfo
    {
        public int ServerId { get; set; }
        public string StateInfo { get; set; }
        public DateTime? UpdateTime { get; set; }

        public Models.ServerMachine Server { get; set; }
    }
}
