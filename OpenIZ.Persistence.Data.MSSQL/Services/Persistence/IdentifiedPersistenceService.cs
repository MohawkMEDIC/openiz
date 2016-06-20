using System;
using OpenIZ.Core.Model.Security;
using System.Linq.Expressions;
using System.Linq;
using OpenIZ.Core.Model;
using System.Text;
using System.Collections.Generic;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;
using System.Data.Linq;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
	/// <summary>
	/// Generic persistence service which can persist between two simple types.
	/// </summary>
	public abstract class IdentifiedPersistenceService<TModel, TDomain> : SqlServerBasePersistenceService<TModel> 
		where TModel : IdentifiedData, new()
		where TDomain : class, IDbIdentified, new()
	{

        #region implemented abstract members of LocalDataPersistenceService
        /// <summary>
        /// Maps the data to a model instance
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="dataInstance">Data instance.</param>
        public override TModel ToModelInstance (object dataInstance, ModelDataContext context, IPrincipal principal)
		{
			var retVal = m_mapper.MapDomainInstance<TDomain, TModel> (dataInstance as TDomain);
            return retVal;
		}


		/// <summary>
		/// Froms the model instance.
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="modelInstance">Model instance.</param>
		/// <param name="context">Context.</param>
		public override object FromModelInstance (TModel modelInstance, ModelDataContext context, IPrincipal princpal)
		{
			return m_mapper.MapModelInstance<TModel, TDomain> (modelInstance);

		}

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		public override TModel Insert (ModelDataContext context, TModel data, IPrincipal principal)
		{
			var domainObject = this.FromModelInstance (data, context, principal) as TDomain;

            if (domainObject.Id == Guid.Empty)
                data.Key = domainObject.Id = Guid.NewGuid();

            context.GetTable<TDomain>().InsertOnSubmit (domainObject);
            context.SubmitChanges();

            return data;
		}

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Update (ModelDataContext context, TModel data, IPrincipal principal)
		{
            // Sanity 
            if(data.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            // Map and copy
            var newDomainObject = this.FromModelInstance (data, context, principal) as TDomain;
            var oldDomainObject = context.GetTable<TDomain>().Single(ExpressionRewriter.Rewrite<TDomain>(o => o.Id == newDomainObject.Id));
            if (oldDomainObject == null)
                throw new KeyNotFoundException(data.Key.ToString());

            oldDomainObject.CopyObjectData(newDomainObject);
            context.SubmitChanges();
			return data;
		}

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Obsolete (ModelDataContext context, TModel data, IPrincipal principal)
		{
            if (data.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            var domainObject = context.GetTable<TDomain>().FirstOrDefault(ExpressionRewriter.Rewrite<TDomain>(o => o.Id == data.Key));

            if (domainObject == null)
                throw new KeyNotFoundException(data.Key.ToString());

			context.GetTable<TDomain>().DeleteOnSubmit (domainObject);
			return data;
		}

        /// <summary>
        /// Performs the actual query
        /// </summary>
        public override IQueryable<TModel> Query (ModelDataContext context, Expression<Func<TModel, bool>> query, int offset, int count, IPrincipal principal, out int totalResults)
		{
			var domainQuery = m_mapper.MapModelExpression<TModel, TDomain> (query);
			var retVal = context.GetTable<TDomain> ().Where (domainQuery);
            // Total count
            totalResults = retVal.Count();
            // Skip
            retVal = retVal.Skip(offset);
            if (count > 0)
				retVal = retVal.Take (count);
            
			return retVal.Select(o=>this.ToModelInstance(o, context, principal));
		}

        /// <summary>
        /// Update associated items
        /// </summary>
        protected virtual void UpdateAssociatedItems<TAssociation, TDomainAssociation>(List<TAssociation> storage, TModel source, ModelDataContext context, IPrincipal principal)
            where TAssociation : IdentifiedData, ISimpleAssociation, new()
            where TDomainAssociation : class, IDbAssociation, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAssociation>>() as SqlServerBasePersistenceService<TAssociation>;
            if (persistenceService == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, "Missing persister for type {0}", typeof(TAssociation).Name);
                return;
            }
            // Ensure the source key is set
            foreach (var itm in storage)
                if (itm.SourceEntityKey == Guid.Empty)
                    itm.SourceEntityKey = source.Key;

            // Get existing
            var existing = context.GetTable<TDomainAssociation>().Where(ExpressionRewriter.Rewrite<TDomainAssociation>(o => o.AssociatedItemKey == source.Key)).ToList().Select(o=>m_mapper.MapDomainInstance<TDomainAssociation, TAssociation>(o).GetLocked() as TAssociation);
            // Remove old
            var obsoleteRecords = existing.Where(o => !storage.Exists(ecn => ecn.Key == o.Key));
            foreach (var del in obsoleteRecords)
                persistenceService.Obsolete(context, del, principal);

            // Update those that need it
            var updateRecords = storage.Where(o => existing.Any(ecn => ecn.Key == o.Key && o.Key != Guid.Empty && o != ecn));
            foreach (var upd in updateRecords)
                persistenceService.Update(context, upd, principal);

            // Insert those that do not exist
            var insertRecords = storage.Where(o => !existing.Any(ecn => ecn.Key == o.Key));
            foreach (var ins in insertRecords)
                persistenceService.Insert(context, ins, principal);


        }
        #endregion
    }
}

