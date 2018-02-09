using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public class DbStructureMySql : IDbStructure
    {
        public List<KVModel> GetDataBases(DbConn dbconn)
        {
            string sql = "select SCHEMA_NAME,DEFAULT_CHARACTER_SET_NAME,DEFAULT_COLLATION_NAME from information_schema.SCHEMATA;";
            DataTable tbofdb = dbconn.SqlToDataTable(sql, null);
            List<KVModel> kvlist = new List<KVModel>();
            foreach (DataRow dr in tbofdb.Rows)
            {
                kvlist.Add(new KVModel()
                {
                    id = dr["SCHEMA_NAME"].ToString(),
                    name = dr["SCHEMA_NAME"].ToString()
                });
            }
            return kvlist;
        }

        public List<KVModel> GetTables(DbConn dbconn)
        {
            var objvalue = dbconn.ExecuteScalar("select database();", null);
            string schemaname = objvalue.ToString();
            string sql = "select table_name from information_schema.tables where table_type='BASE TABLE' and TABLE_SCHEMA = @schemaname ;";
            DataTable tb = dbconn.SqlToDataTable(sql, new { schemaname = schemaname });
            List<KVModel> kvlist = new List<KVModel>();
            foreach (DataRow dr in tb.Rows)
            {
                kvlist.Add(new KVModel()
                {
                    id = dr["table_name"].ToString(),
                    name = dr["table_name"].ToString()
                });
            }
            return kvlist;
        }

        public string GetFieldCheckSql(FieldCheckType checktype, int length, int pricesion, int scale, double minvalue, double maxvalue)
        {
            throw new NotImplementedException();
        }

        public STable GetTableStructure(DbConn dbconn, string tablename)
        {
            throw new NotImplementedException();
        }

        public string ToSqlString(STable table)
        {
            string template = @"
create table {0}
(
{1}
{2}
) engine=InnoDB default charset=utf8;
-- unique keys
{3}
-- default constraints
{4}
-- other indexes(except pk fk uq)
{5}
";
            List<string> list_cols = new List<string>();
            for (int i = 0; i < table.columns.Count; i++)
            {
                if (i == table.columns.Count - 1)
                {
                    list_cols.Add(ColToSqlString(table.columns[i], true) + " -- " + table.columns[i].title);
                }
                else
                {
                    list_cols.Add(ColToSqlString(table.columns[i], true) + " , -- " + table.columns[i].title);
                }
            }
            string str_pk = table.primarykey == null ? "" : PKToSqlString(table.primarykey);
            if (string.IsNullOrEmpty(str_pk))
            {
                if (table.identityattribute.Count > 0)
                {
                    str_pk = IdentityToPKSql(table.identityattribute[0]);
                }
            }
            List<string> list_fk = new List<string>();
            foreach (var a in table.foreignkeys)
            {
                list_fk.Add(FKToSqlString(a));
            }
            if (!string.IsNullOrEmpty(str_pk))
            {
                list_fk.Insert(0, str_pk);
            }

            List<string> list_uq = new List<string>();
            foreach (var a in table.uniquekeys)
            {
                list_uq.Add(UQToSqlString(a));
            }

            List<string> list_df = new List<string>();
            foreach (var a in table.defaultconstraint)
            {
                if (a.column.filedtype.name.Contains("text"))
                    continue;
                list_df.Add(DFToSqlString(a));
            }

            List<string> list_ix = new List<string>();
            foreach (var a in table.indexes)
            {
                if (a.isprimarykey)
                    continue;
                if (a.isuniqueconstraint)
                    continue;
                list_ix.Add(IXToSqlString(a));
            }

            string str_col = string.Join("\r\n", list_cols);
            string str_pk_fk = string.Join(",\r\n", list_fk);
            string str_uq = string.Join("\r\n", list_uq);
            string str_df = string.Join("\r\n", list_df);
            string str_ix = string.Join("\r\n", list_ix);

            string s = string.Format(template, table.name,//0
                  str_col,//1
                  str_pk_fk == "" ? "" : (",\r\n" + str_pk_fk),//2 pk and fk
                  str_uq,//3 uq
                  str_df,// 4 df
                  str_ix// 5
                  );
            return s;
        }

        public string ToSqlString1(STable table, int t)
        {
            string template = "";
            if (t == 1)
            {
                template = @"
create table {0}
(
{1}
) engine=InnoDB default charset=utf8;
-- temp PK
{2}
-- default constraints
{3}
";
            }
            else if (t == 2)
            {
                template = @"
create table {0}
(
{1}
) engine=InnoDB default charset=utf8;
-- temp PK
{2}
-- default constraints
{3}
-- FKs
{4}
-- unique keys
{5}
-- other indexes(except pk fk uq)
{6}
";
            }
            else
            {
                template = @"
create table {0}
(
{1}
) engine=InnoDB default charset=utf8;
-- temp PK
{2}
-- default constraints
{3}
-- FKs
{4}
-- unique keys
{5}
-- other indexes(except pk fk uq)
{6}
-- identity
{7}
-- disable keys
{8}
";
            }
            List<string> list_cols = new List<string>();
            for (int i = 0; i < table.columns.Count; i++)
            {
                if (i == table.columns.Count - 1)
                {
                    list_cols.Add(ColToSqlString(table.columns[i], false) + " -- " + table.columns[i].title);
                }
                else
                {
                    list_cols.Add(ColToSqlString(table.columns[i], false) + " , -- " + table.columns[i].title);
                }
            }
            List<string> list_df = new List<string>();
            foreach (var a in table.defaultconstraint)
            {
                if (a.column.filedtype.name.Contains("text"))
                    continue;
                list_df.Add(DFToSqlString(a));
            }

            string str_col = string.Join("\r\n", list_cols);
            string str_df = string.Join("\r\n", list_df);
            string str_temppk = GetTempPK(table);
            string s = "";
            string str_resumtidentity = "";
            if (table.identityattribute.Count > 0)
            {
                str_resumtidentity = ResumeIdentityCol(table.identityattribute[0]);
            }

            List<string> list_fk = new List<string>();
            foreach (var a in table.foreignkeys)
            {
                list_fk.Add(FKToSqlStringOutter(a));
            }

            List<string> list_uq = new List<string>();
            foreach (var a in table.uniquekeys)
            {
                list_uq.Add(UQToSqlString(a));
            }

            List<string> list_ix = new List<string>();
            foreach (var a in table.indexes)
            {
                if (a.isprimarykey)
                    continue;
                if (a.isuniqueconstraint)
                    continue;
                list_ix.Add(IXToSqlString(a));
            }
            string str_disablekey = " ALTER TABLE " + table.name + " DISABLE KEYS;";

            string str_fk = string.Join(",\r\n", list_fk);
            string str_uq = string.Join("\r\n", list_uq);
            string str_ix = string.Join("\r\n", list_ix);
            s = string.Format(template, table.name,//0
                   str_col,//1
                   str_temppk,
                   str_df,// 2 df
                str_fk,
              str_uq,//3 uq
              str_ix,// 5
               str_resumtidentity,
               str_disablekey
                   );
            return s;
        }
        public string ToSqlString2(STable table, int t)
        {

            string template = "";
            if (t == 1)
            {
                template = @"
-- ================{0}
-- Resume Identity
{1}
-- FKs
{2}
-- unique keys
{3}
-- other indexes(except pk fk uq)
{4}
";
            }
            else if (t == 2)
            {
                template = @"
-- ================{0}
-- Resume Identity
{1}
";
            }
            else
            {
                template = @"
-- ================{0}
-- enable keys 
{1}
";
            }
            //string str_pk = table.primarykey == null ? "" : PKToSqlStringOutter(table.primarykey);
            //if (string.IsNullOrEmpty(str_pk))
            //{
            //    if (table.identityattribute.Count > 0)
            //    {
            //        str_pk = IdentityToPKSqlOutter(table.identityattribute[0]);
            //    }
            //}
            List<string> list_fk = new List<string>();
            foreach (var a in table.foreignkeys)
            {
                list_fk.Add(FKToSqlStringOutter(a));
            }

            List<string> list_uq = new List<string>();
            foreach (var a in table.uniquekeys)
            {
                list_uq.Add(UQToSqlString(a));
            }

            List<string> list_ix = new List<string>();
            foreach (var a in table.indexes)
            {
                if (a.isprimarykey)
                    continue;
                if (a.isuniqueconstraint)
                    continue;
                list_ix.Add(IXToSqlString(a));
            }

            string str_fk = string.Join(",\r\n", list_fk);
            string str_uq = string.Join("\r\n", list_uq);
            string str_ix = string.Join("\r\n", list_ix);
            // string str_droptemppk = DelTempPk(table);
            string str_resumtidentity = "";
            if (table.identityattribute.Count > 0)
            {
                str_resumtidentity = ResumeIdentityCol(table.identityattribute[0]);
            }
            string s = "";
            string str_enablekey = " ALTER TABLE " + table.name + " ENABLE KEYS;";
            if (t == 1)
            {
                s = string.Format(template,
                    table.name,//0
                    str_resumtidentity,
                str_fk,
                    str_uq,//3 uq
                    str_ix// 5
                    );
            }
            else if (t == 2)
            {
                s = string.Format(template,
                    table.name,//0
                    str_resumtidentity
                    );
            }
            else
            {
                s = string.Format(template,
                    table.name,//0
                    str_enablekey
                    );
            }
            return s;
        }


        public string ColToSqlString(SColumn col, bool containidentity = true)
        {
            string templ = "{0} {1} {2} {3}  {4}";
            string s = string.Format(templ, col.name, col.filedtype.ToSqlString(), col.allownull ? "null" : "not null",
                (containidentity && col.isidentity) ? ("AUTO_INCREMENT") : "",
                string.IsNullOrEmpty(col.description) ? "" : ("COMMENT '" + col.description.Replace("'", "\\'") + "' ")
                );
            return s;
        }
        public string PKToSqlString(SPrimaryKey pk)
        {
            string talbename = pk.table.name;
            List<string> pkcloses = new List<string>();
            foreach (var a in pk.columns)
            {
                pkcloses.Add(a.name);
            }
            //bool hasclusteredindex = pk.table.indexes.Count(x => x.isclustered) == 1;
            string template = " primary key ({0})";
            string s = string.Format(template, string.Join(",", pkcloses));
            return s;
        }


        private string PKToSqlStringOutter(SPrimaryKey pk)
        {
            string talbename = pk.table.name;
            List<string> pkcloses = new List<string>();
            foreach (var a in pk.columns)
            {
                pkcloses.Add(a.name);
            }
            //bool hasclusteredindex = pk.table.indexes.Count(x => x.isclustered) == 1;
            string template = " alter table {0} add constraint PK_{0} primary key({1});";
            string s = string.Format(template, pk.table.name, string.Join(",", pkcloses));
            return s;
        }

        private string IdentityToPKSql(SIdentityAttribute id)
        {
            string template = " primary key ({0})";
            string s = string.Format(template, id.column.name);
            return s;
        }

        private string IdentityToPKSqlOutter(SIdentityAttribute id)
        {
            string template = " alter table {0} add constraint PK_{0} primary key({1});";
            string s = string.Format(template, id.column.table.name, id.column.name);
            return s;
        }

        private string ResumeIdentityCol(SIdentityAttribute id)
        {
            string template = " alter table {0} change {1} {2};";
            string s = string.Format(template, id.column.table.name, id.column.name, ColToSqlString(id.column, true));
            return s;
        }

        public string FKToSqlString(SForeignKey fk)
        {
            List<string> colcurr = new List<string>();
            List<string> colref = new List<string>();
            foreach (var a in fk.columns)
            {
                colcurr.Add(a.Key.name);
                colref.Add(a.Value);
            }
            //foreign key({0}) references {1}({2}) on delete cascade on update cascade
            string template = "foreign key({0}) references {1}({2}) on delete cascade on update cascade";
            string s = string.Format(template, string.Join("_", colcurr), fk.tablename, string.Join(",", colref));
            return s;
        }


        private string FKToSqlStringOutter(SForeignKey fk)
        {
            List<string> colcurr = new List<string>();
            List<string> colref = new List<string>();
            foreach (var a in fk.columns)
            {
                colcurr.Add(a.Key.name);
                colref.Add(a.Value);
            }
            //foreign key({0}) references {1}({2}) on delete cascade on update cascade
            string template = "alter table {0} add constraint foreign key({1}) references {2}({3}) on delete cascade on update cascade;";
            string s = string.Format(template, fk.table.name, string.Join("_", colcurr), fk.tablename, string.Join(",", colref));
            return s;
        }

        public string UQToSqlString(SUniqueKey uq)
        {
            List<string> colcurr = new List<string>();
            foreach (var a in uq.columns)
            {
                colcurr.Add(a.name);
            }
            string template = "alter table {0} add  constraint UQ_{0}_{1} unique ({2});";
            string s = string.Format(template, uq.table.name, string.Join("_", colcurr), string.Join(",", colcurr));
            return s;
        }

        public string DFToSqlString(SDefaultConstraint df)
        {
            df.defaultvalue = df.defaultvalue.Trim();
            if (df.defaultvalue.StartsWith("(") && df.defaultvalue.EndsWith(")"))
            {
                while (df.defaultvalue.StartsWith("(") && df.defaultvalue.EndsWith(")"))
                {
                    if (df.defaultvalue.Length == 2)
                        df.defaultvalue = "";
                    else
                        df.defaultvalue = df.defaultvalue.Substring(1, df.defaultvalue.Length - 2);
                    if (df.defaultvalue.IndexOf(')') < df.defaultvalue.IndexOf('('))
                    {
                        df.defaultvalue = "(" + df.defaultvalue + ")";
                        break;
                    }
                }
            }
            if (df.defaultvalue.ToLower() == "getdate()")
            {
                df.defaultvalue = "'1900-01-01 00:00:00'";
            }


            if (!(df.defaultvalue.StartsWith("'") && df.defaultvalue.EndsWith("'")))
            {
                df.defaultvalue = "'" + df.defaultvalue + "'";
            }
            if (df.defaultvalue.Substring(0, df.defaultvalue.Length - 2) == "" && !df.column.filedtype.canemptydefault)
            {
                df.defaultvalue = "'" + df.column.filedtype.defaultvalue + "'";
            }
            // alter table表名 alter column字段名 set default默认值;
            string template = "alter table {0} alter column {1} set default {2} ;";
            string s = string.Format(template, df.column.table.name, df.column.name, df.defaultvalue);
            return s;
        }

        public string IXToSqlString(SIndex index)
        {
            List<string> colcurr = new List<string>();
            List<string> colcurrwitho = new List<string>();
            foreach (var a in index.columns)
            {
                colcurr.Add(a.Key.name);
                colcurrwitho.Add(a.Key.name + " " + (a.Value == 1 ? "desc" : "asc"));
            }
            string indexname = string.Empty;
            if (index.isprimarykey)
            {
                indexname = "PK_" + index.table.name + "_";
            }
            else if (index.isuniqueconstraint)
            {
                indexname = string.Format("UQ_{0}_{1}", index.table.name, string.Join("_", colcurr));
            }
            else
            {
                string tname = string.Join("_", colcurr);
                if (tname.Length > 30)
                {
                    // string tnames = Math.Abs(tname.GetHashCode()).ToString();
                    string rname = "";
                    for (int k = 0; k < 15; k++)
                    {
                        Random rd = new Random(k + 1 + DateTime.Now.Millisecond);
                        rname += ((char)rd.Next(97, 123)).ToString();
                        System.Threading.Thread.Sleep(1);
                    }
                    tname = rname;
                }
                indexname = string.Format("IX_{0}_{1}", index.table.name, tname);
            }

            string template = "create {0} index  {1} on {2}({3});";
            string s = string.Format(template, "", indexname, index.table.name, string.Join(",", colcurrwitho));
            return s;
        }



        public void ToLocalTable(STable other, List<SqlToSqlModel> sqltypemodel, bool autofieldtypechange)
        {
            foreach (var a in other.columns)
            {
                bool exist = false;
                foreach (var b in sqltypemodel)
                {
                    if (a.filedtype.name == b.typename1)
                    {
                        a.filedtype = DbFieldTypeHelper.MySqlGetFieldType(other.name, a.name, b.typename2, a.filedtype.precision, a.filedtype.scale, a.filedtype.maxlength, autofieldtypechange);
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                    throw new Exception(a.filedtype.name + "不存在转化");
            }
            List<DbStructure.SIndex> removeindexes = new List<SIndex>();
            for (int i = 0; i < other.indexes.Count; i++)
            {
                for (int j = i + 1; j < other.indexes.Count; j++)
                {
                    bool rowissame = true;
                    if (other.indexes[i].columns.Count == other.indexes[j].columns.Count)
                    {
                        foreach (var a in other.indexes[i].columns)
                        {
                            if (!other.indexes[j].columns.ContainsKey(a.Key))
                            {
                                rowissame = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        rowissame = false;
                    }
                    if (rowissame)
                    {
                        if (other.indexes[j].isprimarykey || other.indexes[j].isunique)
                        {
                            if (!removeindexes.Contains(other.indexes[i]))
                                removeindexes.Add(other.indexes[i]);
                        }
                        else
                        {
                            if (!removeindexes.Contains(other.indexes[j]))
                                removeindexes.Add(other.indexes[j]);
                        }
                    }
                }
            }
            foreach (var a in removeindexes)
            {
                other.indexes.Remove(a);
            }
        }



        public string GetTempPK(STable table)
        {
            if (table.identityattribute.Count != 0)
            {
                return IdentityToPKSqlOutter(table.identityattribute[0]);
            }
            else if (table.primarykey != null)
            {
                return PKToSqlStringOutter(table.primarykey);
            }
            else
            {
                return "";
            }
        }


        public string DelTempPk(STable table)
        {
            string pkname = "";
            if (table.identityattribute.Count != 0)
            {
                pkname = "PK_" + table.name;
            }
            else if (table.primarykey != null)
            {
                pkname = "PK_" + table.name;
            }
            if (pkname == "")
                return "";
            else
            {
                pkname = "Alter table " + table.name + " drop primary key;";
                return pkname;
            }
        }
    }
}
