using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class WatchLogBll
    {
        DAL.WatchLogDal dal = new DAL.WatchLogDal();
        public Models.PageModel<Models.WatchLog.TimeWatch> GetListLogs(DateTime date, string projectname, int logtype, DateTime? begintime, DateTime? endtime, string title, long? groupid, long? innergroupid, int ordertype, int pno, int pagesize, int usetimemin = 0, int usetimemax = 0)
        {
            using (var dbconn = Pub.GetWatchLogConn())
            {
                if (!dal.IsOkDate(dbconn, date))
                {
                    throw new MException(MExceptionCode.BusinessError, "请求日期日志不存在！");
                }
                if (begintime != null)
                {
                    begintime = DateTime.Parse(date.ToString("yyyy-MM-dd ") + begintime.Value.ToString("HH:mm:ss"));
                }
                if (endtime != null)
                {
                    endtime = DateTime.Parse(date.ToString("yyyy-MM-dd ") + endtime.Value.ToString("HH:mm:ss"));
                }
                int totalcount = 0;
                var model = dal.GetListLogs(dbconn, date, projectname, logtype, begintime, endtime, title, groupid, innergroupid, ordertype, usetimemin, usetimemax, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.WatchLog.TimeWatch>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Tuple<Models.WatchLog.TimeWatch, List<Models.WatchLog.TimeWatch>> GetDetail(DateTime date, int id)
        {
            using (var dbconn = Pub.GetWatchLogConn())
            {
                if (!dal.IsOkDate(dbconn, date))
                {
                    throw new MException(MExceptionCode.BusinessError, "该日志不存在日志记录！");
                }
                var model = dal.GetDetail(dbconn, date, id);
                if (model == null)
                {
                    throw new MException(MExceptionCode.BusinessError, "日志不存在！");
                }
                List<Models.WatchLog.TimeWatch> sub = dal.GetTimeLineList(dbconn, date, model.InnerGroupID);
                return new Tuple<Models.WatchLog.TimeWatch, List<Models.WatchLog.TimeWatch>>(model, sub);
            }
        }
    }
}
