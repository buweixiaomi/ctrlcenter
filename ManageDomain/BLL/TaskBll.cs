using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ManageDomain.BLL
{
    public class TaskBll
    {
        public Models.PageModel<Models.Task> GetPage(string keywords, int serverid, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = new DAL.TaskDal().GetPage(dbconn, keywords, serverid, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.Task>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }
        public Models.Task GetDetail(int taskid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = new DAL.TaskDal().GetDetail(dbconn, taskid);
                return model;
            }
        }


        public Tuple<Models.Task, Models.TaskVersion> GetCurrTaskDetail(int taskid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var dal = new DAL.TaskDal();
                var model = dal.GetDetail(dbconn, taskid);
                if (model == null)
                    return null;
                var versioninfo = dal.GetTaskVersion(dbconn, model.CurrVersionID);
                if (versioninfo == null)
                    return null;
                return new Tuple<Models.Task, Models.TaskVersion>(model, versioninfo);
            }
        }


        public List<object> GetTaskSummary(int serverid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = new DAL.TaskDal().GetServerTasks(dbconn, serverid);
                List<object> objs = new List<object>();
                foreach (var a in model)
                {
                    objs.Add(new
                    {

                        TaskID = a.TaskId,
                        State = a.State
                    });
                }
                return objs;
            }
        }

        public Models.Task AddTask(Models.Task model)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model = new DAL.TaskDal().AddTask(dbconn, model);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.Title + "任务信息",
                        OperationTitle = "修改任务信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();

                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return model;
            }
        }
        public int EditTask(Models.Task model)
        {
            int r = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    r = new DAL.TaskDal().EditTask(dbconn, model);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.Title + "任务信息",
                        OperationTitle = "修改任务信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return r;
            }
        }

        public int DeleteTask(int taskId)
        {
            int r = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    r = new DAL.TaskDal().DeleteTask(dbconn, taskId);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除任务信息",
                        OperationTitle = "删除任务信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return r;
            }
        }
        public int UpdateTaskState(int taskId, int type)
        {
            int r = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    r = new DAL.TaskDal().UpdateTaskState(dbconn, taskId, type);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "更新任务taskid=" + taskId + "的状态为" + type,
                        OperationTitle = "更新任务状态",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return r;
            }
        }
        public int UpdateTaskVersionID(int taskId, int currVersionID)
        {
            int r = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    r = new DAL.TaskDal().UpdateTaskVersionID(dbconn, taskId, currVersionID);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "更新任务taskid=" + taskId + "的当前版本号为" + currVersionID,
                        OperationTitle = "更新任务版本号",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return r;
            }
        }
        public List<Models.TaskVersion> GetTaskVersions(int taskid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.TaskDal().GetTaskVersions(dbconn, taskid);
                return r;
            }
        }

        public Models.TaskVersion AddTaskVersion(Models.TaskVersion model)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model = new DAL.TaskDal().AddTaskVersion(dbconn, model);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "添加" + model.VersionId + "的任务版本信息",
                        OperationTitle = "添加任务版本",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return model;
            }
        }


        public int UpdateTaskVersion(Models.TaskVersion model)
        {
            using (var dbconn = Pub.GetConn())
            {
                int r = 0;
                dbconn.BeginTransaction();
                try
                {
                    r = new DAL.TaskDal().UpdateTaskVersion(dbconn, model);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "添加" + model.VersionId + "的任务版本信息",
                        OperationTitle = "添加任务版本",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return r;
            }
        }
        public int DeleteTaskVersion(int VersionID)
        {
            using (var dbconn = Pub.GetConn())
            {
                int r = 0;
                dbconn.BeginTransaction();
                try
                {
                    r = new DAL.TaskDal().DeleteTaskVersion(dbconn, VersionID);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除任务版本",
                        OperationTitle = "删除任务版本",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
                return r;
            }
        }
        public void SaveTaskSummary(int serverid, string data)
        {
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(data) as Newtonsoft.Json.Linq.JArray;
            var dal = new DAL.TaskDal();

            using (var dbconn = Pub.GetConn())
            {
                var tasks = dal.GetServerTasks(dbconn, serverid);
                for (int i = 0; i < jobj.Count; i++)
                {
                    var a = jobj[i];
                    int tid = a["taskid"].Value<int>();
                    if (tid <= 0)
                        continue;
                    double me = a["memory"].Value<double>();
                    DateTime? lastrun = null;
                    if (a["lastruntime"].ToString() != "")
                    {
                        lastrun = Convert.ToDateTime(a["lastruntime"].ToString());
                    }
                    dal.UpdateRunStateInfo(dbconn, tid, me, lastrun, 1);
                    var task = tasks.FirstOrDefault(x => x.TaskId == tid);
                    if (task != null)
                        tasks.Remove(task);
                }
                foreach (var a in tasks)
                {
                    dal.UpdateRunStateInfo(dbconn, a.TaskId, CCF.DB.LibConvert.StrToDouble(a.Memory), a.LastTime, 0);
                }
            }
        }


    }
}
