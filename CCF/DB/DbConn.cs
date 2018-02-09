using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CCF.DB
{
    public abstract class DbConn : IDisposable
    {   /// <summary>数据库连接内部字段</summary>
        protected DbConnection _conn;
        protected DbType _dbtype;


        public string ConnString
        {
            get { return _conn.ConnectionString; }
        }
        public DbType DbType
        {
            get { return _dbtype; }
        }

        /// <summary>取得数据库连接对象</summary>
        /// <returns></returns>
        public DbConnection GetBaseConnection()
        {
            return _conn;
        }

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public int ExecuteSql(string sql, List<ProcedureParameter> paras)
        {
            return ExecuteSql(sql, paras, -1);
        }

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            List<ProcedureParameter> paras = null;
            return ExecuteSql(sql, paras, -1);
        }

        public int ExecuteSql(string sql, object paras)
        {
            return ExecuteSql(sql, paras, -1);
        }


        public int ExecuteSql(string sql, object paras, int minutes)
        {
            if (paras == null)
                return ExecuteSql(sql, new List<ProcedureParameter>(), minutes);
            return ExecuteSql(sql, Utility.GetParaFromObj(paras), minutes);
        }


        public int ExecuteSql(string sql, Array paras)
        {
            return ExecuteSql(sql, paras, -1);
        }

        public object ExecuteScalar(string sql, object paras)
        {
            return ExecuteScalar(sql, paras, -1);
        }
        public T ExecuteScalar<T>(string sql, object paras)
        {
            object obj = ExecuteScalar(sql, paras, -1);
            return (T)GetValue(typeof(T), obj);
        }

        public object ExecuteScalar(string sql, object paras, int minutes)
        {
            if (paras == null)
                return ExecuteScalar(sql, new List<ProcedureParameter>());
            return ExecuteScalar(sql, Utility.GetParaFromObj(paras), minutes);
        }

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, List<ProcedureParameter> paras)
        {
            return ExecuteScalar(sql, paras, -1);

        }

        public DataSet SqlToDataSet(string sql, object paras)
        {
            if (paras == null)
                return SqlToDataSet(sql, new List<ProcedureParameter>());
            return SqlToDataSet(sql, Utility.GetParaFromObj(paras));
        }
        public DataTable SqlToDataTable(string sql, object paras)
        {
            if (paras == null)
                return SqlToDataTable(sql, new List<ProcedureParameter>());
            return SqlToDataTable(sql, Utility.GetParaFromObj(paras));
        }

        public List<T> Query<T>(string sql, object paras)
        {
            return SqlToModel<T>(sql, paras);
        }

        public List<T> Query<T>(string sql)
        {
            return SqlToModel<T>(sql, null);
        }

        public List<Tuple<T, X>> Query<T, X>(string sql, object para)
        {
            DataTable tb = SqlToDataTable(sql, para);
            var m1 = DataTableToModel<T>(tb);
            var m2 = DataTableToModel<X>(tb);
            List<Tuple<T, X>> result = new List<Tuple<T, X>>();
            for (int i = 0; i < m1.Count; i++)
            {
                result.Add(new Tuple<T, X>(m1[i], m2[i]));
            }
            return result;
        }

        public List<Tuple<T, X>> Query<T, X>(string sql, List<ProcedureParameter> para)
        {
            DataTable tb = SqlToDataTable(sql, para);
            var m1 = DataTableToModel<T>(tb);
            var m2 = DataTableToModel<X>(tb);
            List<Tuple<T, X>> result = new List<Tuple<T, X>>();
            for (int i = 0; i < m1.Count; i++)
            {
                result.Add(new Tuple<T, X>(m1[i], m2[i]));
            }
            return result;
        }

        public List<Tuple<T, X, Y>> Query<T, X, Y>(string sql, object para)
        {
            DataTable tb = SqlToDataTable(sql, para);
            var m1 = DataTableToModel<T>(tb);
            var m2 = DataTableToModel<X>(tb);
            var m3 = DataTableToModel<Y>(tb);
            List<Tuple<T, X, Y>> result = new List<Tuple<T, X, Y>>();
            for (int i = 0; i < m1.Count; i++)
            {
                result.Add(new Tuple<T, X, Y>(m1[i], m2[i], m3[i]));
            }
            return result;
        }

        public List<Tuple<T, X, Y, Z>> Query<T, X, Y, Z>(string sql, object para)
        {
            DataTable tb = SqlToDataTable(sql, para);
            var m1 = DataTableToModel<T>(tb);
            var m2 = DataTableToModel<X>(tb);
            var m3 = DataTableToModel<Y>(tb);
            var m4 = DataTableToModel<Z>(tb);
            List<Tuple<T, X, Y, Z>> result = new List<Tuple<T, X, Y, Z>>();
            for (int i = 0; i < m1.Count; i++)
            {
                result.Add(new Tuple<T, X, Y, Z>(m1[i], m2[i], m3[i], m4[i]));
            }
            return result;
        }

        #region abstract method
        public abstract DataSet SqlToDataSet(string sql, List<ProcedureParameter> paras);
        public abstract DataTable SqlToDataTable(string sql, List<ProcedureParameter> paras);

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public abstract int ExecuteSql(string sql, List<ProcedureParameter> paras, int minutes);

        public abstract int ExecuteSql(string sql, Array paras, int minutes);

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public abstract object ExecuteScalar(string sql, List<ProcedureParameter> para, int minutes);

        /// <summary>启动事务</summary>
        /// <returns></returns>
        public abstract void BeginTransaction();

        /// <summary>提交事务
        /// </summary>
        public abstract void Commit();

        /// <summary>回滚事务
        /// </summary>
        public abstract void Rollback();

        public abstract int GetIdentity();
        public abstract DateTime GetServerDate();

        #endregion abstract method

        #region virtual method
        /// <summary>打开数据库连接
        /// </summary>
        public virtual void Open()
        {
            //string Err = "";
            //if (!Lib.Sys.GetRockState(0,ref Err))
            //    throw new Exception(Err);
            _conn.Open();
        }

        /// <summary>关闭数据库连接
        /// </summary>
        public virtual void Close()
        {
            _conn.Close();
        }

        /// <summary>释放
        /// </summary>
        public virtual void Dispose()
        {
            _conn.Close();
            _conn.Dispose();
            _conn = null;
        }
        #endregion

        public static DbConn CreateConn(DbType dbtype, string connstr)
        {
            DbConn dbconn = null;
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    dbconn = new DbConnMsSql(connstr);
                    break;
                case DbType.MYSQL:
                    dbconn = new DbConnMySql(connstr);
                    break;
                default:
                    throw new Exception("该数据库类型不适合使用CreateConn，请用new创建！");
            }
            return dbconn;
        }

        public static DbConn CreateConn(DbType dbtype, string server, string port, string dbname, string uerid, string pwd)
        {
            string connstring = Utility.CreateConnString(dbtype, server, port, dbname, uerid, pwd);
            return CreateConn(dbtype, connstring);
        }

        public List<T> SqlToModel<T>(string sql, object para)
        {
            DataTable tb = SqlToDataTable(sql, para);
            return DataTableToModel<T>(tb);
        }

        public static List<T> DataTableToModel<T>(DataTable tb)
        {
            Type t = typeof(T);
            List<T> result = new List<T>();
            if (IsEasyType(t))
            {
                foreach (DataRow dr in tb.Rows)
                    result.Add((T)GetValue(t, dr[0]));
                return result;
            }
            SortedDictionary<string, int> cols = new SortedDictionary<string, int>();
            for (int c = 0; c < tb.Columns.Count; c++)
            {
                cols.Add(tb.Columns[c].ColumnName.ToLower(), c);
            }
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                var obj = Activator.CreateInstance<T>();
                result.Add(obj);
            }
            foreach (var a in t.GetProperties())
            {
                //属性里有集合，Pass
                if (a.PropertyType.IsGenericType && a.PropertyType is System.Collections.IEnumerable)
                    continue;
                if (cols.ContainsKey(a.Name.ToLower()))
                {
                    int cindex = cols[a.Name.ToLower()];
                    for (int ain = 0; ain < result.Count; ain++)
                    {
                        a.SetValue(result[ain], GetValue(a.PropertyType, tb.Rows[ain][cindex]), null);
                    }
                }
            }
            return result;
        }

        public static readonly Dictionary<Type, Func<Type, object, object>> TypeHandlers = new Dictionary<Type, Func<Type, object, object>>();
        private static object GetValue(Type denst, object val)
        {
            if (TypeHandlers.ContainsKey(denst))
            {
                return TypeHandlers[denst](denst, val);
            }
            if (denst.Name == "Nullable`1")
            {
                if (val == null || val.Equals(System.DBNull.Value))
                    return null;
                return GetValue(denst.GetGenericArguments()[0], val);
            }
            try
            {
                switch (denst.Name.ToLower())
                {
                    case "int":
                    case "int32":
                        return Convert.ToInt32(val);
                    case "int16":
                        return Convert.ToInt16(val);
                    case "long":
                    case "int64":
                        return Convert.ToInt64(val);
                    case "double":
                        return Convert.ToDouble(val);
                    case "string":
                        return val.ToString();
                    case "decimal":
                        return Convert.ToDecimal(val);
                    case "float":
                        return LibConvert.ObjToFloat(val);
                    case "bool":
                        return Convert.ToBoolean(val);
                    case "byte":
                        return Convert.ToByte(val);
                    case "datetime":
                        if (val is TimeSpan) {
                            return new DateTime(((TimeSpan)val).Ticks);
                        }
                        return Convert.ToDateTime(val);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static List<Type> easyTypes = new List<Type>() { typeof(int), typeof(Int16), typeof(Int64), typeof(byte), typeof(bool), typeof(string), typeof(float), typeof(double), typeof(decimal) };
        private static bool IsEasyType(Type type)
        {
            return easyTypes.Contains(type);
        }
    }
}
