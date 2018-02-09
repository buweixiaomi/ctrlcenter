using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.BLL
{
    public class CommandBll
    {
        DAL.CommandDal dal = new DAL.CommandDal();
        DAL.ProjectDal projectdal = new DAL.ProjectDal();
        DAL.ServerProjectDal serverprojectdal = new DAL.ServerProjectDal();
        public Models.Command AddCommand(Models.Command cmdmodel, List<Models.CmdArgument> args)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    AddCommandWidthArgs(dbconn, cmdmodel, args);
                    dbconn.Commit();
                    return cmdmodel;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public void AddCommandWidthArgs(CCF.DB.DbConn dbconn, Models.Command cmdmodel, List<Models.CmdArgument> args)
        {
            cmdmodel.Title = string.IsNullOrEmpty(cmdmodel.Title) ? cmdmodel.CodeName : cmdmodel.Title;
            cmdmodel = dal.AddCmd(dbconn, cmdmodel);
            for (int i = 0; i < args.Count; i++)
            {
                args[i].ArgIndex = i + 1;
                args[i].CmdId = cmdmodel.CmdId;
                dal.AddCmdArg(dbconn, args[i]);
            }
        }

        public Models.PageModel<Tuple<Models.Command, Models.ServerMachine>> GetPage(string groupid, int serverid, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var data = dal.GetCommandPage(dbconn, groupid, serverid, pno, pagesize, out totalcount);
                return new Models.PageModel<Tuple<Models.Command, Models.ServerMachine>>()
                {
                    list = data,
                    PageNo = pno,
                    PageSize = pagesize,
                    TotalCount = totalcount
                };
            }
        }

        public Tuple<Models.Command, List<Models.CmdArgument>> GetDetailWidthArg(int cmdid, out Models.ServerMachine serverinfo)
        {
            serverinfo = null;
            using (var dbconn = Pub.GetConn())
            {
                var cmd = dal.GetCmd(dbconn, cmdid);
                if (cmd == null)
                    return null;
                serverinfo = new DAL.ServerMachineDal().GetServerDetail(dbconn, cmd.ServerId);
                var args = dal.GetCmdArg(dbconn, cmdid);
                return new Tuple<Models.Command, List<Models.CmdArgument>>(cmd, args);
            }
        }

        public List<Tuple<Models.Command, List<Models.CmdArgument>>> GetServerNowCmd(int serverid, int topcount)
        {
            using (var dbconn = Pub.GetConn())
            {
                var cmd = dal.GetServerNewCmds(dbconn, serverid, topcount);
                List<Tuple<Models.Command, List<Models.CmdArgument>>> rdata = new List<Tuple<Models.Command, List<Models.CmdArgument>>>();
                foreach (var a in cmd)
                {
                    rdata.Add(new Tuple<Models.Command, List<Models.CmdArgument>>(a, dal.GetCmdArg(dbconn, a.CmdId)));
                    dal.SetCmdGetTime(dbconn, a.CmdId);
                }
                return rdata;
            }
        }

        public void PreExec(int cmdid)
        {
            using (var dbconn = Pub.GetConn())
            {
                int r = dal.PreExce(dbconn, cmdid);
                if (r == 0)
                    throw new MException(MExceptionCode.BusinessError, "不是待执行状态！");
            }
        }
        public void ResultExec(int cmdid, int resultcode, string msg, string errormsg)
        {
            using (var dbconn = Pub.GetConn())
            {
                if (resultcode > 0)
                    dal.ResultExce(dbconn, cmdid, 2, msg, errormsg);
                else
                    dal.ResultExce(dbconn, cmdid, -1, msg, "resultcode=" + resultcode + "; msg=" + errormsg);
            }
        }


        public int DeleteCmd(int cmdid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int r = dal.MakeDelete(dbconn, cmdid);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除名称操作ID='" + cmdid + "'的信息",
                        OperationTitle = " 删除命令操作",
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

        public Models.Command ReCommit(int cmdid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = dal.GetCmd(dbconn, cmdid);
                var args = dal.GetCmdArg(dbconn, cmdid);
                dbconn.BeginTransaction();
                try
                {
                    AddCommandWidthArgs(dbconn, model, args);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "重新执行命令ID等于'" + cmdid + "'的命令操作",
                        OperationTitle = " 命令操作",
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

        #region 各命令处理

        public int SetPublishApp(int projectid, int versionid, int[] cusprojectids)
        {
            int count = 0;
            using (var dbconn = Pub.GetConn())
            {
                var projectmodel = projectdal.GetDetail(dbconn, projectid);
                if (projectmodel == null)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "项目不存在！");
                }
                var projectversion = projectdal.GetProjectVersion(dbconn, versionid);
                if (projectversion == null)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "项目版本不存在！");
                }
                List<Models.ServerProject> serverprojects = new List<Models.ServerProject>();
                foreach (var a in cusprojectids)
                {
                    var t_c_p = serverprojectdal.GetDetail(dbconn, a);
                    if (t_c_p != null)
                        serverprojects.Add(t_c_p);
                }
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    foreach (var a in serverprojects)
                    {
                        AddCommandWidthArgs(dbconn, new Models.Command()
                        {
                            CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.PUBLISHAPP),
                            GroupKey = groupkey,
                            ServerId = a.ServerId,
                            Title = "发布项目[" + projectmodel.Title + "-" + projectversion.VersionNo + "]"
                        }, new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ArgValue =  a.ServerProjectId.ToString() },
                        new Models.CmdArgument(){ ArgValue = Pub.GetPrivateProjectName( projectmodel.CodeName,a.ServerProjectId), ContainConfig = 1 },
                        new Models.CmdArgument(){ ArgValue = projectversion.VersionNo, ContainConfig = 0 },
                        new Models.CmdArgument(){ ArgValue = Pub.DirPathGetDowloadUrl(projectversion.DownloadUrl), ContainConfig = 0 }
                        });
                        count++;
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "发布项目'[" + projectmodel.Title + "-" + projectversion.VersionNo + "]",
                        OperationTitle = " 命令操作",
                        Createtime = DateTime.Now
                    });

                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public int SetBackupApp(int projectid, int[] cusprojectids)
        {

            int count = 0;
            using (var dbconn = Pub.GetConn())
            {
                var projectmodel = projectdal.GetDetail(dbconn, projectid);
                if (projectmodel == null)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "项目不存在！");
                }
                List<Models.ServerProject> cusprojects = new List<Models.ServerProject>();
                foreach (var a in cusprojectids)
                {
                    var t_c_p = serverprojectdal.GetDetail(dbconn, a);
                    if (t_c_p != null)
                        cusprojects.Add(t_c_p);
                }
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    foreach (var a in cusprojects)
                    {
                        AddCommandWidthArgs(dbconn, new Models.Command()
                        {
                            CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.BACKUPAPP),
                            GroupKey = groupkey,
                            ServerId = a.ServerId,
                            Title = "备份项目[" + projectmodel.Title + "]"
                        }, new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ArgValue = a.ServerProjectId.ToString()  },
                        new Models.CmdArgument(){ ArgValue = Pub.GetPrivateProjectName( projectmodel.CodeName,a.ServerProjectId) , ContainConfig = 1 }
                        });
                        count++;
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "备份项目'[" + projectmodel.Title + "]",
                        OperationTitle = " 命令操作",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }


        public int SetRollbackApp(int projectid, int[] cusprojectids)
        {

            int count = 0;
            using (var dbconn = Pub.GetConn())
            {
                var projectmodel = projectdal.GetDetail(dbconn, projectid);
                if (projectmodel == null)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "项目不存在！");
                }
                List<Models.ServerProject> cusprojects = new List<Models.ServerProject>();
                foreach (var a in cusprojectids)
                {
                    var t_c_p = serverprojectdal.GetDetail(dbconn, a);
                    if (t_c_p != null)
                        cusprojects.Add(t_c_p);
                }
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    foreach (var a in cusprojects)
                    {
                        AddCommandWidthArgs(dbconn, new Models.Command()
                        {
                            CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.ROLLBACKAPP),
                            GroupKey = groupkey,
                            ServerId = a.ServerId,
                            Title = "回退项目（上一版本）[" + projectmodel.Title + "]"
                        }, new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ArgValue = a.ServerProjectId.ToString()  },
                        new Models.CmdArgument(){ ArgValue =Pub.GetPrivateProjectName( projectmodel.CodeName,a.ServerProjectId), ContainConfig = 1 },
                        new Models.CmdArgument(){ ArgValue = "-1", ContainConfig = 0 },
                        });
                        count++;
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "回退项目（上一版本）[" + projectmodel.Title + "]",
                        OperationTitle = " 命令操作",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }


        public int SetUpdateConfig(int[] serverids)
        {
            int count = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    foreach (var a in serverids)
                    {
                        AddCommandWidthArgs(dbconn,
                            new Models.Command()
                            {
                                CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.UPDATECONFIG),
                                GroupKey = groupkey,
                                ServerId = a,
                                Title = "更新服务器配置"
                            },
                        new List<Models.CmdArgument>()
                        );
                        count++;
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "更新服务器配置",
                        OperationTitle = " 命令操作",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }


        public int SetExecCmd(string cmdline, int[] serverids)
        {
            int count = 0;
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    foreach (var a in serverids)
                    {
                        AddCommandWidthArgs(dbconn,
                            new Models.Command()
                            {
                                CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.CMD),
                                GroupKey = groupkey,
                                ServerId = a,
                                Title = "执行CMD"
                            },
                        new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ContainConfig = 0, ArgValue = cmdline??""}
                        }
                        );
                        count++;
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "执行CMD",
                        OperationTitle = " 命令操作",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }


        public int SetStartTaskCmd(int taskid)
        {
            var dal = new DAL.TaskDal();
            int count = 1;
            using (var dbconn = Pub.GetConn())
            {
                var taskmodel = dal.GetDetail(dbconn, taskid);
                if (taskmodel == null || taskmodel.State == -1)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "任务不存在！");
                }
                if (taskmodel.CurrVersionID <= 0)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "任务版本不存在或没设置当前版本！");
                }
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    new CommandBll().AddCommandWidthArgs(dbconn, new Models.Command()
                    {
                        CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.STARTTASK),
                        GroupKey = groupkey,
                        ServerId = taskmodel.ServerID,
                        Title = "发布任务[" + taskmodel.Title + "-" + taskmodel.CurrVersionID + "]"
                    }, new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ArgValue = taskid.ToString()}
                        });
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理-任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "发布任务'[" + taskmodel.Title + "-" + taskmodel.CurrVersionID + "]",
                        OperationTitle = " 启动任务",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public int SetStopTaskCmd(int taskid)
        {
            var dal = new DAL.TaskDal();
            int count = 1;
            using (var dbconn = Pub.GetConn())
            {
                var taskmodel = dal.GetDetail(dbconn, taskid);
                if (taskmodel == null || taskmodel.State == -1)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "任务不存在！");
                }
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    new CommandBll().AddCommandWidthArgs(dbconn, new Models.Command()
                    {
                        CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.STOPTASK),
                        GroupKey = groupkey,
                        ServerId = taskmodel.ServerID,
                        Title = "停止任务[" + taskmodel.Title + "-" + taskmodel.TaskId + "]"
                    }, new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ArgValue = taskid.ToString()}
                        });
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理-任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "停止任务'[" + taskmodel.Title + "-" + taskmodel.TaskId + "]",
                        OperationTitle = " 停止任务",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }


        public int SetDeleteTaskCmd(int taskid)
        {
            var dal = new DAL.TaskDal();
            int count = 1;
            using (var dbconn = Pub.GetConn())
            {
                var taskmodel = dal.GetDetail(dbconn, taskid);
                if (taskmodel == null || taskmodel.State == -1)
                {
                    throw new ManageDomain.MException(MExceptionCode.BusinessError, "任务不存在！");
                }
                dbconn.BeginTransaction();
                try
                {
                    string groupkey = Pub.GetGroupKey();
                    new CommandBll().AddCommandWidthArgs(dbconn, new Models.Command()
                    {
                        CodeName = EnumHelper.GetEnumDesc(COMMANDTYPE.DELETETASK),
                        GroupKey = groupkey,
                        ServerId = taskmodel.ServerID,
                        Title = "卸载任务[" + taskmodel.Title + "-" + taskmodel.TaskId + "]"
                    }, new List<Models.CmdArgument>() { 
                        new Models.CmdArgument(){ ArgValue = taskid.ToString()}
                        });
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理-任务列表",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "卸载任务'[" + taskmodel.Title + "-" + taskmodel.TaskId + "]",
                        OperationTitle = " 卸载任务",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }
        #endregion
    }
}
