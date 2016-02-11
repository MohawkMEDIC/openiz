using OpenIZ.Authentication.AccessControlService.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.AccessControlService.Wcf
{
    /// <summary>
    /// OAuth2.0 Contract
    /// </summary>
    [ServiceContract]
    public interface IOAuthTokenContract 
    {

        /// <summary>
        /// Token request
        /// </summary>
        [WebInvoke(UriTemplate = "/oauth2_token")]
        [OperationContract]
        Stream Token(NameValueCollection tokenRequest);
    }
}
