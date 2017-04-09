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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public class CsdImport
	{
		/// <summary>
		/// The entity key map.
		/// </summary>
		private static Dictionary<string, Guid> entityKeyMap = new Dictionary<string, Guid>();

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
		public static void ImportCsd(string[] args)
		{
			ApplicationContext.Current.Start();

			ApplicationContext.Current.AddServiceProvider(typeof(LocalMetadataRepositoryService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalConceptRepositoryService));

			AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

			var parameters = new ParameterParser<CsdOptions>().Parse(args);

			var csdDatasetInstall = new DatasetInstall { Id = "CSD", Action = new List<DataInstallAction>() };

			var serializer = new XmlSerializer(typeof(CSD));

			var fileInfo = new FileInfo(parameters.File);

			var csd = (CSD)serializer.Deserialize(new StreamReader(parameters.File));

			var organizations = MapOrganizations(csd.organizationDirectory).Select(o => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = o
			});

			csdDatasetInstall.Action.AddRange(organizations);

			var places = MapPlaces(csd.facilityDirectory).Select(p => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = p
			});

			csdDatasetInstall.Action.AddRange(places);

			var providers = MapProviders(csd.providerDirectory).Select(p => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = p
			});

			csdDatasetInstall.Action.AddRange(providers);

			var services = MapServices(csd.serviceDirectory).Select(s => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = s
			});

			csdDatasetInstall.Action.AddRange(services);

			serializer = new XmlSerializer(typeof(DatasetInstall));

			using (var fileStream = File.Create($"999-CSD-import-{fileInfo.Name}.dataset"))
			{
				serializer.Serialize(fileStream, csdDatasetInstall);
			}
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

			var assigningAuthority = metadataService.FindAssigningAuthority(a => a.Url == otherId.assigningAuthorityName).FirstOrDefault();

			if (assigningAuthority == null)
			{
				throw new InvalidOperationException($"Unable to locate assigning authority: {otherId.assigningAuthorityName}, has this been added to the OpenIZ assigning authorities list?");
			}

			return assigningAuthority;
		}

		/// <summary>
		/// Maps the code.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Concept.</returns>
		/// <exception cref="System.ArgumentNullException">code - Value cannot be null</exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate IConceptRepositoryService</exception>
		private static Concept MapCode(string code, Uri codeSystem)
		{
			if (code == null)
			{
				throw new ArgumentNullException(nameof(code), "Value cannot be null");
			}

			var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			if (conceptRepositoryService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
			}

			return conceptRepositoryService.FindConceptsByReferenceTerm(code, codeSystem).FirstOrDefault();
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

			var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			if (conceptRepositoryService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
			}

			// since CSD coded types are oid based, we want to make sure that the coding scheme starts with "oid"
			if (!codingScheme.StartsWith("oid:"))
			{
				codingScheme = "oid:" + codingScheme;
			}

			return conceptRepositoryService.FindConceptsByReferenceTerm(code, new Uri(codingScheme)).FirstOrDefault();
		}

		/// <summary>
		/// Maps the contact point.
		/// </summary>
		/// <param name="contactPoint">The contact point.</param>
		/// <returns>Returns an entity telecom address.</returns>
		private static EntityTelecomAddress MapContactPoint(contactPoint contactPoint)
		{
			return new EntityTelecomAddress(TelecomAddressUseKeys.Public, contactPoint.codedType?.Value);
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
			var conceptService = ApplicationContext.Current.GetConceptService();

			var entityAddress = new EntityAddress();

			var addressUseConcept = conceptService.FindConceptsByReferenceTerm(address.type, codeSystem).FirstOrDefault();

			if (addressUseConcept == null)
			{
				throw new InvalidOperationException($"Unable to map address use, no related concept found for code {address.type}");
			}

			entityAddress.AddressUseKey = addressUseConcept.Key;

			foreach (var addressComponent in address.addressLine)
			{
				var entityAddressComponent = new EntityAddressComponent
				{
					Value = addressComponent.Value
				};

				var addressComponentConcept = conceptService.FindConceptsByReferenceTerm(addressComponent.component, codeSystem).FirstOrDefault();

				if (addressComponentConcept == null)
				{
					throw new InvalidOperationException($"Unable to map address component, no related concept found for code {addressComponent.component}");
				}

				entityAddressComponent.ComponentTypeKey = addressComponentConcept.Key;

				entityAddress.Component.Add(entityAddressComponent);
			}

			return entityAddress;
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
				throw new InvalidOperationException($"Unable to locate extension type: {extensionUrl}, has this been added to the OpenIZ extension types list?");
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
			return new EntityIdentifier(MapAssigningAuthority(otherId), otherId.assigningAuthorityName);
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
		/// Maps the entity telecom address.
		/// </summary>
		/// <param name="contact">The contact.</param>
		/// <returns>Returns an entity telecom address.</returns>
		private static EntityTelecomAddress MapEntityTelecomAddress(organizationContact contact)
		{
			var entityTelecomAddress = new EntityTelecomAddress();

			return entityTelecomAddress;
		}

		/// <summary>
		/// Maps the key.
		/// </summary>
		/// <param name="entityId">The entity identifier.</param>
		/// <returns>Returns a key for a given entity id.</returns>
		private static Guid MapKey(string entityId)
		{
			var key = Guid.NewGuid();

			if (entityId.StartsWith("urn:uuid:"))
			{
				Guid.TryParse(entityId.Substring(9), out key);
			}

			return key;
		}

		/// <summary>
		/// Maps the organizations.
		/// </summary>
		/// <param name="csdOrganizations">The CSD organizations.</param>
		/// <returns>Returns a list of organizations.</returns>
		private static IEnumerable<Organization> MapOrganizations(IEnumerable<organization> csdOrganizations)
		{
			var organizations = new List<Organization>();

			// map CSD organization to IMSI organization
			foreach (var csdOrganization in csdOrganizations)
			{
				var organization = new Organization
				{
					Addresses = csdOrganization.address?.Select(a => MapEntityAddress(a, new Uri("http://openiz.org/csd/CSD-AddressTypeCodes"))).ToList(),
					CreationTime = csdOrganization.record?.created ?? DateTimeOffset.Now,
					Extensions = csdOrganization.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList(),
					IndustryConceptKey = csdOrganization.codedType?.Select(c => MapCodedType(c.code, c.codingScheme)).FirstOrDefault()?.Key,
					Key = MapKey(csdOrganization.entityID),
					Tags = new List<EntityTag>
					{
						new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
					},
					//StatusConceptKey = MapStatusCode(csdOrganization.record?.status)
				};

				if (csdOrganization.codedType?.Any() == true)
				{
					organization.TypeConceptKey = MapCodedType(csdOrganization.codedType[0].code, csdOrganization.codedType[0].codingScheme)?.Key;
				}

				if (csdOrganization.parent?.entityID != null)
				{
					organization.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, new Organization(MapKey(csdOrganization.parent.entityID))));
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
					Addresses = facility.address?.Select(a => MapEntityAddress(a, new Uri("http://openiz.org/csd/CSD-AddressTypeCodes"))).ToList(),
					CreationTime = facility.record?.created ?? DateTimeOffset.Now,
					Extensions = facility.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList(),
					Key = MapKey(facility.entityID),
					StatusConceptKey = MapStatusCode(facility.record?.status),
					Tags = new List<EntityTag>
					{
						new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
					}
				};

				if (facility.codedType?.Any() == true)
				{
					place.TypeConceptKey = MapCodedType(facility.codedType[0].code, facility.codedType[0].codingScheme)?.Key;
				}

				if (facility.primaryName != null)
				{
					place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, facility.primaryName));
				}

				place.Names.AddRange(facility.otherName.Select(f => new EntityName(NameUseKeys.Assigned, f.Value)));

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
					Addresses = csdProvider.demographic?.address?.Select(a => MapEntityAddress(a, new Uri("http://openiz.org/csd/CSD-AddressTypeCodes"))).ToList(),
					CreationTime = csdProvider.record?.created ?? DateTimeOffset.Now,
					DateOfBirth = csdProvider.demographic?.dateOfBirth,
					Extensions = csdProvider.extension.Select(e => MapEntityExtension(e.urn, e.type)).ToList(),
					Key = MapKey(csdProvider.entityID),
					Identifiers = csdProvider.otherID.Select(MapEntityIdentifier).ToList(),
					Names = csdProvider.demographic?.name.Select(MapEntityNamePerson).ToList(),
					StatusConceptKey = MapStatusCode(csdProvider.record?.status),
					Tags = new List<EntityTag>
					{
						new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
					},
					Telecoms = csdProvider.demographic?.contactPoint?.Select(MapContactPoint).ToList()
				};

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
				};

				services.Add(service);
			}

			return services;
		}

		/// <summary>
		/// Maps the status code.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns>Returns a status key.</returns>
		/// <exception cref="System.ArgumentException">If the code is unknown.</exception>
		private static Guid MapStatusCode(string code)
		{
			if (code == null)
			{
				Console.WriteLine("Warning: unable to map status code of null, defaulting to Active");
				return StatusKeys.Active;
			}

			switch (code)
			{
				case "106-001":
					return StatusKeys.Active;

				case "106-002":
					return StatusKeys.Obsolete;

				default:
					var conceptService = ApplicationContext.Current.GetConceptService();

					if (conceptService == null)
					{
						throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
					}

					throw new ArgumentException();
			}
		}
	}

	/// <summary>
	/// Represents CSD options.
	/// </summary>
	internal class CsdOptions
	{
		/// <summary>
		/// Gets or sets the file.
		/// </summary>
		[Parameter("file")]
		public string File { get; set; }
	}
}