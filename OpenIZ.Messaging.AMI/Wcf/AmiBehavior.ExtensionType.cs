/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may
 * obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * User: khannan
 * Date: 2017-4-18
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.AMI.Wcf
{
	/// <summary>
	/// Represents the administrative contract interface.
	/// </summary>
	public partial class AmiBehavior
	{
		/// <summary>
		/// Creates the type of the extension.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the created extension type.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public ExtensionType CreateExtensionType(ExtensionType extensionType)
		{
			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.CreateExtensionType(extensionType);
		}

		/// <summary>
		/// Deletes the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <returns>Returns the deleted extension type.</returns>
		/// <exception cref="System.ArgumentException">extensionTypeId</exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public ExtensionType DeleteExtensionType(string extensionTypeId)
		{
			Guid id;

			if (!Guid.TryParse(extensionTypeId, out id))
			{
				throw new ArgumentException($"{nameof(extensionTypeId)} must be a valid GUID");
			}

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.DeleteExtensionType(id);
		}

		/// <summary>
		/// Gets the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <returns>Returns the extension type, or null if no extension type is found.</returns>
		/// <exception cref="System.ArgumentException">extensionTypeId</exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public ExtensionType GetExtensionType(string extensionTypeId)
		{
			Guid id;

			if (!Guid.TryParse(extensionTypeId, out id))
			{
				throw new ArgumentException($"{nameof(extensionTypeId)} must be a valid GUID");
			}

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.GetExtensionType(id);
		}

		/// <summary>
		/// Gets the extension types.
		/// </summary>
		/// <returns>Returns a list of extension types.</returns>
		/// <exception cref="System.ArgumentException">parameters</exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public AmiCollection<ExtensionType> GetExtensionTypes()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException($"{nameof(parameters)} cannot be empty");
			}

			var expression = QueryExpressionParser.BuildLinqExpression<ExtensionType>(this.CreateQuery(parameters));

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			var extensionTypes = new AmiCollection<ExtensionType>();

			int totalCount;
			extensionTypes.CollectionItem = metadataService.FindExtensionType(expression, 0, null, out totalCount).ToList();
			extensionTypes.Size = totalCount;

			return extensionTypes;
		}

		/// <summary>
		/// Updates the type of the extension.
		/// </summary>
		/// <param name="extensionTypeId">The extension type identifier.</param>
		/// <param name="extensionType">Type of the extension.</param>
		/// <returns>Returns the updated extension type.</returns>
		/// <exception cref="System.ArgumentException">
		/// extensionTypeId
		/// or
		/// </exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public ExtensionType UpdateExtensionType(string extensionTypeId, ExtensionType extensionType)
		{
			Guid id;

			if (!Guid.TryParse(extensionTypeId, out id))
			{
				throw new ArgumentException($"{nameof(extensionTypeId)} must be a valid GUID");
			}

			if (id != extensionType.Key)
			{
				throw new ArgumentException($"Unable to update extension type using id: {id}, and id: {extensionType.Key}");
			}

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.UpdateExtensionType(extensionType);
		}
	}
}