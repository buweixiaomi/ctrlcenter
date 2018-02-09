using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class WorkItemDal
    {
        public Models.WorkItem Add(CCF.DB.DbConn dbconn, Models.WorkItem model)
        {
            string sql = @"insert into workitem(
`title`,
`content`,
`finalDate`,
`createTime`,
`difficulty`,
`estimateTime`,
`state`,
`remark`,
`point`,
`managerId`,
`managerName`,
`importance`,
`tag`) values(
@title,
@content,
@finaldate,
now(),
@difficulty,
@estimatetime,
@state,
@remark,
@point,
@managerid,
@managername,
@importance,
@tag
);";
            dbconn.ExecuteSql(sql, new
            {
                title = model.Title,
                content = model.Content,
                finaldate = model.Finaldate,
                difficulty = model.Difficulty,
                estimatetime = model.EstimateTime,
                state = model.State,
                remark = model.Remark ?? "",
                point = model.Point,
                managerid = model.ManagerId,
                managername = model.ManagerName ?? "",
                importance = model.Importance,
                tag = model.Tag ?? ""
            });
            model.WorkItemId = dbconn.GetIdentity();
            return model;
        }


        public int Update(CCF.DB.DbConn dbconn, Models.WorkItem model)
        {
            string sql = @"UPDATE `workitem`
SET 
`title` = @title,
`content` = @content,
`finalDate` = @finaldate, 
`updateTime` = now(), 
`difficulty` = @difficulty,
`estimateTime` =@estimatetime,  
`remark` = @remark,
`point` = @point,
state=@state,
`importance` = @importance,
`tag` = @tag
WHERE `workitemId` =@workitemid;";

            int r = dbconn.ExecuteSql(sql, new
              {
                  workitemid = model.WorkItemId,
                  title = model.Title,
                  content = model.Content,
                  finaldate = model.Finaldate,
                  difficulty = model.Difficulty,
                  estimatetime = model.EstimateTime,
                  remark = model.Remark ?? "",
                  state = model.State,
                  point = model.Point,
                  importance = model.Importance,
                  tag = model.Tag ?? ""
              });
            return r;
        }

        public List<Models.WorkItem> GetPage(CCF.DB.DbConn dbconn, int? distributeuserid, int? createuserid, int? state, int pno, int pagesize, out int totalcount)
        {
            string sql = @" select {3} from workitem wi join 
                     (
                    select distinct wi.workitemId as workitemId from workitem wi left join
                    workdistribute wd on wi.workitemid=wd.workitemid 
                    {0}
                    ) b on wi.workitemId=b.workitemId where wi.state<>-1 {1} {2};";
            string distributeuseridwhere = "";
            if (distributeuserid != null)
            {
                distributeuseridwhere += " where wd.managerid is not null and wd.managerid=@distributeuserid ";
            }
            string whereconn = "";
            if (createuserid != null)
            {
                whereconn += " and  wi.managerid=@createuserid ";
            }
            if (state != null)
            {
                whereconn += " and wi.state=@state ";
            }
            string querysql = string.Format(sql, distributeuseridwhere, whereconn, "order by wi.createtime desc limit @startindex,@pagesize", "wi.*");
            string countsql = string.Format(sql, distributeuseridwhere, whereconn, "", " count(1) ");
            var para = new
            {
                distributeuserid = distributeuserid ?? 0,
                createuserid = createuserid ?? 0,
                state = state ?? 0,
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            };
            var models = dbconn.Query<Models.WorkItem>(querysql, para);
            foreach (var m in models)
            {
                m.Distributes = GetDestributeDetail(dbconn, m.WorkItemId);
            }
            totalcount = dbconn.ExecuteScalar<int>(countsql, para);
            return models;
        }

        public Models.WorkItem GetDetail(CCF.DB.DbConn dbconn, int workitemid)
        {
            string sql = "select * from workitem where workitemid=@workitemid;";
            var model = dbconn.Query<Models.WorkItem>(sql, new { workitemid = workitemid }).FirstOrDefault();
            if (model != null)
            {
                model.Distributes = GetDestributeDetail(dbconn, workitemid);
            }
            return model;
        }

        public List<Models.WorkDistribute> GetDestributeDetail(CCF.DB.DbConn dbconn, int workitemid)
        {
            string sql = "select wd.*,m.Name as ManagerName from workdistribute wd left join manager m on wd.managerid=m.managerid where wd.workitemid=@workitemid;";
            return dbconn.Query<Models.WorkDistribute>(sql, new { workitemid = workitemid });
        }


        public Models.WorkDistribute AddDistribute(CCF.DB.DbConn dbconn, Models.WorkDistribute model)
        {
            string sql = @"INSERT INTO `workdistribute`
(`workitemId`,
`managerId`,
`state`,
`workRemark`) values(
@workitemid,
@managerid,
0,
@workremark
)";
            dbconn.ExecuteSql(sql, new
            {
                workitemid = model.WorkItemId,
                managerid = model.ManagerId,
                state = 0,
                workremark = "",
            });
            model.WorkDistributeId = dbconn.GetIdentity();
            return model;
        }

        public int MakeWorkitemComplete(CCF.DB.DbConn dbconn, int workitemid, decimal actualtime)
        {
            string sql = "update workitem set state=2 ,commitTime=now(),actualTime=@actualtime where    workitemid=@workitemid and state=1; ";
            int r = dbconn.ExecuteSql(sql, new { workitemid = workitemid, actualtime = actualtime });
            return r;
        }

        public int DeleteDistribute(CCF.DB.DbConn dbconn, int distributeid) //,int workitemid )
        {
            string sql = "delete from workdistribute where WorkDistributeId =@workdistributeid; -- and workitemid=@workitemid; ";
            int r = dbconn.ExecuteSql(sql, new { workdistributeid = distributeid /*workitemid = workitemid,*/  });
            return r;
        }


        public int DeleteDistribute(CCF.DB.DbConn dbconn, int workitemid, int managerid) //,int workitemid )
        {
            string sql = "delete from workdistribute where   workitemid=@workitemid and managerid=@managerid; ";
            int r = dbconn.ExecuteSql(sql, new { workitemid = workitemid, managerid = managerid });
            return r;
        }

        public int MakeDistributeComplete(CCF.DB.DbConn dbconn, int distributeid, decimal actualtime, string remark)
        {
            string sql = "update workdistribute set state=1, actualTime =@actualtime, workRemark=@workremark, commitTime=now() where WorkDistributeId =@workdistributeid;";
            int r = dbconn.ExecuteSql(sql, new { workdistributeid = distributeid, workremark = remark ?? "", actualtime = actualtime });
            return r;
        }

        public int MakeDelete(CCF.DB.DbConn dbconn, int workitemid)
        {
            string sql = "update workitem set state=-1 where workitemid=@workitemid;";
            int r = dbconn.ExecuteSql(sql, new { workitemid = workitemid });
            return r;
        }

        public List<Models.WorkItem> GetManagerDailyWork(CCF.DB.DbConn dbconn, int managerid, DateTime date)
        {
            string sql = @"select wi.* from workdistribute wd join workitem wi on wd.workitemId=wi.workitemId
                            where wd.managerId=@managerid and wd.state=1 and wd.commitTime>=@begintime and wd.commitTime<@endtime";
            DateTime begintime = DateTime.Parse(date.ToString("yyyy-MM-dd"));
            DateTime endtime = begintime.AddDays(1);
            var models = dbconn.Query<Models.WorkItem>(sql, new
            {
                managerid = managerid,
                begintime = begintime,
                endtime = endtime
            });
            return models;
        }


        public List<Entity.Summary> GetSummary(CCF.DB.DbConn dbconn, string begintime, string endtime, int? distributeuserid, int pno, int pagesize, out int totalcount)
        {
            string strwhere = string.Empty;
            if (distributeuserid != null)
            {
                strwhere = "and wd.managerId=@managerid";
            }
            string sql = @"select * from
(
select  ( select name from manager where managerId=wd.managerId) name,
sum(case wd.state when 0 then 0 else 1 end) as waitexec,
sum(case wd.state when 1 then 1 else 0 end) as completexec,
sum(wd.actualTime) alltime  from
 workdistribute wd join workitem wi on wd.workitemId=wi.workitemId
where wi.createtime>@begintime and wi.createtime<= @endtime
" + strwhere + @"
group by wd.managerId
) T limit @startindex,@pagesize;
";
            string countsql = @"select count(*) from
(
select  ( select name from manager where managerId=wd.managerId) name,
sum(case wd.state when 0 then 0 else 1 end) as waitexec,
sum(case wd.state when 1 then 1 else 0 end) as completexec,
sum(wd.actualTime) alltime  from
 workdistribute wd join workitem wi on wd.workitemId=wi.workitemId
where wi.createtime>@begintime and wi.createtime<= @endtime
" + strwhere + @"
group by wd.managerId
) T;
";
            totalcount = dbconn.ExecuteScalar<int>(countsql, new
            {
                begintime = begintime == "" ? DateTime.Now.AddMonths(-1).ToString() : begintime,
                endtime = endtime == "" ? DateTime.Now.AddDays(1).ToString() : endtime,
                managerid = distributeuserid
            });
            return dbconn.Query<Entity.Summary>(sql, new
            {
                managerid = distributeuserid,
                begintime = begintime == "" ? DateTime.Now.AddMonths(-1).ToString() : begintime,
                endtime = endtime == "" ? DateTime.Now.AddDays(1).ToString() : endtime,
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            });

        }
    }
}
