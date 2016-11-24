using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Represents a resource handler which is for the persistence of bundles
    /// </summary>
    public class BundleResourceHandler : IResourceHandler
    {
        /// <summary>
		/// The internal reference to the <see cref="IBatchRepositoryService"/> instance.
		/// </summary>
		private IBatchRepositoryService m_repositoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityResourceHandler"/> class.
        /// </summary>
        public BundleResourceHandler()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_repositoryService = ApplicationContext.Current.GetService<IBatchRepositoryService>();
        }

        /// <summary>
        /// Gets the resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "Bundle";
            }
        }

        /// <summary>
        /// Gets the type which this handles
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(Bundle);
            }
        }

        /// <summary>
        /// Create the specified bundle
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            else if (updateIfExists)
                throw new ArgumentOutOfRangeException(nameof(updateIfExists));

            var bundle = data as Bundle;
            if (bundle == null)
                throw new ArgumentException("Bundle required", nameof(data));

            // Submit
            return this.m_repositoryService.Insert(bundle);
        }

        /// <summary>
        /// Gets the specified data
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Obsoletes the bundle
        /// </summary>
        public IdentifiedData Obsolete(Guid key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Query for bundle
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Query bundle
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update the bundle
        /// </summary>
        public IdentifiedData Update(IdentifiedData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var bundle = data as Bundle;
            if (bundle == null)
                throw new ArgumentException("Bundle required", nameof(data));

            // Submit
            return this.m_repositoryService.Update(bundle);
        }
    }
}
