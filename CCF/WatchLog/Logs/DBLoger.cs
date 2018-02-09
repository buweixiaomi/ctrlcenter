using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.WatchLog
{
    public class DBLoger : ILoger
    {
        public const string CONFIG_WatchLog_DbConnString = "watchlog:DbConnString";
        public const int PreDayCount = 5;
        private string ConnString = "";
        private DateTime? lastInit = null;

        private object init_locker = new object();
        public DBLoger()
        {
            ConnString = ConfigHelper.GetAppConfig(CONFIG_WatchLog_DbConnString, "");
        }

        public void WriteLog(List<LogEntity> logs)
        {
            if (!InitTable())
                return;
            if (logs == null || logs.Count == 0)
                return;
            _WriteLog(logs);
        }

        private bool InitTable()
        {
            if (lastInit != null && (DateTime.Now - lastInit.Value).TotalHours < 24)
            {
                return true;
            }
            lock (init_locker)
            {
                if (lastInit != null && (DateTime.Now - lastInit.Value).TotalHours < 24)
                {
                    return true;
                }
                if (string.IsNullOrEmpty(ConnString))
                    return false;
                using (MySql.Data.MySqlClient.MySqlConnection dbconn = new MySql.Data.MySqlClient.MySqlConnection(ConnString))
                {
                    DateTime begindate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime endtime = begindate.AddDays(PreDayCount);
                    dbconn.Open();
                    MySql.Data.MySqlClient.MySqlDataAdapter ada = new MySql.Data.MySqlClient.MySqlDataAdapter(" show create table timewatch;", dbconn);
                    System.Data.DataTable tb = new System.Data.DataTable();
                    ada.Fill(tb);
                    if (tb.Rows.Count == 0)
                        return false;
                    string createsql = tb.Rows[0][1].ToString();
                    createsql = createsql.Substring(createsql.IndexOf('('));

                    for (int i = 0; i < PreDayCount; i++)
                    {
                        for (int hour = 0; hour < 24; hour++)
                        {
                            string tablename = "timewatch" + begindate.AddDays(i).ToString("yyyyMMdd") + "_" + hour.ToString("00");
                            ada = new MySql.Data.MySqlClient.MySqlDataAdapter("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + dbconn.Database + "' " +
                             " and TABLE_NAME =  '" + tablename + "' limit 1;", dbconn);
                            tb = new System.Data.DataTable();
                            ada.Fill(tb);
                            if (tb.Rows.Count == 0)
                            {
                                string createtbsql = string.Format("CREATE TABLE {0} {1};\r\n", "`" + tablename + "`", createsql);
                                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();
                                cmd.Connection = dbconn;
                                cmd.CommandType = System.Data.CommandType.Text;
                                cmd.CommandText = createtbsql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                lastInit = DateTime.Now;
                return true;
            }
        }

        private void _WriteLog(List<LogEntity> logs)
        {
            using (MySql.Data.MySqlClient.MySqlConnection dbconn = new MySql.Data.MySqlClient.MySqlConnection(ConnString))
            {
                dbconn.Open();
                int skip = 0;
                int pagesize = 200;
                while (logs.Count > skip)
                {
                    BatchInsert(dbconn, logs.Skip(skip).Take(pagesize).ToList());
                    skip += pagesize;
                }
            }
        }

        private void BatchInsert(MySql.Data.MySqlClient.MySqlConnection dbconn, List<LogEntity> logs)
        {
            string tablename = "timewatch" + DateTime.Now.ToString("yyyyMMdd_HH");
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();
            cmd.Connection = dbconn;
            cmd.CommandType = System.Data.CommandType.Text;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + tablename + "(`projectName`,`groupID`,`innerGroupID`,`logType`,`title`,`content`,`addition`,`createTime`,`createTimeMs`,`dbCreateTime`,`elapsed`)VALUES ");
            List<string> values = new List<string>();
            List<DB.ProcedureParameter> paras = new List<DB.ProcedureParameter>();
            for (int i = 0; i < logs.Count; i++)
            {
                string title = logs[i].Title ?? "";
                if (title.Length > 400)
                    title = title.Substring(0, 400);
                string appendname = "_" + i;
                values.Add(string.Format("(@projectName{0},@groupID{0},@innerGroupID{0},@logType{0},@title{0},@content{0},@addition{0},@createTime{0},@createTimeMs{0},now(),@elapsed{0}) ", appendname));
                cmd.Parameters.AddWithValue("projectName" + appendname, logs[i].ProjectName ?? "");
                cmd.Parameters.AddWithValue("groupID" + appendname, logs[i].GroupID);
                cmd.Parameters.AddWithValue("innerGroupID" + appendname, logs[i].InnerGroupID);
                cmd.Parameters.AddWithValue("logType" + appendname, logs[i].LogType);
                cmd.Parameters.AddWithValue("title" + appendname, title);
                cmd.Parameters.AddWithValue("content" + appendname, logs[i].Content ?? "");
                cmd.Parameters.AddWithValue("addition" + appendname, logs[i].Addition ?? "");
                cmd.Parameters.AddWithValue("createTimeMs" + appendname, logs[i].CreateTime.Millisecond);
                //logs[i].CreateTime = logs[i].CreateTime.AddMilliseconds(logs[i].CreateTime.Millisecond);//去掉毫秒
                cmd.Parameters.AddWithValue("createTime" + appendname, logs[i].CreateTime);
                cmd.Parameters.AddWithValue("elapsed" + appendname, logs[i].Elapsed);
            }
            sb.Append(string.Join(",", values));
            cmd.CommandText = sb.ToString();
            cmd.ExecuteNonQuery();
        }

    }
}
