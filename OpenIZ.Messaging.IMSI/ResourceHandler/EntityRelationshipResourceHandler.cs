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
 * Date: 2017-4-23
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using System.IO;
using System.Data.Linq;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Entity relationship resource handler
    /// </summary>
    /// <remarks>This is a special resource handler which only supports updates/inserts. It actually just creates a new version
    /// of an entity on the server so the changes propagate down</remarks>
    public class EntityRelationshipResourceHandler : ResourceHandlerBase<EntityRelationship>
    {

        /// <summary>
        /// Get the name of the resource
        /// </summary>
        public override string ResourceName
        {
            get
            {
                return "EntityRelationship";
            }
        }

        /// <summary>
        /// Massage query parameters
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public override IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            if (queryParameters.ContainsKey("modifiedOn"))
                queryParameters.Remove("modifiedOn");
            return base.Query(queryParameters, offset, count, out totalCount);
        }
    }
}
