using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpenIZ.Messaging.AMI.Model;
using OpenIZ.Messaging.AMI.Model.Auth;
using OpenIZ.Messaging.AMI.Model.Security;
using MARC.Util.CertificateTools;
using OpenIZ.Messaging.AMI.Configuration;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
    /// <summary>
    /// Represents the AMI behavior
    /// </summary>
    [ServiceBehavior(ConfigurationName = "AMI_1.0")]
    public class AmiBehavior : IAmiContract
    {
        // Certificate tool
        private CertTool m_certTool;

        // Configuration
        private AmiConfiguration m_configuration = ConfigurationManager.GetSection("openiz.messaging.ami") as AmiConfiguration;

        /// <summary>
        /// Creates the AMI behavior
        /// </summary>
        public AmiBehavior()
        {
            this.m_certTool = new CertTool();
            this.m_certTool.CertificationAuthorityName = this.m_configuration.CaConfiguration.Name;
            this.m_certTool.ServerName = this.m_configuration.CaConfiguration.ServerName;
        }

        /// <summary>
        /// Accept the signing request
        /// </summary>
        public SubmissionResult AcceptCsr(int id)
        {
            this.m_certTool.Approve(id);
            var result = new SubmissionResult(this.m_certTool.GetRequestStatus(id));
            result.Certificate = null;
            return result;
        }

        public SecurityUserInfo CreateUser(SecurityUserInfo user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Revokes a certificate
        /// </summary>
        public SubmissionResult DeleteCertificate(Int32 id , Model.Security.RevokeReason reason)
        {
            var result = this.m_certTool.GetRequestStatus(id);
            
            if (String.IsNullOrEmpty(result.AuthorityResponse))
                throw new InvalidOperationException("Cannot revoke an un-issued certificate");
            // Now get the serial key
            SignedCms importer = new SignedCms();
            importer.Decode(Convert.FromBase64String(result.AuthorityResponse));

            foreach (var cert in importer.Certificates)
                if (cert.Subject != cert.Issuer)
                    this.m_certTool.RevokeCertificate(cert.SerialNumber, (MARC.Util.CertificateTools.RevokeReason)reason);

            result.Outcome = SubmitOutcome.Revoked;
            result.AuthorityResponse = null;
            return new SubmissionResult(result);
        }

        public SecurityUserInfo DeleteUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the certificate in PKCS certificate
        /// </summary>
        public byte[] GetCertificate(int id)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs12";
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", String.Format("attachment; filename=\"crt-{0}.p12\"", id));
            var result = this.m_certTool.GetRequestStatus(id);
            return Encoding.UTF8.GetBytes(result.AuthorityResponse);
        }

        /// <summary>
        /// Get certificates
        /// </summary>
        /// <returns></returns>
        public AmiCollection<X509Certificate2Info> GetCertificates()
        {
            AmiCollection<X509Certificate2Info> collection = new AmiCollection<X509Certificate2Info>();
            var certs = this.m_certTool.GetCertificates();
            foreach (var cert in certs)
                collection.CollectionItem.Add(new X509Certificate2Info(cert.Attribute));
            return collection;
        }

        /// <summary>
        /// Get CRL
        /// </summary>
        public byte[] GetCrl()
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs7-crl";
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=\"openiz.crl\"");
            return Encoding.UTF8.GetBytes(this.m_certTool.GetCRL());
        }

        /// <summary>
        /// Get submissions
        /// </summary>
        /// <returns></returns>
        public AmiCollection<SubmissionInfo> GetCsr()
        {
            AmiCollection<SubmissionInfo> collection = new AmiCollection<SubmissionInfo>();
            var certs = this.m_certTool.GetCertificates();
            foreach (var cert in certs)
            {
                SubmissionInfo info = new SubmissionInfo();
                foreach (var kv in cert.Attribute)
                {
                    string key = kv.Key.Replace("Request.", "");
                    PropertyInfo pi = typeof(CertificateInfo).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        pi.SetValue(info, kv.Value, null);

                }
                info.XmlStatusCode = (SubmissionStatus)this.m_certTool.GetRequestStatus(Int32.Parse(info.RequestID)).Outcome;
                if (info.XmlStatusCode == SubmissionStatus.Submission)
                    collection.CollectionItem.Add(info);
            }
            return collection;
        }

        /// <summary>
        /// Get the specified CSR
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SubmissionResult GetCsr(int id)
        {
            var result = new SubmissionResult(this.m_certTool.GetRequestStatus(id));
            return result;
        }

        public XmlSchema GetSchema(int schemaId)
        {
            throw new NotImplementedException();
        }

        public SecurityUserInfo GetUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public AmiCollection<SecurityUserInfo> GetUsers()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reject the CSR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public SubmissionResult RejectCsr(int id, Model.Security.RevokeReason reason)
        {
            this.m_certTool.DenyRequest(id);
            var result = new SubmissionResult(this.m_certTool.GetRequestStatus(id));
            result.Certificate = null;
            return result;
        }

        /// <summary>
        /// Submit a CSR request
        /// </summary>
        public SubmissionResult SubmitCsr(SubmissionRequest s)
        {

            var result = new SubmissionResult(this.m_certTool.SubmitRequest(s.CmcRequest, s.AdminContactName, s.AdminAddress));
            if (this.m_configuration.CaConfiguration.AutoApprove)
                return this.AcceptCsr(result.RequestId);
            else
                return result;
        }

        public SecurityUserInfo UpdateUser(Guid userId, SecurityUserInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
