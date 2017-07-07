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
 * Date: 2017-7-6
 */
using System;
using System.Collections.Generic;
using System.Linq;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public partial class CsdImport
	{
		/// <summary>
		/// Maps the providers.
		/// </summary>
		/// <param name="csdProviders">The CSD providers.</param>
		/// <returns>Returns a list of providers.</returns>
		private static IEnumerable<Provider> MapProviders(IEnumerable<provider> csdProviders)
		{
			var providerService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();
			var providers = new List<Provider>();

			foreach (var csdProvider in csdProviders)
			{
				var key = Guid.NewGuid();

				int totalResults;

				// try get existing
				var provider = providerService.Query(c => c.Identifiers.Any(i => i.Value == csdProvider.entityID), 0, 1, AuthenticationContext.SystemPrincipal, out totalResults).FirstOrDefault();

				if (provider == null)
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine($"Provider not found using key: {key}, will create one {Environment.NewLine}");
					Console.ResetColor();

					provider = new Provider
					{
						Addresses = csdProvider.demographic?.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem))).ToList() ?? new List<EntityAddress>(),
						CreationTime = csdProvider.record?.created ?? DateTimeOffset.Now,
						DateOfBirth = csdProvider.demographic?.dateOfBirth,
						Extensions = csdProvider.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList() ?? new List<EntityExtension>(),
						Identifiers = csdProvider.otherID?.Select(MapEntityIdentifier).ToList() ?? new List<EntityIdentifier>(),
						Key = key,
						LanguageCommunication = csdProvider.language?.Select(MapLanguageCommunication).ToList() ?? new List<PersonLanguageCommunication>(),
						StatusConceptKey = csdProvider.record?.status != null ? MapStatusCode(csdProvider.record?.status, "http://openiz.org/csd/CSD-ProviderStatusCodes") : StatusKeys.Active,
						Tags = new List<EntityTag>
						{
							new EntityTag(ImportedDataTag, "true")
						}
					};
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine($"Provider found using key: {key}, will reset basic properties {Environment.NewLine}");
					Console.ResetColor();

					// reset basic properties
					provider.CreationTime = DateTimeOffset.Now;
					provider.PreviousVersion = null;
					provider.VersionKey = null;
					provider.VersionSequence = null;
				}

				// map specialty key
				if (csdProvider.specialty?.Any() == true)
				{
					// we don't support multiple specialties for a provider at the moment, so we only take the first one
					// TODO: cleanup
					provider.ProviderSpecialtyKey = MapCodedType(csdProvider.specialty[0].code, csdProvider.specialty[0].codingScheme)?.Key;
				}

				// map type concept
				if (csdProvider.codedType?.Any() == true)
				{
					// we don't support multiple types for a provider at the moment, so we only take the first one
					// TODO: cleanup
					provider.TypeConceptKey = MapCodedType(csdProvider.codedType[0].code, csdProvider.codedType[0].codingScheme)?.Key;
				}

				// map names
				if (csdProvider.demographic?.name?.Any() == true)
				{
					provider.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
					provider.Names.AddRange(csdProvider.demographic.name.Select(c => MapEntityNamePerson(NameUseKeys.OfficialRecord, c)));
				}

				// map contact points
				if (csdProvider.demographic?.contactPoint?.Any() == true)
				{
					provider.Telecoms.RemoveAll(t => t.AddressUseKey == TelecomAddressUseKeys.Public);
					provider.Telecoms.AddRange(csdProvider.demographic.contactPoint.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c)));
				}

				providers.Add(provider);

				Console.WriteLine($"Mapped provider: {provider.Key.Value} {string.Join(" ", provider.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
			}

			return providers;
		}
	}
}
