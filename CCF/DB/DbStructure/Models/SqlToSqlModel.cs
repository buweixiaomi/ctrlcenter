using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public class SqlToSqlModel
    {
        public string typename1 { get; set; }
        public string typename2 { get; set; }
        public FieldCheckType checktype { get; set; }
        public string checksql { get; set; }
    }
}
