using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.BLL
{
    public class ServerMachineBll
    {
        DAL.ServerMachineDal dal = new DAL.ServerMachineDal();
        public Models.PageModel<Models.ServerMachine> GetPage(string keywords, int pno, int pagesize)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = new DAL.ServerMachineDal().GetServerPage(dbconn, keywords, pno, pagesize, out totalcount);
                return new Models.PageModel<Models.ServerMachine>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }

        public Models.ServerMachine GetDetail(int serverid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = new DAL.ServerMachineDal().GetServerDetail(dbconn, serverid);
                return model;
            }
        }

        public object PingTask(int serverid)
        {
            var serverdal = new DAL.ServerMachineDal();
            var cmddal = new DAL.CommandDal();
            using (var dbconn = Pub.GetConn())
            {
                serverdal.MakeHeart(dbconn, serverid);
                var configdata = serverdal.GetConfig(dbconn, serverid);
                int maxcmdid = cmddal.GetServerMaxCmdId(dbconn, serverid);
                return new { MaxCmdID = maxcmdid, configsign = configdata == null ? "" : configdata.Item1 };
            }
        }


        public Tuple<Models.ServerMachine, List<Models.ServerConfig>> MxGetDetail(int serverid)
        {
            var dal = new DAL.ServerMachineDal();
            using (var dbconn = Pub.GetConn())
            {
                var model = dal.GetServerDetail(dbconn, serverid);
                var configs = dal.GetConfigs(dbconn, serverid);
                return new Tuple<Models.ServerMachine, List<Models.ServerConfig>>(model, configs);
            }
        }


        public List<Models.ServerConfig> GetConfig(int serverid)
        {
            var dal = new DAL.ServerMachineDal();
            using (var dbconn = Pub.GetConn())
            {
                var configs = dal.GetConfigs(dbconn, serverid);
                return configs;
            }
        }
        //public Dictionary<string, string> GetSystemConfig(int serverid)
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    var dal = new DAL.ServerMachineDal();
        //    using (var dbconn = Pub.GetConn())
        //    {
        //        var configs = dal.GetConfigs(dbconn, serverid);
        //        foreach (var a in configs)
        //            dic[a.ConfigKey] = a.ConfigValue;
        //    }
        //    return dic;
        //}


        public void ResetConfig(int serverid)
        {
            using (var dbconn = Pub.GetConn())
            {
                ResetConfig(dbconn, serverid);
            }
        }

        public void ResetConfig(CCF.DB.DbConn dbconn, int serverid)
        {
            var dic = GetConfigDic(dbconn, serverid);
            string config = Pub.SerializeObject(dic);
            string configsign = CCF.DB.Utility.MakeMD5(config);
            dal.SetConfigSign(dbconn, serverid, config, configsign);
        }

        private Dictionary<string, string> GetConfigDic(CCF.DB.DbConn dbconn, int serverid)
        {
            var serverprojectdal = new DAL.ServerProjectDal();
            var prodal = new DAL.ProjectDal();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var configs = dal.GetConfigs(dbconn, serverid);
            foreach (var a in configs)
                dic[a.ConfigKey] = a.ConfigValue;
            var cusproject = serverprojectdal.GetServerProjects(dbconn, serverid);
            dic["ProjectList"] = string.Format("[{0}]", string.Join(",", cusproject.Select(x => x.ServerProjectId).ToList()));
            foreach (var a in cusproject)
            {
                var c_config = serverprojectdal.GetConfigs(dbconn, a.ServerProjectId);
                var c_model = prodal.GetDetail(dbconn, a.ProjectId);
                foreach (var b in c_config)
                {
                    dic[Pub.GetPrivateConfigName(c_model.CodeName, b.ServerProjectId, b.ConfigKey)] = b.ConfigValue;
                }
            }
            return dic;
        }

        public object GetServerUnionConfig(int serverid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var data = dal.GetConfig(dbconn, serverid);
                dal.UpdateConfigUpdateTime(dbconn, serverid);
                if (data == null)
                    return new { config = "", configsign = "" };
                return new { config = data.Item2, configsign = data.Item1 };
            }
        }

        //public Dictionary<string, string> GetPrivateConfig(int serverid)
        //{
        //    var dal = new DAL.ServerMachineDal();
        //    var cusprojectdal = new DAL.CustomerProjectDal();
        //    var prodal = new DAL.ProjectDal();
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    using (var dbconn = Pub.GetConn())
        //    {
        //        var cusproject = cusprojectdal.GetServerProjects(dbconn, serverid);
        //        foreach (var a in cusproject)
        //        {
        //            var c_config = cusprojectdal.GetConfigs(dbconn, a.CusProjectId);
        //            var c_model = prodal.GetDetail(dbconn, a.ProjectId);
        //            foreach (var b in c_config)
        //            {
        //                dic[Pub.GetPrivateConfigName(c_model.CodeName, b.CusProjectId, b.ConfigKey)] = b.ConfigValue;
        //            }
        //        }
        //        return dic;
        //    }
        //}

        public int Update(Models.ServerMachine model, List<ManageDomain.Models.ServerConfig> configs)
        {
            var dal = new DAL.ServerMachineDal();
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                int r = 0;
                try
                {
                    r = dal.UpdateServer(dbconn, model);
                    dal.SetProjectConfig(dbconn, model.ServerId, configs);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.ServerName + "服务器信息",
                        OperationTitle = "修改服务器信息",
                        Createtime = DateTime.Now
                    });
                    ResetConfig(dbconn, model.ServerId);
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


        public Models.ServerMachine Add(Models.ServerMachine model, List<ManageDomain.Models.ServerConfig> configs)
        {
            var dal = new DAL.ServerMachineDal();
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var r = dal.AddServer(dbconn, model);
                    dal.SetProjectConfig(dbconn, r.ServerId, configs);


                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.ServerName + "服务器信息",
                        OperationTitle = "新增服务器信息",
                        Createtime = DateTime.Now
                    });
                    ResetConfig(dbconn, r.ServerId);
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

        public int Delete(int serverid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    int projectscount = new DAL.ServerProjectDal().GetServerProjectCount(dbconn, serverid);
                    if (projectscount > 0)
                    {
                        throw new MException(MExceptionCode.BusinessError, "服务器上存在项目，不能删除！");
                    }
                    var r = new DAL.ServerMachineDal().DeleteServer(dbconn, serverid);

                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "服务器管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除服务器ID等于" + serverid + "服务器信息",
                        OperationTitle = "删除服务器信息",
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

        public List<Models.ServerMachine> GetMiniServers(int count)
        {
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.ServerMachineDal().GetMinServers(dbconn, count);
                return r;
            }
        }

        public Models.ServerMachine GetServerByClientId(string clientid)
        {
            clientid = (clientid ?? "").Replace(",", "").Trim();
            if (string.IsNullOrEmpty(clientid))
                return null;
            using (var dbconn = Pub.GetConn())
            {
                var r = new DAL.ServerMachineDal().GetByClient(dbconn, clientid);
                if (r.Count() != 1)
                    return null;
                return r.First();
            }
        }

        public Models.ServerMachine GetUnionServer(string[] mac, string[] ip, string clientid)
        {
            var dal = new DAL.ServerMachineDal();
            using (var dbconn = Pub.GetConn())
            {
                List<Models.ServerMachine> r = new List<Models.ServerMachine>();
                if (!string.IsNullOrEmpty(clientid))
                {
                    r = dal.GetByClient(dbconn, clientid);
                    if (r.Count == 1)
                    {
                        return r[0];
                    }
                }

                foreach (var m in mac)
                {
                    r = dal.GetByMac(dbconn, m);
                    if (r.Count == 1)
                    {
                        return r[0];
                    }
                }
                foreach (var m in ip)
                {
                    r = dal.GetByIp(dbconn, m);
                    if (r.Count == 1)
                    {
                        return r[0];
                    }
                }
                return null;
            }
        }
    }
}
