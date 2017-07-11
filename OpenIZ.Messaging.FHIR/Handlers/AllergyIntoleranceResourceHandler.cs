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
 * User: fyfej
 * Date: 2017-7-9
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Allergy / intolerance resource handler
	/// </summary>
	public class AllergyIntoleranceResourceHandler : RepositoryResourceHandlerBase<AllergyIntolerance, CodedObservation>
	{
		protected override AllergyIntolerance MapToFhir(CodedObservation model, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}

		protected override CodedObservation MapToModel(AllergyIntolerance resource, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Query which filters only allergies and intolerances
		/// </summary>
		protected override IEnumerable<CodedObservation> Query(Expression<Func<CodedObservation, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
		{
			var anyRef = base.CreateConceptSetFilter(ConceptSetKeys.AllergyIntoleranceTypes, query.Parameters[0]);
			query = Expression.Lambda<Func<CodedObservation, bool>>(Expression.AndAlso(query.Body, anyRef), query.Parameters);
			return base.Query(query, issues, queryId, offset, count, out totalResults);
		}
	}
}