using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.DAL
{
    public class CusServiceDal
    {
        public Models.CusService GetDetail(CCF.DB.DbConn dbconn, int cusServiceId)
        {
            string sql = "select * from cusService where cusServiceId=@cusserviceid";
            var model = dbconn.Query<Models.CusService>(sql, new { cusserviceid = cusServiceId }).FirstOrDefault();
            return model;
        }

        public Models.CusService Add(CCF.DB.DbConn dbconn, Models.CusService model)
        {
            string sql = "insert into cusService(cusid,title,servicetype,servicedesc,servicetime,serviceMan,workitemid,servicecharge,state,createmanagerid,createmanagername,createtime,remark) " +
                "values(@cusid,@title,@servicetype,@servicedesc,@servicetime,@serviceMan,@workitemid,@servicecharge,@state,@createmanagerid,@createmanagername,now(),@remark);";
            dbconn.ExecuteSql(sql, new
            {
                cusid = model.CusId,
                title = model.Title,
                servicetype = model.ServiceType,
                servicedesc = model.ServiceDesc ?? "",
                servicetime = model.ServiceTime,
                serviceMan = model.ServiceMan ?? "",
                workitemid = model.WorkItemId,
                servicecharge = model.ServiceCharge,
                state = model.State,
                createmanagerid = model.CreateManagerId,
                createmanagername = model.CreateManagerName ?? "",
                remark = model.Remark ?? ""
            });
            model.CusServiceId = dbconn.GetIdentity();
            return model;
        }

        public List<Models.CusService> GetPage(CCF.DB.DbConn dbconn, int cusid, string keywords, int pno, int pagesize, out int totalcount)
        {
            string where = " title like concat('%',@keywords,'%') ";
            if (cusid > 0)
                where += " and cusid=@cusid";
            string sql = "select * from cusservice where " + where + " limit @startindex,@pagesize;";
            string countsql = "select count(1) from cusservice where " + where + " ;";
            List<Models.CusService> models = dbconn.Query<Models.CusService>(sql, new
            {
                cusid = cusid,
                keywords = keywords ?? "",
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            });
            totalcount = dbconn.ExecuteScalar<int>(countsql, new
            {
                cusid = cusid,
                keywords = keywords ?? ""
            });
            return models;
        }
    }
}
