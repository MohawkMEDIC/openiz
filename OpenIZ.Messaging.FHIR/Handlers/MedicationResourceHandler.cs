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
	/// Represents a medication resource handler
	/// </summary>
	public class MedicationResourceHandler : RepositoryResourceHandlerBase<Medication, ManufacturedMaterial>
	{
		/// <summary>
		/// Map this manufactured material to FHIR
		/// </summary>
		protected override Medication MapToFhir(ManufacturedMaterial model, WebOperationContext webOperationContext)
		{
			var retVal = DataTypeConverter.CreateResource<Medication>(model);

			// Code of medication code
			retVal.Code = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"));

			if (model.StatusConceptKey == StatusKeys.Active)
				retVal.Status = SubstanceStatus.Active;
			else if (model.StatusConceptKey == StatusKeys.Obsolete)
				retVal.Status = SubstanceStatus.Inactive;
			else if (model.StatusConceptKey == StatusKeys.Nullified)
				retVal.Status = SubstanceStatus.Nullified;

			// Is brand?
			retVal.IsBrand = false;
			retVal.IsOverTheCounter = model.Tags.Any(o => o.TagKey == "isOtc");

			var manufacturer = model.LoadCollection<EntityRelationship>("Relationships").FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct);
			if (manufacturer != null)
				retVal.Manufacturer = DataTypeConverter.CreateReference<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Organization>(manufacturer.LoadProperty<Entity>("TargetEntity"), webOperationContext);

			// Form
			retVal.Form = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("FormConcept"), "http://hl7.org/fhir/ValueSet/medication-form-codes");
			retVal.Package = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationPackage();
			retVal.Package.Batch = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationBatch()
			{
				LotNumber = model.LotNumber,
				Expiration = model.ExpiryDate
			};

			// Picture of the object?

			var photo = model.LoadCollection<EntityExtension>("Extensions").FirstOrDefault(o => o.ExtensionTypeKey == ExtensionTypeKeys.JpegPhotoExtension);
			if (photo != null)
				retVal.Image = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.Attachment()
				{
					ContentType = "image/jpg",
					Data = photo.ExtensionValueXml
				};
			return retVal;
		}

		protected override ManufacturedMaterial MapToModel(Medication resource, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}
	}
}