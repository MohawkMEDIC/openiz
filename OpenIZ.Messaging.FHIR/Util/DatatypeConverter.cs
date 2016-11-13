/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

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
        public static FhirCoding Convert(ReferenceTerm rt)
        {
            if (rt == null)
                return null;
            return new FhirCoding(new Uri(rt.CodeSystem.Url ?? String.Format("urn:oid:{0}", rt.CodeSystem.Oid)), rt.Mnemonic);
        }

        /// <summary>
        /// Convert the telecommunications address
        /// </summary>
        public static FhirTelecom Convert(EntityTelecomAddress tel)
        {
            return new FhirTelecom()
            {
                Use = DatatypeConverter.Convert(tel.AddressUse)?.GetPrimaryCode()?.Code,
                Value = tel.IETFValue
            };
        }

        /// <summary>
        /// Convert the entity address to a FHIR address
        /// </summary>
        public static FhirAddress Convert(EntityAddress addr)
        {
            if (addr == null) return null;

            // Return value
            var retVal = new FhirAddress()
            {
                Use = DatatypeConverter.Convert(addr.AddressUse)?.GetPrimaryCode()?.Code,
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
        public static FhirHumanName Convert(EntityName en)
        {
            if (en == null) return null;

            // Return value
            var retVal = new FhirHumanName()
            {
                Use = DatatypeConverter.Convert(en.NameUse)?.GetPrimaryCode()?.Code
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
        public static FhirIdentifier Convert<TBoundModel>(IdentifierBase<TBoundModel> identifier) where TBoundModel : VersionedEntityData<TBoundModel>, new()
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
        /// Convert the specified concept
        /// </summary>
        public static FhirCodeableConcept Convert(Concept concept)
        {
            if (concept == null)
                return null;
            return new FhirCodeableConcept()
            {
                Coding = concept.ReferenceTerms.Select(o => DatatypeConverter.Convert(o.ReferenceTerm)).ToList(),
                Text = concept.ConceptNames.FirstOrDefault()?.Name
            };
        }

        internal static FhirCodeableConcept Convert(object site)
        {
            throw new NotImplementedException();
        }
    }
}
