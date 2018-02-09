using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManageWeb
{
    public class ClientAuthAttribute : Attribute
    {
        public ClientAuthType AuthType { get; private set; }
        public ClientAuthAttribute(ClientAuthType authType)
        {
            AuthType = authType;
        }

        public ClientAuthAttribute()
        {
            AuthType = ClientAuthType.TryAuth;
        }
    }

    public enum ClientAuthType
    {
        None,
        Auth,
        TryAuth
    }
}