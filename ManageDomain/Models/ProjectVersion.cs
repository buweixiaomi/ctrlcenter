using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.Models
{
    public class ProjectVersion
    {
        public int VersionId { get; set; }
        public int ProjectId { get; set; }
        public string VersionNo { get; set; }

        public DateTime CreateTime { get; set; }

        public string VersionInfo { get; set; }
        public string DownloadUrl { get; set; }

        public string Remark { get; set; }
    }
}
