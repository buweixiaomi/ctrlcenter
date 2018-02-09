using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.BLL
{

    public class PermissionBll
    {
        DAL.ManagerDal managerdal = new DAL.ManagerDal();
        public bool ManagerExistKey(int managerid, string key)
        {
            using (var dbconn = Pub.GetConn())
            {
                return managerdal.ExistPermissionKey(dbconn, managerid, key);
            }
        }

        public List<string> GetManagerKeys(int managerid)
        {
            using (var dbconn = Pub.GetConn())
            {
                return managerdal.ManagerKeys(dbconn, managerid);
            }
        }

        public List<string> UserTagKeys(int usertagid)
        {
            using (var dbconn = Pub.GetConn())
            {
                return managerdal.TagPermission(dbconn, usertagid);
            }
        }

        public List<PermissionItem> GetTagPermission(int usertagid)
        {
            var result = ManageDomain.PermissionProvider.GetPermissionTree();
            ManageDomain.BLL.PermissionBll pbll = new ManageDomain.BLL.PermissionBll();
            var keys = pbll.UserTagKeys(usertagid);
            foreach (var k in keys)
            {
                SetKey(result, k);
            }
            return result;
        }

        private void SetKey(List<PermissionItem> pis, string key)
        {
            foreach (var pi in pis)
            {
                if (pi.Key == key)
                {
                    pi.HasPermission = 1;
                }
                SetKey(pi.SubPermissions, key);
            }
        }

        public bool SaveTagPermission(int usertagid, List<string> keys)
        {
            using (var dbconn = Pub.GetConn())
            {
                dbconn.BeginTransaction();
                try
                {
                    managerdal.SetTagPermission(dbconn, usertagid, keys);

                    //添加操作日志
                    string ke= string.Empty;
                    foreach (var k in keys)
                    {
                        ke += k + ",";
                    }
                    new OperationLogBll().AddLog(new ManageDomain.Models.OperationLog
                    {
                        Module = "员工管理",
                        OperationName = ManageDomain.Pub.CurrUserName(),
                        OperationContent = "添加" + ke.TrimEnd(',') + "权限",
                        OperationTitle = "编辑标签权限",
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
    }
}
