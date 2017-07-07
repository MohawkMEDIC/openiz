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
 * Date: 2017-4-22
 */
using OpenIZ.OrmLite.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Warehouse.ADO.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    public class AdoConfiguration
    {

        /// <summary>
        /// Readonly connection string
        /// </summary>
        public String ReadWriteConnectionString { get; set; }

        /// <summary>
        /// Readonly connection string
        /// </summary>
        public String ReadonlyConnectionString { get; set; }

        /// <summary>
        /// Trace SQL enabled
        /// </summary>
        public bool TraceSql { get; set; }

        /// <summary>
        /// Provider type
        /// </summary>
        public IDbProvider Provider { get; set; }
    }
}
