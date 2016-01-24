using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents the base class for tags
    /// </summary>
    
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class Tag<TSourceType> : Association<TSourceType> where TSourceType : IdentifiedData
    {

        /// <summary>
        /// Gets or sets the key of the tag
        /// </summary>
        [XmlElement("key"), JsonProperty("key")]
        public String TagKey { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }

    }

    /// <summary>
    /// Represents a tag associated with an entity
    /// </summary>
    
    [XmlType("EntityTag",  Namespace = "http://openiz.org/model"), JsonObject("EntityTag")]
    public class EntityTag : Tag<Entity>
    {

    }


    /// <summary>
    /// Represents a tag on an act
    /// </summary>
    
    [XmlType("ActTag",  Namespace = "http://openiz.org/model"), JsonObject("ActTag")]
    public class ActTag : Tag<Act>
    {

    }

}
