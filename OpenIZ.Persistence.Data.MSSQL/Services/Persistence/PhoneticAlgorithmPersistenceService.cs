using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Collections.Generic;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Phonetic algorithm 
    /// </summary>
    internal class PhoneticAlgorithmPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.PhoneticAlgorithm>
    {
        /// <summary>
        /// Convert from model to domain classes
        /// </summary>
        internal override object ConvertFromModel(Core.Model.DataTypes.PhoneticAlgorithm model)
        {
            return s_mapper.MapModelInstance<Core.Model.DataTypes.PhoneticAlgorithm, Data.PhoneticAlgorithm>(model);
        }

        /// <summary>
        /// Convert from data model to busines model
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm ConvertToModel(object data)
        {
            return s_mapper.MapDomainInstance<Data.PhoneticAlgorithm, Core.Model.DataTypes.PhoneticAlgorithm>(data as Data.PhoneticAlgorithm);
        }

        /// <summary>
        /// Get the specified phonetic algorithm from the database storage layer
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext)
        {
            var domainPhoneticAlgorithm = dataContext.PhoneticAlgorithms.FirstOrDefault(o => o.PhoneticAlgorithmId == containerId.Id);
            if (domainPhoneticAlgorithm == null)
                return null;
            else
                return this.ConvertToModel(domainPhoneticAlgorithm);
        }

        /// <summary>
        /// Insert a phonetic algorithm
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Insert(Core.Model.DataTypes.PhoneticAlgorithm storageData, IPrincipal principal, ModelDataContext dataContext)
        {

            var domainPhoneticAlgorithm = this.ConvertFromModel(storageData) as Data.PhoneticAlgorithm;
            dataContext.PhoneticAlgorithms.InsertOnSubmit(domainPhoneticAlgorithm);
            dataContext.SubmitChanges();
            return this.ConvertToModel(domainPhoneticAlgorithm);

        }

        /// <summary>
        /// Phonetic algorithms can only be deleted
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Obsolete(Core.Model.DataTypes.PhoneticAlgorithm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            var existingDomainAlgorithm = dataContext.PhoneticAlgorithms.FirstOrDefault(o => o.PhoneticAlgorithmId == storageData.Key);
            if (existingDomainAlgorithm == null)
                throw new KeyNotFoundException();
            dataContext.PhoneticAlgorithms.DeleteOnSubmit(existingDomainAlgorithm);
            dataContext.SubmitChanges();

            existingDomainAlgorithm.PhoneticAlgorithmId = Guid.Empty;

            return this.ConvertToModel(existingDomainAlgorithm);
        }

        /// <summary>
        /// Query the phonetic algorithm types
        /// </summary>
        internal override IQueryable<Core.Model.DataTypes.PhoneticAlgorithm> Query(Expression<Func<Core.Model.DataTypes.PhoneticAlgorithm, bool>> query, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainQuery = s_mapper.MapModelExpression<Core.Model.DataTypes.PhoneticAlgorithm, Data.PhoneticAlgorithm>(query);
            return dataContext.PhoneticAlgorithms.Where(domainQuery).Select(o => this.ConvertToModel(o));
        }

        /// <summary>
        /// Update the phonetic algorithm
        /// </summary>
        internal override Core.Model.DataTypes.PhoneticAlgorithm Update(Core.Model.DataTypes.PhoneticAlgorithm storageData, IPrincipal principal, ModelDataContext dataContext)
        {
            var domainPhoneticAlgorithm = dataContext.PhoneticAlgorithms.FirstOrDefault(o => o.PhoneticAlgorithmId == storageData.Key);
            domainPhoneticAlgorithm.CopyObjectData(this.ConvertFromModel(storageData));
            // Update
            dataContext.SubmitChanges();

            return this.ConvertToModel(domainPhoneticAlgorithm);
        }
    }
}