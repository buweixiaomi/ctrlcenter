using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure.FieldMySql
{

    public class Tinyint : FieldType
    {
        public Tinyint()
        {
            this.name = "tinyint";
            this.defaultvalue = "0";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }

    public class Smalint : FieldType
    {
        public Smalint()
        {
            this.name = "smallint";
            this.defaultvalue = "0";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }

    public class Int : FieldType
    {
        public Int()
        {
            this.name = "int";
            this.defaultvalue = "0";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }

    public class Bigint : FieldType
    {
        public Bigint()
        {
            this.name = "bigint";
            this.defaultvalue = "0";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }

    public class Float : FieldType
    {
        public Float()
        {
            this.name = "float";
            this.defaultvalue = "0";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }

    public class Double : FieldType
    {
        public Double()
        {
            this.name = "double";
            this.defaultvalue = "0";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }


    public class Decimal : FieldType
    {
        public Decimal(int precision, int scale)
        {
            this.name = "decimal";
            this.precision = precision;
            this.scale = scale;
            this.defaultvalue = "0";
        }

        public override string ToSqlString()
        {
            return string.Format("{0}({1},{2})", name, precision, scale);
        }
    }

    public class Char : FieldType
    {
        public Char(int maxlength)
        {
            this.name = "char";
            this.maxlength = maxlength;
            this.canemptydefault = true;
        }
        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength);
        }
    }

    public class Varchar : FieldType
    {
        public Varchar(int maxlength)
        {
            this.name = "varchar";
            this.maxlength = maxlength;
            this.canemptydefault = true;
        }
        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength);
        }
    }

    public class Tinytext : FieldType
    {
        public Tinytext()
        {
            this.name = "tinytext";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Text : FieldType
    {
        public Text()
        {
            this.name = "text";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Mediumtext : FieldType
    {
        public Mediumtext()
        {
            this.name = "mediumtext";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Longtext : FieldType
    {
        public Longtext()
        {
            this.name = "longtext";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Binary : FieldType
    {
        public Binary(int maxlength)
        {
            this.name = "binary";
            this.maxlength = maxlength;
        }
        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength);
        }
    }

    public class Varbinary : FieldType
    {
        public Varbinary(int maxlength)
        {
            this.name = "varbinary";
            this.maxlength = maxlength;
        }
        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength);
        }
    }



    public class Tinyblob : FieldType
    {
        public Tinyblob()
        {
            this.name = "tinyblob";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }
    public class Blob : FieldType
    {
        public Blob()
        {
            this.name = "blob";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }
    public class Mediumblob : FieldType
    {
        public Mediumblob()
        {
            this.name = "mediumblob";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }
    public class Longblob : FieldType
    {
        public Longblob()
        {
            this.name = "longblob";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Date : FieldType
    {
        public Date()
        {
            this.name = "date";
            this.defaultvalue = "1900-01-01";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Time : FieldType
    {
        public Time()
        {
            this.name = "time";
            this.defaultvalue = "00:00:00";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Datetime : FieldType
    {
        public Datetime()
        {
            this.name = "datetime";
            this.defaultvalue = "1900-01-01 00:00:00";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }
}
