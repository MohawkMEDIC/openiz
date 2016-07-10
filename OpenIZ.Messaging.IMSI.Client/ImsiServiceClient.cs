using OpenIZ.Core.Http;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Client
{
	/// <summary>
	/// Represents the IMSI service client.
	/// </summary>
	public class ImsiServiceClient : ServiceClientBase, IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Messaging.IMSI.Client.ImsiServiceClient"/> class
		/// with a specified <see cref="OpenIZ.Core.Http.IRestClient"/> instance.
		/// </summary>
		/// <param name="client">The <see cref="OpenIZ.Core.Http.IRestClient"/> instance.</param>
		public ImsiServiceClient(IRestClient client) : base(client)
		{
			this.Client.Accept = client.Accept ?? "application/xml";
		}

		/// <summary>
		/// Creates specified data.
		/// </summary>
		/// <typeparam name="TModel">The type of data to be created.</typeparam>
		/// <param name="data">The data to be created.</param>
		/// <returns>Returns the newly created data.</returns>
		public TModel Create<TModel>(TModel data) where TModel : IdentifiedData
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// Create with version?
			if (data.Key != null)
			{
				return this.Client.Post<TModel, TModel>(String.Format("{0}/{1}", resourceName, data.Key), this.Client.Accept, data);
			}
			else
			{
				return this.Client.Post<TModel, TModel>(resourceName, this.Client.Accept, data);
			}
		}

		/// <summary>
		/// Gets a specified object by key and an optional version key.
		/// </summary>
		/// <typeparam name="TModel">The type of data for which to retrieve.</typeparam>
		/// <param name="key">The key of the data.</param>
		/// <param name="versionKey">The version key of the data.</param>
		/// <returns>Returns the specified data.</returns>
		public IdentifiedData Get<TModel>(Guid key, Guid? versionKey) where TModel : IdentifiedData
		{
			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// URL
			StringBuilder url = new StringBuilder(resourceName);

			url.AppendFormat("/{0}", key);

			if (versionKey.HasValue)
			{
				url.AppendFormat("/history/{0}", versionKey);
			}

			// Optimize?
			if (this.Client.Description.Binding.Optimize)
			{
				return this.Client.Get<Bundle>(url.ToString(), new KeyValuePair<string, object>("_bundle", true));
			}
			else
			{
				return this.Client.Get<TModel>(url.ToString());
			}
		}

		/// <summary>
		/// Gets history of the specified object.
		/// </summary>
		/// <typeparam name="TModel">The type of object for which to retrieve history.</typeparam>
		/// <param name="key">The key of the object.</param>
		/// <returns>Returns a bundle containing the history of the object.</returns>
		public Bundle History<TModel>(Guid key) where TModel : IdentifiedData
		{
			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// URL
			StringBuilder url = new StringBuilder(resourceName);
			url.AppendFormat("/{0}/history", key);

			// Request
			return this.Client.Get<Bundle>(url.ToString());
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <typeparam name="TModel">The type of data to be obsoleted.</typeparam>
		/// <param name="data">The data to obsolete.</param>
		/// <returns>Returns the obsoleted data.</returns>
		public TModel Obsolete<TModel>(TModel data) where TModel : IdentifiedData
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// Create with version?
			if (data.Key != null)
			{
				return this.Client.Delete<TModel>(String.Format("{0}/{1}", resourceName, data.Key));
			}
			else
			{
				throw new KeyNotFoundException(data.Key.ToString());
			}
		}

		/// <summary>
		/// Performs a query.
		/// </summary>
		/// <typeparam name="TModel">The type of object to query.</typeparam>
		/// <param name="query">The query parameters as a LINQ expression.</param>
		/// <returns>Returns a Bundle containing the data.</returns>
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

		/// <summary>
		/// Updates a specified object.
		/// </summary>
		/// <typeparam name="TModel">The type of data to be updated.</typeparam>
		/// <param name="data">The data to be updated.</param>
		/// <returns>Returns the updated data.</returns>
		public TModel Update<TModel>(TModel data) where TModel : IdentifiedData
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Resource name
			String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

			// Create with version?
			if (data.Key != null)
			{
				return this.Client.Put<TModel, TModel>(String.Format("{0}/{1}", resourceName, data.Key.Value), this.Client.Accept, data);
			}
			else
			{
				throw new KeyNotFoundException();
			}
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.Client?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ImsiServiceClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		/// <summary>
		/// Dispose of any resources.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}