using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Model extensions
    /// </summary>
    public static class ModelExtensions
    {

        /// <summary>
        /// Get the identifier
        /// </summary>
        public static Identifier<Guid> Id(this IIdentifiedEntity me)
        {
            // TODO: My AA
            return new Identifier<Guid>(me.Key);
        }

        /// <summary>
        /// Get the identifier
        /// </summary>
        public static Identifier<Guid> Id(this IVersionedEntity me)
        {
            return new Identifier<Guid>(me.Key, me.VersionKey);
        }

        /// <summary>
        /// Validates that this object has a target entity
        /// </summary>
        public static IEnumerable<IResultDetail> Validate<TSourceType>(this VersionedAssociation<TSourceType> me) where TSourceType : VersionedEntityData<TSourceType>
        {
            var validResults = new List<IResultDetail>();
            if (me.SourceEntityKey == Guid.Empty)
                validResults.Add(new RequiredElementMissingResultDetail(ResultDetailType.Error, String.Format("({0}).{1} required", me.GetType().Name, "SourceEntityKey"), null));
            return validResults;
        }

        /// <summary>
        /// Validate the state of this object
        /// </summary>
        public static IEnumerable<IResultDetail> Validate(this IdentifiedData me)
        {
            return new List<IResultDetail>();
        }

        /// <summary>
        /// Convert this AA to OID Data for configuration purposes
        /// </summary>
        public static OidData ToOidData(this AssigningAuthority me)
        {
            return new OidData()
            {
                Name = me.Name,
                Description = me.Description,
                Oid = me.Oid,
                Ref = new Uri(String.IsNullOrEmpty(me.Url) ? String.Format("urn:uuid:{0}", me.Oid) : me.Url),
                Attributes = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>()
                {
                    new System.Collections.Generic.KeyValuePair<string, string>("HL7CX4", me.DomainName)
                    //new System.Collections.Generic.KeyValuePair<string, string>("AssigningDevFacility", this.AssigningDevice.DeviceEvidence)
                }
            };
        }
    }
}
