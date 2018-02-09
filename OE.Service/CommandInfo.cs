using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service
{
    public class CommandInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<CommandArgument> Args { get; set; }

        public class CommandArgument
        {
            public bool ContainConfig { get; set; }
            public string OriValue { get; set; }

        }

    }


}
