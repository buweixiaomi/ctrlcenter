using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Areas.Api.Controllers
{
    [ClientAuth(ClientAuthType.Auth)]
    public class CommandController : ApiBaseController
    {
        //
        // GET: /Api/Command/

        public ActionResult getnews(int CurrId = 0)
        {
            var obj = new List<object>();
            var cmdbll = new ManageDomain.BLL.CommandBll();
            var models = cmdbll.GetServerNowCmd(ServerId, 10);
            foreach (var a in models)
            {
                var arg = new List<object>();
                foreach (var b in a.Item2)
                {
                    arg.Add(new
                    {
                        ContainConfig = b.ContainConfig == 1,
                        OriValue = b.ArgValue
                    });
                }
                obj.Add(new
                {
                    ID = a.Item1.CmdId,
                    Args = arg,
                    Name = a.Item1.CodeName
                });
            }
            return ApiResult(obj);
        }

        public ActionResult processresult(int cmdid, int resultCode, string msg, string exception)
        {
            var cmdbll = new ManageDomain.BLL.CommandBll();
            cmdbll.ResultExec(cmdid, resultCode, msg, exception);
            return JsonE("");
        }

        public ActionResult processnotify(int cmdid)
        {
            var cmdbll = new ManageDomain.BLL.CommandBll();
            cmdbll.PreExec(cmdid);
            return JsonE("");
        }
    }
}
