using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Code system persistence service
    /// </summary>
    public class CodeSystemPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.CodeSystem>
    {
        /// <summary>
        /// Convert from model
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.CodeSystem model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.CodeSystem, Data.CodeSystem>(model);
        }

        /// <summary>
        /// Convert to model
        /// </summary>
        internal override Core.Model.DataTypes.CodeSystem ConvertToModel(object data)
        {
            return s_mapper.MapDomainInstance<Data.CodeSystem, Core.Model.DataTypes.CodeSystem>(data as Data.CodeSystem);
        }

        /// <summary>
        /// Get the code system
        /// </summary>
        internal override Core.Model.DataTypes.CodeSystem Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            return this.ConvertToModel(dataContext.CodeSystems.SingleOrDefault(o => o.CodeSystemId == containerId.Id));
        }

        /// <summary>
        /// Insert the specified storage data into the data persistence store
        /// </summary>
        internal override Core.Model.DataTypes.CodeSystem Insert(Core.Model.DataTypes.CodeSystem storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var domainCodeSystem = this.ConvertFromModel(storageData) as Data.CodeSystem;
            domainCodeSystem.CreatedByEntity = principal.GetUser(dataContext);
            dataContext.CodeSystems.InsertOnSubmit(domainCodeSystem);

            dataContext.SubmitChanges();

            storageData.Key = domainCodeSystem.CodeSystemId;
            return storageData;
        }

        /// <summary>
        /// Obsolete the code system
        /// </summary>
        internal override Core.Model.DataTypes.CodeSystem Obsolete(Core.Model.DataTypes.CodeSystem storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var domainCodeSystem = dataContext.CodeSystems.SingleOrDefault(o => o.CodeSystemId == storageData.Key);
            domainCodeSystem.ObsoletedByEntity = principal.GetUser(dataContext);
            domainCodeSystem.ObsoletionTime = DateTimeOffset.Now;

            dataContext.SubmitChanges();

            storageData.Key = Guid.Empty;
            domainCodeSystem.ObsoletionTime = domainCodeSystem.ObsoletionTime;

            return storageData;
        }

        /// <summary>
        /// Query the data model
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.CodeSystem> Query(Expression<Func<Core.Model.DataTypes.CodeSystem, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.CodeSystem, Data.CodeSystem>(query);
            return dataContext.CodeSystems.Where(domainQuery).Select(cs => this.ConvertToModel(cs));
        }

        /// <summary>
        /// Update the code system
        /// </summary>
        internal override Core.Model.DataTypes.CodeSystem Update(Core.Model.DataTypes.CodeSystem storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            throw new NotImplementedException();
        }
    }
}
