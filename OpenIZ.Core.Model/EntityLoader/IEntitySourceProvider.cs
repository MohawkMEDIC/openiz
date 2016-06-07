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
 * Date: 2016-2-1
 */
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
    /// Delay load provider
    /// </summary>
    public interface IEntitySourceProvider
    {
        
        /// <summary>
        /// Get the specified object
        /// </summary>
        TObject Get<TObject>(Guid key) where TObject : IdentifiedData;

        /// <summary>
        /// Get the specified object
        /// </summary>
        TObject Get<TObject>(Guid key, Guid versionKey) where TObject : IdentifiedData, IVersionedEntity;

        /// <summary>
        /// Query the specified data from the delay load provider
        /// </summary>
        IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData;
    }
}
