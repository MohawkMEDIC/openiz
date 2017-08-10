/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Security.Audit;
using OpenIZ.Core.Wcf.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
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

            bool isSecurityViolation = false;

            // Formulate appropriate response
            if (error is DomainStateException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.ServiceUnavailable;
            else if (error is PolicyViolationException || error is SecurityException || (error as FaultException)?.Code.SubCode?.Name == "FailedAuthentication") {
                AuditUtil.AuditRestrictedFunction(error, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            else if (error is SecurityTokenException)
            {
                isSecurityViolation = true;
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", "Bearer");
            }
            else if (error is LimitExceededException)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = (HttpStatusCode)429;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Too Many Requests";
                WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.RetryAfter, "1200");
            }
            else if (error is UnauthorizedRequestException)
            {
                isSecurityViolation = true;
                AuditUtil.AuditRestrictedFunction(error as UnauthorizedRequestException, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", (error as UnauthorizedRequestException).AuthenticateChallenge);
            }
            else if (error is UnauthorizedAccessException)
            {
                AuditUtil.AuditRestrictedFunction(error, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            else if (error is WebFaultException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = (error as WebFaultException).StatusCode;
            else if (error is FaultException) // Other fault exception do nothing
                ;
            else if (error is Newtonsoft.Json.JsonException ||
                error is System.Xml.XmlException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            else if (error is DuplicateKeyException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
            else if (error is FileNotFoundException || error is KeyNotFoundException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
            else if (error is DomainStateException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.ServiceUnavailable;
            else if (error is DetectedIssueException)
                WebOperationContext.Current.OutgoingResponse.StatusCode = (System.Net.HttpStatusCode)422;
            else
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
           
       

            fault = Message.CreateMessage(version, MessageFault.CreateFault(code, reason), String.Empty);
            
        }
    }
}
