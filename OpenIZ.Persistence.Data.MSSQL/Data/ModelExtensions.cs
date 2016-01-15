using OpenIZ.Persistence.Data.MSSQL.Services.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Model extension methods
    /// </summary>
    public static class ModelExtensions
    {

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.DataTypes.Concept EnsureExists(this Core.Model.DataTypes.Concept me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
            {
                var retVal = new ConceptPersistenceService().Insert(me, principal, context);
                me.Key = retVal.Key;
                return retVal;
            }
            return me;
        }

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.DataTypes.PhoneticAlgorithm EnsureExists(this Core.Model.DataTypes.PhoneticAlgorithm me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
            {
                var retVal = new PhoneticAlgorithmPersistenceService().Insert(me, principal, context);
                me.Key = retVal.Key;
                return retVal;

            }
            return me;
        }

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.Security.SecurityRole EnsureExists(this Core.Model.Security.SecurityRole me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
            {
                var retVal = new SecurityRolePersistenceService().Insert(me, principal, context);
                me.Key = retVal.Key;
                return retVal;
            }
            return me;
        }

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.Security.SecurityUser EnsureExists(this Core.Model.Security.SecurityUser me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
            {
                var retVal = new SecurityUserPersistenceService().Insert(me, principal, context);
                me.Key = retVal.Key;
                return retVal;
            }
            return me;
        }

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.Security.SecurityPolicy EnsureExists(this Core.Model.Security.SecurityPolicy me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
                return new SecurityPolicyPersistenceService().Insert(me, principal, context);
            return me;
        }

        /// <summary>
        /// Create a new version of the concept
        /// </summary>
        public static Data.ConceptVersion NewVersion(this Data.ConceptVersion me, IPrincipal principal, ModelDataContext dataContext)
        {
            if (me.ConceptVersionId == Guid.Empty) // Not committed yet, no need to create
                return me;
            var newConceptVersion = new Data.ConceptVersion();
            newConceptVersion.CopyObjectData(me);
            newConceptVersion.VersionSequenceId = default(Decimal);
            newConceptVersion.ConceptVersionId = default(Guid);
            newConceptVersion.ReplacesVersionId = me.ConceptVersionId;
            newConceptVersion.CreatedBy = principal.GetUserGuid(dataContext);
            // Obsolete the old version 
            me.ObsoletedBy = principal.GetUserGuid(dataContext);
            me.ObsoletionTime = DateTime.Now;

            dataContext.ConceptVersions.InsertOnSubmit(newConceptVersion);

            return newConceptVersion;
        }

        /// <summary>
        /// Update property data if required
        /// </summary>
        public static void CopyObjectData(this Object toEntity, Object fromEntity)
        {
            if (toEntity == null)
                throw new ArgumentNullException(nameof(toEntity));
            else if (fromEntity == null)
                throw new ArgumentNullException(nameof(fromEntity));
            else if (fromEntity.GetType() != toEntity.GetType())
                throw new ArgumentException("Type mismatch", nameof(fromEntity));
            foreach (var pi in toEntity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.PropertyType.IsGenericTypeDefinition &&
                    typeof(Nullable<>) == pi.PropertyType.GetGenericTypeDefinition() ||
                    pi.PropertyType == typeof(DateTimeOffset) ||
                    pi.PropertyType == typeof(String) ||
                    pi.PropertyType.IsPrimitive)
                {
                    object newValue = pi.GetValue(fromEntity),
                        oldValue = pi.GetValue(toEntity);
                    if (newValue != oldValue)
                        pi.SetValue(toEntity, newValue);
                }
            }
        }

        /// <summary>
        /// Get the user identifier from the authorization context
        /// </summary>
        /// <param name="principal">The current authorization context</param>
        /// <returns>The UUID of the user which the authorization context subject represents</returns>
        public static Guid GetUserGuid(this IPrincipal principal, ModelDataContext dataContext)
        {

            var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == principal.Identity.Name && !o.ObsoletionTime.HasValue);
            if (user == null)
                throw new SecurityException("User in authorization context does not exist or is obsolete");

            return user.UserId;

        }

    }
}
