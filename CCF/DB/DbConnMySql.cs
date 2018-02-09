using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CCF.DB
{
    public class DbConnMySql : DbConn
    {
        private MySql.Data.MySqlClient.MySqlConnection privateconn;
        MySqlTransaction ts;

        public DbConnMySql(string dbconnstring)
        {
            base._dbtype = DbType.MYSQL;
            privateconn = new MySqlConnection(dbconnstring);
            base._conn = privateconn;
        }
        public override DataSet SqlToDataSet(string sql, List<ProcedureParameter> paras)
        {
            DataSet ds = new DataSet();
            WatchLog.Loger.TimeWatchSql(sql, paras, () =>
            {
                MySqlCommand selectCmd = new MySqlCommand();
                selectCmd.CommandTimeout = 0;
                selectCmd.Transaction = ts;
                selectCmd.Connection = privateconn;
                selectCmd.CommandType = CommandType.Text;
                selectCmd.CommandText = sql;
                if (paras != null)
                {
                    for (int i = 0; i < paras.Count; i++)
                    {
                        MySqlParameter p = ParameterTransform(paras[i]);
                        selectCmd.Parameters.Add(p);
                    }
                }
                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = selectCmd;
                da.Fill(ds);
            });
            return ds;

        }

        public override System.Data.DataTable SqlToDataTable(string sql, List<ProcedureParameter> paras)
        {
            DataTable tb = new DataTable();
            WatchLog.Loger.TimeWatchSql(sql, paras, () =>
            {
                MySqlCommand selectCmd = new MySqlCommand();
                selectCmd.CommandTimeout = 0;
                selectCmd.Transaction = ts;
                selectCmd.Connection = privateconn;
                selectCmd.CommandType = CommandType.Text;
                selectCmd.CommandText = sql;
                if (paras != null)
                {
                    for (int i = 0; i < paras.Count; i++)
                    {
                        MySqlParameter p = ParameterTransform(paras[i]);
                        selectCmd.Parameters.Add(p);
                    }
                }
                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = selectCmd;
                da.Fill(tb);
            });
            return tb;
        }

        public override int ExecuteSql(string sql, List<ProcedureParameter> paras, int minutes)
        {
            int r = 0;
            WatchLog.Loger.TimeWatchSql(sql, paras, () =>
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = privateconn;
                cmd.Transaction = ts;
                if (minutes > 0)
                {
                    cmd.CommandTimeout = minutes * 60;
                }
                else
                {
                    cmd.CommandTimeout = 0;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                if (paras != null)
                {
                    for (int i = 0; i < paras.Count; i++)
                    {
                        MySqlParameter p = ParameterTransform(paras[i]);
                        cmd.Parameters.Add(p);
                    }
                }
                r = cmd.ExecuteNonQuery();
            });
            return r;
        }


        public int ExecuteSqlLocal(string sql, List<MySql.Data.MySqlClient.MySqlParameter> paras, int minutes)
        {
            int r = 0;
            WatchLog.Loger.TimeWatchSql(sql, null, () =>
              {
                  MySql.Data.MySqlClient.MySqlCommand cmd = new MySqlCommand();
                  cmd.Connection = privateconn;
                  cmd.Transaction = ts;
                  if (minutes > 0)
                  {
                      cmd.CommandTimeout = minutes * 60;
                  }
                  else
                  {
                      cmd.CommandTimeout = 0;
                  }
                  cmd.CommandType = CommandType.Text;
                  cmd.CommandText = sql;
                  if (paras != null)
                  {
                      foreach (var a in paras)
                      {
                          cmd.Parameters.Add(a);
                      }
                  }
                  r = cmd.ExecuteNonQuery();
              });
            return r;
        }

        public override int ExecuteSql(string sql, Array paras, int minutes)
        {
            int r = 0;
            WatchLog.Loger.TimeWatchSql(sql, null, () =>
              {
                  MySql.Data.MySqlClient.MySqlCommand cmd = new MySqlCommand();
                  cmd.Connection = privateconn;
                  cmd.Transaction = ts;
                  if (minutes > 0)
                  {
                      cmd.CommandTimeout = minutes * 60;
                  }
                  else
                  {
                      cmd.CommandTimeout = 0;
                  }
                  cmd.CommandType = CommandType.Text;
                  cmd.CommandText = sql;
                  if (paras != null)
                  {
                      cmd.Parameters.AddRange(paras);
                  }
                  r = cmd.ExecuteNonQuery();

              });
            return r;
        }

        public override object ExecuteScalar(string sql, List<ProcedureParameter> paras, int minutes)
        {
            object ob = null;
            WatchLog.Loger.TimeWatchSql(sql, paras, () =>
           {
               MySqlCommand cmd = new MySqlCommand();
               if (minutes > 0)
               {
                   cmd.CommandTimeout = minutes * 60;
               }
               cmd.Connection = privateconn;
               cmd.Transaction = ts;
               cmd.CommandTimeout = 0;
               cmd.CommandType = CommandType.Text;
               cmd.CommandText = sql;
               if (paras != null)
               {
                   for (int i = 0; i < paras.Count; i++)
                   {
                       MySqlParameter p = ParameterTransform(paras[i]);
                       cmd.Parameters.Add(p);
                   }
               }
               ob = cmd.ExecuteScalar();
           });
            return ob;
        }

        public override void BeginTransaction()
        {
            ts = privateconn.BeginTransaction();
        }

        public override void Commit()
        {
            ts.Commit();
        }

        public override void Rollback()
        {
            ts.Rollback();
        }

        public override int GetIdentity()
        {
            object ojb = ExecuteScalar("select LAST_INSERT_ID();", null);
            return Convert.ToInt32(ojb);
        }

        public override DateTime GetServerDate()
        {
            object obj = ExecuteScalar("select now() as Systime;", null);
            return Convert.ToDateTime(obj);
        }


        private MySqlParameter ParameterTransform(ProcedureParameter Par)
        {
            /*车毅修改 支持无类型参数*/
            if (Par.ParType == ProcParType.Default)
                return new MySqlParameter(Par.Name, Par.Value);
            MySqlParameter p = new MySqlParameter();
            p.ParameterName = Par.Name;
            switch (Par.ParType)
            {
                case ProcParType.Int16:
                    p.MySqlDbType = MySqlDbType.Int16;
                    break;
                case ProcParType.Int32:
                    p.MySqlDbType = MySqlDbType.Int32;
                    break;
                case ProcParType.Int64:
                    p.MySqlDbType = MySqlDbType.Int64;
                    break;
                case ProcParType.Single:
                    p.MySqlDbType = MySqlDbType.Float;
                    break;
                case ProcParType.Double:
                    p.MySqlDbType = MySqlDbType.Double;
                    break;
                case ProcParType.Decimal:
                    p.MySqlDbType = MySqlDbType.Decimal;
                    break;
                case ProcParType.Char:
                    p.MySqlDbType = MySqlDbType.VarChar;
                    break;
                case ProcParType.VarChar:
                    p.MySqlDbType = MySqlDbType.VarChar;
                    break;
                case ProcParType.NVarchar:
                    p.MySqlDbType = MySqlDbType.VarString;
                    break;
                case ProcParType.Image:
                    p.MySqlDbType = MySqlDbType.LongBlob;
                    break;
                case ProcParType.DateTime:
                    p.MySqlDbType = MySqlDbType.DateTime;
                    break;
                default:
                    throw new Exception("未知类型ProcParType：" + Par.ParType.ToString());
            }
            p.Size = Par.Size;
            p.Direction = Par.Direction;

            switch (Par.Direction)
            {
                case ParameterDirection.Input:
                case ParameterDirection.InputOutput:
                    if (Par.Value == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        p.Value = Par.Value;
                    }
                    break;
            }
            return p;
        }
    }
}
