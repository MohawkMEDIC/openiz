/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
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
        public TObject Get<TObject>(Guid key) where TObject : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if(persistenceService != null)
                return persistenceService.Get(new Identifier<Guid>(key), null, true);
            return default(TObject);
        }

        /// <summary>
        /// Get the specified version
        /// </summary>
        public TObject Get<TObject>(Guid key, Guid versionKey) where TObject : IdentifiedData, IVersionedEntity
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if (persistenceService != null)
                return persistenceService.Get(new Identifier<Guid>(key, versionKey), null, true);
            return default(TObject);
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            if(persistenceService != null)
                return persistenceService.Query(query, null);
            return new List<TObject>();
        }
    }
}
