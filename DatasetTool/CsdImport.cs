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
 * Date: 2017-4-7
 */

using MARC.HI.EHRS.SVC.Core;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Persistence;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using OpenIZ.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
    [Description("Care Services Discovery (CSD) tooling")]
	public class CsdImport
	{

        /// <summary>
        /// Represents CSD options.
        /// </summary>
        internal class CsdOptions
        {
            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            [Parameter("file")]
            [Description("The path to the CSD file")]
            public string File { get; set; }
        }

		/// <summary>
		/// The emergency message.
		/// </summary>
		private static string emergencyMessage = null;

		/// <summary>
		/// The address type code system.
		/// </summary>
		private const string AddressTypeCodeSystem = "urn:ihe:iti:csd:2013:addressType";

		/// <summary>
		/// The address component code system.
		/// </summary>
		private const string AddressComponentCodeSystem = "urn:ihe:iti:csd:2013:address";

		/// <summary>
		/// The program exit message.
		/// </summary>
		private const string ProgramExitMessage = "Unable to continue import CSD document, press any key to exit.";

		/// <summary>
		/// The CSD entity identifier tag.
		/// </summary>
		private const string CsdEntityIdTag = "http://openiz.org/tags/contrib/importedData/csd/entityID";

		/// <summary>
		/// The imported data tag.
		/// </summary>
		private const string ImportedDataTag = "http://openiz.org/tags/contrib/importedData";

		/// <summary>
		/// The assigning authority keys.
		/// </summary>
		private static readonly Dictionary<string, Guid> assigningAuthorityKeys = new Dictionary<string, Guid>();

		/// <summary>
		/// The concept keys.
		/// </summary>
		private static readonly Dictionary<CompositeKey, Guid> conceptKeys = new Dictionary<CompositeKey, Guid>();

		/// <summary>
		/// Initializes a new instance of the <see cref="CsdImport"/> class.
		/// </summary>
		public CsdImport()
		{
		}

        /// <summary>
        /// Imports the CSD.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [ParameterClass(typeof(CsdOptions))]
        [Description("Converts a Care Services Discovery (CSD) export to DATASET import file")]
		[Example("Import a Care Services Discovery (CSD) export to a DATASET import file", "--tool=CsdImport --operation=ImportCsd --file=CSD-Organizations-Connectathon-20150120.xml")]
		public static void ImportCsd(string[] args)
		{
			ApplicationContext.Current.Start();

			ApplicationContext.Current.AddServiceProvider(typeof(LocalEntityRepositoryService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalMetadataRepositoryService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalConceptRepositoryService));

			AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

			ApplicationContext.Current.Started += (o, e) =>
			{
				emergencyMessage = ApplicationContext.Current.GetLocaleString("01189998819991197253");
			};

			var parameters = new ParameterParser<CsdOptions>().Parse(args);

			var csdDatasetInstall = new DatasetInstall { Id = "CSD", Action = new List<DataInstallAction>() };

			var serializer = new XmlSerializer(typeof(CSD));

			var fileInfo = new FileInfo(parameters.File);

			var csd = (CSD)serializer.Deserialize(new StreamReader(parameters.File));

			var stopwatch = new Stopwatch();

			stopwatch.Start();

			var organizations = MapOrganizations(csd.organizationDirectory).Select(o => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = o
			});

			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Mapped {organizations.Count()} organizations in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");
			Console.ResetColor();

			stopwatch = new Stopwatch();
			stopwatch.Start();

			csdDatasetInstall.Action.AddRange(organizations);

			var places = MapPlaces(csd.facilityDirectory).Select(p => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = p
			});

			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Mapped {places.Count()} places in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");
			Console.ResetColor();

			csdDatasetInstall.Action.AddRange(places);

			stopwatch = new Stopwatch();
			stopwatch.Start();

			var providers = MapProviders(csd.providerDirectory).Select(p => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = p
			});

			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Mapped {providers.Count()} providers in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");
			Console.ResetColor();

			csdDatasetInstall.Action.AddRange(providers);

			stopwatch = new Stopwatch();
			stopwatch.Start();

			var services = MapServices(csd.serviceDirectory).Select(s => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = s
			});

			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Mapped {services.Count()} services in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");
			Console.ResetColor();

			csdDatasetInstall.Action.AddRange(services);

			serializer = new XmlSerializer(typeof(DatasetInstall));

			using (var fileStream = File.Create($"999-CSD-import-{fileInfo.Name}.dataset"))
			{
				serializer.Serialize(fileStream, csdDatasetInstall);
			}
		}

		/// <summary>
		/// Exits the application, when an entity is not found.
		/// </summary>
		/// <param name="message">The message.</param>
		private static void ExitOnNotFound(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.WriteLine(ProgramExitMessage);
			Console.ResetColor();
			Console.ReadKey();
			Environment.Exit(999);
		}

		/// <summary>
		/// Lookups the by entity identifier or tag.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entityId">The entity identifier.</param>
		/// <param name="createNewIfNotFound">if set to <c>true</c> [create new if not found].</param>
		/// <returns>Returns an entity or null of no entity is found.</returns>
		private static T LookupByEntityIdOrTag<T>(string entityId, bool createNewIfNotFound = true) where T : Entity, new()
		{
			var entityService = ApplicationContext.Current.GetService<IEntityRepositoryService>();

			T entity;
			Guid key;

			// lookup by entity tag if the entity id value is not a GUID
			if (!Guid.TryParse(entityId, out key))
			{
				entity = entityService.Find(p => p.Tags.Any(t => t.TagKey == CsdEntityIdTag && t.Value == entityId)).FirstOrDefault() as T;
			}
			else
			{
				entity = entityService.Find(p => p.Key == key).FirstOrDefault() as T;
			}

			// if the entity wasn't found, we want to create a new entity
			if (createNewIfNotFound && entity == null)
			{
				entity = new T
				{
					Tags = new List<EntityTag>
					{
						new EntityTag(ImportedDataTag, "true")
					}
				};

				// if the key is an empty GUID, we were unable to parse the entity id as a valid GUID
				if (key == Guid.Empty)
				{
					entity.Key = Guid.NewGuid();
					entity.Tags.Add(new EntityTag(CsdEntityIdTag, entityId));
				}
				else
				{
					entity.Key = Guid.Parse(entityId);
				}
			}

			return entity;
		}

		/// <summary>
		/// Maps the assigning authority.
		/// </summary>
		/// <param name="otherId">The other identifier.</param>
		/// <returns>AssigningAuthority.</returns>
		/// <exception cref="System.InvalidOperationException">If the assigning authority is not found.</exception>
		private static AssigningAuthority MapAssigningAuthority(otherID otherId)
		{
			var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			AssigningAuthority assigningAuthority;

			if (!assigningAuthorityKeys.ContainsKey(otherId.assigningAuthorityName))
			{ 
				// lookup by URL
				assigningAuthority = metadataService.FindAssigningAuthority(a => a.Url == otherId.assigningAuthorityName).FirstOrDefault();

				if (assigningAuthority != null)
				{
					assigningAuthorityKeys.Add(otherId.assigningAuthorityName, assigningAuthority.Key.Value);
					return assigningAuthority;
				}

				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Warning, unable to locate assigning authority by URL using value: {otherId.assigningAuthorityName}, will attempt to lookup by OID");
				Console.ResetColor();

				// lookup by OID
				assigningAuthority = metadataService.FindAssigningAuthority(a => a.Oid == otherId.assigningAuthorityName).FirstOrDefault();

				if (assigningAuthority == null)
				{
					ExitOnNotFound($"Error, {emergencyMessage} Unable to locate assigning authority using URL or OID. Has {otherId.assigningAuthorityName} been added to the OpenIZ assigning authority list?");
				}

				assigningAuthorityKeys.Add(otherId.assigningAuthorityName, assigningAuthority.Key.Value);
			}
			else
			{
				assigningAuthority = new AssigningAuthority
				{
					Key = assigningAuthorityKeys[otherId.assigningAuthorityName]
				};
			}

			return assigningAuthority;
		}

		/// <summary>
		/// Maps the coded type.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="codingScheme">The coding scheme.</param>
		/// <returns>Concept.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// code - Value cannot be null
		/// or
		/// codingScheme - Value cannot be null
		/// </exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		private static Concept MapCodedType(string code, string codingScheme)
		{
			if (code == null)
			{
				throw new ArgumentNullException(nameof(code), "Value cannot be null");
			}

			if (codingScheme == null)
			{
				throw new ArgumentNullException(nameof(codingScheme), "Value cannot be null");
			}

			Concept concept;

			if (!conceptKeys.ContainsKey(new CompositeKey(code, codingScheme)))
			{
				var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

				if (conceptRepositoryService == null)
				{
					throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
				}

				// since CSD coded types are oid based, we want to make sure that the coding scheme starts with "oid"
				if (!codingScheme.StartsWith("oid:") && !codingScheme.StartsWith("http://") && !codingScheme.StartsWith("urn:"))
				{
					codingScheme = "oid:" + codingScheme;
				}

				concept = conceptRepositoryService.FindConceptsByReferenceTerm(code, new Uri(codingScheme)).FirstOrDefault();

				if (concept == null)
				{
					ExitOnNotFound($"Error, {emergencyMessage} Unable to locate concept using code: {code} and coding scheme: {codingScheme}");
				}
				else
				{
					conceptKeys.Add(new CompositeKey(code, codingScheme), concept.Key.Value);
				}
			}
			else
			{
				Guid key;
				conceptKeys.TryGetValue(new CompositeKey(code, codingScheme), out key);
				concept = new Concept
				{
					Key = key
				};
			}

			return concept;
		}

		/// <summary>
		/// Maps the contact point.
		/// </summary>
		/// <param name="contactPoint">The contact point.</param>
		/// <returns>Returns an entity telecom address.</returns>
		private static EntityTelecomAddress MapContactPoint(contactPoint contactPoint)
		{
			var concept = MapCodedType(contactPoint.codedType.code, contactPoint.codedType.codingScheme);

			var entityTelecomAddress = new EntityTelecomAddress(TelecomAddressUseKeys.Public, contactPoint.codedType.Value);

			if (concept == null)
			{
				WarningOnNotFound($"Warning, unable to map telecommunications use, no related concept found for code: {contactPoint.codedType.code} using scheme: {contactPoint.codedType.codingScheme}", nameof(TelecomAddressUseKeys.Public), TelecomAddressUseKeys.Public);
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
					WarningOnNotFound($"Warning, unable to map address use, no related concept found for code: {address.type}", nameof(AddressUseKeys.Public), AddressUseKeys.Public);
					entityAddress.AddressUseKey = AddressUseKeys.Public;
				}
				else
				{
					entityAddress.AddressUseKey = addressUseConcept.Key;
				}
			} 

			if (address.addressLine?.Any() != true)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Warning, address has no address components, this may affect the import process");
				Console.ResetColor();

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
				WarningOnNotFound($"Warning, unable to map address component, no related concept found for code: {addressComponent.component}", nameof(AddressComponentKeys.CensusTract), AddressComponentKeys.CensusTract);
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
				ExitOnNotFound($"Error, {emergencyMessage} Unable to locate extension type: {extensionUrl}, has this been added to the OpenIZ extension types list?");
			}

			return new EntityExtension(extensionType.Key.Value, Encoding.ASCII.GetBytes(value));
		}

		/// <summary>
		/// Maps the entity identifier.
		/// </summary>
		/// <param name="otherId">The other identifier.</param>
		/// <returns>Returns an entity identifier.</returns>
		private static EntityIdentifier MapEntityIdentifier(otherID otherId)
		{
			return new EntityIdentifier(MapAssigningAuthority(otherId), otherId.code);
		}

		/// <summary>
		/// Maps the name of the entity.
		/// </summary>
		/// <param name="organizationName">Name of the organization.</param>
		/// <returns>Returns an entity name.</returns>
		private static EntityName MapEntityNameOrganization(organizationOtherName organizationName)
		{
			return new EntityName(NameUseKeys.OfficialRecord, organizationName.Value);
		}

		/// <summary>
		/// Maps the entity name person.
		/// </summary>
		/// <param name="personName">Name of the person.</param>
		/// <returns>EntityName.</returns>
		private static EntityName MapEntityNamePerson(personName personName)
		{
			var entityName = new EntityName(NameUseKeys.OfficialRecord, personName.surname, personName.forename);

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
		private static EntityRelationship MapEntityRelationshipOrganizationContact(organizationContact contact)
		{
			EntityRelationship entityRelationship = null;

			if (contact.Item.GetType() == typeof(uniqueID))
			{
				var csdProvider = contact.Item as uniqueID;

				var provider = LookupByEntityIdOrTag<Provider>(csdProvider.entityID);

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
					Key = Guid.NewGuid(),
					Names = csdPerson.name?.Select(MapEntityNamePerson).ToList() ?? new List<EntityName>(),
					Telecoms = csdPerson.contactPoint?.Select(MapContactPoint).ToList() ?? new List<EntityTelecomAddress>()
				};

				if (csdPerson.dateOfBirthSpecified)
				{
					person.DateOfBirth = csdPerson.dateOfBirth;
				}

				entityRelationship = new EntityRelationship(EntityRelationshipTypeKeys.Contact, person);
			}
			else
			{
				ExitOnNotFound($"Error, {emergencyMessage} {nameof(organizationContact.Item)} is not of type: {nameof(person)} or {nameof(uniqueID)}");
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
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Warning, language not found using code: {language.code} will attempt lookup using: {language.Value}");
				Console.ResetColor();

				concept = MapCodedType(language.Value, "urn:ietf:bcp:47");
			}

			PersonLanguageCommunication personLanguageCommunication = null;

			if (concept != null)
			{
				personLanguageCommunication = new PersonLanguageCommunication(concept.Mnemonic, false);
			}
			else
			{
				ExitOnNotFound($"Error, {emergencyMessage} language not found using code: {language.code} or using value: {language.Value}");
			}

			return personLanguageCommunication;
		}

		/// <summary>
		/// Maps the organizations.
		/// </summary>
		/// <param name="csdOrganizations">The CSD organizations.</param>
		/// <returns>Returns a list of organizations.</returns>
		private static IEnumerable<Organization> MapOrganizations(IEnumerable<organization> csdOrganizations)
		{
			var organizations = new List<Organization>();

			foreach (var csdOrganization in csdOrganizations)
			{
				var organization = new Organization
				{
					Addresses = csdOrganization.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem))).ToList() ?? new List<EntityAddress>(),
					CreationTime = csdOrganization.record?.created ?? DateTimeOffset.Now,
					Extensions = csdOrganization.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList() ?? new List<EntityExtension>(),
					Identifiers = csdOrganization.otherID?.Select(MapEntityIdentifier).ToList() ?? new List<EntityIdentifier>(),
					StatusConceptKey = MapStatusCode(csdOrganization.record?.status, "http://openiz.org/csd/CSD-OrganizationStatusCodes"),
					Tags = new List<EntityTag>
					{
						new EntityTag(ImportedDataTag, "true")
					}
				};

				Guid key;

				if (!TryMapKey(csdOrganization.entityID, out key))
				{
					organization.Key = Guid.NewGuid();
					organization.Tags.Add(new EntityTag(CsdEntityIdTag, csdOrganization.entityID));
				}
				else
				{
					organization.Key = key;
				}

				if (csdOrganization.codedType?.Any() == true)
				{
					// we don't support multiple specialties for a organization at the moment, so we only take the first one
					// TODO: cleanup
					organization.TypeConceptKey = MapCodedType(csdOrganization.codedType[0].code, csdOrganization.codedType[0].codingScheme)?.Key;
				}

				if (csdOrganization.specialization?.Any() == true)
				{
					// we don't support multiple industry values for a organization at the moment, so we only take the first one
					// TODO: cleanup
					organization.IndustryConceptKey = MapCodedType(csdOrganization.specialization[0].code, csdOrganization.specialization[0].codingScheme)?.Key;
				}

				if (csdOrganization.parent?.entityID != null)
				{
					organization.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, LookupByEntityIdOrTag<Organization>(csdOrganization.parent.entityID)));
				}

				if (csdOrganization.primaryName != null)
				{
					organization.Names.Add(new EntityName(NameUseKeys.OfficialRecord, csdOrganization.primaryName));
				}

				if (csdOrganization.record?.sourceDirectory != null)
				{
					organization.Tags.Add(new EntityTag("sourceDirectory", csdOrganization.record.sourceDirectory));
				}

				if (csdOrganization.otherID?.Any() == true)
				{
					organization.Identifiers.AddRange(csdOrganization.otherID.Select(MapEntityIdentifier));
				}

				if (csdOrganization.otherName?.Any() == true)
				{
					organization.Names.AddRange(csdOrganization.otherName.Select(MapEntityNameOrganization));
				}

				if (csdOrganization.contact?.Any() == true)
				{
					organization.Relationships.AddRange(csdOrganization.contact.Select(MapEntityRelationshipOrganizationContact));
				}

				if (csdOrganization.contactPoint?.Any() == true)
				{
					organization.Telecoms.AddRange(csdOrganization.contactPoint.Select(MapContactPoint));
				}

				organizations.Add(organization);

				Console.WriteLine($"Mapped organization: {organization.Key.Value} {string.Join(" ", organization.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
			}

			return organizations;
		}

		/// <summary>
		/// Maps the places.
		/// </summary>
		/// <param name="csdFacilities">The CSD facilities.</param>
		/// <returns>Returns a list of places.</returns>
		private static IEnumerable<Place> MapPlaces(IEnumerable<facility> csdFacilities)
		{
			var places = new List<Place>();

			foreach (var facility in csdFacilities)
			{
				var place = new Place
				{
					Addresses = facility.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem))).ToList() ?? new List<EntityAddress>(),
					CreationTime = facility.record?.created ?? DateTimeOffset.Now,
					Extensions = facility.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList() ?? new List<EntityExtension>(),
					Identifiers = facility.otherID?.Select(MapEntityIdentifier).ToList() ?? new List<EntityIdentifier>(),
					StatusConceptKey = MapStatusCode(facility.record?.status, "http://openiz.org/csd/CSD-FacilityStatusCode"),
					Tags = new List<EntityTag>
					{
						new EntityTag(ImportedDataTag, "true")
					}
				};

				Guid key;

				if (!TryMapKey(facility.entityID, out key))
				{
					place.Key = Guid.NewGuid();
					place.Tags.Add(new EntityTag(CsdEntityIdTag, facility.entityID));
				}
				else
				{
					place.Key = key;
				}

				if (facility.geocode?.latitude != null)
				{
					place.Lat = Convert.ToDouble(facility.geocode.latitude);
				}

				if (facility.geocode?.longitude != null)
				{
					place.Lng = Convert.ToDouble(facility.geocode.longitude);
				}

				if (facility.codedType?.Any() == true)
				{
					// we don't support multiple types for a place at the moment, so we only take the first one
					// TODO: cleanup
					place.TypeConceptKey = MapCodedType(facility.codedType[0].code, facility.codedType[0].codingScheme)?.Key;
				}

				if (facility.primaryName != null)
				{
					place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, facility.primaryName));
				}

				if (facility.otherName?.Any() == true)
				{
					place.Names.AddRange(facility.otherName.Select(f => new EntityName(NameUseKeys.Assigned, f.Value)));
				}

				if (facility.contactPoint?.Any() == true)
				{
					place.Telecoms.AddRange(facility.contactPoint.Select(MapContactPoint));
				}

				places.Add(place);

				Console.WriteLine($"Mapped place: {place.Key.Value} {string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
			}

			return places;
		}

		/// <summary>
		/// Maps the providers.
		/// </summary>
		/// <param name="csdProviders">The CSD providers.</param>
		/// <returns>Returns a list of providers.</returns>
		private static IEnumerable<Provider> MapProviders(IEnumerable<provider> csdProviders)
		{
			var providers = new List<Provider>();

			foreach (var csdProvider in csdProviders)
			{
				var provider = new Provider
				{
					Addresses = csdProvider.demographic?.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem))).ToList() ?? new List<EntityAddress>(),
					CreationTime = csdProvider.record?.created ?? DateTimeOffset.Now,
					DateOfBirth = csdProvider.demographic?.dateOfBirth,
					Extensions = csdProvider.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList() ?? new List<EntityExtension>(),
					Identifiers = csdProvider.otherID?.Select(MapEntityIdentifier).ToList() ?? new List<EntityIdentifier>(),
					LanguageCommunication = csdProvider.language?.Select(MapLanguageCommunication).ToList() ?? new List<PersonLanguageCommunication>(),
					Names = csdProvider.demographic?.name?.Select(MapEntityNamePerson).ToList() ?? new List<EntityName>(),
					StatusConceptKey = MapStatusCode(csdProvider.record?.status, "http://openiz.org/csd/CSD-ProviderStatusCodes"),
					Tags = new List<EntityTag>
					{
						new EntityTag(ImportedDataTag, "true")
					},
					Telecoms = csdProvider.demographic?.contactPoint?.Select(MapContactPoint).ToList() ?? new List<EntityTelecomAddress>()
				};

				if (csdProvider.specialty?.Any() == true)
				{
					// we don't support multiple specialties for a provider at the moment, so we only take the first one
					// TODO: cleanup
					provider.ProviderSpecialtyKey = MapCodedType(csdProvider.specialty[0].code, csdProvider.specialty[0].codingScheme)?.Key;
				}

				if (csdProvider.codedType?.Any() == true)
				{
					// we don't support multiple types for a provider at the moment, so we only take the first one
					// TODO: cleanup
					provider.TypeConceptKey = MapCodedType(csdProvider.codedType[0].code, csdProvider.codedType[0].codingScheme)?.Key;
				}

				Guid key;

				if (!TryMapKey(csdProvider.entityID, out key))
				{
					provider.Key = Guid.NewGuid();
					provider.Tags.Add(new EntityTag(CsdEntityIdTag, csdProvider.entityID));
				}
				else
				{
					provider.Key = key;
				}

				providers.Add(provider);

				Console.WriteLine($"Mapped provider: {provider.Key.Value} {string.Join(" ", provider.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
			}

			return providers;
		}

		/// <summary>
		/// Maps the services.
		/// </summary>
		/// <param name="csdServices">The CSD services.</param>
		/// <returns>Returns a list of place services.</returns>
		private static IEnumerable<PlaceService> MapServices(IEnumerable<service> csdServices)
		{
			var services = new List<PlaceService>();

			foreach (var csdService in csdServices)
			{
				var service = new PlaceService
				{
					ServiceConceptKey = MapCodedType(csdService.codedType[0].code, csdService.codedType[0].codingScheme)?.Key,
				};

				Guid key;

				service.Key = !TryMapKey(csdService.entityID, out key) ? Guid.NewGuid() : key;

				services.Add(service);
			}

			return services;
		}

		/// <summary>
		/// Maps the status code.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Returns a status key.</returns>
		/// <exception cref="System.InvalidOperationException">IConceptRepositoryService</exception>
		private static Guid? MapStatusCode(string code, string codeSystem)
		{
			if (code == null)
			{
				throw new ArgumentNullException(nameof(code), "Value cannot be null");
			}

			if (codeSystem == null)
			{
				throw new ArgumentNullException(nameof(codeSystem), "Value cannot be null");
			}

			var conceptService = ApplicationContext.Current.GetConceptService();

			if (conceptService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
			}

			return conceptService.FindConceptsByReferenceTerm(code, new Uri(codeSystem)).FirstOrDefault()?.Key;
		}

		/// <summary>
		/// Maps the key.
		/// </summary>
		/// <param name="entityId">The entity identifier.</param>
		/// <returns>Returns a key for a given entity id.</returns>
		private static bool TryMapKey(string entityId, out Guid key)
		{
			var status = false;

			key = Guid.Empty;

			if (entityId.StartsWith("urn:uuid:"))
			{
				status = Guid.TryParse(entityId.Substring(9), out key);
			}

			return status;
		}

		/// <summary>
		/// Prints a warning when an entity is not found.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="defaultValueName">Default name of the value.</param>
		/// <param name="defaultValue">The default value.</param>
		private static void WarningOnNotFound(string message, string defaultValueName, Guid defaultValue)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.WriteLine($"Defaulting to {defaultValueName} {defaultValue}");
			Console.ResetColor();
		}
	}

	/// <summary>
	/// Represents a composite key.
	/// </summary>
	internal class CompositeKey
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeKey" /> class.
		/// </summary>
		public CompositeKey()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeKey"/> class.
		/// </summary>
		/// <param name="firstKey">The first key.</param>
		/// <param name="secondKey">The second key.</param>
		public CompositeKey(string firstKey, string secondKey)
		{
			this.FirstKey = firstKey;
			this.SecondKey = secondKey;
		}

		/// <summary>
		/// Gets or sets the first key.
		/// </summary>
		/// <value>The first key.</value>
		public string FirstKey { get; set; }

		/// <summary>
		/// Gets or sets the second key.
		/// </summary>
		/// <value>The second key.</value>
		public string SecondKey { get; set; }

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as CompositeKey;

			if (other == null)
			{
				return false;
			}

			return this.FirstKey == other.FirstKey && this.SecondKey == other.SecondKey;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ 17;
		}

		/// <summary>
		/// Implements the == operator.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(CompositeKey left, CompositeKey right)
		{
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			if (left == null || right == null)
			{
				return false;
			}

			return left.FirstKey == right.FirstKey && left.SecondKey == right.SecondKey;
		}

		/// <summary>
		/// Implements the != operator.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(CompositeKey left, CompositeKey right)
		{
			return !(left == right);
		}
	}
}