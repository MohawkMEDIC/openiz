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
 * Date: 2016-11-30
 */
using MARC.HI.EHRS.SVC.Core;
using NHapi.Model.V231.Datatype;
using NHapi.Model.V231.Segment;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.HL7.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using OpenIZ.Core.Security;
using TS = MARC.Everest.DataTypes.TS;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;

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
		protected static readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// The internal reference to the <see cref="IAssigningAuthorityRepositoryService"/> instance.
		/// </summary>
		private static readonly IAssigningAuthorityRepositoryService assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

		/// <summary>
		/// Ensures the authentication context is set.
		/// </summary>
		private static void EnsureAuthenticated()
		{
			// ensure authenticated
			AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
		}

		/// <summary>
		/// Updates the address.
		/// </summary>
		/// <param name="entityAddress">The entity address.</param>
		/// <param name="address">The address.</param>
		internal static void UpdateAD(EntityAddress entityAddress, XAD address)
		{
			// ensure authenticated
			EnsureAuthenticated();

			tracer.TraceEvent(TraceEventType.Verbose, 0, "Adding addresses");

			var addressUse = entityAddress.AddressUseKey;

			if (addressUse != null)
			{
				address.AddressType.Value = MessageUtil.GetCode(addressUse.Value, CodeSystemKeys.PostalAddressUse);
			}

            // TODO: Is the CT a UUID? if so we need to get the CT-ID (public id) from the place which the CT represents
			//address.CensusTract.Value = string.Join(" ", entityAddress.LoadCollection<EntityAddressComponent>("Component").Where(c => c.ComponentTypeKey == AddressComponentKeys.CensusTract).Select(c => c.Value));
			address.City.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.City).Select(c => c.Value));
			address.Country.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.Country).Select(c => c.Value));
			address.StateOrProvince.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.State).Select(c => c.Value));
			address.StreetAddress.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.StreetAddressLine).Select(c => c.Value));
			address.CountyParishCode.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.County).Select(c => c.Value));
			address.OtherDesignation.Value = string.Join(" ",
                entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.AdditionalLocator).Union(
                entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.Precinct)).Select(c => c.Value));
            address.ZipOrPostalCode.Value = string.Join(" ", entityAddress.Component.Where(c => c.ComponentTypeKey == AddressComponentKeys.PostalCode).Select(c => c.Value));
		}

		/// <summary>
		/// Updates the gender of a <see cref="PID"/> segment.
		/// </summary>
		/// <param name="gender">The gender.</param>
		/// <param name="pid">The PID segment to update.</param>
		internal static void UpdateGender(string gender, PID pid)
		{
			// these are unchanging concepts (readonly)
			// female 094941e9-a3db-48b5-862c-bc289bd7f86c
			// male f4e3a6bb-612e-46b2-9f77-ff844d971198
			// undifferentiated ae94a782-1485-4241-9bca-5b09db2156bf

			tracer.TraceEvent(TraceEventType.Information, 0, "Adding gender");

			// TODO: fix this to use concept lookup
			switch (gender?.ToLowerInvariant())
			{
				case "male":
				case "f4e3a6bb-612e-46b2-9f77-ff844d971198":
					pid.Sex.Value = "M";
					break;

				case "female":
				case "094941e9-a3db-48b5-862c-bc289bd7f86c":
					pid.Sex.Value = "F";
					break;

				case "undifferentiated":
				case "ae94a782-1485-4241-9bca-5b09db2156bf":
                default:
					pid.Sex.Value = "U";
					break;
			}
		}

		/// <summary>
		/// Updates a <see cref="MSH" /> segment.
		/// </summary>
		/// <param name="msh">The MSH segment to update.</param>
		/// <param name="patient">The patient.</param>
		/// <param name="targetConfiguration">The target configuration.</param>
		internal static void UpdateMSH(MSH msh, Patient patient, TargetConfiguration targetConfiguration)
		{
			// ensure authenticated
			EnsureAuthenticated();

			tracer.TraceEvent(TraceEventType.Verbose, 0, "Start updating MSH segment");

			msh.AcceptAcknowledgmentType.Value = "AL";
			msh.DateTimeOfMessage.TimeOfAnEvent.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
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

			// set MSH-3 as the NSID of the patient identifier
			msh.SendingApplication.NamespaceID.Value = patient.Identifiers.FirstOrDefault()?.Authority?.DomainName ?? configuration.DeviceName;
			msh.SendingFacility.NamespaceID.Value = configuration.JurisdictionData.Name;
			msh.VersionID.VersionID.Value = "2.3.1";
		}

		/// <summary>
		/// Updates a <see cref="PID"/> segment.
		/// </summary>
		/// <param name="patient">The patient to use to update the PID segment.</param>
		/// <param name="pid">The PID segment to update.</param>
		/// <param name="targetConfiguration">The target configuration.</param>
		internal static void UpdatePID(Patient patient, PID pid, TargetConfiguration targetConfiguration)
		{
			// ensure authenticated
			EnsureAuthenticated();

			tracer.TraceEvent(TraceEventType.Verbose, 0, "Start updating PID segment");

			if (patient.GenderConceptKey.HasValue)
			{
				UpdateGender(patient.GenderConceptKey.ToString(), pid);
			}

			if (patient.MultipleBirthOrder.HasValue)
			{
                if(patient.MultipleBirthOrder != 0)
				    pid.BirthOrder.Value = patient.MultipleBirthOrder.ToString();
				pid.MultipleBirthIndicator.Value = "Y";
			}

			if (patient.DateOfBirth.HasValue)
			{
				pid.DateTimeOfBirth.TimeOfAnEvent.Value = (TS)patient.DateOfBirth.Value;
			}

			if (patient.DeceasedDate.HasValue)
			{
				pid.PatientDeathDateAndTime.TimeOfAnEvent.Value = (TS)patient.DeceasedDate.Value;
				pid.PatientDeathIndicator.Value = "Y";
			}

			foreach (var address in patient.LoadCollection<EntityAddress>("Addresses"))
			{
				NotifierBase.UpdateAD(address, pid.GetPatientAddress(pid.PatientAddressRepetitionsUsed));
			}

            var pids = patient.LoadCollection<EntityIdentifier>("Identifiers").Where(item => assigningAuthorityRepositoryService.Find(a => a.DomainName == item.Authority.DomainName).FirstOrDefault() != null).ToArray();

            for (var i = 0; i < pids.Length; i++)
			{
                var patientIdentifier = pids[i];

				pid.GetPatientIdentifierList(i).ID.Value = patientIdentifier.Value;
				pid.GetPatientIdentifierList(i).AssigningAuthority.NamespaceID.Value = patientIdentifier.Authority.DomainName;
				pid.GetPatientIdentifierList(i).AssigningAuthority.UniversalID.Value = patientIdentifier.Authority.Oid;
				pid.GetPatientIdentifierList(i).AssigningAuthority.UniversalIDType.Value = "ISO";
				pid.GetPatientIdentifierList(i).IdentifierTypeCode.Value = "PI";

			}

            // Create the PI for the patient key
            var lastPid = pid.PatientIdentifierListRepetitionsUsed;
            pid.GetPatientIdentifierList(lastPid).ID.Value = patient.Key.ToString();
            pid.GetPatientIdentifierList(lastPid).AssigningAuthority.UniversalID.Value = ApplicationContext.Current.Configuration?.Custodianship?.Id?.AssigningAuthority?.Oid;
            pid.GetPatientIdentifierList(lastPid).AssigningAuthority.UniversalIDType.Value = "ISO";
            pid.GetPatientIdentifierList(lastPid).IdentifierTypeCode.Value = "PI";

            foreach (var personLanguage in patient.LoadCollection<PersonLanguageCommunication>("LanguageCommunication").Where(l => l.IsPreferred))
			{
				pid.PrimaryLanguage.Identifier.Value = personLanguage.LanguageCode;
				//pid.PrimaryLanguage.NameOfCodingSystem.Value = "ISO639-1";
			}

			foreach (var mother in patient.LoadCollection<EntityRelationship>("Relationships").Where(r => (r.RelationshipTypeKey == EntityRelationshipTypeKeys.Mother ||
																	r.RelationshipTypeKey == EntityRelationshipTypeKeys.NaturalMother) &&
																	r.LoadProperty<Entity>("TargetEntity") is Person).Select(relationship => relationship.TargetEntity as Person))
			{
				mother.Identifiers.ForEach(c =>
				{
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).ID.Value = c.Value;
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).AssigningAuthority.NamespaceID.Value = c.Authority.Oid;
					pid.GetMotherSIdentifier(pid.MotherSIdentifierRepetitionsUsed).AssigningAuthority.UniversalIDType.Value = "ISO";
					pid.GetPatientIdentifierList(pid.PatientIdentifierListRepetitionsUsed).IdentifierTypeCode.Value = c.Authority.DomainName;
				});

				mother.Names.ForEach(c =>
				{
					NotifierBase.UpdateXPN(c, pid.GetMotherSMaidenName(pid.MotherSMaidenNameRepetitionsUsed));
				});
			}

			foreach (var name in patient.LoadCollection<EntityName>("Names"))
			{
				NotifierBase.UpdateXPN(name, pid.GetPatientName(pid.PatientNameRepetitionsUsed));
			}
		}

		/// <summary>
		/// Updates an <see cref="XPN"/> segment.
		/// </summary>
		/// <param name="entityName">The entity name to use to update the XPN segment.</param>
		/// <param name="name">The XPN segment to update.</param>
		internal static XPN UpdateXPN(EntityName entityName, XPN name)
		{
			// ensure authenticated
			EnsureAuthenticated();

			tracer.TraceEvent(TraceEventType.Verbose, 0, "Adding names");

			name.NameTypeCode.Value = MessageUtil.GetCode(entityName.NameUseKey.Value, CodeSystemKeys.EntityNameUse);
			name.DegreeEgMD.Value = string.Join(" ", entityName.LoadCollection<EntityNameComponent>("Component").Where(c => c.ComponentTypeKey == NameComponentKeys.Suffix).Select(c => c.Value));
			name.FamilyLastName.FamilyName.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value));
			name.GivenName.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value));
			name.PrefixEgDR.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Prefix).Select(c => c.Value));
			name.MiddleInitialOrName.Value = string.Join(" ", entityName.Component.Where(c => c.ComponentTypeKey == NameComponentKeys.Delimiter).Select(c => c.Value));

			return name;
		}
	}
}