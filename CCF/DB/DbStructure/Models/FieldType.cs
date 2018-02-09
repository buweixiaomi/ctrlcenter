using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public abstract class FieldType
    {
        public bool canemptydefault { get; set; }
        public string defaultvalue { get; set; }
        public string name { get; protected set; }
        public int scale { get; set; }
        public int precision { get; set; }
        public int maxlength { get; set; }
        public virtual string ToSqlString()
        {
            if (maxlength == 0)
            {
                return string.Format("{0}", name);
            }
            else
            {
                if (scale == 0)
                {
                    return string.Format("{0}({1})", name, maxlength);
                }
                else
                {
                    return string.Format("{0}({1},{2})", name, precision, scale);
                }
            }
        }
    }
}
