using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class CmdArgument
    {
        public int CmdId { get; set; }

        public int ArgIndex { get; set; }
        public string ArgValue { get; set; }
        public int ContainConfig { get; set; }
    }
}
