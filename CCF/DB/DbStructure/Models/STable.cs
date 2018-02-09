using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public class STable
    {
        public string name { get; set; }
        public List<SColumn> columns { get; set; }
        public SPrimaryKey primarykey { get; set; }
        public List<SForeignKey> foreignkeys { get; set; }
        public List<SUniqueKey> uniquekeys { get; set; }
        public List<SIdentityAttribute> identityattribute { get; set; }
        public List<SDefaultConstraint> defaultconstraint { get; set; }
        public List<SIndex> indexes { get; set; }
        public STable()
        {
            foreignkeys = new List<SForeignKey>();
            uniquekeys = new List<SUniqueKey>();
            defaultconstraint = new List<SDefaultConstraint>();
            indexes = new List<SIndex>();
        }
    }

    public class SColumn
    {
        public STable table { get; set; }
        public string name { get; set; }
        public FieldType filedtype { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool allownull { get; set; }
        public bool isidentity { get; set; }
        public bool isprimarykey { get;set; }
        public SIdentityAttribute identityattribute { get; set; }
        public SDefaultConstraint defaultconstraint { get; set; }
        public SPrimaryKey primarykey { get; set; }
       public List<SForeignKey> foreignkeys { get; set; }
       public List<SUniqueKey> uniquekeys { get; set; }
       public List<SIndex> indexes { get; set; }

       public SColumn()
       {
           foreignkeys = new List<SForeignKey>();
           uniquekeys = new List<SUniqueKey>();
           indexes = new List<SIndex>();
       }
    }

    public class SPrimaryKey
    {
        public STable table { get; set; }
        public string name { get; set; }
        public List<SColumn> columns { get; set; }

        public SPrimaryKey()
        {
            columns = new List<SColumn>();
        }
    }

    public class SForeignKey
    {
        public STable table { get; set; }
        public string name { get; set; }
        public string tablename { get; set; }
        public Dictionary<SColumn, string> columns { get; set; }

        public SForeignKey()
        {
            columns = new Dictionary<SColumn, string>();
        }
    }

    public class SUniqueKey
    {
        public STable table { get; set; }
        public string name { get; set; }
        public List<SColumn> columns { get; set; }

        public SUniqueKey()
        {
            columns = new List<SColumn>();
        }
    }

    public class SIdentityAttribute
    {
        public SColumn column { get; set; }
        public long send { get; set; }
        public long crement { get; set; }
        public long lastvalue { get; set; }
    }

    public class SDefaultConstraint
    {
        public SColumn column { get; set; }
        public string defaultvalue { get; set; }
    }

    public class SIndex
    {
        public STable table { get; set; }
        public string name { get; set; }
        public int indexid { get; set; }
        public bool isclustered { get; set; }
        public bool isunique { get; set; }
        public bool isuniqueconstraint { get; set; }
        public bool isprimarykey { get; set; }
        public Dictionary<SColumn, int> columns { get; set; }

        public SIndex()
        {
            columns = new Dictionary<SColumn, int>();
        }
    }


}
