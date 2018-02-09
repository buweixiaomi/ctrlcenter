using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.BLL
{
    public class ProjectBll
    {
        public Models.PageModel<Models.Project> GetPage(string keywords, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = new DAL.ProjectDal().GetPage(dbconn, keywords, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.Project>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Models.Project GetDetail(int projectId)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = new DAL.ProjectDal().GetDetail(dbconn, projectId);
                return model;
            }
        }

        public Tuple<Models.Project, List<Models.ProjectConfig>> GetDetailWidthConfigs(int projectId)
        {
            var dal = new DAL.ProjectDal();
            using (var dbconn = Pub.GetConn())
            {
                var model = dal.GetDetail(dbconn, projectId);
                if (model == null)
                    return null;
                var configs = dal.GetProjectConfig(dbconn, projectId);
                return new Tuple<Models.Project, List<Models.ProjectConfig>>(model, configs);
            }
        }

        public Models.Project Add(Models.Project model, List<Models.ProjectConfig> configs)
        {
            var dal = new DAL.ProjectDal();
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    model = dal.AddProject(dbconn, model);
                    dal.SetProjectConfig(dbconn, model.ProjectId, configs);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.Title + "项目信息",
                        OperationTitle = "新增项目信息",
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

        public int Update(Models.Project model, List<Models.ProjectConfig> configs)
        {
            var dal = new DAL.ProjectDal();
            using (var dbconn = Pub.GetConn())
            {
                int r = 0;
                dbconn.BeginTransaction();
                try
                {
                    r = dal.EditProject(dbconn, model);
                    dal.SetProjectConfig(dbconn, model.ProjectId, configs);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.Title + "项目信息",
                        OperationTitle = "修改项目信息",
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

        public int Delete(int projectId)
        {
            var dal = new DAL.ProjectDal();
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int r = dal.DeleteProject(dbconn, projectId);
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = " 删除项目ID等于" +projectId + "的项目信息",
                        OperationTitle = "删除项目信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return r;
                }
                catch(Exception ex){
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }

        public int SynConfigToCusProject(string configkey)
        {
            var dal = new DAL.ProjectDal();
            using (var dbconn = Pub.GetConn())
            {
                int r = dal.SynConfig(dbconn, configkey);
                return r;
            }
        }


        public List<Models.Project> GetMiniProjects(int count)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.ProjectDal().GetMinProjects(dbconn, count);
                return r;
            }
        }

        public List<Models.ProjectVersion> GetProjectVersions(int projectid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.ProjectDal().GetProjectVersions(dbconn, projectid);
                return r;
            }
        }

        public Models.ProjectVersion AddProjectVersion(Models.ProjectVersion model)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.ProjectDal().AddProjectVersion(dbconn, model);
                return r;
            }
        }


        public int UpdateProjectVersion(Models.ProjectVersion model)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.ProjectDal().UpdateProjectVersion(dbconn, model);
                return r;
            }
        }
    }
}
