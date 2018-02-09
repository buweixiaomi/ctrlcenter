using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure.FieldSqlServer
{
    public class Bit : FieldType
    {
        public Bit()
        {
            this.name = "bit";
        }

        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }
    }

    public class Tinyint : FieldType
    {
        public Tinyint()
        {
            this.name = "tinyint";
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
        }
        public override string ToSqlString()
        {
            return string.Format("{0}({1},{2})", name, precision, scale);
        }
    }

    public class Numeric : FieldType
    {
        public Numeric(int precision, int scale)
        {
            this.name = "numeric";
            this.precision = precision;
            this.scale = scale;
        }
        public override string ToSqlString()
        {
            return string.Format("{0}({1},{2})", name, precision, scale);
        }
    }

    public class Real : FieldType
    {
        public Real()
        {
            this.name = "real";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }


    }

    public class Money : FieldType
    {
        public Money()
        {
            this.name = "money";
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }


    }

    public class Char : FieldType
    {
        public Char(int maxlength)
        {
            this.name = "char";
            this.maxlength = maxlength;
        }

        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength == -1 ? "max" : maxlength.ToString());
        }


    }

    public class Varchar : FieldType
    {
        public Varchar(int maxlength)
        {
            this.name = "varchar";
            this.maxlength = maxlength;
        }

        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength == -1 ? "max" : maxlength.ToString());
        }


    }

    public class Nchar : FieldType
    {
        public Nchar(int maxlength)
        {
            this.name = "nchar";
            this.maxlength = maxlength;
        }

        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength == -1 ? "max" : maxlength.ToString());
        }


    }

    public class Nvarchar : FieldType
    {
        public Nvarchar(int maxlength)
        {
            this.name = "nvarchar";
            this.maxlength = maxlength;
        }

        public override string ToSqlString()
        {
            return string.Format("{0}({1})", name, maxlength == -1 ? "max" : maxlength.ToString());
        }


    }

    public class Text : FieldType
    {
        public Text()
        {
            this.name = "text";
        }
    }

    public class Ntext : FieldType
    {
        public Ntext()
        {
            this.name = "ntext";
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

    public class Image : FieldType
    {
        public Image()
        {
            this.name = "image";
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
        }
        public override string ToSqlString()
        {
            return string.Format("{0}", name);
        }

    }

}
