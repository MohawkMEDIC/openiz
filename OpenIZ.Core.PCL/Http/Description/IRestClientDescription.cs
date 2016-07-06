using System.Collections.Generic;

namespace OpenIZ.Core.Http.Description
{
	/// <summary>
	/// Represents a description of a service
	/// </summary>
	public interface IRestClientDescription
	{
		/// <summary>
		/// Gets or sets the endpoints for the client
		/// </summary>
		List<IRestClientEndpointDescription> Endpoint { get; }

		/// <summary>
		/// Gets or sets the binding for the service client.
		/// </summary>
		IRestClientBindingDescription Binding { get; }
	}
}