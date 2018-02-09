using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CCF.DB.Mig
{
    public class Comm
    {
        public static Config.ImportActionModel MigMiniModelToModel(List<Config.Database> dbs1, List<Config.Database> dbs2, Config.ImportActionMiniModel action)
        {
            Config.ImportActionModel newaction = null;
            if (action != null)
            {
                Config.Database db1 = null;
                Config.Database db2 = null;
                foreach (var a in dbs1)
                {
                    if (a.source + "_" + a.database == action.database1)
                    {
                        db1 = a;
                        break;
                    }
                }

                foreach (var a in dbs2)
                {
                    if (a.source + "_" + a.database == action.database2)
                    {
                        db2 = a;
                        break;
                    }
                }
                if (db1 != null && db2 != null)
                {
                    Config.ImportActionModel model = new Config.ImportActionModel();
                    model.database1 = db1;
                    model.database2 = db2;
                    model.importthreadcount = action.importthreadcount;
                    model.insertpagesize = action.insertpagesize;
                    model.excepttables = action.excepttables;
                    model.getpagesize = action.getpagesize;
                    model.cutomsactions = action.cutomsactions;
                    newaction = model;
                }
            }
            return newaction;
        }

        public static Config.ImportActionMiniModel MigModelToMiniModel(Config.ImportActionModel model)
        {
            Config.ImportActionMiniModel minmodel = new Config.ImportActionMiniModel();
            minmodel.database1 = model.database1.source + "_" + model.database1.database;
            minmodel.database2 = model.database2.source + "_" + model.database2.database;
            minmodel.importthreadcount = model.importthreadcount;
            minmodel.insertpagesize = model.insertpagesize;
            minmodel.cutomsactions = model.cutomsactions;
            minmodel.getpagesize = model.getpagesize;
            minmodel.excepttables = model.excepttables;
            return minmodel;

        }

        public static string StringToSafeString(string stringtext)
        {
            string str_result = "";
            List<byte> safe_bs = new List<byte>();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(stringtext);
            int ccount = 0;
            int b_index = -1;
            int bcount = 0;
             byte cbyte = 0;
            for (int k = 0; k < bs.Length; k++)
            {
                b_index = k;
                bcount = 0;
                cbyte = bs[k];

                if (cbyte < 0xc0)//一个
                {
                    bcount = 1;
                }
                else if (cbyte < 0xe0)//二个
                {
                    k = k + 1;
                    bcount = 2;
                }
                else if (cbyte < 0xf0)//三个
                {
                    k = k + 2;
                    bcount = 3;
                }
                else// if (cbyte <= 0xf0)//四个
                {
                    k = k + 3;
                   bcount = 0;
                }
                for(int x=0;x<bcount;x++)
                {
                    safe_bs.Add(bs[b_index+x]);
                }
                ccount++;
            }
            str_result = System.Text.Encoding.UTF8.GetString(safe_bs.ToArray());
            return str_result;
        }

        //public static DataTable ListProcessModelToTable(List<Config.MigDataProcessModel> listmodel)
        //{
        //    DataTable tb = new DataTable();
        //    tb.Columns.Add(new DataColumn() { ColumnName = "tablename", Caption = "表名", DataType = typeof(string) });
        //    tb.Columns.Add(new DataColumn() { ColumnName = "totalitemcount", Caption = "总记录数", DataType = typeof(string) });
        //    tb.Columns.Add(new DataColumn() { ColumnName = "successcount", Caption = "已处理记录数", DataType = typeof(string) });
        //    tb.Columns.Add(new DataColumn() { ColumnName = "processpercent", Caption = "处理百分比", DataType = typeof(string) });
        //    tb.Columns.Add(new DataColumn() { ColumnName = "remaintime", Caption = "剩余时间", DataType = typeof(string) });
        //    tb.Columns.Add(new DataColumn() { ColumnName = "status", Caption = "状态", DataType = typeof(string) });

        //}
    }
}
