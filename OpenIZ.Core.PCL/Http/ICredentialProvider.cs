using System;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Authorization event args
	/// </summary>
	public interface ICredentialProvider 
	{

		/// <summary>
		/// Gets or sets the credentials which are used to authenticate
		/// </summary>
		Credentials GetCredentials(IRestClient context);

		/// <summary>
		/// Authenticate a user in the credential.
		/// </summary>
		/// <param name="context">Context.</param>
		Credentials Authenticate(IRestClient context);
	}

}

