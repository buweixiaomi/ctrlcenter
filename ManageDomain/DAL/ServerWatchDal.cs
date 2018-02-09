using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class ServerWatchDal
    {
        public const int ChartMaxItmes = 4000;//10万
        public void AddDataCpu(CCF.DB.DbConn dbconn, Models.ServerWatch.DataCpu model)
        {
            string sql = "INSERT INTO `datacpu`(`serverid`,`timestamp`,`userange`,`createtime`) values(@serverid,@timestamp,@userange,now());";
            dbconn.ExecuteSql(sql, new
            {
                serverid = model.serverid,
                timestamp = model.timestamp,
                userange = model.userange
            });
        }

        public void AddDataDiskSpace(CCF.DB.DbConn dbconn, Models.ServerWatch.DataDiskSpace model)
        {
            string sql = "INSERT INTO `datadiskspace`(`serverid`,`timestamp`,`subkey`,`used`,`available`,`createtime`) values(@serverid,@timestamp,@subkey, @used,@available,now());";
            dbconn.ExecuteSql(sql, new
            {
                serverid = model.serverid,
                timestamp = model.timestamp,
                subkey = model.subkey,
                used = model.used,
                available = model.available
            });
        }


        public void AddDataDiskIO(CCF.DB.DbConn dbconn, Models.ServerWatch.DataDiskIO model)
        {
            string sql = "INSERT INTO `datadiskio`(`serverid`,`timestamp`,`subkey`,`iovalue`,`createtime`) values(@serverid,@timestamp,@subkey,@iovalue,now());";
            dbconn.ExecuteSql(sql, new
            {
                serverid = model.serverid,
                timestamp = model.timestamp,
                subkey = model.subkey,
                iovalue = model.iovalue
            });
        }


        public void AddDataHttpRequest(CCF.DB.DbConn dbconn, Models.ServerWatch.DataHttpRequest model)
        {
            string sql = "INSERT INTO `datahttprequest`(`serverid`,`timestamp`,`requestcount`,`createtime`) values(@serverid,@timestamp,@requestcount,now());";
            dbconn.ExecuteSql(sql, new
            {
                serverid = model.serverid,
                timestamp = model.timestamp,
                requestcount = model.requestcount
            });
        }


        public void AddDataMemory(CCF.DB.DbConn dbconn, Models.ServerWatch.DataMemory model)
        {
            string sql = "INSERT INTO `datamemory`(`serverid`,`timestamp`,`used`,`available`,`createtime`) values(@serverid,@timestamp, @used,@available,now());";
            dbconn.ExecuteSql(sql, new
            {
                serverid = model.serverid,
                timestamp = model.timestamp,
                used = model.used,
                available = model.available
            });
        }

        public void AddDataNewWorkIO(CCF.DB.DbConn dbconn, Models.ServerWatch.DataNetWorkIO model)
        {
            string sql = "INSERT INTO `datanetworkio`(`serverid`,`timestamp`,`subkey`,`sent`,`received`,`createtime`) values(@serverid,@timestamp,@subkey, @sent,@received,now());";
            dbconn.ExecuteSql(sql, new
            {
                serverid = model.serverid,
                timestamp = model.timestamp,
                subkey = model.subkey,
                sent = model.sent,
                received = model.received
            });
        }

        public Models.ServerWatch.ServerStateInfo GetServerSummary(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql1 = "select stateinfo,updatetime from serverstateinfo where serverid=@serverid;";
            var objstamp = dbconn.Query<Models.ServerWatch.ServerStateInfo>(sql1, new { serverid = serverid }).FirstOrDefault();
            return objstamp;
        }


        public void AddServerSummary(CCF.DB.DbConn dbconn, int serverid, string p)
        {
            string sql = "update serverstateinfo set stateinfo=@stateinfo ,updatetime=now() where serverid=@serverid;";
            int r = dbconn.ExecuteSql(sql, new
             {
                 stateinfo = p,
                 serverid = serverid
             });
            if (r == 0)
            {
                string sql2 = "insert into serverstateinfo(serverid,stateinfo,updatetime) values(@serverid,@stateinfo ,now() );";
                dbconn.ExecuteSql(sql2, new
                {
                    stateinfo = p,
                    serverid = serverid
                });
            }
        }

        public List<Models.ServerWatch.DataCpu> GetChartCpu(CCF.DB.DbConn dbconn, int serverid, DateTime begintime, DateTime endtime)
        {
            string sql = "select * from datacpu where serverid=@serverid and timestamp>=@begintime and timestamp<=@endtime order by timestamp desc limit  " + ChartMaxItmes + ";";
            var data = dbconn.Query<Models.ServerWatch.DataCpu>(sql, new
            {
                serverid = serverid,
                begintime = begintime,
                endtime = endtime
            });
            return data;
        }

        public List<Models.ServerWatch.DataDiskSpace> GetChartDiskSpace(CCF.DB.DbConn dbconn, int serverid, DateTime begintime, DateTime endtime)
        {
            string sql = "select * from datadiskspace where serverid=@serverid and timestamp>=@begintime and timestamp<=@endtime order by timestamp desc limit  " + ChartMaxItmes + ";";
            var data = dbconn.Query<Models.ServerWatch.DataDiskSpace>(sql, new
            {
                serverid = serverid,
                begintime = begintime,
                endtime = endtime
            });
            return data;
        }


        public List<Models.ServerWatch.DataDiskIO> GetChartDiskIO(CCF.DB.DbConn dbconn, int serverid, DateTime begintime, DateTime endtime)
        {
            string sql = "select * from datadiskio where serverid=@serverid and timestamp>=@begintime and timestamp<=@endtime order by timestamp desc limit  " + ChartMaxItmes + ";";
            var data = dbconn.Query<Models.ServerWatch.DataDiskIO>(sql, new
            {
                serverid = serverid,
                begintime = begintime,
                endtime = endtime
            });
            return data;
        }


        public List<Models.ServerWatch.DataMemory> GetChartMemory(CCF.DB.DbConn dbconn, int serverid, DateTime begintime, DateTime endtime)
        {
            string sql = "select * from datamemory where serverid=@serverid and timestamp>=@begintime and timestamp<=@endtime order by timestamp desc limit  " + ChartMaxItmes + ";";
            var data = dbconn.Query<Models.ServerWatch.DataMemory>(sql, new
            {
                serverid = serverid,
                begintime = begintime,
                endtime = endtime
            });
            return data;
        }


        public List<Models.ServerWatch.DataNetWorkIO> GetChartNetworkIO(CCF.DB.DbConn dbconn, int serverid, DateTime begintime, DateTime endtime)
        {
            string sql = "select * from datanetworkio where serverid=@serverid and timestamp>=@begintime and timestamp<=@endtime order by timestamp desc limit  " + ChartMaxItmes + ";";
            var data = dbconn.Query<Models.ServerWatch.DataNetWorkIO>(sql, new
            {
                serverid = serverid,
                begintime = begintime,
                endtime = endtime
            });
            return data;
        }


        public List<Models.ServerWatch.DataHttpRequest> GetChartHttpRequest(CCF.DB.DbConn dbconn, int serverid, DateTime begintime, DateTime endtime)
        {
            string sql = "select * from datahttprequest where serverid=@serverid and timestamp>=@begintime and timestamp<=@endtime order by timestamp desc limit  " + ChartMaxItmes + ";";
            var data = dbconn.Query<Models.ServerWatch.DataHttpRequest>(sql, new
            {
                serverid = serverid,
                begintime = begintime,
                endtime = endtime
            });
            return data;
        }
    }
}
