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
		/// Maps the organizations.
		/// </summary>
		/// <param name="csdOrganizations">The CSD organizations.</param>
		/// <returns>Returns a list of organizations.</returns>
		private static IEnumerable<Entity> MapOrganizations(IEnumerable<organization> csdOrganizations)
		{
			var organizations = new List<Entity>();

			foreach (var csdOrganization in csdOrganizations)
			{
				var organization = GetOrCreateEntity<Organization>(csdOrganization.entityID);

				// addresses
				if (csdOrganization.address?.Any() == true)
				{
					ShowInfoMessage("Mapping organization addresses...");

					var addresses = ReconcileVersionedAssociations(organization.Addresses, csdOrganization.address?.Select(a => MapEntityAddress(a, new Uri(AddressTypeCodeSystem)))).Cast<EntityAddress>();

					organization.Addresses.AddRange(addresses);
				}

				// map type concept
				if (csdOrganization.codedType?.Any() == true)
				{
					ShowInfoMessage("Mapping organization type concept...");

					// we don't support multiple specialties for a organization at the moment, so we only take the first one
					// TODO: cleanup
					organization.TypeConceptKey = MapCodedType(csdOrganization.codedType[0].code, csdOrganization.codedType[0].codingScheme)?.Key;
				}

				// map specializations
				if (csdOrganization.specialization?.Any() == true)
				{
					ShowInfoMessage("Mapping organization industry concept...");

					// we don't support multiple industry values for a organization at the moment, so we only take the first one
					// TODO: cleanup
					organization.IndustryConceptKey = MapCodedType(csdOrganization.specialization[0].code, csdOrganization.specialization[0].codingScheme)?.Key;
				}

				// map extensions
				if (csdOrganization.extension?.Any() == true)
				{
					ShowInfoMessage("Mapping organization extensions...");

					var extensions = ReconcileVersionedAssociations(organization.Extensions, csdOrganization.extension.Select(e => MapEntityExtension(e.urn, e.Any.InnerText))).Cast<EntityExtension>();

					organization.Extensions.AddRange(extensions);
				}

				// map identifiers
				if (csdOrganization.otherID?.Any() == true)
				{
					ShowInfoMessage("Mapping organization identifiers...");

					var identifiers = ReconcileVersionedAssociations(organization.Identifiers, csdOrganization.otherID.Select(MapEntityIdentifier)).Cast<EntityIdentifier>();

					organization.Identifiers.AddRange(identifiers);
				}

				Entity parent = null;

				// map parent relationships
				if (csdOrganization.parent?.entityID != null)
				{
					ShowInfoMessage("Mapping organization parent relationships...");

					organization.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);

					parent = GetOrCreateEntity<Organization>(csdOrganization.parent.entityID);

					organization.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Parent, parent));
				}

				// map primary name
				if (csdOrganization.primaryName != null)
				{
					ShowInfoMessage("Mapping organization names...");

					organization.Names.RemoveAll(c => c.NameUseKey == NameUseKeys.OfficialRecord);
					organization.Names.Add(new EntityName(NameUseKeys.OfficialRecord, csdOrganization.primaryName));
				}

				// map names
				if (csdOrganization.otherName?.Any() == true)
				{
					ShowInfoMessage("Mapping organization additional names...");

					var names = ReconcileVersionedAssociations(organization.Names, csdOrganization.otherName.Select(c => MapEntityNameOrganization(NameUseKeys.Assigned, c))).Cast<EntityName>();

					organization.Names.AddRange(names);
				}

				// map tags
				if (csdOrganization.record?.sourceDirectory != null)
				{
					ShowInfoMessage("Mapping organization tags...");

					organization.Tags.RemoveAll(t => t.TagKey == "sourceDirectory");
					organization.Tags.Add(new EntityTag("sourceDirectory", csdOrganization.record.sourceDirectory));
				}

				// map contacts
				if (csdOrganization.contact?.Any() == true)
				{
					ShowInfoMessage("Mapping organization contact relationships...");

					// HACK
					organization.Relationships.RemoveAll(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Contact);
					organization.Relationships.AddRange(csdOrganization.contact.Select(MapEntityRelationshipOrganizationContact));
				}

				// map contact points
				if (csdOrganization.contactPoint?.Any() == true)
				{
					ShowInfoMessage("Mapping organization telecommunications...");

					var telecoms = ReconcileVersionedAssociations(organization.Telecoms, csdOrganization.contactPoint.Select(c => MapContactPoint(TelecomAddressUseKeys.Public, c))).Cast<EntityTelecomAddress>();

					organization.Telecoms.AddRange(telecoms);
				}

				// map status concept
				if (csdOrganization.record?.status != null)
				{
					ShowInfoMessage("Mapping organization status...");

					organization.StatusConceptKey = MapStatusCode(csdOrganization.record.status, "http://openiz.org/csd/CSD-OrganizationStatusCodes");
				}

				organizations.Add(organization);

				if (parent != null && organizations.Any(c => c.Identifiers.All(i => i.Value != csdOrganization.parent?.entityID)))
				{
					organizations.Add(parent);
				}

				if (!entityMap.ContainsKey(csdOrganization.entityID))
				{
					entityMap.Add(csdOrganization.entityID, organization);
				}

				if (parent != null && !entityMap.ContainsKey(csdOrganization.parent?.entityID))
				{
					entityMap.Add(csdOrganization.parent?.entityID, parent);
				}

				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine($"Mapped organization: {organization.Key.Value} {string.Join(" ", organization.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
				Console.ResetColor();
			}

			return organizations;
		}
	}
}