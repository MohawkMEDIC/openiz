using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;

namespace OpenIZ.Authentication.OAuth2.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    /// <openiz.authentication.oauth2>
    ///     <token expiry=""/>
    ///     <signature>
    ///         <!-- If using X509 RSA certificates -->
    ///         <certificate storeLocation="" storeName="" x509FindType="" findValue=""/>
    ///         <!-- If using symmetric key -->
    ///         <symmetric secret=""/>
    ///     </signature>
    ///     <claims>
    ///         <add claimType="claimName"/>
    ///     </claims>
    ///     <scopes>
    ///         <add name="scopeName"/>
    ///     </scopes>
    /// </openiz.authentication.oauth2>
    /// ]]>
    /// </remarks>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration handler
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            OAuthConfiguration retVal = new OAuthConfiguration();

            // Nodes
            XmlElement tokenElement = section.SelectSingleNode("./token") as XmlElement,
                x509Signature = section.SelectSingleNode("./signature/certificate") as XmlElement,
                symmSignature = section.SelectSingleNode("./signature/symmetric") as XmlElement;
            XmlNodeList claims = section.SelectNodes("./claims/add/@claimType"),
                scopes = section.SelectNodes("./scopes/add/@name");

            retVal.ValidityTime = TimeSpan.Parse(tokenElement?.Attributes["expiry"]?.Value ?? "00:00:10:00");
            if (x509Signature != null)
            {
                X509Store store = new X509Store(
                    (StoreName)Enum.Parse(typeof(StoreName), x509Signature.Attributes["storeName"]?.Value),
                    (StoreLocation)Enum.Parse(typeof(StoreLocation), x509Signature.Attributes["storeLocation"]?.Value)
                );
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    // Now find the certificate
                    var matches = store.Certificates.Find((X509FindType)Enum.Parse(typeof(X509FindType), x509Signature.Attributes["x509FindType"]?.Value), x509Signature.Attributes["value"]?.Value, false);
                    if (matches.Count > 1)
                        throw new ConfigurationErrorsException("More than one candidate certificate found", x509Signature);
                    else if (matches.Count == 0)
                        throw new ConfigurationErrorsException("No matching certificates found", x509Signature);
                    else
                        retVal.Certificate = matches[0];
                }
                finally
                {
                    store.Close();
                }
            }
            else if (symmSignature != null)
            {
                retVal.ServerSecret = symmSignature.Attributes["secret"]?.Value;
            }
            else
                throw new ConfigurationErrorsException("One of certificate or symmetric key must be selected", null, section);

            // Claims
            if (claims != null)
                foreach (XmlNode itm in claims)
                    retVal.AllowedClientClaims.Add(itm.Value);
            if (scopes != null)
                foreach (XmlNode itm in scopes)
                    retVal.AllowedScopes.Add(itm.Value);

            // Validate the configuration
            retVal.Validate();

            return retVal;

        }
    }
}
