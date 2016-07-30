using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using System.Security.Permissions;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// User entty
    /// </summary>
    public class UserEntityResourceHandler : IResourceHandler
    {

        // Repository
        private ISecurityRepositoryService m_repository;

        /// <summary>
        /// Place resource handler subscription
        /// </summary>
        public UserEntityResourceHandler()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
        }

        /// <summary>
        /// Gets the name of the resource that this handler 
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "UserEntity";
            }
        }

        /// <summary>
        /// Gets the .NET type of the resource handler
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(UserEntity);
            }
        }

        /// <summary>
        /// Creates the speciified user entity
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Bundle bundleData = data as Bundle;
            bundleData?.Reconstitute();
            var processData = bundleData?.Entry ?? data;

            if (processData is Bundle) // Client submitted a bundle
                throw new InvalidOperationException("Bundle must have an entry point");
            else if (processData is UserEntity)
            {
                var userEntity = processData as UserEntity;
                if (updateIfExists)
                    return this.m_repository.CreateUserEntity(userEntity);
                else
                    return this.m_repository.CreateUserEntity(userEntity);
            }
            else
                throw new ArgumentException(nameof(data), "Invalid data type");
        }

        /// <summary>
        /// Gets the specified user entity
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            return this.m_repository.GetUserEntity(id, versionId);
        }

        /// <summary>
        /// Obsolete the entity
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedAdministration)]
        public IdentifiedData Obsolete(Guid key)
        {
            return this.m_repository.ObsoleteUserEntity(key);
        }

        /// <summary>
        /// Queries the specified user entity
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            return this.m_repository.FindUserEntity(QueryExpressionParser.BuildLinqExpression<UserEntity>(queryParameters));

        }

        /// <summary>
        /// Query the specified user entity with restrictions
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            return this.m_repository.FindUserEntity(QueryExpressionParser.BuildLinqExpression<UserEntity>(queryParameters), offset, count, out totalCount);
        }

        /// <summary>
        /// Updates the specified user entity
        /// </summary>
        public IdentifiedData Update(IdentifiedData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var bundleData = data as Bundle;
            bundleData?.Reconstitute();
            var saveData = bundleData?.Entry ?? data;

            if (saveData is Bundle)
                throw new InvalidOperationException("Bundle must have an entry");
            else if (saveData is UserEntity)
                return this.m_repository.SaveUserEntity(saveData as UserEntity);
            else
                throw new ArgumentException(nameof(data), "Invalid storage type");
        }
    }
}
