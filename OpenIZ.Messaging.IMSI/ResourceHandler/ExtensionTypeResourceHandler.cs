using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Services;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Represents a handler for extension types
    /// </summary>
    public class ExtensionTypeResourceHandler : IResourceHandler
    {
        /// <summary>
        /// Resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "ExtensionType";
            }
        }

        /// <summary>
        /// Gets the type of the handler
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(ExtensionType);
            }
        }

        /// <summary>
        /// Readonly
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get the extension
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            var repository = ApplicationContext.Current.GetService<IDataPersistenceService<ExtensionType>>();
            return repository?.Get<Guid>(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Read only
        /// </summary>
        public IdentifiedData Obsolete(Guid key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Query the specified types
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Query with offset and count
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            var repository = ApplicationContext.Current.GetService<IDataPersistenceService<ExtensionType>>();
            var filter = QueryExpressionParser.BuildLinqExpression<ExtensionType>(queryParameters);
            List<String> queryId = null;
            if (repository is IStoredQueryDataPersistenceService<ExtensionType> && queryParameters.TryGetValue("_queryId", out queryId))
                return (repository as IStoredQueryDataPersistenceService<ExtensionType>).Query(filter, Guid.Parse(queryId[0]), offset, count, AuthenticationContext.Current.Principal, out totalCount);
            else
                return repository.Query(filter, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Readonly
        /// </summary>
        public IdentifiedData Update(IdentifiedData data)
        {
            throw new NotSupportedException();
        }
    }
}
