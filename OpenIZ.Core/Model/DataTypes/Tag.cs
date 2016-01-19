using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents the base class for tags
    /// </summary>
    [Serializable]
    [DataContract(Name = "Tag", Namespace = "http://openiz.org/model")]
    public abstract class Tag<TSourceType> : BoundRelationData<TSourceType> where TSourceType : IdentifiedData
    {

        /// <summary>
        /// Gets or sets the key of the tag
        /// </summary>
        [DataMember(Name ="key")]
        public String TagKey { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag
        /// </summary>
        [DataMember(Name = "value")]
        public String Value { get; set; }

    }

    /// <summary>
    /// Represents a tag associated with an entity
    /// </summary>
    [Serializable]
    [DataContract(Name = "EntityTag", Namespace = "http://openiz.org/model")]
    public class EntityTag : Tag<Entity>
    {

    }

    /*
    /// <summary>
    /// Represents a tag on an act
    /// </summary>
    public class ActTag : Tag<Act>
    {

    }
    */
}
