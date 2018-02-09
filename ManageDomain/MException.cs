using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain
{
    public class MException : Exception
    {
        public int Code { get; protected set; }
        public MException(string msg)
            : base(msg)
        { }
        public MException(int code, string msg)
            : base(msg)
        {
            Code = code;
        }

        public MException(MExceptionCode code, string msg)
            : base(msg)
        {
            Code = (int)code;
        }
    }

    public enum MExceptionCode
    {
        ServerError = -2000,
        BusinessError = -1000,
        Information = -1,
        NoPermission = -3000,
        NotExist = -1001,
    }

    public class MPermissionException : MException
    {
        public static string GetPermissionDesc(SystemPermissionKey pk)
        {
            var keys = EnumHelper.GetEnumAttr<PermissionKeyAttribute>(pk);
            if (keys == null)
                return "";
            return string.Format("你没有 {0} 权限", keys.Name);
        }
        public MPermissionException(SystemPermissionKey permessionkey) :
            base(MExceptionCode.NoPermission, GetPermissionDesc(permessionkey))
        { }
        public MPermissionException(SystemPermissionKey permessionkey, string msg) :
            base(MExceptionCode.NoPermission, GetPermissionDesc(permessionkey))
        { }
    }
}
