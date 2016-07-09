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
 * Date: 2016-1-13
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Reflection;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using OpenIZ.Persistence.Data.MSSQL.Services;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{

    
    /// <summary>
    /// An attribute used by the LINQ expression rewriter to handle the queries on interface properties
    /// </summary>
    public class LinqPropertyMapAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance of the concept map attribute
        /// </summary>
        public LinqPropertyMapAttribute(String linqMember)
        {
            this.LinqMember = linqMember;
        }

        /// <summary>
        /// Gets or sets the name of the column which is the source for this
        /// </summary>
        public String LinqMember { get; set; }
    }

    /// <summary>
    /// Http query expression visitor.
    /// </summary>
    public class ExpressionRewriter : ExpressionVisitor
    {
      
        /// <summary>
        /// Visit a query expression
        /// </summary>
        /// <returns>The modified expression list, if any one of the elements were modified; otherwise, returns the original
        /// expression list.</returns>
        /// <param name="nodes">The expressions to visit.</param>
        /// <param name="node">Node.</param>
        public override Expression Visit(Expression node)
        {
            if (node == null)
                return node;

            // Convert node type
            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)node);
                default:
                    return base.Visit(node);
            }
        }

        /// <summary>
        /// Visits the member access.
        /// </summary>
        /// <returns>The member access.</returns>
        /// <param name="expr">Expr.</param>
        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            var cma = node.Expression.Type.GetMember(node.Member.Name)[0].GetCustomAttribute<LinqPropertyMapAttribute>();
            if (cma != null)
            {
                var rwMember = node.Expression.Type.GetMember(cma.LinqMember).Single();
                node = Expression.MakeMemberAccess(this.Visit(node.Expression), rwMember);
            }
            return node;
        }

        /// <summary>
        /// Rewrites the query mapping any mapped data elements 
        /// </summary>
        public static Expression<Func<TDomain, bool>> Rewrite<TDomain>(Expression<Func<TDomain, bool>> expr)
        {
            ExpressionRewriter rw = new ExpressionRewriter();
            var rwExpr = rw.Visit(expr.Body);
            return Expression.Lambda<Func<TDomain, bool>>(rwExpr, expr.Parameters);
        }
    }

    /// <summary>
    /// Model extension methods
    /// </summary>
    public static class DataModelExtensions
    {



        // Field cache
        private static Dictionary<Type, FieldInfo[]> s_fieldCache = new Dictionary<Type, FieldInfo[]>();

        // Lock object
        private static Object s_lockObject = new object();

        /// <summary>
        /// Get fields
        /// </summary>
        private static FieldInfo[] GetFields(Type type)
        {
            
            FieldInfo[] retVal = null;
            if(!s_fieldCache.TryGetValue(type, out retVal))
            {
                retVal = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(o =>!typeof(MulticastDelegate).IsAssignableFrom(o.FieldType)).ToArray();
                lock(s_lockObject)
                    if (!s_fieldCache.ContainsKey(type))
                        s_fieldCache.Add(type, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Ensures a model has been persisted
        /// </summary>
        public static void EnsureExists(this IIdentifiedEntity me, ModelDataContext context, IPrincipal principal) 
        {
            // Me
            var vMe = me as IVersionedEntity;
            String dkey = String.Format("{0}.{1}", me.GetType().FullName, me.Key);

            IIdentifiedEntity existing = null;
            var idpType = typeof(IDataPersistenceService<>).MakeGenericType(me.GetType());
            var idpInstance = ApplicationContext.Current.GetService(idpType);

            // Is the key null? 
            if (me.Key == Guid.Empty || me.Key == null) 
            {
                // Is there a classifier?
                var classAtt = me.GetType().GetCustomAttribute<KeyLookupAttribute>();
                if (classAtt != null)
                {
                    object classifierValue = me;// me.GetType().GetProperty(classAtt.ClassifierProperty).GetValue(me);
                    // Follow the classifier
                    Type predicateType = typeof(Func<,>).MakeGenericType(me.GetType(), typeof(bool));
                    ParameterExpression parameterExpr = Expression.Parameter(me.GetType(), "o");
                    Expression accessExpr = parameterExpr;
                    while (classAtt != null)
                    {
                        var property = accessExpr.Type.GetRuntimeProperty(classAtt.UniqueProperty);
                        accessExpr = Expression.MakeMemberAccess(accessExpr, property);
                        classifierValue = property.GetValue(classifierValue);

                        classAtt = accessExpr.Type.GetCustomAttribute<KeyLookupAttribute>();

                    }

                    // public abstract IQueryable<TData> Query(ModelDataContext context, Expression<Func<TData, bool>> query, IPrincipal principal);
                    var queryMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Query" && o.GetParameters().Length == 3 && o.GetParameters()[0].ParameterType == typeof(ModelDataContext));
                    var builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { predicateType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
                    var expression = builderMethod.Invoke(null, new object[] { Expression.MakeBinary(ExpressionType.Equal, accessExpr, Expression.Constant(classifierValue)), new ParameterExpression[] { parameterExpr } }) as Expression;

                    if (queryMethod == null) return;
                    var iq = queryMethod.Invoke(idpInstance, new object[] { context, expression, principal }) as IQueryable;
                    foreach (var i in iq)
                    {
                        existing = i as IIdentifiedEntity;
                        me.Key = existing.Key;
                        if(vMe != null)
                            vMe.VersionKey = (existing as IVersionedEntity)?.VersionKey ?? Guid.Empty;
                        break;
                    }
                }
            }
            else
            {
                // We have to find it
                var getMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Get" && o.GetParameters().Length == 3 && o.GetParameters()[0].ParameterType == typeof(ModelDataContext));
                if (getMethod == null) return;
                existing = getMethod.Invoke(idpInstance, new object[] { context, me.Key, principal }) as IIdentifiedEntity;
            }

            // Existing exists?
            if (existing != null)
            {
                // Exists but is an old version
                if ((existing as IVersionedEntity)?.VersionKey != vMe?.VersionKey &&
                    vMe?.VersionKey != Guid.Empty)
                {
                    // Update method
                    var updateMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Update" && o.GetParameters().Length == 3 && o.GetParameters()[0].ParameterType == typeof(ModelDataContext));
                    if (updateMethod != null)
                    {
                        IVersionedEntity updated = updateMethod.Invoke(idpInstance, new object[] { context, me, principal }) as IVersionedEntity;
                        me.Key = updated.Key;
                        if (vMe != null)
                            vMe.VersionKey = (updated as IVersionedEntity).VersionKey;
                    }
                }
            }
            else // Insert
            {
                var insertMethod = idpInstance.GetType().GetRuntimeMethods().SingleOrDefault(o => o.Name == "Insert" && o.GetParameters().Length == 3 && o.GetParameters()[0].ParameterType == typeof(ModelDataContext));
                if (insertMethod != null)
                {
                    IIdentifiedEntity inserted = insertMethod.Invoke(idpInstance, new object[] { context, me, principal }) as IIdentifiedEntity;
                    me.Key = inserted.Key;

                    if (vMe != null)
                        vMe.VersionKey = (inserted as IVersionedEntity).VersionKey;
                }
            }
        }

        /// <summary>
        /// Updates a keyed delay load field if needed
        /// </summary>
        public static void UpdateParentKeys(this IIdentifiedEntity instance, PropertyInfo field)
        {
            var delayLoadProperty = field.GetCustomAttribute<DelayLoadAttribute>();
            if (delayLoadProperty == null || String.IsNullOrEmpty(delayLoadProperty.KeyPropertyName))
                return;
            var value = field.GetValue(instance) as IIdentifiedEntity;
            if (value == null)
                return;
            // Get the delay load key property!
            var keyField = instance.GetType().GetRuntimeProperty(delayLoadProperty.KeyPropertyName);
            keyField.SetValue(instance, value.Key);
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
        /// Has data changed
        /// </summary>
        public static bool IsSame<TObject>(this TObject me, TObject other)
        {
            bool retVal = true;
            if ((me == null) ^ (other == null)) return false;
            foreach(var pi in GetFields(me.GetType()))
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
        public static Data.SecurityUser GetUser(this IPrincipal principal, ModelDataContext dataContext)
        {

            if (principal == null)
                return null;

            var user = dataContext.SecurityUsers.Single(o => o.UserName == principal.Identity.Name && !o.ObsoletionTime.HasValue);
            // TODO: Enable auto-creation of users via configuration
            if (user == null)
                throw new SecurityException("User in authorization context does not exist or is obsolete");

            return user;

        }

        /// <summary>
        /// Get the current version of the concept
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static TData CurrentVersion<TData>(this EntitySet<TData> me)
            where TData : class, IDbVersionedData, new()
        {
            return me.Where(o => !o.ObsoletionTime.HasValue).OrderByDescending(o => o.VersionSequenceId).FirstOrDefault();
        }
    }
}
