using OpenIZ.Core.Model.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Reflection
{
    /// <summary>
    /// Reflection tools
    /// </summary>
    public static class ReflectionUtil
    {
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
                Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, Expression.MakeMemberAccess(guardParameter, effectiveVersionMethod), Expression.Constant(currentSequenceId)),
                Expression.MakeBinary(ExpressionType.OrElse,
                    Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(guardParameter, obsoleteVersionMethod), Expression.Constant(null)),
                    Expression.MakeBinary(ExpressionType.LessThanOrEqual, Expression.MakeMemberAccess(
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


    }
}
