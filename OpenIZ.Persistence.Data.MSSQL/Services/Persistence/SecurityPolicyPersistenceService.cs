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
 * Date: 2016-1-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Security;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Security policy persistence service
    /// </summary>
    public class SecurityPolicyPersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityPolicy>
    {
        /// <summary>
        /// Convert the specified item from model to domain
        /// </summary>
        internal override object ConvertFromModel(SecurityPolicy model)
        {
            return s_mapper.MapModelInstance<SecurityPolicy, Policy>(model);
        }

        /// <summary>
        /// Convert to a model
        /// </summary>
        internal override SecurityPolicy ConvertToModel(object data)
        {
            return this.ConvertToModel(data as Data.Policy);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal Core.Model.Security.SecurityPolicy ConvertToModel(Data.Policy data)
        {
            return this.ConvertItem(data);
        }

        /// <summary>
        /// Get the specified policy
        /// </summary>
        internal override SecurityPolicy Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));

            var dataPolicy = dataContext.Policies.SingleOrDefault(p => p.PolicyId == containerId.Id);
            if (dataPolicy != null)
                return this.ConvertToModel(dataPolicy);
            else
                return null;
        }

        /// <summary>
        /// Insert the security policy
        /// </summary>
        internal override SecurityPolicy Insert(SecurityPolicy storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            else if (storageData.Key != default(Guid))
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // Insert
            var dataPolicy = this.ConvertFromModel(storageData) as Policy;
            dataPolicy.CreatedByEntity = principal.GetUser(dataContext);
            dataContext.Policies.InsertOnSubmit(dataPolicy);
            dataContext.SubmitChanges();

            return this.ConvertToModel(dataPolicy);
        }

        /// <summary>
        /// Obsoletes a data policy object
        /// </summary>
        internal override SecurityPolicy Obsolete(SecurityPolicy storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            else if (storageData.Key == default(Guid))
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            // Update 
            var dataPolicy = dataContext.Policies.SingleOrDefault(p => p.PolicyId == storageData.Key);
            dataPolicy.ObsoletedByEntity = principal.GetUser(dataContext);
            dataPolicy.ObsoletionTime = DateTimeOffset.Now;
            dataContext.SubmitChanges();

            return this.ConvertToModel(dataPolicy);

        }

        /// <summary>
        /// Query for 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="principal"></param>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        internal override IQueryable<SecurityPolicy> Query(Expression<Func<SecurityPolicy, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var expression = s_mapper.MapModelExpression<SecurityPolicy, Policy>(query);
            return dataContext.Policies.Where(expression).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update the policy
        /// </summary>
        internal override SecurityPolicy Update(SecurityPolicy storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            throw new NotSupportedException("Policies cannot be updated");
        }
    }
    
}
