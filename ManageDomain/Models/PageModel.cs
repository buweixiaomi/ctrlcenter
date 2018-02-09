using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class PageModel<T>
    {
        public List<T> list { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}
