using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCF.DB;

namespace ManageDomain.DAL
{
    public class WatchLogDal
    {
        public List<Models.WatchLog.TimeWatch> GetListLogs(CCF.DB.DbConn dbconn, DateTime date, string projectname, int logtype,
            DateTime? begintime, DateTime? endtime, string title, long? groupid, long? innergroupid, int ordertype, int usetimemin, int usetimemax, int pno, int pagesize, out int totalcount)
        {
            string wherecon = "";
            string ordercon = " order by createtime desc ";
            if (ordertype == 1)
            {
                ordercon = " order by elapsed desc,createtime desc ";
            }
            wherecon += " and logtype=@logtype ";
            if (!string.IsNullOrEmpty(projectname))
            {
                wherecon += " and projectname=@projectname ";
            }

            if (begintime != null)
            {
                wherecon += " and createtime>=@begintime ";
            }
            if (endtime != null)
            {
                wherecon += " and createtime<=@endtime ";
            }
            if (!string.IsNullOrEmpty(title))
            {
                wherecon += " and title like concat('%',@title,'%') ";
            }
            if (groupid != null)
            {
                wherecon += " and groupid=@groupid ";
            }
            if (innergroupid != null)
            {
                wherecon += " and innergroupid=@innergroupid ";
            }
            if (usetimemin > 0)
            {
                wherecon += " and elapsed>=@usetimemin ";
            }

            if (usetimemax > 0)
            {
                wherecon += " and elapsed<=@usetimemax ";
            }
            string tablename = "timewatch" + date.ToString("yyyyMMdd");
            string sql = string.Format("select * from {0} where 1=1 {1} {2} limit @startindex,@pagesize;", tablename, wherecon, ordercon);
            string countsql = string.Format("select count(1) from {0} where 1=1 {1} {2};", tablename, wherecon, ordercon);
            var para = new
            {
                logtype = logtype,
                projectname = projectname ?? "",
                begintime = begintime,
                endtime = endtime,
                title = title ?? "",
                groupid = groupid,
                innergroupid = innergroupid,
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize,
                usetimemin = usetimemin / 1000m,
                usetimemax = usetimemax / 1000m
            };
            var data = dbconn.Query<Models.WatchLog.TimeWatch>(sql, para);
            foreach (var a in data)
            {
                a.CreateTime = a.CreateTime.AddMilliseconds(a.CreateTimeMs);
            }
            totalcount = dbconn.ExecuteScalar<int>(countsql, para);
            return data;
        }
         

        public Tuple<DateTime?, DateTime?> GetTableRange(CCF.DB.DbConn dbconn)
        {
            string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + dbconn.GetBaseConnection().Database + "' " +
                   " and TABLE_NAME like 'timewatch________' order by TABLE_NAME asc limit 1;";
            System.Data.DataTable tb = dbconn.SqlToDataTable(sql, null);
            DateTime? begindate = null;
            if (tb.Rows.Count > 0)
            {
                string t = tb.Rows[0][0].ToString();
                begindate = DateTime.Parse(string.Format("{0}-{1}-{2}", t.Substring(9, 4), t.Substring(13, 2), t.Substring(15, 2)));
            }


            sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + dbconn.GetBaseConnection().Database + "' " +
                   " and TABLE_NAME like 'timewatch________' order by TABLE_NAME desc limit 1;";
            tb = dbconn.SqlToDataTable(sql, null);
            DateTime? enddate = null;
            if (tb.Rows.Count > 0)
            {
                string t = tb.Rows[0][0].ToString();
                enddate = DateTime.Parse(string.Format("{0}-{1}-{2}", t.Substring(9, 4), t.Substring(13, 2), t.Substring(15, 2)));
            }
            return new Tuple<DateTime?, DateTime?>(begindate, enddate);
        }

        public bool IsOkDate(CCF.DB.DbConn dbconn, DateTime date)
        {
            string sql = "SELECT count(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + dbconn.GetBaseConnection().Database + "' " +
                  " and TABLE_NAME=@tablename limit 1;";
            int count = dbconn.ExecuteScalar<int>(sql, new { tablename = "timewatch" + date.ToString("yyyyMMdd") });
            return count > 0;
        }

        public Models.WatchLog.TimeWatch GetDetail(CCF.DB.DbConn dbconn, DateTime date, int id)
        {
            string sql = string.Format("select * from timewatch{0} where id=@id;", date.ToString("yyyyMMdd"));
            var para = new
            {
                id = id
            };
            var data = dbconn.Query<Models.WatchLog.TimeWatch>(sql, para);
            foreach (var a in data)
            {
                a.CreateTime = a.CreateTime.AddMilliseconds(a.CreateTimeMs);
            }
            return data.FirstOrDefault();
        }


        public List<Models.WatchLog.TimeWatch> GetTimeLineList(CCF.DB.DbConn dbconn, DateTime date, long innergroupid)
        {
            string sql = string.Format("select * from timewatch{0} where innergroupid=@innergroupid limit 500;", date.ToString("yyyyMMdd"));
            var para = new
            {
                innergroupid = innergroupid
            };
            var data = dbconn.Query<Models.WatchLog.TimeWatch>(sql, para);
            if (data.Count > 0)
            {
                if (data.FirstOrDefault().CreateTime.Hour < 1 && data.FirstOrDefault().CreateTime.Minute < 10)
                {
                    if (IsOkDate(dbconn, date.AddDays(-1)))
                    {
                        sql = string.Format("select * from timewatch{0} where innergroupid=@innergroupid  limit 500;", date.AddDays(-1).ToString("yyyyMMdd"));
                        data.AddRange(dbconn.Query<Models.WatchLog.TimeWatch>(sql, para));
                    }
                }
                if (data.FirstOrDefault().CreateTime.Hour > 22 && data.FirstOrDefault().CreateTime.Minute > 50)
                {
                    if (IsOkDate(dbconn, date.AddDays(1)))
                    {
                        sql = string.Format("select * from timewatch{0} where innergroupid=@innergroupid  limit 500;", date.AddDays(1).ToString("yyyyMMdd"));
                        data.AddRange(dbconn.Query<Models.WatchLog.TimeWatch>(sql, para));
                    }
                }
            }
            foreach (var a in data)
            {
                a.CreateTime = a.CreateTime.AddMilliseconds(a.CreateTimeMs);
            }
            return data;
        }
    }
}
