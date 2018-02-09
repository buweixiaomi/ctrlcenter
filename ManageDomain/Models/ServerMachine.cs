using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class ServerMachine
    {
        public int ServerId { get; set; }
        public string ServerName { get; set; }

        public string ServerIPs { get; set; }
        public string ServerMACs { get; set; }
        public string ClientIds { get; set; }

        public string ServerOS { get; set; }

        public DateTime? LastHeartTime { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public DateTime? ConfigUpdateTime { get; set; }

        public int ServerOfType { get; set; }

        public int ServerState { get; set; }

        public DateTime? ValStartTime { get; set; }
        public DateTime? ValEndTime { get; set; }
        public string Remark { get; set; }

        public string Config { get; set; }

    }
}
