using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.Config
{
    public class ExecModel
    {
        public string sql { get; set; }
        public int count { get; set; }
        public List<MySql.Data.MySqlClient.MySqlParameter> paras { get; set; }
    }
}
