using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.Config
{
    public class Database
    {
        public string source { get; set; }
        public string database { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string port { get; set; }
    }
}
