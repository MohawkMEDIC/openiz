using OpenIZ.Authentication.OAuth2.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Wcf
{
    /// <summary>
    /// OAuth2.0 Contract
    /// </summary>
    [ServiceContract(ConfigurationName = "OpenIZ.Authentication.OAuth2")]
    public interface IOAuthTokenContract 
    {

        /// <summary>
        /// Token request
        /// </summary>
        [WebInvoke(UriTemplate = "oauth2_token", Method = "POST")]
        [OperationContract]
        Stream Token(Message inboundData);
    }
}
