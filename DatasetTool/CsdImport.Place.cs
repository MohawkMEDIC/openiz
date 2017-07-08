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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
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
		/// Maps the places.
		/// </summary>
		/// <param name="csdFacilities">The CSD facilities.</param>
		/// <returns>Returns a list of places.</returns>
		private static IEnumerable<Entity> MapPlaces(IEnumerable<facility> csdFacilities)
		{
			var places = new List<Entity>();

			foreach (var facility in csdFacilities)
			{
				var place = GetOrCreateEntity<Place>(facility.entityID);

				// map addresses
				if (facility.address?.Any() == true)
				{
					ShowInfoMessage("Mapping place addresses...");

					var addresses = ReconcileVersionedAssociations(place.Addresses, facility.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem)))).Cast<EntityAddress>();

					place.Addresses.AddRange(addresses);
				}

				// map latitude
				if (facility.geocode?.latitude != null)
				{
					ShowInfoMessage("Mapping place latitude...");

					place.Lat = Convert.ToDouble(facility.geocode.latitude);
				}

				// map longitude
				if (facility.geocode?.longitude != null)
				{
					ShowInfoMessage("Mapping place longitude...");

					place.Lng = Convert.ToDouble(facility.geocode.longitude);
				}

				// map extensions
				if (facility.extension?.Any() == true)
				{
					ShowInfoMessage("Mapping place extensions...");

					var extensions = ReconcileVersionedAssociations(place.Extensions, facility.extension.Select(e => MapEntityExtension(e.urn, e.Any.InnerText))).Cast<EntityExtension>();

					place.Extensions.AddRange(extensions);
				}

				// map identifiers
				if (facility.otherID?.Any() == true)
				{
					ShowInfoMessage("Mapping place identifiers...");

					var identifiers = ReconcileVersionedAssociations(place.Identifiers, facility.otherID.Select(MapEntityIdentifier)).Cast<EntityIdentifier>();

					place.Identifiers.AddRange(identifiers);
				}

				Entity parent = null;

				// map parent relationships
				if (facility.parent?.entityID != null)
				{
					ShowInfoMessage("Mapping place parent relationships...");

					place.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);

					parent = GetOrCreateEntity<Place>(facility.parent.entityID);

					place.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, parent));
				}

				// map coded type
				if (facility.codedType?.Any() == true)
				{
					ShowInfoMessage("Mapping place type concept...");

					// we don't support multiple types for a place at the moment, so we only take the first one
					// TODO: cleanup
					place.TypeConceptKey = MapCodedType(facility.codedType[0].code, facility.codedType[0].codingScheme)?.Key;
				}

				// map primary name
				if (facility.primaryName != null)
				{
					ShowInfoMessage("Mapping place names...");

					place.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
					place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, facility.primaryName));
				}

				// map other names
				if (facility.otherName?.Any() == true)
				{
					ShowInfoMessage("Mapping place additional names...");

					var names = ReconcileVersionedAssociations(place.Names, facility.otherName.Select(c => new EntityName(NameUseKeys.Assigned, c.Value))).Cast<EntityName>();

					place.Names.AddRange(names);
				}

				// map organization relationships
				if (facility.organizations?.Any() == true)
				{
					//place.Relationships.RemoveAll(r => r.)
				}

				// map contact points
				if (facility.contactPoint?.Any() == true)
				{
					ShowInfoMessage("Mapping place telecommunications...");

					var telecoms = ReconcileVersionedAssociations(place.Telecoms, facility.contactPoint.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c))).Cast<EntityTelecomAddress>();

					place.Telecoms.AddRange(telecoms);
				}

				// map status concept
				if (facility.record?.status != null)
				{
					ShowInfoMessage("Mapping place status...");

					place.StatusConceptKey = MapStatusCode(facility.record.status, "http://openiz.org/csd/CSD-FacilityStatusCode");
				}

				places.Add(place);

				if (parent != null && places.Any(c => c.Identifiers.All(i => i.Value != facility.parent?.entityID)))
				{
					places.Add(parent);
				}

				if (!entityMap.ContainsKey(facility.entityID))
				{
					entityMap.Add(facility.entityID, place);
				}

				if (parent != null && !entityMap.ContainsKey(facility.parent?.entityID))
				{
					entityMap.Add(facility.parent.entityID, parent);
				}

				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine($"Mapped place: {place.Key.Value} {string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
				Console.ResetColor();
			}

			return places;
		}
	}
}