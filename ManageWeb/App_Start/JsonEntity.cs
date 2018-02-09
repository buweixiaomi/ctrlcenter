using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManageWeb
{
    public class JsonEntity
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}