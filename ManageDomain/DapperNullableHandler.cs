using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ManageDomain
{
    public class DapperNullableHandler : Dapper.SqlMapper.ITypeHandler
    {
        public object Parse(Type destinationType, object value)
        {
            if (destinationType.Name != "Nullable`1")
            {
              //  Dapper.SqlMapper.GetTypeMap(destinationType).
            }
            if (value == null || value == System.DBNull.Value)
                return null;
            else
                return value;
        }

        public void SetValue(System.Data.IDbDataParameter parameter, object value)
        {
            if (value == null)
                parameter.Value = System.DBNull.Value;
            else
                parameter.Value = value;
        }
    }

}
