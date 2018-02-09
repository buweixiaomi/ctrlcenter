using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class WatchLogBllNew
    {
        DAL.WatchLogDalNew dal = new DAL.WatchLogDalNew();
        public Models.PageModel<Models.WatchLog.TimeWatch> GetListLogs(DateTime date, int hour, string projectname, int logtype, DateTime? begintime, DateTime? endtime, string title, string addition, long? groupid, long? innergroupid, int ordertype, int pno, int pagesize, int usetimemin = 0, int usetimemax = 0)
        {
            using (var dbconn = Pub.GetWatchLogConn())
            {
                if (!dal.IsOkDateAndHour(dbconn, date, hour))
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
                var model = dal.GetListLogs(dbconn, date, hour, projectname, logtype, begintime, endtime, title, addition, groupid, innergroupid, ordertype, usetimemin, usetimemax, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.WatchLog.TimeWatch>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Tuple<Models.WatchLog.TimeWatch, List<Models.WatchLog.TimeWatch>> GetDetail(DateTime date, int hour, int id)
        {
            using (var dbconn = Pub.GetWatchLogConn())
            {
                if (!dal.IsOkDateAndHour(dbconn, date, hour))
                {
                    throw new MException(MExceptionCode.BusinessError, "请求日期日志不存在！");
                }
                var model = dal.GetDetail(dbconn, date, hour, id);
                if (model == null)
                {
                    throw new MException(MExceptionCode.BusinessError, "日志不存在！");
                }
                List<Models.WatchLog.TimeWatch> sub = dal.GetTimeLineList(dbconn, date, hour, model.InnerGroupID);
                return new Tuple<Models.WatchLog.TimeWatch, List<Models.WatchLog.TimeWatch>>(model, sub);
            }
        }


        private string BuildAnaTable(DateTime date, int hour)
        {
            return "timewatch" + date.ToString("yyyyMMdd") + "_" + hour.ToString("00") + "_ana";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <param name="groupid"></param>
        /// <param name="ordertype">
        /// 10 总次数增序 11总数倒序 
        /// 20 总用时增序 21用时倒序 
        /// 30 平均用时增序 31平均用时倒序 
        /// 40 最大用时增序 41最大用时倒序 
        /// 50 最小用时增序 51最小用时倒序 
        /// </param>
        /// <returns></returns>
        public Models.PageModel<Models.WatchLog.TimeWatchAna> GetAna(int pno, int pageSize, DateTime date, int hour, int? groupid, int mincount, int maxcount, string dbname, int ordertype1, int ordertype2)
        {
            using (var dbconn = Pub.GetWatchLogConn())
            {
                string tablename = BuildAnaTable(date, hour);

                Models.PageModel<Models.WatchLog.TimeWatchAna> models = new Models.PageModel<Models.WatchLog.TimeWatchAna>()
                {
                    PageNo = pno,
                    PageSize = 1,
                    list = new List<Models.WatchLog.TimeWatchAna>(),
                    TotalCount = 0
                };
                if (!dal.IsOkTable(dbconn, tablename))
                {
                    return models;
                }
                int totalcount = 0;
                var ormodels = dal.GetAnaList(dbconn, tablename, pno, pageSize, groupid, mincount, maxcount, dbname, ordertype1, ordertype2, out totalcount);
                models.list = ormodels;
                models.PageNo = pno;
                models.PageSize = pageSize;
                models.TotalCount = totalcount;
                return models;
            }
        }

        public void BuildAnaData(DateTime date, int hour)
        {
            using (var dbconn = Pub.GetWatchLogConn())
            {
                if (!dal.IsOkDateAndHour(dbconn, date, hour))
                {
                    throw new MException(MExceptionCode.BusinessError, "请求日期日志不存在！");
                }
                string tablename = BuildAnaTable(date, hour);
                if (!dal.IsOkTable(dbconn, tablename))
                {
                    dal.BuildAnaTAble(dbconn, tablename);
                }
                dal.ReSetAnaData(dbconn, date, hour, tablename);
            }
        }
    }
}
