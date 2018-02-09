using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    public interface IDbStructure
    {
        List<KVModel> GetDataBases(DbConn dbconn);
        List<KVModel> GetTables(DbConn dbconn);
        void ToLocalTable(STable other, List<SqlToSqlModel> sqltypemodel, bool autofieldtypechange);
        string GetFieldCheckSql(FieldCheckType checktype, int length, int pricesion, int scale, double minvalue, double maxvalue);
        STable GetTableStructure(DbConn dbconn, string tablename);

        #region convert to sql string
        string ToSqlString(STable table);
        string ToSqlString1(STable table,int t);
        string ToSqlString2(STable table,int t);

        string ColToSqlString(SColumn col, bool containidentity = true);
        string PKToSqlString(SPrimaryKey pk);
        string FKToSqlString(SForeignKey fk);
        string UQToSqlString(SUniqueKey uq);
        string DFToSqlString(SDefaultConstraint df);
        string IXToSqlString(SIndex index);
        string GetTempPK(STable table);
        string DelTempPk(STable table);
        #endregion
    }
}
