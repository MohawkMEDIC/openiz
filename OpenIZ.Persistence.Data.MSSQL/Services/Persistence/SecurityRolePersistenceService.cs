﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Security;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Diagnostics;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Security role persistence service
    /// </summary>
    public class SecurityRolePersistenceService : BaseDataPersistenceService<Core.Model.Security.SecurityRole>
    {
        /// <summary>
        /// Perform a get operation
        /// </summary>
        internal override Core.Model.Security.SecurityRole Get(Identifier<Guid> containerId, ClaimsPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            var dataRole = dataContext.SecurityRoles.FirstOrDefault(o => o.RoleId == containerId.Id);

            if (dataRole != null)
                return this.ConvertToModel(dataRole);
            else
                return null;
        }

        /// <summary>
        /// Insert the security role
        /// </summary>
        internal override Core.Model.Security.SecurityRole Insert(Core.Model.Security.SecurityRole storageData, ClaimsPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key != default(Guid)) // Trying to insert an already inserted user?
                throw new SqlFormalConstraintException("Insert must be for an unidentified object");
            else if (principal == null)
                throw new SqlFormalConstraintException("Insert must have an authorization context");

            if (storageData.DelayLoad) // We want a frozen asset
                storageData = storageData.AsFrozen() as Core.Model.Security.SecurityRole;

            var dataRole = this.ConvertFromModel(storageData) as Data.SecurityRole;
            dataRole.CreatedBy = base.GetUserFromprincipal(principal, dataContext);
            dataContext.SecurityRoles.InsertOnSubmit(dataRole);

            if (storageData.Users != null)
                dataContext.SecurityUserRoles.InsertAllOnSubmit(storageData.Users.Select(u => new SecurityUserRole() { UserId = u.EnsureExists(principal, dataContext).Key, SecurityRole = dataRole }));

            // Persist data to the db
            dataContext.SubmitChanges();

            return this.ConvertToModel(dataRole);
        }

        internal override Core.Model.Security.SecurityRole Obsolete(Core.Model.Security.SecurityRole storageData, ClaimsPrincipal principal, ModelDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a query
        /// </summary>
        internal override IQueryable<Core.Model.Security.SecurityRole> Query(Expression<Func<Core.Model.Security.SecurityRole, bool>> query, ClaimsPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.Security.SecurityRole, Data.SecurityRole>(query);
            Trace.TraceInformation("MSSQL: {0}: QUERY Tx {1} > {2}", this.GetType().Name, query, domainQuery);

            return dataContext.SecurityRoles.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        internal override Core.Model.Security.SecurityRole Update(Core.Model.Security.SecurityRole storageData, ClaimsPrincipal principal, ModelDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert a data item from model
        /// </summary>
        internal override object ConvertFromModel(Core.Model.Security.SecurityRole model)
        {
            return s_mapper.MapModelInstance<Core.Model.Security.SecurityRole, Data.SecurityRole>(model);

        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal override Core.Model.Security.SecurityRole ConvertToModel(object data)
        {
            return s_mapper.MapDomainInstance<Data.SecurityRole, Core.Model.Security.SecurityRole>(data as Data.SecurityRole);
        }

    }
}
