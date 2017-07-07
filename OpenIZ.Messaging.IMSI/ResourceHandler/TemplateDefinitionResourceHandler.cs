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
 * Date: 2017-4-11
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Template definition resource handler
    /// </summary>
    public class TemplateDefinitionResourceHandler : IResourceHandler
    {
        /// <summary>
        /// Get the resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "TemplateDefinition";
            }
        }

        /// <summary>
        /// Get the type
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(TemplateDefinition);
            }
        }

        /// <summary>
        /// Create not supported
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get not supported
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsolete not supported
        /// </summary>
        public IdentifiedData Obsolete(Guid key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Query the template definitions
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Query the properties
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            var metaService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();
            if (metaService == null)
                throw new InvalidOperationException("Cannot locate metadata repository");
            else
                return metaService.FindTemplateDefinitions(QueryExpressionParser.BuildLinqExpression<TemplateDefinition>(queryParameters), offset, count, out totalCount);
        }

        public IdentifiedData Update(IdentifiedData data)
        {
            throw new NotImplementedException();
        }
    }
}
