using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;

namespace OpenIZ.Core.Interop.Clients
{
	/// <summary>
	/// Represents a basic service client
	/// </summary>
	public abstract class ServiceClientBase
	{
		// The configuration
		private IRestClientDescription m_configuration;

		// The rest client
		private IRestClient m_restClient;

		/// <summary>
		/// Gets the client.
		/// </summary>
		/// <value>The client.</value>
		protected IRestClient Client
		{
			get
			{
				return this.m_restClient;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Interop.Clients.ServiceClientBase"/> class.
		/// </summary>
		/// <param name="clientName">Client name.</param>
		public ServiceClientBase(IRestClient restClient)
		{
			this.m_restClient = restClient;
			this.m_configuration = this.m_restClient.Description;
		}
	}
}