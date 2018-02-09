using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class ZhiBoProbation
    {
       public int ID { get; set; }
       public string Name { get; set; }
       public string Profession { get; set; }
       public int CompanyNum { get; set; }
       public string Mobile { get; set; }
       public string QQ { get; set; }
       public DateTime CreateTime { get; set; }
       public string Remark { get; set; }
    }
}
