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
 * Date: 2017-4-8
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core;
using OpenIZ.Core.Model.Collection;
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
using System.Xml.Serialization;
using OpenIZ.Core.Model;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	[Description("Care Services Discovery (CSD) tooling")]
	public partial class CsdImport
	{
		/// <summary>
		/// The address component code system.
		/// </summary>
		private const string AddressComponentCodeSystem = "urn:ihe:iti:csd:2013:address";

		/// <summary>
		/// The address type code system.
		/// </summary>
		private const string AddressTypeCodeSystem = "urn:ihe:iti:csd:2013:addressType";

		/// <summary>
		/// The imported data tag.
		/// </summary>
		private const string ImportedDataTag = "http://openiz.org/tags/contrib/importedData";

		/// <summary>
		/// The program exit message.
		/// </summary>
		private const string ProgramExitMessage = "Unable to continue import CSD document, press any key to exit.";

		/// <summary>
		/// The concept keys.
		/// </summary>
		private static readonly Dictionary<CompositeKey, Guid> conceptKeys = new Dictionary<CompositeKey, Guid>();

		/// <summary>
		/// The related entities.
		/// </summary>
		private static readonly Dictionary<string, Entity> relatedEntities = new Dictionary<string, Entity>();

		/// <summary>
		/// The emergency message.
		/// </summary>
		private static string emergencyMessage;

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
		[Example("Import a Care Services Discovery (CSD) export to a DATASET import file", "--tool=CsdImport --operation=ImportCsd --file=CSD-Organizations-Connectathon-20150120.xml --live")]
		public static void ImportCsd(string[] args)
		{
			ApplicationContext.Current.Start();

			Console.WriteLine("Adding service providers...");

			ApplicationContext.Current.AddServiceProvider(typeof(LocalEntityRepositoryService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalMetadataRepositoryService));
			ApplicationContext.Current.AddServiceProvider(typeof(LocalConceptRepositoryService));

			AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

			ApplicationContext.Current.Started += (o, e) =>
			{
				emergencyMessage = ApplicationContext.Current.GetLocaleString("01189998819991197253");
			};

			var parameters = new ParameterParser<CsdOptions>().Parse(args);

			var csdDatasetInstall = new DatasetInstall { Id = "HFR via CSD, Organizations, Places, Providers, Services", Action = new List<DataInstallAction>() };

			var actions = new List<DataInstallAction>();

			var serializer = new XmlSerializer(typeof(CSD));

			var fileInfo = new FileInfo(parameters.File);

			Console.WriteLine($"Loading file: {fileInfo.Name}...");

			var csd = (CSD)serializer.Deserialize(new StreamReader(parameters.File));

			Console.WriteLine($"File: {fileInfo.Name} loaded successfully, starting mapping process...");

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

			actions.AddRange(organizations);

			var places = MapPlaces(csd.facilityDirectory).Select(p => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = p
			});

			stopwatch.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Mapped {places.Count()} places in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");
			Console.ResetColor();

			actions.AddRange(places);

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

			actions.AddRange(providers);

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

			actions.AddRange(services);

			var entities = new List<Entity>();
			var relationships = new List<EntityRelationship>();

			foreach (var entity in actions.Where(a => a.Element is Entity).Select(c => c.Element as Entity))
			{
				relationships.AddRange(entity.Relationships);

				// HACK: clear the entity relationships because we are going to import them separately
				entity.Relationships.Clear();

				entities.Add(entity);
			}

			// add entities to the list of items to import
			csdDatasetInstall.Action.AddRange(entities.Select(e => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = e
			}).ToList());

			// add relationships to the list of items to import
			csdDatasetInstall.Action.AddRange(relationships.Select(e => new DataUpdate
			{
				InsertIfNotExists = true,
				Element = e
			}).ToList());

			// add the places services to the list of items to import
			csdDatasetInstall.Action.AddRange(services);

			serializer = new XmlSerializer(typeof(DatasetInstall));

			var filename = $"999-CSD-import-{fileInfo.Name}.dataset";

			using (var fileStream = File.Create(filename))
			{
				serializer.Serialize(fileStream, csdDatasetInstall);
			}

			Console.WriteLine($"Dataset file created: {filename}");

			if (parameters.Live)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Warning, the live flag is set to true, data will be imported directly into the database");
				Console.ResetColor();

				var bundle = new Bundle
				{
					Item = actions.Select(a => a.Element).ToList()
				};

				Console.WriteLine("Starting live import");

				var bundlePersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();

				Console.WriteLine("Importing data directly into the database...");

				bundlePersistenceService.Insert(bundle, AuthenticationContext.SystemPrincipal, TransactionMode.Rollback);

				Console.WriteLine("The CSD live import is now complete");
			}
		}

		/// <summary>
		/// Looks up the entity by entity identifier. This will also create a new entity if one is not found.
		/// </summary>
		/// <typeparam name="T">The type of entity to lookup.</typeparam>
		/// <param name="entityId">The entity identifier.</param>
		/// <returns>Returns the entity instance.</returns>
		private static T GetOrCreateEntity<T>(string entityId) where T : Entity, new()
		{
			Entity entity;

			if (relatedEntities.TryGetValue(entityId, out entity))
			{
				return entity as T;
			}

			var entityService = ApplicationContext.Current.GetService<IDataPersistenceService<T>>();

			int totalResults;
			entity = entityService.Query(c => c.Identifiers.Any(i => i.Value == entityId) && c.ObsoletionTime == null, 0, 1, AuthenticationContext.SystemPrincipal, out totalResults).FirstOrDefault();

			if (totalResults > 1)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Warning, found multiple entities with the same entityID: '{entityId}'");
				Console.WriteLine($"Will default to: '{entity.Key.Value}' {Environment.NewLine}");
				Console.ResetColor();
			}

			// if the entity wasn't found, we want to create a new entity
			if (entity != null)
			{
				return (T)entity;
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Warning, ENTITY NOT FOUND, will create one");
			Console.ResetColor();

			// setup basic properties of the entity instance
			entity = new T
			{
				CreationTime = DateTimeOffset.Now,
				Key = Guid.NewGuid(),
				StatusConceptKey = StatusKeys.Active,
				Tags = new List<EntityTag>
				{
					new EntityTag(ImportedDataTag, "true")
				}
			};

			entity.Identifiers.Add(new EntityIdentifier("HIE_FRID", entityId));

			return (T)entity;
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

			// lookup by NSID
			var assigningAuthority = metadataService.FindAssigningAuthority(a => a.DomainName == otherId.assigningAuthorityName).FirstOrDefault();

			if (assigningAuthority != null)
			{
				return assigningAuthority;
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Warning, unable to locate assigning authority by NSID using value: {otherId.assigningAuthorityName}, will attempt to lookup by URL");
			Console.ResetColor();

			// lookup by URL
			assigningAuthority = metadataService.FindAssigningAuthority(a => a.Url == otherId.assigningAuthorityName).FirstOrDefault();

			if (assigningAuthority != null)
			{
				return assigningAuthority;
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Warning, unable to locate assigning authority by URL using value: {otherId.assigningAuthorityName}, will attempt to lookup by OID");
			Console.ResetColor();

			// lookup by OID
			assigningAuthority = metadataService.FindAssigningAuthority(a => a.Oid == otherId.assigningAuthorityName).FirstOrDefault();

			if (assigningAuthority == null)
			{
				ShowExitOnNotFound($"Error, {emergencyMessage} Unable to locate assigning authority using URL or OID. Has {otherId.assigningAuthorityName} been added to the OpenIZ assigning authority list?");
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

			// since CSD coded types are oid based, we want to make sure that the coding scheme starts with "oid"
			if (!codingScheme.StartsWith("oid:") && !codingScheme.StartsWith("http://") && !codingScheme.StartsWith("urn:"))
			{
				codingScheme = "oid:" + codingScheme;
			}

			var compositeKey = new CompositeKey(code, codingScheme);

			Concept concept;

			if (conceptKeys.All(c => c.Key != compositeKey))
			{
				var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

				if (conceptRepositoryService == null)
				{
					throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
				}

				concept = conceptRepositoryService.FindConceptsByReferenceTerm(code, new Uri(codingScheme)).FirstOrDefault();

				if (concept == null)
				{
					ShowExitOnNotFound($"Error, {emergencyMessage} Unable to locate concept using code: {code} and coding scheme: {codingScheme}");
				}
				else
				{
					conceptKeys.Add(compositeKey, concept.Key.Value);
				}
			}
			else
			{
				Guid key;
				conceptKeys.TryGetValue(compositeKey, out key);
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
				ShowExitOnNotFound($"Error, {emergencyMessage} Unable to locate extension type: {extensionUrl}, has this been added to the OpenIZ extension types list?");
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
		private static EntityRelationship MapEntityRelationshipOrganizationContact(organizationContact contact)
		{
			EntityRelationship entityRelationship = null;

			if (contact.Item.GetType() == typeof(uniqueID))
			{
				var csdProvider = contact.Item as uniqueID;

				var provider = GetOrCreateEntity<Provider>(csdProvider.entityID);

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
				ShowExitOnNotFound($"Error, {emergencyMessage} {nameof(organizationContact.Item)} is not of type: {nameof(person)} or {nameof(uniqueID)}");
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
				ShowExitOnNotFound($"Error, {emergencyMessage} language not found using code: {language.code} or using value: {language.Value}");
			}

			return personLanguageCommunication;
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
		/// Reconciles the versioned associations.
		/// </summary>
		/// <param name="existingAddresses">The existing addresses.</param>
		/// <param name="newAddresses">The new addresses.</param>
		/// <returns>System.Collections.Generic.IEnumerable&lt;OpenIZ.Core.Model.VersionedAssociation&lt;OpenIZ.Core.Model.Entities.Entity&gt;&gt;.</returns>
		private static IEnumerable<VersionedAssociation<Entity>> ReconcileVersionedAssociations(IEnumerable<VersionedAssociation<Entity>> existingAddresses, IEnumerable<VersionedAssociation<Entity>> newAddresses)
		{
			return (from address
					in newAddresses
					from organizationAddress
					in existingAddresses
					where !organizationAddress.SemanticEquals(address)
					select address).ToList();
		}

		/// <summary>
		/// Exits the application, when an entity is not found.
		/// </summary>
		/// <param name="message">The message.</param>
		private static void ShowExitOnNotFound(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.WriteLine(ProgramExitMessage);
			Console.ResetColor();
			Console.ReadKey();
			Environment.Exit(999);
		}

		/// <summary>
		/// Prints an informational message.
		/// </summary>
		/// <param name="message">The message.</param>
		private static void ShowInfoMessage(string message)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"{message} {Environment.NewLine}");
			Console.ResetColor();
		}

		/// <summary>
		/// Prints a warning when an entity is not found.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="defaultValueName">Default name of the value.</param>
		/// <param name="defaultValue">The default value.</param>
		private static void ShowWarningOnNotFound(string message, string defaultValueName, Guid defaultValue)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.WriteLine($"Defaulting to {defaultValueName} {defaultValue} {Environment.NewLine}");
			Console.ResetColor();
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
			[Description("The path to the CSD file")]
			public string File { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="CsdOptions"/> is live.
			/// </summary>
			/// <value><c>true</c> if true, data is directly imported into the database vs generating dataset files to be imported at a later date; otherwise, <c>false</c>.</value>
			[Parameter("live")]
			[Description("Directly import data into the database vs generating dataset files to import at a later date")]
			public bool Live { get; set; }
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
		/// Implements the != operator.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(CompositeKey left, CompositeKey right)
		{
			return !(left == right);
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

			return left?.FirstKey == right?.FirstKey && left?.SecondKey == right?.SecondKey;
		}

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
			return this.FirstKey.GetHashCode() ^ this.SecondKey.GetHashCode();
		}
	}

	/// <summary>
	/// Represents an entity comparer to sort based on parent relationships, inorder to preserve the hierarchy is one exists.
	/// </summary>
	/// <seealso cref="Entity" />
	internal class EntityComparer : IComparer<Entity>
	{
		/// <summary>
		/// Compares the specified left.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>System.Int32.</returns>
		public int Compare(Entity left, Entity right)
		{
			var result = 0;

			Console.WriteLine($"Comparing: {left.Key} to {right.Key}");

			// don't move position
			if (left.Key == right.Key)
			{
				return result;
			}

			// if I have no relationships, or the next entries relationship of type parent's target is me, move up
			if (left.Relationships.All(r => r.RelationshipTypeKey != EntityRelationshipTypeKeys.Parent) ||
				right.Relationships.Any(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent && r.TargetEntityKey == left.Key))
			{
				result = 1;
			}
			// if I am the next entries target, move down
			else if (left.Relationships.Any(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent && r.TargetEntityKey == right.Key))
			{
				result = -1;
			}

			return result;
		}
	}
}