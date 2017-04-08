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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MARC.HI.EHRS.SVC.Core;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Persistence;
using OpenIZ.Core.Services;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public class CsdImport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CsdImport"/> class.
		/// </summary>
		public CsdImport()
		{
			
		}

		/// <summary>
		/// Console parameters
		/// </summary>
		private class ConsoleParameters
		{
			/// <summary>
			/// Gets or sets the file.
			/// </summary>
			[Parameter("file")]
			public string File { get; set; }
		}

		/// <summary>
		/// Imports the CSD.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public static void ImportCsd(string[] args)
		{
			var parameters = new ParameterParser<ConsoleParameters>().Parse(args);

			var healthFacilityDataSetInstall = new DatasetInstall { Id = "Health Facilities", Action = new List<DataInstallAction>() };

			var serializer = new XmlSerializer(typeof(CSD));

			var csd = (CSD)serializer.Deserialize(new StreamReader(parameters.File));

			// map CSD organization to IMSI organization
			foreach (var csdOrganization in csd.organizationDirectory)
			{
				var organization = new Organization
				{
					CreationTime = csdOrganization.record?.created ?? DateTimeOffset.Now,
					Key = MapKey(csdOrganization.entityID),
					Tags = new List<EntityTag>
					{
						new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
					},
					StatusConceptKey = MapCode(csdOrganization.record.status).Key
				};

				if (csdOrganization.parent?.entityID != null)
				{
					organization.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, new Organization(Guid.Parse(csdOrganization.parent.entityID))));
				}

				if (csdOrganization.primaryName != null)
				{
					organization.Names.Add(new EntityName(NameUseKeys.OfficialRecord, csdOrganization.primaryName));
				}

				if (csdOrganization.record?.sourceDirectory != null)
				{
					organization.Tags.Add(new EntityTag("sourceDirectory", csdOrganization.record.sourceDirectory));
				}

				organization.Identifiers.AddRange(csdOrganization.otherID.Select(MapEntityIdentifier));
				organization.Names.AddRange(csdOrganization.otherName.Select(MapEntityNameOrganization));
				//organization.Telecoms.add

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
		/// <returns>Concept.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate IConceptRepositoryService</exception>
		private static Concept MapCode(string code)
		{
			var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			if (conceptRepositoryService == null)
			{
				throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
			}

			return null;
		}

		/// <summary>
		/// Maps the entity address.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns>EntityAddress.</returns>
		private static EntityAddress MapEntityAddress(address address)
		{
			var entityAddress = new EntityAddress
			{
				AddressUseKey = AddressUseKeys.Public,
				Component = address.addressLine.Select(a => new EntityAddressComponent
				{
					ComponentTypeKey = MapCode(a.component).Key,
					Value = a.Value
				}).ToList()
			};

			return entityAddress;
		}

		/// <summary>
		/// Maps the entity identifier.
		/// </summary>
		/// <param name="otherId">The other identifier.</param>
		/// <returns>EntityIdentifier.</returns>
		private static EntityIdentifier MapEntityIdentifier(otherID otherId)
		{
			return new EntityIdentifier(MapAssigningAuthority(otherId), otherId.assigningAuthorityName);
		}

		/// <summary>
		/// Maps the name of the entity.
		/// </summary>
		/// <param name="organizationName">Name of the organization.</param>
		/// <returns>EntityName.</returns>
		private static EntityName MapEntityNameOrganization(organizationOtherName organizationName)
		{
			return new EntityName(NameUseKeys.OfficialRecord, organizationName.Value);
		}

		private static EntityTelecomAddress MapEntityTelecomAddress(organizationContact contact)
		{
			var entityTelecomAddress = new EntityTelecomAddress();

			return entityTelecomAddress;
		}

		/// <summary>
		/// Maps the key.
		/// </summary>
		/// <param name="entityId">The entity identifier.</param>
		/// <returns>Guid.</returns>
		private static Guid MapKey(string entityId)
		{
			var key = Guid.NewGuid();

			if (entityId.StartsWith("urn:uuid"))
			{
				key = Guid.Parse(entityId.Substring(8, entityId.Length - 8));
			}

			return key;
		}
	}
}

