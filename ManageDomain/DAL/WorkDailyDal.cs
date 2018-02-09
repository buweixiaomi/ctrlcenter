using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class WorkDailyDal
    {

        public Models.WorkDaily Add(CCF.DB.DbConn dbconn, Models.WorkDaily model)
        {
            string sql = "insert into workdaily(managerid,summary,createtime,worktime,content,state,score) values(@managerid,@summary,now(),@worktime,@content,0,@score);";
            dbconn.ExecuteSql(sql, new
            {
                managerid = model.ManagerId,
                summary = model.Summary ?? "",
                worktime = model.WorkTime,
                content = model.Content ?? "",
                state = 0,
                score = model.Score
            });
            model.WorkDailyId = dbconn.GetIdentity();
            return model;
        }
        public int Update(CCF.DB.DbConn dbconn, Models.WorkDaily model)
        {
            string sql = "update workdaily  set summary=@summary, worktime=@worktime,content=@content,score=@score where workdailyid=@workdailyid;";
            int r = dbconn.ExecuteSql(sql, new
            {
                workdailyid = model.WorkDailyId,
                summary = model.Summary ?? "",
                worktime = model.WorkTime,
                content = model.Content ?? "",
                score = model.Score
            });
            return r;
        }

        public Models.WorkDaily GetDetail(CCF.DB.DbConn dbconn, int workdailyid)
        {
            string sql = "select w.*,m.Name as managername from workdaily w left join manager m on w.managerid=m.managerid where workdailyid=@workdailyid;";
            return dbconn.Query<Models.WorkDaily>(sql, new { workdailyid = workdailyid }).FirstOrDefault();
        }

        public Models.WorkDaily GetDetailByDay(CCF.DB.DbConn dbconn, int managerid, DateTime date)
        {
            string sql = "select * from workdaily where managerid=@managerid and worktime=@worktime and state<>-1 limit 2;";
            return dbconn.Query<Models.WorkDaily>(sql, new { managerid = managerid, worktime = date.ToString("yyyy-MM-dd") }).FirstOrDefault();
        }


        public List<Models.WorkDaily> GetPage(CCF.DB.DbConn dbconn, int? currmanagerid, int? managerid, DateTime? begintime, DateTime? endtime, int pno, int pagesize, out int totalcount)
        {
            string sql = "select w.*,m.Name as managername from workdaily w left join manager m on w.managerid=m.managerid where w.state<>-1 ";

            string whereconn = "";
            if (currmanagerid != null)
            {
                whereconn += " and w.managerid=@currmanagerid ";
            }
            if (managerid != null)
            {
                whereconn += " and w.managerid=@managerid ";
            }
            if (begintime != null)
            {
                whereconn += " and w.workTime>=@begintime ";
            }
            if (endtime != null)
            {
                whereconn += " and w.workTime<=@endtime ";
            }

            sql += whereconn + " order by workTime desc limit @startindex,@pagesize;";
            var para = new
            {
                currmanagerid = currmanagerid,
                managerid = managerid,
                begintime = begintime,
                endtime = endtime,
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            };
            string countsql = "select count(1) from workdaily w where w.state<>-1  " + whereconn;
            totalcount = dbconn.ExecuteScalar<int>(countsql, para);
            return dbconn.Query<Models.WorkDaily>(sql, para);
        }

        public List<Models.WorkDaily> GetUserRangeDaily(CCF.DB.DbConn dbconn, int managerid, DateTime begintime, DateTime endtime)
        {
            string sql = "select wd.*,m.Name as ManagerName from workdaily wd left join manager m on wd.managerid=m.managerid "+
                " where wd.state<>-1 and wd.managerid=@managerid and wd.worktime>=@begintime and wd.worktime<=@endtime";
            return dbconn.Query<Models.WorkDaily>(sql, new { managerid = managerid, begintime = begintime, endtime = endtime });
        }

        public int Delete(CCF.DB.DbConn dbconn, int workdailyid)
        {
            string sql = "update workdaily  set  state=-1 where workdailyid=@workdailyid;";
            int r = dbconn.ExecuteSql(sql, new
            {
                workdailyid = workdailyid 
            });
            return r;
        }
    }
}
