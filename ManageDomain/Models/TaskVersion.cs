using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Models
{
    public class TaskVersion
    {
        public int VersionId { get; set; }
        public int TaskId { get; set; }
        public string VersionNo { get; set; }

        public DateTime CreateTime { get; set; }

        public string VersionInfo { get; set; }
        public string DownloadUrl { get; set; }

        public string Remark { get; set; }
    }
}
