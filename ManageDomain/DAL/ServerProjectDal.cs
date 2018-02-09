using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCF.DB;
using System.Text.RegularExpressions;

namespace ManageDomain.DAL
{
    public class ServerProjectDal
    {
        public List<string> GetAllTag(DbConn dbconn)
        {
            string sql = "select tag from serverproject where state<>-1 and tag<>'';";
            List<string> result = dbconn.Query<string>(sql);
            List<string> tags = new List<string>();
            Regex reg = new Regex(@"\[.{1,*}\]");
            foreach (var a in result)
            {
                foreach (var tag in ManageDomain.Pub.SplitTags(a))
                {
                    if (tags.Contains(tag))
                        continue;
                    tags.Add(tag);
                }
            }
            return tags;
        }

        public List<Models.ServerProject> GetServerProjects(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = "select * from serverproject where serverid=@serverid and state<>-1";
            return dbconn.Query<Models.ServerProject>(sql, new { serverid = serverid });
        }

        public List<Models.ServerProject> GetServerProjectsByProjectid(CCF.DB.DbConn dbconn, int projectid)
        {
            string sql = "select * from serverproject where projectid=@projectid and state<>-1";
            return dbconn.Query<Models.ServerProject>(sql, new { projectid = projectid });
        }
        public List<Tuple<Models.ServerProject, Models.Project, Models.ServerMachine>> SearchPage(DbConn dbconn, string tag, string serverinfo, string projectinfo,
            int pno, int pagesize, out int totalcount)
        {
            #region sql
            string sql = @"select 
    cp.`serverProjectId`,
    cp.`projectId`,
    cp.`serverId`,
    cp.`state`,
    cp.`copyRightConfig`,
    cp.`tag`,
    cp.`serverVersion`,
    cp.`lastUpdateTime`,
    cp.`remark`,
    cp.`functionRemark`,
    
    -- p.`projectId`,
    p.`CodeName`,
    p.`Title`,
    p.`State`,
    p.`createTime`,
    p.`updateTime`,
    p.`remark`,
    
   -- sm.`serverId`,
    sm.`serverName`,
    sm.`serverIPs`,
    sm.`serverMACs`,
    sm.`clientIds`,
    sm.`serverOS`,
    sm.`lastHeartTime`,
    sm.`createTime`,
    sm.`updateTime`,
    sm.`configUpdateTime`,
    sm.`serverOfType`,
    sm.`serverState`,
    sm.`remark`
 from serverproject cp 
left join project p on cp.projectId=p.projectid
left join servermachine sm on cp.serverId=sm.serverId

where 
cp.tag like concat('%',@tag,'%') 
and ( p.title like concat('%',@projectinfo,'%') or p.CodeName like concat('%',@projectinfo,'%') )
and  ( sm.serverName like concat('%',@serverinfo,'%') or sm.serverIPs like concat('%',@serverinfo,'%') 
       or sm.serverMACs like concat('%',@serverinfo,'%') or sm.clientIds like concat('%',@serverinfo,'%'))

order by cp.createTime desc limit @startindex,@pagesize;";
            string countsql = @"select     count(1)  from serverproject cp 
                                        left join project p on cp.projectId=p.projectid
                                        left join servermachine sm on cp.serverId=sm.serverId
                                        where 
                                        cp.tag like concat('%',@tag,'%') 
                                        and ( p.title like concat('%',@projectinfo,'%') or p.CodeName like concat('%',@projectinfo,'%') )
                                        and  ( sm.serverName like concat('%',@serverinfo,'%') or sm.serverIPs like concat('%',@serverinfo,'%') 
                                               or sm.serverMACs like concat('%',@serverinfo,'%') or sm.clientIds like concat('%',@serverinfo,'%'));";
            #endregion
            var para = new
            {
                tag = tag ?? "",
                serverinfo = serverinfo ?? "",
                projectinfo = projectinfo ?? "",
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            };
            var rmodel = dbconn.Query<Models.ServerProject, Models.Project, Models.ServerMachine>(sql, para);
            totalcount = dbconn.ExecuteScalar<int>(countsql, para);
            return rmodel;
        }


        public Tuple<Models.ServerProject, Models.Project, Models.ServerMachine, List<Models.ServerProjectConfig>> GetDetailWidth(DbConn dbconn, int serverprojectid)
        {
            var m1 = GetDetail(dbconn, serverprojectid);
            if (m1 == null)
                return null;
            string sql3 = "select * from project where projectid = @projectid;";
            string sql4 = "select * from servermachine where serverid = @serverid;";

            var m3 = dbconn.Query<Models.Project>(sql3, new { projectid = m1.ProjectId }).FirstOrDefault();
            var m4 = dbconn.Query<Models.ServerMachine>(sql4, new { serverid = m1.ServerId }).FirstOrDefault();
            return new Tuple<Models.ServerProject, Models.Project, Models.ServerMachine, List<Models.ServerProjectConfig>>(m1, m3, m4, GetConfigs(dbconn, serverprojectid));
        }

        public int DeleteServerProject(DbConn dbconn, int serverprojectid)
        {
            string sql = "update serverproject set state=-1  where serverprojectid=@serverprojectid;";
            int r = dbconn.ExecuteSql(sql, new { serverprojectid = serverprojectid });
            return r;
        }

        public Models.ServerProject GetDetail(DbConn dbconn, int serverprojectid)
        {
            string sql = "select * from serverproject where serverprojectid=@serverprojectid;";
            var model = dbconn.Query<Models.ServerProject>(sql, new { serverprojectid = serverprojectid }).FirstOrDefault();
            return model;
        }

        public Models.ServerProject Add(DbConn dbconn, Models.ServerProject model)
        {
            string sql = "insert into serverproject(title,projectid,serverid,state,copyrightconfig,tag,serverversion,remark,functionremark,createtime)" +
                "values (@title,@projectid,@serverid,@state,@copyrightconfig,@tag,'',@remark,@functionremark,now())";
            var para = new
            {
                title = model.Title ?? "",
                projectid = model.ProjectId,
                serverid = model.ServerId,
                state = model.State,
                copyrightconfig = model.CopyRightConfig ?? "",
                tag = model.Tag ?? "",
                remark = model.Remark ?? "",
                functionremark = model.FunctionRemark ?? ""
            };
            dbconn.ExecuteSql(sql, para);
            int id = dbconn.GetIdentity();
            model.ServerProjectId = id;
            return model;
        }

        public int Update(DbConn dbconn, Models.ServerProject model)
        {
            string sql = "update serverproject set serverid=@serverid,title=@title,state=@state,copyrightconfig=@copyrightconfig,tag=@tag," +
                "remark=@remark,functionremark=@functionremark,updatetime = now() where serverprojectid=@serverprojectid;";
            var para = new
            {
                serverprojectid = model.ServerProjectId,
                title = model.Title ?? "",
                serverid = model.ServerId,
                state = model.State,
                copyrightconfig = model.CopyRightConfig ?? "",
                tag = model.Tag ?? "",
                remark = model.Remark ?? "",
                functionremark = model.FunctionRemark ?? ""
            };
            int r = dbconn.ExecuteSql(sql, para);
            return r;
        }

        public int UpdateServerVersion(DbConn dbconn, int serverprojectid, string serverversion)
        {
            string sql = "update serverproject set serverversion=@serverversion,lastupdatetime=now() where serverprojectid=@serverprojectid;";
            var para = new
            {
                serverprojectid = serverprojectid,
                serverversion = serverversion ?? ""
            };
            int r = dbconn.ExecuteSql(sql, para);
            return r;
        }

        public List<Models.ServerProjectConfig> GetConfigs(DbConn dbconn, int serverprojectid)
        {
            string sql = "select * from serverprojectconfig where serverprojectid=@serverprojectid order by candelete asc;";
            var models = dbconn.Query<Models.ServerProjectConfig>(sql, new { serverprojectid = serverprojectid });
            return models;
        }

        public int SetConfigs(DbConn dbconn, int serverprojectid, int projectid, List<Models.ServerProjectConfig> configs)
        {
            foreach (var a in GetConfigs(dbconn, serverprojectid).Where(x => x.CanDelete == 0))
            {
                configs.Where(x => x.ConfigKey == a.ConfigKey).Where(x => { x.CanDelete = 0; return false; });
            }
            string delsql = "delete from serverprojectconfig where serverprojectid=@serverprojectid;";
            string insertsql = "insert into serverprojectconfig( serverprojectid, configkey,  projectid, configvalue, candelete, remark) values( @serverprojectid,@configkey,@projectid,@configvalue,@candelete,@remark )";
            dbconn.ExecuteSql(delsql, new { serverprojectid = serverprojectid });
            foreach (var config in configs)
            {
                dbconn.ExecuteSql(insertsql, new
                {
                    serverprojectid = serverprojectid,
                    configkey = config.ConfigKey,
                    projectid = projectid,
                    configvalue = config.ConfigValue ?? "",
                    candelete = config.CanDelete,
                    remark = config.Remark ?? ""

                });
            }
            return configs.Count;
        }

        public int GetServerProjectCount(DbConn dbconn, int serverid)
        {
            string sql = "select count(1) from serverproject where serverid=@serverid and state<>-1;";
            return dbconn.ExecuteScalar<int>(sql, new { serverid = serverid });
        }
    }
}
