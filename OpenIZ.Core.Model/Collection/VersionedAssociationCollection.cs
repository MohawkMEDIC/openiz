using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq.Expressions;

namespace OpenIZ.Core.Model.Collection
{
    /// <summary>
    /// Represents a collection of entities
    /// </summary>
    [XmlType(Namespace = "http://openiz.org/model")]
    [JsonObject]
    public class VersionedAssociationCollection<TEntity> : SimpleAssociationCollection<TEntity> where TEntity : IdentifiedData, IVersionedAssociation, new()
    {


        /// <summary>
        /// Entity collection
        /// </summary>
        public VersionedAssociationCollection() : base()
        {
        }

        /// <summary>
        /// Creates the specified entity collection in the specified context
        /// </summary>
        public VersionedAssociationCollection(IVersionedEntity context) : base(context)
        {
        }

        /// <summary>
        /// Perform a delay load on the entire collection
        /// </summary>
        protected override IList<TEntity> Ensure()
        {
            if (this.m_context == null) return this.m_sourceData;

            // Load if needed
            if (this.m_sourceData == null)
            {
                if (this.m_context.Key.HasValue && (this.m_load.HasValue && this.m_load.Value || !this.m_load.HasValue && this.m_context.IsDelayLoadEnabled))
                {
                    this.m_sourceData = new List<TEntity>(EntitySource.Current.Provider.GetRelations<TEntity>(this.m_context.Key, (this.m_context as IVersionedEntity)?.VersionSequence));
                }
                else
                    this.m_sourceData = new List<TEntity>();
            }

            return this.m_sourceData;
        }

        /// <summary>
        /// Creates a new simple association from the specified list
        /// </summary>
        public static implicit operator VersionedAssociationCollection<TEntity>(List<TEntity> entity)
        {
            return new VersionedAssociationCollection<TEntity>() { m_sourceData = entity };
        }
    }
}
