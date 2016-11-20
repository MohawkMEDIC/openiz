using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Patch
{
    /// <summary>
    /// Represents a series of patch instructions 
    /// </summary>
    [XmlType(nameof(Patch), Namespace = "http://openiz.org/model")]
    [XmlRoot(nameof(Patch), Namespace = "http://openiz.org/model")]
    [JsonObject(nameof(Patch))]
    public class Patch : BaseEntityData
    {
        /// <summary>
        /// Patch
        /// </summary>
        public Patch()
        {
            this.Operation = new List<PatchOperation>();
        }

        /// <summary>
        /// Application version
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IdentifiedData AppliesTo { get; set; }

        /// <summary>
        /// A list of patch operations to be applied to the object
        /// </summary>
        [XmlElement("change"), JsonProperty("change")]
        public List<PatchOperation> Operation { get; set; }

        /// <summary>
        /// To string representation
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var itm in this.Operation)
                builder.AppendFormat("{0}\r\n", itm);
            return builder.ToString();
        }
    }
}
