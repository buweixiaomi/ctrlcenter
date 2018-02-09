using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class Command
    {
        public int CmdId { get; set; }
        public string CodeName { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? GetTime { get; set; }
        public DateTime? PreExecuteTime { get; set; }
        public DateTime? CompleteTime { get; set; }

        public int CompleteState { get; set; }

        public string CompleteMessage { get; set; }
        public string CompleteError { get; set; }
        public int ServerId { get; set; }

        public int State { get; set; }

        public string GroupKey { get; set; }
    }
}
