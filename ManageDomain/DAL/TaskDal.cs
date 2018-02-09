using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class TaskDal
    {
        public List<Models.Task> GetPage(CCF.DB.DbConn dbconn, string keywords, int serverid, int pno, int pagesize, out int totalcount)
        {
            string strwhere = string.Empty;
            if (serverid > 0)
            {
                strwhere = " and t.serverid=@serverid ";
            }
            string sql = string.Format(@"select t.*,s.serverName from task t join servermachine s on t.serverid=s.serverid where state<>-1 {0} and 
(codeName like concat('%',@keywords,'%') or title like concat('%',@keywords,'%') )  limit @startindex,@pagesize", strwhere);
            var models = dbconn.Query<Models.Task>(sql, new { keywords = keywords, serverid = serverid, startindex = pagesize * (pno - 1), pagesize = pagesize }).ToList();
            string countsql = string.Format("select count(1) from task t join servermachine s on t.serverid=s.serverid where state<>-1 {0} and (codeName like concat('%',@keywords,'%') or title like concat('%',@keywords,'%') ); ", strwhere);
            totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords, serverid = serverid });
            return models;
        }

        public List<Models.Task> GetServerTasks(CCF.DB.DbConn dbconn, int serverid)
        {
            string sql = string.Format("select * from task where serverid=@serverid and state<>-1;");
            var models = dbconn.Query<Models.Task>(sql, new { serverid = serverid }).ToList();
            return models;
        }

        public Models.Task GetDetail(CCF.DB.DbConn dbconn, int taskid)
        {
            string sql = "select  * from task where taskid=@taskid ;";
            var model = dbconn.Query<Models.Task>(sql, new { taskid = taskid }).FirstOrDefault();
            return model;
        }
        public Models.Task AddTask(CCF.DB.DbConn dbconn, Models.Task model)
        {
            string sql = @"INSERT INTO `task`(`CodeName`,`Title`,`State`,`createTime`,`remark`,`severState`,`Memory`,`LastTime`,`ServerID`,`taskconfig`,`ClassFullName`,`RunCron`,`Dll`,`CurrVersionID`)
                    VALUES(@codeName,@title,@state,now(),@remark,@severState,@Memory,@LastTime,@ServerID,@taskconfig,@ClassFullName,@RunCron,@Dll,@CurrVersionID);";
            dbconn.ExecuteSql(sql, new
            {
                codeName = model.CodeName ?? "",
                title = model.Title ?? "",
                state = model.State,
                remark = model.Remark ?? "",
                severState = model.SeverState,
                memory = model.Memory ?? "",
                lasttime = model.LastTime,
                serverid = model.ServerID,
                taskconfig = model.TaskConfig,
                ClassFullName = model.ClassFullName,
                RunCron = model.RunCron,
                DLL = model.Dll,
                CurrVersionID = model.CurrVersionID
            });
            int id = dbconn.GetIdentity();
            model.TaskId = id;
            return model;
        }

        public int EditTask(CCF.DB.DbConn dbconn, Models.Task model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE `task`");
            sql.Append("SET  ");
            sql.Append("`CodeName` = @codeName,");
            sql.Append("`Title` = @title,");
            //sql.Append("`State` = @state,");
            sql.Append("`updateTime` =now(),");
            sql.Append("`remark` = @remark,");
            sql.Append("`taskconfig` = @taskconfig,");
            sql.Append("`serverid` =@serverid, ");
            sql.Append("`ClassFullName` =@ClassFullName, ");
            sql.Append("`RunCron` =@RunCron, ");
            sql.Append("`Dll` =@Dll ");
            sql.Append("WHERE `taskId` = @taskId;");
            int r = dbconn.ExecuteSql(sql.ToString(), new
            {
                taskId = model.TaskId,
                codeName = model.CodeName ?? "",
                title = model.Title ?? "",
                //state = model.State,
                remark = model.Remark ?? "",
                taskconfig = model.TaskConfig ?? "",
                serverid = model.ServerID,
                ClassFullName = model.ClassFullName,
                RunCron = model.RunCron,
                Dll = model.Dll
            });
            return r;
        }

        public int DeleteTask(CCF.DB.DbConn dbconn, int taskId)
        {
            string sql = "update task set state=-1 where taskId=@taskId";
            int r = dbconn.ExecuteSql(sql, new { taskId = taskId });
            return r;
        }
        public int UpdateTaskState(CCF.DB.DbConn dbconn, int taskId, int type)
        {
            string sql = "update task set state=@state where taskId=@taskId";
            int r = dbconn.ExecuteSql(sql, new { taskId = taskId, state = type });
            return r;
        }
        public int UpdateTaskVersionID(CCF.DB.DbConn dbconn, int taskId, int currVersionID)
        {
            string sql = "update task set currVersionID=@currVersionID where taskId=@taskId";
            int r = dbconn.ExecuteSql(sql, new { taskId = taskId, currVersionID = currVersionID });
            return r;
        }
        public List<Models.TaskVersion> GetTaskVersions(CCF.DB.DbConn dbconn, int taskid)
        {
            string sql = "select * from taskversion where taskid=@taskid order by createtime desc;";
            return dbconn.Query<Models.TaskVersion>(sql, new { taskid = taskid });
        }


        public Models.TaskVersion GetTaskVersion(CCF.DB.DbConn dbconn, int versionid)
        {
            string sql = "select * from taskversion where versionid=@versionid;";
            return dbconn.Query<Models.TaskVersion>(sql, new { versionid = versionid }).FirstOrDefault();
        }
        public int DeleteTaskVersion(CCF.DB.DbConn dbconn, int VersionID)
        {
            string sql = " delete from  taskversion  where versionId=@versionId";
            int r = dbconn.ExecuteSql(sql, new { versionId = VersionID });
            return r;
        }

        public Models.TaskVersion AddTaskVersion(CCF.DB.DbConn dbconn, Models.TaskVersion model)
        {
            string sql = "insert into taskversion( taskid,versionNo,createtime,versioninfo,downloadurl,remark) values(@taskid,@versionno,now(),@versioninfo,@downloadurl,@remark);";
            dbconn.ExecuteSql(sql, new
            {
                taskid = model.TaskId,
                versionno = model.VersionNo,
                versioninfo = model.VersionInfo ?? "",
                downloadurl = model.DownloadUrl,
                remark = model.Remark ?? ""
            });
            model.VersionId = dbconn.GetIdentity();
            return model;
        }


        public int UpdateTaskVersion(CCF.DB.DbConn dbconn, Models.TaskVersion model)
        {
            string sql = "update taskversion set versionNo=@versionno,versioninfo=@versioninfo,downloadurl=@downloadurl,remark=@remark where versionId=@versionid;";
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

        public int UpdateRunStateInfo(CCF.DB.DbConn dbconn, int taskid, double memory, DateTime? lastupdatetime, int runstate)
        {
            string sql = "update task set severState=@severState,memory=@memory,lasthearttime=now(),lastTime=@lastTime  where taskid=@taskid;";
            int r = dbconn.ExecuteSql(sql, new
            {
                severState = runstate,
                memory = memory.ToString("0.00"),
                lastTime = lastupdatetime,
                taskid = taskid
            });
            return r;
        }

    }
}
