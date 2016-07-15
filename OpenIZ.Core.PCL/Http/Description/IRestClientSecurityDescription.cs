namespace OpenIZ.Core.Http.Description
{
	/// <summary>
	/// Represtens REST client security description
	/// </summary>
	public interface IRestClientSecurityDescription
	{
		/// <summary>
		/// Gets the certificate validator
		/// </summary>
		ICertificateValidator CertificateValidator { get; }

		/// <summary>
		/// Gets the credential provider
		/// </summary>
		ICredentialProvider CredentialProvider { get; }

		/// <summary>
		/// Gets or sets the mode of security
		/// </summary>
		SecurityScheme Mode { get; }

		/// <summary>
		/// Gets the certificate
		/// </summary>
		IRestClientCertificateDescription ClientCertificate { get; }

		/// <summary>
		/// Gets the authentication realm
		/// </summary>
		string AuthRealm { get; }
        /// <summary>
        /// When true instructs the client to pre-emptively authenticate itself
        /// </summary>
        bool PreemtiveAuthentication { get; set; }
    }

	/// <summary>
	/// Security scheme
	/// </summary>
	public enum SecurityScheme
	{
		None = 0,
		Basic = 1,
		Bearer = 2
	}
}