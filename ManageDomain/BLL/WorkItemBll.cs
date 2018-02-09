using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class WorkItemBll
    {
        private DAL.WorkItemDal workitemdal = new DAL.WorkItemDal();


        public Models.PageModel<Models.WorkItem> GetPage(int? distributeuserid, int? createuserid, int? state, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var models = workitemdal.GetPage(dbconn, distributeuserid, createuserid, state, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.WorkItem>() { list = models, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Models.WorkItem GetDetail(int workitemid)
        {
            using (var dbconn = Pub.GetConn())
            {
                return workitemdal.GetDetail(dbconn, workitemid);
            }
        }

        public List<Models.WorkDistribute> BuildDistributes(int[] userids)
        {
            List<Models.WorkDistribute> ms = new List<Models.WorkDistribute>();
            if (userids == null)
                return ms;
            using (var dbconn = Pub.GetConn())
            {
                var mdal = new DAL.ManagerDal();
                foreach (var a in userids)
                {
                    var model = mdal.GetManagerDetail(dbconn, a);
                    if (model != null)
                    {
                        ms.Add(new Models.WorkDistribute()
                        {
                            ManagerId = model.ManagerId,
                            ManagerName = model.Name
                        });
                    }
                }
            }
            return ms;
        }

        public Models.WorkItem Add(Models.WorkItem model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                throw new MException(MExceptionCode.BusinessError, "标题不能为空！");
            }
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model.State = model.Distributes.Count > 0 ? 1 : 0;
                    model = workitemdal.Add(dbconn, model);
                    foreach (var a in model.Distributes)
                    {
                        a.WorkItemId = model.WorkItemId;
                        workitemdal.AddDistribute(dbconn, a);
                    }

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.Title + "任务",
                        OperationTitle = "新增工作任务",
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


        public Models.WorkItem Update(Models.WorkItem model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                throw new MException(MExceptionCode.BusinessError, "标题不能为空！");
            }
            using (var dbconn = Pub.GetConn())
            {
                var oldmodel = workitemdal.GetDetail(dbconn, model.WorkItemId);
                if (oldmodel.State != 0 && oldmodel.State != 1)
                {
                    throw new MException(MExceptionCode.BusinessError, "不可编辑！");
                }
                dbconn.BeginTransaction();
                try
                {
                    model.State = model.Distributes.Count > 0 ? 1 : 0;
                    workitemdal.Update(dbconn, model);
                    foreach (var a in oldmodel.Distributes.Select(x => x.ManagerId).Except(model.Distributes.Select(x => x.ManagerId)))
                    {
                        workitemdal.DeleteDistribute(dbconn, model.WorkItemId, a);
                    }

                    foreach (var a in model.Distributes.Select(x => x.ManagerId).Except(oldmodel.Distributes.Select(x => x.ManagerId)))
                    {
                        workitemdal.AddDistribute(dbconn, new Models.WorkDistribute()
                        {
                            ManagerId = a,
                            WorkItemId = model.WorkItemId
                        });
                    }

                    //更新状态
                    var workitem = workitemdal.GetDetail(dbconn, model.WorkItemId);
                    if (workitem.Distributes.Count(x => x.State == 0) == 0)
                    {
                        workitemdal.MakeWorkitemComplete(dbconn, workitem.WorkItemId, workitem.Distributes.Sum(x => x.ActualTime ?? 0));
                    }
                    if ((workitem.FeedbackId ?? 0) > 0)
                    {
                        new DAL.FeedbackDal().WorkitemMakeComplete(dbconn, workitem.FeedbackId ?? 0);
                    }

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.Title + "任务",
                        OperationTitle = "修改工作任务",
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

        public bool MakeDelete(int workitemid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int res = workitemdal.MakeDelete(dbconn, workitemid);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除工作任务ID等于" + workitemid + "的任务",
                        OperationTitle = "删除工作任务",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();

                    return res > 0;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public bool DistributeExec(Models.WorkDistribute model)
        {
            using (var dbconn = Pub.GetConn())
            {
                var workitem = workitemdal.GetDetail(dbconn, model.WorkItemId);
                if (workitem.State != 0 && workitem.State != 1)
                {
                    throw new MException(MExceptionCode.BusinessError, "不允许修改");
                }
                if (workitem.Distributes == null || workitem.Distributes.Count == 0)
                {
                    throw new MException(MExceptionCode.BusinessError, "还未分配");
                }
                var curr = workitem.Distributes.Where(x => x.ManagerId == model.ManagerId).FirstOrDefault();
                if (curr == null)
                {
                    throw new MException(MExceptionCode.BusinessError, "没有分配给你");
                }
                dbconn.BeginTransaction();
                try
                {
                    var dis = workitemdal.MakeDistributeComplete(dbconn, curr.WorkDistributeId, model.ActualTime ?? 0, model.WorkRemark ?? "");
                    workitem = workitemdal.GetDetail(dbconn, model.WorkItemId);
                    if (workitem.Distributes.Count(x => x.State == 0) == 0)
                    {
                        workitemdal.MakeWorkitemComplete(dbconn, workitem.WorkItemId, workitem.Distributes.Sum(x => x.ActualTime ?? 0));
                    }
                    if ((workitem.FeedbackId ?? 0) > 0)
                    {
                        new DAL.FeedbackDal().WorkitemMakeComplete(dbconn, workitem.FeedbackId ?? 0);
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = ""+model.ManagerName+"执行工作任务ID等于" + model.WorkItemId+ "的任务",
                        OperationTitle = "执行工作任务",
                        Createtime = DateTime.Now
                    });

                    dbconn.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }
        public Models.PageModel<Entity.Summary> GetSummary(string begintime, string endtime, int? distributeuserid, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = workitemdal.GetSummary(dbconn, begintime, endtime, distributeuserid, pno, pagesize, out totalcount);
                return new Models.PageModel<Entity.Summary>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

    }
}
