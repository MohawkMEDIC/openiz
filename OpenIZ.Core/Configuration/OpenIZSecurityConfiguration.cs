namespace OpenIZ.Core.Configuration
{
    /// <summary>
    /// OpenIZ Security configuration
    /// </summary>
    public class OpenIzSecurityConfiguration
    {

        /// <summary>
        /// Basic authentication configuration
        /// </summary>
        public OpenIzBasicAuthorization BasicAuth { get; set; }

        /// <summary>
        /// Gets or sets the claims auth
        /// </summary>
        public OpenIzClaimsAuthorization ClaimsAuth { get; set; }
    }
}