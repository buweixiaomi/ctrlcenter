using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CCF.DB
{
    public class Utility
    {
        public static List<ProcedureParameter> GetParaFromObj(object obj)
        {
            List<ProcedureParameter> paralist = new List<ProcedureParameter>();
            if (obj == null)
                return paralist;
            if (obj is List<CCF.DB.ProcedureParameter>)
                return obj as List<CCF.DB.ProcedureParameter>;
            foreach (var p in obj.GetType().GetProperties())
            {
                object v = p.GetValue(obj, null);
                ProcedureParameter para = new ProcedureParameter(p.Name, v);
                paralist.Add(para);
            }
            return paralist;
        }

        /// <summary>取得数据库连接字符串(SQL传所有参数、ORACLE传AServerName ALoginName ALoginPass、ACCESS传ADatabaseName ALoginPass)</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="AServerName">服务器名</param>
        /// <param name="ADatabaseName">数据库名</param>
        /// <param name="ALoginName">用户</param>
        /// <param name="ALoginPass">密码</param>
        /// <returns></returns>
        public static string CreateConnString(DbType dbtype, string AServerName, string port, string ADatabaseName, string ALoginName, string ALoginPass)
        {
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    if (ADatabaseName == "")
                        ADatabaseName = "master";
                    if (port == "")
                        port = "1433";
                    return "Data Source=" + AServerName + "," + port + ";Initial Catalog=" + ADatabaseName + ";Persist Security Info=True;User ID=" + ALoginName + ";Password=" + ALoginPass;
                case DbType.MYSQL:
                    if (ADatabaseName == "")
                        ADatabaseName = "information_schema";
                    if (port == "")
                        port = "3306";
                    string t = "server={0};port={1};database={2};user id={3};password={4};CharSet=utf8;Allow Zero Datetime=true";
                    return string.Format(t, AServerName, port, ADatabaseName, ALoginName, ALoginPass);
                //  return "Server=" + AServerName + ";Database=" + ADatabaseName + ";Uid=" + ALoginName + ";Pwd=" + ALoginPass + ";CharSet=UTF8;";
            }
            return "";
        }

        [Obsolete("不再使用，已迁移至CCF.Utils.Sedcurity.MakeMD5")]
        public static string MakeMD5(string oristring)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(oristring);
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] resultbs = md5.ComputeHash(bs);
            md5.Dispose();
            StringBuilder sb = new StringBuilder();
            foreach (var a in resultbs)
            {
                sb.Append(a.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
