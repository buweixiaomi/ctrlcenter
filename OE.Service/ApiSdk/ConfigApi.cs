using OE.Service.ApiSdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OE.Service.ApiSdk
{
    public class ConfigApi
    {

        public ApiResult<Tuple<string, List<Configrations.ConfigKeyValue>>> GetUnionConfig()
        {
            var resultmodel = new ApiResult<Tuple<string, List<Configrations.ConfigKeyValue>>>();
            var result = SdkCore.InvokeApi<object>(Configrations.ConfigConst.API_CONFIG_UNION, null);
            resultmodel.code = result.code;
            resultmodel.msg = result.msg;
            if (result.code > 0)
            {
                Newtonsoft.Json.Linq.JObject jobj = result.data as Newtonsoft.Json.Linq.JObject;
                if (jobj != null)
                {
                    resultmodel.data = new Tuple<string, List<Configrations.ConfigKeyValue>>(jobj["configsign"].ToString(), new List<Configrations.ConfigKeyValue>());
                    string config = jobj["config"].Value<string>();
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        var jobjconfig = Newtonsoft.Json.Linq.JObject.Parse(config);
                        foreach (var a in jobjconfig.Children<Newtonsoft.Json.Linq.JProperty>())
                        {
                            resultmodel.data.Item2.Add(new Configrations.ConfigKeyValue() { Key = a.Name, Value = a.Value.ToString() });
                        }
                    }
                }
            }
            return resultmodel;
        }

        /*
        public ApiResult<List<Configrations.ConfigKeyValue>> GetGlobalConfig()
        {
            var resultmodel = new ApiResult<List<Configrations.ConfigKeyValue>>();
            var result = SdkCore.InvokeApi<object>(Configrations.ConfigConst.API_CONFIG_GLOBAL, null);
            resultmodel.code = result.code;
            resultmodel.msg = result.msg;
            if (result.code > 0)
            {
                Newtonsoft.Json.Linq.JObject jobj = result.data as Newtonsoft.Json.Linq.JObject;
                resultmodel.data = new List<Configrations.ConfigKeyValue>();
                if (jobj != null)
                {
                    foreach (var a in jobj.Children<Newtonsoft.Json.Linq.JProperty>())
                    {
                        resultmodel.data.Add(new Configrations.ConfigKeyValue() { Key = a.Name, Value = a.Value.ToString() });
                    }
                }
            }
            return resultmodel;
        }


        public ApiResult<List<Configrations.ConfigKeyValue>> GetPrivateConfig()
        {
            var resultmodel = new ApiResult<List<Configrations.ConfigKeyValue>>();
            var result = SdkCore.InvokeApi<object>(Configrations.ConfigConst.API_CONFIG_PRIVATE, null);
            resultmodel.code = result.code;
            resultmodel.msg = result.msg;
            if (result.code > 0)
            {
                Newtonsoft.Json.Linq.JObject jobj = result.data as Newtonsoft.Json.Linq.JObject;
                resultmodel.data = new List<Configrations.ConfigKeyValue>();
                if (jobj != null)
                {
                    foreach (var a in jobj.Children<Newtonsoft.Json.Linq.JProperty>())
                    {
                        resultmodel.data.Add(new Configrations.ConfigKeyValue() { Key = a.Name, Value = a.Value.ToString() });
                    }
                }
            }
            return resultmodel;
        }
        */
    }
}
