using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OE.Service
{
    public class ServiceException : Exception
    {
        public int Code { get; private set; }

        public ServiceException(string msg)
            : base(msg)
        {
            Code = -1;
        }
        public ServiceException(int code, string msg)
            : base(msg)
        {
            Code = code;
        }
        public ServiceException(int code, Exception innerex)
            : base(innerex.Message, innerex)
        {
            Code = code;
        }

        public ServiceException(ServiceErrorCode code, string msg)
            : base(msg)
        {
            Code = (int)code;
        }

        public ServiceException(ServiceErrorCode code, Exception innerex)
            : base(innerex.Message, innerex)
        {
            Code = (int)code;
        }

    }
    public enum ServiceErrorCode
    {
        Error = -5000,
        Warning = -4000,
        Infor = -1
    }
}
