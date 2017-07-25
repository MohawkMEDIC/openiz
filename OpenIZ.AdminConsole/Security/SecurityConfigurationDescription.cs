using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Http;
using OpenIZ.AdminConsole.Shell;
using System.Security.Principal;

namespace OpenIZ.AdminConsole.Security
{
    /// <summary>
    /// Rest Client Security Description
    /// </summary>
    public class SecurityConfigurationDescription : IRestClientSecurityDescription
    {

        // Cert validator
        private ICertificateValidator m_certificateValidator = new ConsoleCertificateValidator();

        /// <summary>
        /// Authentication realm
        /// </summary>
        public string AuthRealm
        {
            get
            {
                return ApplicationContext.Current.RealmId;
            }
        }

        /// <summary>
        /// Certificate validator
        /// </summary>
        public ICertificateValidator CertificateValidator
        {
            get
            {
                return this.m_certificateValidator;
            }
        }

        /// <summary>
        /// Gets the client certificate
        /// </summary>
        public IRestClientCertificateDescription ClientCertificate
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the credential provider
        /// </summary>
        public ICredentialProvider CredentialProvider
        {
            get; set;
        }

        /// <summary>
        /// Security scheme
        /// </summary>
        public SecurityScheme Mode
        {
            get; set;
        }

        /// <summary>
        /// Preemtive authentication?
        /// </summary>
        public bool PreemptiveAuthentication
        {
            get; set;
        }
    }

    /// <summary>
    /// Certificate validator
    /// </summary>
    internal class ConsoleCertificateValidator : ICertificateValidator
    {

        private static HashSet<object> m_trustedCerts = new HashSet<object>();

        /// <summary>
        /// Validate certificate
        /// </summary>
        public bool ValidateCertificate(object certificate, object chain)
        {
            if (m_trustedCerts.Contains(certificate.ToString())) return true;
            String response = String.Empty;
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                while (response != "y" && response != "n" && response != "s")
                {
                    Console.WriteLine("Certificate {0} presented by server is invalid.", certificate.ToString());
                    Console.Write("Trust this certificate? ([y]es/[n]o/[s]ession):");
                    response = Console.ReadLine();
                }
            }
            finally
            {
                Console.ResetColor();
            }

            if (response == "s")
            {
                m_trustedCerts.Add(certificate.ToString());
                return true;
            }
            else
                return response == "y";
        }
    }
}
