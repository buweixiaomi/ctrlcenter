using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.Utils
{
    public class UrlHelper
    {
        public static string BuildFullUrl(string host, string part)
        {
            part = (part ?? "").Replace("\\", "/");
            if (part.StartsWith("http"))
                return part;
            else
            {
                return host.TrimEnd('/') + "/" + part.TrimStart('/');
            }
        }
    }
}
