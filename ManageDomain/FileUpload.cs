using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain
{
    /// <summary>
    /// 上传文件到资源网站
    /// ##说明##
    /// 1、资源网站的域名地址(ResourceSite_URL)和资源网站上传者身份ID(ResourceSite_AppId)需要在appsetting里配置
    /// 2、对于非Public_Static的上传类型，返回值json对象中的data为资源码，下载里需要通过Auth接口得到相对url，最后通过这个 BuildFullUrl(url)得到完整下载地址
    /// 3、上传类型为Public_staitc返回url ，通过 BuildFullUrl(url)生成地址可无限下载，非Public_staitc类型可能为非无限下载，详情请看UpLoadMode 和 AuthCodeUseMode说明
    /// </summary>
    public class FileUpload
    {
        public const string UploadUrl = "/file/upload";
        public const string AuthUrl = "/file/auth";
        /// <summary>
        /// appsetting 需要配置 ResourceSite_AppId
        /// </summary>
        public static long ResourceSite_AppId
        {
            get { return Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["ResourceSite_AppId"] ?? "0"); }
        }

        /// <summary>
        /// appsetting 需要配置 ResourceSite_URL
        /// </summary>
        public static string ResourceSite_URL
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ResourceSite_URL"] ?? ""; }
        }

        /// <summary>
        /// POST请求基本方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public static bool PostRequest(string url, List<Field> param, out string responseString)
        {
            responseString = "";
            string boundary = "--------abc" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.KeepAlive = true;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";
            // webrequest.Accept = "text/html";
            byte[] bs = BuildBody(param, boundary, Encoding.UTF8);
            webrequest.ContentLength = bs.Length;
            var requeststream = webrequest.GetRequestStream();
            requeststream.Write(bs, 0, bs.Length);
            requeststream.Flush();
            requeststream.Close();
            HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                responseString = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                return true;
            }
            else
            {
                responseString = "请求出错！";
                return false;
            }

        }

        /// <summary>
        /// 上传资源到资源网站
        /// </summary>
        /// <param name="fileName">文件名，需要带扩展名</param>
        /// <param name="fileBytes">文件字节流</param>
        /// <param name="upladmodel">上传方式，即是否需要对资源进行访问权限控制</param>
        /// <param name="responseString">当return为true时，该值为json,否则认为是普通字符串</param>
        /// <returns></returns>
        public static bool Upload(string fileName, byte[] fileBytes, UpLoadMode upladmodel, out  string responseString)
        {
            string url = ResourceSite_URL.TrimEnd('/') + UploadUrl;
            long appId = ResourceSite_AppId;
            return PostRequest(url, new List<Field>() { 
                new Field() { IsFile = true, FileBytes = fileBytes, FileName = fileName, Name = "uploadFile" },
                new Field() { IsFile = false, Name = "AppId",Value = appId.ToString() },
                new Field() { IsFile = false, Name = "UploadMode", Value =upladmodel.ToString()  }
            }, out responseString);
        }

        /// <summary>
        /// 为需要授权的资源下载前需要验证，取得认证码
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="useMode"></param>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public static bool Auth(string fileKey, AuthCodeUseMode useMode, out string responseString)
        {
            string url = ResourceSite_URL.TrimEnd('/') + AuthUrl;
            return PostRequest(url, new List<Field>() { 
                new Field() { IsFile = false, Name = "AppId",Value = ResourceSite_AppId.ToString() },
                new Field() { IsFile = false, Name = "KeyName", Value =fileKey  },
                new Field() { IsFile = false, Name = "UseMode", Value =useMode.ToString()  } 
            }, out responseString);
        }

        /// <summary>
        /// 根据相对url得 拼上资源网站地址得到完整地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string BuildFullUrl(string url)
        {
            if (url.StartsWith("http://"))
                return url;
            return ResourceSite_URL.TrimEnd('/') + "/" + url.TrimStart('/');
        }

        /// <summary>
        /// 去掉完整地址前的域名地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string BuildPartUrl(string url)
        {
            if (!url.StartsWith("http://"))
                return url;
            int index_split = url.IndexOf('/', 7);
            if (index_split >= 0)
                return url.Substring(index_split + 1);
            return url;
        }


        private static byte[] BuildBody(List<Field> param, string boundary, System.Text.Encoding enc)
        {
            List<byte> buffer = new List<byte>();
            if (param == null)
            {
                return buffer.ToArray();
            }
            byte[] byte_boundary = enc.GetBytes("--" + boundary);
            byte[] byte_enter = enc.GetBytes("\r\n");
            foreach (var a in param)
            {
                buffer.AddRange(byte_boundary);
                buffer.AddRange(byte_enter);
                if (a.IsFile)
                {
                    buffer.AddRange(enc.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n", a.Name, a.FileName, a.ContentType)));
                    buffer.AddRange(a.FileBytes);
                    buffer.AddRange(byte_enter);
                    //buffer.AddRange(byte_boundary);
                    //buffer.AddRange(byte_enter);
                }
                else
                {
                    buffer.AddRange(enc.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"\r\nContent-Type:{2}\r\n\r\n{1}\r\n", a.Name, a.Value, a.ContentType)));
                }
            }
            if (param.Count > 0)
            {
                buffer.AddRange(byte_boundary);
            }
            return buffer.ToArray();
        }

    }
    public class Field
    {
        public bool IsFile { get; set; }
        public byte[] FileBytes { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Value { get; set; }
        public string ContentType
        {
            get
            {
                if (IsFile)
                {
                    return "application/octet-stream";
                }
                else
                {
                    return "text/plain";
                }
            }
        }
    }

    public enum UpLoadMode
    {
        /// <summary>
        /// 上传为静态文件，可以通过返回的URL访问
        /// </summary>
        PUBLIC_STATIC,//= "PUBLIC_STATIC";
        /// <summary>
        /// 上传为非静态文件，可以通过返回的资料码通过下载授权接口(无需AppID)返回URL再访问
        /// </summary>
        PUBLIC_NOAUTH,// = "PUBLIC_NOAUTH";

        /// <summary>
        /// 上传为非静态文件，可以通过返回的资料码通过下载授权接口(需要AppID)返回URL再访问
        /// </summary>
        PUBLIC_AUTHCODE, //= "PUBLIC_AUTHCODE";
        /// <summary>
        /// 上传为非静态文件，可以通过返回的资料码通过下载授权接口(需要上传资料时的AppID相同)返回URL再访问
        /// </summary>
        PRIVATE_AUTHCODE// = "PRIVATE_AUTHCODE";
    }

    public enum AuthCodeUseMode
    {
        /// <summary>
        /// 直到过期都可以用
        /// </summary>
        UNTIL_EXPIRED,//= "UNTIL_EXPIRED";

        //在过期前且只能使用一次
        ONLY_ONCE// = "ONLY_ONCE";

    }

}
