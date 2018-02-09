using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCF.DB;

namespace ManageDomain.DAL
{
    public class WatchLogDalNew
    {
        public List<Models.WatchLog.TimeWatch> GetListLogs(CCF.DB.DbConn dbconn, DateTime date, int hour, string projectname, int logtype,
            DateTime? begintime, DateTime? endtime, string title, string addition, long? groupid, long? innergroupid, int ordertype, int usetimemin, int usetimemax, int pno, int pagesize, out int totalcount)
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
            if (!string.IsNullOrEmpty(addition))
            {
                wherecon += " and addition=@addition ";
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
            tablename += "_" + hour.ToString("00");
            string sql = string.Format("select * from {0} where 1=1 {1} {2} limit @startindex,@pagesize;", tablename, wherecon, ordercon);
            string countsql = string.Format("select count(1) from {0} where 1=1 {1} {2};", tablename, wherecon, ordercon);
            var para = new
            {
                logtype = logtype,
                projectname = projectname ?? "",
                begintime = begintime,
                endtime = endtime,
                title = title ?? "",
                addition = addition ?? "",
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

        internal bool IsOkTable(DbConn dbconn, string tableName)
        {
            string sql = "SELECT count(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + dbconn.GetBaseConnection().Database + "' " +
                  " and TABLE_NAME=@tablename limit 1;";
            int count = dbconn.ExecuteScalar<int>(sql, new { tablename = tableName });
            return count > 0;
        }
        internal bool IsOkDateAndHour(DbConn dbconn, DateTime date, int hour)
        {
            string sql = "SELECT count(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + dbconn.GetBaseConnection().Database + "' " +
                  " and TABLE_NAME=@tablename limit 1;";
            int count = dbconn.ExecuteScalar<int>(sql, new { tablename = "timewatch" + date.ToString("yyyyMMdd") + "_" + hour.ToString("00") });
            return count > 0;
        }


        public Models.WatchLog.TimeWatch GetDetail(CCF.DB.DbConn dbconn, DateTime date, int hour, int id)
        {
            string sql = string.Format("select * from timewatch{0} where id=@id;", date.ToString("yyyyMMdd_") + hour.ToString("00"));

            string sqlbefore = string.Format("select * from timewatch{0} where id=@id;", new DateTime(date.Year, date.Month, date.Day, hour, 0, 0).AddHours(-1).ToString("yyyyMMdd_HH"));

            var para = new
            {
                id = id
            };
            var data = dbconn.Query<Models.WatchLog.TimeWatch>(sql, para);
            if (data.Count == 0)
            {
                var afdata = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0).AddHours(1);
                if (IsOkDateAndHour(dbconn, afdata, afdata.Hour))
                {
                    string sqlafter = string.Format("select * from timewatch{0} where id=@id;", afdata.ToString("yyyyMMdd_HH"));
                    data = dbconn.Query<Models.WatchLog.TimeWatch>(sqlafter, para);
                }
            }
            if (data.Count == 0)
            {
                var bfdata = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0).AddHours(-1);
                if (IsOkDateAndHour(dbconn, bfdata, bfdata.Hour))
                {
                    string sqlafter = string.Format("select * from timewatch{0} where id=@id;", bfdata.ToString("yyyyMMdd_HH"));
                    data = dbconn.Query<Models.WatchLog.TimeWatch>(sqlafter, para);
                }
            }
            foreach (var a in data)
            {
                a.CreateTime = a.CreateTime.AddMilliseconds(a.CreateTimeMs);
            }
            return data.FirstOrDefault();
        }


        public List<Models.WatchLog.TimeWatch> GetTimeLineList(CCF.DB.DbConn dbconn, DateTime date, int hour, long innergroupid)
        {
            string sql = string.Format("select * from timewatch{0} where innergroupid=@innergroupid limit 500;", date.ToString("yyyyMMdd_") + hour.ToString("00"));
            var para = new
            {
                innergroupid = innergroupid
            };
            var data = dbconn.Query<Models.WatchLog.TimeWatch>(sql, para);
            if (data.Count > 0)
            {
                if (data.FirstOrDefault().CreateTime.Minute < 10)
                {
                    var newdate = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0).AddHours(-1);
                    if (IsOkDateAndHour(dbconn, newdate, newdate.Hour))
                    {
                        sql = string.Format("select * from timewatch{0} where innergroupid=@innergroupid  limit 500;", newdate.ToString("yyyyMMdd_HH"));
                        data.AddRange(dbconn.Query<Models.WatchLog.TimeWatch>(sql, para));
                    }
                }
                if (data.LastOrDefault().CreateTime.Minute > 50)
                {
                    var newdate = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0).AddHours(1);
                    if (IsOkDateAndHour(dbconn, newdate, newdate.Hour))
                    {
                        sql = string.Format("select * from timewatch{0} where innergroupid=@innergroupid  limit 500;", newdate.ToString("yyyyMMdd_HH"));
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

        public List<Models.WatchLog.TimeWatchAna> GetAnaList(DbConn dbconn, string tablename, int pno, int pagesize, int? groupid,int mincount,int maxcount, string dbname, int ordertype1, int ordertype2, out int totalcount)
        {
            Dictionary<string, string> fieldtypedic = new Dictionary<string, string>();
            fieldtypedic["1"] = "_count";
            fieldtypedic["2"] = "_sum";
            fieldtypedic["3"] = "_avg";
            fieldtypedic["4"] = "_max";
            fieldtypedic["5"] = "_min";
            Dictionary<string, string> fieldordertypedic = new Dictionary<string, string>();
            fieldordertypedic["0"] = " asc";
            fieldordertypedic["1"] = " desc";



            string wherecon = "";
            string ordercon = " order by  ";
            string ofiled1 = ordertype1.ToString();
            ordercon += fieldtypedic[ofiled1[0] + ""] + fieldordertypedic[ofiled1[1] + ""];
            if (ordertype2 != ordertype1)
            {
                ordercon += ",";
                string ofiled2 = ordertype2.ToString();
                ordercon += fieldtypedic[ofiled2[0] + ""] + fieldordertypedic[ofiled2[1] + ""];
            }
            if (groupid != null)
            {
                wherecon += " and _groupId=@groupid ";
            }
            if (!string.IsNullOrWhiteSpace(dbname))
            {
                wherecon += " and _dbname=@dbname ";
            }
            if (mincount > 0)
            {
                wherecon += " and _count>=" + mincount + " ";
            }

            if (maxcount > 0)
            {
                wherecon += " and _count<=" + maxcount + " ";
            }

            string sql = string.Format("select * from {0} where 1=1 {1} {2} limit @startindex,@pagesize;", tablename, wherecon, ordercon);
            string countsql = string.Format("select count(1) from {0} where 1=1 {1} {2};", tablename, wherecon, ordercon);
            var para = new
            {
                dbname = dbname,
                groupid = groupid,
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize,
            };
            var data = dbconn.Query<Models.WatchLog.TimeWatchAna>(sql, para);
            totalcount = dbconn.ExecuteScalar<int>(countsql, para);
            return data;
        }

        public void BuildAnaTAble(DbConn dbconn, string tablename)
        {
            string sql = @"
CREATE TABLE `{tablename}` (
  `_groupId` bigint(20) NOT NULL ,
  `_dbname` varchar(255)  ,
  `_count` bigint(21)  ,
  `_sum` double,
  `_avg` double ,
  `_max` double ,
  `_min` double ,
  `egid` int(11) ,
  `egcontent` text ,
  KEY `IX_twana_groupId` (`_groupId`),
  KEY `IX_twana_dbname` (`_dbname`),
  KEY `IX_twana_count` (`_count`),
  KEY `IX_twana_sum` (`_sum`),
  KEY `IX_twana_avg` (`_avg`),
  KEY `IX_twana_max` (`_max`),
  KEY `IX_twana_min` (`_min`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
";
            sql = sql.Replace("{tablename}", tablename);
            dbconn.ExecuteSql(sql);
        }

        public void ReSetAnaData(DbConn dbconn, DateTime date, int hour, string tablename)
        {
            //清空原数据
            dbconn.ExecuteSql("TRUNCATE `" + tablename + "`;");
            string ortablename = "timewatch" + date.ToString("yyyyMMdd_") + hour.ToString("00");
            string sql2 = @"insert into {tablename}(_groupId,_dbname,_count,_sum,_avg,_max,_min,egid,egcontent)
                            select A._groupId,A._dbname,A._count,A._sum,A._avg,A._max,A._min,A.egid,B.content egcontent
                            from
                            (
                            SELECT groupID _groupId,addition _dbname,sum(elapsed) _sum,avg(elapsed) _avg,max(elapsed) _max,min(elapsed) _min,count(1) _count,Min(id) egid 
                            FROM {ortablename} where logType=1
                            group by groupID,addition
                            -- limit 100
                            ) A
                            left join {ortablename} B on A.egid=B.id";
            sql2 = sql2.Replace("{ortablename}", ortablename).Replace("{tablename}", tablename);
            dbconn.ExecuteSql(sql2);
        }
    }
}
