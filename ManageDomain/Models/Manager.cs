using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class Manager
    {
        public int ManagerId { get; set; }
        public string Name { get; set; }
        public string SubName { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }

        public int AllowLogin { get; set; }
        public int State { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public string Remark { get; set; }

        //extend
        public List<UserTag> Tags { get; set; }
    }
}
