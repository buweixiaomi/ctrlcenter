using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.BLL
{
    public class CustomerBll
    {
        DAL.CustomerDal cusdal = new DAL.CustomerDal();
        public Models.PageModel<Models.Customer> PageCustomer(string keywords, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = cusdal.GetCustomerPage(dbconn, keywords, pno, pagesize, out totalcount);
                foreach (var a in model)
                {
                    a.LinkManagers = cusdal.GetCusLinks(dbconn, a.CusId);
                }
                return new Models.PageModel<Models.Customer>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Models.Customer GetCusDetail(int cusid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = cusdal.GetDetail(dbconn, cusid);
                model.LinkManagers = cusdal.GetCusLinks(dbconn, cusid);
                return model;
            }
        }

        public Models.Customer GetCusDetailByCusNo(string cusno)
        {
            if (string.IsNullOrWhiteSpace(cusno))
                return null;
            using (var dbconn = Pub.GetConn())
            {
                var model = cusdal.GetDetailByCusNo(dbconn, cusno);
                return model;
            }
        }

        public List<Models.Customer> GetMinCustomers(int count)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = cusdal.GetMinCustomers(dbconn, count);
                return r;
            }
        }

        public Models.Customer AddCus(Models.Customer model, List<ManageDomain.Models.CustomerLinkManager> links)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var r = cusdal.AddCus(dbconn, model);
                    foreach (var a in links)
                    {
                        a.CusId = r.CusId;
                        cusdal.AddCusLink(dbconn, a);
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "客户管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.CustomerName + "的信息",
                        OperationTitle = "新增客户信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return r;
                }
                catch (Exception ex) { dbconn.Rollback(); throw ex; }
            }
        }

        public int UpdateCus(Models.Customer model, List<ManageDomain.Models.CustomerLinkManager> links)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var r = cusdal.UpdateCus(dbconn, model);
                    var dblinks = cusdal.GetCusLinks(dbconn, model.CusId).Select(x => x.ManagerId).ToList();
                    //del
                    foreach (var a in dblinks.Except(links.Select(x => x.ManagerId)))
                    {
                        cusdal.DeleteCusLink(dbconn, model.CusId, a);
                    }
                    //add
                    foreach (var a in links.Where(x => !dblinks.Contains(x.ManagerId)))
                    {
                        a.CusId = model.CusId;
                        cusdal.AddCusLink(dbconn, a);
                    }
                    //update
                    foreach (var a in links.Where(x => dblinks.Contains(x.ManagerId)))
                    {
                        a.CusId = model.CusId;
                        cusdal.UpdateCusLink(dbconn, a);
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "客户管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.CustomerName + "的信息",
                        OperationTitle = "修改客户信息",
                        Createtime = DateTime.Now
                    });

                    dbconn.Commit();
                    return r;
                }
                catch (Exception ex) { dbconn.Rollback(); throw ex; }
            }
        }

        public bool Delete(int cusid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int r = cusdal.DeleteCus(dbconn, cusid);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "客户管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除客户ID=" + cusid + "的信息",
                        OperationTitle = "删除客户信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return r > 0;
                }
                catch (Exception ex) { dbconn.Rollback(); throw ex; }

            }
        }
    }
}
