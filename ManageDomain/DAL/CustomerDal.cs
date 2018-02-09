using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
namespace ManageDomain.DAL
{
    public class CustomerDal
    {
        public List<Models.Customer> GetCustomerPage(CCF.DB.DbConn dbconn, string keywords, int pno, int pagesize, out int totalcount)
        {
            string sql = string.Format("select * from customer where state<>-1 and customerName like concat('%',@keywords,'%') order by createtime desc limit @startindex,@pagesize");
            var models = dbconn.Query<Models.Customer>(sql, new { keywords = keywords, startindex = pagesize * (pno - 1), pagesize = pagesize }).ToList();
            string countsql = string.Format("select count(1) from customer where state<>-1 and  customerName like concat('%',@keywords,'%'); ");
            totalcount = dbconn.ExecuteScalar<int>(countsql, new { keywords = keywords });
            return models;
        }

        public Models.Customer GetDetail(CCF.DB.DbConn dbconn, int cusid)
        {
            string sql = "select * from customer where cusid=@cusid;";
            var model = dbconn.Query<Models.Customer>(sql, new { cusid = cusid }).FirstOrDefault();
            return model;
        }


        public Models.Customer GetDetailByCusNo(CCF.DB.DbConn dbconn, string cusno)
        {
            string sql = "select * from customer where cusno=@cusno;";
            var model = dbconn.Query<Models.Customer>(sql, new { cusno = cusno });
            if (model.Count > 0)
                return null;
            return model.FirstOrDefault();
        }



        public List<Models.Customer> GetMinCustomers(CCF.DB.DbConn dbconn, int topcount)
        {
            string sql = "select * from customer where state<>-1 order by createtime desc limit " + topcount;
            return dbconn.Query<Models.Customer>(sql);
        }

        public Models.Customer AddCus(CCF.DB.DbConn dbconn, Models.Customer model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO `customer`");
            sb.Append("(`customerName`,`state`,`remark`,`webDomains`,`createTime`,`cusNo`,`submitTime`,`serverOfType`,");
            sb.Append("`contractNo`,`contractRemark`,`contractBeginTime`,`contractEndTime`,`serverRemark`,`customFunction`,`tag`)");
            sb.Append("VALUES");
            sb.Append("(@customername,@state,@remark,@webdomains,now(),@cusno,@submittime,@serveroftype,");
            sb.Append("@contractno,@contractremark,@contractbegintime,@contractendtime,@serverremark,@customfunction,@tag);");

            dbconn.ExecuteSql(sb.ToString(), new
            {
                customername = model.CustomerName ?? "",
                state = model.State,
                remark = model.Remark ?? "",
                webdomains = model.WebDomains ?? "",
                cusno = model.CusNo ?? "",
                submittime = model.SubmitTime,
                serveroftype = model.ServerOfType,
                contractno = model.ContractNo ?? "",
                contractremark = model.ContractRemark ?? "",
                contractbegintime = model.ContractBeginTime,
                contractendtime = model.ContractEndTime,
                serverremark = model.ServerRemark ?? "",
                customfunction = model.CustomFunction ?? "",
                tag = model.Tag ?? ""
            });
            model.CusId = dbconn.GetIdentity();
            return model;
        }

        public int UpdateCus(CCF.DB.DbConn dbconn, Models.Customer model)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("update `customer` set ");
            sb.Append(" customerName =@customername, ");
            sb.Append(" state =@state, ");
            sb.Append(" remark =@remark, ");
            sb.Append(" webDomains =@webdomains, ");
            sb.Append(" updateTime =now(), ");
            sb.Append(" cusNo =@cusno, ");
            sb.Append(" submitTime =@submittime, ");
            sb.Append(" customFunction =@customfunction, ");
            sb.Append(" serverOfType =@serveroftype, ");
            sb.Append(" contractNo =@contractno, ");
            sb.Append(" contractremark =@contractremark, ");
            sb.Append(" contractbegintime =@contractbegintime, ");
            sb.Append(" contractendtime =@contractendtime, ");
            sb.Append(" serverremark =@serverremark, ");
            sb.Append(" tag =@tag ");
            sb.Append(" where cusid=@cusid;");

            var r = dbconn.ExecuteSql(sb.ToString(), new
             {
                 cusid = model.CusId,
                 customername = model.CustomerName ?? "",
                 state = model.State,
                 remark = model.Remark ?? "",
                 webdomains = model.WebDomains ?? "",
                 cusno = model.CusNo ?? "",
                 submittime = model.SubmitTime,
                 serveroftype = model.ServerOfType,
                 contractno = model.ContractNo ?? "",
                 contractremark = model.ContractRemark ?? "",
                 contractbegintime = model.ContractBeginTime,
                 contractendtime = model.ContractEndTime,
                 serverremark = model.ServerRemark ?? "",
                 customfunction = model.CustomFunction ?? "",
                 tag = model.Tag ?? ""
             });
            return r;
        }

        public List<Models.CustomerLinkManager> GetCusLinks(CCF.DB.DbConn dbconn, int cusid)
        {
            string sql = "select clm.*,m.Name as managername from CustomerLinkManager clm left join manager m on clm.managerid=m.managerid where clm.cusid=@cusid;";
            var models = dbconn.Query<Models.CustomerLinkManager>(sql, new { cusid = cusid });
            return models;
        }

        public Models.CustomerLinkManager AddCusLink(CCF.DB.DbConn dbconn, Models.CustomerLinkManager model)
        {
            string sql = "insert into CustomerLinkManager(cusid,managerid,title,remark)values(@cusid,@managerid,@title,@remark);";
            dbconn.ExecuteSql(sql, new
            {
                cusid = model.CusId,
                managerid = model.ManagerId,
                title = model.Title ?? "",
                remark = model.Remark ?? ""
            });
            model.CusLinkId = dbconn.GetIdentity();
            return model;
        }
         
        public int UpdateCusLink(CCF.DB.DbConn dbconn, Models.CustomerLinkManager model)
        {
            string sql = "update CustomerLinkManager set title=@title , remark=@remark  where cusid=@cusid and managerid=@managerid;";
            int r = dbconn.ExecuteSql(sql, new
            {
                cusid = model.CusId,
                managerid = model.ManagerId,
                title = model.Title ?? "",
                remark = model.Remark ?? ""
            });
            return r;
        }


        public int DeleteCusLink(CCF.DB.DbConn dbconn, int cusid, int managerid)
        {
            string sql = "delete from CustomerLinkManager where cusid=@cusid and managerid=@managerid;";
            int r = dbconn.ExecuteSql(sql, new
              {
                  cusid = cusid,
                  managerid = managerid
              });
            return r;
        }


        public int DeleteCus(CCF.DB.DbConn dbconn, int cusid)
        {
            string sql = "update Customer set state=-1 where cusid=@cusid";
            int r = dbconn.ExecuteSql(sql, new
            {
                cusid = cusid
            });
            return r;
        }
    }
}
