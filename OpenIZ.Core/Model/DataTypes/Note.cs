using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Generic note class
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "http://openiz.org", Name = "Note")]
    public abstract class Note<TBoundModel> : VersionBoundRelationData<TBoundModel> where TBoundModel : VersionedEntityData<TBoundModel>
    {

        // Author id
        private Guid m_authorKey;
        // Author entity
        [NonSerialized]
        private Entity m_author;

        /// <summary>
        /// Gets or sets the note text
        /// </summary>
        [DataMember(Name = "text")]
        public String Text { get; set; }

        /// <summary>
        /// Gets or sets the author key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "authorRef")]
        public Guid AuthorKey
        {
            get { return this.m_authorKey; }
            set
            {
                this.m_authorKey = value;
                this.m_author = null;
            }
        }

        /// <summary>
        /// Gets or sets the author entity
        /// </summary>
        [IgnoreDataMember]
        [DelayLoad]
        public Entity Author
        {
            get
            {
                return base.DelayLoad(this.m_authorKey, this.m_author);
            }
            set
            {
                this.m_author = value;
                if (value == null)
                    this.m_authorKey = Guid.Empty;
                else
                    this.m_authorKey = value.Key;
            }
        }
        
    }

    /// <summary>
    /// Represents a note attached to an entity
    /// </summary>
    [DataContract(Name = "EntityName", Namespace = "http://openiz.org/model")]
    public class EntityNote : Note<Entity>
    {

    }
}
