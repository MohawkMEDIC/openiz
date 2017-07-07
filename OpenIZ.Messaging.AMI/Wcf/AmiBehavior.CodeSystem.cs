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
 * Date: 2017-4-8
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
		/// Creates the code system.
		/// </summary>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the created code system.</returns>
		/// <exception cref="System.InvalidOperationException">IMetadataRepositoryService</exception>
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