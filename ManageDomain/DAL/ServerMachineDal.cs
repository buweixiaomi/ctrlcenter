using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ManageDomain.DAL
{
    public class ServerMachineDal
    {
        public List<Models.ServerMachine> GetServerPage(CCF.DB.DbConn dbconn, string keywords, int pno, int pagesize, out int totalcount)
        {
            string sql = @"select  `serverId`,
    `serverName`,
    `serverIPs`,
    `serverMACs`,
    `clientIds`,
    `serverOS`,
    `lastHeartTime`,
    `createTime`,
    `updateTime`,
    `configUpdateTime`,
    `serverOfType`,
    `serverState`,
       valstarttime,
valendtime,
    `remark` from servermachine  
                                where serverState<>-1 and 
                                ( serverName like concat('%',@keywords,'%')
                                or serverName like concat('%',@keywords,'%') 
                                or serverIPs like concat('%',@keywords,'%') 
                                or serverMACs like concat('%',@keywords,'%') 
                                or clientIds like concat('%',@keywords,'%') 
                                )
                               limit @startindex,@pagesize;";
            var models = dbconn.SqlToModel<Models.ServerMachine>(sql, new { keywords = keywords, startindex = pagesize * (pno - 1), pagesize = pagesize });
            //var models = dbconn.Query<Models.ServerMachine>(sql, new { keywords = keywords, startindex = pagesize * (pno - 1), pagesize = pagesize }, dbconn.InnerTrans).ToList();

            string countsql = @"select count(1) from servermachine  
                                where serverState<>-1 and 
                                 (serverName like concat('%',@keywords,'%')
                                or serverName like concat('%',@keywords,'%') 
                                or serverIPs like concat('%',@keywords,'%') 
                                or serverMACs like concat('%',@keywords,'%') 
                                or clientIds like concat('%',@keywords,'%') 
                                );";
            //  totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords }, dbconn.InnerTrans);

            totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords });
            return models;
        }

        public Models.ServerMachine GetServerDetail(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "select * from serverMachine where serverId=@serverid;";
            var model = dbconn.Query<Models.ServerMachine>(sql, new { serverid = serverid }).FirstOrDefault();
            return model;
        }

        public Models.ServerMachine AddServer(CCF.DB.DbConn dbconn, Models.ServerMachine model)
        {
            string sql = "INSERT INTO `servermachine`(`serverName`,`serverIPs`,`serverMACs`,`clientIds`,`serverOS`,`createTime`,`serverOfType`,`serverState`,`remark`,valstarttime,valendtime) " +
                 "VALUES(@serverName,@serverIPs,@serverMACs,@clientIds,@serverOS,now(),@serverOfType,@serverState,@remark,@valstarttime,@valendtime);";
            dbconn.ExecuteSql(sql, new
            {
                serverName = model.ServerName,
                serverIPs = model.ServerIPs ?? "",
                serverMACs = model.ServerMACs ?? "",
                clientIds = model.ClientIds ?? "",
                serverOS = model.ServerOS,
                serverOfType = model.ServerOfType,
                serverState = model.ServerState,
                remark = model.Remark ?? "",
                valstarttime = model.ValStartTime,
                valendtime = model.ValEndTime
            });
            int id = dbconn.GetIdentity();
            model.ServerId = id;
            return model;
        }

        public int UpdateServer(CCF.DB.DbConn dbconn, Models.ServerMachine model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update servermachine set ")
                .Append(" serverName=@serverName,")
                .Append(" serverIPs=@serverIPs,")
                .Append(" serverMACs=@serverMACs,")
                .Append(" clientIds=@clientIds,")
                .Append(" serverOS=@serverOS,")
                .Append(" updateTime=now(),")
                .Append(" serverOfType=@serverOfType,")
                .Append(" serverState=@serverState,")
                .Append(" valstarttime=@valstarttime,")
                .Append(" valendtime=@valendtime,")
                .Append(" remark=@remark ")
                .Append(" where serverId=@serverId; ");
            int r = dbconn.ExecuteSql(sb.ToString(), new
             {
                 serverId = model.ServerId,
                 serverName = model.ServerName,
                 serverIPs = model.ServerIPs ?? "",
                 serverMACs = model.ServerMACs ?? "",
                 clientIds = model.ClientIds ?? "",
                 serverOS = model.ServerOS,
                 serverOfType = model.ServerOfType,
                 serverState = model.ServerState,
                 remark = model.Remark ?? "",
                 valstarttime = model.ValStartTime,
                 valendtime = model.ValEndTime
             });
            return r;
        }

        public int DeleteServer(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "update servermachine set serverState = -1 where serverId = @serverId ;";
            int r = dbconn.ExecuteSql(sql, new { serverId = serverid });
            return r;
        }


        public int SetProjectConfig(CCF.DB.DbConn dbconn, int serverid, List<Models.ServerConfig> configs)
        {
            dbconn.ExecuteSql("delete from serverconfig where serverid=@serverid", new { serverid = serverid });
            string insertsql = "insert into serverconfig (serverid,configKey,configvalue,remark) values(@serverid,@configkey,@configvalue,@remark );";
            foreach (var a in configs)
            {
                a.ServerId = serverid;
                dbconn.ExecuteSql(insertsql, new
                {
                    serverid = a.ServerId,
                    configkey = a.ConfigKey,
                    configvalue = a.ConfigValue ?? "",
                    remark = a.Remark ?? ""
                });
            }
            return configs.Count;
        }

        public List<Models.ServerConfig> GetConfigs(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "select * from serverconfig where serverId=@serverid;";
            var model = dbconn.Query<Models.ServerConfig>(sql, new { serverid = serverid });
            return model;
        }

        public void UpdateConfigUpdateTime(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "update servermachine set configUpdateTime = now() where serverId = @serverId ;";
            int r = dbconn.ExecuteSql(sql, new { serverId = serverid });
        }


        public List<Models.ServerMachine> GetMinServers(CCF.DB.DbConn dbconn, int topcount)
        {
            string sql = "select * from servermachine where serverState<>-1 order by createtime desc limit " + topcount;
            return dbconn.Query<Models.ServerMachine>(sql);
        }

        public List<Models.ServerMachine> GetByMac(CCF.DB.DbConn dbconn, string mac)
        {
            string sql = "select * from servermachine where serverstate<>-1 and ServerMacs like concat('%',@mac,'%');";
            return dbconn.Query<Models.ServerMachine>(sql, new { mac = mac ?? "" });
        }

        public List<Models.ServerMachine> GetByClient(CCF.DB.DbConn dbconn, string clientid)
        {
            string sql = "select * from servermachine where serverstate<>-1 and clientIds like concat('%',@clientid,'%');";
            return dbconn.Query<Models.ServerMachine>(sql, new { clientid = clientid ?? "" });
        }

        public List<Models.ServerMachine> GetByIp(CCF.DB.DbConn dbconn, string ip)
        {
            string sql = "select * from servermachine where serverstate<>-1 and serverIps like concat('%',@serverIps,'%');";
            return dbconn.Query<Models.ServerMachine>(sql, new { serverIps = ip ?? "" });
        }

        public void MakeHeart(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "update servermachine set lastHeartTime = now() where serverId = @serverId ;";
            int r = dbconn.ExecuteSql(sql, new { serverId = serverid });
        }

        public Tuple<string, string> GetConfig(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "select config,configsign from  servermachine  where serverId = @serverId ;";
            var r = dbconn.SqlToDataTable(sql, new { serverId = serverid });
            if (r.Rows.Count == 0)
                return null;
            return new Tuple<string, string>(r.Rows[0]["configsign"].ToString(), CCF.DB.LibConvert.NullToStr(r.Rows[0]["config"]));
        }

        public void SetConfigSign(CCF.DB.DbConn dbconn, int serverid, string config, string configsign)
        {
            string sql = "update servermachine set config=@config, configsign=@configsign   where serverId = @serverId ;";
            dbconn.ExecuteSql(sql, new { serverId = serverid, config = config ?? "", configsign = configsign ?? "" });
        }


    }
}
