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
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Web;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone;
using OpenIZ.Core.Model.Acts;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Security;

namespace OpenIZ.Messaging.FHIR.Util
{
	/// <summary>
	/// Represents a data type converter.
	/// </summary>
	public static class DataTypeConverter
	{
		/// <summary>
		/// The trace source.
		/// </summary>
		private static readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.FHIR");

		/// <summary>
		/// Creates a FHIR reference.
		/// </summary>
		/// <typeparam name="TResource">The type of the t resource.</typeparam>
		/// <param name="targetEntity">The target entity.</param>
		/// <returns>Returns a reference instance.</returns>
		public static Reference<TResource> CreateReference<TResource>(IVersionedEntity targetEntity, WebOperationContext context) where TResource : DomainResourceBase, new()
		{
            if (targetEntity == null)
                throw new ArgumentNullException(nameof(targetEntity));
            else if (context == null)
                throw new ArgumentNullException(nameof(context));
			var refer =  Reference.CreateResourceReference(DataTypeConverter.CreateResource<TResource>(targetEntity), context.IncomingRequest.UriTemplateMatch.BaseUri);
            refer.Display = (targetEntity as Entity)?.Names?.FirstOrDefault()?.ToString();
            return refer;
		}

        /// <summary>
        /// Creates a FHIR reference.
        /// </summary>
        /// <typeparam name="TResource">The type of the t resource.</typeparam>
        /// <param name="targetEntity">The target entity.</param>
        /// <returns>Returns a reference instance.</returns>
        public static Reference CreatePlainReference<TResource>(IVersionedEntity targetEntity, WebOperationContext context) where TResource : DomainResourceBase, new()
        {
            var refer = Reference.CreateResourceReference((DomainResourceBase)DataTypeConverter.CreateResource<TResource>(targetEntity), context.IncomingRequest.UriTemplateMatch.BaseUri);
            refer.Display = (targetEntity as Entity)?.Names?.FirstOrDefault()?.ToString();
            return refer;

        }
        /// <summary>
        /// Creates the resource.
        /// </summary>
        /// <typeparam name="TResource">The type of the t resource.</typeparam>
        /// <param name="resource">The resource.</param>
        /// <returns>TResource.</returns>
        public static TResource CreateResource<TResource>(IVersionedEntity resource) where TResource : ResourceBase, new()
		{
			var retVal = new TResource();
			retVal.Id = resource.Key.ToString();
			retVal.VersionId = resource.VersionKey.ToString();

            // metadata
            retVal.Meta = new ResourceMetadata()
            {
                LastUpdated = (resource as IdentifiedData).ModifiedOn.DateTime,
                VersionId = resource.VersionKey?.ToString(),
                Profile = new Uri("http://openiz.org/fhir")
            };
            retVal.Meta.Tags = (resource as ITaggable)?.Tags.Select(o => DataTypeConverter.ToFhirTag(o)).ToList();
            // TODO: Configure this namespace / coding scheme
            retVal.Meta.Security = (resource as ISecurable)?.Policies.Where(o => o.GrantType == Core.Model.Security.PolicyGrantType.Grant).Select(o => new FhirCoding(new Uri("http://openiz.org/security/policy"), o.Policy.Oid)).ToList() ?? new List<FhirCoding>();
            retVal.Meta.Security.Add(new FhirCoding(new Uri("http://openiz.org/security/policy"), PermissionPolicyIdentifiers.ReadClinicalData));
            retVal.Extension = (resource as IExtendable)?.Extensions.Where(o=>o.ExtensionTypeKey != ExtensionTypeKeys.JpegPhotoExtension).Select(o => DataTypeConverter.ToExtension(o)).ToList();
            return retVal;
		}

        /// <summary>
        /// Creates a FHIR tag
        /// </summary>
        private static FhirCoding ToFhirTag(ITag o)
        {

            Uri tagUri = null;
            if (!Uri.TryCreate(o.TagKey, UriKind.Absolute, out tagUri))
                return new FhirCoding(new Uri("http://openiz.org/tags/fhir/" + o.TagKey), o.Value);
            else
                return new FhirCoding(tagUri, o.Value);

        }

        /// <summary>
        /// Converts an <see cref="Extension"/> instance to an <see cref="ActExtension"/> instance.
        /// </summary>
        /// <param name="fhirExtension">The FHIR extension.</param>
        /// <returns>Returns the converted act extension instance.</returns>
        /// <exception cref="System.ArgumentNullException">fhirExtension - Value cannot be null</exception>
        public static ActExtension ToActExtension(Extension fhirExtension)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR extension");

			var extension = new ActExtension();

			if (fhirExtension == null)
			{
				throw new ArgumentNullException(nameof(fhirExtension), "Value cannot be null");
			}

			var extensionTypeService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

			extension.ExtensionType = extensionTypeService.FindExtensionType(e => e.Name == fhirExtension.Url).FirstOrDefault();
            //extension.ExtensionValue = fhirExtension.Value;
            if (extension.ExtensionType.ExtensionHandler == typeof(DecimalExtensionHandler))
                extension.ExtensionValue = (fhirExtension.Value as FhirDecimal).Value;
            else if (extension.ExtensionType.ExtensionHandler == typeof(StringExtensionHandler))
                extension.ExtensionValue = (fhirExtension.Value as FhirString).Value;
            else if (extension.ExtensionType.ExtensionHandler == typeof(DateExtensionHandler))
                extension.ExtensionValue = (fhirExtension.Value as FhirDateTime).Value;
            else
                extension.ExtensionValueXml = (fhirExtension.Value as FhirBase64Binary).Value;

            // Now will 
            return extension;
		}

        /// <summary>
        /// Converts an <see cref="Extension"/> instance to an <see cref="ActExtension"/> instance.
        /// </summary>
        /// <param name="fhirExtension">The FHIR extension.</param>
        /// <returns>Returns the converted act extension instance.</returns>
        /// <exception cref="System.ArgumentNullException">fhirExtension - Value cannot be null</exception>
        public static EntityExtension ToEntityExtension(Extension fhirExtension)
        {
            traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR extension");

            var extension = new EntityExtension();

            if (fhirExtension == null)
            {
                throw new ArgumentNullException(nameof(fhirExtension), "Value cannot be null");
            }

            var extensionTypeService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

            extension.ExtensionType = extensionTypeService.FindExtensionType(e => e.Name == fhirExtension.Url).FirstOrDefault();
            //extension.ExtensionValue = fhirExtension.Value;
            if (extension.ExtensionType.ExtensionHandler == typeof(DecimalExtensionHandler))
                extension.ExtensionValue = (fhirExtension.Value as FhirDecimal).Value;
            else if (extension.ExtensionType.ExtensionHandler == typeof(StringExtensionHandler))
                extension.ExtensionValue = (fhirExtension.Value as FhirString).Value;
            else if (extension.ExtensionType.ExtensionHandler == typeof(DateExtensionHandler))
                extension.ExtensionValue = (fhirExtension.Value as FhirDateTime).Value;
            else
                extension.ExtensionValueXml = (fhirExtension.Value as FhirBase64Binary).Value;

            // Now will 
            return extension;
        }

        /// <summary>
        /// Converts a <see cref="FhirIdentifier"/> instance to an <see cref="ActIdentifier"/> instance.
        /// </summary>
        /// <param name="fhirIdentifier">The FHIR identifier.</param>
        /// <returns>Returns the converted act identifier instance.</returns>
        public static ActIdentifier ToActIdentifier(FhirIdentifier fhirIdentifier)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR identifier");

			if (fhirIdentifier == null)
			{
				return null;
			}

			ActIdentifier retVal;

			if (fhirIdentifier.System != null)
			{
				retVal = new ActIdentifier(DataTypeConverter.ToAssigningAuthority(fhirIdentifier.System), fhirIdentifier.Value.Value);
			}
			else
			{
                throw new InvalidOperationException("Identifier must carry a coding system");
			}

			// TODO: Fill in use
			return retVal;
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
            IOidRegistrarService d;

            traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping assigning authority");

			var oidRegistrar = ApplicationContext.Current.GetService<IOidRegistrarService>();
            var oid = oidRegistrar.FindData(fhirSystem.Value);

            return new AssigningAuthority(oid.Mnemonic, oid.Name, oid.Oid);
		}

		/// <summary>
		/// Converts a <see cref="ReferenceTerm"/> instance to a <see cref="FhirCoding"/> instance.
		/// </summary>
		/// <param name="referenceTerm">The reference term.</param>
		/// <returns>Returns a FHIR coding instance.</returns>
		public static FhirCoding ToCoding(ReferenceTerm referenceTerm)
		{
			if (referenceTerm == null)
			{
				return null;
			}

			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping reference term");

            var cs = referenceTerm.LoadProperty<CodeSystem>(nameof(ReferenceTerm.CodeSystem));
			return new FhirCoding(new Uri(cs.Url ?? String.Format("urn:oid:{0}", cs.Oid)), referenceTerm.Mnemonic);
		}

        /// <summary>
        /// Act Extension to Fhir Extension
        /// </summary>
        public static Extension ToExtension(IModelExtension ext)
        {

            var extensionTypeService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();
            var eType = extensionTypeService.GetExtensionType(ext.ExtensionTypeKey);

            var retVal = new Extension()
            {
                Url = eType.Name
            };

            if (ext.Value is Decimal)
                retVal.Value = new FhirDecimal((Decimal)ext.Value);
            else if (ext.Value is String)
                retVal.Value = new FhirString((String)ext.Value);
            else if (ext.Value is Boolean)
                retVal.Value = new FhirBoolean((bool)ext.Value);
            else
                retVal.Value = new FhirBase64Binary(ext.Data);
            return retVal;
        }
        
        /// <summary>
        /// Gets the concept via the codeable concept
        /// </summary>
        /// <param name="codeableConcept">The codeable concept.</param>
        /// <returns>Returns a concept.</returns>
        public static Concept ToConcept(FhirCodeableConcept codeableConcept)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping codeable concept");
			return codeableConcept?.Coding.Select(o => DataTypeConverter.ToConcept(o)).FirstOrDefault(o => o != null);
		}

		/// <summary>
		/// Convert from FHIR coding to concept
		/// </summary>
		/// <param name="coding">The coding.</param>
		/// <param name="defaultSystem">The default system.</param>
		/// <returns>Returns a concept which matches the given code and code system or null if no concept is found.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// Unable to locate service
		/// or
		/// Coding must have system attached
		/// </exception>
		public static Concept ToConcept(FhirCoding coding, FhirUri defaultSystem = null)
		{
			if (coding == null)
			{
				return null;
			}

			var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			var system = coding.System ?? defaultSystem;

			if (system == null)
			{
				throw new InvalidOperationException("Coding must have system attached");
			}

			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR coding");

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

			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR code");

			return ToConcept(new FhirCoding(new Uri(system), code.Value.ToString()));
		}

		/// <summary>
		/// Converts an <see cref="FhirAddress"/> instance to an <see cref="EntityAddress"/> instance.
		/// </summary>
		/// <param name="fhirAddress">The FHIR address.</param>
		/// <returns>Returns an entity address instance.</returns>
		public static EntityAddress ToEntityAddress(FhirAddress fhirAddress)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR address");

			var address = new EntityAddress
			{
				AddressUseKey = ToConcept(fhirAddress.Use, "http://hl7.org/fhir/address-use")?.Key
			};

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
		/// Convert a FhirIdentifier to an identifier
		/// </summary>
		/// <param name="fhirId">The fhir identifier.</param>
		/// <returns>Returns an entity identifier instance.</returns>
		public static EntityIdentifier ToEntityIdentifier(FhirIdentifier fhirId)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR identifier");

			if (fhirId == null)
			{
				return null;
			}

			EntityIdentifier retVal;

			if (fhirId.System != null)
			{
				retVal = new EntityIdentifier(DataTypeConverter.ToAssigningAuthority(fhirId.System), fhirId.Value.Value);
			}
			else
			{
                throw new InvalidOperationException("Identifier must carry a coding system");
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
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR human name");

			var name = new EntityName
			{
				NameUseKey = ToConcept(fhirHumanName.Use, "http://hl7.org/fhir/name-use")?.Key
			};

			name.Component.AddRange(fhirHumanName.Family.Select(f => new EntityNameComponent(NameComponentKeys.Family, f.Value)));
			name.Component.AddRange(fhirHumanName.Given.Select(g => new EntityNameComponent(NameComponentKeys.Given, g.Value)));
			name.Component.AddRange(fhirHumanName.Prefix.Select(p => new EntityNameComponent(NameComponentKeys.Prefix, p.Value)));
			name.Component.AddRange(fhirHumanName.Suffix.Select(s => new EntityNameComponent(NameComponentKeys.Suffix, s.Value)));

			return name;
		}

		/// <summary>
		/// Converts a <see cref="PatientContact"/> instance to an <see cref="EntityRelationship"/> instance.
		/// </summary>
		/// <param name="patientContact">The patient contact.</param>
		/// <returns>Returns the mapped entity relationship instance..</returns>
		public static EntityRelationship ToEntityRelationship(PatientContact patientContact)
		{
			return new EntityRelationship();
		}

		/// <summary>
		/// Converts a <see cref="FhirTelecom"/> instance to an <see cref="EntityTelecomAddress"/> instance.
		/// </summary>
		/// <param name="fhirTelecom">The telecom.</param>
		/// <returns>Returns an entity telecom address.</returns>
		public static EntityTelecomAddress ToEntityTelecomAddress(FhirTelecom fhirTelecom)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping FHIR telecom");

			return new EntityTelecomAddress
			{
				Value = fhirTelecom.Value.Value,
				AddressUseKey = ToConcept(fhirTelecom.Use, "http://hl7.org/fhir/contact-point-use")?.Key
			};
		}

		/// <summary>
		/// Converts an <see cref="EntityAddress"/> instance to a <see cref="FhirAddress"/> instance.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns>Returns a FHIR address.</returns>
		public static FhirAddress ToFhirAddress(EntityAddress address)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping entity address");

			if (address == null) return null;

			// Return value
			var retVal = new FhirAddress()
			{
				Use = DataTypeConverter.ToFhirCodeableConcept(address.AddressUse, "http://hl7.org/fhir/address-use")?.GetPrimaryCode()?.Code,
				Line = new List<FhirString>()
			};

			// Process components
			foreach (var com in address.LoadCollection<EntityAddressComponent>(nameof(EntityAddress.Component)))
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
						Url = FhirConstants.OpenIZProfile + "#address-" + com.LoadProperty<Concept>(nameof(EntityAddressComponent.ComponentType)).Mnemonic,
						Value = new FhirString(com.Value)
					});
				}
			}

			return retVal;
		}

		/// <summary>
		/// Converts a <see cref="Concept"/> instance to an <see cref="FhirCodeableConcept"/> instance.
		/// </summary>
		/// <param name="concept">The concept.</param>
		/// <returns>Returns a FHIR codeable concept.</returns>
		public static FhirCodeableConcept ToFhirCodeableConcept(Concept concept, String preferredCodeSystem = null)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping concept");

			if (concept == null)
			{
				return null;
			}

            if (String.IsNullOrEmpty(preferredCodeSystem))
                return new FhirCodeableConcept
                {
                    Coding = concept.LoadCollection<ConceptReferenceTerm>(nameof(Concept.ReferenceTerms)).Select(o => DataTypeConverter.ToCoding(o.LoadProperty<ReferenceTerm>(nameof(ConceptReferenceTerm.ReferenceTerm)))).ToList(),
                    Text = concept.LoadCollection<ConceptName>(nameof(Concept.ConceptNames)).FirstOrDefault()?.Name
                };
            else {
                var codeSystemService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
                var refTerm = codeSystemService.GetConceptReferenceTerm(concept.Key.Value, preferredCodeSystem);
                if (refTerm == null) // No code in the preferred system, ergo, we will instead use our own
                    return new FhirCodeableConcept
                    {
                        Coding = concept.LoadCollection<ConceptReferenceTerm>(nameof(Concept.ReferenceTerms)).Select(o => DataTypeConverter.ToCoding(o.LoadProperty<ReferenceTerm>(nameof(ConceptReferenceTerm.ReferenceTerm)))).ToList(),
                        Text = concept.LoadCollection<ConceptName>(nameof(Concept.ConceptNames)).FirstOrDefault()?.Name
                    };
                else
                    return new FhirCodeableConcept
                    {
                        Coding = new List<FhirCoding>() { ToCoding(refTerm) },
                        Text = concept.LoadCollection<ConceptName>(nameof(Concept.ConceptNames)).FirstOrDefault()?.Name
                    };
            }
        }

		/// <summary>
		/// Converts an <see cref="EntityName"/> instance to a <see cref="FhirHumanName"/> instance.
		/// </summary>
		/// <param name="entityName">Name of the entity.</param>
		/// <returns>Returns the mapped FHIR human name.</returns>
		public static FhirHumanName ToFhirHumanName(EntityName entityName)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping entity name");

			if (entityName == null)
			{
				return null;
			}

			// Return value
			var retVal = new FhirHumanName
			{
				Use = DataTypeConverter.ToFhirCodeableConcept(entityName.NameUse, "http://hl7.org/fhir/name-use")?.GetPrimaryCode()?.Code

            };

			// Process components
			foreach (var com in entityName.LoadCollection<EntityNameComponent>(nameof(EntityName.Component)))
			{
                if (string.IsNullOrEmpty(com.Value)) continue; 

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
		/// Converts a <see cref="IdentifierBase{TBoundModel}" /> instance to an <see cref="FhirIdentifier" /> instance.
		/// </summary>
		/// <typeparam name="TBoundModel">The type of the bound model.</typeparam>
		/// <param name="identifier">The identifier.</param>
		/// <returns>Returns the mapped FHIR identifier.</returns>
		public static FhirIdentifier ToFhirIdentifier<TBoundModel>(IdentifierBase<TBoundModel> identifier) where TBoundModel : VersionedEntityData<TBoundModel>, new()
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping entity identifier");

			if (identifier == null)
			{
				return null;
			}

            var imetaService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();
            var authority = imetaService.GetAssigningAuthority(identifier.AuthorityKey.Value);
			return new FhirIdentifier
			{
				System = new FhirUri(new Uri(authority?.Url ?? $"urn:oid:{authority?.Oid}")),
				Type = ToFhirCodeableConcept(identifier.LoadProperty<IdentifierType>(nameof(EntityIdentifier.IdentifierType))?.TypeConcept),
				Value = identifier.Value
			};
		}

		/// <summary>
		/// Converts an <see cref="EntityTelecomAddress"/> instance to <see cref="FhirTelecom"/> instance.
		/// </summary>
		/// <param name="telecomAddress">The telecom address.</param>
		/// <returns>Returns the mapped FHIR telecom.</returns>
		public static FhirTelecom ToFhirTelecom(EntityTelecomAddress telecomAddress)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, "Mapping entity telecom address");

			return new FhirTelecom()
			{
				Use = DataTypeConverter.ToFhirCodeableConcept(telecomAddress.AddressUse)?.GetPrimaryCode()?.Code,
				Value = telecomAddress.IETFValue
			};
		}

		/// <summary>
		/// Converts a <see cref="Communication"/> instance to a <see cref="PersonLanguageCommunication"/> instance.
		/// </summary>
		/// <param name="communication">The communication.</param>
		/// <returns>Returns the mapped person language communication instance.</returns>
		public static PersonLanguageCommunication ToPersonLanguageCommunication(Communication communication)
		{
			var languageCode = ToConcept(communication.Value);
			return new PersonLanguageCommunication(languageCode.Mnemonic, communication.Preferred.Value ?? false);
		}
	}
}