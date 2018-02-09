using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class ProjectConfig
    {
        public int ProjectId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Remark { get; set; }
    }
}
