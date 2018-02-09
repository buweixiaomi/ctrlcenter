using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CCF.DB.Mig
{
    public class DataMig
    {
        public bool isrunning { get; set; }
        List<DbStructure.KVModel> tables = new List<DbStructure.KVModel>();
        List<TableMig> tablemigs = new List<TableMig>();
        Config.ImportActionModel actmodel = null;
        Stopwatch sw = new Stopwatch();

        public void GetSpeed(out long avgspeed, out long lastspeed)
        {
            avgspeed = 0;
            lastspeed = 0;

            long allcount = tablemigs.Sum(x => x.currentprocesscount);
            double scound = sw.Elapsed.TotalSeconds;
            scound = scound <= 0 ? 1 : scound;
            avgspeed = (int)(allcount / scound);

            double se = scound - lastseconds;
            if (se != 0)
            {
                lastspeed = (int)((allcount - lastitemcount) / se);
            }
            else
            {
                lastspeed = allcount - lastitemcount;
            }
            lastitemcount = allcount;
            lastseconds = scound;
        }

        Thread mainthread = null;
        public bool finishedinit = false;
        public double processmins { get { return sw.Elapsed.TotalMinutes; } }
        public DataMig(Config.ImportActionModel actmodel)
        {
            this.actmodel = actmodel;
            using (var dbconn1 = DbConn.CreateConn(DbType.SQLSERVER, actmodel.database1.source, actmodel.database1.port, actmodel.database1.database, actmodel.database1.userid, actmodel.database1.password))
            {
                dbconn1.Open();
                DbStructure.IDbStructure dbstru = new DbStructure.DbStructureSqlServer();
                tables = dbstru.GetTables(dbconn1);
            }

            if (actmodel.excepttables != null)
            {
                foreach (var a in actmodel.excepttables)
                {
                    tables.RemoveAll(x => x.name == a);
                }
            }
            foreach (var a in tables)
            {
                TableMig tm = new TableMig(actmodel, a.name);
                tablemigs.Add(tm);
            }
        }

        long lastitemcount = 0;
        double lastseconds = 0;

        public void Start()
        {
            sw.Restart();
            mainthread = new Thread(() =>
            {
                //==
                try
                {
                    finishedinit = false;
                    Config.Log.Write("正在初始化...");
                    foreach (var tm in tablemigs)
                    {
                        tm.Step1_Syn();
                    }
                    Config.Log.Write("初始化完成！");
                    finishedinit = true;
                    Config.Log.Write("正在按表开始数据迁移...");
                    while (true)
                    {
                        int rcount = tablemigs.Count(x => x.runstatus == 1);
                        if (rcount < actmodel.importthreadcount)
                        {
                            var torun = tablemigs.Where(x => x.runstatus == 0).Take(actmodel.importthreadcount - rcount);
                            foreach (var a in torun)
                            {
                                a.Start();
                            }
                        }
                        int ncount = tablemigs.Count(x => x.runstatus == 0);
                        if (ncount == 0)
                            break;
                        else
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    Config.Log.Write("所有表数据迁移已开启完成！");
                }
                catch (Exception ex)
                {
                    Config.Log.Error("启动出错！{0}", ex.Message);
                }
            });
            mainthread.IsBackground = true;
            mainthread.Start();


            isrunning = true;
        }



        public void Stop()
        {
            if (mainthread != null)
            {
                try { mainthread.Abort(); mainthread = null; }
                catch { }
            }
            foreach (var a in tablemigs)
            {
                try { a.Stop(); }
                catch { }
            }
            sw.Stop();
            isrunning = false;
        }

        public List<Config.MigDataProcessModel> GetProcessTableBar()
        {
            //DataTable tb = new DataTable();
            //tb.Columns.Add(new DataColumn() { ColumnName = "tablename", Caption = "表名", DataType = typeof(string) });
            //tb.Columns.Add(new DataColumn() { ColumnName = "totalitemcount", Caption = "总记录数", DataType = typeof(string) });
            //tb.Columns.Add(new DataColumn() { ColumnName = "successcount", Caption = "已处理记录数", DataType = typeof(string) });
            //tb.Columns.Add(new DataColumn() { ColumnName = "processpercent", Caption = "处理百分比", DataType = typeof(string) });
            //tb.Columns.Add(new DataColumn() { ColumnName = "remaintime", Caption = "剩余时间", DataType = typeof(string) });
            //tb.Columns.Add(new DataColumn() { ColumnName = "status", Caption = "状态", DataType = typeof(string) });
            List<Config.MigDataProcessModel> tb = new List<Config.MigDataProcessModel>();

            foreach (var a in tablemigs)
            {
                Config.MigDataProcessModel dr = new Config.MigDataProcessModel();
                long succ_count = a.successcount;
                long curr_pro_count = a.currentprocesscount;
                long all_count = a.allitemcount;
                dr.tablename = a.tablename;
                dr.totalitemcount = a.allitemcount;
                dr.waitcount = a.waitcount;
                dr.successcount = succ_count + curr_pro_count;
                dr.rowbuffercount = a.rowbuffercount;
                dr.execbuffercount = a.execbuffercount;
                if (all_count > 0)
                {
                    dr.processpercent = (succ_count + curr_pro_count) * 100 / all_count;
                }
                else
                {
                    dr.processpercent = finishedinit ? 100 : 0;
                }
                long reamin_count = all_count - curr_pro_count - succ_count;
                double remain_percent = 0;
                if (curr_pro_count > 0)
                {
                    remain_percent = ((double)reamin_count) / curr_pro_count;
                    dr.remainmins = a.currentprocesstime.Elapsed.TotalMinutes * remain_percent;
                }
                else
                {
                    if (a.allitemcount == succ_count + curr_pro_count)
                    {
                        dr.remainmins = 0;
                    }
                    else
                    {
                        dr.remainmins = -1;
                    }
                }
                dr.statuscode = a.runstatus;
                switch (a.runstatus)
                {
                    case -1:
                        dr.statusmsg = "异常";
                        break;
                    case 0:
                        dr.statusmsg = "未开始";
                        break;
                    case 1:
                        dr.statusmsg = "运行中";
                        break;
                    case 2:
                        dr.statusmsg = "已完成(停止,用时" + a.currentprocesstime.Elapsed.TotalMinutes.ToString("0.00") + "mins)";
                        break;
                    default:
                        dr.statusmsg = "未知状态" + a.runstatus.ToString();
                        break;
                }
                tb.Add(dr);
            }
            return tb;

        }
    }

    public class TableMig
    {
        public int runstatus { get; private set; }
        //public int processpercent { get; set; }
        public long currentprocesscount { get; set; }
        public long successcount { get; set; }
        public long allitemcount { get; set; }
        public long waitcount { get { return listdata.Count + listinsert.Sum(x => x.count); } }
        public long rowbuffercount { get { return listdata.Count; } }
        public long execbuffercount { get { return listinsert.Count; } }
        public Stopwatch currentprocesstime = new Stopwatch();

        DbStructure.STable tablestru;
        //   DbConn dbconn1 = null;
        //  DbConn dbconn2 = null;
        Config.ImportActionModel actmodel = null;
        bool getdata_complet = false;
        bool preparedata_complet = false;
        const int Buffer_Page_Count = 3;
        const int Buffer_Exec_Count = 5;
        //  int setdata_pagesize = 1000;
        int beginindex = 0;
        public string tablename = "";
        List<DataRow> listdata = new List<DataRow>();
        List<Config.ExecModel> listinsert = new List<Config.ExecModel>();
        object lock_rows = new object();
        object lock_listinsert = new object();

        public TableMig(Config.ImportActionModel actmodel, string tablename)
        {
            this.actmodel = actmodel;
            this.tablename = tablename;
        }

        Thread mainthread = null;
        Thread getdatathread = null;
        Thread setdatathread = null;
        Thread preparethread = null;
        int zero_count = 0;
        DbConn getdataconn = null;
        private void GetDataAction()
        {
            if (getdata_complet)
                return;
            int getdata_pagesize = 20000;

            lock (actmodel.lockobjofcus)
            {
                if (actmodel.cutomsactions != null && actmodel.cutomsactions.Exists(x => x.tablename == tablename))
                {
                    getdata_pagesize = actmodel.cutomsactions.FirstOrDefault(x => x.tablename == tablename).getpagesize;
                }
                else if (actmodel.getpagesize > 0)
                {
                    getdata_pagesize = actmodel.getpagesize;
                }
            }
            if (listdata.Count > getdata_pagesize * Buffer_Page_Count)
            {
                //Config.Log.Write("GetDataAction 空转");
                return;
            }
            if (listdata.Count == 0)
            {
                zero_count++;
            }
            else
            {
                zero_count = 0;
            }
            if (zero_count >= 2)
            {
                zero_count = 0;
                getdata_pagesize = 2 * getdata_pagesize;
            }
            if (getdataconn == null)
            {
                getdataconn = DbConn.CreateConn(DbType.SQLSERVER, actmodel.database1.source, actmodel.database1.port, actmodel.database1.database, actmodel.database1.userid, actmodel.database1.password);
                getdataconn.Open();
            }

            List<ProcedureParameter> paras = new List<ProcedureParameter>();
            string sql = "select * from (SELECT ROW_NUMBER() over (order by {0}) as rownumber,*  FROM {1}  {2}) A where A.rownumber between @beginindex and @endindex";
            string order_con = "";
            if (tablestru.primarykey != null)
            {
                foreach (var a in tablestru.primarykey.columns)
                {
                    order_con += a.name + " asc,";
                }
            }
            else
            {
                order_con += " getdate()";
            }
            //else if (tablestru.identityattribute != null && tablestru.identityattribute.Count == 1)
            //{
            //    order_con += tablestru.identityattribute[0].column.name + " asc,";
            //}
            //else
            //{
            //    order_con += tablestru.columns.First().name + " asc,";
            //}
            order_con = order_con.TrimEnd(',');
            string where_con = "";
            sql = string.Format(sql, order_con, tablename, where_con);
            paras.Add(new ProcedureParameter("@beginindex", beginindex + 1));
            paras.Add(new ProcedureParameter("@endindex", beginindex + getdata_pagesize));
            DataTable tb = getdataconn.SqlToDataTable(sql, paras);
            tb.Columns.Remove("rownumber");
            beginindex = beginindex + tb.Rows.Count;
            lock (lock_rows)
            {
                foreach (DataRow r in tb.Rows)
                {
                    listdata.Add(r);
                }
            }
            if (tb.Rows.Count != getdata_pagesize)
                getdata_complet = true;

        }

        int setdataerrorcount = 0;
        DbConnMySql setdatamysqlconn = null;
        private int SetDataAction()
        {
            if (listinsert.Count == 0)
            {
                //Config.Log.Write("SetDataAction 空转");//test
                return 0;
            }
            Config.ExecModel insert_model = null;
            lock (lock_listinsert)
            {
                if (listinsert.Count > 0)
                {
                    insert_model = listinsert[0];
                    listinsert.Remove(insert_model);
                }
            }
            if (insert_model == null)
                return 0;
            if (setdatamysqlconn == null)
            {

                var dbconn2 = DbConn.CreateConn(DbType.MYSQL, actmodel.database2.source, actmodel.database2.port, actmodel.database2.database, actmodel.database2.userid, actmodel.database2.password);
                dbconn2.Open();
                setdatamysqlconn = dbconn2 as DbConnMySql;
            }
            // Stopwatch swofexce = new Stopwatch();
            //  swofexce.Start();

            //Config.Log.Write("---SetDataAction Sql执行开始");
            setdatamysqlconn.BeginTransaction();
            try
            {
                setdatamysqlconn.ExecuteSqlLocal(insert_model.sql, insert_model.paras, -1);
                setdatamysqlconn.Commit();
                // swofexce.Stop();

                //  Config.Log.Write("---SetDataAction Sql执行用时:{0}", swofexce.Elapsed.TotalMilliseconds.ToString("0.00"));

                setdataerrorcount = 0;
            }
            catch (Exception ex)
            {
                setdataerrorcount++;
                setdatamysqlconn.Rollback();
                lock (lock_listinsert)
                {
                    listinsert.Insert(0, insert_model);
                }
                Config.Log.Error("插入数据出错：" + ex.Message);
                if (setdataerrorcount >= 3)
                {
                    setdataerrorcount = 0;
                    throw ex;
                }
                else
                {
                    return -1;
                }
            }
            currentprocesscount += insert_model.count;
            return insert_model.count;
        }

        private int PrepareData()
        {
            if (listdata.Count == 0)
            {
                // Config.Log.Write("PrepareData 空转");//test
                return 0;
            }
            if (listinsert.Count >= Buffer_Exec_Count)
            {
                return 0;
            }
            int insertpagesize = 1000;
            lock (actmodel.lockobjofcus)
            {
                if (actmodel.cutomsactions != null && actmodel.cutomsactions.Exists(x => x.tablename == tablename))
                {
                    insertpagesize = actmodel.cutomsactions.FirstOrDefault(x => x.tablename == tablename).insertpagesize;
                }
                else if (actmodel.insertpagesize > 0)
                {
                    insertpagesize = actmodel.insertpagesize;
                }
            }

            List<MySql.Data.MySqlClient.MySqlParameter> paras = new List<MySql.Data.MySqlClient.MySqlParameter>();
            List<DataRow> toinsertrows = new List<DataRow>();
            lock (lock_rows)
            {
                toinsertrows = listdata.Take(insertpagesize).ToList();
                listdata.RemoveRange(0, toinsertrows.Count);
            }
            if (toinsertrows.Count == 0)
            {
                // Config.Log.Write("SetDataAction toinsertrows=0 空转");//test
                return 0;
            }
            StringBuilder sb = new StringBuilder();
            List<string> allcolumns = new List<string>();
            foreach (DataColumn dc in toinsertrows[0].Table.Columns)
            {
                allcolumns.Add(dc.ColumnName);
            }
            sb.AppendFormat("insert into {0}({1})\r\nvalues\r\n", tablename, string.Join(",", allcolumns));
            for (int i = 0; i < toinsertrows.Count; i++)
            {
                allcolumns.Clear();
                foreach (DataColumn dc in toinsertrows[i].Table.Columns)
                {
                    string parname = "@" + dc.ColumnName + "_" + i.ToString();
                    allcolumns.Add(parname);
                    object objvalue = toinsertrows[i][dc.ColumnName];
                    if (objvalue == null)
                    {
                        objvalue = System.DBNull.Value;
                    }
                    if (objvalue.GetType() == typeof(string))
                    {
                        string strvalue = objvalue.ToString();
                        string rvalue = Comm.StringToSafeString(strvalue);
                        if (strvalue != rvalue)
                        {
                            objvalue = rvalue;
                            Config.Log.Alert("四字节字符过滤：[{2}]转化为[{3}] =表:{0} 列:{1} ", tablename, dc.ColumnName, strvalue, rvalue);
                        }
                    }
                    paras.Add(new MySql.Data.MySqlClient.MySqlParameter(parname, objvalue));
                }
                sb.AppendFormat("({0})\r\n,", string.Join(",", allcolumns));
            }
            lock (lock_listinsert)
            {
                listinsert.Add(new Config.ExecModel() { sql = sb.ToString().TrimEnd(',') + ";", paras = paras, count = toinsertrows.Count });
            }
            return toinsertrows.Count;
        }

        public void Start()
        {
            runstatus = 1;
            if (mainthread == null || mainthread.ThreadState != System.Threading.ThreadState.Running)
            {
                mainthread = new Thread(Step2_Asyn);
                mainthread.IsBackground = true;
                mainthread.Start();
            }
        }

        public void Step1_Syn()
        {
            using (var dbconn1 = DbConn.CreateConn(DbType.SQLSERVER, actmodel.database1.source, actmodel.database1.port, actmodel.database1.database, actmodel.database1.userid, actmodel.database1.password))
            using (var dbconn2 = DbConn.CreateConn(DbType.MYSQL, actmodel.database2.source, actmodel.database2.port, actmodel.database2.database, actmodel.database2.userid, actmodel.database2.password))
            {
                dbconn1.Open();
                dbconn2.Open();

                DbStructure.DbStructureSqlServer mssqlstr = new DbStructure.DbStructureSqlServer();
                tablestru = mssqlstr.GetTableStructure(dbconn1, tablename);
                string sql = "select count(1) as cc from " + tablename + " ";
                allitemcount = (int)dbconn1.ExecuteScalar(sql, null);
                successcount = (int)((long)dbconn2.ExecuteScalar(sql, null));

                // (tablestru.identityattribute != null && tablestru.identityattribute.Count == 1)
                if (tablestru.primarykey != null)
                {
                    List<string> pkcol = new List<string>();
                    List<string> pkorder = new List<string>();
                    if (tablestru.primarykey != null)
                    {
                        foreach (var a in tablestru.primarykey.columns)
                        {
                            pkcol.Add(a.name);
                            pkorder.Add(a.name + " desc ");
                        }
                    }
                    //else// if (tablestru.identityattribute != null && tablestru.identityattribute.Count == 1)
                    //{
                    //    pkcol.Add(tablestru.identityattribute[0].column.name);
                    //    pkorder.Add(tablestru.identityattribute[0].column.name + " desc ");
                    //}
                    //else
                    //{
                    //    pkcol.Add(tablestru.columns.First().name);
                    //    pkorder.Add(tablestru.columns.First().name + " desc ");
                    //}
                    string sqltop = "select   {0} from {1} order by {2} limit 0,1;";
                    sqltop = string.Format(sqltop, string.Join(",", pkcol), tablename, string.Join(",", pkorder));



                    DataTable tboftop = dbconn2.SqlToDataTable(sqltop, null);

                    if (tboftop.Rows.Count == 1)
                    {
                        List<ProcedureParameter> parasofgetrownum = new List<ProcedureParameter>();
                        string sqlofpno = "select top 1 A.rownumber from ( SELECT ROW_NUMBER() over (order by {0}) as rownumber,{1} from {2} ) A where {3}";
                        List<string> getrownum_order = new List<string>();
                        List<string> getrownum_where = new List<string>();

                        foreach (string s in pkcol)
                        {
                            object value = tboftop.Rows[0][s];
                            if (value != null)
                            {
                                if (value.GetType() == typeof(MySql.Data.Types.MySqlDateTime))
                                {
                                    MySql.Data.Types.MySqlDateTime nvalue = (MySql.Data.Types.MySqlDateTime)value;
                                    string sofmydatetime = value.ToString();
                                    if (string.IsNullOrEmpty(sofmydatetime))
                                    {
                                        value = System.DBNull.Value;
                                    }
                                    else
                                    {
                                        value = Convert.ToDateTime(sofmydatetime);
                                    }
                                }
                            }

                            getrownum_order.Add(s + " asc");
                            getrownum_where.Add(s + "=@" + s);
                            parasofgetrownum.Add(new ProcedureParameter("@" + s, value));
                        }
                        sqlofpno = string.Format(sqlofpno, string.Join(",", getrownum_order), string.Join(",", pkcol), tablename, string.Join(" and ", getrownum_where));

                        object prerownum = dbconn1.ExecuteScalar(sqlofpno, parasofgetrownum, 2);
                        if (prerownum != null && prerownum.GetType() != typeof(System.DBNull))
                        {
                            beginindex = Convert.ToInt32(prerownum);
                        }
                    }
                }
                else
                {
                    beginindex =(int)successcount;
                }
            }
        }

        private void Step2_Asyn()
        {
            Config.Log.Write("表[{0}]开始数据迁移...", tablename);
            runstatus = 1;
            currentprocesscount = 0;
            currentprocesstime.Restart();
            getdatathread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (allitemcount == successcount + currentprocesscount)
                        {
                            getdata_complet = true;
                        }
                        if (getdata_complet)
                        {
                            break;
                        }
                        // Stopwatch swofallfun = new Stopwatch();

                        GetDataAction();
                        //swofallfun.Stop();
                        //Config.Log.Write("GetDataAction 调用用时:{0}", swofallfun.Elapsed.TotalMilliseconds.ToString("0.00"));

                        Thread.Sleep(10);
                    }
                    Config.Log.Write("表[{0}]生产者线程[取数据]正常关闭", tablename);
                }
                catch (Exception ex)
                {
                    runstatus = -1;
                    Config.Log.Error("表[{0}]生产者线程[取数据]终止:{1}", tablename, ex.Message);
                }
                finally
                {
                    getdata_complet = true;
                    if (getdataconn != null)
                        getdataconn.Dispose();
                }
            });
            getdatathread.IsBackground = true;
            getdatathread.Start();

            preparethread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (getdata_complet && rowbuffercount == 0)
                        {
                            preparedata_complet = true;
                            break;
                        }
                        PrepareData();
                        Thread.Sleep(10);
                    }
                    Config.Log.Write("表[{0}]生产者线程[预处理数据]正常关闭", tablename);
                }
                catch (Exception ex)
                {
                    preparedata_complet = true;
                    runstatus = -1;
                    Config.Log.Error("表[{0}]生产者线程[预处理数据]终止:{1}", tablename, ex.Message);
                }
            });
            preparethread.IsBackground = true;
            preparethread.Start();

            setdatathread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        //Stopwatch swofallfun = new Stopwatch();
                        // swofallfun.Start();
                        //Config.Log.Write("");
                        //Config.Log.Write("--SetDataAction 调用开始");
                        int insert_count = SetDataAction();
                        // swofallfun.Stop();
                        // Config.Log.Write("--SetDataAction 调用用时:{0}", swofallfun.Elapsed.TotalMilliseconds.ToString("0.00"));
                        if (preparedata_complet && waitcount == 0)
                        {
                            if (runstatus != -1)
                                runstatus = 2;
                            currentprocesstime.Stop();
                            break;
                        }
                        else
                        {
                            if (insert_count <= 0)
                                Thread.Sleep(50);
                        }
                    }

                    Config.Log.Write("表[{0}]消费者线程正常关闭", tablename);
                }
                catch (Exception ex)
                {
                    runstatus = -1;
                    Config.Log.Error("表[{0}]消费者线程终止:{1}", tablename, ex.Message);
                }
                finally
                {
                    if (setdatamysqlconn != null)
                        setdatamysqlconn.Dispose();
                }
            });
            setdatathread.IsBackground = true;
            setdatathread.Start();
        }

        public void Stop()
        {
            Config.Log.Write("正在关闭[{0}]的数据迁移...", tablename);
            currentprocesstime.Stop();
            try
            {
                if (mainthread != null && mainthread.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    mainthread.Abort();
                    mainthread = null;
                }
            }
            catch { }
            try
            {
                if (preparethread != null && preparethread.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    preparethread.Abort();
                    preparethread = null;
                }
            }
            catch { }
            try
            {
                if (setdatathread != null && setdatathread.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    setdatathread.Abort();
                    setdatathread = null;
                }
            }
            catch { }
            try
            {
                if (getdatathread != null && getdatathread.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    getdatathread.Abort();
                    getdatathread = null;
                }
            }
            catch { }
            runstatus = 0;
            Config.Log.Write("表[{0}]的数据迁移关闭！", tablename);
        }
    }
}
