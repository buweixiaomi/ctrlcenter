using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class UserTag
    {
        public int UserTagId { get; set; }
        public string Tag { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remark { get; set; }
    }
}
