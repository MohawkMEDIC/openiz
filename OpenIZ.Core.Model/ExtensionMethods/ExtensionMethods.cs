/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Reflection tools
    /// </summary>
    public static class ExtensionMethods
    {

        // Property cache
        private static Dictionary<String, PropertyInfo> s_propertyCache = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// Compute a basic hash string
        /// </summary>
        public static String HashCode(this byte[] me)
        {
            long hash = 1009;
            foreach(var b in me)
                hash = ((hash << 5) + hash) ^ b;
            return BitConverter.ToString(BitConverter.GetBytes(hash)).Replace("-", "");
        }

        /// <summary>
        /// Determine semantic equality of each item in me and other
        /// </summary>
        public static bool SemanticEquals<TEntity>(this IEnumerable<TEntity> me, IEnumerable<TEntity> other) where TEntity : IdentifiedData
        {

            if (other == null) return false;
            bool equals = true;
            foreach (var itm in me)
                equals &= other.Any(o => o.SemanticEquals(itm));
            return equals;
        }

        /// <summary>
        /// Strips list
        /// </summary>
        public static Type StripGeneric(this Type t)
        {
            if (t.GetTypeInfo().IsGenericType)
                return t.GetTypeInfo().GenericTypeArguments[0];
            return t;
        }

        /// <summary>
        /// Strips any nullable typing
        /// </summary>
        public static Type StripNullable(this Type t)
        {
            if (t.GetTypeInfo().IsGenericType &&
                t.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                return t.GetTypeInfo().GenericTypeArguments[0];
            return t;
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
            foreach (var destinationPi in toEntity.GetType().GetRuntimeProperties())
            {
                var sourcePi = fromEntity.GetType().GetRuntimeProperty(destinationPi.Name);
                // Skip properties no in the source
                if (sourcePi == null)
                    continue;

                // Skip data ignore
                if (destinationPi.GetCustomAttribute<DataIgnoreAttribute>() == null &&
                    destinationPi.CanWrite)
                {
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
        }

        /// <summary>
        /// Create a version filter
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="parm"></param>
        /// <param name="domainInstance"></param>
        /// <returns></returns>
        public static Expression IsActive<TDomain>(this Expression me, TDomain domainInstance)
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
            var currentSequenceId = typeof(TDomain).GetRuntimeProperty("VersionSequenceId").GetValue(domainInstance);
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
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="parm"></param>
        /// <param name="domainInstance"></param>
        /// <returns></returns>
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
        /// Create sort expression
        /// </summary>
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
                if(followReferences) retVal = type.GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty == retVal.Name) ?? retVal;
                lock (s_propertyCache)
                    if (!s_propertyCache.ContainsKey(key))
                        s_propertyCache.Add(key, retVal);
            }
            return retVal;
        }

    }
}
