/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-14
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.EntityLoader
{
    /// <summary>
    /// Represents an entity source which is a basic source provider
    /// used by the model for delay loading
    /// </summary>
    public class PersistenceServiceEntitySource : IEntitySourceProvider
    {
        /// <summary>
        /// Get the persistence service source
        /// </summary>
        public TObject Get<TObject>(Guid? key) where TObject : IdentifiedData, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if(persistenceService != null  && key.HasValue)
                return persistenceService.Get(new Identifier<Guid>(key.Value), null, true);
            return default(TObject);
        }

        /// <summary>
        /// Get the specified version
        /// </summary>
        public TObject Get<TObject>(Guid? key, Guid? versionKey) where TObject : IdentifiedData, IVersionedEntity, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if (persistenceService != null && key.HasValue && versionKey.HasValue)
                return persistenceService.Get(new Identifier<Guid>(key.Value, versionKey.Value), null, true);
            else if(persistenceService != null && key.HasValue)
                return persistenceService.Get(new Identifier<Guid>(key.Value), null, true);

            return default(TObject);
        }

        /// <summary>
        /// Get versioned relationships
        /// </summary>
        public IEnumerable<TObject> GetRelations<TObject>(Guid? sourceKey, decimal? sourceVersionSequence) where TObject : IdentifiedData, IVersionedAssociation,new()
        {
            return this.Query<TObject>(o => sourceKey == o.SourceEntityKey && sourceVersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || sourceVersionSequence < o.ObsoleteVersionSequenceId)).ToList();
        }

        /// <summary>
        /// Get versioned relationships
        /// </summary>
        public IEnumerable<TObject> GetRelations<TObject>(Guid? sourceKey) where TObject : IdentifiedData, ISimpleAssociation, new()
        {
            return this.Query<TObject>(o => sourceKey == o.SourceEntityKey).ToList();
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if(persistenceService != null)
                return persistenceService.Query(query, null);
            return new List<TObject>();
        }
        
    }
}
