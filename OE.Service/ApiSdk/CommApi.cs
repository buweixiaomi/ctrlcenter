using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.ApiSdk
{
    public class CommApi
    {
        public ApiResult<byte[]> DownloadFile(string url)
        {
            return SdkCore.Download(url);
        }

        public ApiResult<object> UploadData(string uploadtype, string data)
        {
            var result = SdkCore.InvokeApi<object>(Configrations.ConfigConst.API_UPLOAD_DATA, new
            {
                uploadtype = uploadtype,
                data = data
            });
            return result;
        }
    }
}
