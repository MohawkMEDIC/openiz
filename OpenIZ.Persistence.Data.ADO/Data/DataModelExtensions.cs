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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using OpenIZ.Persistence.Data.ADO.Services;
using OpenIZ.Persistence.Data.ADO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data
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
        public static IIdentifiedEntity TryGetExisting(this IIdentifiedEntity me, DataContext context, IPrincipal principal)
        {

            // Is there a classifier?
            var idpInstance = AdoPersistenceService.GetPersister(me.GetType()) as IAdoPersistenceService;

            IIdentifiedEntity existing = null;

            // Is the key not null?
            if (me.Key != Guid.Empty && me.Key != null)
            {
                existing = idpInstance.Get(context, me.Key.Value, principal) as IIdentifiedEntity;
            }

            var classAtt = me.GetType().GetCustomAttribute<KeyLookupAttribute>();
            if (classAtt != null)
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
                SqlStatement stmt = new SqlStatement().SelectFrom(dataType)
                    .Where($"{column.Name} = ?", classifierValue);

                var dataObject = context.FirstOrDefault(dataType, stmt);
                if (dataObject != null)
                    existing = AdoPersistenceService.GetMapper().MapDomainInstance(dataType, me.GetType(), dataObject) as IIdentifiedEntity;

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
        public static void EnsureExists(this IIdentifiedEntity me, DataContext context, IPrincipal principal)
        {

            if (!AdoPersistenceService.GetConfiguration().AutoInsertChildren) return;

            // Me
            var vMe = me as IVersionedEntity;
            String dkey = String.Format("{0}.{1}", me.GetType().FullName, me.Key);

            IIdentifiedEntity existing = me.TryGetExisting(context, principal);
            var idpInstance = AdoPersistenceService.GetPersister(me.GetType());

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
                }
            }
            else // Insert
            {
                IIdentifiedEntity inserted = idpInstance.Insert(context, me, principal) as IIdentifiedEntity;
                me.Key = inserted.Key;

                if (vMe != null)
                    vMe.VersionKey = (inserted as IVersionedEntity).VersionKey;
            }
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
        public static DbSecurityUser GetUser(this IPrincipal principal, DataContext dataContext)
        {

            if (principal == null)
                return null;

            var user = dataContext.SingleOrDefault<DbSecurityUser>(o => o.UserName == principal.Identity.Name && !o.ObsoletionTime.HasValue);
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
        public static TData CurrentVersion<TData>(this TData me, DataContext context)
            where TData : DbVersionedData, new()
        {
            var stmt = new SqlStatement<TData>().SelectFrom().Where(o => !o.ObsoletionTime.HasValue).OrderBy<TData>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
            return (TData)context.FirstOrDefault<TData>(stmt);
        }
    }
}
