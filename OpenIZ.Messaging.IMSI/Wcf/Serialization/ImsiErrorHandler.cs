using MARC.HI.EHRS.SVC.Core.Exceptions;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Wcf;
using OpenIZ.Messaging.IMSI.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Wcf.Serialization
{
    /// <summary>
    /// Error handler
    /// </summary>
    public class ImsiErrorHandler : IErrorHandler
    {
        /// <summary>
        /// Handle error
        /// </summary>
        public bool HandleError(Exception error)
        {
            return true;
        }

        /// <summary>
        /// Provide fault
        /// </summary>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // Formulate appropriate response
            if (error is PolicyViolationException || error is SecurityException || (error as FaultException)?.Code.SubCode?.Name == "FailedAuthentication")
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden;
            else if (error is SecurityTokenException)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", "Bearer");
            }
            else if (error is WebFaultException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = (error as WebFaultException).StatusCode;
            else if (error is Newtonsoft.Json.JsonException ||
                error is System.Xml.XmlException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            else if (error is UnauthorizedRequestException)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", (error as UnauthorizedRequestException).AuthenticateChallenge);
            }
            else
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

            // Construct an error result
            var errorResult = new ErrorResult()
            {
                Key = Guid.NewGuid(),
                Type = error.GetType().Name,
                Details = new List<ResultDetail>()
                    {
                        new ResultDetail(DetailType.Error, error.Message)
                    }
            };

            // Cascade inner exceptions
            var ie = error.InnerException;
            while (ie != null)
                errorResult.Details.Add(new ResultDetail(DetailType.Error, String.Format("Caused By: {0}", ie.Message)));

            // Return error in XML only at this point
            fault = new ImsiMessageDispatchFormatter().SerializeReply(version, null, errorResult);

            
        }
    }
}
