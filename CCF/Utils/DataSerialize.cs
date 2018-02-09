using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.Utils
{
    public class DataSerialize
    {
        private static readonly IsoDateTimeConverter _timeFormat;
        static DataSerialize()
        {
            _timeFormat = _timeFormat = new IsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
        }

        public static string SerializeJson(object data)
        {
            string s = Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings()
            {
                Converters = new List<Newtonsoft.Json.JsonConverter>() { _timeFormat },
              //  ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            return s;
        }

        public static string SerializeJsonBeauty(object data)
        {
            string s = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
            {
                Converters = new List<Newtonsoft.Json.JsonConverter>() { _timeFormat },
                //  ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            return s;
        }

        public static string SerializeJsonCamel(object data)
        {
            string s = Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings()
            {
                Converters = new List<Newtonsoft.Json.JsonConverter>() { _timeFormat },
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            return s;
        }


        public static object DeSerializeJson(string jsondata)
        {
            object s = Newtonsoft.Json.JsonConvert.DeserializeObject(jsondata, new Newtonsoft.Json.JsonSerializerSettings()
            {
                //Converters = new List<Newtonsoft.Json.JsonConverter>() { _timeFormat },
                //ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            return s;
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        public static T DeserializeObject<T>(string str)
        {
            if (string.IsNullOrEmpty(str)) return default(T);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
        }
    }
}
