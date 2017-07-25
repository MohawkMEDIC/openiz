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
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenIZ.OrmLite;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persister which is a act relationship 
    /// </summary>
    public class ActRelationshipPersistenceService : IdentifiedPersistenceService<ActRelationship, DbActRelationship>, IAdoAssociativePersistenceService
    {

        /// <summary>
        /// Get from source
        /// </summary>
        public IEnumerable GetFromSource(DataContext context, Guid id, decimal? versionSequenceId, IPrincipal principal)
        {
            int tr = 0;
            return this.QueryInternal(context, base.BuildSourceQuery<ActRelationship>(id, versionSequenceId), Guid.Empty, 0, null, out tr, principal, false).ToList();
        }

        /// <summary>
        /// Insert the relationship
        /// </summary>
        public override ActRelationship InsertInternal(DataContext context, ActRelationship data, IPrincipal principal)
        {
            // Ensure we haven't already persisted this
            if (data.TargetAct != null) data.TargetAct = data.TargetAct.EnsureExists(context, principal) as Act;
            data.TargetActKey = data.TargetAct?.Key ?? data.TargetActKey;
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            byte[] target = data.TargetActKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.RelationshipTypeKey.Value.ToByteArray();

            //SqlStatement sql = new SqlStatement<DbActRelationship>().SelectFrom()
            //    .Where<DbActRelationship>(o => o.SourceUuid == source)
            //    .Limit(1).Build();

            //IEnumerable<DbActRelationship> dbrelationships = context.TryGetData($"EX:{sql.ToString()}") as IEnumerable<DbActRelationship>;
            //if (dbrelationships == null)
            //{
            //    dbrelationships = context.Connection.Query<DbActRelationship>(sql.SQL, sql.Arguments.ToArray()).ToList();
            //    context.AddData($"EX{sql.ToString()}", dbrelationships);
            //}
            //var existing = dbrelationships.FirstOrDefault(
            //        o => o.RelationshipTypeUuid == typeKey &&
            //        o.TargetUuid == target);

            //if (existing == null)
            //{
            return base.InsertInternal(context, data, principal);
            //    (dbrelationships as List<DbActRelationship>).Add(new DbActRelationship()
            //    {
            //        Uuid = retVal.Key.Value.ToByteArray(),
            //        RelationshipTypeUuid = typeKey,
            //        SourceUuid = source,
            //        TargetUuid = target
            //    });
            //    return retVal;
            //}
            //else
            //{
            //    data.Key = new Guid(existing.Uuid);
            //    return data;
            //}
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        public override ActRelationship UpdateInternal(DataContext context, ActRelationship data, IPrincipal principal)
        {
            data.TargetActKey = data.TargetAct?.Key ?? data.TargetActKey;
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Comparer for entity relationships
        /// </summary>
        internal class Comparer : IEqualityComparer<ActRelationship>
        {
            /// <summary>
            /// Determine equality between the two relationships
            /// </summary>
            public bool Equals(ActRelationship x, ActRelationship y)
            {
                return x.SourceEntityKey == y.SourceEntityKey &&
                    x.TargetActKey == y.TargetActKey &&
                    (x.RelationshipTypeKey == y.RelationshipTypeKey ||  x.RelationshipType?.Mnemonic == y.RelationshipType?.Mnemonic);
            }

            /// <summary>
            /// Get hash code
            /// </summary>
            public int GetHashCode(ActRelationship obj)
            {
                int result = obj.SourceEntityKey.GetHashCode();
                result = 37 * result + obj.RelationshipTypeKey.GetHashCode();
                result = 37 * result + obj.TargetActKey.GetHashCode();
                result = 37 * result + (obj.RelationshipType?.Mnemonic.GetHashCode() ?? 0);
                return result;
            }
        }
    }
}
