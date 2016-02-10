using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.AccessControlService.Wcf
{
    /// <summary>
    /// OAuth Token Service
    /// </summary>
    public class OAuthTokenBehavior : IOAuthTokenContract
    {
        /// <summary>
        /// OAuth Token Service
        /// </summary>
        public Stream Token()
        {
            throw new NotImplementedException();
        }
    }
}
