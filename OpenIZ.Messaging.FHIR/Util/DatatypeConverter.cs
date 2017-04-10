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
 * Date: 2016-8-14
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core;

namespace OpenIZ.Messaging.FHIR.Util
{
    /// <summary>
    /// Represents a datatype converter
    /// </summary>
    public static class DatatypeConverter
    {

        /// <summary>
        /// Create the base resource data
        /// </summary>
        public static TResource CreateResource<TResource>(IVersionedEntity resource) where TResource : ResourceBase, new()
        {
            var retVal = new TResource();
            retVal.Id = resource.Key.ToString();
            retVal.VersionId = resource.VersionKey.ToString();
            return retVal;
        }

        /// <summary>
        /// Convert reference term
        /// </summary>
        public static FhirCoding ToCoding(ReferenceTerm rt)
        {
            if (rt == null)
                return null;
            return new FhirCoding(new Uri(rt.CodeSystem.Url ?? String.Format("urn:oid:{0}", rt.CodeSystem.Oid)), rt.Mnemonic);
        }

        /// <summary>
        /// Convert the telecommunications address
        /// </summary>
        public static FhirTelecom ToFhirTelecom(EntityTelecomAddress tel)
        {
            return new FhirTelecom()
            {
                Use = DatatypeConverter.ToFhirCodeableConcept(tel.AddressUse)?.GetPrimaryCode()?.Code,
                Value = tel.IETFValue
            };
        }

        /// <summary>
        /// Convert the entity address to a FHIR address
        /// </summary>
        public static FhirAddress ToFhirAddress(EntityAddress addr)
        {
            if (addr == null) return null;

            // Return value
            var retVal = new FhirAddress()
            {
                Use = DatatypeConverter.ToFhirCodeableConcept(addr.AddressUse)?.GetPrimaryCode()?.Code,
                Line = new List<FhirString>()
            };

            // Process components
            foreach(var com in addr.Component)
            {
                if (com.ComponentTypeKey == AddressComponentKeys.City)
                    retVal.City = com.Value;
                else if (com.ComponentTypeKey == AddressComponentKeys.Country)
                    retVal.Country = com.Value;
                else if (com.ComponentTypeKey == AddressComponentKeys.AddressLine ||
                    com.ComponentTypeKey == AddressComponentKeys.StreetAddressLine)
                    retVal.Line.Add(com.Value);
                else if (com.ComponentTypeKey == AddressComponentKeys.State)
                    retVal.State = com.Value;
                else if (com.ComponentTypeKey == AddressComponentKeys.PostalCode)
                    retVal.Zip = com.Value;
                else
                {
                    retVal.Extension.Add(new Extension()
                    {
                        IsModifier = false,
                        Url = FhirConstants.OpenIZProfile + "#address-" + com.ComponentType.Mnemonic,
                        Value = new FhirString(com.Value)
                    });
                }
            }

            return retVal;
        }

        /// <summary>
        /// Convert the entity name to a FHIR name
        /// </summary>
        public static FhirHumanName ToFhirHumanName(EntityName en)
        {
            if (en == null) return null;

            // Return value
            var retVal = new FhirHumanName()
            {
                Use = DatatypeConverter.ToFhirCodeableConcept(en.NameUse)?.GetPrimaryCode()?.Code
            };

            // Process components
            foreach (var com in en.Component)
            {
                if (com.ComponentTypeKey == NameComponentKeys.Given)
                    retVal.Given.Add(com.Value);
                else if (com.ComponentTypeKey == NameComponentKeys.Family)
                    retVal.Family.Add(com.Value);
                else if (com.ComponentTypeKey == NameComponentKeys.Prefix)
                    retVal.Prefix.Add(com.Value);
                else if (com.ComponentTypeKey == NameComponentKeys.Suffix)
                    retVal.Suffix.Add(com.Value);
            }

            return retVal;
        }

        /// <summary>
        /// Creates a FHIR reference
        /// </summary>
        public static Reference<TResource> CreateReference<TResource>(IVersionedEntity targetEntity) where TResource : DomainResourceBase, new()
        {
            return Reference<TResource>.CreateResourceReference(DatatypeConverter.CreateResource<TResource>(targetEntity), WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri);
        }

        /// <summary>
        /// Convert the model
        /// </summary>
        public static FhirIdentifier ToFhirIdentifier<TBoundModel>(IdentifierBase<TBoundModel> identifier) where TBoundModel : VersionedEntityData<TBoundModel>, new()
        {
            if (identifier == null)
                return null;
            return new FhirIdentifier()
            {
                Label = identifier.Authority?.Name,
                System = new FhirUri(new Uri(identifier.Authority?.Url ?? String.Format("urn:oid:{0}", identifier.Authority?.Oid))),
                Use = identifier.IdentifierType?.TypeConcept?.Mnemonic,
                Value = identifier.Value
            };
        }

        /// <summary>
        /// Gets the concept via the codable concept
        /// </summary>
        public static Concept ToConcept(FhirCodeableConcept codeableConcept)
        {
            if (codeableConcept == null) return null;
            return codeableConcept.Coding.Select(o => DatatypeConverter.ToConcept(o)).FirstOrDefault(o => o != null);
        }

		/// <summary>
		/// Convert from FHIR coding to concept
		/// </summary>
		/// <param name="coding">The coding.</param>
		/// <param name="defaultSystem">The default system.</param>
		/// <returns>Concept.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// Unable to locate service
		/// or
		/// Coding must have system attached
		/// </exception>
		public static Concept ToConcept(FhirCoding coding, FhirUri defaultSystem = null)
        {
            if (coding == null) return null;

            var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

	        if (conceptService == null)
	        {
		        throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
	        }

            var system = coding.System ?? defaultSystem;

	        if (system == null)
	        {
		        throw new InvalidOperationException("Coding must have system attached");
	        }

            // Lookup
            return conceptService.FindConceptsByReferenceTerm(coding.Code, coding.System.Value).FirstOrDefault();
        }

		/// <summary>
		/// Converts a <see cref="FhirCode{T}"/> instance to a <see cref="Concept"/> instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="code">The code.</param>
		/// <param name="system">The system.</param>
		/// <returns>Concept.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// code - Value cannot be null
		/// or
		/// system - Value cannot be null
		/// </exception>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public static Concept ToConcept<T>(FhirCode<T> code, string system)
	    {
		    if (code == null)
		    {
			    throw new ArgumentNullException(nameof(code), "Value cannot be null");
		    }

		    if (system == null)
		    {
			    throw new ArgumentNullException(nameof(system), "Value cannot be null");
		    }

		    var conceptService = ApplicationContext.Current.GetConceptService();

		    if (conceptService == null)
		    {
			    throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
		    }

		    return conceptService.FindConceptsByReferenceTerm(code.Value.ToString(), new Uri(system)).FirstOrDefault();
	    }

		/// <summary>
		/// Convert to assigning authority
		/// </summary>
		/// <param name="fhirSystem">The FHIR system.</param>
		/// <returns>AssigningAuthority.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public static AssigningAuthority ToAssigningAuthority(FhirUri fhirSystem)
        {
	        if (fhirSystem == null)
	        {
		        return null;
	        }

            var metaDataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

	        if (metaDataService == null)
	        {
		        throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
	        }

            return metaDataService.GetAssigningAuthority(fhirSystem.Value);
        }

		/// <summary>
		/// Converts an <see cref="FhirAddress"/> instance to an <see cref="EntityAddress"/> instance.
		/// </summary>
		/// <param name="fhirAddress">The FHIR address.</param>
		/// <returns>Returns an entity address instance.</returns>
		public static EntityAddress ToEntityAddress(FhirAddress fhirAddress)
	    {
		    var address = new EntityAddress();

		    if (fhirAddress.City?.Value != null)
		    {
			    address.Component.Add(new EntityAddressComponent(AddressComponentKeys.City, fhirAddress.City.Value));
		    }

		    if (fhirAddress.Country?.Value != null)
		    {
			    address.Component.Add(new EntityAddressComponent(AddressComponentKeys.Country, fhirAddress.Country.Value));
		    }

		    if (fhirAddress.Line?.Any() == true)
		    {
			    address.Component.AddRange(fhirAddress.Line.Select(a => new EntityAddressComponent(AddressComponentKeys.AddressLine, a.Value)));
		    }

		    if (fhirAddress.State?.Value != null)
		    {
			    address.Component.Add(new EntityAddressComponent(AddressComponentKeys.State, fhirAddress.State.Value));
		    }

		    if (fhirAddress.Zip?.Value != null)
		    {
			    address.Component.Add(new EntityAddressComponent(AddressComponentKeys.PostalCode, fhirAddress.Zip.Value));
		    }

		    return address;
	    }

		/// <summary>
		/// Converts an <see cref="Extension"/> instance to an <see cref="EntityExtension"/> instance.
		/// </summary>
		/// <param name="fhirExtension">The FHIR extension.</param>
		/// <returns>EntityExtension.</returns>
		/// <exception cref="System.ArgumentNullException">fhirExtension - Value cannot be null</exception>
		/// <exception cref="System.InvalidOperationException">
		/// IMetadataRepositoryService
		/// or
		/// Unable to locate extension type
		/// or
		/// IEntityExtensionRepositoryService
		/// </exception>
		public static EntityExtension ToEntityExtension(Extension fhirExtension)
	    {
		    var extension = new EntityExtension();

		    if (fhirExtension == null)
		    {
			    throw new ArgumentNullException(nameof(fhirExtension), "Value cannot be null");
		    }

		    var extensionTypeService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

		    if (extensionTypeService == null)
		    {
			    throw new InvalidOperationException($"Unable to locate service: {nameof(IMetadataRepositoryService)}");
		    }

		    var extensionType = extensionTypeService.FindExtensionType(e => e.Name == fhirExtension.Url).FirstOrDefault();

		    if (extensionType == null)
		    {
			    throw new InvalidOperationException("Unable to locate extension type");
		    }

		    var extensionService = ApplicationContext.Current.GetService<IEntityExtensionRepositoryService>();

		    if (extensionService == null)
		    {
			    throw new InvalidOperationException($"Unable to locate service: {nameof(IEntityExtensionRepositoryService)}");
		    }

		    extension.ExtensionType = extensionType;
		    //extension.ExtensionValue = fhirExtension.Value

		    return extension;
	    }

		/// <summary>
		/// Convert a FhirIdentifier to an identifier
		/// </summary>
		/// <param name="fhirId">The fhir identifier.</param>
		/// <returns>Returns an entity identifier instance.</returns>
		public static EntityIdentifier ToEntityIdentifier(FhirIdentifier fhirId) 
        {
            if (fhirId == null) return null;

            EntityIdentifier retVal = null;
            if (fhirId.System != null)
                retVal = new EntityIdentifier(DatatypeConverter.ToAssigningAuthority(fhirId.System), fhirId.Value.Value);
            else
            {
                var metaService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();
                var assigningAuthority = metaService.FindAssigningAuthority(o => o.DomainName == fhirId.Label).FirstOrDefault();
                retVal = new EntityIdentifier(assigningAuthority.Key.Value, fhirId.Value);
            }
            // TODO: Fill in use
            return retVal;
        }

		/// <summary>
		/// Converts a <see cref="FhirHumanName" /> instance to an <see cref="EntityName" /> instance.
		/// </summary>
		/// <param name="fhirHumanName">The name of the human.</param>
		/// <returns>Returns an entity name instance.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
		public static EntityName ToEntityName(FhirHumanName fhirHumanName)
	    {
			var name = new EntityName();

		    var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

		    if (conceptService == null)
		    {
			    throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
		    }

		    name.NameUseKey = ToConcept(fhirHumanName.Use, "http://hl7.org/fhir/name-use")?.Key;

		    name.Component.AddRange(fhirHumanName.Family.Select(f => new EntityNameComponent(NameComponentKeys.Family, f.Value)));
		    name.Component.AddRange(fhirHumanName.Given.Select(g => new EntityNameComponent(NameComponentKeys.Given, g.Value)));
		    name.Component.AddRange(fhirHumanName.Prefix.Select(p => new EntityNameComponent(NameComponentKeys.Prefix, p.Value)));
		    name.Component.AddRange(fhirHumanName.Suffix.Select(s => new EntityNameComponent(NameComponentKeys.Suffix, s.Value)));

		    return name;
		}

		/// <summary>
		/// Converts a <see cref="FhirTelecom"/> instance to an <see cref="EntityTelecomAddress"/> instance.
		/// </summary>
		/// <param name="fhirTelecom">The telecom.</param>
		/// <returns>Returns an entity telecom address.</returns>
		public static EntityTelecomAddress ToEntityTelecomAddress(FhirTelecom fhirTelecom)
		{
			var telecom = new EntityTelecomAddress
			{
				Value = fhirTelecom.Value.Value,
				AddressUseKey = ToConcept(fhirTelecom.Use, "http://hl7.org/fhir/contact-point-use")?.Key
			};

		    return telecom;
	    }

        /// <summary>
        /// Convert the specified concept
        /// </summary>
        public static FhirCodeableConcept ToFhirCodeableConcept(Concept concept)
        {
            if (concept == null)
                return null;
            return new FhirCodeableConcept()
            {
                Coding = concept.ReferenceTerms.Select(o => DatatypeConverter.ToCoding(o.ReferenceTerm)).ToList(),
                Text = concept.ConceptNames.FirstOrDefault()?.Name
            };
        }
        
    }
}
