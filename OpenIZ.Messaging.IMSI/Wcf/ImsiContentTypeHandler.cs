using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Wcf
{
    /// <summary>
    /// Content-type handler
    /// </summary>
    public class ImsiContentTypeHandler : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            if (contentType.StartsWith("application/xml+openiz-imsi") ||
                contentType.StartsWith("application/xml"))
                return WebContentFormat.Xml;
            else if (contentType.StartsWith("application/json+openiz-imsi") ||
                contentType.StartsWith("application/json"))
                return WebContentFormat.Json;
            else
                return WebContentFormat.Raw;
        }
    }
}
