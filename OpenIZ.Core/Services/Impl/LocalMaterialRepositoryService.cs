using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local material persistence service
    /// </summary>
    public class LocalMaterialRepositoryService : IMaterialRepositoryService
    {

        /// <summary>
        /// Finds the specified material
        /// </summary>
        public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            return persistence.Query(expression, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Find the specified material
        /// </summary>
        public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression, int offset, int count, out int totalCount)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            return persistence.Query(expression, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Gets the specified identified material
        /// </summary>
        public Material GetMaterial(Guid id, Guid versionId)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            return persistence.Get<Guid>(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Inserts the specified material
        /// </summary>
        public Material InsertMaterial(Material material)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            return persistence.Insert(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Obsoletes the speciied material
        /// </summary>
        public Material ObsoleteMaterial(Guid key)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            return persistence.Obsolete(new Material() { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Save the specified material
        /// </summary>
        public Material SaveMaterial(Material material)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>();
            if (persistence == null)
                throw new InvalidOperationException("Missing persistence service");
            try
            {
                return persistence.Update(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            catch
            {
                return persistence.Insert(material, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
        }
    }
}
