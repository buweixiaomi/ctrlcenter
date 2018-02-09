using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManageDomain.Models;

namespace ManageDomain.DAL
{
    public class OperationLogDal
    {
        public Models.OperationLog AddLog(CCF.DB.DbConn dbconn, Models.OperationLog logmodel)
        {
            string sql = @"INSERT INTO OperationLog (OperationContent,OperationName,Createtime,OperationTitle,Module)
                  values (@OperationContent,@OperationName,now(),@OperationTitle,@Module) ";
            dbconn.ExecuteSql(sql, new
            {
                OperationContent = logmodel.OperationContent,
                OperationName = logmodel.OperationName,
                Createtime = logmodel.Createtime,
                OperationTitle = logmodel.OperationTitle,
                Module = logmodel.Module
            });
            return logmodel;
        }

        public List<Models.OperationLog> GetLogPage(CCF.DB.DbConn dbconn, int pno, int pagesize, string keywords, string begintime, string endtime, out int totalcount)
        {
            string sql = @"select * from OperationLog where
                                    (OperationName like concat('%',@keywords,'%')
                                     or OperationTitle like concat('%',@keywords,'%')
                                     or Module like concat('%',@keywords,'%')) 
                                     and Createtime between @begintime and @endtime
                                    order by id desc
                                    limit @startindex,@pagesize;";
            string countsql = @"select count(1) from OperationLog where
                                    (OperationName like concat('%',@keywords,'%')
                                     or OperationTitle like concat('%',@keywords,'%')
                                      or Module like concat('%',@keywords,'%')) 
                                     and Createtime between @begintime and @endtime";
            var models = dbconn.Query<Models.OperationLog>(sql, new
            {
                keywords = keywords ?? "",
                begintime = begintime == "" ? DateTime.Now.AddMonths(-3).ToString() : begintime,
                endtime = endtime == "" ? DateTime.Now.AddDays(1).ToString() : endtime,
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            });
            totalcount = dbconn.ExecuteScalar<int>(countsql, new
            {
                keywords = keywords ?? "",
                begintime = begintime == "" ? DateTime.Now.AddMonths(-3).ToString() : begintime,
                endtime = endtime == "" ? DateTime.Now.AddDays(1).ToString() : endtime
            });
            return models;
        }

        public Models.OperationLog GetLogDetail(CCF.DB.DbConn dbconn, int logid)
        {
            string sql = "select * from OperationLog where id=@id;";
            var model = dbconn.Query<Models.OperationLog>(sql, new { id = logid }).FirstOrDefault();
            return model;
        }
    }
}
