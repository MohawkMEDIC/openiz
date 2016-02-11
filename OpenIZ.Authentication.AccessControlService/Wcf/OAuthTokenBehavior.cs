using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Authentication.AccessControlService.Model;
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

namespace OpenIZ.Authentication.AccessControlService.Wcf
{
    /// <summary>
    /// OAuth Token Service
    /// </summary>
    public class OAuthTokenBehavior : IOAuthTokenContract
    {

        // Trace source name
        private TraceSource m_traceSource = new TraceSource(OAuthConstants.TraceSourceName);

        /// <summary>
        /// OAuth token request
        /// </summary>
        // TODO: Add ability to authentication a claim with POU
        public Stream Token(NameValueCollection tokenRequest)
        {

            // Get the client application 
            IApplicationIdentityProviderService clientIdentityService = ApplicationContext.Current.GetService<IApplicationIdentityProviderService>();
            IIdentityProviderService identityProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();

            // Only password grants
            if (tokenRequest["grant_type"] != OAuthConstants.GrantNamePassword)
                return this.CreateErrorCondition(OAuthErrorType.unsupported_grant_type, "Only 'password' grants allowed");

            // Password grant needs well formed scope
            Uri scope = null;
            if (String.IsNullOrWhiteSpace(tokenRequest["scope"]) || !Uri.TryCreate(tokenRequest["scope"], UriKind.Absolute, out scope))
                return this.CreateErrorCondition(OAuthErrorType.invalid_scope, "Password grant must have well known scope");

            IPrincipal clientPrincipal = null; 
            
            // First is there a client Basic header on the request
            if (!String.IsNullOrEmpty(WebOperationContext.Current.IncomingRequest.Headers["Authorization"]))
            {

                // Validate the client
                if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
                    return this.CreateErrorCondition(OAuthErrorType.invalid_client, "Anonymous clients not allowed");
                var passwordClaim = ClaimsPrincipal.Current.FindFirst("password");

                // Validate password / secret
                if (passwordClaim == null)
                    return this.CreateErrorCondition(OAuthErrorType.unauthorized_client, "No client secret provided");

                try
                {
                    clientPrincipal = clientIdentityService.Authenticate(ClaimsPrincipal.Current.Identity.Name, passwordClaim.Value);
                    this.m_traceSource.TraceInformation("Client principal : {0}", clientPrincipal?.Identity.Name);
                }
                catch(SecurityException e)
                {
                    return this.CreateErrorCondition(OAuthErrorType.unauthorized_client, e.Message);
                }

            }
            
            // Client is not authenticated
            if(clientPrincipal == null)
                return this.CreateErrorCondition(OAuthErrorType.unauthorized_client, "Unauthorized Client");
            
            this.m_traceSource.TraceInformation("Begin owner password credential grant for {0}", clientPrincipal.Identity.Name);
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
                catch(SecurityException e)
                {
                    return this.CreateErrorCondition(OAuthErrorType.invalid_grant, e.Message);
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
            foreach(var itm in WebOperationContext.Current.IncomingRequest.Headers.GetValues("X-OpenIZACS-Claim"))
            {
                var claim = itm.Split('=');

                // Purpose of use claim
                if (String.Equals(claim[0], OpenIzClaimTypes.XspaPurposeOfUseClaim, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (pdp.GetPolicyOutcome(userPrincipal, PermissionPolicyIdentifiers.ElevateClinicalData) == PolicyDecisionOutcomeType.Grant) // Uesr can use POU to elevate to get all clinical data
                        retVal.Add(new Claim(OpenIzClaimTypes.XspaPurposeOfUseClaim, claim[1]));
                    else
                        throw new SecurityException(String.Format("No right to make claim {0}", claim[0]));
                }
                else
                    throw new SecurityException(String.Format("Unrecognized claim {0}", claim[0]));
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
                expires = DateTime.Now.AddMinutes(10);

            // System claims
            List<Claim> claims = new List<Claim>(
                    roleProvider.GetAllRoles(oizPrincipal.Identity.Name).Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r, "xs:string", ApplicationContext.Current.Configuration.Custodianship.Name))
            )
            {
                new Claim(ClaimTypes.AuthenticationInstant, issued.ToString("o")), 
                new Claim(ClaimTypes.AuthenticationMethod, "OAuth2"),
                new Claim(ClaimTypes.Expiration, expires.ToString("o")), 
                new Claim(ClaimTypes.Name, oizPrincipal.Identity.Name),
                new Claim(OpenIzClaimTypes.OpenIzApplicationIdentifierClaim, clientPrincipal.Identity.Name)
            };

            // Get policies
            var oizPrincipalPolicies = pip.GetActivePolicies(oizPrincipal);
            claims.AddRange(oizPrincipalPolicies.Where(o=>o.Rule == PolicyDecisionOutcomeType.Grant).Select(o => new Claim(OpenIzClaimTypes.OpenIzGrantedPolicyClaim, o.Policy.Oid)));
                
            // Is the user elevated? If so, add claims for those policies
            if(claims.Exists(o=>o.Type == OpenIzClaimTypes.XspaPurposeOfUseClaim))
                claims.AddRange(oizPrincipalPolicies.Where(o => o.Rule == PolicyDecisionOutcomeType.Elevate).Select(o => new Claim(OpenIzClaimTypes.OpenIzGrantedPolicyClaim, o.Policy.Oid)));

            // Additional claims
            claims.AddRange(additionalClaims);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(oizPrincipal.Identity, claims));

            // TODO: Add configuration parameters to customize this behavior
            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
            byte[] key = new byte[64];
            cryptoProvider.GetNonZeroBytes(key);

            var jwt = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(
                    new InMemorySymmetricSecurityKey(key),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"
                ),
                audience: appliesTo.Uri.ToString(),
                notBefore: issued,
                expires: expires,
                claims: claims
            );
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(handler.WriteToken(jwt)));
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
        private Stream CreateResponse(OAuthError err)
        {
            String result = JsonConvert.SerializeObject(err, Formatting.None);
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            return ms;
        }
    }
}
