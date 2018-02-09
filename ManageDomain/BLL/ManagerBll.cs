using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ManageDomain.BLL
{
    public class ManagerBll
    {
        DAL.ManagerDal managerdal = new DAL.ManagerDal();
        public Models.PageModel<Models.Manager> GetManagerPage(int pno, int pagesize, string keywords)
        {
            using (var dbconn = Pub.GetConn())
            {
                int totalcount = 0;
                var model = managerdal.GetManagerPage(dbconn, pno, pagesize, keywords, out totalcount);
                foreach (var m in model)
                {
                    m.Tags = managerdal.GetManagerTags(dbconn, m.ManagerId);
                }
                return new Models.PageModel<Models.Manager>() { list = model, PageNo = pno, PageSize = pagesize, TotalCount = totalcount };
            }
        }
        public List<Models.Manager> GetManagerTop(int topcount)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = managerdal.GetManagerMiniTop(dbconn, topcount);
                return model;
            }
        }


        public List<Models.UserTag> GetManagerTags(int managerid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = managerdal.GetManagerTags(dbconn, managerid);
                return model;
            }
        }
        public Models.Manager GetManagerDetail(int managerid)
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = managerdal.GetManagerDetail(dbconn, managerid);
                if (model == null)
                    throw new MException(MExceptionCode.NotExist, "用户不存在！");
                model.Tags = managerdal.GetManagerTags(dbconn, model.ManagerId);
                return model;
            }
        }

        public bool DeleteManager(int managerid)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    var model = managerdal.DeleteManager(dbconn, managerid);
                    //添加操作日志
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "员工管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除" + managerid + "号的员工",
                        OperationTitle = "删除信息",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return model > 0;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }

            }
        }


        public Models.Manager AddManager(ManageDomain.Models.Manager model, int[] tags)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new MException(MExceptionCode.BusinessError, "名称不能为空！");
            }
            if (tags == null)
                tags = new int[0];
            tags = tags.Distinct().ToArray();
            using (var dbconn = Pub.GetConn())
            {
                if (!string.IsNullOrEmpty(model.LoginName))
                {
                    if (managerdal.ExistLoginName(dbconn, model.LoginName, 0) > 0)
                        throw new MException(MExceptionCode.BusinessError, "登录名已存在！");
                }
                dbconn.BeginTransaction();
                try
                {
                    model.LoginPwd = CCF.DB.Utility.MakeMD5((model.LoginPwd ?? "000000").Trim());
                    model = managerdal.AddManager(dbconn, model);
                    foreach (var a in tags)
                    {
                        if (managerdal.ExistUserTag(dbconn, a) > 0)
                        {
                            managerdal.AddManagerTag(dbconn, model.ManagerId, a);
                        }
                    }
                    //添加操作日志
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "员工管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "新增" + model.LoginName + "的信息",
                        OperationTitle = "新增信息",
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

        public List<Models.UserTag> GetAllManagerTag()
        {
            using (var dbconn = Pub.GetConn())
            {
                var model = managerdal.GetAllUserTag(dbconn);
                return model;
            }
        }

        public Models.UserTag AddUserTag(string tagname, string remark)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    if (string.IsNullOrEmpty(tagname))
                        throw new MException(MExceptionCode.BusinessError, "标签不能为空！");
                    if (managerdal.ExistUserTag(dbconn, tagname ?? "") > 0)
                    {
                        throw new MException(MExceptionCode.BusinessError, "标签名称已存在！");
                    }
                    Models.UserTag tag = managerdal.AddUserTag(dbconn, new Models.UserTag() { Tag = tagname, Remark = remark ?? "" });
                    //添加操作日志
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "员工管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "添加" + tagname + "标签",
                        OperationTitle = "添加标签",
                        Createtime = DateTime.Now
                    });
                    dbconn.Commit();
                    return tag;
                }
                catch (Exception ex)
                {
                    dbconn.Rollback();
                    throw ex;
                }
            }
        }


        public int DeleteUserTag(int id)
        {
            using (var dbconn = Pub.GetConn())
            {
                if (managerdal.ExistUserTag(dbconn, id) <= 0)
                {
                    throw new MException(MExceptionCode.BusinessError, "标签不存在！");
                }
                dbconn.BeginTransaction();
                try
                {
                    int r = managerdal.DeleteUserTag(dbconn, id);
                    managerdal.DeleteManagerTag(dbconn, id);
                    managerdal.DeleteTagPermission(dbconn, id);

                   var model=managerdal.GetUserTag(dbconn, id);
                    //添加操作日志
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "员工管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "删除usertagid="+id+"的标签and相应的权限删除",
                        OperationTitle = "删除标签",
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


        public bool UpdateManager(ManageDomain.Models.Manager model, int[] tags)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new MException(MExceptionCode.BusinessError, "名称不能为空！");
            }
            if (tags == null)
                tags = new int[0];
            tags = tags.Distinct().ToArray();
            using (var dbconn = Pub.GetConn())
            {
                if (!string.IsNullOrEmpty(model.LoginName))
                {
                    if (managerdal.ExistLoginName(dbconn, model.LoginName, model.ManagerId) > 0)
                        throw new MException(MExceptionCode.BusinessError, "登录名已存在！");
                }
                dbconn.BeginTransaction();
                try
                {

                    int r = managerdal.UpdateManager(dbconn, model);
                    if (r <= 0)
                        throw new MException(MExceptionCode.BusinessError, "更新失败");
                    var managertags = managerdal.GetManagerTags(dbconn, model.ManagerId).Select(x => x.UserTagId).ToArray();
                    foreach (var a in managertags.Except(tags))
                    {
                        managerdal.DeleteManagerTag(dbconn, model.ManagerId, a);
                    }
                    foreach (var a in tags.Except(managertags))
                    {
                        managerdal.AddManagerTag(dbconn, model.ManagerId, a);
                    }
                    //添加操作日志               
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "员工管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "修改" + model.LoginName + "的信息",
                        OperationTitle = "修改信息",
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

        public bool ResetPwd(int managerid)
        {
            string newpwd = CCF.DB.Utility.MakeMD5("000000");
            using (var dbconn = Pub.GetConn())
            {
                if (managerdal.UpdateManagerPwd(dbconn, managerid, newpwd) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Models.Manager LoginIn(string loginname, string loginpwd)
        {
            if (string.IsNullOrEmpty(loginname))
                throw new MException(MExceptionCode.BusinessError, "用户名不能为空！");
            using (var dbconn = Pub.GetConn())
            {
                var models = managerdal.GetByLoginName(dbconn, loginname);
                if (models.Count == 0)
                    throw new MException(MExceptionCode.BusinessError, "用户名不存在！");
                if (models.Count > 1)
                    throw new MException(MExceptionCode.BusinessError, "用户名有重名，请联系管理员！");
                var model = models[0];
                if (model.AllowLogin == 0)
                {
                    throw new MException(MExceptionCode.BusinessError, "不允许登录！");
                }
                if (model.LoginPwd != CCF.DB.Utility.MakeMD5(loginpwd ?? ""))
                {
                    throw new MException(MExceptionCode.BusinessError, "密码不正确！");
                }
                return model;
            }
        }


        public bool ChangePwd(int p, string oldpwd, string newpwd)
        {
            if (string.IsNullOrWhiteSpace(oldpwd))
            {
                throw new MException(MExceptionCode.BusinessError, "原密码不能为空！");
            }
            if (string.IsNullOrWhiteSpace(newpwd))
            {
                throw new MException(MExceptionCode.BusinessError, "新密码不能为空！");
            }
            string md5newpwd = CCF.DB.Utility.MakeMD5(newpwd.Trim());
            string md5oldpwd = CCF.DB.Utility.MakeMD5(oldpwd.Trim());
            using (var dbconn = Pub.GetConn())
            {
                var model = managerdal.GetManagerDetail(dbconn, p);
                if (model.LoginPwd != md5oldpwd)
                {
                    throw new MException(MExceptionCode.BusinessError, "原密码不正确！");
                }
                if (managerdal.UpdateManagerPwd(dbconn, p, md5newpwd) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
