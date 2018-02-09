using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class CustomerLinkManager
    {
        public int CusLinkId { get; set; }
        public int CusId { get; set; }
        public int ManagerId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }

        //extend
        public string ManagerName { get; set; }
    }
}
