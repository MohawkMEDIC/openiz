namespace OpenIZ.Messaging.AMI.Configuration
{
    /// <summary>
    /// CA configuration information
    /// </summary>
    public class CertificationAuthorityConfiguration
    {

        /// <summary>
        /// Gets or sets the name of the certification authority
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// When true, automatically approve CA
        /// </summary>
        public bool AutoApprove { get; set; }
    }
}