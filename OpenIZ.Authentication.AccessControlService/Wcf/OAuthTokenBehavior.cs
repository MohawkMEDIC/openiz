/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-4-19
 */
using OpenIZ.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Authentication.OAuth2.Model;
using System.Diagnostics;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using System.ServiceModel.Web;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Security.Principal;
using System.IdentityModel.Protocols.WSTrust;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using OpenIZ.Core.Security;
using System.Security;
using System.Collections.Specialized;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System.Security.Claims;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Xml;
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Authentication.OAuth2.Configuration;
using Newtonsoft.Json.Converters;
using OpenIZ.Core.Security.Claims;
using System.Security.Authentication;

namespace OpenIZ.Authentication.OAuth2.Wcf
{
    /// <summary>
    /// OAuth Token Service
    /// </summary>
    [ServiceBehavior(ConfigurationName = "OpenIZ.Authentication.OAuth2")]
    public class OAuthTokenBehavior : IOAuthTokenContract
    {

        // Trace source name
        private TraceSource m_traceSource = new TraceSource(OAuthConstants.TraceSourceName);

        // OAuth configuration
        private OAuthConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(OAuthConstants.ConfigurationName) as OAuthConfiguration;

        /// <summary>
        /// OAuth token request
        /// </summary>
        // TODO: Add ability to authentication a claim with POU
        public Stream Token(Message incomingMessage)
        {
            // Convert inbound data to token request
            // HACK: This is to overcome WCF's lack of easy URL encoded form processing
            // Why use WCF you ask? Well, everything else is hosted in WCF and we 
            // want to be able to use the same ports as our other services. Could find
            // no documentation about running WCF and WepAPI stuff in the same app domain
            // on the same ports
            NameValueCollection tokenRequest = new NameValueCollection();
            XmlDictionaryReader bodyReader = incomingMessage.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            String rawBody = Encoding.UTF8.GetString(bodyReader.ReadContentAsBase64());
            var parms = rawBody.Split('&');
            foreach (var p in parms)
            {
                var kvp = p.Split('=');
                tokenRequest.Add(kvp[0], kvp[1]);
            }

            // Get the client application 
            IApplicationIdentityProviderService clientIdentityService = ApplicationContext.Current.GetService<IApplicationIdentityProviderService>();
            IIdentityProviderService identityProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();

            // Only password grants
            if (tokenRequest["grant_type"] != OAuthConstants.GrantNamePassword)
                return this.CreateErrorCondition(OAuthErrorType.unsupported_grant_type, "Only 'password' grants allowed");

            // Password grant needs well formed scope
            Uri scope = null;
            if (String.IsNullOrWhiteSpace(tokenRequest["scope"]) || !Uri.TryCreate(tokenRequest["scope"], UriKind.Absolute, out scope))
            {
                this.m_traceSource.TraceEvent(TraceEventType.Warning, 0, "Scope:{0} is not well formed", tokenRequest["scope"]);
                return this.CreateErrorCondition(OAuthErrorType.invalid_scope, "Password grant must have well known scope");
            }

            IPrincipal clientPrincipal = ClaimsPrincipal.Current;
            
            // Client is not authenticated
            if(clientPrincipal == null || !clientPrincipal.Identity.IsAuthenticated)
                return this.CreateErrorCondition(OAuthErrorType.unauthorized_client, "Unauthorized Client");
            
            this.m_traceSource.TraceInformation("Begin owner password credential grant for {0}", clientPrincipal.Identity.Name);

            if (this.m_configuration.AllowedScopes != null && !this.m_configuration.AllowedScopes.Contains(tokenRequest["scope"]))
                return this.CreateErrorCondition(OAuthErrorType.invalid_scope, "Scope not registered with provider");

            var appliesTo = new EndpointReference(tokenRequest["scope"]);

            // Validate username and password
            if (String.IsNullOrWhiteSpace(tokenRequest["username"]) || String.IsNullOrWhiteSpace(tokenRequest["password"]))
                return this.CreateErrorCondition(OAuthErrorType.invalid_request, "Invalid username or password");
            else
            {
                try
                {
                    var principal = identityProvider.Authenticate(tokenRequest["username"], tokenRequest["password"]);
                    if (principal == null)
                        return this.CreateErrorCondition(OAuthErrorType.invalid_grant, "Invalid username or password");
                    else
                    {
                        return this.CreateTokenResponse(principal, clientPrincipal, appliesTo, this.ValidateClaims(principal));
                    }
                }
                catch (AuthenticationException e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error generating token: {0}", e);
                    return this.CreateErrorCondition(OAuthErrorType.invalid_grant, e.Message);
                }
                catch (SecurityException e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error generating token: {0}", e);
                    return this.CreateErrorCondition(OAuthErrorType.invalid_grant, e.Message);
                }
                catch(Exception e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, "Error generating token: {0}", e);
                    return this.CreateErrorCondition(OAuthErrorType.invalid_request, e.Message);
                }
            }
        }

        /// <summary>
        /// Validate claims made by the requestor
        /// </summary>
        private IEnumerable<Claim> ValidateClaims(IPrincipal userPrincipal)
        {
            IPolicyDecisionService pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            List<Claim> retVal = new List<Claim>();

            // HACK: Find a better way to make claims
            // Claims are stored as X-OpenIZACS-Claim headers
            foreach (var itm in OpenIzClaimTypes.ExtractClaims(WebOperationContext.Current.IncomingRequest.Headers))
            {

                // Claim allowed
                if (this.m_configuration.AllowedClientClaims == null ||
                    !this.m_configuration.AllowedClientClaims.Contains(itm.Type))
                    throw new SecurityException(ApplicationContext.Current.GetLocaleString("SECE001"));
                else
                {
                    // Validate the claim
                    var handler = OpenIzClaimTypes.GetHandler(itm.Type);
                    if (handler == null || handler.Validate(userPrincipal, itm.Value))
                        retVal.Add(itm);
                    else
                        throw new SecurityException(String.Format(ApplicationContext.Current.GetLocaleString("SECE002"), itm.Type));
                }
            }

            return retVal;
        }

        /// <summary>
        /// Create a token response
        /// </summary>
        private Stream CreateTokenResponse(IPrincipal oizPrincipal, IPrincipal clientPrincipal, EndpointReference appliesTo, IEnumerable<Claim> additionalClaims)
        {

            this.m_traceSource.TraceInformation("Will create new ClaimsPrincipal based on existing principal");

            IRoleProviderService roleProvider = ApplicationContext.Current.GetService<IRoleProviderService>();
            IPolicyInformationService pip = ApplicationContext.Current.GetService<IPolicyInformationService>();

            // TODO: Add configuration for expiry
            DateTime issued = DateTime.Now,
                expires = DateTime.Now.Add(this.m_configuration.ValidityTime);

            // System claims
            List<Claim> claims = new List<Claim>(
                    roleProvider.GetAllRoles(oizPrincipal.Identity.Name).Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r))
            )
            {
                new Claim("iss", this.m_configuration.IssuerName),
                new Claim("SubjectID", oizPrincipal.Identity.Name),
                new Claim(ClaimTypes.AuthenticationInstant, issued.ToString("o")), 
                new Claim(ClaimTypes.AuthenticationMethod, "OAuth2"),
                new Claim(ClaimTypes.Expiration, expires.ToString("o")), 
                new Claim(ClaimTypes.Name, oizPrincipal.Identity.Name),
                new Claim(OpenIzClaimTypes.OpenIzApplicationIdentifierClaim,  
                (clientPrincipal as ClaimsPrincipal).FindFirst(ClaimTypes.Sid).Value)
            };


            // Additional claims
            claims.AddRange(additionalClaims);

            // Get policies
            var oizPrincipalPolicies = pip.GetActivePolicies(oizPrincipal);
            claims.AddRange(oizPrincipalPolicies.Where(o=>o.Rule == PolicyDecisionOutcomeType.Grant).Select(o => new Claim(OpenIzClaimTypes.OpenIzGrantedPolicyClaim, o.Policy.Oid)));
                
            // Is the user elevated? If so, add claims for those policies
            if(claims.Exists(o=>o.Type == OpenIzClaimTypes.XspaPurposeOfUseClaim))
                claims.AddRange(oizPrincipalPolicies.Where(o => o.Rule == PolicyDecisionOutcomeType.Elevate).Select(o => new Claim(OpenIzClaimTypes.OpenIzGrantedPolicyClaim, o.Policy.Oid)));


            // Add claims made by the IdP
            if(oizPrincipal is ClaimsPrincipal)
                claims.AddRange((oizPrincipal as ClaimsPrincipal).Claims.Where(o => !claims.Any(c => c.Type == o.Type)));

            // Find the nameid
            var nameId = claims.Find(o => o.Type == ClaimTypes.NameIdentifier);
            if (nameId != null)
                claims.Add(new Claim("sub", nameId.Value));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(oizPrincipal.Identity, claims));

            SigningCredentials credentials = null;
            // Signing credentials
            if (this.m_configuration.Certificate != null)
                credentials = new X509SigningCredentials(this.m_configuration.Certificate);
            else if (!String.IsNullOrEmpty(this.m_configuration.ServerSecret) ||
                this.m_configuration.ServerKey != null)
            {
                var sha = SHA256.Create();
                credentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(this.m_configuration.ServerKey ?? sha.ComputeHash(Encoding.UTF8.GetBytes(this.m_configuration.ServerSecret))),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"
                );
            }
            else
                throw new SecurityException("Invalid signing configuration");

            // Generate security token            
            var jwt = new JwtSecurityToken(
                signingCredentials: credentials,
                audience: appliesTo.Uri.ToString(),
                notBefore: issued,
                expires: expires,
                claims: claims
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            OAuthTokenResponse response = new OAuthTokenResponse()
            {
                TokenType = OAuthConstants.JwtTokenType,
                AccessToken = handler.WriteToken(jwt),
                ExpiresIn = (int)this.m_configuration.ValidityTime.TotalMilliseconds,
                //RefreshToken = // TODO: Need to write a SessionProvider for this so we can keep track of refresh tokens 
            };

            return this.CreateResponse(response);
        }

        /// <summary>
        /// Create error condition
        /// </summary>
        private Stream CreateErrorCondition(OAuthErrorType errorType, String message)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            OAuthError err = new OAuthError()
            {
                Error = errorType,
                ErrorDescription = message
            };
            return this.CreateResponse(err);
        }

        /// <summary>
        /// Create response
        /// </summary>
        private Stream CreateResponse(Object response)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            String result = JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, settings);
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            return ms;
        }
    }
}
