namespace OpenIZ.Core.Http.Description
{
	/// <summary>
	/// REST based client endpoint description
	/// </summary>
	public interface IRestClientEndpointDescription
	{
		/// <summary>
		/// Gets the address of the endpoint
		/// </summary>
		string Address { get; }
	}
}