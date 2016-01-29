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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Data;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Security.Claims;
using System.Collections.Specialized;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// A resource handler for a concept
    /// </summary>
    public class ConceptResourceHandler : IResourceHandler
    {
        /// <summary>
        /// Gets the resource name
        /// </summary>
        public string ResourceName {  get { return nameof(Concept); } }

        /// <summary>
        /// Gets the model type of the handler
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(Concept);
            }
        }

        /// <summary>
        /// Create the specified object in the database
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified instance
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            return persistenceService.Get(new Identifier<Guid>(id, versionId), null, true); // TODO: AUTH
        }

        public IdentifiedData Obsolete(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData Update(IdentifiedData data)
        {
            throw new NotImplementedException();
        }
    }
}
