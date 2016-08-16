using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Services.Impl;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Resource handler which can deal with metadata resources
    /// </summary>
    public class AssigningAuthorityResourceHandler : IResourceHandler
    {

        // repository
        private IMetadataRepositoryService m_repository;

        public AssigningAuthorityResourceHandler()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IMetadataRepositoryService>();
        }

        /// <summary>
        /// The name of the resource 
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "AssigningAuthority";
            }
        }

        /// <summary>
        /// Gets the type this resource handler exposes
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(AssigningAuthority);
            }
        }

        /// <summary>
        /// Create an assigning authority - not supported
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get the assigning authority
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            return this.m_repository.GetAssigningAuthority(id);
        }

        /// <summary>
        /// Obsoletes an assigning authority
        /// </summary>
        public IdentifiedData Obsolete(Guid key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Queries for assigning authority
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            return this.m_repository.FindAssigningAuthority(QueryExpressionParser.BuildLinqExpression<AssigningAuthority>(queryParameters));

        }

        /// <summary>
        /// Query for the specified AA
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            return this.m_repository.FindAssigningAuthority(QueryExpressionParser.BuildLinqExpression<AssigningAuthority>(queryParameters), offset, count, out totalCount);
        }

        /// <summary>
        /// Update assigning authority
        /// </summary>
        public IdentifiedData Update(IdentifiedData data)
        {
            throw new NotSupportedException();
        }
    }
}
