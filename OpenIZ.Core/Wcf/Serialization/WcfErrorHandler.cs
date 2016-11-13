/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Configuration;
using OpenIZ.Core.Wcf.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace OpenIZ.Core.Wcf.Serialization
{
    /// <summary>
    /// Error handler
    /// </summary>
    public class WcfErrorHandler : IErrorHandler
    {

        private OpenIzConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(OpenIzConstants.OpenIZConfigurationName) as OpenIzConfiguration;

        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.WcfTraceSourceName);
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

            FaultCode code = FaultCode.CreateSenderFaultCode("GENERR", "http://openiz.org/model");
            FaultReason reason = new FaultReason(error.Message);
            this.m_traceSource.TraceEvent(TraceEventType.Error, error.HResult, "Error on WCF pipeline: {0}", error);

            // Formulate appropriate response
            if (error is PolicyViolationException || error is SecurityException || (error as FaultException)?.Code.SubCode?.Name == "FailedAuthentication")
            {
                code = FaultCode.CreateSenderFaultCode("POLICY", "http://openiz.org/model");
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            else if (error is SecurityTokenException)
            {
                code = FaultCode.CreateSenderFaultCode("TOKEN", "http://openiz.org/model");

                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", String.Format("Bearer realm=\"{0}\"", this.m_configuration.Security.ClaimsAuth.Realm));
            }
            else if (error is WebFaultException)
            {
                code = FaultCode.CreateSenderFaultCode("FAULT", "http://openiz.org/model");
                WebOperationContext.Current.OutgoingResponse.StatusCode = (error as WebFaultException).StatusCode;
            }
            else if (error is Newtonsoft.Json.JsonException ||
                error is System.Xml.XmlException)
            {
                code = FaultCode.CreateSenderFaultCode("BAD_REQUEST", "http://openiz.org/model");

                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            else if (error is UnauthorizedRequestException)
            {
                code = FaultCode.CreateSenderFaultCode("POLICY", "http://openiz.org/model");

                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", (error as UnauthorizedRequestException).AuthenticateChallenge);
            }
            else
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
           
            fault = Message.CreateMessage(version, MessageFault.CreateFault(code, reason), String.Empty);
            
        }
    }
}
