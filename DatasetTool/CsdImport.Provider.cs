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
 * Date: 2017-7-6
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Core.Model.DataTypes;

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
		private static IEnumerable<Provider> MapProviders(IEnumerable<provider> csdProviders, CsdOptions options)
		{
			var providers = new List<Provider>();

			foreach (var csdProvider in csdProviders)
			{
				var provider = GetOrCreateEntity<Provider>(csdProvider.entityID, options.EntityUidAuthority, options);

				// map addresses
				if (csdProvider.demographic?.address?.Any() == true)
				{
					ShowInfoMessage("Mapping provider addresses...");

					var addresses = ReconcileVersionedAssociations(provider.Addresses, csdProvider.demographic.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem)))).Cast<EntityAddress>();

					provider.Addresses.AddRange(addresses);
				}

				// map date of birth
				if (csdProvider.demographic?.dateOfBirthSpecified == true)
				{
					ShowInfoMessage("Mapping provider date of birth...");

					provider.DateOfBirth = csdProvider.demographic.dateOfBirth;
				}

				// map specialty key
				if (csdProvider.specialty?.Any() == true)
				{
					ShowInfoMessage("Mapping provider specialty concept...");

					// we don't support multiple specialties for a provider at the moment, so we only take the first one
					// TODO: cleanup
					provider.ProviderSpecialtyKey = MapCodedType(csdProvider.specialty[0].code, csdProvider.specialty[0].codingScheme)?.Key;
				}

				// map type concept
				if (csdProvider.codedType?.Any() == true)
				{
					ShowInfoMessage("Mapping provider type concept...");

					// we don't support multiple types for a provider at the moment, so we only take the first one
					// TODO: cleanup
					provider.TypeConceptKey = MapCodedType(csdProvider.codedType[0].code, csdProvider.codedType[0].codingScheme)?.Key;
				}

				// map extensions
				if (csdProvider.extension?.Any() == true)
				{
					ShowInfoMessage("Mapping provider extensions...");

					var extensions = ReconcileVersionedAssociations(provider.Extensions, csdProvider.extension.Select(e => MapEntityExtension(e.urn, e.Any.InnerText))).Cast<EntityExtension>();

					provider.Extensions.AddRange(extensions);
				}

				// map identifiers
				if (csdProvider.otherID?.Any() == true)
				{
					ShowInfoMessage("Mapping provider identifiers...");

					var identifiers = ReconcileVersionedAssociations(provider.Identifiers, csdProvider.otherID.Select(MapEntityIdentifier)).Cast<EntityIdentifier>();

					provider.Identifiers.AddRange(identifiers);
				}

				// map language communication
				if (csdProvider.language?.Any() == true)
				{
					ShowInfoMessage("Mapping provider languages...");

					var languages = ReconcileVersionedAssociations(provider.LanguageCommunication, csdProvider.language.Select(MapLanguageCommunication)).Cast<PersonLanguageCommunication>();

					provider.LanguageCommunication.AddRange(languages);
				}

				// map names
				if (csdProvider.demographic?.name?.Any() == true)
				{
					ShowInfoMessage("Mapping provider names...");

					provider.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
					provider.Names.AddRange(csdProvider.demographic.name.Select(c => MapEntityNamePerson(NameUseKeys.OfficialRecord, c)));
				}

				// map status concept
				if (csdProvider.record?.status != null)
				{
					ShowInfoMessage("Mapping provider status...");

					provider.StatusConceptKey = MapStatusCode(csdProvider.record.status, "http://openiz.org/csd/CSD-ProviderStatusCodes");
				}

				// map contact points
				if (csdProvider.demographic?.contactPoint?.Any() == true)
				{
					ShowInfoMessage("Mapping provider telecommunications...");

					var telecoms = ReconcileVersionedAssociations(provider.Telecoms, csdProvider.demographic.contactPoint.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c))).Cast<EntityTelecomAddress>();

					provider.Telecoms.AddRange(telecoms);
				}

				providers.Add(provider);

				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine($"Mapped provider: {provider.Key.Value} {string.Join(" ", provider.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
				Console.ResetColor();
			}

			return providers;
		}
	}
}