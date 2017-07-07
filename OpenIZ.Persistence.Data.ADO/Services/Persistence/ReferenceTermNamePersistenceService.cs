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
 * User: justi
 * Date: 2017-6-21
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Interfaces;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
	/// <summary>
	/// Represents a reference term name persistence service.
	/// </summary>
	/// <seealso cref="DbReferenceTermName" />
	public class ReferenceTermNamePersistenceService : BaseDataPersistenceService<ReferenceTermName, DbReferenceTermName>
	{
		/// <summary>
		/// Converts a domain instance into a model instance.
		/// </summary>
		/// <param name="modelInstance">Model instance.</param>
		/// <param name="context">Context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>The model instance.</returns>
		public override object FromModelInstance(ReferenceTermName modelInstance, DataContext context, IPrincipal principal)
		{
			var domainInstance = base.FromModelInstance(modelInstance, context, principal) as DbReferenceTermName;

			var phoneticCoder = ApplicationContext.Current.GetService<IPhoneticAlgorithmHandler>();

			domainInstance.PhoneticAlgorithm = phoneticCoder?.AlgorithmId ?? PhoneticAlgorithmKeys.None;
			domainInstance.PhoneticCode = phoneticCoder?.GenerateCode(modelInstance.Name);

			return domainInstance;
		}

		/// <summary>
		/// Performs the actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the inserted reference term name.</returns>
		public override ReferenceTermName InsertInternal(DataContext context, ReferenceTermName data, IPrincipal principal)
		{
			// set the key if we don't have one
			if (!data.Key.HasValue || data.Key == Guid.Empty)
				data.Key = Guid.NewGuid();

			// set the creation time if we don't have one
			if (data.CreationTime == default(DateTimeOffset))
				data.CreationTime = DateTimeOffset.Now;

			return base.InsertInternal(context, data, principal);
		}
	}
}
