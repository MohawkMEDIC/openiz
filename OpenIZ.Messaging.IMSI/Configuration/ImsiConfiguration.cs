namespace OpenIZ.Messaging.IMSI.Configuration
{
    /// <summary>
    /// Configuration class for IMSI configuration
    /// </summary>
    public class ImsiConfiguration
    {

        /// <summary>
        /// Creates a new IMSI configuration
        /// </summary>
        public ImsiConfiguration(string wcfServiceName)
        {
            this.WcfServiceName = wcfServiceName;
        }

        /// <summary>
        /// Gets the wcf service name
        /// </summary>
        public string WcfServiceName { get; private set; }
    }
}