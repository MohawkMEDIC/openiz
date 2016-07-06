using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using OpenIZ.Core.Model;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Core.Http;
using System.Text;

namespace OpenIZ.Messaging.IMSI.Client
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
			this.Client.Accept = this.Client.Accept ?? "application/xml";
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

        /// <summary>
        /// Gets the specified object from the IMS
        /// </summary>
        public IdentifiedData Get<TModel>(Guid key, Guid? versionKey)
            where TModel : IdentifiedData
        {

            // Resource name
            String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

            // URL
            StringBuilder url = new StringBuilder(resourceName);
            url.AppendFormat("/{0}", key);
            if (versionKey.HasValue)
                url.AppendFormat("/history/{0}", versionKey);

            // Request
            if (this.Client.Description.Binding.Optimize) // bundle
                return this.Client.Get<Bundle>(url.ToString(), new KeyValuePair<string, object>("_bundle", true));
            else
                return this.Client.Get<TModel>(url.ToString());
        }

        /// <summary>
        /// Gets history of the specified object
        /// </summary>
        public Bundle History<TModel>(Guid key)
            where TModel : IdentifiedData
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
        /// Creates the specified object on the server
        /// </summary>
        public TModel Create<TModel>(TModel data) where TModel : IdentifiedData
        {

            if (data == null)
                throw new ArgumentNullException(nameof(data));
            // Resource name
            String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

            // Create with version?
            if (data.Key != Guid.Empty)
                return this.Client.Post<TModel, TModel>(String.Format("{0}/{1}", resourceName, data.Key), this.Client.Accept, data);
            else
                return this.Client.Post<TModel, TModel>(resourceName, this.Client.Accept, data);
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        public TModel Update<TModel>(TModel data) where TModel : IdentifiedData
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            // Resource name
            String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

            // Create with version?
            if (data.Key != Guid.Empty)
                return this.Client.Put<TModel, TModel>(String.Format("{0}/{1}", resourceName, data.Key), this.Client.Accept, data);
            else
                throw new KeyNotFoundException(data.Key.ToString());

        }

        /// <summary>
        /// Obsolete the specified data
        /// </summary>
        public TModel Obsolete<TModel>(TModel data) where TModel : IdentifiedData
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            // Resource name
            String resourceName = typeof(TModel).GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;

            // Create with version?
            if (data.Key != Guid.Empty)
                return this.Client.Delete<TModel>(String.Format("{0}/{1}", resourceName, data.Key));
            else
                throw new KeyNotFoundException(data.Key.ToString());
        }

    }
}

