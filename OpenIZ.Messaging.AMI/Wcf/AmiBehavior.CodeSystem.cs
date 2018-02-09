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
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		/// <exception cref="System.InvalidOperationException">IMetadataRepositoryService</exception>
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
		public CodeSystem CreateCodeSystem(CodeSystem codeSystem)
		{
			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.CreateCodeSystem(codeSystem);
		}

		/// <summary>
		/// Deletes the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <returns>Returns the deleted code system.</returns>
		/// <exception cref="System.ArgumentException">codeSystemId</exception>
		/// <exception cref="System.InvalidOperationException">IMetadataRepositoryService</exception>
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
		public CodeSystem DeleteCodeSystem(string codeSystemId)
		{
			Guid id;

			if (!Guid.TryParse(codeSystemId, out id))
			{
				throw new ArgumentException($"{nameof(codeSystemId)} must be a valid GUID");
			}

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.DeleteCodeSystem(id);
		}

		/// <summary>
		/// Gets the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <returns>Returns a code system.</returns>
		/// <exception cref="System.ArgumentException">codeSystemId</exception>
		/// <exception cref="System.InvalidOperationException">IMetadataRepositoryService</exception>
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
		public CodeSystem GetCodeSystem(string codeSystemId)
		{
			Guid id;

			if (!Guid.TryParse(codeSystemId, out id))
			{
				throw new ArgumentException($"{nameof(codeSystemId)} must be a valid GUID");
			}

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.GetCodeSystem(id);
		}

		/// <summary>
		/// Gets the code systems.
		/// </summary>
		/// <returns>Returns a list of code systems.</returns>
		/// <exception cref="System.ArgumentException">parameters</exception>
		/// <exception cref="System.InvalidOperationException">IMetadataRepositoryService</exception>
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
		public AmiCollection<CodeSystem> GetCodeSystems()
		{
			var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

			if (parameters.Count == 0)
			{
				throw new ArgumentException($"{nameof(parameters)} cannot be empty");
			}

			var expression = QueryExpressionParser.BuildLinqExpression<CodeSystem>(this.CreateQuery(parameters));

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			var codeSystems = new AmiCollection<CodeSystem>();

			int totalCount;
			codeSystems.CollectionItem = metadataService.FindCodeSystem(expression, 0, null, out totalCount).ToList();
			codeSystems.Size = totalCount;

			return codeSystems;
		}

		/// <summary>
		/// Updates the code system.
		/// </summary>
		/// <param name="codeSystemId">The code system identifier.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Return the updated code system.</returns>
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
		public CodeSystem UpdateCodeSystem(string codeSystemId, CodeSystem codeSystem)
		{
			Guid id;

			if (!Guid.TryParse(codeSystemId, out id))
			{
				throw new ArgumentException($"{nameof(codeSystemId)} must be a valid GUID");
			}

			if (id != codeSystem.Key)
			{
				throw new ArgumentException($"Unable to update extension type using id: {id}, and id: {codeSystem.Key}");
			}

			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			if (metadataService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
			}

			return metadataService.UpdateCodeSystem(codeSystem);
		}
	}
}