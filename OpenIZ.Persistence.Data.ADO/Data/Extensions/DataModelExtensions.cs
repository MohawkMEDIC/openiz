/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
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
 * User: justi
 * Date: 2017-1-21
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Services;
using OpenIZ.OrmLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OpenIZ.Persistence.Data.ADO.Services.Persistence;

namespace OpenIZ.Persistence.Data.ADO.Data
{


    /// <summary>
    /// Model extension methods
    /// </summary>
    public static class DataModelExtensions
    {
        
        // Trace source
        private static TraceSource s_traceSource = new TraceSource(AdoDataConstants.TraceSourceName);

        // Field cache
        private static Dictionary<Type, FieldInfo[]> s_fieldCache = new Dictionary<Type, FieldInfo[]>();

        // Lock object
        private static Object s_lockObject = new object();

        // Constructor info
        private static Dictionary<Type, ConstructorInfo> m_constructors = new Dictionary<Type, ConstructorInfo>();

        // Classification properties for autoload
        private static Dictionary<Type, PropertyInfo> s_classificationProperties = new Dictionary<Type, PropertyInfo>();

        // Runtime properties
        private static Dictionary<String, IEnumerable<PropertyInfo>> s_runtimeProperties = new Dictionary<string, IEnumerable<PropertyInfo>>();

        // Cache ids
        private static Dictionary<String, Guid> m_classIdCache = new Dictionary<string, Guid>();

        private static Dictionary<String, Guid> m_userIdCache = new Dictionary<string, Guid>();

        /// <summary>
        /// Get fields
        /// </summary>
        private static FieldInfo[] GetFields(Type type)
        {

            FieldInfo[] retVal = null;
            if (!s_fieldCache.TryGetValue(type, out retVal))
            {
                retVal = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(o => !typeof(MulticastDelegate).IsAssignableFrom(o.FieldType)).ToArray();
                lock (s_lockObject)
                    if (!s_fieldCache.ContainsKey(type))
                        s_fieldCache.Add(type, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Try get by classifier
        /// </summary>
        public static IIdentifiedEntity TryGetExisting(this IIdentifiedEntity me, DataContext context, IPrincipal principal, bool forceDatabase = false)
        {

            // Is there a classifier?
            var idpInstance = AdoPersistenceService.GetPersister(me.GetType()) as IAdoPersistenceService;
            var cacheService = new AdoPersistenceCache(context);

            IIdentifiedEntity existing = null;

            // Forcing from database load from
            if (forceDatabase && me.Key.HasValue)
                // HACK: This should really hit the database instead of just clearing the cache
                ApplicationContext.Current.GetService<IDataCachingService>()?.Remove(me.Key.Value);
                //var tableType = AdoPersistenceService.GetMapper().MapModelType(me.GetType());
                //if (me.GetType() != tableType)
                //{
                //    var tableMap = TableMapping.Get(tableType);
                //    var dbExisting = context.FirstOrDefault(tableType, context.CreateSqlStatement().SelectFrom(tableType).Where($"{tableMap.Columns.FirstOrDefault(o=>o.IsPrimaryKey).Name}=?", me.Key.Value));
                //    if (dbExisting != null)
                //        existing = idpInstance.ToModelInstance(dbExisting, context, principal) as IIdentifiedEntity;
                //}
            if (me.Key != Guid.Empty && me.Key != null)
                existing = idpInstance.Get(context, me.Key.Value, principal) as IIdentifiedEntity;

            var classAtt = me.GetType().GetCustomAttribute<KeyLookupAttribute>();
            if (classAtt != null && existing == null)
            {

                // Get the domain type
                var dataType = AdoPersistenceService.GetMapper().MapModelType(me.GetType());
                var tableMap = TableMapping.Get(dataType);

                // Get the classifier attribute value
                var classProperty = me.GetType().GetProperty(classAtt.UniqueProperty);
                object classifierValue = classProperty.GetValue(me); // Get the classifier

                // Is the classifier a UUID'd item?
                if (classifierValue is IIdentifiedEntity)
                {
                    classifierValue = (classifierValue as IIdentifiedEntity).Key.Value;
                    classProperty = me.GetType().GetProperty(classProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty ?? classProperty.Name);
                }

                // Column 
                var column = tableMap.GetColumn(AdoPersistenceService.GetMapper().MapModelProperty(me.GetType(), dataType, classProperty));
                // Now we want to query 
                SqlStatement stmt = context.CreateSqlStatement().SelectFrom(dataType)
                    .Where($"{column.Name} = ?", classifierValue);

                Guid objIdCache = Guid.Empty;
                IDbIdentified dataObject = null;

                // We've seen this before

                String classKey = $"{dataType}.{classifierValue}";
                if (m_classIdCache.TryGetValue(classKey, out objIdCache))
                    existing = cacheService?.GetCacheItem(objIdCache) as IdentifiedData;
                if (existing == null)
                {
                    dataObject = context.FirstOrDefault(dataType, stmt) as IDbIdentified;
                    if (dataObject != null)
                    {
                        lock (m_classIdCache)
                            if (!m_classIdCache.ContainsKey(classKey))
                                m_classIdCache.Add(classKey, dataObject.Key);
                        var existCache = cacheService?.GetCacheItem((dataObject as IDbIdentified).Key);
                        if (existCache != null)
                            existing = existCache as IdentifiedData;
                        else
                            existing = idpInstance.ToModelInstance(dataObject, context, principal) as IIdentifiedEntity;
                    }
                }
            }

            return existing;

        }


        /// <summary>
        /// Updates a keyed delay load field if needed
        /// </summary>
        public static void UpdateParentKeys(this IIdentifiedEntity instance, PropertyInfo field)
        {
            var delayLoadProperty = field.GetCustomAttribute<SerializationReferenceAttribute>();
            if (delayLoadProperty == null || String.IsNullOrEmpty(delayLoadProperty.RedirectProperty))
                return;
            var value = field.GetValue(instance) as IIdentifiedEntity;
            if (value == null)
                return;
            // Get the delay load key property!
            var keyField = instance.GetType().GetRuntimeProperty(delayLoadProperty.RedirectProperty);
            keyField.SetValue(instance, value.Key);
        }

        /// <summary>
        /// Ensures a model has been persisted
        /// </summary>
        public static IIdentifiedEntity EnsureExists(this IIdentifiedEntity me, DataContext context, IPrincipal principal)
        {

            if (me == null) return null;

            // Me
            var vMe = me as IVersionedEntity;
            String dkey = String.Format("{0}.{1}", me.GetType().FullName, me.Key);

            IIdentifiedEntity existing = me.TryGetExisting(context, principal);
            var idpInstance = AdoPersistenceService.GetPersister(me.GetType());

            // Don't touch the child just return reference
            if (!AdoPersistenceService.GetConfiguration().AutoInsertChildren)
            {
                if (existing != null)
                {
                    if (me.Key != existing.Key ||
                        vMe?.VersionKey != (existing as IVersionedEntity)?.VersionKey)
                        me.CopyObjectData(existing); // copy data into reference
                    return existing;
                }
                else throw new KeyNotFoundException(me.Key.Value.ToString());
            }

            // Existing exists?
            if (existing != null && me.Key.HasValue)
            {
                // Exists but is an old version
                if ((existing as IVersionedEntity)?.VersionKey != vMe?.VersionKey &&
                    vMe?.VersionKey != null && vMe?.VersionKey != Guid.Empty)
                {
                    // Update method
                    IVersionedEntity updated = idpInstance.Update(context, me, principal) as IVersionedEntity;
                    me.Key = updated.Key;
                    if (vMe != null)
                        vMe.VersionKey = (updated as IVersionedEntity).VersionKey;
                    return updated;
                }
                return existing;
            }
            else if (existing == null) // Insert
            {
                IIdentifiedEntity inserted = idpInstance.Insert(context, me, principal) as IIdentifiedEntity;
                me.Key = inserted.Key;

                if (vMe != null)
                    vMe.VersionKey = (inserted as IVersionedEntity).VersionKey;
                return inserted;
            }
            return existing;
        }

        /// <summary>
        /// Has data changed
        /// </summary>
        public static bool IsSame<TObject>(this TObject me, TObject other)
        {
            bool retVal = true;
            if ((me == null) ^ (other == null)) return false;
            foreach (var pi in GetFields(me.GetType()))
            {
                if (pi.FieldType.IsGenericType && !pi.FieldType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) continue; /// Skip generics
                object meValue = pi.GetValue(me),
                    otherValue = pi.GetValue(other);

                retVal &= meValue != null ? meValue.Equals(otherValue) : otherValue == null;// Case null

            }
            return retVal;
        }

        /// <summary>
        /// Get the user identifier from the authorization context
        /// </summary>
        /// <param name="principal">The current authorization context</param>
        /// <param name="dataContext">The context under which the get operation should be completed</param>
        /// <returns>The UUID of the user which the authorization context subject represents</returns>
        public static Guid? GetUserKey(this IPrincipal principal, DataContext dataContext)
        {

            if (principal == null)
                return null;

            Guid userId = Guid.Empty;
            DbSecurityUser user = null;
            if (!m_userIdCache.TryGetValue(principal.Identity.Name.ToLower(), out userId))
            {
                user = dataContext.SingleOrDefault<DbSecurityUser>(o => o.UserName.ToLower() == principal.Identity.Name.ToLower() && !o.ObsoletionTime.HasValue);
                userId = user.Key;
                // TODO: Enable auto-creation of users via configuration
                if (user == null)
                    throw new SecurityException("User in authorization context does not exist or is obsolete");
                else
                {
                    lock (m_userIdCache)
                        if (!m_userIdCache.ContainsKey(principal.Identity.Name.ToLower()))
                            m_userIdCache.Add(principal.Identity.Name.ToLower(), user.Key);
                }
            }


            return userId;

        }

        /// <summary>
        /// Get the current version of the concept
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static TData CurrentVersion<TData>(this TData me, DataContext context)
            where TData : DbVersionedData, new()
        {
            var stmt = context.CreateSqlStatement<TData>().SelectFrom().Where(o => !o.ObsoletionTime.HasValue).OrderBy<TData>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
            return (TData)context.FirstOrDefault<TData>(stmt);
        }

        /// <summary>
        /// This method will load all basic properties for the specified model object
        /// </summary>
        public static void LoadAssociations<TModel>(this TModel me, DataContext context, IPrincipal principal, params String[] loadProperties) where TModel : IIdentifiedEntity
        {
            // I duz not haz a chzbrgr?
            if (me == null || me.LoadState >= context.LoadState)
                return;
            else if (context.Transaction != null) // kk.. I haz a transaction
                return;

#if DEBUG
            /*
             * Me neez all the timez

               /\_/\
               >^.^<.---.
              _'-`-'     )\
             (6--\ |--\ (`.`-.
                 --'  --'  ``-'
            */
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            // Cache get classification property - thiz makez us fasters
            PropertyInfo classProperty = null;
            if (!s_classificationProperties.TryGetValue(typeof(TModel), out classProperty))
            {
                classProperty = typeof(TModel).GetRuntimeProperty(typeof(TModel).GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty ?? "____XXX");
                if (classProperty != null)
                    classProperty = typeof(TModel).GetRuntimeProperty(classProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty ?? classProperty.Name);
                lock (s_lockObject)
                    if (!s_classificationProperties.ContainsKey(typeof(TModel)))
                        s_classificationProperties.Add(typeof(TModel), classProperty);
            }

            // Classification property?
            String classValue = classProperty?.GetValue(me)?.ToString();

            // Cache the props so future kitties can call it
            IEnumerable<PropertyInfo> properties = null;
            var propertyCacheKey = $"{me.GetType()}.FullName[{classValue}]";
            if (!s_runtimeProperties.TryGetValue(propertyCacheKey, out properties))
                lock (s_runtimeProperties)
                {
                    properties = me.GetType().GetRuntimeProperties().Where(o => o.GetCustomAttribute<DataIgnoreAttribute>() == null && o.GetCustomAttributes<AutoLoadAttribute>().Any(p => p.ClassCode == classValue || p.ClassCode == null) && typeof(IdentifiedData).IsAssignableFrom(o.PropertyType.StripGeneric())).ToList();

                    if (!s_runtimeProperties.ContainsKey(propertyCacheKey))
                    {
                        s_runtimeProperties.Add(propertyCacheKey, properties);
                    }
                }

            // Load fast or lean mode only root associations which will appear on the wire
            if (context.LoadState == LoadState.PartialLoad)
            {
                if (me.LoadState == LoadState.PartialLoad) // already partially loaded :/ 
                    return;
                else
                {
                    me.LoadState = LoadState.PartialLoad;
                    properties = properties.Where(o => o.GetCustomAttribute<XmlAttributeAttribute>() != null || o.GetCustomAttribute<XmlElementAttribute>() != null).ToList();
                }
            }

            // Iterate over the properties and load the properties
            foreach (var pi in properties)
            {
                if (loadProperties.Length > 0 &&
                    !loadProperties.Contains(pi.Name))
                    continue;

                // Map model type to domain
                var adoPersister = AdoPersistenceService.GetPersister(pi.PropertyType.StripGeneric());

                // Loading associations, so what is the associated type?
                if (typeof(IList).IsAssignableFrom(pi.PropertyType) &&
                    adoPersister is IAdoAssociativePersistenceService &&
                    me.Key.HasValue) // List so we select from the assoc table where we are the master table
                {
                    // Is there not a value?
                    var assocPersister = adoPersister as IAdoAssociativePersistenceService;

                    // We want to query based on our PK and version if applicable
                    decimal? versionSequence = (me as IBaseEntityData)?.ObsoletionTime.HasValue == true ? (me as IVersionedEntity)?.VersionSequence : null;
                    var assoc = assocPersister.GetFromSource(context, me.Key.Value, versionSequence, principal);
                    ConstructorInfo ci = null;
                    if(!m_constructors.TryGetValue(pi.PropertyType, out ci))
                    {
                        var type = pi.PropertyType.StripGeneric();
                        while (type != typeof(Object) && ci == null)
                        {
                            ci = pi.PropertyType.GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(type) });
                            type = type.BaseType;
                        }
                        if (ci != null)
                        {
                            lock (s_lockObject)
                                if (!m_constructors.ContainsKey(pi.PropertyType))
                                    m_constructors.Add(pi.PropertyType, ci);
                        }
                        else
                            throw new InvalidOperationException($"This is odd, you seem to have a list with no constructor -> {pi.PropertyType}");
                    }
                    var listValue = ci.Invoke(new object[] { assoc });
                    pi.SetValue(me, listValue);
                }
                else if (typeof(IIdentifiedEntity).IsAssignableFrom(pi.PropertyType)) // Single
                {
                    // Single property, we want to execute a get on the key property
                    var pid = pi;
                    if (pid.Name.EndsWith("Xml")) // Xml purposes only
                        pid = pi.DeclaringType.GetProperty(pi.Name.Substring(0, pi.Name.Length - 3));
                    var redirectAtt = pid.GetCustomAttribute<SerializationReferenceAttribute>();

                    // We want to issue a query
                    var keyProperty = redirectAtt != null ? pi.DeclaringType.GetProperty(redirectAtt.RedirectProperty) : pi.DeclaringType.GetProperty(pid.Name + "Key");
                    if (keyProperty == null)
                        continue;
                    var keyValue = keyProperty?.GetValue(me);
                    if (keyValue == null ||
                        Guid.Empty.Equals(keyValue))
                        continue; // No key specified

                    // This is kinda messy.. maybe iz to be changez
                    object value = null;
                    if (!context.Data.TryGetValue(keyValue.ToString(), out value))
                    {
                        value = adoPersister.Get(context, (Guid)keyValue, principal);
                        context.AddData(keyValue.ToString(), value);
                    }
                    pid.SetValue(me, value);
                }

            }
#if DEBUG
            sw.Stop();
            s_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Load associations for {0} took {1} ms", me, sw.ElapsedMilliseconds);
#endif

            if (me.LoadState == LoadState.New)
                me.LoadState = LoadState.FullLoad;
        }
    }
}
