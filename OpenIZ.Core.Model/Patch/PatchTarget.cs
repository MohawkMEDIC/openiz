using Newtonsoft.Json;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Patch
{
    /// <summary>
    /// Represents a target of a patch
    /// </summary>
    [XmlType(nameof(PatchTarget), Namespace = "http://openiz.org/model")]
    [JsonObject(nameof(PatchTarget))]
    public class PatchTarget
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public PatchTarget()
        {

        }

        /// <summary>
        /// Construct a new patch target
        /// </summary>
        public PatchTarget(IdentifiedData existing)
        {
            this.Type = existing.GetType();
            this.Key = existing.Key;
            this.VersionKey = (existing as IVersionedEntity).VersionKey;
            this.Tag = existing.Tag;
        }

        /// <summary>
        /// Identifies the target type
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public string TypeXml { get { return this.Type.AssemblyQualifiedName; } set { this.Type = Type.GetType(value); } }

        /// <summary>
        /// Represents the type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlElement("id"), JsonProperty("id")]
        public Guid? Key { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlElement("version"), JsonProperty("version")]
        public Guid? VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the tag of the item
        /// </summary>
        [XmlElement("tag"), JsonProperty("etag")]
        public string Tag { get; set; }
    }
}