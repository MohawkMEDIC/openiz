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
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Core.Model
{
	/// <summary>
	/// Reflection tools
	/// </summary>
	public static class ExtensionMethods
	{
		// Property cache
		private static Dictionary<String, PropertyInfo> s_propertyCache = new Dictionary<string, PropertyInfo>();

		private static Dictionary<Type, PropertyInfo[]> s_typePropertyCache = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Get postal code
        /// </summary>
        public static String Value(this EntityAddress me, Guid addressType)
        {
            return me.LoadCollection<EntityAddressComponent>("Component").FirstOrDefault(o => o.ComponentTypeKey == addressType)?.Value;
        }

        /// <summary>
        /// Delay load property
        /// </summary>
        public static TReturn LoadProperty<TReturn>(this IdentifiedData me, string propertyName)
        {
            return (TReturn)me.LoadProperty(propertyName);
        }

        /// <summary>
        /// Delay load property
        /// </summary>
        public static IEnumerable<TReturn> LoadCollection<TReturn>(this IdentifiedData me, string propertyName)
        {
            return me.LoadProperty(propertyName) as IEnumerable<TReturn>;
        }

        /// <summary>
        /// Delay load property
        /// </summary>
        public static object LoadProperty(this IdentifiedData me, string propertyName)
        {
            if (me == null) return null;

            var propertyToLoad = me.GetType().GetRuntimeProperty(propertyName);
            if (propertyToLoad == null) return null;

            var currentValue = propertyToLoad.GetValue(me);

            if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(propertyToLoad.PropertyType.GetTypeInfo())) // Collection we load by key
            {
                if ((currentValue as IList)?.Count == 0)
                {
                    var mi = typeof(IEntitySourceProvider).GetGenericMethod(nameof(IEntitySourceProvider.GetRelations), new Type[] { propertyToLoad.PropertyType.StripGeneric() }, new Type[] { typeof(Guid?) });
                    var loaded = Activator.CreateInstance(propertyToLoad.PropertyType, mi.Invoke(EntitySource.Current.Provider, new object[] { me.Key.Value }));
                    propertyToLoad.SetValue(me, loaded);
                    return loaded;
                }
                return currentValue;
            }
            else if (currentValue == null)
            {
                var xsr = propertyToLoad.GetCustomAttribute<SerializationReferenceAttribute>();
                if (xsr == null) return me;
                var keyValue = me.GetType().GetRuntimeProperty(xsr.RedirectProperty).GetValue(me) as Guid?;
                if (keyValue.GetValueOrDefault() == default(Guid))
                    return currentValue;
                else
                {
                    var mi = typeof(IEntitySourceProvider).GetGenericMethod(nameof(IEntitySourceProvider.Get), new Type[] { propertyToLoad.PropertyType }, new Type[] { typeof(Guid?) });
                    var loaded = mi.Invoke(EntitySource.Current.Provider, new object[] { keyValue });
                    propertyToLoad.SetValue(me, loaded);
                    return loaded;
                }
            }
            else return currentValue;
            // Now we want to 

        }
        
        /// <summary>
        /// Create aggregation functions
        /// </summary>
        public static Expression Aggregate(this Expression me, AggregationFunctionType aggregation)
		{
			var aggregateMethod = typeof(Enumerable).GetGenericMethod(aggregation.ToString(),
			   new Type[] { me.Type.GetTypeInfo().GenericTypeArguments[0] },
			   new Type[] { me.Type });
			return Expression.Call(aggregateMethod as MethodInfo, me);
		}

		/// <summary>
		/// The purpose of this method is to convert object <paramref name="me"/> to <typeparamref name="TReturn"/>. Why?
		/// Because if you have an instance of Act that actually needs to be a SubstanceAdministration we can't just cast
		/// so we have to copy.
		/// </summary>
		public static TReturn Convert<TReturn>(this Object me) where TReturn : new()
		{
			if (me is TReturn) return (TReturn)me;
			else
			{
				var retVal = new TReturn();
				retVal.CopyObjectData(me);
				return retVal;
			}
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
			else if (!fromEntity.GetType().GetTypeInfo().IsAssignableFrom(toEntity.GetType().GetTypeInfo()))
				throw new ArgumentException("Type mismatch", nameof(fromEntity));

			PropertyInfo[] properties = null;
			if (!s_typePropertyCache.TryGetValue(toEntity.GetType(), out properties))
			{
				properties = toEntity.GetType().GetRuntimeProperties().Where(destinationPi => destinationPi.GetCustomAttribute<DataIgnoreAttribute>() == null &&
					destinationPi.CanWrite).ToArray();
				lock (s_typePropertyCache)
					if (!s_typePropertyCache.ContainsKey(toEntity.GetType()))
						s_typePropertyCache.Add(toEntity.GetType(), properties);
			}
			foreach (var destinationPi in properties.AsParallel())
			{
				var sourcePi = fromEntity.GetType().GetRuntimeProperty(destinationPi.Name);
				// Skip properties no in the source
				if (sourcePi == null)
					continue;

				// Skip data ignore
				if (destinationPi.PropertyType.GetTypeInfo().IsGenericType &&
					destinationPi.PropertyType.GetGenericTypeDefinition().Namespace.StartsWith("System.Data.Linq") ||
					destinationPi.PropertyType.Namespace.StartsWith("OpenIZ.Persistence"))
					continue;

				object newValue = sourcePi.GetValue(fromEntity),
					oldValue = destinationPi.GetValue(toEntity);

				// HACK: New value wrap for nullables
				if (newValue is Guid? && newValue != null)
					newValue = (newValue as Guid?).Value;

				// HACK: Empty lists are NULL
				if ((newValue as IList)?.Count == 0)
					newValue = null;

				if (newValue != null &&
					!newValue.Equals(oldValue) == true &&
                    (destinationPi.PropertyType.GetTypeInfo().IsValueType && !newValue.Equals(Activator.CreateInstance(newValue.GetType())) || !destinationPi.PropertyType.GetTypeInfo().IsValueType))
					destinationPi.SetValue(toEntity, newValue);
			}
		}

		/// <summary>
		/// Get generic method
		/// </summary>
		public static MethodBase GetGenericMethod(this Type type, string name, Type[] typeArgs, Type[] argTypes)
		{
			int typeArity = typeArgs.Length;
			var methods = type.GetRuntimeMethods()
				.Where(m => m.Name == name)
				.Where(m => m.GetGenericArguments().Length == typeArity)
				.Where(m => m.GetParameters().Length == argTypes.Length)
				.Select(m => m.MakeGenericMethod(typeArgs)).ToList()
				.Where(m => m.GetParameters().All(o => argTypes.Any(p => o.ParameterType.GetTypeInfo().IsAssignableFrom(p.GetTypeInfo()))));

			return methods.FirstOrDefault();
			//return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
		}

		/// <summary>
		/// Get a property based on XML property and/or serialization redirect
		/// </summary>
		public static PropertyInfo GetXmlProperty(this Type type, string propertyName, bool followReferences = false)
		{
			PropertyInfo retVal = null;
			var key = String.Format("{0}.{1}[{2}]", type.FullName, propertyName, followReferences);
			if (!s_propertyCache.TryGetValue(key, out retVal))
			{
				retVal = type.GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttributes<XmlElementAttribute>()?.FirstOrDefault()?.ElementName == propertyName);
                if (retVal == null)
                    throw new MissingMemberException(propertyName);
				if (followReferences) retVal = type.GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty == retVal.Name) ?? retVal;

				if (retVal.Name.EndsWith("Xml"))
					retVal = type.GetRuntimeProperty(retVal.Name.Substring(0, retVal.Name.Length - 3));

				lock (s_propertyCache)
					if (!s_propertyCache.ContainsKey(key))
						s_propertyCache.Add(key, retVal);
			}
			return retVal;
		}

		/// <summary>
		/// Compute a basic hash string
		/// </summary>
		public static String HashCode(this byte[] me)
		{
			long hash = 1009;
			foreach (var b in me)
				hash = ((hash << 5) + hash) ^ b;
			return BitConverter.ToString(BitConverter.GetBytes(hash)).Replace("-", "");
		}

		/// <summary>
		/// Determines whether the specified property name has property.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns><c>true</c> if the specified property name has property; otherwise, <c>false</c>.</returns>
		/// <exception cref="System.ArgumentNullException">source - Value cannot be null</exception>
		/// <exception cref="System.ArgumentException">Value cannot be null or empty - propertyName</exception>
		public static bool HasProperty(this ExpandoObject source, string propertyName)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), "Value cannot be null");
			}

			if (string.IsNullOrEmpty(propertyName) || string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException("Value cannot be null or empty", nameof(propertyName));
			}

			return ((IDictionary<string, object>)source).ContainsKey(propertyName);
		}

		/// <summary>
		/// Gets the full name of the user entity.
		/// </summary>
		/// <param name="entity">The user entity.</param>
		/// <param name="nameUseKey">The name use key.</param>
		/// <returns>Returns the full name of the user entity.</returns>
		/// <exception cref="System.ArgumentNullException">If the entity is null.</exception>
		public static string GetFullName(this UserEntity entity, Guid nameUseKey)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "Value cannot be null");
			}

			var given = entity.Names.Where(n => n.NameUseKey == nameUseKey).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList();
			var family = entity.Names.Where(n => n.NameUseKey == nameUseKey).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList();

			return string.Join(" ", given) + " " + string.Join(" ", family);
		}

		/// <summary>
		/// Create a version filter
		/// </summary>
		/// <param name="me">Me.</param>
		/// <param name="domainInstance">The domain instance.</param>
		/// <returns>Expression.</returns>
		public static Expression IsActive(this Expression me, Object domainInstance)
		{
			// Extract boundary properties
			var effectiveVersionMethod = me.Type.GetTypeInfo().GenericTypeArguments[0].GetRuntimeProperty("EffectiveVersionSequenceId");
			var obsoleteVersionMethod = me.Type.GetTypeInfo().GenericTypeArguments[0].GetRuntimeProperty("ObsoleteVersionSequenceId");
			if (effectiveVersionMethod == null || obsoleteVersionMethod == null)
				return me;

			// Create predicate type and find WHERE method
			Type predicateType = typeof(Func<,>).MakeGenericType(me.Type.GetTypeInfo().GenericTypeArguments[0], typeof(bool));
			var whereMethod = typeof(Enumerable).GetGenericMethod("Where",
				new Type[] { me.Type.GetTypeInfo().GenericTypeArguments[0] },
				new Type[] { me.Type, predicateType });

			// Create Where Expression
			var guardParameter = Expression.Parameter(me.Type.GetTypeInfo().GenericTypeArguments[0], "x");
			var currentSequenceId = domainInstance?.GetType().GetRuntimeProperty("VersionSequenceId").GetValue(domainInstance);
			var bodyExpression = Expression.MakeBinary(ExpressionType.AndAlso,
				Expression.MakeBinary(ExpressionType.LessThanOrEqual, Expression.MakeMemberAccess(guardParameter, effectiveVersionMethod), Expression.Constant(currentSequenceId)),
				Expression.MakeBinary(ExpressionType.OrElse,
					Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(guardParameter, obsoleteVersionMethod), Expression.Constant(null)),
					Expression.MakeBinary(ExpressionType.GreaterThan, Expression.MakeMemberAccess(
						Expression.MakeMemberAccess(guardParameter, obsoleteVersionMethod),
						typeof(Nullable<Decimal>).GetRuntimeProperty("Value")), Expression.Constant(currentSequenceId))
				)
			);

			// Build strongly typed lambda
			var builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { predicateType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
			var sortLambda = builderMethod.Invoke(null, new object[] { bodyExpression, new ParameterExpression[] { guardParameter } }) as Expression;
			return Expression.Call(whereMethod as MethodInfo, me, sortLambda);
		}

		/// <summary>
		/// Create a version filter
		/// </summary>
		/// <param name="me">Me.</param>
		/// <returns>Expression.</returns>
		public static Expression IsActive(this Expression me)
		{
			// Extract boundary properties
			var obsoleteVersionMethod = me.Type.GetTypeInfo().GenericTypeArguments[0].GetRuntimeProperty("ObsoleteVersionSequenceId");
			if (obsoleteVersionMethod == null)
				return me;

			// Create predicate type and find WHERE method
			Type predicateType = typeof(Func<,>).MakeGenericType(me.Type.GetTypeInfo().GenericTypeArguments[0], typeof(bool));
			var whereMethod = typeof(Enumerable).GetGenericMethod("Where",
				new Type[] { me.Type.GetTypeInfo().GenericTypeArguments[0] },
				new Type[] { me.Type, predicateType });

			// Create Where Expression
			var guardParameter = Expression.Parameter(me.Type.GetTypeInfo().GenericTypeArguments[0], "x");
			var bodyExpression = Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(guardParameter, obsoleteVersionMethod), Expression.Constant(null));

			// Build strongly typed lambda
			var builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { predicateType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
			var sortLambda = builderMethod.Invoke(null, new object[] { bodyExpression, new ParameterExpression[] { guardParameter } }) as Expression;
			return Expression.Call(whereMethod as MethodInfo, me, sortLambda);
		}

		/// <summary>
		/// Gets the latest version of the versioned entity data instance from a given list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <returns>Returns the latest version only of the versioned entity data.</returns>
		public static IEnumerable<T> LatestVersionOnly<T>(this IEnumerable<T> source) where T : IVersionedEntity
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), "Value cannot be null");
			}

			var latestVersions = new List<T>();

			var keys = source.Select(e => e.Key.Value).Distinct();

			foreach (var key in keys)
			{
				var maxVersionSequence = source.Select(e => source.Where(a => a.Key == key).Max<T>(a => a.VersionSequence)).FirstOrDefault();

				var latestVersion = source.FirstOrDefault(a => a.Key == key && a.VersionSequence == maxVersionSequence);

				if (latestVersion != null)
				{
					latestVersions.Add(latestVersion);
				}
			}

			return latestVersions;
		}

		/// <summary>
		/// Determine semantic equality of each item in me and other
		/// </summary>
		public static bool SemanticEquals<TEntity>(this IEnumerable<TEntity> me, IEnumerable<TEntity> other) where TEntity : IdentifiedData
		{
			if (other == null) return false;
			bool equals = me.Count() == other.Count();
			foreach (var itm in me)
				equals &= other.Any(o => o.SemanticEquals(itm));
			foreach (var itm in other)
				equals &= me.Any(o => o.SemanticEquals(itm));

			return equals;
		}

		/// <summary>
		/// Create sort expression.
		/// </summary>
		/// <param name="me">Me.</param>
		/// <param name="orderByProperty">The order by property.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <returns>Expression.</returns>
		public static Expression Sort(this Expression me, String orderByProperty, SortOrderType sortOrder)
		{
			// Get sort property
			var sortProperty = me.Type.GenericTypeArguments[0].GetRuntimeProperty(orderByProperty);
			Type predicateType = typeof(Func<,>).MakeGenericType(me.Type.GetTypeInfo().GenericTypeArguments[0], sortProperty.PropertyType);
			var sortMethod = typeof(Enumerable).GetGenericMethod(sortOrder.ToString(),
				new Type[] { me.Type.GetTypeInfo().GenericTypeArguments[0], sortProperty.PropertyType },
				new Type[] { me.Type, predicateType });

			// Get builder methods
			var sortParameter = Expression.Parameter(me.Type.GetTypeInfo().GenericTypeArguments[0], "sort");
			var builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { predicateType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
			var sortLambda = builderMethod.Invoke(null, new object[] { Expression.MakeMemberAccess(sortParameter, sortProperty), new ParameterExpression[] { sortParameter } }) as Expression;
			return Expression.Call(sortMethod as MethodInfo, me, sortLambda);
		}

		/// <summary>
		/// Strips list
		/// </summary>
		/// <param name="t">The type.</param>
		/// <returns>Returns the type.</returns>
		public static Type StripGeneric(this Type t)
		{
			return t.GetTypeInfo().IsGenericType ? t.GetTypeInfo().GenericTypeArguments[0] : t;
		}

		/// <summary>
		/// Strips any nullable typing.
		/// </summary>
		/// <param name="t">The typ.</param>
		/// <returns>Returns the type.</returns>
		public static Type StripNullable(this Type t)
		{
			if (t.GetTypeInfo().IsGenericType &&
				t.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
				return t.GetTypeInfo().GenericTypeArguments[0];
			return t;
		}
	}
}