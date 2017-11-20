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
 * Date: 2016-11-30
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Util
{
    /// <summary>
    /// Object expansion tool
    /// </summary>
    public static class ObjectExpander
    {
        // Trace source
        private static TraceSource m_tracer = new TraceSource("OpenIZ.Messaging.IMSI");

        // Sync lock
        private static Object s_syncLock = new object();

        // Related load methods
        private static Dictionary<Type, MethodInfo> m_relatedLoadMethods = new Dictionary<Type, MethodInfo>();

        // Reloated load association
        private static Dictionary<Type, MethodInfo> m_relatedLoadAssociations = new Dictionary<Type, MethodInfo>();


		/// <summary>
		/// Load related object
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="key">The key.</param>
		/// <returns>System.Object.</returns>
		internal static object LoadRelated(Type propertyType, Guid key)
        {
            MethodInfo methodInfo = null;
            if (!m_relatedLoadMethods.TryGetValue(propertyType, out methodInfo))
            {
                methodInfo = typeof(ObjectExpander).GetRuntimeMethod(nameof(LoadRelated), new Type[] { typeof(Guid) }).MakeGenericMethod(propertyType);
                lock (s_syncLock)
                    if (!m_relatedLoadMethods.ContainsKey(propertyType))
                        m_relatedLoadMethods.Add(propertyType, methodInfo);

            }
            return methodInfo.Invoke(null, new object[] { key });
        }

		/// <summary>
		/// Load collection
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="entity">The entity.</param>
		/// <returns>IList.</returns>
		internal static IList LoadCollection(Type propertyType, IIdentifiedEntity entity)
        {
            MethodInfo methodInfo = null;

            var key = entity.Key;
            var versionKey = (entity as IVersionedEntity)?.VersionSequence;

            // Load
            if (!m_relatedLoadAssociations.TryGetValue(propertyType, out methodInfo))
            {
                if (versionKey.HasValue && typeof(IVersionedAssociation).IsAssignableFrom(propertyType.StripGeneric()))
                    methodInfo = typeof(ObjectExpander).GetRuntimeMethod(nameof(LoadCollection), new Type[] { typeof(Guid), typeof(decimal?) }).MakeGenericMethod(propertyType.StripGeneric());
                else
                    methodInfo = typeof(ObjectExpander).GetRuntimeMethod(nameof(LoadCollection), new Type[] { typeof(Guid) }).MakeGenericMethod(propertyType.StripGeneric());

                lock (s_syncLock)
                    if (!m_relatedLoadAssociations.ContainsKey(propertyType))
                        m_relatedLoadAssociations.Add(propertyType, methodInfo);

            }

            IList listValue = null;
            if (methodInfo.GetParameters().Length == 2)
                listValue = methodInfo.Invoke(null, new object[] { key, versionKey }) as IList;
            else
                listValue = methodInfo.Invoke(null, new object[] { key }) as IList;
            if (propertyType.GetTypeInfo().IsAssignableFrom(listValue.GetType().GetTypeInfo()))
                return listValue;
            else
            {
                var retVal = Activator.CreateInstance(propertyType, listValue);
                return retVal as IList;
            }

        }

		/// <summary>
		/// Delay loads the specified collection association
		/// </summary>
		/// <typeparam name="TAssociation">The type of the t association.</typeparam>
		/// <param name="sourceKey">The source key.</param>
		/// <param name="sourceSequence">The source sequence.</param>
		/// <returns>IEnumerable&lt;TAssociation&gt;.</returns>
		public static IEnumerable<TAssociation> LoadCollection<TAssociation>(Guid sourceKey, Decimal? sourceSequence) where TAssociation : IdentifiedData, IVersionedAssociation, new()
        {
            return EntitySource.Current.Provider.GetRelations<TAssociation>(sourceKey, sourceSequence);
        }

		/// <summary>
		/// Delay loads the specified collection association
		/// </summary>
		/// <typeparam name="TAssociation">The type of the t association.</typeparam>
		/// <param name="sourceKey">The source key.</param>
		/// <returns>IEnumerable&lt;TAssociation&gt;.</returns>
		public static IEnumerable<TAssociation> LoadCollection<TAssociation>(Guid sourceKey) where TAssociation : IdentifiedData, ISimpleAssociation, new()
        {
            return EntitySource.Current.Provider.GetRelations<TAssociation>(sourceKey);
        }

		/// <summary>
		/// Load the related information
		/// </summary>
		/// <typeparam name="TRelated">The type of the t related.</typeparam>
		/// <param name="objectKey">The object key.</param>
		/// <returns>TRelated.</returns>
		public static TRelated LoadRelated<TRelated>(Guid? objectKey) where TRelated : IdentifiedData, new()
		{
			if (objectKey.HasValue && objectKey != Guid.Empty)
                return EntitySource.Current.Provider.Get<TRelated>(objectKey);

			return default(TRelated);
		}


		/// <summary>
		/// Expand properties
		/// </summary>
		/// <param name="returnValue">The return value.</param>
		/// <param name="qp">The qp.</param>
		/// <param name="keyStack">The key stack.</param>
		/// <param name="emptyCollections">The empty collections.</param>
		public static void ExpandProperties(IdentifiedData returnValue, NameValueCollection qp, Stack<Guid> keyStack = null, Dictionary<Guid, HashSet<String>> emptyCollections = null)
        {
            if (emptyCollections == null)
                emptyCollections = new Dictionary<Guid, HashSet<string>>();

            // Set the stack
            if (keyStack == null)
                keyStack = new Stack<Guid>();
            else if (returnValue.Key.HasValue && keyStack.Contains(returnValue.Key.Value))
                return;

	        if (!returnValue.Key.HasValue || returnValue.Key.Equals(Guid.Empty))
	        {
		        return;
	        }

	        keyStack.Push(returnValue.Key.Value);

	        try
            {
                // Expand property?
                if (qp.ContainsKey("_expand") && qp.ContainsKey("_all"))
                    return;


                if (qp.ContainsKey("_all"))
                {
                    if (keyStack.Count > 3) return;

                    foreach (var pi in returnValue.GetType().GetRuntimeProperties().Where(o => (o.GetCustomAttribute<SerializationReferenceAttribute>() != null || o.GetCustomAttributes<XmlElementAttribute>().Count() > 0) &&
                    o.GetCustomAttribute<DataIgnoreAttribute>() == null))
                    {

                        // Get current value
                        var scope = pi.GetValue(returnValue);

                        // Force a load if null!!!
                        if (scope == null || (scope as IList)?.Count == 0)
                        {
                            if (typeof(IdentifiedData).IsAssignableFrom(pi.PropertyType))
                            {
                                var keyPi = pi.GetCustomAttribute<SerializationReferenceAttribute>()?.GetProperty(returnValue.GetType());
                                var keyValue = keyPi?.GetValue(returnValue);

                                // Get the value
                                if (keyValue != null)
                                    scope = LoadRelated(pi.PropertyType, (Guid)keyValue);
                                if (scope != null)
                                    pi.SetValue(returnValue, scope);
                            }
                            else if (typeof(IList).IsAssignableFrom(pi.PropertyType) && !pi.PropertyType.IsArray &&
                                typeof(IdentifiedData).IsAssignableFrom(pi.PropertyType.StripGeneric()))
                            {
                                // Already loaded?
                                HashSet<String> properties = null;
                                if (emptyCollections.TryGetValue(returnValue.Key.Value, out properties))
                                    if (!properties.Contains(pi.Name))
                                        properties.Add(pi.Name);
                                    else
                                        continue;
                                else
                                    emptyCollections.Add(returnValue.Key.Value, new HashSet<string>() { pi.Name });

                                scope = LoadCollection(pi.PropertyType, returnValue);
                                if ((scope as IList).Count > 0)
                                    pi.SetValue(returnValue, scope);
                            }
                        }

                        // Cascade
                        if (scope is IdentifiedData)
                            ExpandProperties(scope as IdentifiedData, qp, keyStack);
                        else if (scope is IList)
                            foreach (var itm in (scope as IList).OfType<Object>().ToList())
                                if (itm is IdentifiedData)
                                    ExpandProperties(itm as IdentifiedData, qp, keyStack);
                    }

                    //ApplicationContext.Current.GetService<IDataCachingService>()?.Add(returnValue);
                }
                else if (qp.ContainsKey("_expand"))
                {
                    foreach (var nvs in qp["_expand"])
                    {
                        // Get the property the user wants to expand
                        DoExpand(returnValue, nvs);
                    }

                    //ApplicationContext.Current.GetService<IDataCachingService>()?.Add(returnValue);
                }
            }
            finally
            {
                keyStack.Pop();
            }
        }

        /// <summary>
        /// Do expansion
        /// </summary>
        private static void DoExpand(object scope, string propertySpec)
        {
            Regex propertyRegex = new Regex(@"(\w*)?\.?([\w\.]*)?");
            var match = propertyRegex.Match(propertySpec);
            if (!match.Success) return;

            // My property
            if (scope is IList)
                foreach (var itm in scope as IList)
                {
                    var subScope = DoPopulateObject(itm, match.Groups[1].Value);

	                if (subScope == null)
		                return;

					if (!String.IsNullOrEmpty(match.Groups[2].Value))
                        DoExpand(subScope, match.Groups[2].Value);
                }
            else
            {
                var subScope = DoPopulateObject(scope, match.Groups[1].Value);

	            if (subScope == null)
		            return;
						
                if (!String.IsNullOrEmpty(match.Groups[2].Value))
                    DoExpand(subScope, match.Groups[2].Value);
            }
        }

        /// <summary>
        /// Do population of an object
        /// </summary>
        private static object DoPopulateObject(object scope, string property)
        {
	        if (scope == null)
		        return null;

            // Look for the property in the scope
            var propertyInfo = scope.GetType().GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault()?.ElementName == property);
            if (propertyInfo == null || !propertyInfo.CanWrite) return null; // invalid property name

            var propertyValue = propertyInfo.GetValue(scope);
            if (propertyValue is IList && (propertyValue as IList).Count == 0) // need to load
            {

                var rmi = typeof(ExtensionMethods).GetGenericMethod(nameof(ExtensionMethods.LoadCollection), new Type[] { propertyInfo.PropertyType.StripGeneric() }, new Type[] { scope.GetType(), typeof(String) });
                if (rmi == null)
                {
                    propertyValue = LoadCollection(propertyInfo.PropertyType, scope as IIdentifiedEntity);
                    propertyInfo.SetValue(scope, propertyValue);
                    scope = propertyValue;
                }
                else
                    propertyValue = rmi.Invoke(null, new object[] { scope, propertyInfo.Name });
            }
            else if (propertyValue is Guid) // A key!
            {
                var backingFieldInfo = scope.GetType().GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty == propertyInfo.Name);
                if (backingFieldInfo == null) return null; // stop 
                var rmi = typeof(ExtensionMethods).GetGenericMethod(nameof(ExtensionMethods.LoadProperty), new Type[] { backingFieldInfo.PropertyType }, new Type[] { scope.GetType(), typeof(String) });

                if (rmi == null)
                {
                    var existingValue = backingFieldInfo.GetValue(scope);
                    if (existingValue != null) return existingValue;
                    propertyValue = LoadRelated(backingFieldInfo.PropertyType, (Guid)propertyValue);
                    backingFieldInfo.SetValue(scope, propertyValue);
                }
                else
                    propertyValue = rmi.Invoke(null, new object[] { scope, backingFieldInfo.Name });

            }

            return propertyValue;
        }

        /// <summary>
        /// Excludes the specified properties from the result
        /// </summary>
        internal static void ExcludeProperties(IdentifiedData returnValue, NameValueCollection qp, Stack<Guid> keyStack = null)
        {
            // Set the stack
            if (keyStack == null)
                keyStack = new Stack<Guid>();
            else if (keyStack.Contains(returnValue.Key.Value))
                return;

            keyStack.Push(returnValue.Key.Value);

            try
            {
                // Expand property?
                if (!qp.ContainsKey("_exclude"))
                    return;
                else
                {
                    foreach (var property in qp["_exclude"])
                    {
                        PropertyInfo keyPi = returnValue.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault()?.ElementName == property);
                        if (keyPi == null)
                            continue;
                        // Get the backing property
                        PropertyInfo excludeProp = returnValue.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttributes<SerializationReferenceAttribute>().FirstOrDefault()?.RedirectProperty == keyPi.Name);

                        if (excludeProp != null && excludeProp.CanWrite)
                            excludeProp.SetValue(returnValue, null);
                        else if (keyPi.CanWrite)
                            keyPi.SetValue(returnValue, null);
                    }

                }
            }
            finally
            {
                keyStack.Pop();
            }
        }
    }
}
