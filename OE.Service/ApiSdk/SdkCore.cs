using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk
{
    public class SdkCore
    {
        public static ApiResult<T> InvokeApi<T>(string url, object para) where T : new()
        {
            try
            {
                string fullurl = url;
                if (!url.ToLower().StartsWith("http://"))
                    fullurl = Configrations.Config.GetSystemConfig(Configrations.ConfigConst.ServerUrlKeyName, "").TrimEnd('/') + "/" + url.TrimStart('/');
                Dictionary<string, string> paras = Utils.Utils.GetDicFromObject(para);
                byte[] bs = Utils.HttpHelper.Post(fullurl, paras);
                string resultstring = System.Text.Encoding.UTF8.GetString(bs);
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(resultstring);
                return model;
            }
            catch (Exception ex)
            {
                return new ApiResult<T>() { code = -9999, msg = "调用接口出错！" + ex.Message };
            }
        }


        public static ApiResult<byte[]> Download(string url)
        {
            try
            {
                string fullurl = url;
                if (!url.ToLower().StartsWith("http://"))
                    fullurl = Configrations.Config.GetSystemConfig(Configrations.ConfigConst.ServerUrlKeyName, "").TrimEnd('/') + "/" + url.TrimStart('/');
                byte[] bs = Utils.HttpHelper.Post(fullurl, null);
                return new ApiResult<byte[]>() { code = 1, msg = "ok", data = bs };
            }
            catch (Exception ex)
            {
                return new ApiResult<byte[]>() { code = -9999, msg = "调用接口出错！" };
            }
        }

    }
}
