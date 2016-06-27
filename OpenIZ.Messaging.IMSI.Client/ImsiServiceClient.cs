using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Core.Model;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.PCL.Interop.Clients;
using OpenIZ.Core.PCL.Http;

namespace OpenIZ.Mobile.Core.Interop.Clients
{
	/// <summary>
	/// Represents the IMSI service client 
	/// </summary>
	public class ImsiServiceClient : ServiceClientBase
	{

		/// <summary>
		/// Creates a new service client
		/// </summary>
		/// <param name="clientName">Client name.</param>
		public ImsiServiceClient (IRestClient client) : base(client)
		{
			this.Client.Accept = "application/json";
		}

		/// <summary>
		/// Perform a query
		/// </summary>
		/// <param name="query">Query.</param>
		public Bundle Query<TModel>(Expression<Func<TModel, bool>> query) where TModel : IdentifiedData
		{
			// Map the query to HTTP parameters
			var queryParms = QueryExpressionBuilder.BuildQuery(query);

			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// The IMSI uses the XMLName as the root of the request
			var retVal = this.Client.Get<Bundle>(resourceName, queryParms.ToArray());

			// Return value
			return retVal;
		}

	}
}

