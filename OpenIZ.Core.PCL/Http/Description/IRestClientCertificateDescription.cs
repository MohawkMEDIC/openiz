namespace OpenIZ.Core.Http.Description
{
    /// <summary>
    /// Certificate description
    /// </summary>
    public interface IRestClientCertificateDescription
    {
        /// <summary>
        /// Gets the type of find algorithm of X509FindType
        /// </summary>
        string FindType { get; }
        /// <summary>
        /// Gets the name of the certificate store
        /// </summary>
        string StoreName { get; }
        /// <summary>
        /// Gets the location of the certificate store
        /// </summary>
        string StoreLocation { get; }
        /// <summary>
        /// Gets the find value
        /// </summary>
        string FindValue { get; }
    }
}