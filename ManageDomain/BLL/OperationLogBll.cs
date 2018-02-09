using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class OperationLogBll
    {
        DAL.OperationLogDal dal = new DAL.OperationLogDal();
        public Models.OperationLog AddLog(Models.OperationLog logmodel)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    logmodel = dal.AddLog(dbconn, logmodel);
                    dbconn.Commit();
                    return logmodel;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }
        public Models.PageModel<Models.OperationLog> GetLogPage(int pno, int pagesize, string keywords, string begintime, string endtime)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = dal.GetLogPage(dbconn, pno, pagesize, keywords, begintime, endtime, out totalcount);
                return new Models.PageModel<Models.OperationLog>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }
        public Models.OperationLog GetLogDetail(int logid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = dal.GetLogDetail(dbconn, logid);
                if (model == null)
                    throw new MException(MExceptionCode.NotExist, "用户不存在！");     
                return model;
            }
        }


    }
}
