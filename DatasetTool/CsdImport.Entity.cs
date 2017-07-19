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
 * Date: 2017-7-7
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public partial class CsdImport
	{
		/// <summary>
		/// Maps the contact point.
		/// </summary>
		/// <param name="addressUseKey">The address use key.</param>
		/// <param name="contactPoint">The contact point.</param>
		/// <returns>Returns an entity telecom address.</returns>
		private static EntityTelecomAddress MapContactPoint(Guid addressUseKey, contactPoint contactPoint)
		{
			var concept = MapCodedType(contactPoint.codedType.code, contactPoint.codedType.codingScheme);

			var entityTelecomAddress = new EntityTelecomAddress(addressUseKey, contactPoint.codedType.Value);

			if (concept == null)
			{
				ShowWarningOnNotFound($"Warning, unable to map telecommunications use, no related concept found for code: {contactPoint.codedType.code} using scheme: {contactPoint.codedType.codingScheme}", nameof(TelecomAddressUseKeys.Public), TelecomAddressUseKeys.Public);
			}
			else
			{
				entityTelecomAddress.AddressUseKey = concept.Key;
			}

			return entityTelecomAddress;
		}

		/// <summary>
		/// Maps the entity address.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns an entity address.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// Unable to map address use
		/// -or-
		/// Unable to map address component</exception>
		private static EntityAddress MapEntityAddress(address address, Uri codeSystem)
		{
			var entityAddress = new EntityAddress
			{
				AddressUseKey = AddressUseKeys.Public
			};

			if (address.type != null)
			{
				var addressUseConcept = MapCodedType(address.type, codeSystem.ToString());

				if (addressUseConcept == null)
				{
					ShowWarningOnNotFound($"Warning, unable to map address use, no related concept found for code: {address.type}", nameof(AddressUseKeys.Public), AddressUseKeys.Public);
					entityAddress.AddressUseKey = AddressUseKeys.Public;
				}
				else
				{
					entityAddress.AddressUseKey = addressUseConcept.Key;
				}
			}

			if (address.addressLine?.Any() != true)
			{
				ShowWarningMessage("Warning, address has no address components, this may affect the import process");

				return entityAddress;
			}

			entityAddress.Component = address.addressLine.Select(c => MapEntityAddressComponent(c, new Uri(AddressComponentCodeSystem))).ToList();

			return entityAddress;
		}

		/// <summary>
		/// Maps the entity address component.
		/// </summary>
		/// <param name="addressComponent">The address component.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns the mapped entity address component.</returns>
		private static EntityAddressComponent MapEntityAddressComponent(addressAddressLine addressComponent, Uri codeSystem)
		{
			var conceptService = ApplicationContext.Current.GetConceptService();

			var entityAddressComponent = new EntityAddressComponent
			{
				Value = addressComponent.Value
			};

			var addressComponentConcept = conceptService.FindConceptsByReferenceTerm(addressComponent.component, codeSystem).FirstOrDefault();

			if (addressComponentConcept == null)
			{
				ShowWarningOnNotFound($"Warning, unable to map address component, no related concept found for code: {addressComponent.component}", nameof(AddressComponentKeys.CensusTract), AddressComponentKeys.CensusTract);
				entityAddressComponent.ComponentTypeKey = AddressComponentKeys.CensusTract;
			}
			else
			{
				entityAddressComponent.ComponentTypeKey = addressComponentConcept.Key;
			}

			return entityAddressComponent;
		}

		/// <summary>
		/// Maps the entity extension.
		/// </summary>
		/// <param name="extensionUrl">The extension URL.</param>
		/// <param name="value">The value.</param>
		/// <returns>Returns an entity extension.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate extension type</exception>
		private static EntityExtension MapEntityExtension(string extensionUrl, string value)
		{
			var extensionTypeService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			var extensionType = extensionTypeService.GetExtensionType(new Uri(extensionUrl));

			if (extensionType == null)
			{
				ShowErrorOnNotFound($"Error, {emergencyMessage} Unable to locate extension type: {extensionUrl}, has this been added to the OpenIZ extension types list?");
			}

			return new EntityExtension(extensionType.Key.Value, extensionType.ExtensionHandler, value);
		}

		/// <summary>
		/// Maps the entity identifier.
		/// </summary>
		/// <param name="otherId">The other identifier.</param>
		/// <returns>Returns an entity identifier.</returns>
		private static EntityIdentifier MapEntityIdentifier(otherID otherId)
		{
			return new EntityIdentifier(MapAssigningAuthority(otherId), otherId.Value);
		}

		/// <summary>
		/// Maps the name of the entity.
		/// </summary>
		/// <param name="nameUseKey">The name use key.</param>
		/// <param name="organizationName">Name of the organization.</param>
		/// <returns>Returns an entity name.</returns>
		private static EntityName MapEntityNameOrganization(Guid nameUseKey, organizationOtherName organizationName)
		{
			return new EntityName(nameUseKey, organizationName.Value);
		}

		/// <summary>
		/// Maps the entity name person.
		/// </summary>
		/// <param name="nameUseKey">The name use key.</param>
		/// <param name="personName">Name of the person.</param>
		/// <returns>EntityName.</returns>
		private static EntityName MapEntityNamePerson(Guid nameUseKey, personName personName)
		{
			var entityName = new EntityName(nameUseKey, personName.surname, personName.forename);

			if (personName.honorific != null)
			{
				entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Title, personName.honorific));
			}

			if (personName.suffix != null)
			{
				entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Suffix, personName.suffix));
			}

			return entityName;
		}

		/// <summary>
		/// Maps the entity relationship organization contact.
		/// </summary>
		/// <param name="contact">The contact.</param>
		/// <returns>Returns the mapped entity relationship from an organization contact instance.</returns>
		private static EntityRelationship MapEntityRelationshipOrganizationContact(organizationContact contact, CsdOptions options)
		{
			EntityRelationship entityRelationship = null;

			if (contact.Item.GetType() == typeof(uniqueID))
			{
				var csdProvider = contact.Item as uniqueID;

				var provider = GetOrCreateEntity<Provider>(csdProvider.entityID, options.EntityUidAuthority, options);

				entityRelationship = new EntityRelationship(EntityRelationshipTypeKeys.Contact, provider);
			}
			else if (contact.Item.GetType() == typeof(person))
			{
				var csdPerson = contact.Item as person;

				var personService = ApplicationContext.Current.GetService<IPersonRepositoryService>();

				// TODO: fix to search the person by address, name, date of birth, and gender, to find an existing person before creating a new one
				// we are creating a new person here, because the person class in CSD doesn't have an entityID
				// property for us to use to lookup the person to see if they exist.
				var person = new Person
				{
					Addresses = csdPerson.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem))).ToList() ?? new List<EntityAddress>(),
					Key = Guid.NewGuid()
				};

				if (csdPerson.dateOfBirthSpecified)
				{
					person.DateOfBirth = csdPerson.dateOfBirth;
				}

				// map names
				if (csdPerson.name?.Any() == true)
				{
					person.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
					person.Names.AddRange(csdPerson.name.Select(c => MapEntityNamePerson(NameUseKeys.OfficialRecord, c)));
				}

				// map telecommunications
				if (csdPerson.contactPoint?.Any() == true)
				{
					person.Telecoms.RemoveAll(c => c.AddressUseKey == TelecomAddressUseKeys.Public);
					person.Telecoms.AddRange(csdPerson.contactPoint?.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c)));
				}

				entityRelationship = new EntityRelationship(EntityRelationshipTypeKeys.Contact, person);
			}
			else
			{
				ShowErrorOnNotFound($"Error, {emergencyMessage} {nameof(organizationContact.Item)} is not of type: {nameof(person)} or {nameof(uniqueID)}");
			}

			return entityRelationship;
		}

		/// <summary>
		/// Maps the language communication.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns>Returns a person language communication.</returns>
		private static PersonLanguageCommunication MapLanguageCommunication(codedtype language)
		{
			var concept = MapCodedType(language.code, "urn:ietf:bcp:47");

			// if not found using code, attempt lookup by language value
			if (concept == null)
			{
				ShowWarningMessage($"Warning, language not found using code: {language.code} will attempt lookup using: {language.Value}");

				concept = MapCodedType(language.Value, "urn:ietf:bcp:47");
			}

			PersonLanguageCommunication personLanguageCommunication = null;

			if (concept != null)
			{
				personLanguageCommunication = new PersonLanguageCommunication(concept.Mnemonic, false);
			}
			else
			{
				ShowErrorOnNotFound($"Error, {emergencyMessage} language not found using code: {language.code} or using value: {language.Value}");
			}

			return personLanguageCommunication;
		}
	}
}