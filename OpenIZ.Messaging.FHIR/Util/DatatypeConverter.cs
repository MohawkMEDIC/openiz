using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
