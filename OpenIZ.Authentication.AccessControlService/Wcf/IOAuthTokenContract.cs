using System;
using System.Collections.Generic;
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
        /// Represents a token request
        /// </summary>
        [WebInvoke(UriTemplate = "/oauth20_token", Method = "POST")]
        Stream Token();

    }
}
