using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.Config
{
    public class MigDataProcessModel
    {
        public string tablename { get; set; }
        public long totalitemcount { get; set; }
        public long successcount { get; set; }
        public double processpercent { get; set; }
        public double remainmins { get; set; }
        public string statusmsg { get; set; }
        public int statuscode { get; set; }
        public long waitcount { get; set; }
        public long rowbuffercount { get; set; }
        public long execbuffercount { get; set; }
    }
}
