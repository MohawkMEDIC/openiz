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
 * Date: 2017-4-13
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using System.Xml.Serialization;
using System.Reflection;
using OpenIZ.Core.Services;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.Collection;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Resource handler base
    /// </summary>
    public abstract class ResourceHandlerBase<TResource> : IResourceHandler where TResource : IdentifiedData
    {

        /// <summary>
        /// IRepository service
        /// </summary>
        private IRepositoryService<TResource> m_repository = null;

        /// <summary>
        /// Constructs the resource handler base
        /// </summary>
        public ResourceHandlerBase()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IRepositoryService<TResource>>();
        }

        /// <summary>
        /// Gets the resource name
        /// </summary>
        public virtual string ResourceName
        {
            get
            {
                return typeof(TResource).GetCustomAttribute<XmlRootAttribute>().ElementName;
            }
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(TResource);
            }
        }

        /// <summary>
        /// Create a resource
        /// </summary>
        public virtual IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var bundle = data as Bundle;

            bundle?.Reconstitute();

            var processData = bundle?.Entry ?? data;

            if (processData is Bundle)
                throw new InvalidOperationException("Bundle must have an entry point");

            if (processData is TResource)
            {
                var resourceData = processData as TResource;
                return updateIfExists ? this.m_repository.Save(resourceData) : this.m_repository.Insert(resourceData);
            }

            throw new ArgumentException(nameof(data), "Invalid data type");
        }

        /// <summary>
        /// Read clinical data
        /// </summary>
        public virtual IdentifiedData Get(Guid id, Guid versionId)
        {
            return this.m_repository.Get(id, versionId);
        }

        /// <summary>
        /// Obsolete data
        /// </summary>
        public virtual IdentifiedData Obsolete(Guid key)
        {
            return this.m_repository.Obsolete(key);
        }

        /// <summary>
        /// Perform a query 
        /// </summary>
        public virtual IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Perform the actual query
        /// </summary>
        public virtual IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            var queryExpression = QueryExpressionParser.BuildLinqExpression<TResource>(queryParameters, null, false);
            List<String> query = null;

            if (queryParameters.TryGetValue("_queryId", out query) && this.m_repository is IPersistableQueryRepositoryService)
            {
                Guid queryId = Guid.Parse(query[0]);
                List<String> lean = null;
                if (queryParameters.TryGetValue("_lean", out lean) && lean[0] == "true" && this.m_repository is IFastQueryRepositoryService)
                    return (this.m_repository as IFastQueryRepositoryService).FindFast<TResource>(queryExpression, offset, count, out totalCount, queryId);
                else
                    return (this.m_repository as IPersistableQueryRepositoryService).Find<TResource>(queryExpression, offset, count, out totalCount, queryId);
            }
            else
            {
                List<String> lean = null;
                if (queryParameters.TryGetValue("_lean", out lean) && lean[0] == "true" && this.m_repository is IFastQueryRepositoryService)
                    return (this.m_repository as IFastQueryRepositoryService).FindFast<TResource>(queryExpression, offset, count, out totalCount, Guid.Empty);
                else
                    return this.m_repository.Find(queryExpression, offset, count, out totalCount);
            }
        }

        /// <summary>
        /// Perform an update
        /// </summary>
        public virtual IdentifiedData Update(IdentifiedData data)
        {
            Bundle bundleData = data as Bundle;
            bundleData?.Reconstitute();
            var processData = bundleData?.Entry ?? data;

            if (processData is Bundle)
                throw new InvalidOperationException(string.Format("Bundle must have entry of type {0}", typeof(TResource).Name));
            else if (processData is TResource)
            {
                var entityData = data as TResource;

                return this.m_repository.Save(entityData);
            }
            else
                throw new ArgumentException("Invalid persistence type");
        }
    }
}
