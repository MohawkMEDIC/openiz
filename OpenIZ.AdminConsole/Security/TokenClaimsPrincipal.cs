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
 * Date: 2017-3-31
 */
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Security.Principal;

namespace OpenIZ.AdminConsole.Security
{
	/// <summary>
	/// Token claims principal.
	/// </summary>
	public class TokenClaimsPrincipal : ClaimsPrincipal
	{

		// Claim map
		private readonly Dictionary<String, String> claimMap = new Dictionary<string, string>() {
			{ "unique_name", ClaimsIdentity.DefaultNameClaimType },
			{ "role", ClaimsIdentity.DefaultRoleClaimType },
			{ "sub", ClaimTypes.Sid },
			{ "authmethod", ClaimTypes.AuthenticationMethod },
			{ "exp", ClaimTypes.Expiration },
			{ "nbf", ClaimTypes.AuthenticationInstant },
			{ "email", ClaimTypes.Email },
            { "tel", ClaimTypes.MobilePhone }
		};

		// The token
		private String m_token;
        
        /// <summary>
        /// Gets the refresh token
        /// </summary>
        public String RefreshToken { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.AdminConsole.Security.TokenClaimsPrincipal"/> class.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="tokenType">Token type.</param>
        public TokenClaimsPrincipal (String token, String tokenType, String refreshToken) : base()
		{
			if (String.IsNullOrEmpty (token))
				throw new ArgumentNullException (nameof (token));
			else if (String.IsNullOrEmpty (tokenType))
				throw new ArgumentNullException (nameof (tokenType));
			else if (tokenType != "urn:ietf:params:oauth:token-type:jwt")
				throw new ArgumentOutOfRangeException (nameof (tokenType), "expected urn:ietf:params:oauth:token-type:jwt");

			// Token
			this.m_token = token;

			String[] tokenObjects = token.Split ('.');
            // Correct each token to be proper B64 encoding
            for (int i = 0; i < tokenObjects.Length; i++)
                tokenObjects[i] = tokenObjects[i].PadRight(tokenObjects[i].Length + (tokenObjects[i].Length % 4), '=').Replace("===","=");
			JObject headers = JObject.Parse (Encoding.UTF8.GetString (Convert.FromBase64String (tokenObjects [0]))),
				body = JObject.Parse (Encoding.UTF8.GetString (Convert.FromBase64String (tokenObjects [1])));

			// Attempt to get the certificate
			if (((String)headers ["alg"]).StartsWith ("RS")) {
				var cert = X509CertificateUtils.FindCertificate (X509FindType.FindByThumbprint, StoreLocation.CurrentUser, StoreName.My, headers ["x5t"].ToString ());
				//if (cert == null)
				//	throw new SecurityTokenException(SecurityTokenExceptionType.KeyNotFound, String.Format ("Cannot find certificate {0}", headers ["x5t"]));
				// TODO: Verify signature
			} else if (((String)headers ["alg"]).StartsWith ("HS")) {
				var keyId = headers ["keyid"].Value<Int32>();
				// TODO: Verfiy signature
			} 
			
			// Parse the jwt
			List<Claim> claims = new List<Claim>();

			foreach (var kf in body) {
				String claimName = kf.Key;
				if (!claimMap.TryGetValue (kf.Key, out claimName))
					claims.AddRange (this.ProcessClaim (kf, kf.Key));
				else
					claims.AddRange (this.ProcessClaim (kf, claimName));
			}

			Claim expiryClaim = claims.Find (o => o.Type == ClaimTypes.Expiration),
				notBeforeClaim = claims.Find (o => o.Type == ClaimTypes.AuthenticationInstant);

            if (expiryClaim == null || notBeforeClaim == null)
                throw new SecurityException("Missing NBF or EXP claim");
            else
            {
                DateTime expiry = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    notBefore = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                expiry = expiry.AddSeconds(Int32.Parse(expiryClaim.Value)).ToLocalTime();
                notBefore = notBefore.AddSeconds(Int32.Parse(notBeforeClaim.Value)).ToLocalTime();

                if (expiry == null || expiry < DateTime.Now)
                    throw new SecurityException("Token expired");
                else if (notBefore == null || Math.Abs(notBefore.Subtract(DateTime.Now).TotalMinutes) > 3)
                    throw new SecurityException("Token cannot yet be used (issued in the future)");
            }
            this.RefreshToken = refreshToken;

			this.AddIdentity(new ClaimsIdentity(new GenericIdentity(body["unique_name"]?.Value<String>().ToLower() ?? body["sub"]?.Value<String>().ToLower(), "OAUTH"), claims, "OAUTH2", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
		}


		/// <summary>
		/// Processes the claim.
		/// </summary>
		/// <returns>The claim.</returns>
		/// <param name="jwtClaim">Jwt claim.</param>
		public IEnumerable<Claim> ProcessClaim(KeyValuePair<String, JToken> jwtClaim, String claimType)
		{
			List<Claim> retVal = new List<Claim> ();
			if(jwtClaim.Value is JArray)
				foreach(var val in jwtClaim.Value as JArray)
					retVal.Add(new Claim(claimType, (String)val));
			else
				retVal.Add(new Claim(claimType, jwtClaim.Value.ToString()));
			return retVal;
		}

		/// <summary>
		/// Represent the token claims principal as a string (the JWT token itself)
		/// </summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		public override string ToString ()
		{
			return this.m_token;
		}
	}
}

