using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.EntityLoader
{
    /// <summary>
    /// Represents an entity source which is a basic source provider
    /// used by the model for delay loading
    /// </summary>
    public class PersistenceServiceEntitySource : IEntitySourceProvider
    {
        /// <summary>
        /// Get the persistence service source
        /// </summary>
        public TObject Get<TObject>(Guid key)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            return persistenceService.Get(new Identifier<Guid>(key), null, true);
        }

        /// <summary>
        /// Get the specified version
        /// </summary>
        public TObject Get<TObject>(Guid key, Guid versionKey)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            return persistenceService.Get(new Identifier<Guid>(key, versionKey), null, true);
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TObject>>();
            return persistenceService.Query(query, null);
        }
    }
}
