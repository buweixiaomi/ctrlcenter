using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Commands.Database
{
    public class ExecuteQueryCommand : ICommand
    {

        /// <summary>
        /// Dbtype connstring  sql
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override int Execute(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                Msg = "参数为三个！";
                return -1;
            }
            string dbtype = args[0];
            string connstring = args[1];
            string sql = args[2];
            CCF.DB.DbType cdbtype = CCF.DB.DbType.MYSQL;
            switch (dbtype.ToLower())
            {
                case "mysql":
                    cdbtype = CCF.DB.DbType.MYSQL;
                    break;
                case "sqlserver":
                    cdbtype = CCF.DB.DbType.SQLSERVER;
                    break;
                default:
                    Msg = "数据库类型不存在！";
                    return -1;
            }

            var sw = new Stopwatch();
            sw.Start();
            System.Data.DataSet ds = new System.Data.DataSet();
            using (CCF.DB.DbConn dbconn = CCF.DB.DbConn.CreateConn(cdbtype, connstring))
            {
                dbconn.Open();
                ds = dbconn.SqlToDataSet(sql, null);
            }
            sw.Stop();
            Msg = "执行完成，用时" + sw.Elapsed.TotalSeconds + "s。\r\n";
            foreach (System.Data.DataTable tb in ds.Tables)
            {
                Msg += tb.TableName + "如果：\r\n";
                Msg += Utils.Utils.SerializeDataTable(tb);
            }
            Msg += " 结束。";
            return 1;
        }


    }
}
