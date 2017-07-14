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

using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.FHIR.Util;
using System;
using System.Linq;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Facility resource handler
	/// </summary>
	public class LocationResourceHandler : RepositoryResourceHandlerBase<Location, Place>
	{
		/// <summary>
		/// Map the inbound place to a FHIR model
		/// </summary>
		protected override Location MapToFhir(Place model, WebOperationContext webOperationContext)
		{
			Location retVal = DataTypeConverter.CreateResource<Location>(model);
			retVal.Identifier = model.Identifiers.Select(o => DataTypeConverter.ToFhirIdentifier<Entity>(o)).ToList();

			// Map status
			if (model.StatusConceptKey == StatusKeys.Active)
				retVal.Status = LocationStatus.Active;
			else if (model.StatusConceptKey == StatusKeys.Obsolete)
				retVal.Status = LocationStatus.Inactive;
			else
				retVal.Status = LocationStatus.Suspended;

			retVal.Name = model.LoadCollection<EntityName>("Names").FirstOrDefault(o => o.NameUseKey == NameUseKeys.OfficialRecord)?.LoadCollection<EntityNameComponent>("Component")?.FirstOrDefault()?.Value;
			retVal.Alias = model.LoadCollection<EntityName>("Names").Where(o => o.NameUseKey != NameUseKeys.OfficialRecord)?.Select(n => (FhirString)n.LoadCollection<EntityNameComponent>("Component")?.FirstOrDefault()?.Value).ToList();

			// Convert the determiner code
			if (model.DeterminerConceptKey == DeterminerKeys.Described)
				retVal.Mode = LocationMode.Kind;
			else
				retVal.Mode = LocationMode.Instance;

			retVal.Type = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"), "http://hl7.org/fhir/ValueSet/v3-ServiceDeliveryLocationRoleType");

			retVal.Telecom = model.LoadCollection<EntityTelecomAddress>("Telecoms").Select(o => DataTypeConverter.ToFhirTelecom(o)).ToList();
			retVal.Address = DataTypeConverter.ToFhirAddress(model.LoadCollection<EntityAddress>("Addresses").FirstOrDefault());

			if (model.Lat.HasValue && model.Lng.HasValue)
				retVal.Position = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.Position()
				{
					Latitude = (decimal)model.Lat.Value,
					Longitude = (decimal)model.Lng.Value
				};

			// Part of?
			var parent = model.LoadCollection<EntityRelationship>("Relationships").FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
			if (parent != null)
				retVal.PartOf = DataTypeConverter.CreateReference<Location>(parent.LoadProperty<Entity>("TargetEntity"), webOperationContext);

			return retVal;
		}

		protected override Place MapToModel(Location resource, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}
	}
}