using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Data.Linq;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{

    /// <summary>
    /// Association persistence
    /// </summary>
    public abstract class AssociationPersistenceService<TModel, TAssociated, TDomain> : BaseDataPersistenceService<TModel> where TModel : Core.Model.Association<TAssociated>, new()
        where TAssociated : Core.Model.VersionedEntityData<TAssociated>
        where TDomain : class, new()
    {

        /// <summary>
        /// Gets the specified domain table
        /// </summary>
        protected abstract Table<TDomain> GetDataTable(ModelDataContext context);

        /// <summary>
        /// Convert from model into domain class
        /// </summary>
        internal override object ConvertFromModel(TModel model)
        {
            return s_mapper.MapModelInstance<TModel, TDomain>(model);
        }

        /// <summary>
        /// Convert from domain to model
        /// </summary>
        internal override TModel ConvertToModel(object data)
        {
            return this.ConvertItem(data as TDomain);
        }

        /// <summary>
        /// Gets the specified data
        /// </summary>
        internal override TModel Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));
            else if (containerId.Id == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.AssociatedEntityWithoutSourceKey);

            ParameterExpression lambdaParm = Expression.Parameter(typeof(TDomain), "o");
            PropertyInfo pkey = typeof(TDomain).GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(o => o.GetCustomAttribute<ColumnAttribute>().IsPrimaryKey == true);

            Expression<Func<TDomain, bool>> query = Expression.Lambda<Func<TDomain, bool>>(
                Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(lambdaParm, pkey), Expression.Constant(containerId.Id))
                );
            var tRetVal = this.GetDataTable(dataContext).Single(query);

            if (tRetVal != null)
                return this.ConvertToModel(tRetVal);
            else
                return null;
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        internal override IQueryable<TModel> Query(Expression<Func<TModel, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<TModel, TDomain>(query);
            return this.GetDataTable(dataContext).Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

    }

    /// <summary>
    /// Address persistence service
    /// </summary>
    public abstract class VersionedAssociationPersistenceService<TModel, TAssociated, TDomain> : AssociationPersistenceService<TModel, TAssociated, TDomain> where TModel : Core.Model.VersionedAssociation<TAssociated>, new()
        where TAssociated : Core.Model.VersionedEntityData<TAssociated>
        where TDomain : class, new()
    {


        /// <summary>
        /// Insert the data into the database
        /// </summary>
        internal override TModel Insert(TModel storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Insert(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Obsolete the data
        /// </summary>
        /// <param name="storageData"></param>
        /// <param name="principal"></param>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        internal override TModel Obsolete(TModel storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Obsolete(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Update object
        /// </summary>
        internal override TModel Update(TModel storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            return this.Update(storageData, principal, dataContext, true);
        }

        /// <summary>
        /// Perform the actual insertion of the data
        /// </summary>
        internal abstract TModel Insert(TModel storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion);
        /// <summary>
        /// Perform the actual update of the data
        /// </summary>
        internal abstract TModel Update(TModel storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion);
        /// <summary>
        /// Perform the actual obsoletion of the data
        /// </summary>
        internal abstract TModel Obsolete(TModel storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion);

    }
}
