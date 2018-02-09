using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace ManageDomain
{
    public class Pub
    {
        /// <summary>
        /// 0 sqlserver 1mysql 
        /// </summary>
        public static int DataBaseType = 1;
        public static MDbConnection GetConnMySql()
        {
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(GetConnnectionString(SystemConst.MYSQL_CONFIG_NAME));
            conn.Open();
            return new MDbConnection(conn);
        }


        public static CCF.DB.DbConn GetConn()
        {
            CCF.DB.DbConn conn = CCF.DB.DbConn.CreateConn(CCF.DB.DbType.MYSQL, GetConnnectionString(SystemConst.MYSQL_CONFIG_NAME));
            conn.Open();
            return conn;
        }


        public static CCF.DB.DbConn GetServerWatchConn()
        {
            CCF.DB.DbConn conn = CCF.DB.DbConn.CreateConn(CCF.DB.DbType.MYSQL, GetConnnectionString(SystemConst.MYSQL_WATCH_CONFIG_NAME));
            conn.Open();
            return conn;
        }


        public static CCF.DB.DbConn GetWatchLogConn()
        {
            CCF.DB.DbConn conn = CCF.DB.DbConn.CreateConn(CCF.DB.DbType.MYSQL, GetConnnectionString(SystemConst.MySQL_WATCHLOG_CONFIG_NAME));
            conn.Open();
            return conn;
        }


        public static string GetConfig(string key, string deval)
        {
            var conn = System.Configuration.ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(conn))
                return deval;
            return conn;
        }

        public static string GetConnnectionString(string key)
        {
            var conn = System.Configuration.ConfigurationManager.ConnectionStrings[key];
            if (conn == null)
                return null;
            return conn.ConnectionString;
        }

        private static string ToRightTagName(string ortag)
        {
            ortag = (ortag ?? "").Trim();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ortag.Length; i++)
            {
                sb.Append(ortag.Substring(i, 1).Trim());
            }
            ortag = sb.ToString();
            ortag = ortag.Replace(" ", "").Replace(",", "").Replace("[", "").Replace("]", "").Replace(";", "");
            if (string.IsNullOrEmpty(ortag))
                return "";
            ortag = "[" + ortag + "]";
            return ortag;
        }

        public static string[] SplitTags(string ortags)
        {
            ortags = ortags ?? "";
            List<string> tags = new List<string>();
            for (int i = 0; i < ortags.Length; )
            {
                if (ortags[i] != '[')
                {
                    break;
                }
                int end = ortags.IndexOf(']', i);
                if (end < i)
                    break;
                tags.Add(ortags.Substring(i + 1, end - i - 1));
                i = end + 1;
            }
            return tags.ToArray();
        }

        public static string CombineTags(IEnumerable<string> ortags)
        {
            List<string> resulttag = new List<string>();
            if (ortags != null)
            {
                foreach (var a in ortags)
                {
                    resulttag.Add(ToRightTagName(a));
                }
            }
            return string.Join("", resulttag.Distinct());
        }

        public static string GetPrivateConfigName(string projectCodeName, int cusprojectid, string orconfigname)
        {
            return string.Format("{0}_{1}", GetPrivateProjectName(projectCodeName, cusprojectid), orconfigname);
        }


        public static string GetPrivateProjectName(string projectCodeName, int serverprojectid)
        {
            return string.Format("Private_{0}_{1}", serverprojectid, projectCodeName);
        }
        public static string DirToUrl(string dirpath)
        {
            return dirpath.Replace("\\", "/");


        }

        public static string DirPathGetDowloadUrl(string dirpath)
        {
            string durl = string.Format("/api/config/downloadfile?filename={0}", System.Web.HttpUtility.UrlEncode(dirpath));
            return durl;
        }
        public static string SerializeObject(object obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static T DeSerialize<T>(string jsonobj)
        {
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonobj);
            return json;
        }

        public static string GetGroupKey()
        {
            string groupkey = DateTime.Now.ToString("yyMMddHHmmssfff");
            return groupkey;
        }

        public static bool IsRigthHeartbeat(DateTime dt)
        {
            if ((DateTime.Now - dt).TotalSeconds > ManageDomain.SystemConst.Max_Heart_SpanSeconds)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static bool IsServerAlert(DateTime dt)
        {
            if ((DateTime.Now - dt).TotalDays > ManageDomain.SystemConst.Max_Server_Days)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static int IsServerAlertCode(DateTime dt)
        {
            double days = (dt - DateTime.Now).TotalDays;
            if (days < 0)
                return -1;
            if (days <= ManageDomain.SystemConst.Max_Server_Days)
                return 0;
            return 1;
        }

        public static string CurrUserName()
        {
            try
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.User != null)
                {
                    var model = GetTokenModel(System.Web.HttpContext.Current.User.Identity.Name);
                    if (model == null)
                        return "";
                    return model.Name;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static Entity.LoginTokenModel GetTokenModel(string tokenname)
        {
            try
            {
                var model = Pub.DeSerialize<Entity.LoginTokenModel>(tokenname);
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string URLDecode(string encodestring)
        {
            var v = System.Web.HttpUtility.UrlDecode(encodestring ?? "");
            return v;
        }


        public static int CurrUserId()
        {
            if (System.Web.HttpContext.Current == null)
                return 0;
            return CCF.DB.LibConvert.ObjToInt(System.Web.HttpContext.Current.Session["CurrUserId"]);
        }

    }

    public class CCDbConnection2
    {
        public IDbConnection Conn { get; protected set; }
        public CCDbConnection2(IDbConnection baseconn)
        {
            this.Conn = baseconn;
        }

        public IDbTransaction Trans { get; protected set; }

        public void BeginTransaction()
        {
            Trans = Conn.BeginTransaction();
        }
        public void Commit()
        {
            if (Trans != null)
            {
                Trans.Commit();
                Trans = null;
            }
        }

        public void RollBack()
        {
            if (Trans != null)
            {
                Trans.Rollback();
                Trans = null;
            }
        }
    }


    public class MDbConnection : IDbConnection
    {
        public IDbConnection BaseConn { get; protected set; }
        public MDbConnection(IDbConnection baseconn)
        {
            this.BaseConn = baseconn;
        }

        #region 内部扩展
        public IDbTransaction InnerTrans { get; protected set; }

        public void InnerTransaction()
        {
            InnerTrans = BaseConn.BeginTransaction();
        }
        public void InnerCommit()
        {
            if (InnerTrans != null)
            {
                InnerTrans.Commit();
                InnerTrans = null;
            }
        }

        public void InnerRollBack()
        {
            if (InnerTrans != null)
            {
                InnerTrans.Rollback();
                InnerTrans = null;
            }
        }
        #endregion
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return BaseConn.BeginTransaction(il);
        }

        public IDbTransaction BeginTransaction()
        {
            return BaseConn.BeginTransaction();
        }

        public void ChangeDatabase(string databaseName)
        {
            BaseConn.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            BaseConn.Close();
        }

        public string ConnectionString
        {
            get
            {
                return BaseConn.ConnectionString;
            }
            set
            {
                BaseConn.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return BaseConn.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            return BaseConn.CreateCommand();
        }

        public string Database
        {
            get { return BaseConn.Database; }
        }

        public void Open()
        {
            BaseConn.Open();
        }

        public ConnectionState State
        {
            get { return BaseConn.State; }
        }

        public void Dispose()
        {
            BaseConn.Dispose();
        }


    }
}
