using OpenIZ.Messaging.AMI.Model;
using OpenIZ.Messaging.AMI.Model.Auth;
using OpenIZ.Messaging.AMI.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace OpenIZ.Messaging.AMI.Wcf
{
    /// <summary>
    /// Administrative management interface contract
    /// </summary>
    [ServiceContract(ConfigurationName = "AMI_1.0", Name = "AMI"), XmlSerializerFormat]
    [ServiceKnownType(typeof(SubmissionRequest))]
    [ServiceKnownType(typeof(SubmissionResult))]
    [ServiceKnownType(typeof(SubmissionInfo))]
    [ServiceKnownType(typeof(X509Certificate2Info))]
    [ServiceKnownType(typeof(SecurityUserInfo))]
    [ServiceKnownType(typeof(SecurityRoleInfo))]
    [ServiceKnownType(typeof(AmiCollection<SubmissionInfo>))]
    [ServiceKnownType(typeof(AmiCollection<X509Certificate2Info>))]
    [ServiceKnownType(typeof(AmiCollection<SecurityUserInfo>))]
    public interface IAmiContract
    {
        
        /// <summary>
        /// Submit the specified CSR
        /// </summary>
        [WebInvoke(UriTemplate = "csr/", BodyStyle = WebMessageBodyStyle.Bare, Method ="POST")]
        SubmissionResult SubmitCsr(SubmissionRequest s);

        /// <summary>
        /// Get the submissions
        /// </summary>
        [WebGet(UriTemplate = "csr/", BodyStyle = WebMessageBodyStyle.Bare)]
        AmiCollection<SubmissionInfo> GetCsrs();

        /// <summary>
        /// Reject the specified CSR
        /// </summary>
        [WebInvoke(UriTemplate = "csr/{certId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        SubmissionResult RejectCsr(string certId, RevokeReason reason);

        /// <summary>
        /// Gets the specified CSR request
        /// </summary>
        [WebGet(UriTemplate = "csr/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        SubmissionResult GetCsr(string id);

        /// <summary>
        /// Accept the CSR
        /// </summary>
        [WebInvoke(UriTemplate = "csr/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
        SubmissionResult AcceptCsr(string id);

        /// <summary>
        /// Get the CRL
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/crl", BodyStyle = WebMessageBodyStyle.Bare)]
        byte[] GetCrl();

        /// <summary>
        /// Get specified certificates
        /// </summary>
        [WebGet(UriTemplate = "certificate/", BodyStyle = WebMessageBodyStyle.Bare)]
        AmiCollection<X509Certificate2Info> GetCertificates();
         
        /// <summary>
        /// Get the specified certificate
        /// </summary>
        [WebGet(UriTemplate = "certificate/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        byte[] GetCertificate(string id);

        /// <summary>
        /// Reject the specified CSR
        /// </summary>
        [WebInvoke(UriTemplate = "certificate/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        SubmissionResult DeleteCertificate(string id, RevokeReason reason);

        /// <summary>
        /// Security user information
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "user/", BodyStyle = WebMessageBodyStyle.Bare)]
        AmiCollection<SecurityUserInfo> GetUsers();

        /// <summary>
        /// Create a security user 
        /// </summary>
        [WebInvoke(UriTemplate = "user/", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
        SecurityUserInfo CreateUser(SecurityUserInfo user);

        /// <summary>
        /// Get a security user
        /// </summary>
        [WebGet(UriTemplate = "user/{userId}", BodyStyle = WebMessageBodyStyle.Bare)]
        SecurityUserInfo GetUser(string userId);

        /// <summary>
        /// Get a security user
        /// </summary>
        [WebInvoke(UriTemplate = "user/{userId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
        SecurityUserInfo UpdateUser(string userId, SecurityUserInfo info);

        /// <summary>
        /// Delete a security user
        /// </summary>
        [WebInvoke(UriTemplate = "user/{userId}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        SecurityUserInfo DeleteUser(string userId);

        /// <summary>
        /// Get the schema
        /// </summary>
        [WebGet(UriTemplate = "/?xsd={schemaId}")]
        XmlSchema GetSchema(int schemaId);
    }
}
