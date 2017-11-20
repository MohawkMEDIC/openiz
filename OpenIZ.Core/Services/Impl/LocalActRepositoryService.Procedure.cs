using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local act repository service
    /// </summary>
    public partial class LocalActRepositoryService : IRepositoryService<Procedure>
    {
        /// <summary>
        /// Find the specified procedure
        /// </summary>
        public IEnumerable<Procedure> Find(Expression<Func<Procedure, bool>> query)
        {
            int tr = 0;
            return this.Find<Procedure>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified procedures with query controls
        /// </summary>
        public IEnumerable<Procedure> Find(Expression<Func<Procedure, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert procedure data
        /// </summary>
        public Procedure Insert(Procedure data)
        {
            return this.Insert<Procedure>(data);
        }

        /// <summary>
        /// Update or insert data
        /// </summary>
        public Procedure Save(Procedure data)
        {
            return this.Save<Procedure>(data);
        }

        /// <summary>
        /// Get the specified object
        /// </summary>
        Procedure IRepositoryService<Procedure>.Get(Guid key)
        {
            return this.Get<Procedure>(key, Guid.Empty);
        }

        /// <summary>
        /// Get the specified object by key and version
        /// </summary>
        Procedure IRepositoryService<Procedure>.Get(Guid key, Guid versionKey)
        {
            return this.Get<Procedure>(key, versionKey);
        }

        /// <summary>
        /// Obsolete the specified object
        /// </summary>
        Procedure IRepositoryService<Procedure>.Obsolete(Guid key)
        {
            return this.Obsolete<Procedure>(key);
        }
    }
}
