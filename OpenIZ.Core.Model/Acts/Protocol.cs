using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents the model of a protocol
    /// </summary>
    [XmlType(nameof(Protocol), Namespace = "http://openiz.org/model"), JsonObject(nameof(Protocol))]
    public class Protocol : BaseEntityData
    {

        /// <summary>
        /// Gets or sets the name of the protocol
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the handler for this protocol (which can load the definition
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type HandlerClass
        {
            get
            {
                return System.Type.GetType(this.HandlerClassName);
            }
            set
            {
                this.HandlerClassName = value.AssemblyQualifiedName;
            }
        }

        /// <summary>
        /// Gets or sets the handler class AQN
        /// </summary>
        [XmlElement("handlerClass"), JsonProperty("handlerClass")]
        public String HandlerClassName { get; set; }

        /// <summary>
        /// Contains instructions which the handler class can understand
        /// </summary>
        [XmlElement("definition"), JsonProperty("definition")]
        public byte[] Definition { get; set; }

    }
}