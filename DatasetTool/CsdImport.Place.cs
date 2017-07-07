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

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security;
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
		private static IEnumerable<Place> MapPlaces(IEnumerable<facility> csdFacilities)
		{
			var placeService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
			var places = new List<Place>();

			foreach (var facility in csdFacilities)
			{
				var key = Guid.NewGuid();

				int totalResults;

				// try get existing
				var place = placeService.Query(c => c.Identifiers.Any(i => i.Value == facility.entityID), 0, 1, AuthenticationContext.SystemPrincipal, out totalResults).FirstOrDefault();

				if (place == null)
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine($"Place not found using key: {key}, will create one {Environment.NewLine}");
					Console.ResetColor();

					place = new Place
					{
						Addresses = facility.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem))).ToList() ?? new List<EntityAddress>(),
						CreationTime = facility.record?.created ?? DateTimeOffset.Now,
						Extensions = facility.extension?.Select(e => MapEntityExtension(e.urn, e.type)).ToList() ?? new List<EntityExtension>(),
						Identifiers = facility.otherID?.Select(MapEntityIdentifier).ToList() ?? new List<EntityIdentifier>(),
						Key = key,
						StatusConceptKey = facility.record?.status != null ? MapStatusCode(facility.record?.status, "http://openiz.org/csd/CSD-FacilityStatusCode") : StatusKeys.Active,
						Tags = new List<EntityTag>
						{
							new EntityTag(ImportedDataTag, "true")
						}
					};
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine($"Place found using key: {key}, will reset basic properties {Environment.NewLine}");
					Console.ResetColor();

					// reset basic properties
					place.CreationTime = DateTimeOffset.Now;
					place.PreviousVersion = null;
					place.VersionKey = null;
					place.VersionSequence = null;
				}

				// map latitude
				if (facility.geocode?.latitude != null)
				{
					place.Lat = Convert.ToDouble(facility.geocode.latitude);
				}

				// map longitude
				if (facility.geocode?.longitude != null)
				{
					place.Lng = Convert.ToDouble(facility.geocode.longitude);
				}

				// map parent relationships
				if (facility.parent?.entityID != null)
				{
					place.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
					place.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, LookupByEntityId<Place>(facility.parent.entityID)));
				}

				// map coded type
				if (facility.codedType?.Any() == true)
				{
					// we don't support multiple types for a place at the moment, so we only take the first one
					// TODO: cleanup
					place.TypeConceptKey = MapCodedType(facility.codedType[0].code, facility.codedType[0].codingScheme)?.Key;
				}

				// map primary name
				if (facility.primaryName != null)
				{
					place.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
					place.Names.Add(new EntityName(NameUseKeys.OfficialRecord, facility.primaryName));
				}

				// map other names
				if (facility.otherName?.Any() == true)
				{
					place.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.Assigned);
					place.Names.AddRange(facility.otherName.Select(f => new EntityName(NameUseKeys.Assigned, f.Value)));
				}

				// map organization relationships
				if (facility.organizations?.Any() == true)
				{
					//place.Relationships.RemoveAll(r => r.)
				}

				// map contact points
				if (facility.contactPoint?.Any() == true)
				{
					place.Telecoms.RemoveAll(c => c.AddressUseKey == TelecomAddressUseKeys.Public);
					place.Telecoms.AddRange(facility.contactPoint.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c)));
				}

				places.Add(place);

				Console.WriteLine($"Mapped place: {place.Key.Value} {string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
			}

			return places;
		}
	}
}