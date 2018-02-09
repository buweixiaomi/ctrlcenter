using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public class DbFieldTypeHelper
    {

        public readonly static List<SqlToSqlModel> sqltomysqlnormaltrans = new List<SqlToSqlModel>();
        public readonly static List<SqlToSqlModel> sqltomysqlsafetrans = new List<SqlToSqlModel>();
        static DbFieldTypeHelper()
        {
            //===normal begin
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "bit", typename2 = "tinyint", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "tinyint", typename2 = "smallint", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "smallint", typename2 = "smallint", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "int", typename2 = "int", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "bigint", typename2 = "bigint", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "float", typename2 = "float", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "decimal", typename2 = "decimal", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "numeric", typename2 = "decimal", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "real", typename2 = "double", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "money", typename2 = "decimal", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "char", typename2 = "char", checktype = FieldCheckType.Length });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "varchar", typename2 = "varchar", checktype = FieldCheckType.Length });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "nchar", typename2 = "char", checktype = FieldCheckType.Length });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "nvarchar", typename2 = "varchar", checktype = FieldCheckType.Length });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "text", typename2 = "longtext", checktype = 0 });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "ntext", typename2 = "longtext", checktype = 0 });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "binary", typename2 = "binary", checktype = FieldCheckType.Length });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "varbinary", typename2 = "varbinary", checktype = FieldCheckType.Length });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "image", typename2 = "longblob", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "date", typename2 = "date", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "time", typename2 = "time", checktype = FieldCheckType.None });
            sqltomysqlnormaltrans.Add(new SqlToSqlModel() { typename1 = "datetime", typename2 = "datetime", checktype = FieldCheckType.None });
            //===normal end

            //==safe begin
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "bit", typename2 = "tinyint", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "tinyint", typename2 = "smallint", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "smallint", typename2 = "smallint", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "int", typename2 = "int", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "bigint", typename2 = "bigint", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "float", typename2 = "float", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "decimal", typename2 = "decimal", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "numeric", typename2 = "decimal", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "real", typename2 = "double", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "money", typename2 = "decimal", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "char", typename2 = "longtext", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "varchar", typename2 = "longtext", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "nchar", typename2 = "longtext", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "nvarchar", typename2 = "longtext", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "text", typename2 = "longtext", checktype = 0 });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "ntext", typename2 = "longtext", checktype = 0 });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "binary", typename2 = "longblob", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "varbinary", typename2 = "longblob", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "image", typename2 = "longblob", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "date", typename2 = "date", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "time", typename2 = "time", checktype = FieldCheckType.None });
            sqltomysqlsafetrans.Add(new SqlToSqlModel() { typename1 = "datetime", typename2 = "datetime", checktype = FieldCheckType.None });
            //== safe end

        }

        public static FieldType SqlServerGetFieldType(string typename, int precsion, int scale, int maxlength)
        {

            typename = typename.ToLower();
            FieldType ft = null;
            switch (typename)
            {
                case "bit":
                    ft = new FieldSqlServer.Bit();
                    break;
                case "tinyint":
                    ft = new FieldSqlServer.Tinyint();
                    break;

                case "smallint":
                    ft = new FieldSqlServer.Smalint();
                    break;

                case "int":
                    ft = new FieldSqlServer.Int();
                    break;
                case "bigint":
                    ft = new FieldSqlServer.Bigint();
                    break;
                case "float":
                    ft = new FieldSqlServer.Float();
                    break;
                case "decimal":
                    ft = new FieldSqlServer.Decimal(precsion, scale);
                    break;
                case "numeric":
                    ft = new FieldSqlServer.Numeric(precsion, scale);
                    break;
                case "real":
                    ft = new FieldSqlServer.Real();
                    break;
                case "money":
                    ft = new FieldSqlServer.Money();
                    break;
                case "char":
                    ft = new FieldSqlServer.Char(maxlength);
                    break;
                case "varchar":
                    ft = new FieldSqlServer.Varchar(maxlength);
                    break;
                case "nchar":
                    ft = new FieldSqlServer.Nchar(maxlength / 2);
                    break;
                case "nvarchar":
                    ft = new FieldSqlServer.Nvarchar(maxlength / 2);
                    break;
                case "text":
                    ft = new FieldSqlServer.Text();
                    break;
                case "ntext":
                    ft = new FieldSqlServer.Ntext();
                    break;
                case "binary":
                    ft = new FieldSqlServer.Binary(maxlength);
                    break;
                case "varbinary":
                    ft = new FieldSqlServer.Varbinary(maxlength);
                    break;
                case "image":
                    ft = new FieldSqlServer.Image();
                    break;
                case "date":
                    ft = new FieldSqlServer.Date();
                    break;
                case "time":
                    ft = new FieldSqlServer.Time();
                    break;
                case "datetime":
                    ft = new FieldSqlServer.Datetime();
                    break;
                default:
                    throw new Exception(typename + "类型暂不支持转化");
            }
            return ft;
        }


        public static FieldType MySqlGetFieldType(string tablename, string fieldname, string typename, int precsion, int scale, int maxlength, bool autochange)
        {

            typename = typename.ToLower();
            if (autochange)
            {
                string oldtype = typename;
                if (typename == "char" && maxlength > 255)
                {
                    typename = "varchar";
                }
                if (typename.Contains("char") && maxlength == -1)
                {
                    typename = "text";
                }
                if (oldtype != typename)
                {
                    Config.Log.Alert("表{0} 字段{1} 类型从{2}自动改为{3}", tablename, fieldname, oldtype, typename);
                }
            }
            FieldType ft = null;
            switch (typename)
            {
                case "tinyint":
                    ft = new FieldMySql.Tinyint();
                    break;
                case "smallint":
                    ft = new FieldMySql.Smalint();
                    break;
                case "int":
                    ft = new FieldMySql.Int();
                    break;
                case "bigint":
                    ft = new FieldMySql.Bigint();
                    break;
                case "float":
                    ft = new FieldMySql.Float();
                    break;
                case "double":
                    ft = new FieldMySql.Double();
                    break;
                case "decimal":
                    ft = new FieldMySql.Decimal(precsion, scale);
                    break;
                case "char":
                    ft = new FieldMySql.Char(maxlength);
                    break;
                case "varchar":
                    ft = new FieldMySql.Varchar(maxlength);
                    break;
                case "tinytext":
                    ft = new FieldMySql.Tinytext();
                    break;
                case "text":
                    ft = new FieldMySql.Text();
                    break;
                case "mediumtext":
                    ft = new FieldMySql.Mediumtext();
                    break;
                case "longtext":
                    ft = new FieldMySql.Longtext();
                    break;
                case "binary":
                    ft = new FieldMySql.Binary(maxlength);
                    break;
                case "varbinary":
                    ft = new FieldMySql.Varbinary(maxlength);
                    break;
                case "tinyblob":
                    ft = new FieldMySql.Tinyblob();
                    break;
                case "blob":
                    ft = new FieldMySql.Blob();
                    break;
                case "mediumblob":
                    ft = new FieldMySql.Mediumblob();
                    break;
                case "longblob":
                    ft = new FieldMySql.Longblob();
                    break;
                case "date":
                    ft = new FieldMySql.Date();
                    break;
                case "time":
                    ft = new FieldMySql.Time();
                    break;
                case "datetime":
                    ft = new FieldMySql.Datetime();
                    break;
                default:
                    throw new Exception(typename + "类型暂不支持转化");
            }
            return ft;
        }

        public static List<DbStructure.STable> ToSafeTables(List<DbStructure.STable> tables)
        {
            List<DbStructure.STable> ts = new List<STable>();
            ts.AddRange(tables.Where(x => x.foreignkeys.Count() == 0));
            var tbhasfor = tables.Where(x => x.foreignkeys.Count() != 0).ToList();
            while (tbhasfor.Count() > 0)
            {
                foreach (var a in tbhasfor)
                {
                    int c = 0;
                    foreach (var b in a.foreignkeys)
                    {
                        if (ts.Exists(x => x.name == b.tablename))
                        {
                            c++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (c == a.foreignkeys.Count())
                    {
                        ts.Add(a);
                        tbhasfor.Remove(a);
                        break;
                    }
                    else
                    {
                        bool faildtb = false;
                        foreach (var b in a.foreignkeys)
                        {
                            if (!tbhasfor.Where(x => x.name != a.name).ToList().Exists(x => x.name == b.tablename))
                            {
                                faildtb = true;
                                break;
                            }
                        }
                        if (faildtb)
                        {
                            ts.Add(a);
                            tbhasfor.Remove(a);
                            break;
                        }
                    }
                }
            }
            return ts;
        }

        #region
        public static List<string> SqlServerTypes = new List<string>() { 
                "bit",//可以取值为 1、0 或 NULL 的整数数据类型。SQL Server 数据库引擎可优化 bit 列的存储。如果表中的列为 8 bit 或更少，则这些列作为 1 个字节存储。如果列为 9 到 16 bit，则这些列作为 2 个字节存储，以此类推。
                "tinyint",//0 到 255	1 字节
                "smallint",//-2^15 (-32,768) 到 2^15-1 (32,767)	2 字节	 
                "int",//-2^31 (-2,147,483,648) 到 2^31-1 (2,147,483,647)	4 字节
                "bigint",//-2^63 (-9,223,372,036,854,775,808) 到 2^63-1 (9,223,372,036,854,775,807)	8 字节

                "float",//-1.79E + 308 至 -2.23E - 308、0 以及 2.23E - 308 至 1.79E + 308	取决于 n 的值( float[(n)])
                "decimal",//- 10^38 +1 到 10^38 - 1	存储字节数	精度
                "numeric",//- 10^38 +1 到 10^38 - 1	5 字节	1 - 9 9 字节	10-19	 	 	 
                                                                                            //13 字节	20-28	 	 	 
                                                                                            //17 字节	29-38
                "real",//-3.40E + 38 至 -1.18E - 38、0 以及 1.18E - 38 至 3.40E + 38	4 字节
                "money",//-922,337,203,685,477.5808 到 922,337,203,685,477.5807	8 字节

                "char",//n 的取值范围为 1 至 8,000	存储大小是 n 个字节
                "varchar",//n 的取值范围为 1 至 8,000	存储大小是输入数据的实际长度加 2 个字节,max 指示最大存储大小是 2^31-1 个字节
                "nchar",//介于 0 与 65535 之间的正整数。如果指定了超出此范围的值，将返回 NULL。	存储大小为两倍 n 字节
                "nvarchar",//n 的取值范围为 1 至 4,000	存储大小是所输入字符个数的两倍 + 2 个字节,max 指示最大存储大小为 2^31-1 字节

                "text",//长度可变的非 Unicode 数据,最大长度为 2^31-1 (2,147,483,647) 个字符	当服务器代码页使用双字节字符时，存储仍是 2,147,483,647 字节。根据字符串，存储大小可能小于 2,147,483,647 字节。
                "ntext",//长度可变的 Unicode 数据,最大长度为 2^30 - 1 (1,073,741,823) 个字符	存储大小是所输入字符个数的两倍（以字节为单位）。

                "binary",//binary [(n)]	长度为 n 字节的固定长度二进制数据，其中 n 是从 1 到 8,000 的值	存储大小为 n 字节
                "varbinary",//varbinary [(n|max)]	可变长度二进制数据。n 可以是从 1 到 8,000 之间的值。	max 指示最大存储大小为 2^31-1 字节。存储大小为所输入数据的实际长度 + 2 个字节
                "image",//长度可变的二进制数据,最大长度为2^31-1 (2,147,483,647) 个字节

                "date",
                "time",
                "datetime"
        };
        public static List<string> MySqlTypes = new List<string>() {
        "tinyint",//1个字节表示(-128~127)
        "smallint",//2个字节表示(-32768~32767)
       // "mediumint",//3个字节表示(-8388608~8388607) 
        "int",//4个字节表示(-2147483648~2147483647) 
        "bigint",//8个字节表示(+-9.22*10的18次方) 
        
        "float",//float(m,d)单精度浮点型，8位精度(4字节),m是十进制数字的总个数，d是小数点后面的数字个数。
        "double",//double(m,d)	双精度浮点型，16位精度(8字节)
        "decimal",//decimal(m,d)  定点类型浮点型在数据库中存放的是近似值，而定点类型在数据库中存放的是精确值。参数m是定点类型数字的最大个数（精度），范围为0~65，d小数点右侧数字的个数，范围为0~30，但不得超过m。对定点数的计算能精确到65位数字
        
        "char",//char(n) 	固定长度的字符串，最多255个字符
        "varchar",//varchar(n) 	固定长度的字符串，最多65535个字符

        "tinytext",//tinytext 	可变长度字符串，最多255个字符
        "text",//可变长度字符串，最多65535个字符
        "mediumtext",//mediumtext 	可变长度字符串，最多2的24次方-1个字符 
        "longtext",//可变长度字符串，最多2的32次方-1个字符 

        "binary",//Binary(M)	M	类似Char的二进制存储，特点是插入定长不足补0
        "varbinary",//VarBinary(M)	M	类似VarChar的变长二进制存储，特点是定长不补0


        "tinyblob",//TinyBlob	Max:255	大小写敏感
        "blob",//Blob	Max:64K	大小写敏感
        "mediumblob",//MediumBlob	Max:16M	大小写敏感
        "longblob",//LongBlob	Max:4G	大小写敏感

        "date",//日期'2008-12-2' 
        "time",//时间'12:25:36'
        "datetime",//日期时间'2008-12-2 22:06:44' 
       // "timestamp",//不固定 timestamp比较特殊，如果定义一个字段的类型为timestamp，这个字段的时间会在其他字段修改的时候自动刷新。所以这个数据类型的字段可以存放这条记录最后被修改的时间，而不是真正来的存放时间。
        };
        #endregion


    }
}
