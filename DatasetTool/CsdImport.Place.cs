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
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
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
        private static IEnumerable<Entity> MapPlaces(IEnumerable<facility> csdFacilities, IEnumerable<organization> csdOrgaizations, CsdOptions options)
        {
            var places = new List<Entity>();

            int idx = 0;
            foreach (var facility in csdFacilities)
            {
                var place = GetOrCreateEntity<Place>(facility.entityID, options.EntityUidAuthority, options);

                if (place == null)
                    ShowErrorOnNotFound($"Facility with entityID {facility.entityID} not found in database and it cannot be created");

                if (!options.RelationshipsOnly)
                {
                    place.ClassConceptKey = EntityClassKeys.ServiceDeliveryLocation;
                    place.StatusConceptKey = StatusKeys.Active;
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
                    if (facility.extension?.Any(o => o.type != options.FacilityTypeExtension) == true)
                    {
                        ShowInfoMessage("Mapping place extensions...");

                        var extensions = ReconcileVersionedAssociations(place.Extensions, facility.extension.Select(e => MapEntityExtension(e.urn, e.Any.InnerText))).Cast<EntityExtension>();

                        place.Extensions.AddRange(extensions);
                    }

                    if (facility.extension.Any(o => o.type == options.FacilityTypeExtension) && !place.TypeConceptKey.HasValue)
                    {
                        var typeExtension = facility.extension.FirstOrDefault(o => o.type == options.FacilityTypeExtension);
                        place.TypeConceptKey = MapCodedType(typeExtension.Any.InnerText, typeExtension.urn + ":facility_type")?.Key;
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

                        parent = GetOrCreateEntity<Place>(facility.parent.entityID, options.EntityUidAuthority, options);

                        place.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, parent));
                    }
                    else if (options.HackParentFind) // HACK: for Tanzania
                    {
                        string hackParentCode = options.ParentOrgCode;

                        // We go to the org
                        var serviceOrg = csdOrgaizations.FirstOrDefault(o => o.entityID == facility.organizations?.FirstOrDefault()?.entityID);
                        var parentFacility = facility;
                        // Then we use the parent until we hit the parent code
                        while (serviceOrg != null)
                        {
                            serviceOrg = csdOrgaizations.FirstOrDefault(o => o.entityID == serviceOrg.parent?.entityID);
                            if (serviceOrg == null) break;
                            parentFacility = csdFacilities.FirstOrDefault(f => f.organizations.Any(o => o.entityID == serviceOrg?.entityID) &&
                                (f.codedType?.Any(c => options.ParentCodeType.Contains(c.code)) == true ||
                                !String.IsNullOrEmpty(options.FacilityTypeExtension) && options.ParentCodeType.Contains(f.extension.FirstOrDefault(o => o.type == options.FacilityTypeExtension)?.Any.InnerText)));
                            // Find the facility which services that
                            if (string.IsNullOrEmpty(hackParentCode) || serviceOrg.codedType.Any(o => o.code == hackParentCode))
                                break;
                        }

                        // Now we want to assign the parent
                        if (parentFacility != null)
                        {
                            place.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
                            parent = GetOrCreateEntity<Place>(parentFacility.entityID, options.EntityUidAuthority, options);
                            place.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, parent));
                            facility.parent = new uniqueID() { entityID = parentFacility.entityID };
                        }

                    }
                    else if (facility.extension.Any(o => o.type == "DistrictVaccinationStore")) // DVS extension
                    {
                        var dvsExtension = facility.extension.FirstOrDefault(o => o.type == "DVS");

                        var dvsPlace = GetOrCreateEntity<Place>(dvsExtension.Any.Attributes["entityID"]?.Value, options.EntityUidAuthority, options);
                        if (dvsPlace != null)
                            place.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, dvsPlace.Key));

                        if (!entityMap.ContainsKey(dvsExtension.Any.Attributes["entityID"]?.Value))
                            entityMap.Add(dvsExtension.Any.Attributes["entityID"]?.Value, dvsPlace);

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

                    if (parent != null && places.Any(c => c.Identifiers.All(i => i.Value != facility.parent?.entityID)))
                    {
                        places.Add(parent);
                    }

                    if (parent != null && !entityMap.ContainsKey(facility.parent?.entityID))
                    {
                        entityMap.Add(facility.parent.entityID, parent);
                    }

                }

                // map organization relationships - These are the villages which the place supports
                if (facility.organizations?.Any() == true)
                {
                    ShowInfoMessage("Mapping serviced organizations relationships...");

                    CreateEntityRelationships(place, facility, csdOrgaizations, options);
                }


                if (facility?.record.created != null)
                {
                    place.CreationTime = facility.record.created;
                }

                places.Add(place);


                if (!entityMap.ContainsKey(facility.entityID))
                {
                    entityMap.Add(facility.entityID, place);
                }


                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Mapped place: ({idx++}/{csdFacilities.Count()}) {place.Key.Value} {string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
                Console.ResetColor();
            }

            return places;
        }

        /// <summary>
        /// Create entity relationships
        /// </summary>
        private static IEnumerable<EntityRelationship> CreateEntityRelationships(Place place, facility facility, IEnumerable<organization> csdOrganizations, CsdOptions options)
        {
            var retVal = new List<EntityRelationship>();
            // Cascade organizations
            foreach (var org in facility.organizations)
            {
                if (String.IsNullOrEmpty(org.entityID))
                    continue;

                var linkedOrgs = new List<String>() { org.entityID };

                if (options.CascadeAssignedFacilities)
                    linkedOrgs = linkedOrgs.Union(csdOrganizations.Where(o => o.parent?.entityID == org.entityID).Select(o => o.entityID)).ToList();

                foreach (var lo in linkedOrgs)
                {
                    var villageServiced = GetOrCreateEntity<Place>(lo, options.EntityUidAuthority, options);

                    if (lo == linkedOrgs.FirstOrDefault() && place.Addresses == null)
                    {
                        var addr = villageServiced.Addresses.FirstOrDefault(o => o.AddressUseKey == AddressUseKeys.Direct)?.Clone() as EntityAddress;
                        place.Addresses.Add(addr);
                    }
                    if (options.NoDbCheck ||
                        !options.RelationshipsOnly || // We need to create everything anyways
                        ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>().Count(x => x.SourceEntityKey == villageServiced.Key && x.TargetEntityKey == place.Key, AuthenticationContext.SystemPrincipal) == 0)
                    {
                        var er = new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, place.Key.Value) { SourceEntityKey = villageServiced.Key };
                        villageServiced.Relationships.Add(er);
                        retVal.Add(er);
                        if (!entityMap.ContainsKey(lo))
                        {
                            entityMap.Add(lo, villageServiced);
                        }
                    }

                }

            }

            return retVal;

        }
    }
}