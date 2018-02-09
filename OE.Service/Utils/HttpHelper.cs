using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Utils
{
    public class HttpHelper
    {

        public static byte[] Get(string url)
        {
            return Request(url, "GET", null);
        }

        public static byte[] Post(string url, Dictionary<string, string> values)
        {
            string strbody = string.Empty;
            if (values != null)
                strbody = string.Join("&", values.Select(x => x.Key + "=" + System.Web.HttpUtility.UrlEncode(x.Value)));
            byte[] bs = null;
            if (strbody != string.Empty)
            {
                bs = System.Text.Encoding.UTF8.GetBytes(strbody);
            }
            return Request(url, "Post", bs);
        }

        public static byte[] Request(string url, string method, byte[] body)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            var head = GetHead();
            if (head != null)
            {
                foreach (var a in head)
                {
                    request.Headers.Set(a.Key, a.Value ?? "");
                }
            }
            request.Method = method;
            if (method.ToLower() == "post")
            {
                if (body == null)
                    body = new byte[0];
                request.ContentLength = body.Length;
                var requeststream = request.GetRequestStream();
                requeststream.Write(body, 0, body.Length);
                requeststream.Flush();
                requeststream.Close();
                requeststream.Dispose();
            }
            var response = (System.Net.HttpWebResponse)request.GetResponse();
            var responsestream = response.GetResponseStream();
            List<byte> bsfuffer = new List<byte>();
            int i = 0;
            while ((i = responsestream.ReadByte()) != -1)
            {
                bsfuffer.Add((byte)i);
            }
            responsestream.Close();
            responsestream.Dispose();
            return bsfuffer.ToArray();
        }

        private static Dictionary<string, string> GetHead()
        {
            string[] macAddress = null;
            string[] ips = null;
            Utils.GetIpsAndMacs(out ips, out macAddress);
            Dictionary<string, string> head = new Dictionary<string, string>();
            head.Add("Client_Macs", string.Join(",", macAddress));
            head.Add("Client_IPs", string.Join(",", ips));
            head.Add("Client_ID", Configrations.Config.ClientID);
            return head;
        }
    }
}
