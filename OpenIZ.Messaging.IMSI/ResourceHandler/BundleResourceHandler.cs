using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a resource handler which is for the persistence of bundles.
	/// </summary>
	public class BundleResourceHandler : IResourceHandler
	{
		/// <summary>
		/// The internal reference to the <see cref="IBatchRepositoryService"/> instance.
		/// </summary>
		private IBatchRepositoryService repositoryService;

		/// <summary>
		/// Initializes a new instance of the <see cref="BundleResourceHandler"/> class.
		/// </summary>
		public BundleResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repositoryService = ApplicationContext.Current.GetService<IBatchRepositoryService>();
		}

		/// <summary>
		/// Gets the resource name which this resource handler handles.
		/// </summary>
		public string ResourceName => "Bundle";

		/// <summary>
		/// Gets the type which this resource handler handles.
		/// </summary>
		public Type Type => typeof(Bundle);

		/// <summary>
		/// Creates a bundle.
		/// </summary>
		/// <param name="data">The data to create.</param>
		/// <param name="updateIfExists">Whether to update an existing entity.</param>
		/// <returns>Returns the created bundle.</returns>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}
			var bundle = data as Bundle;

			if (bundle == null)
			{
				throw new ArgumentException("Bundle required", nameof(data));
			}

			bundle.Reconstitute();

			if (updateIfExists)
			{
				return this.repositoryService.Update(bundle);
			}
			else
			{
				// Submit
				return this.repositoryService.Insert(bundle);
			}
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
		/// Updates a specific bundle.
		/// </summary>
		/// <param name="data">The data to be updated.</param>
		/// <returns>Returns the updated data.</returns>
		public IdentifiedData Update(IdentifiedData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var bundle = data as Bundle;

			if (bundle == null)
			{
				throw new ArgumentException("Bundle required", nameof(data));
			}

			// Submit
			return this.repositoryService.Update(bundle);
		}
	}
}