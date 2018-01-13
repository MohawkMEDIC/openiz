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
 * User: fyfej
 * Date: 2017-10-30
 */
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local act repository service
    /// </summary>
    public partial class LocalActRepositoryService : IRepositoryService<Procedure>
    {
        /// <summary>
        /// Find the specified procedure
        /// </summary>
        public IEnumerable<Procedure> Find(Expression<Func<Procedure, bool>> query)
        {
            int tr = 0;
            return this.Find<Procedure>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified procedures with query controls
        /// </summary>
        public IEnumerable<Procedure> Find(Expression<Func<Procedure, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert procedure data
        /// </summary>
        public Procedure Insert(Procedure data)
        {
            return this.Insert<Procedure>(data);
        }

        /// <summary>
        /// Update or insert data
        /// </summary>
        public Procedure Save(Procedure data)
        {
            return this.Save<Procedure>(data);
        }

        /// <summary>
        /// Get the specified object
        /// </summary>
        Procedure IRepositoryService<Procedure>.Get(Guid key)
        {
            return this.Get<Procedure>(key, Guid.Empty);
        }

        /// <summary>
        /// Get the specified object by key and version
        /// </summary>
        Procedure IRepositoryService<Procedure>.Get(Guid key, Guid versionKey)
        {
            return this.Get<Procedure>(key, versionKey);
        }

        /// <summary>
        /// Obsolete the specified object
        /// </summary>
        Procedure IRepositoryService<Procedure>.Obsolete(Guid key)
        {
            return this.Obsolete<Procedure>(key);
        }
    }
}
