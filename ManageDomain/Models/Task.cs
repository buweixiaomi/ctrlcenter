using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class Task
    {

        public int TaskId { get; set; }
        public string CodeName { get; set; }
        public string Title { get; set; }
        public int State { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Remark { get; set; }
        public int SeverState { get; set; }
        public string Memory { get; set; }
        public DateTime? LastTime { get; set; }
        public DateTime? LastHeartTime { get; set; }
        public int ServerID { get; set; }
        public string TaskConfig { get; set; }
        public string ClassFullName { get; set; }
        public string RunCron { get; set; }
        public string Dll { get; set; }
        public int CurrVersionID { get; set; }
        public string ServerName { get; set; }


    }
}
