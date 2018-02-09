using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.BLL
{
    public class ServerProjectBll
    {

        DAL.ServerProjectDal dal = new DAL.ServerProjectDal();
        public List<string> GetAllTag()
        {
            using (var dbconn = Pub.GetConn())
            {
                return dal.GetAllTag(dbconn);
            }
        }

        public Models.PageModel<Tuple<Models.ServerProject, Models.Project, Models.ServerMachine>> SearchPage(string tag, string serverinfo, string projectinfo, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var mdoels = dal.SearchPage(dbconn, tag, serverinfo, projectinfo, pno, pagesize, out totalcount);
                return new Models.PageModel<Tuple<Models.ServerProject, Models.Project, Models.ServerMachine>>() { list = mdoels, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Models.ServerProject GetDetail(int serverprojectid)
        {
            Models.ServerProject model = null;
            using (var dbconn = Pub.GetConn())
            {
                model = dal.GetDetail(dbconn, serverprojectid);
                return model;
            }
        }

        public List<Models.ServerProject> GetServerProjects(int serverid)
        {
            using (var dbconn = Pub.GetConn())
            {
                return dal.GetServerProjects(dbconn, serverid);
            }
        }


        public List<Models.ServerProject> GetServerProjectsByProjectid(int projectid)
        {
            using (var dbconn = Pub.GetConn())
            {
                return dal.GetServerProjectsByProjectid(dbconn, projectid);
            }
        }


        public Models.ServerProject Add(Models.ServerProject model, List<Models.ServerProjectConfig> configs)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model = dal.Add(dbconn, model);
                    foreach (var a in new DAL.ProjectDal().GetProjectConfig(dbconn, model.ProjectId))
                    {
                        var ci = configs.Where(x => x.ConfigKey == a.ConfigKey);
                        if (ci.Count() > 0)
                        {
                            ci.Where(x => { x.CanDelete = 0; return false; });
                        }
                        else
                        {
                            configs.Add(new Models.ServerProjectConfig()
                            {
                                CanDelete = 0,
                                ConfigKey = a.ConfigKey,
                                ConfigValue = a.ConfigValue,
                                ServerProjectId = model.ServerProjectId,
                                ProjectId = model.ProjectId,
                                Remark = ""
                            });
                        }
                    }
                    dal.SetConfigs(dbconn, model.ServerProjectId, model.ProjectId, configs);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "添加" + model.Title + "服务器项目信息",
                        OperationTitle = "添加服务器项目管理",
                        Createtime = DateTime.Now
                    });
                    new BLL.ServerMachineBll().ResetConfig(dbconn, model.ServerId);
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


        public int Update(Models.ServerProject model, List<Models.ServerProjectConfig> configs)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int r = dal.Update(dbconn, model);
                    foreach (var a in configs)
                        a.CanDelete = 1;
                    dal.SetConfigs(dbconn, model.ServerProjectId, model.ProjectId, configs);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改'" + model.Title + "'服务器项目信息",
                        OperationTitle = " 修改服务器项目管理",
                        Createtime = DateTime.Now
                    });
                    new BLL.ServerMachineBll().ResetConfig(dbconn, model.ServerId);
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

        public int Delete(int serverprojectid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int r = dal.DeleteServerProject(dbconn, serverprojectid);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除服务器项目ID等于" + serverprojectid + "",
                        OperationTitle = " 删除服务器项目",
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

        public Tuple<Models.ServerProject, Models.Project, Models.ServerMachine, List<Models.ServerProjectConfig>> GetDetailWith(int serverprojectid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = dal.GetDetailWidth(dbconn, serverprojectid);
                return model;
            }
        }

    }
}
