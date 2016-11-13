/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-12
 */

using MARC.HI.EHRS.SVC.Core;
using NHapi.Model.V25.Segment;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.HL7.Configuration;
using OpenIZ.Messaging.HL7.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using NHapi.Model.V25.Datatype;
using TS = MARC.Everest.DataTypes.TS;

namespace OpenIZ.Messaging.HL7.Notifier
{
	/// <summary>
	/// Represents a base notifier.
	/// </summary>
	public abstract class NotifierBase
	{
		/// <summary>
		/// The protected reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		protected readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// The internal reference to the <see cref="IAssigningAuthorityRepositoryService"/> instance.
		/// </summary>
		private static IAssigningAuthorityRepositoryService assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

		/// <summary>
		/// Initializes a new instance of the <see cref="NotifierBase"/> class.
		/// </summary>
		protected NotifierBase()
		{

		}

		internal static void UpdateAD(EntityAddress entityAddress, XAD address)
		{
			var addressUse = entityAddress.AddressUse?.Key;

			if (addressUse != null)
			{
				address.AddressType.Value = MessageUtil.ReverseLookup<string, Guid>(MessageUtil.AddressUseMap, addressUse.ToGuid());
			}

			address.CensusTract.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.CensusTract).Select(c => c.Value));
			address.City.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.City).Select(c => c.Value));
			address.Country.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.Country).Select(c => c.Value));
			address.StateOrProvince.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.State).Select(c => c.Value));
			address.StreetAddress.StreetName.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.StreetName).Select(c => c.Value));
			address.StreetAddress.StreetOrMailingAddress.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.StreetAddressLine).Select(c => c.Value));
			address.ZipOrPostalCode.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Select(c => c.Value));
		}

		/// <summary>
		/// Updates a <see cref="MSH"/> segment.
		/// </summary>
		/// <param name="msh">The MSH segment to update.</param>
		/// <param name="targetConfiguration">The target configuration.</param>
		internal static void UpdateMSH(MSH msh, TargetConfiguration targetConfiguration)
		{
			msh.AcceptAcknowledgmentType.Value = "AL";
			msh.DateTimeOfMessage.Time.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
			msh.MessageControlID.Value = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString();
			msh.ProcessingID.ProcessingID.Value = "P";

			if (targetConfiguration.DeviceId.Contains('|'))
			{
				msh.ReceivingApplication.NamespaceID.Value = targetConfiguration.DeviceId.Split('|')[0];
				msh.ReceivingFacility.NamespaceID.Value = targetConfiguration.DeviceId.Split('|')[1];
			}
			else
			{
				msh.ReceivingApplication.NamespaceID.Value = targetConfiguration.DeviceId;
				msh.ReceivingFacility.NamespaceID.Value = targetConfiguration.NotificationDomainConfigurations.FirstOrDefault()?.Domain;
			}

			var configuration = ApplicationContext.Current.Configuration;

			msh.SendingApplication.NamespaceID.Value = configuration.DeviceName;
			msh.SendingFacility.NamespaceID.Value = configuration.JurisdictionData.Name;
		}

		/// <summary>
		/// Updates a <see cref="PID"/> segment.
		/// </summary>
		/// <param name="patient">The patient to use to update the PID segment.</param>
		/// <param name="pid">The PID segment to update.</param>
		/// <param name="targetConfiguration">The target configuration.</param>
		internal static void UpdatePID(Patient patient, PID pid, TargetConfiguration targetConfiguration)
		{
			switch (patient.GenderConcept.Mnemonic)
			{
				case "male":
					pid.AdministrativeSex.Value = "M";
					break;

				case "female":
					pid.AdministrativeSex.Value = "F";
					break;

				case "undifferentiated":
					pid.AdministrativeSex.Value = "U";
					break;
			}

			if (patient.MultipleBirthOrder.HasValue)
			{
				pid.BirthOrder.Value = patient.MultipleBirthOrder.ToString();
				pid.MultipleBirthIndicator.Value = "Y";
			}

			if (patient.DateOfBirth.HasValue)
			{
				pid.DateTimeOfBirth.Time.Value = (TS)patient.DateOfBirth.Value;
			}

			if (patient.DeceasedDate.HasValue)
			{
				pid.PatientDeathDateAndTime.Time.Value = (TS)patient.DeceasedDate.Value;
				pid.PatientDeathIndicator.Value = "Y";
			}

			foreach (var address in patient.Addresses)
			{
				NotifierBase.UpdateAD(address, pid.GetPatientAddress(pid.PatientAddressRepetitionsUsed));
			}

			patient.Identifiers.RemoveAll(i => !targetConfiguration.NotificationDomainConfigurations.Exists(o => o.Domain.Equals(i.Authority.DomainName)));

			foreach (var entityIdentifier in patient.Identifiers.Where(item => assigningAuthorityRepositoryService.Find(a => a.DomainName == item.Authority.DomainName).FirstOrDefault() != null))
			{
				pid.GetPatientIdentifierList(pid.PatientIdentifierListRepetitionsUsed).AssigningAuthority.UniversalID.Value = entityIdentifier.Authority.Oid;
				pid.GetPatientIdentifierList(pid.PatientIdentifierListRepetitionsUsed).AssigningAuthority.UniversalIDType.Value = "ISO";
				pid.GetPatientIdentifierList(pid.PatientIdentifierListRepetitionsUsed).AssigningAuthority.NamespaceID.Value = entityIdentifier.Authority.Oid;
				pid.GetPatientIdentifierList(pid.PatientIdentifierListRepetitionsUsed).IDNumber.Value = entityIdentifier.Value;
			}

			foreach (var personLanguage in patient.LanguageCommunication.Where(l => l.IsPreferred))
			{
				pid.PrimaryLanguage.Identifier.Value = personLanguage.LanguageCode;
				//pid.PrimaryLanguage.NameOfCodingSystem.Value = "ISO639-1";
			}

			foreach (var mother in patient.Relationships.Where(r => (r.RelationshipType.Key == EntityRelationshipTypeKeys.Mother ||
																	r.RelationshipType.Key == EntityRelationshipTypeKeys.NaturalMother) &&
																	r.TargetEntity is Person).Select(relationship => relationship.TargetEntity as Person))
			{
				mother.Identifiers.ForEach(c =>
				{
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).AssigningAuthority.UniversalID.Value = c.Authority.Oid;
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).AssigningAuthority.UniversalIDType.Value = "ISO";
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).AssigningAuthority.NamespaceID.Value = c.Authority.Oid;
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).IDNumber.Value = c.Value;
				});
			}

			foreach (var name in patient.Names)
			{
				NotifierBase.UpdateXPN(name, pid.GetPatientName(pid.PatientNameRepetitionsUsed));
			}
		}

		/// <summary>
		/// Updates an <see cref="XPN"/> segment.
		/// </summary>
		/// <param name="entityName">The entity name to use to update the XPN segment.</param>
		/// <param name="name">The XPN segment to update.</param>
		internal static void UpdateXPN(EntityName entityName, XPN name)
		{
			var nameUse = entityName.NameUse?.Key;

			if (nameUse != null)
			{
				name.NameTypeCode.Value = MessageUtil.ReverseLookup(MessageUtil.NameUseMap, nameUse.ToGuid());
			}

			name.DegreeEgMD.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Suffix).Select(c => c.Value));
			name.FamilyName.Surname.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value));
			name.GivenName.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value));
			name.PrefixEgDR.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Prefix).Select(c => c.Value));
			name.SecondAndFurtherGivenNamesOrInitialsThereof.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Delimiter).Select(c => c.Value));
		}
	}
}