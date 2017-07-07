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
using System.Linq;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
	/// <summary>
	/// Represents a reference term persistence service.
	/// </summary>
	public class ReferenceTermPersistenceService : BaseDataPersistenceService<ReferenceTerm, DbReferenceTerm>
	{
		/// <summary>
		/// Inserts a reference term.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the inserted reference term.</returns>
		public override ReferenceTerm InsertInternal(DataContext context, ReferenceTerm data, IPrincipal principal)
		{
			var referenceTerm = base.InsertInternal(context, data, principal);

			if (referenceTerm.DisplayNames != null)
			{
				base.UpdateAssociatedItems<ReferenceTermName, DbReferenceTermName>(referenceTerm.DisplayNames, data, context, principal);
			}

			return referenceTerm;
		}

		/// <summary>
		/// Updates a reference term.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the updated reference term.</returns>
		public override ReferenceTerm UpdateInternal(DataContext context, ReferenceTerm data, IPrincipal principal)
		{
			var referenceTerm = base.UpdateInternal(context, data, principal);

			if (referenceTerm.DisplayNames != null)
			{
				base.UpdateAssociatedItems<ReferenceTermName, DbReferenceTermName>(referenceTerm.DisplayNames, data, context, principal);
			}

			return referenceTerm;
		}
	}
}
