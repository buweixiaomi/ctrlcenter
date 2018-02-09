using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk
{
    public class CommandApi
    {
        public ApiResult<List<CommandInfo>> GetCommands(int currmaxId)
        {
            var result = SdkCore.InvokeApi<List<CommandInfo>>(Configrations.ConfigConst.API_COMMAND_GETNEWS, new { CurrID = currmaxId });
            return result;
        }

        public ApiResult<List<CommandInfo>> CommandResult(int currid, int resultcode, string msg, string exception)
        {
            var result = SdkCore.InvokeApi<List<CommandInfo>>(Configrations.ConfigConst.API_COMMAND_PROCESSRESULT, new { CmdID = currid, ResultCode = resultcode, Msg = msg, Exception = exception });
            return result;
        }


        public ApiResult<List<CommandInfo>> CommandNotify(int currid)
        {
            var result = SdkCore.InvokeApi<List<CommandInfo>>(Configrations.ConfigConst.API_COMMAND_PROCESSNOTIFY, new { CmdID = currid });
            return result;
        }



    }
}
