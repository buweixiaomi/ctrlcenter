using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ManageDomain.DAL
{
    public class ProjectDal
    {
        public List<Models.Project> GetPage(CCF.DB.DbConn dbconn, string keywords, int pno, int pagesize, out int totalcount)
        {
            string sql = string.Format("select * from project where state<>-1 and (codeName like concat('%',@keywords,'%') or title like concat('%',@keywords,'%') )  limit @startindex,@pagesize");
            var models = dbconn.Query<Models.Project>(sql, new { keywords = keywords, startindex = pagesize * (pno - 1), pagesize = pagesize }).ToList();
            string countsql = string.Format("select count(1) from project where state<>-1 and (codeName like concat('%',@keywords,'%') or title like concat('%',@keywords,'%') ); ");
            totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords });
            return models;
        }

        public Models.Project GetDetail(CCF.DB.DbConn dbconn, int projectId)
        {
            string sql = "select  * from project where projectid=@projectid ;";
            var model = dbconn.Query<Models.Project>(sql, new { projectId = projectId }).FirstOrDefault();
            return model;
        }

        public Models.Project AddProject(CCF.DB.DbConn dbconn, Models.Project model)
        {
            string sql = "INSERT INTO `project`(`CodeName`,`Title`,`State`,`createTime`,`remark`)VALUES(@codeName,@title,@state,now(),@remark);";
            dbconn.ExecuteSql(sql, new
            {
                codeName = model.CodeName ?? "",
                title = model.Title ?? "",
                state = model.State,
                remark = model.Remark ?? ""
            });
            int id = dbconn.GetIdentity();
            model.ProjectId = id;
            return model;
        }

        public int EditProject(CCF.DB.DbConn dbconn, Models.Project model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE `project`");
            sql.Append("SET  ");
            sql.Append("`CodeName` = @codeName,");
            sql.Append("`Title` = @title,");
            sql.Append("`State` = @state,");
            sql.Append("`updateTime` =now(),");
            sql.Append("`remark` = @remark ");
            sql.Append("WHERE `projectId` = @projectId;");
            int r = dbconn.ExecuteSql(sql.ToString(), new
            {
                projectId = model.ProjectId,
                codeName = model.CodeName ?? "",
                title = model.Title ?? "",
                state = model.State,
                remark = model.Remark ?? ""
            });
            return r;
        }

        public int DeleteProject(CCF.DB.DbConn dbconn, int projectId)
        {
            string sql = "update project set state=-1 where projectId=@projectId";
            int r = dbconn.ExecuteSql(sql, new { projectId = projectId });
            return r;
        }

        public List<Models.ProjectConfig> GetProjectConfig(CCF.DB.DbConn dbconn, int projectid)
        {
            string sql = "select * from projectconfig where projectId = @projectid;";
            var models = dbconn.Query<Models.ProjectConfig>(sql, new { projectid = projectid });
            return models;
        }

        public int SetProjectConfig(CCF.DB.DbConn dbconn, int projectid, List<Models.ProjectConfig> configs)
        {
            dbconn.ExecuteSql("delete from projectconfig where projectid=@projectid", new { projectid = projectid });
            string insertsql = "insert into projectconfig (projectId,configKey,configValue,remark) values(@projectid,@configkey,@configvalue,@remark );";
            foreach (var a in configs)
            {
                a.ProjectId = projectid;

                dbconn.ExecuteSql(insertsql, new
                {
                    projectid = a.ProjectId,
                    configkey = a.ConfigKey,
                    configvalue = a.ConfigValue ?? "",
                    remark = a.Remark ?? ""
                });
            }
            return configs.Count;
        }

        public int SynConfig(CCF.DB.DbConn dbconn, string configkey)
        {
            Models.ProjectConfig config = dbconn.Query<Models.ProjectConfig>("select * from projectconfig where configkey=@configkey;", new { configkey = configkey }).FirstOrDefault();
            if (config == null)
                return 0;
            var serverprojects = dbconn.Query<Models.ServerProject>("select serverproject from serverproject where projectid=@projectid", new { projectid = config.ProjectId });
            string updatesql = "update serverprojectconfig set canDelete=0  where projectid=@projectid and configkey=@configkey";
            string insertsql = "insert into serverprojectconfig (serverprojectid,configkey,projectid,configvalue,candelete,remark) values(@serverprojectid,@configkey,@projectid,@configvalue,0,@remark);";

            foreach (var a in serverprojects)
            {
                int update_rows = dbconn.ExecuteSql(updatesql, new { projectid = config.ProjectId, configkey = config.ConfigKey, configvalue = config.ConfigValue, remark = config.Remark ?? "" });
                if (update_rows == 0)
                {
                    update_rows = dbconn.ExecuteSql(insertsql, new { serverprojectid = a.ServerProjectId, projectid = config.ProjectId, configkey = config.ConfigKey, configvalue = config.ConfigValue, remark = config.Remark ?? "" });
                }
            }
            return serverprojects.Count;
        }

        public List<Models.Project> GetMinProjects(CCF.DB.DbConn dbconn, int topcount)
        {
            string sql = "select * from project  where state<>-1  order by createtime desc limit " + topcount;
            return dbconn.Query<Models.Project>(sql);
        }

        public List<Models.ProjectVersion> GetProjectVersions(CCF.DB.DbConn dbconn, int projectid)
        {
            string sql = "select * from projectversion where projectid=@projectid;";
            return dbconn.Query<Models.ProjectVersion>(sql, new { projectid = projectid });
        }


        public Models.ProjectVersion GetProjectVersion(CCF.DB.DbConn dbconn, int versionid)
        {
            string sql = "select * from projectversion where versionid=@versionid;";
            return dbconn.Query<Models.ProjectVersion>(sql, new { versionid = versionid }).FirstOrDefault();
        }

        public Models.ProjectVersion AddProjectVersion(CCF.DB.DbConn dbconn, Models.ProjectVersion model)
        {
            string sql = "insert into projectversion( projectid,versionNo,createtime,versioninfo,downloadurl,remark) values(@projectid,@versionno,now(),@versioninfo,@downloadurl,@remark);";
            dbconn.ExecuteSql(sql, new
            {
                projectid = model.ProjectId,
                versionno = model.VersionNo,
                versioninfo = model.VersionInfo ?? "",
                downloadurl = model.DownloadUrl,
                remark = model.Remark ?? ""
            });
            model.VersionId = dbconn.GetIdentity();
            return model;
        }


        public int UpdateProjectVersion(CCF.DB.DbConn dbconn, Models.ProjectVersion model)
        {
            string sql = "update projectversion set versionNo=@versionno,versioninfo=@versioninfo,downloadurl=@downloadurl,remark=@remark where versionId=@versionid;";
            int r = dbconn.ExecuteSql(sql, new
            {
                versionid = model.VersionId,
                versionno = model.VersionNo,
                versioninfo = model.VersionInfo ?? "",
                downloadurl = model.DownloadUrl,
                remark = model.Remark ?? ""
            });
            return r;
        }
    }
}
