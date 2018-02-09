using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class FeedbackBll
    {
        ManageDomain.DAL.FeedbackDal feedal = new DAL.FeedbackDal();
        public Models.PageModel<Models.Feedback> PageFeedback(string keyword, int? search_state, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = feedal.GetFeedBack(dbconn, keyword, search_state, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.Feedback>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }

        }

        public Models.Feedback Add(Models.Feedback model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                throw new MException(MExceptionCode.BusinessError, "标题不能为空");
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                throw new MException(MExceptionCode.BusinessError, "反馈内容不能为空");
            }
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model = feedal.Add(dbconn, model);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.CusName + "客户反馈信息",
                        OperationTitle = "新增客户反馈信息",
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

        public int Delete(int feedbackid)
        {
            using (var dbconn = Pub.GetConn())
            {

                dbconn.BeginTransaction();
                try
                {
                    var r = new DAL.FeedbackDal().DeleteFeedback(dbconn, feedbackid);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除返回id等于" + feedbackid + "反馈信息",
                        OperationTitle = "删除客户反馈信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return r;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public Models.Feedback GetFeedbackDetail(int feedbackid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = feedal.GetFeedbackDetail(dbconn, feedbackid);
                if (model == null)
                    throw new MException(MExceptionCode.NotExist, "记录不存在！");
                return model;
            }
        }

        public bool Edit(Models.Feedback model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                throw new MException(MExceptionCode.BusinessError, "标题不能为空");
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                throw new MException(MExceptionCode.BusinessError, "反馈内容不能为空");
            }
            using (var dbconn = Pub.GetConn())
            {

                dbconn.BeginTransaction();
                try
                {
                    int f = feedal.UpdateFeedback(dbconn, model);
                    if (f <= 0)
                        throw new MException(MExceptionCode.BusinessError, "更新失败");

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.CusName + "客户反馈的信息",
                        OperationTitle = "修改反馈信息",
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedbackid"></param>
        /// <param name="resolvetype">1 不允处理   3完成处理</param>
        /// <param name="managerid"></param>
        /// <param name="managername"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool MakeResolve(int feedbackid, int resolvetype, int managerid, string managername, string remark, out int workitemid)
        {
            workitemid = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var r = feedal.GetDetail(dbconn, feedbackid);
                    if (r == null && r.State == -1)
                    {
                        throw new MException(MExceptionCode.BusinessError, "反馈不存在！");
                    }
                    if (r.State != 0)
                    {
                        throw new MException(MExceptionCode.BusinessError, "反馈不是待审核状态！");
                    }
                    int updaterows = 0;
                    switch (resolvetype)
                    {
                        //不与
                        case 1:
                            updaterows = feedal.MakeCheck(dbconn, feedbackid, 1, managerid, managername, null, remark ?? "");
                            break;
                        case 2:
                            var newworkitem = new DAL.WorkItemDal().Add(dbconn, new Models.WorkItem()
                            {
                                Content = r.Content,
                                Difficulty = 3,
                                EstimateTime = 0,
                                FeedbackId = r.FeedbackId,
                                ActualTime = null,
                                CommitTime = null,
                                CreateTime = DateTime.Now,
                                Distributes = new List<Models.WorkDistribute>(),
                                UpdateTime = null,
                                Finaldate = null,
                                Title = r.Title,
                                Importance = 3,
                                ManagerId = managerid,
                                ManagerName = managername ?? "",
                                Point = 0,
                                Remark = "",
                                State = 0,
                                Tag = ""
                            });
                            updaterows = feedal.MakeCheck(dbconn, feedbackid, 2, managerid, managername, newworkitem.WorkItemId, remark ?? "");
                            workitemid = newworkitem.WorkItemId;
                            break;
                        case 3://完成
                            updaterows = feedal.MakeCheck(dbconn, feedbackid, 3, managerid, managername, null, remark ?? "");
                            break;
                        default:
                            throw new MException(MExceptionCode.BusinessError, "无效审核类型！");
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "" + managername + "审核了feedbackId等于" + feedbackid + "的反馈信息",
                        OperationTitle = "审核客户反馈信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return updaterows > 0;
                }
                catch (Exception ex)
                {
                    dbconn.Commit();
                    throw ex;
                }
            }
        }
    }
}