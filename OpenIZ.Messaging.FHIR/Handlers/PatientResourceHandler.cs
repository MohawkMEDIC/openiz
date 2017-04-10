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
 * Date: 2016-9-7
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.FHIR.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DatePrecision = OpenIZ.Core.Model.DataTypes.DatePrecision;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Represents a resource handler which can handle patients
	/// </summary>
	public class PatientResourceHandler : ResourceHandlerBase<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Patient, Core.Model.Roles.Patient>
	{
		// Repository
		private IPatientRepositoryService m_repository;

		/// <summary>
		/// Resource handler subscription
		/// </summary>
		public PatientResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
		}

		/// <summary>
		/// Create the specified patient instance
		/// </summary>
		protected override Core.Model.Roles.Patient Create(Core.Model.Roles.Patient modelInstance, List<IResultDetail> issues, TransactionMode mode)
		{
			return this.m_repository.Insert(modelInstance);
		}

		/// <summary>
		/// Delete the specified patient
		/// </summary>
		protected override Core.Model.Roles.Patient Delete(Guid modelId, List<IResultDetail> details)
		{
			return this.m_repository.Obsolete(modelId);
		}

		/// <summary>
		/// Map a patient object to FHIR
		/// </summary>
		protected override MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Patient MapToFhir(Core.Model.Roles.Patient model)
		{
			var retVal = DataTypeConverter.CreateResource<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Patient>(model);
			retVal.Active = model.StatusConceptKey == StatusKeys.Active;
			retVal.Address = model.Addresses.Select(o => DataTypeConverter.ToFhirAddress(o)).ToList();
			retVal.BirthDate = model.DateOfBirth;
			retVal.Deceased = model.DeceasedDate == DateTime.MinValue ? (object)new FhirBoolean(true) : model.DeceasedDate != null ? new FhirDate(model.DeceasedDate.Value) : null;
			retVal.Gender = DataTypeConverter.ToFhirCodeableConcept(model.GenderConcept)?.GetPrimaryCode()?.Code;
			retVal.Identifier = model.Identifiers?.Select(o => DataTypeConverter.ToFhirIdentifier(o)).ToList();
			retVal.MultipleBirth = model.MultipleBirthOrder == 0 ? (FhirElement)new FhirBoolean(true) : model.MultipleBirthOrder.HasValue ? new FhirInt(model.MultipleBirthOrder.Value) : null;
			retVal.Name = model.Names.Select(o => DataTypeConverter.ToFhirHumanName(o)).ToList();
			retVal.Timestamp = model.ModifiedOn.DateTime;
			retVal.Telecom = model.Telecoms.Select(o => DataTypeConverter.ToFhirTelecom(o)).ToList();

			// TODO: Relationships
			foreach (var rel in model.Relationships.Where(o => !o.InversionIndicator))
			{
				// Family member
				if (rel.RelationshipType.ConceptSets.Any(o => o.Key == ConceptSetKeys.FamilyMember))
				{
					// Create the relative object
					var relative = DataTypeConverter.CreateResource<RelatedPerson>(rel.TargetEntity);
					relative.Relationship = DataTypeConverter.ToFhirCodeableConcept(rel.RelationshipType);
					relative.Address = DataTypeConverter.ToFhirAddress(rel.TargetEntity.Addresses.FirstOrDefault());
					relative.Gender = DataTypeConverter.ToFhirCodeableConcept((rel.TargetEntity as Core.Model.Roles.Patient)?.GenderConcept);
					relative.Identifier = rel.TargetEntity.Identifiers.Select(o => DataTypeConverter.ToFhirIdentifier(o)).ToList();
					relative.Name = DataTypeConverter.ToFhirHumanName(rel.TargetEntity.Names.FirstOrDefault());
					if (rel.TargetEntity is Core.Model.Roles.Patient)
						relative.Patient = DataTypeConverter.CreateReference<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Patient>(rel.TargetEntity);
					relative.Telecom = rel.TargetEntity.Telecoms.Select(o => DataTypeConverter.ToFhirTelecom(o)).ToList();
					retVal.Contained.Add(new ContainedResource()
					{
						Item = relative
					});
				}
				else if (rel.RelationshipTypeKey == EntityRelationshipTypeKeys.HealthcareProvider)
					retVal.Provider = DataTypeConverter.CreateReference<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Practictioner>(rel.TargetEntity);
			}

			// TODO: Links
			return retVal;
		}

		/// <summary>
		/// Maps to model.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <returns>Returns the mapped model.</returns>
		protected override Core.Model.Roles.Patient MapToModel(MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Patient resource)
		{
			var patient = new Core.Model.Roles.Patient
			{
				Addresses = resource.Address.Select(DataTypeConverter.ToEntityAddress).ToList(),
				CreationTime = DateTimeOffset.Now,
				DateOfBirth = resource.BirthDate?.DateValue,
				// TODO: Extensions
				Extensions = resource.Extension.Select(DataTypeConverter.ToEntityExtension).ToList(),
				GenderConceptKey = DataTypeConverter.ToConcept(new FhirCoding(new Uri("http://hl7.org/fhir/administrative-gender"), resource.Gender?.Value))?.Key,
				Identifiers = resource.Identifier.Select(DataTypeConverter.ToEntityIdentifier).ToList(),
				LanguageCommunication = resource.Communication.Select(DataTypeConverter.ToPersonLanguageCommunication).ToList(),
				Key = Guid.NewGuid(),
				Names = resource.Name.Select(DataTypeConverter.ToEntityName).ToList(),
				Relationships = resource.Contact.Select(DataTypeConverter.ToEntityRelationship).ToList(),
				StatusConceptKey = resource.Active?.Value == true ? StatusKeys.Active : StatusKeys.Obsolete,
				Telecoms = resource.Telecom.Select(DataTypeConverter.ToEntityTelecomAddress).ToList()
			};

			Guid key;

			if (!Guid.TryParse(resource.Id, out key))
			{
				key = Guid.NewGuid();
			}

			patient.Key = key;

			if (resource.Deceased is FhirDateTime)
			{
				patient.DeceasedDate = (FhirDateTime)resource.Deceased;
			}
			else if (resource.Deceased is FhirBoolean)
			{
				// we don't have a field for "deceased indicator" to say that the patient is dead, but we don't know that actual date/time of death
				// should find a better way to do this
				patient.DeceasedDate = DateTime.Now;
				patient.DeceasedDatePrecision = DatePrecision.Year;
			}

			if (resource.MultipleBirth is FhirBoolean)
			{
				patient.MultipleBirthOrder = 0;
			}
			else if (resource.MultipleBirth is FhirInt)
			{
				patient.MultipleBirthOrder = ((FhirInt)resource.MultipleBirth).Value;
			}

			return patient;
		}

		/// <summary>
		/// Query for patients
		/// </summary>
		protected override IEnumerable<Core.Model.Roles.Patient> Query(Expression<Func<Core.Model.Roles.Patient, bool>> query, List<IResultDetail> issues, int offset, int count, out int totalResults)
		{
			Expression<Func<Core.Model.Roles.Patient, bool>> filter = o => o.ClassConceptKey == EntityClassKeys.Patient && o.ObsoletionTime == null;
			var parm = Expression.Parameter(typeof(Core.Model.Roles.Patient));
			query = Expression.Lambda<Func<Core.Model.Roles.Patient, bool>>(Expression.AndAlso(Expression.Invoke(filter, parm), Expression.Invoke(query, parm)), parm);
			return this.m_repository.Find(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Retrieves the specified patient
		/// </summary>
		protected override Core.Model.Roles.Patient Read(Identifier<Guid> id, List<IResultDetail> details)
		{
			return this.m_repository.Get(id.Id, id.VersionId);
		}

		/// <summary>
		/// Update the specified resource
		/// </summary>
		protected override Core.Model.Roles.Patient Update(Core.Model.Roles.Patient model, List<IResultDetail> details, TransactionMode mode)
		{
			return this.m_repository.Save(model);
		}
	}
}