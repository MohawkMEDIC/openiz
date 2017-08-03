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
 * Date: 2017-3-31
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.OrmLite;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Entity relationship persistence service
    /// </summary>
    public class EntityRelationshipPersistenceService : IdentifiedPersistenceService<EntityRelationship, DbEntityRelationship>, IAdoAssociativePersistenceService
    {

        /// <summary>
        /// Get relationships from source
        /// </summary>
        public IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal)
        {
            int tr = 0;
            return this.QueryInternal(context, base.BuildSourceQuery<EntityRelationship>(id, versionSequenceId), Guid.Empty, 0, null, out tr, principal, false).ToList();

        }

        /// <summary>
        /// Represents as a model instance
        /// </summary>
        public override EntityRelationship ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            if (dataInstance == null) return null;

            var entPart = dataInstance as DbEntityRelationship;
            return new EntityRelationship()
            {
                EffectiveVersionSequenceId = entPart.EffectiveVersionSequenceId,
                ObsoleteVersionSequenceId = entPart.ObsoleteVersionSequenceId,
                HolderKey = entPart.SourceKey,
                TargetEntityKey = entPart.TargetKey,
                RelationshipType = context.LoadState == Core.Model.LoadState.FullLoad ?  AdoPersistenceService.GetPersister(typeof(Concept)).Get(entPart.RelationshipTypeKey) as Concept : null,
                RelationshipTypeKey = entPart.RelationshipTypeKey,
                Quantity = entPart.Quantity,
                LoadState = context.LoadState,
                Key = entPart.Key,
                SourceEntityKey = entPart.SourceKey
            };
        }

        /// <summary>
        /// Insert the relationship
        /// </summary>
        public override EntityRelationship InsertInternal(DataContext context, EntityRelationship data, IPrincipal principal)
        {
            
            // Ensure we haven't already persisted this
            if(data.TargetEntity != null && !data.InversionIndicator) data.TargetEntity = data.TargetEntity.EnsureExists(context, principal) as Entity;
            data.TargetEntityKey = data.TargetEntity?.Key ?? data.TargetEntityKey;
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;
            
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        public override EntityRelationship UpdateInternal(DataContext context, EntityRelationship data, IPrincipal principal)
        {
            // Ensure we haven't already persisted this
            data.TargetEntityKey = data.TargetEntity?.Key ?? data.TargetEntityKey;
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;
            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Comparer for entity relationships
        /// </summary>
        internal class Comparer : IEqualityComparer<EntityRelationship>
        {
            /// <summary>
            /// Determine equality between the two relationships
            /// </summary>
            public bool Equals(EntityRelationship x, EntityRelationship y)
            {
                return x.SourceEntityKey == y.SourceEntityKey &&
                    x.TargetEntityKey == y.TargetEntityKey &&
                    (x.RelationshipTypeKey == y.RelationshipTypeKey ||  x.RelationshipType?.Mnemonic == y.RelationshipType?.Mnemonic);
            }

            /// <summary>
            /// Get hash code
            /// </summary>
            public int GetHashCode(EntityRelationship obj)
            {
                int result = obj.SourceEntityKey.GetHashCode();
                result = 37 * result + obj.RelationshipTypeKey.GetHashCode();
                result = 37 * result + obj.TargetEntityKey.GetHashCode();
                result = 37 * result + (obj.RelationshipType?.Mnemonic.GetHashCode() ?? 0);
                return result;
            }
        }
    }
}
