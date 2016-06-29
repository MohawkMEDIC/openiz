using System;
using System.Security.Principal;
using System.Net;
using System.Collections.Generic;

namespace OpenIZ.Core.Http
{
	/// <summary>
	/// Represents a series of credentials which are used when accessing the mobile core
	/// </summary>
	public abstract class Credentials
	{

		// Principal
		private IPrincipal m_principal;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Authentication.Credentials"/> class.
		/// </summary>
		/// <param name="principal">Principal.</param>
		protected Credentials(IPrincipal principal)
		{
			this.m_principal = principal;
		}

		/// <summary>
		/// Gets the principal represented by this credential
		/// </summary>
		/// <value>The principal.</value>
		public virtual IPrincipal Principal { get { return this.m_principal; } }

		/// <summary>
		/// Get the http headers which are requried for the credential
		/// </summary>
		public abstract Dictionary<String, String> GetHttpHeaders();
	}

}

