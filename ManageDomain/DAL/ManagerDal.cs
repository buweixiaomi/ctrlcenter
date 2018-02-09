using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain.DAL
{
    public class ManagerDal
    {

        #region Tag相关
        public List<Models.UserTag> GetAllUserTag(CCF.DB.DbConn dbconn)
        {
            string sql = "select * from usertag;";
            var mdoels = dbconn.Query<Models.UserTag>(sql);
            return mdoels;
        }

        public Models.UserTag AddUserTag(CCF.DB.DbConn dbconn, Models.UserTag model)
        {
            string sql = "insert into usertag(tag,createtime,remark) values(@tag,now(),@remark);";
            dbconn.ExecuteSql(sql, new { tag = model.Tag, remark = @model.Remark ?? "" });
            model.UserTagId = dbconn.GetIdentity();
            return model;
        }


        public int UpdateUserTag(CCF.DB.DbConn dbconn, Models.UserTag model)
        {
            string sql = "update usertag set tag=@tag,remark＠remark where usertagid=@usertagid;";
            int r = dbconn.ExecuteSql(sql, new { tag = model.Tag, remark = @model.Remark ?? "", usertagid = model.UserTagId });
            return r;
        }

        public int DeleteUserTag(CCF.DB.DbConn dbconn, string tag)
        {
            string sql = "delete from  usertag  where tag=@tag;";
            int r = dbconn.ExecuteSql(sql, new { tag = tag });
            return r;
        }


        public int DeleteUserTag(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "delete from  usertag  where usertagid=@usertagid;";
            int r = dbconn.ExecuteSql(sql, new { usertagid = usertagid });
            return r;
        }


        public int ExistUserTag(CCF.DB.DbConn dbconn, string tag, int except_usertagid = 0)
        {
            string sql = "select count(1) from  usertag  where tag=@tag and usertagid<>@usertagid;";
            int r = dbconn.ExecuteScalar<int>(sql, new { tag = tag, usertagid = except_usertagid });
            return r;
        }

        public int ExistUserTag(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "select count(1) from  usertag  where usertagid=@usertagid;";
            int r = dbconn.ExecuteScalar<int>(sql, new { usertagid = usertagid });
            return r;
        }

        public Models.UserTag GetUserTag(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "select * from  usertag  where usertagid=@usertagid;";
            var r = dbconn.Query<Models.UserTag>(sql, new { usertagid = usertagid }).FirstOrDefault();
            return r;
        }


        #endregion end TAG

        #region managerTAg
        public List<Models.UserTag> GetManagerTags(CCF.DB.DbConn dbconn, int managerid)
        {
            string sql = "SELECT ut.* FROM managertaglink mtl left join usertag ut on mtl.usertagid=ut.usertagid WHERE managerid=@managerid;";
            var models = dbconn.Query<Models.UserTag>(sql, new { managerid = managerid });
            return models;
        }

        public int AddManagerTag(CCF.DB.DbConn dbconn, int managerid, int usertagid)
        {
            string sql = "insert into managertaglink(managerid,usertagid) values(@managerid,@usertagid);";
            int r = dbconn.ExecuteSql(sql, new { managerid = managerid, usertagid = usertagid });
            return r;
        }

        public bool ExistManagerTag(CCF.DB.DbConn dbconn, int managerid, int usertagid)
        {
            string sql = "select count(1) from managertaglink where managerid=@managerid and usertagid=@usertagid;";
            int r = dbconn.ExecuteScalar<int>(sql, new { managerid = managerid, usertagid = usertagid });
            return r > 0;
        }


        public int DeleteManagerTag(CCF.DB.DbConn dbconn, int managerid, int usertagid)
        {
            string sql = "delete from managertaglink where managerid=@managerid and usertagid=@usertagid;";
            int r = dbconn.ExecuteSql(sql, new { managerid = managerid, usertagid = usertagid });
            return r;
        }

        public int DeleteManagerTag(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "delete from managertaglink where usertagid=@usertagid;";
            int r = dbconn.ExecuteSql(sql, new { usertagid = usertagid });
            return r;
        }

        #endregion

        #region Manger相关

        public Models.Manager GetManagerDetail(CCF.DB.DbConn dbconn, int managerid)
        {
            string sql = "select * from manager where managerid=@managerid;";
            var model = dbconn.Query<Models.Manager>(sql, new { managerid = managerid }).FirstOrDefault();
            return model;
        }

        public List<Models.Manager> GetByLoginName(CCF.DB.DbConn dbconn, string loginname)
        {
            string sql = "select * from manager where loginname=@loginname and  state<>-1;";
            var model = dbconn.Query<Models.Manager>(sql, new { loginname = loginname });
            return model.ToList();
        }

        public List<Models.Manager> GetManagerPage(CCF.DB.DbConn dbconn, int pno, int pagesize, string keywords, out int totalcount)
        {
            string sql = @"select * from manager where state<>-1 and 
                                    name like concat('%',@keywords,'%')
                                    and 
                                    subname like concat('%',@keywords,'%')
                                    and 
                                    loginname like concat('%',@keywords,'%')
                                    order by managerid asc
                                    limit @startindex,@pagesize;";
            string countsql = @"select count(1) from manager where state<>-1 and 
                                    name like concat('%',@keywords,'%')
                                    and 
                                    subname like concat('%',@keywords,'%')
                                    and 
                                    loginname like concat('%',@keywords,'%')";
            var models = dbconn.Query<Models.Manager>(sql, new
            {
                keywords = keywords ?? "",
                startindex = (pno - 1) * pagesize,
                pagesize = pagesize
            });
            totalcount = dbconn.ExecuteScalar<int>(countsql, new
            {
                keywords = keywords ?? ""
            });
            return models;
        }


        public List<Models.Manager> GetManagerMiniTop(CCF.DB.DbConn dbconn, int topcount)
        {
            string sql = @"select managerid,name,subname from manager where state<>-1 limit @topcount;";
            var models = dbconn.Query<Models.Manager>(sql, new
            {
                topcount = topcount
            });
            return models;
        }

        public Models.Manager AddManager(CCF.DB.DbConn dbconn, Models.Manager model)
        {
            string sql = "insert into manager(name,subname,loginname,loginpwd,allowlogin,state,createtime,remark)" +
                "values(@name,@subname,@loginname,@loginpwd,@allowlogin,@state,now(),@remark);";
            dbconn.ExecuteSql(sql, new
            {
                name = model.Name,
                subname = model.SubName ?? "",
                loginname = model.LoginName ?? "",
                loginpwd = model.LoginPwd ?? "",
                allowlogin = model.AllowLogin,
                state = model.State,
                remark = model.Remark ?? ""
            });
            model.ManagerId = dbconn.GetIdentity();
            return model;
        }

        public int UpdateManager(CCF.DB.DbConn dbconn, Models.Manager model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE  `manager` set ");
            sb.Append("`name` = @name,");
            sb.Append("`subName` = @subname,");
            sb.Append("`loginName` = @loginname,");
            //  sb.Append("`loginPwd` = @loginpwd,");
            sb.Append("`allowLogin` = @allowlogin,");
            sb.Append("`state` = @state,");
            sb.Append("`updateTime` = now(),");
            sb.Append("`remark` = @remark ");
            sb.Append(" WHERE `managerId` = @managerid;");

            int rows = dbconn.ExecuteSql(sb.ToString(), new
             {
                 managerid = model.ManagerId,

                 name = model.Name,
                 subname = model.SubName ?? "",
                 loginname = model.LoginName ?? "",
                 loginpwd = model.LoginPwd ?? "",
                 allowlogin = model.AllowLogin,
                 state = model.State,
                 remark = model.Remark ?? ""
             });
            return rows;
        }

        public int DeleteManager(CCF.DB.DbConn dbconn, int managerid)
        {
            string sql = "update manager set state=-1 where managerid=@managerid;";
            int rows = dbconn.ExecuteSql(sql, new { managerid = managerid });
            return rows;
        }

        public int ExistLoginName(CCF.DB.DbConn dbconn, string loginname, int except_managerid)
        {
            string sql = "select count(1) from manager where loginname=@loginname and state<>-1 and managerid<>@managerid;";
            int count = dbconn.ExecuteScalar<int>(sql, new { loginname = loginname, managerid = except_managerid });
            return count;
        }

        public int UpdateManagerPwd(CCF.DB.DbConn dbconn, int managerid, string newpwd)
        {
            string sql = "update manager set loginpwd=@loginpwd where managerid=@managerid;";
            int rows = dbconn.ExecuteSql(sql, new { managerid = managerid, loginpwd = newpwd ?? "" });
            return rows;
        }

        public bool ExistPermissionKey(CCF.DB.DbConn dbconn, int managerid, string permissionkey)
        {
            string sql = "SELECT count(1) FROM managertaglink ml " +
                            "join tagpermission tp on ml.usertagId=tp.usertagId " +
                            "where ml.managerId=@managerid and tp.permissionKey=@permissionkey;";
            int r = dbconn.ExecuteScalar<int>(sql, new { managerid = managerid, permissionkey = permissionkey });
            return r > 0;
        }

        public List<string> ManagerKeys(CCF.DB.DbConn dbconn, int managerid)
        {
            string sql = "SELECT distinct tp.permissionKey FROM managertaglink ml " +
                            "join tagpermission tp on ml.usertagId=tp.usertagId " +
                            "where ml.managerId=@managerid;";
            List<string> r = dbconn.Query<string>(sql, new { managerid = managerid });
            return r;
        }

        public List<string> TagPermission(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "select permissionKey from tagpermission where usertagid=@usertagid;";
            List<string> r = dbconn.Query<string>(sql, new { usertagid = usertagid });
            return r;
        }


        public void SetTagPermission(CCF.DB.DbConn dbconn, int usertagid, List<string> keys)
        {
            DeleteTagPermission(dbconn, usertagid);
            string sqlinsert = "insert into   tagpermission(usertagid,permissionKey) values(@usertagid,@key);"; 
            foreach (var k in keys)
            {
                if (string.IsNullOrEmpty(k))
                    continue;
                dbconn.ExecuteSql(sqlinsert, new { usertagid = usertagid, key = k });
            }
        }

        public int DeleteTagPermission(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "delete from   tagpermission where usertagid=@usertagid;";
            return dbconn.ExecuteSql(sql, new { usertagid = usertagid });
        }

        public List<Models.Manager> GetTagUsers(CCF.DB.DbConn dbconn, int usertagid)
        {
            string sql = "select m.* from managertaglink ml join manager m on m.managerid=ml.managerid where m.state<>-1 and ml.usertagid=@usertagid;";
            List<Models.Manager> r = dbconn.Query<Models.Manager>(sql, new { usertagid = usertagid });
            return r;
        }

        #endregion end Manager相关


    }
}
