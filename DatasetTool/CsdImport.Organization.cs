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
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OizDevTool
{
	/// <summary>
	/// Represents a CSD import utility.
	/// </summary>
	public partial class CsdImport
	{
		/// <summary>
		/// Maps the organizations.
		/// </summary>
		/// <param name="csdOrganizations">The CSD organizations.</param>
		/// <returns>Returns a list of organizations.</returns>
		private static IEnumerable<Entity> MapOrganizations(IEnumerable<organization> csdOrganizations, CsdOptions options)
		{

			var organizations = new List<Entity>();
            IConceptRepositoryService icpr = ApplicationContext.Current.GetService<IConceptRepositoryService>();

            var idx = 0;
			foreach (var csdOrganization in csdOrganizations)
			{

                Entity importItem = null;
                if (!options.OrganizationsAsPlaces)
                    importItem = GetOrCreateEntity<Organization>(csdOrganization.entityID, options.EntityUidAuthority, options);
                else
                {
                    importItem = GetOrCreateEntity<Place>(csdOrganization.entityID, options.EntityUidAuthority, options);
                    // Set proper class code for the place
                    importItem.ClassConceptKey = EntityClassKeys.Place;
                }

                // addresses
                if (csdOrganization.address?.Any() == true)
				{
					ShowInfoMessage("Mapping organization addresses...");

					var addresses = ReconcileVersionedAssociations(importItem.Addresses, csdOrganization.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem)))).Cast<EntityAddress>();

                    importItem.Addresses.AddRange(addresses);
				}

                // If organization as place
                if (options.OrganizationsAsPlaces)  // Import village hiearchy as address
                {
                    importItem.Addresses = new List<EntityAddress>()
                    {
                        TraversePlaceHeirarchyAsAddress(csdOrganization, csdOrganizations)
                    };
                }

				// map type concept
				if (csdOrganization.codedType?.Any() == true)
				{
					ShowInfoMessage("Mapping organization type concept...");

                    // we don't support multiple specialties for a organization at the moment, so we only take the first one
                    // TODO: cleanup
                    var typeConcept = MapCodedType(csdOrganization.codedType[0].code, csdOrganization.codedType[0].codingScheme);
                    importItem.TypeConceptKey = typeConcept?.Key;

                    // We now need to create the proper class concept key
                    Guid classKey = Guid.Empty;
                    if (typeConcept != null && !m_classCodeTypeMaps.TryGetValue(typeConcept.Key.Value, out classKey))
                    {
                        // Look for a relationship in the EntityClass
                        var adptConcept = icpr.FindConcepts(o => o.Relationship.Any(r => r.SourceEntityKey == typeConcept.Key) && o.ConceptSets.Any(s => s.Key == ConceptSetKeys.EntityClass)).FirstOrDefault();
                        if (adptConcept != null)
                            importItem.ClassConceptKey = adptConcept.Key;
                        else if (adptConcept  == null && typeConcept.ConceptSetsXml.Contains(ConceptSetKeys.EntityClass) ||
                            icpr.FindConcepts(o => o.ConceptSets.Any(c => c.Key == ConceptSetKeys.EntityClass) && o.Key == typeConcept.Key).Any())
                            importItem.ClassConceptKey = typeConcept.Key;

                        if (importItem.ClassConceptKey.HasValue)
                            m_classCodeTypeMaps.Add(typeConcept.Key.Value, importItem.ClassConceptKey.Value);
                        else
                            m_classCodeTypeMaps.Add(importItem.ClassConceptKey.Value, Guid.Empty);
                        classKey = importItem.ClassConceptKey ?? EntityClassKeys.Place;
                    }

                    importItem.ClassConceptKey = classKey;
                }

				// map specializations
                
				if (csdOrganization.specialization?.Any() == true && importItem is Organization)
				{
					ShowInfoMessage("Mapping organization industry concept...");

					// we don't support multiple industry values for a organization at the moment, so we only take the first one
					// TODO: cleanup
					(importItem as Organization).IndustryConceptKey = MapCodedType(csdOrganization.specialization[0].code, csdOrganization.specialization[0].codingScheme)?.Key;
				}

				// map extensions
				if (csdOrganization.extension?.Any() == true)
				{
					ShowInfoMessage("Mapping organization extensions...");

					var extensions = ReconcileVersionedAssociations(importItem.Extensions, csdOrganization.extension.Select(e => MapEntityExtension(e.urn, e.Any.InnerText))).Cast<EntityExtension>();

					importItem.Extensions.AddRange(extensions);
				}

				// map identifiers
				if (csdOrganization.otherID?.Any() == true)
				{
					ShowInfoMessage("Mapping organization identifiers...");

					var identifiers = ReconcileVersionedAssociations(importItem.Identifiers, csdOrganization.otherID.Select(MapEntityIdentifier)).Cast<EntityIdentifier>();

                    importItem.Identifiers.AddRange(identifiers);
				}

				Entity parent = null;

				// map parent relationships
				if (csdOrganization.parent?.entityID != null)
				{
					ShowInfoMessage("Mapping organization parent relationships...");

                    importItem.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);

                    if(!options.OrganizationsAsPlaces)
					    parent = GetOrCreateEntity<Organization>(csdOrganization.parent.entityID, options.EntityUidAuthority, options);
                    else
                        parent = GetOrCreateEntity<Place>(csdOrganization.parent.entityID, options.EntityUidAuthority, options);

                    importItem.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, parent));
				}

				// map primary name
				if (csdOrganization.primaryName != null)
				{
					ShowInfoMessage("Mapping organization names...");

                    importItem.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
                    importItem.Names.Add(new EntityName(NameUseKeys.OfficialRecord, csdOrganization.primaryName));
				}

				// map names
				if (csdOrganization.otherName?.Any() == true)
				{
					ShowInfoMessage("Mapping organization additional names...");

					var names = ReconcileVersionedAssociations(importItem.Names, csdOrganization.otherName.Select(c => MapEntityNameOrganization(NameUseKeys.Assigned, c))).Cast<EntityName>();

                    importItem.Names.AddRange(names);
				}

				// map tags
				if (csdOrganization.record?.sourceDirectory != null)
				{
					ShowInfoMessage("Mapping organization tags...");

                    importItem.Tags.RemoveAll(t => t.TagKey == "sourceDirectory");
                    importItem.Tags.Add(new EntityTag("sourceDirectory", csdOrganization.record.sourceDirectory));
				}

				// map contacts
				if (csdOrganization.contact?.Any() == true)
				{
					ShowInfoMessage("Mapping organization contact relationships...");

                    // HACK
                    importItem.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Contact);
                    importItem.Relationships.AddRange(csdOrganization.contact.Select(o=>MapEntityRelationshipOrganizationContact(o, options)));
				}

				// map contact points
				if (csdOrganization.contactPoint?.Any() == true)
				{
					ShowInfoMessage("Mapping organization telecommunications...");

					var telecoms = ReconcileVersionedAssociations(importItem.Telecoms, csdOrganization.contactPoint.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c))).Cast<EntityTelecomAddress>();

                    importItem.Telecoms.AddRange(telecoms);
				}

				// map status concept
				if (csdOrganization.record?.status != null)
				{
					ShowInfoMessage("Mapping organization status...");

                    importItem.StatusConceptKey = MapStatusCode(csdOrganization.record.status, "http://openiz.org/csd/CSD-OrganizationStatusCodes");
				}

				organizations.Add(importItem);

				if (parent != null && organizations.Any(c => c.Identifiers.All(i => i.Value != csdOrganization.parent?.entityID)))
				{
					organizations.Add(parent);
				}

				if (!entityMap.ContainsKey(csdOrganization.entityID))
				{
					entityMap.Add(csdOrganization.entityID, importItem);
				}

				if (parent != null && !entityMap.ContainsKey(csdOrganization.parent?.entityID))
				{
					entityMap.Add(csdOrganization.parent?.entityID, parent);
				}

				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine($"Mapped organization {(options.OrganizationsAsPlaces ? "as place" : "")} ({idx++}/{csdOrganizations.Count()}): {importItem.Key.Value} {string.Join(" ", importItem.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
				Console.ResetColor();
			}

			return organizations;
		}

        // Code type maps
        private static Dictionary<Guid, Guid> m_addressCodeTypeMaps = new Dictionary<Guid, Guid>();

        // Class code maps
        private static Dictionary<Guid, Guid> m_classCodeTypeMaps = new Dictionary<Guid, Guid>();

        /// <summary>
        /// Traverses a place hierarchy as an address structure
        /// </summary>
        private static EntityAddress TraversePlaceHeirarchyAsAddress(organization importItem, IEnumerable<organization> context)
        {

            EntityAddress retVal = new EntityAddress()
            {
                AddressUseKey = AddressUseKeys.Direct
            };

            // First Census tract?
            var codeID = importItem.otherID.FirstOrDefault(o => o.code == "code");
            
            // Process components
            var parent = importItem;

            IConceptRepositoryService icpr = ApplicationContext.Current.GetService<IConceptRepositoryService>();

            while(parent != null)
            {

                EntityAddressComponent comp = new EntityAddressComponent();
                comp.Value = parent.primaryName;
                // Get the component type from CSD
                Concept codeType = null;
                foreach (var ct in parent.codedType) {
                    codeType = MapCodedType(ct.code, ct.codingScheme);
                    if (codeType != null)
                        break;
                }

                Guid addressCompKey = Guid.Empty;
                if(codeType != null && !m_addressCodeTypeMaps.TryGetValue(codeType.Key.Value, out addressCompKey))
                {
                    if (codeType != null)
                    {
                        // Look for a relationship in the AddressPartType 
                        var adptConcept = icpr.FindConcepts(o => o.Relationship.Any(r => r.SourceEntityKey == codeType.Key) && o.ConceptSets.Any(s => s.Mnemonic == "AddressComponentType")).FirstOrDefault();
                        if (adptConcept != null)
                            comp.ComponentTypeKey = adptConcept.Key;
                    }
                    if (!comp.ComponentTypeKey.HasValue && codeType.ConceptSetsXml.Contains(ConceptSetKeys.AddressComponentType) ||
                        icpr.FindConcepts(o => o.ConceptSets.Any(c => c.Key == ConceptSetKeys.AddressComponentType) && o.Key == codeType.Key).Any())
                        comp.ComponentTypeKey = codeType.Key;

                    if (comp.ComponentTypeKey.HasValue)
                        m_addressCodeTypeMaps.Add(codeType.Key.Value, comp.ComponentTypeKey.Value);
                    else
                        m_addressCodeTypeMaps.Add(codeType.Key.Value, Guid.Empty);
                    addressCompKey = comp.ComponentTypeKey.GetValueOrDefault();

                }

                if (addressCompKey != Guid.Empty)
                    comp.ComponentTypeKey = addressCompKey;
                // Add
                if (comp.ComponentTypeKey.HasValue)
                    retVal.Component.Add(comp);

                // Get parent
                parent = context.FirstOrDefault(o=>o.entityID == parent.parent?.entityID);
            }

            return retVal;

        }
    }
}