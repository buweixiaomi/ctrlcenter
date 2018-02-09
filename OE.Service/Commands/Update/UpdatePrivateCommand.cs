using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Update
{
    public class UpdatePrivateCommand : ICommand
    {
        public override int Execute(string[] args)
        {
            ApiSdk.ConfigApi configapi = new ApiSdk.ConfigApi();
            var result = configapi.GetPrivateConfig();
            if (result.code <= 0)
            {
                Msg = result.msg;
                return -1;
            }
            Configrations.Config.PrivateConfig = result.data;
            Configrations.Config.UnionConfig();
            return 1;
        }
    }
}
