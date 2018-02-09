using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.DAL
{
    public class CommandDal
    {
        public Models.Command AddCmd(CCF.DB.DbConn dbconn, Models.Command cmdmodel)
        {
            string sql = @"INSERT INTO `command`
(
`codeName`,
`title`,
`groupKey`,
`createTime`,
`serverId`)
VALUES
(
@codename,
@title,
@groupkey,
now(),
@serverid);";
            dbconn.ExecuteSql(sql, new
            {
                codename = cmdmodel.CodeName,
                title = cmdmodel.Title ?? "",
                groupkey = cmdmodel.GroupKey ?? "",
                serverid = cmdmodel.ServerId
            });
            cmdmodel.CmdId = dbconn.GetIdentity();
            return cmdmodel;
        }

        public Models.CmdArgument AddCmdArg(CCF.DB.DbConn dbconn, Models.CmdArgument cmdarg)
        {
            string sql = "insert into cmdargument(cmdid,argindex,argvalue,containconfig) values(@cmdid,@argindex,@argvalue,@containconfig);";
            dbconn.ExecuteSql(sql, new
            {
                cmdid = cmdarg.CmdId,
                argindex = cmdarg.ArgIndex,
                argvalue = cmdarg.ArgValue ?? "",
                containconfig = cmdarg.ContainConfig
            });
            return cmdarg;
        }

        public List<Tuple<Models.Command, Models.ServerMachine>> GetCommandPage(CCF.DB.DbConn dbconn, string groupid, int serverid, int pno, int pagesize, out int totalcount)
        {
            string wherecon = " ";
            if (!string.IsNullOrEmpty(groupid))
            {
                wherecon += " and groupKey=@groupid ";
            }
            if (serverid > 0)
            {
                wherecon += " and serverid=@serverid ";
            }
            var para = new
            {
                startindex = pagesize * (pno - 1),
                pagesize = pagesize,
                groupid = groupid,
                serverid = serverid,
            };
            string sql = "select * from command where state<>-1 " + wherecon + " order by cmdid desc limit @startindex,@pagesize; ";
            var models = dbconn.Query<Models.Command, Models.ServerMachine>(sql, para).ToList();

            string countsql = "select count(1) from command where state<>-1 " + wherecon + " order by cmdid desc; ";
            totalcount = dbconn.ExecuteScalar<int>(countsql, para);
            return models;
        }

        public Models.Command GetCmd(CCF.DB.DbConn dbconn, int cmdid)
        {
            string sql = "select * from command where cmdid=@cmdid;";
            var model = dbconn.Query<Models.Command>(sql, new { cmdid = cmdid }).FirstOrDefault();
            return model;
        }


        public List<Models.CmdArgument> GetCmdArg(CCF.DB.DbConn dbconn, int cmdid)
        {
            string sql = "select * from cmdargument where cmdid=@cmdid order by argindex asc;";
            var model = dbconn.Query<Models.CmdArgument>(sql, new { cmdid = cmdid });
            return model;
        }

        public int MakeDelete(CCF.DB.DbConn dbconn, int cmdid)
        {
            string sql = "update command set state=-1 where cmdid=@cmdid;";
            var r = dbconn.ExecuteSql(sql, new { cmdid = cmdid });
            return r;
        }

        public int GetServerMaxCmdId(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "select cmdid from command where serverid=@serverid and state<>-1 order by createtime desc limit 1;";
            var model = dbconn.ExecuteScalar(sql, new { serverid = serverid });
            return CCF.DB.LibConvert.ObjToInt(model);
        }

        public List<Models.Command> GetServerNewCmds(CCF.DB.DbConn dbconn, int serverid, int count)
        {
            string sql = "select * from command where serverid=@serverid and state<>-1 and completeState=0 order by createtime asc limit @topcount;";
            var model = dbconn.Query<Models.Command>(sql, new { serverid = serverid, topcount = count });
            return model;
        }


        public int SetCmdGetTime(CCF.DB.DbConn dbconn, int cmdid)
        {
            string sql = "update command set getTime=now()  where cmdid=@cmdid and getTime is null ;";
            var r = dbconn.ExecuteSql(sql, new { cmdid = cmdid });
            return r;
        }

        public int PreExce(CCF.DB.DbConn dbconn, int cmdid)
        {
            string sql = "update command set completeState=1 ,preExecuteTime=now() where cmdid=@cmdid and completeState=0 ;";
            var r = dbconn.ExecuteSql(sql, new { cmdid = cmdid });
            return r;
        }
        public void ResultExce(CCF.DB.DbConn dbconn, int cmdid, int newstate, string msg, string error)
        {
            string sql = "update command set completeState=@newstate ,completeTime=now() ,completeMessage=@msg ,completeError=@errormsg where cmdid=@cmdid;";
            var r = dbconn.ExecuteSql(sql, new
            {
                cmdid = cmdid,
                newstate = newstate,
                msg = msg ?? "",
                errormsg = error ?? ""
            });
        }
    }
}
