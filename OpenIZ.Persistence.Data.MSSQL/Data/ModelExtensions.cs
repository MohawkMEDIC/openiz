/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using OpenIZ.Persistence.Data.MSSQL.Services.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Linq;
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
        /// Ensures a model has been persisted
        /// </summary>
        public static TModel EnsureExists<TModel>(this TModel me, IPrincipal principal, ModelDataContext dataContext) where TModel : IdentifiedData, new()
        {
            var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>() as BaseDataPersistenceService<TModel>;
            if (dataService == null)
                throw new InvalidOperationException(String.Format("Cannot locate SQL storage provider for {0}", typeof(TModel).FullName));
            if(me.Key == Guid.Empty)
            {
                var retVal = dataService.Insert(me, principal, dataContext);
                me.Key = retVal.Key; // prevents future loading
                return retVal;
            }
            return me;
        }

        /// <summary>
        /// Create a new version of the concept
        /// </summary>
        public static Data.ConceptVersion NewVersion(this Data.ConceptVersion me, IPrincipal principal, ModelDataContext dataContext)
        {
            if (me.Concept.IsSystemConcept)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.UpdatedReadonlyObject);

            var newConceptVersion = new Data.ConceptVersion();
            var user = principal.GetUser(dataContext);
            newConceptVersion.CopyObjectData(me);
            newConceptVersion.VersionSequenceId = default(Decimal);
            newConceptVersion.ConceptVersionId = default(Guid);
            newConceptVersion.Concept = me.Concept;
            newConceptVersion.ReplacesVersionId = me.ConceptVersionId;
            newConceptVersion.CreatedByEntity = user;
            // Obsolete the old version 
            me.ObsoletedByEntity = user;
            me.ObsoletionTime = DateTime.Now;

            dataContext.ConceptVersions.InsertOnSubmit(newConceptVersion);

            return newConceptVersion;
        }

        /// <summary>
        /// Update property data if required
        /// </summary>
        public static void CopyObjectData<TObject>(this TObject toEntity, TObject fromEntity)
        {
            if (toEntity == null)
                throw new ArgumentNullException(nameof(toEntity));
            else if (fromEntity == null)
                throw new ArgumentNullException(nameof(fromEntity));
            else if (fromEntity.GetType() != toEntity.GetType())
                throw new ArgumentException("Type mismatch", nameof(fromEntity));
            foreach (var pi in toEntity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                
                // Skip delay load 
                if (pi.GetCustomAttribute<DelayLoadAttribute>() == null &&
                    pi.GetSetMethod() != null)
                {
                    if (pi.PropertyType.IsGenericType &&
                        pi.PropertyType.GetGenericTypeDefinition() == typeof(EntitySet<>) ||
                        pi.PropertyType.Namespace.StartsWith("OpenIZ.Persistence"))
                        continue;


                    object newValue = pi.GetValue(fromEntity),
                        oldValue = pi.GetValue(toEntity);

                    // HACK: New value wrap for nullables
                    if (newValue is Guid? && newValue != null)
                        newValue = (newValue as Guid?).Value;

                    if (newValue != null &&
                        !newValue.Equals(oldValue) == true && 
                        (pi.PropertyType.IsValueType && !newValue.Equals(Activator.CreateInstance(newValue.GetType())) || !pi.PropertyType.IsValueType))
                        pi.SetValue(toEntity, newValue);
                }
            }
        }

        /// <summary>
        /// Get the user identifier from the authorization context
        /// </summary>
        /// <param name="principal">The current authorization context</param>
        /// <param name="dataContext">The context under which the get operation should be completed</param>
        /// <returns>The UUID of the user which the authorization context subject represents</returns>
        public static Data.SecurityUser GetUser(this IPrincipal principal, ModelDataContext dataContext)
        {

            if (principal == null)
                return null;

            var user = dataContext.SecurityUsers.Single(o => o.UserName == principal.Identity.Name && !o.ObsoletionTime.HasValue);
            if (user == null)
                throw new SecurityException("User in authorization context does not exist or is obsolete");

            return user;

        }

    }
}
