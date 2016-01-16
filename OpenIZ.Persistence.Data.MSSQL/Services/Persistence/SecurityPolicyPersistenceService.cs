﻿using System;
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
            return s_mapper.MapDomainInstance<Policy, SecurityPolicy>(data as Policy);
        }

        /// <summary>
        /// Get the specified policy
        /// </summary>
        internal override SecurityPolicy Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));

            var dataPolicy = dataContext.Policies.FirstOrDefault(p => p.PolicyId == containerId.Id);
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
            dataPolicy.CreatedBy = principal.GetUserGuid(dataContext);
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
            var dataPolicy = dataContext.Policies.FirstOrDefault(p => p.PolicyId == storageData.Key);
            dataPolicy.ObsoletedBy = principal.GetUserGuid(dataContext);
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