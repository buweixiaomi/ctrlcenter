using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Entity
{
    public class Summary
    {
        public string name { get; set; }
        public decimal alltime { get; set; }
        public int waitexec { get; set; }
        public int completexec { get; set; }
    }
}
