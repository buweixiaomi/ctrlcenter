using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class CusServiceBll
    {
        DAL.CusServiceDal dal = new DAL.CusServiceDal();
        public Models.CusService Add(Models.CusService model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                throw new MException(MExceptionCode.BusinessError, "标题不能为空");
            }
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model = dal.Add(dbconn, model);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "客户管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.Customer.CustomerName + "的客户服务信息",
                        OperationTitle = "新增客户服务信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public Models.PageModel<Models.CusService> GetPage(int pno, int pagesize, int cusid, string keywords)
        {
            DAL.CustomerDal cusdal = new DAL.CustomerDal();
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = dal.GetPage(dbconn, cusid, keywords ?? "", pno, pagesize, out totalcount);
                foreach (var a in model)
                {
                    a.Customer = cusdal.GetDetail(dbconn, a.CusId);
                }
                return new Models.PageModel<Models.CusService>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Models.CusService GetDetail(int cusserviceid)
        {
            DAL.CustomerDal cusdal = new DAL.CustomerDal();
            using (var dbconn = Pub.GetConn())
            {
                var model = dal.GetDetail(dbconn, cusserviceid);
                if (model != null)
                {
                    model.Customer = cusdal.GetDetail(dbconn, model.CusId);
                }
                return model;
            }
        }
    }
}
