using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk
{
    public class ApiResult<T>
    {
        public int code { get; set; }
        public T data { get; set; }
        public string msg { get; set; }
    }
}
