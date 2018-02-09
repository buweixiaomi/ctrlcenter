using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{
    public class WorkDailyBll
    {
        const int CannotEditDays = 7;
        DAL.WorkDailyDal workdailydal = new DAL.WorkDailyDal();
        public Models.WorkDaily Add(Models.WorkDaily model)
        {

            if (DateTime.MinValue == model.WorkTime)
            {
                throw new MException(MExceptionCode.BusinessError, "无效提交时间");
            }
            //if (string.IsNullOrEmpty(model.Summary))
            //{
            //    throw new MException(MExceptionCode.BusinessError, "工作概要不能为空");
            //}
            if (string.IsNullOrEmpty(model.Content))
            {
                throw new MException(MExceptionCode.BusinessError, "工作内容不能为空");
            }

            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var oldmodel = workdailydal.GetDetailByDay(dbconn, model.ManagerId, model.WorkTime);
                    if (oldmodel != null)
                    {
                        throw new MException(MExceptionCode.BusinessError, "该日期已存在记录，请确定日期或手动合并！");
                    }
                    workdailydal.Add(dbconn, model);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "添加" + model.WorkTime + "工作日志",
                        OperationTitle = "添加工作日志",
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

        public bool Update(Models.WorkDaily model)
        {
            if (DateTime.MinValue == model.WorkTime)
            {
                throw new MException(MExceptionCode.BusinessError, "无效提交时间");
            }
            //if (string.IsNullOrEmpty(model.Summary))
            //{
            //    throw new MException(MExceptionCode.BusinessError, "工作概要不能为空");
            //}
            if (string.IsNullOrEmpty(model.Content))
            {
                throw new MException(MExceptionCode.BusinessError, "工作内容不能为空");
            }

            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var oldmodel = workdailydal.GetDetail(dbconn, model.WorkDailyId);
                    if (!CanEdit(oldmodel))
                    {
                        throw new MException(MExceptionCode.BusinessError, "不可编辑！");
                    }
                    var updateresult = workdailydal.Update(dbconn, model);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.WorkTime + "工作日志",
                        OperationTitle = "修改工作日志",
                        Createtime = DateTime.Now
                    });

                    dbconn.Commit();
                    if (updateresult > 0)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public Models.WorkDaily Detail(int workdailyid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = workdailydal.GetDetail(dbconn, workdailyid);
                return model;
            }
        }

        public Models.WorkDaily GetByDate(int managerid, DateTime date)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = workdailydal.GetDetailByDay(dbconn, managerid, date);
                return model;
            }
        }

        public bool CanEdit(Models.WorkDaily model)
        {
            if ((DateTime.Now - model.CreateTime).TotalDays > CannotEditDays)
            {
                return false;
            }
            if (model.ManagerId == Pub.CurrUserId())
                return true;
            return false;
        }

        public string BuildFromWorkItem(int managerid, DateTime dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}  完成工作如下:\r\n\r\n", dt.ToString("yyyy年MM月dd日dddd"));
            string template = "\t{index}、 完成任务：{workitemname}";
            using (var dbconn = Pub.GetConn())
            {
                var models = new DAL.WorkItemDal().GetManagerDailyWork(dbconn, managerid, dt);
                for (int i = 0; i < models.Count; i++)
                {
                    sb.AppendLine(template.Replace("{index}", (i + 1).ToString()).Replace("{workitemname}", models[i].Title));
                }
            }
            sb.AppendLine("\r\n继续努力！");
            return sb.ToString();
        }

        public Models.PageModel<Models.WorkDaily> GetPate(int? currmanagerid, int? managerid, DateTime? begintime, DateTime? endtime, int pno, int pageisze)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var models = workdailydal.GetPage(dbconn, currmanagerid, managerid, begintime, endtime, pno, pageisze, out totalcount);
                return new Models.PageModel<Models.WorkDaily>()
                {
                    list = models,
                    PageNo = pno,
                    PageSize = pageisze,
                    TotalCount = totalcount
                };
            }
        }

        public List<Tuple<Models.Manager, Dictionary<string, Models.WorkDaily>>> GetGroupDaily(int tagid, DateTime begintime, DateTime endtime)
        {
            var rdata = new List<Tuple<Models.Manager, Dictionary<string, Models.WorkDaily>>>();
            DAL.ManagerDal mdal = new DAL.ManagerDal();
            using (var dbconn = Pub.GetConn())
            {
                var users = mdal.GetTagUsers(dbconn, tagid);
                foreach (var user in users)
                {
                    var items = workdailydal.GetUserRangeDaily(dbconn, user.ManagerId, begintime, endtime);
                    Dictionary<string, Models.WorkDaily> datas = new Dictionary<string, Models.WorkDaily>();
                    foreach (var a in items)
                    {
                        datas[a.WorkTime.ToString("yyyy-MM-dd")] = a;
                    }
                    rdata.Add(new Tuple<Models.Manager, Dictionary<string, Models.WorkDaily>>(user, datas));
                }
            }
            return rdata;
        }

        public bool Delete(int targid, int managerid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();

                var model = workdailydal.GetDetail(dbconn, targid);
                if (model == null)
                {
                    throw new MException(MExceptionCode.NotExist, "不存在！");
                }
                if (model.ManagerId != managerid)
                {
                    PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_DeleteOther);
                }
                PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Delete);
                try
                {
                    var r = workdailydal.Delete(dbconn, model.WorkDailyId);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "工作管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除" + model.WorkTime + "的工作日志",
                        OperationTitle = "删除工作日志",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return r > 0;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }
    }
}
