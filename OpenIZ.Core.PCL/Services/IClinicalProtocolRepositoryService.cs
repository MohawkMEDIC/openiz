using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a service that can do clinical protocols
    /// </summary>
    public interface IClinicalProtocolRepositoryService
    {

        /// <summary>
        /// Find protocols in the repository service
        /// </summary>
        IEnumerable<Core.Model.Acts.Protocol> FindProtocol(Expression<Func<Core.Model.Acts.Protocol, bool>> predicate, int offset, int? count, out int totalResults);

        /// <summary>
        /// Find protocols in the repository service
        /// </summary>
        Core.Model.Acts.Protocol InsertProtocol(Core.Model.Acts.Protocol data);

    }
}
