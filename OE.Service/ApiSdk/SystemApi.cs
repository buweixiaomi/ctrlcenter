using OE.Service.ApiSdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk
{
    public class SystemApi
    {
        public ApiResult<PingResult> Ping()
        {
            return SdkCore.InvokeApi<PingResult>(Configrations.ConfigConst.API_SYSTEM_PING, null);
        }
    }
}
