using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class FeedbackDal
    {
        public List<Models.Feedback> GetFeedBack(CCF.DB.DbConn dbconn, string keywords, int? search_state, int pno, int pagesize, out int totalcount)
        {
            string whereconn = "";
            if (!string.IsNullOrEmpty(keywords))
            {
                whereconn += " and (  managerName like concat('%',@keywords,'%') or Title  like concat('%',@keywords,'%')  ) ";
            }
            if (search_state != null)
            {
                whereconn += " and  state=@search_state ";
            }
            string sql = string.Format("select * from feedback where  state<>-1 {0} order by feedbackid desc limit @startindex,@pagesize", whereconn);
            var models = dbconn.Query<Models.Feedback>(sql, new { keywords = keywords, search_state = search_state ?? 0, startindex = pagesize * (pno - 1), pagesize = pagesize }).ToList();
            string countsql = string.Format("select count(1) from feedback where  state<>-1 {0} ; ", whereconn);
            totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords, search_state = search_state ?? 0 });
            return models;
        }

        public Models.Feedback Add(CCF.DB.DbConn dbconn, Models.Feedback model)
        {
            string sql = "insert into feedback(title,content,state,createTime,cusId,cusName,managerId,managerName,lastProcessTime,remark,feedbackType,fromSource) " +
                "values(@title,@content,@state,now(),@cusId,@cusName,@managerId,@managerName,now(),@remark,@feedbackType,@fromSource);";
            dbconn.ExecuteSql(sql, new
            {
                feedbackId = model.FeedbackId,
                title = model.Title,
                content = model.Content,
                state = 0,
                // createTime = DateTime.Now,
                cusId = model.cusId,
                cusName = model.CusName,
                managerId = model.ManagerId,
                managerName = model.ManagerName,
                remark = model.Remark ?? "",
                feedbackType = model.FeedbackType,
                fromSource = model.FromSource
            });
            model.FeedbackId = dbconn.GetIdentity();
            return model;
        }

        public int UpdateFeedback(CCF.DB.DbConn dbconn, Models.Feedback model)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("update `feedback` set ");
            sb.Append(" title =@title, ");
            sb.Append(" content =@content, ");
            sb.Append(" cusName =@cusName, ");
            sb.Append(" lastProcessTime =now(), ");
            sb.Append(" remark =@remark, ");
            sb.Append(" feedbackType =@feedbackType ");
            //sb.Append(" fromSource =@fromSource ");
            sb.Append(" where feedbackId=@feedbackId;");

            var f = dbconn.ExecuteSql(sb.ToString(), new
            {
                title = model.Title,
                content = model.Content,
                cusname = model.CusName,
                remark = model.Remark ?? "",
                feedbacktype = model.FeedbackType,
                feedbackid = model.FeedbackId,
            });
            return f;
        }
        public int DeleteFeedback(CCF.DB.DbConn dbconn, int feedbackid)
        {
            string sql = "update feedback set state = -1 where feedbackId = @feedbackId ;";
            int r = dbconn.ExecuteSql(sql, new { feedbackId = feedbackid });
            return r;
        }


        public Models.Feedback GetFeedbackDetail(CCF.DB.DbConn dbconn, int feedbackid)
        {
            string sql = "select * from feedback where feedbackid=@feedbackid;";
            var model = dbconn.Query<Models.Feedback>(sql, new { feedbackid = feedbackid }).FirstOrDefault();
            return model;
        }


        public int WorkitemMakeComplete(CCF.DB.DbConn dbconn, int feedbackid)
        {
            string sql = "update feedback set state = 3 where feedbackId = @feedbackId ;";
            int r = dbconn.ExecuteSql(sql, new { feedbackId = feedbackid });
            return r;
        }


        public int MakeCheck(CCF.DB.DbConn dbconn, int feedbackid, int newstate, int managerid, string managername, int? workitemid, string checkremark)
        {
            string sql = "update feedback set state = @newstate,checkTime=now(),checkManagerId=@managerid,checkManagerName=@managername,workitemid=@workitemid, checkRemark=@checkremark where feedbackId = @feedbackId ;";
            int r = dbconn.ExecuteSql(sql, new { feedbackId = feedbackid, newstate = newstate, managerid = managerid, managername = managername ?? "", checkremark = checkremark ?? "", workitemid = workitemid });
            return r;
        }

        public Models.Feedback GetDetail(CCF.DB.DbConn dbconn, int feedbackid)
        {
            string sql = "select * from feedback where feedbackid=@feedbackid;";
            return dbconn.Query<Models.Feedback>(sql, new { feedbackid = feedbackid }).FirstOrDefault();
        }

    }
}
