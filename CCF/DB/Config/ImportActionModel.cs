using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.Config
{
    public class ImportActionModel
    {
        public Database database1 { get; set; }
        public Database database2 { get; set; }
        public int importthreadcount { get; set; }
        public bool isrunning { get; set; }
        public int insertpagesize { get; set; }
        public int getpagesize { get; set; }
        public List<string> excepttables { get; set; }
        public object lockobjofcus = new object();
       // public object lockobjofexc = new object();
        public List<CustomImportActionModel> cutomsactions { get; set; }
    }

    public class ImportActionMiniModel
    {
        public string database1 { get; set; }
        public string database2 { get; set; }
        public int importthreadcount { get; set; }
        public int insertpagesize { get; set; }
        public int getpagesize { get; set; }
        public List<string> excepttables { get; set; }
        public List<CustomImportActionModel> cutomsactions { get; set; }
    }

    public class CustomImportActionModel
    {
        public string tablename { get; set; }
        public int insertpagesize { get; set; }
        public int getpagesize { get; set; }
    }
}
