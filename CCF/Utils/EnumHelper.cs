using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.Utils
{
    public static class EnumHelper
    {
        public static List<T> GetEnumItems<T>()
        {
            Type t = typeof(T);
            List<T> values = new List<T>();
            foreach (var a in t.GetEnumValues())
            {
                values.Add((T)a);
            }
            return values;
        }
        public static T GetEnumAttribute<T>(Enum item)
        {
            var t = item.GetType();
            List<T> values = new List<T>();
            var field = t.GetField(item.ToString());
            object[] attrs = field.GetCustomAttributes(typeof(T), false);
            if (attrs == null || attrs.Length == 0)
                return default(T);
            return (T)attrs[0];
        }
    }
}
