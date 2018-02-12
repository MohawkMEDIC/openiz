/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using SwaggerWcf.Attributes;
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
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(409, "You are attempting to create a resource that already exists")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(201, "The object was created successfully")]
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
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(409, "You are attempting to perform an obsolete on an old version of the resource, or the conditional HTTP headers don't match the current version of the resource")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The object was obsoleted successfully")]
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
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
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
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
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
		[SwaggerWcfTag("Administrative Management Interface (AMI)")]
		[SwaggerWcfSecurity("OAUTH2")]
		[SwaggerWcfResponse(503, "The AMI service is unavailable (for example: Server is still starting up, or didn't start up correctly)")]
		[SwaggerWcfResponse(403, "User attempted to perform an operation but they are unauthorized to do so")]
		[SwaggerWcfResponse(401, "Operation requires authentication")]
		[SwaggerWcfResponse(429, "The server throttling has been exceeded")]
		[SwaggerWcfResponse(400, "The client has made a request that this server cannot fulfill")]
		[SwaggerWcfResponse(405, "You are not allowed to perform this operation on this resource")]
		[SwaggerWcfResponse(501, "The method / operation you are calling is not implemented")]
		[SwaggerWcfResponse(404, "The provided resource could not be found")]
		[SwaggerWcfResponse(422, "The operation resulted in one or more business rules being violated")]
		[SwaggerWcfResponse(200, "The operation was successful, and the most recent version of the resource is in the response")]
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