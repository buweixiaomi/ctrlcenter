using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public class DbStructureSqlServer : IDbStructure
    {
        public List<KVModel> GetDataBases(DbConn dbconn)
        {
            string sql = "select  database_id as id,name from master.sys.databases ";
            DataTable tb = dbconn.SqlToDataTable(sql, null);
            List<KVModel> rs = new List<KVModel>();
            foreach (DataRow dr in tb.Rows)
            {
                rs.Add(new KVModel()
                {
                    id = dr["id"].ToString(),
                    name = dr["name"].ToString()
                });
            }
            return rs;
        }

        public List<KVModel> GetTables(DbConn dbconn)
        {
            string sql = "select name,object_id as id from sys.all_objects where type='U' and is_ms_shipped=0 order by name ";
            DataTable tb = dbconn.SqlToDataTable(sql, null);
            List<KVModel> rs = new List<KVModel>();
            foreach (DataRow dr in tb.Rows)
            {
                rs.Add(new KVModel()
                {
                    id = dr["id"].ToString(),
                    name = dr["name"].ToString()
                });
            }
            return rs;
        }

        public STable GetTableStructure(DbConn dbconn, string tablename)
        {
            string sql = " select top 1 object_id from sys.tables where name=@name ";
            var tbid = dbconn.ExecuteScalar(sql, new { name = tablename });

            string tableid = tbid.ToString();
            STable table = new STable();
            table.columns = GetBaseColumns(dbconn, tableid);//列
            table.name = tablename;
            foreach (var a in table.columns)
                a.table = table;
            table.primarykey = GetPrimaryKey(dbconn, tableid, table.columns);
            table.foreignkeys = GetForeignKeys(dbconn, tableid, table.columns);
            table.uniquekeys = GetUniqueKeys(dbconn, tableid, table.columns);
            table.defaultconstraint = GetDefaultConstraints(dbconn, tableid, table.columns);
            table.identityattribute = GetIdentityAttributes(dbconn, tableid, table.columns);
            table.indexes = GetIndexes(dbconn, tableid, table.columns);

            return table;
        }

        private List<SColumn> GetBaseColumns(DbConn dbconn, string tableid)
        {
            string sql = @"select SC.object_id,SC.name,SC.column_id,SC.user_type_id,ST.name as typename,
                                    SC.max_length,SC.precision,SC.scale,SC.is_nullable,
                                    SC.is_identity,IC.seed_value,IC.increment_value,IC.last_value,
                                    SC.default_object_id,DFC.definition,EP.value as des
                                    from sys.all_columns   SC 
                                    left join sys.types ST on SC.user_type_id=ST.user_type_id
                                    left join sys.identity_columns IC on SC.object_id=IC.object_id and SC.column_id=IC.column_id
                                    left join sys.default_constraints DFC on SC.default_object_id=DFC.object_id
                                    left join sys.extended_properties EP on EP.minor_id=SC.column_id and SC.object_id=EP.major_id and EP.name='MS_Description'
                                    where SC.object_id=@objid";
            DataTable tbcolumns = dbconn.SqlToDataTable(sql, new { objid = tableid });
            List<SColumn> columns = new List<SColumn>();
            foreach (DataRow dr in tbcolumns.Rows)
            {
                string desc = dr["des"].ToString();
                FieldType fieldtype = DbFieldTypeHelper.SqlServerGetFieldType(
                    dr["typename"].ToString(),
                    Convert.ToInt32(dr["precision"]),
                    Convert.ToInt32(dr["scale"]),
                    Convert.ToInt32(dr["max_length"]));
                columns.Add(new SColumn()
                {
                    name = dr["name"].ToString(),
                    allownull = LibConvert.ObjToBool(dr["is_nullable"]),
                    description = desc,
                    title = string.IsNullOrEmpty(desc) ? dr["name"].ToString() : desc,
                    isidentity = LibConvert.ObjToBool("is_identity"),
                    filedtype = fieldtype
                });
            }
            return columns;
        }

        private SPrimaryKey GetPrimaryKey(DbConn dbconn, string tableid, List<SColumn> columns)
        {
            string sql1 = "  select name,object_id,parent_object_id from sys.key_constraints where type='PK' and parent_object_id=@objid";
            DataTable tbpks = dbconn.SqlToDataTable(sql1, new { objid = tableid });
            if (tbpks.Rows.Count == 0)
                return null;
            if (tbpks.Rows.Count > 1)
            {
                throw new Exception("非法表");
            }
            SPrimaryKey pk = new SPrimaryKey();
            pk.name = tbpks.Rows[0]["name"].ToString();
            string sql2 = @" select IC.object_id,IC.column_id,IC.index_column_id,IC.index_id,AC.name as columnname,KC.name from sys.index_columns IC
                                     left join sys.all_columns AC on IC.object_id=AC.object_id and IC.column_id=AC.column_id
                                     left join sys.indexes ID on IC.object_id=ID.object_id and IC.index_id=ID.index_id
                                     left join sys.key_constraints KC on ID.name=KC.name and KC.type='PK'
                                     where KC.parent_object_id=@objid";
            DataTable tbpkcols = dbconn.SqlToDataTable(sql2, new { objid = tableid });
            if (tbpkcols.Rows.Count == 0)
                return null;
            foreach (DataRow dr in tbpkcols.Rows)
            {
                string cn = dr["columnname"].ToString();
                var c = columns.FirstOrDefault(x => x.name == cn);
                if (c == null)
                    throw new Exception(cn + "不在表中");
                c.primarykey = pk;
                c.isprimarykey = true;
                pk.columns.Add(c);
                pk.table = c.table;
            }
            return pk;
        }

        private List<SForeignKey> GetForeignKeys(DbConn dbconn, string tableid, List<SColumn> columns)
        {
            string sql1 = " select name,object_id,parent_object_id from sys.foreign_keys where parent_object_id=@objid";
            DataTable tbforkeys = dbconn.SqlToDataTable(sql1, new { objid = tableid });
            List<SForeignKey> forkeys = new List<SForeignKey>();
            foreach (DataRow dr in tbforkeys.Rows)
            {
                forkeys.Add(new SForeignKey() { name = dr["name"].ToString() });
            }
            string sql2 = @" select FK.name,FK.object_id, FKC.parent_object_id,FKC.parent_column_id,AC1.name as parent_columnname,
                             O.name as referenced_table_name,
                             FKC.referenced_object_id,FKC.referenced_column_id,AC2.name as referenced_columnname
                             from sys.foreign_key_columns FKC
                             left join sys.objects O on FKC.referenced_object_id=O.object_id
                             left join sys.all_columns AC1 on FKC.parent_column_id=AC1.column_id and FKC.parent_object_id=AC1.object_id
                             left join sys.all_columns AC2 on FKC.referenced_column_id=AC2.column_id and FKC.referenced_object_id=AC2.object_id
                             left join sys.foreign_keys FK on FKC.constraint_object_id=FK.object_id
                             where FK.parent_object_id=@objid and FK.name=@name";
            foreach (var a in forkeys)
            {
                a.columns = new Dictionary<SColumn, string>();
                DataTable tbforcolumns = dbconn.SqlToDataTable(sql2, new { objid = tableid, name = a.name });
                foreach (DataRow dr in tbforcolumns.Rows)
                {
                    string currname = dr["parent_columnname"].ToString();
                    string refname = dr["referenced_columnname"].ToString();
                    string reftablename = dr["referenced_table_name"].ToString();
                    var c = columns.FirstOrDefault(x => x.name == currname);
                    if (c == null)
                        throw new Exception(currname + "不在表中");
                    c.foreignkeys.Add(a);
                    a.columns.Add(c, refname);
                    a.tablename = reftablename;
                    a.table = c.table;
                }
            }
            return forkeys;
        }

        private List<SUniqueKey> GetUniqueKeys(DbConn dbconn, string tableid, List<SColumn> columns)
        {
            string sql1 = "select parent_object_id,object_id,name from sys.key_constraints where parent_object_id=@objid and type='UQ'";
            DataTable tbukeys = dbconn.SqlToDataTable(sql1, new { objid = tableid });
            List<SUniqueKey> ukeys = new List<SUniqueKey>();
            foreach (DataRow dr in tbukeys.Rows)
            {
                ukeys.Add(new SUniqueKey() { name = dr["name"].ToString() });
            }
            foreach (var a in ukeys)
            {
                string sql2 = @" select IC.object_id,IC.column_id,IC.index_column_id,IC.index_id,AC.name as columnname,KC.name from sys.index_columns IC
                                     left join sys.all_columns AC on IC.object_id=AC.object_id and IC.column_id=AC.column_id
                                     left join sys.indexes ID on IC.object_id=ID.object_id and IC.index_id=ID.index_id
                                     left join sys.key_constraints KC on ID.name=KC.name and KC.type='UQ'
                                     where KC.parent_object_id=@objid and KC.name=@name ";
                DataTable tbucolumns = dbconn.SqlToDataTable(sql2, new { objid = tableid, name = a.name });
                foreach (DataRow dr in tbucolumns.Rows)
                {
                    string cn = dr["columnname"].ToString();
                    var r = columns.FirstOrDefault(x => x.name == cn);
                    if (r == null)
                        throw new Exception(cn + "列在表中不存在");
                    r.uniquekeys.Add(a);
                    a.columns.Add(r);
                    a.table = r.table;
                }
            }
            return ukeys;
        }

        private List<SDefaultConstraint> GetDefaultConstraints(DbConn dbconn, string tableid, List<SColumn> columns)
        {
            string sql1 = @"select DC.name,DC.object_id,DC.parent_column_id,DC.definition,DC.parent_column_id, AC.name as columnname from sys.default_constraints DC
                            left join sys.all_columns AC on DC.parent_object_id=AC.object_id and DC.parent_column_id=AC.column_id
                             where DC.parent_object_id=@objid";
            DataTable tb = dbconn.SqlToDataTable(sql1, new { objid = tableid });
            List<SDefaultConstraint> cons = new List<SDefaultConstraint>();
            foreach (DataRow dr in tb.Rows)
            {
                string cn = dr["columnname"].ToString();
                var c = columns.FirstOrDefault(x => x.name == cn);
                if (c == null)
                    throw new Exception(cn + "列在表中不存在");
                var defattr = new SDefaultConstraint()
                {
                    column = c,
                    defaultvalue = dr["definition"].ToString()
                };
                c.defaultconstraint = defattr;
                cons.Add(defattr);
            }
            return cons;
        }

        private List<SIdentityAttribute> GetIdentityAttributes(DbConn dbconn, string tableid, List<SColumn> columns)
        {
            string sql1 = "select object_id,name,column_id,seed_value,increment_value,last_value from sys.identity_columns where object_id=@objid";
            DataTable tb = dbconn.SqlToDataTable(sql1, new { objid = tableid });
            List<SIdentityAttribute> identitys = new List<SIdentityAttribute>();
            foreach (DataRow dr in tb.Rows)
            {
                string cn = dr["name"].ToString();
                var r = columns.FirstOrDefault(x => x.name == cn);
                if (r == null)
                {
                    throw new Exception(cn + "在表中不存在");
                }
                var iden = new SIdentityAttribute()
                {
                    column = r,
                    send = Convert.ToInt64(dr["seed_value"]),
                    crement = Convert.ToInt64(dr["increment_value"]),
                    lastvalue = (dr["last_value"] == null || dr["last_value"].ToString() == "") ? (Convert.ToInt64(dr["seed_value"]) - 1) : Convert.ToInt64(dr["last_value"])
                };
                r.isidentity = true;
                r.identityattribute = iden;
                identitys.Add(iden);
            }
            return identitys;
        }

        private List<SIndex> GetIndexes(DbConn dbconn, string tableid, List<SColumn> columns)
        {
            string sql1 = "select object_id,name,index_id,type,is_unique,is_primary_key,is_unique_constraint from sys.indexes where [object_id]=@objid and [type]>0";
            DataTable tbindexes = dbconn.SqlToDataTable(sql1, new { objid = tableid });
            List<SIndex> indexes = new List<SIndex>();
            foreach (DataRow dr in tbindexes.Rows)
            {
                indexes.Add(new SIndex()
                {
                    indexid = Convert.ToInt32(dr["index_id"]),
                    isclustered = Convert.ToInt32(dr["type"]) == 1,
                    isprimarykey = Convert.ToInt32(dr["is_primary_key"]) == 1,
                    isunique = Convert.ToInt32(dr["is_unique"]) == 1,
                    isuniqueconstraint = Convert.ToInt32(dr["is_unique_constraint"]) == 1,
                    name = dr["name"].ToString()
                });
            }

            string sql2 = @"select  IC.column_id,IC.object_id,AC.name,IC.is_descending_key  from sys.index_columns  IC
                                    left join sys.all_columns AC on IC.object_id=AC.object_id and IC.column_id=AC.column_id
                                     where index_id=@indexid and IC.object_id=@objid
                                     order by IC.column_id asc ";
            foreach (var a in indexes)
            {
                DataTable tbindexcolumns = dbconn.SqlToDataTable(sql2, new { objid = tableid, indexid = a.indexid });
                a.columns = new Dictionary<SColumn, int>();
                foreach (DataRow dr in tbindexcolumns.Rows)
                {
                    string cn = dr["name"].ToString();
                    int isdesc = Convert.ToInt32(dr["is_descending_key"]);
                    var r = columns.FirstOrDefault(x => x.name == cn);
                    if (r == null)
                    {
                        throw new Exception(cn + "列不在所有列中,请检查代码");
                    }
                    a.table = r.table;
                    r.indexes.Add(a);
                    a.columns.Add(r, isdesc);
                }
            }
            return indexes;
        }

        public string GetFieldCheckSql(FieldCheckType checktype, int length, int pricesion, int scale, double minvalue, double maxvalue)
        {
            string s = "";
            switch (checktype)
            {
                case FieldCheckType.Length:
                    break;
                case FieldCheckType.PrecisionAndScale:
                    break;
                case FieldCheckType.Range:
                    break;
                default:
                    break;
            }
            return s;
        }


        public string ToSqlString(STable table)
        {
            string template = @"
create table {0}
(
{1}
)
-- descriptions
{2}

-- primary key
{3}

-- foreign keys
{4}

-- unique keys
{5}

-- default constraints
{6}

-- other indexes(except pk fk uq)
{7}
";
            List<string> list_cols = new List<string>();
            for (int i = 0; i < table.columns.Count; i++)
            {
                if (i == table.columns.Count - 1)
                {
                    list_cols.Add(ColToSqlString(table.columns[i]) + " -- " + table.columns[i].title);
                }
                else
                {
                    list_cols.Add(ColToSqlString(table.columns[i]) + " , -- " + table.columns[i].title);
                }
            }
            string str_pk = table.primarykey == null ? "" : PKToSqlString(table.primarykey);
            List<string> list_fk = new List<string>();
            foreach (var a in table.foreignkeys)
            {
                list_fk.Add(FKToSqlString(a));
            }

            List<string> list_uq = new List<string>();
            foreach (var a in table.uniquekeys)
            {
                list_uq.Add(UQToSqlString(a));
            }

            List<string> list_df = new List<string>();
            foreach (var a in table.defaultconstraint)
            {
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
            string str_fk = string.Join("\r\n", list_fk);
            string str_uq = string.Join("\r\n", list_uq);
            string str_df = string.Join("\r\n", list_df);
            string str_ix = string.Join("\r\n", list_ix);

            string s = string.Format(template, table.name,//0
                  str_col,//1
                  "",//2 desc
                  str_pk,//3 pk
                  str_fk,//4 fk
                  str_uq,//5 uq
                  str_df,// 6 df
                  str_ix// 7
                  );
            return s;
        }

        public string ToSqlString1(STable table, int t)
        {
            string template = @"
create table {0}
(
{1}
)
-- descriptions
{2}
-- default constraints
{3}

";
            List<string> list_cols = new List<string>();
            for (int i = 0; i < table.columns.Count; i++)
            {
                if (i == table.columns.Count - 1)
                {
                    list_cols.Add(ColToSqlString(table.columns[i]) + " -- " + table.columns[i].title);
                }
                else
                {
                    list_cols.Add(ColToSqlString(table.columns[i]) + " , -- " + table.columns[i].title);
                }
            }

            List<string> list_df = new List<string>();
            foreach (var a in table.defaultconstraint)
            {
                list_df.Add(DFToSqlString(a));
            }


            string str_col = string.Join("\r\n", list_cols);
            string str_df = string.Join("\r\n", list_df);

            string s = string.Format(template, table.name,//0
                  str_col,//1
                  "",//2
                  str_df// 3 df
                  );
            return s;
        }
        public string ToSqlString2(STable table, int t)
        {

            string template = @"
-- ============={0}
-- primary key
{1}
-- foreign keys
{2}
-- unique keys
{3}
-- other indexes(except pk fk uq)
{5}
";

            string str_pk = table.primarykey == null ? "" : PKToSqlString(table.primarykey);
            List<string> list_fk = new List<string>();
            foreach (var a in table.foreignkeys)
            {
                list_fk.Add(FKToSqlString(a));
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

            string str_fk = string.Join("\r\n", list_fk);
            string str_uq = string.Join("\r\n", list_uq);
            string str_ix = string.Join("\r\n", list_ix);

            string s = string.Format(template, table.name,
                  str_pk,//3 pk
                  str_fk,//4 fk
                  str_uq,//5 uq
                  str_ix// 7
                  );
            return s;
        }

        public string ColToSqlString(SColumn col, bool containidentity = true)
        {
            string templ = "{0} {1} {2} {3}";
            string s = string.Format(templ, col.name, col.filedtype.ToSqlString(), col.allownull ? "null" : "not null",
                col.isidentity ? ("identity(" + col.identityattribute.send + "," + col.identityattribute.crement + ")") : ""
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
            bool hasclusteredindex = pk.table.indexes.Count(x => x.isclustered) == 1;
            string template = "alter table {0} add constraint PK_{0} primary key {1} ({2});";
            string s = string.Format(template, talbename, hasclusteredindex ? "nonclustered" : "clustered", string.Join(",", pkcloses));
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
            string template = "alter table {0} add constraint FK_{0}_{1}_{2} foreign key references {1}({3});";
            string s = string.Format(template, fk.table.name, fk.tablename, string.Join("_", colcurr), string.Join(",", colref));
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
            string template = "alter table {0} add constraint DF_{0}_{1} default {2} for {1} ;";
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
                indexname = string.Format("IX_{0}_{1}", index.table.name, string.Join("_", colcurr));
            }

            string template = "create {0} index  {1} on {2}({3});";
            string s = string.Format(template, index.isclustered ? "clustered" : "nonclustered", indexname, index.table.name, string.Join(",", colcurrwitho));
            return s;
        }


        public void ToLocalTable(STable other, List<SqlToSqlModel> sqltypemodel, bool autofieldtypechange)
        {
            foreach (var a in other.columns)
            {
                foreach (var b in sqltypemodel)
                {
                    if (a.filedtype.name == b.typename1)
                    {
                        a.filedtype = DbFieldTypeHelper.SqlServerGetFieldType(b.typename2, a.filedtype.precision, a.filedtype.scale, a.filedtype.maxlength);
                        break;
                    }
                }
                throw new Exception(a.filedtype.name + "不存在转化");
            }
        }


        public string GetTempPK(STable table)
        {
            throw new NotImplementedException();
        }


        public string DelTempPk(STable table)
        {
            throw new NotImplementedException();
        }
    }
}
