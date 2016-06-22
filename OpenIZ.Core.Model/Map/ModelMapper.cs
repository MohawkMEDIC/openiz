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
 * Date: 2016-4-19
 */
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Reflection;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Model mapper
    /// </summary>
    public sealed class ModelMapper
    {

        // The map file
        private ModelMap m_mapFile;
        
        /// <summary>
        /// Creates a new mapper from source stream
        /// </summary>
        public ModelMapper(Stream sourceStream)
        {
            this.Load(sourceStream);
        }

        /// <summary>
        /// Load mapping from a stream
        /// </summary>
        private void Load(Stream sourceStream)
        {
            this.m_mapFile = ModelMap.Load(sourceStream);
        }

        /// <summary>
        /// Map member 
        /// </summary>
        public Expression MapModelMember(MemberExpression memberExpression, Expression accessExpression)
        {
            
            ClassMap classMap = this.m_mapFile.GetModelClassMap(memberExpression.Expression.Type);

            if (classMap == null)
                return memberExpression;

            // Expression is the same class? Collapse if it is a key
            MemberExpression accessExpressionAsMember = accessExpression as MemberExpression;
            CollapseKey collapseKey = null;
            PropertyMap propertyMap = null;

            if (memberExpression.Member.Name == "Key" && classMap.TryGetCollapseKey(accessExpressionAsMember?.Member.Name, out collapseKey))
                return Expression.MakeMemberAccess(accessExpressionAsMember.Expression, accessExpressionAsMember.Expression.Type.GetRuntimeProperty(collapseKey.KeyName));
            else if (classMap.TryGetModelProperty(memberExpression.Member.Name, out propertyMap))
            {
                // We have to map through an associative table
                if(propertyMap.Via != null )
                {
                    Expression viaExpression = Expression.MakeMemberAccess(accessExpression, accessExpression.Type.GetRuntimeProperty(propertyMap.DomainName));
                    var via = propertyMap.Via;
                    while (via != null)
                    {
                        
                        MemberInfo viaMember = viaExpression.Type.GetRuntimeProperty(via.DomainName);
                        if (viaMember == null)
                            break;
                        viaExpression = Expression.MakeMemberAccess(viaExpression, viaMember);

                        if (via.OrderBy != null && viaExpression.Type.GetTypeInfo().ImplementedInterfaces.Any(o=>o == typeof(IEnumerable)))
                            viaExpression = this.CreateSortExpression(viaExpression, via);
                        if (via.Aggregate != AggregationFunctionType.None)
                            viaExpression = this.CreateAggregateFunction(viaExpression, via);
                        via = via.Via;
                    }
                    return viaExpression;
                }
                else
                    return Expression.MakeMemberAccess(accessExpression, this.ExtractDomainType(accessExpression.Type).GetRuntimeProperty(propertyMap.DomainName));
            }
            else
            {
                // look for idenical named property
                Type domainType = this.MapModelType(memberExpression.Expression.Type);

                // Get domain member and map
                MemberInfo domainMember = domainType.GetRuntimeProperty(memberExpression.Member.Name);
                if (domainMember != null)
                    return Expression.MakeMemberAccess(accessExpression, domainMember);
                else
                    throw new NotSupportedException(String.Format("Cannot find property information for {0}({1}).{2}", memberExpression.Expression, memberExpression.Expression.Type.Name, memberExpression.Member.Name));
            }
        }

        /// <summary>
        /// Create aggregation functions
        /// </summary>
        private Expression CreateAggregateFunction(Expression viaExpression, PropertyMap via)
        {
            var aggregateMethod = typeof(Enumerable).GetGenericMethod(via.Aggregate.ToString(),
               new Type[] { viaExpression.Type.GetTypeInfo().GenericTypeArguments[0] },
               new Type[] { viaExpression.Type });
            return Expression.Call(aggregateMethod as MethodInfo, viaExpression);

        }

        /// <summary>
        /// Create sort expression
        /// </summary>
        private Expression CreateSortExpression(Expression viaExpression, PropertyMap via)
        {
            // Get sort property
            var sortProperty = viaExpression.Type.GenericTypeArguments[0].GetRuntimeProperty(via.OrderBy);
            Type predicateType = typeof(Func<,>).MakeGenericType(viaExpression.Type.GetTypeInfo().GenericTypeArguments[0], sortProperty.PropertyType);
            var sortMethod = typeof(Enumerable).GetGenericMethod(via.SortOrder.ToString(),
                new Type[] { viaExpression.Type.GetTypeInfo().GenericTypeArguments[0], sortProperty.PropertyType },
                new Type[] { viaExpression.Type, predicateType });

            // Get builder methods
            var sortParameter = Expression.Parameter(viaExpression.Type.GetTypeInfo().GenericTypeArguments[0], "sort");
            var builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { predicateType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
            var sortLambda = builderMethod.Invoke(null, new object[] { Expression.MakeMemberAccess(sortParameter, sortProperty), new ParameterExpression[] { sortParameter } }) as Expression;
            return Expression.Call(sortMethod as MethodInfo, viaExpression, sortLambda);
        }

        /// <summary>
        /// Extracts a domain type from a generic if needed
        /// </summary>
        public Type ExtractDomainType(Type domainType)
        {
            if (!domainType.IsConstructedGenericType) return domainType;
            else if (domainType.GenericTypeArguments.Length == 1)
                return this.ExtractDomainType(domainType.GenericTypeArguments[0]);
            else
                throw new InvalidOperationException("Cannot determine domain model type");
        }

        /// <summary>
        /// Gets the domain type for the specified model type
        /// </summary>
        public Type MapModelType(Type modelType)
        {
            ClassMap classMap = this.m_mapFile.GetModelClassMap(modelType);
            if (classMap == null)
                return modelType;
            Type domainType = Type.GetType(classMap.DomainClass);
            if (domainType == null)
                throw new InvalidOperationException(String.Format("Cannot find class {0}", classMap.DomainClass));
            return domainType;
        }

        /// <summary>
        /// Create a traversal expression for a lambda expression
        /// </summary>
        public Expression CreateLambdaMemberAdjustmentExpression(MemberExpression rootExpression, ParameterExpression lambdaParameterExpression)
        {
            ClassMap classMap = this.m_mapFile.GetModelClassMap(this.ExtractDomainType(rootExpression.Expression.Type));

            if (classMap == null)
                return lambdaParameterExpression;

            // Expression is the same class? Collapse if it is a key
            PropertyMap propertyMap = null;
            classMap.TryGetModelProperty(rootExpression.Member.Name, out propertyMap);

            // Is there a VIA that we need to express?
            if (propertyMap.Via != null)
            {
                Expression viaExpression = lambdaParameterExpression;
                var via = propertyMap.Via;
                while (via != null)
                {

                    MemberInfo viaMember = viaExpression.Type.GetRuntimeProperty(via.DomainName);
                    if (viaMember != null)
                        viaExpression = Expression.MakeMemberAccess(viaExpression, viaMember);
                    via = via.Via;
                }
                return viaExpression;
            }
            else
                return lambdaParameterExpression;


        }

        /// <summary>
        /// Convert the specified lambda expression from model into query
        /// </summary>
        /// <param name="expression">The expression to be converted</param>
        public Expression<Func<TTo, bool>> MapModelExpression<TFrom, TTo>(Expression<Func<TFrom, bool>> expression)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(TTo), expression.Parameters[0].Name);
                Expression expr = new ModelExpressionVisitor(this, parameter).Visit(expression.Body);
                var retVal = Expression.Lambda<Func<TTo, bool>>(expr, parameter);
                //Debug.WriteLine("Map Expression: {0} > {1}", expression, retVal);
                return retVal;
            }
            catch(Exception e)
            {
                //Debug.WriteLine("Error converting {0}. {1}", expression, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Map model instance
        /// </summary>
        public TDomain MapModelInstance<TModel, TDomain>(TModel modelInstance) where TDomain : new()
        {
            ClassMap classMap = this.m_mapFile.GetModelClassMap(typeof(TModel), typeof(TDomain));
            if (classMap == null)
                classMap = this.m_mapFile.GetModelClassMap(typeof(TModel));

            if (classMap == null || modelInstance == null)
                return default(TDomain);

            // Now the property maps
            TDomain retVal = new TDomain();
            foreach(var propInfo in typeof(TModel).GetRuntimeProperties())
            {

                // Property info
                if (propInfo.GetCustomAttribute<DelayLoadAttribute>() != null ||
                    propInfo.GetValue(modelInstance) == null)
                    continue;

                if (!propInfo.PropertyType.GetTypeInfo().IsPrimitive && propInfo.PropertyType != typeof(Guid) &&
                    (!propInfo.PropertyType.GetTypeInfo().IsGenericType || propInfo.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) &&
                    propInfo.PropertyType != typeof(String) &&
                    propInfo.PropertyType != typeof(DateTime) &&
                    propInfo.PropertyType != typeof(DateTimeOffset) &&
                    propInfo.PropertyType != typeof(Type) &&
                    propInfo.PropertyType != typeof(Decimal) &&
					propInfo.PropertyType != typeof(byte[]) &&
                    !propInfo.PropertyType.GetTypeInfo().IsEnum)
                    continue;

                // Map property
                PropertyMap propMap = null;
                classMap.TryGetModelProperty(propInfo.Name, out propMap);
                PropertyInfo domainProperty = null;
                Object targetObject = retVal;

                // Set 
                if (propMap == null)
                    domainProperty = typeof(TDomain).GetRuntimeProperty(propInfo.Name);
                else
                    domainProperty = typeof(TDomain).GetRuntimeProperty(propMap.DomainName);

                object domainValue = null;
                // Set value
                if (domainProperty == null)
                    continue;
					//Debug.WriteLine ("Unmapped property ({0}).{1}", typeof(TModel).Name, propInfo.Name);
				else if (domainProperty.PropertyType == typeof(byte[]) && propInfo.PropertyType == typeof(Guid))
					domainProperty.SetValue (targetObject, ((Guid)propInfo.GetValue (modelInstance)).ToByteArray ());
				else if (
					(domainProperty.PropertyType == typeof(DateTime) || domainProperty.PropertyType == typeof(DateTime?))
					&& (propInfo.PropertyType == typeof(DateTimeOffset) || propInfo.PropertyType == typeof(DateTimeOffset?))) {
					domainProperty.SetValue (targetObject, ((DateTimeOffset)propInfo.GetValue (modelInstance)).DateTime);
				}
				else if (domainProperty.PropertyType.GetTypeInfo().IsAssignableFrom(propInfo.PropertyType.GetTypeInfo()))
                    domainProperty.SetValue(targetObject, propInfo.GetValue(modelInstance));
                else if (propInfo.PropertyType == typeof(Type) && domainProperty.PropertyType == typeof(String))
                    domainProperty.SetValue(targetObject, (propInfo.GetValue(modelInstance) as Type).AssemblyQualifiedName);
                else if (MapUtil.TryConvert(propInfo.GetValue(modelInstance), domainProperty.PropertyType, out domainValue))
                    domainProperty.SetValue(targetObject, domainValue);

            }

            return retVal;
        }

        /// <summary>
        /// Map model instance
        /// </summary>
        public TModel MapDomainInstance<TDomain, TModel>(TDomain domainInstance) where TModel : new()
        {
            ClassMap classMap = this.m_mapFile.GetModelClassMap(typeof(TModel));

            if (domainInstance == null)
                return default(TModel);
            else
            {
                var cType = typeof(TModel);
                while (classMap == null || !typeof(TDomain).GetTypeInfo().IsAssignableFrom(Type.GetType(classMap.DomainClass).GetTypeInfo()))
                {
                    cType = cType.GetTypeInfo().BaseType;
                    classMap = this.m_mapFile.GetModelClassMap(cType);
                } // work up the tree
            }

            // Now the property maps
            TModel retVal = new TModel();
            foreach (var modelPropertyInfo in typeof(TModel).GetRuntimeProperties())
            {

                // Map property
                PropertyMap propMap = null;
                classMap.TryGetModelProperty(modelPropertyInfo.Name, out propMap);

                if (propMap?.DontLoad == true)
                    continue;
                var propInfo = typeof(TDomain).GetRuntimeProperty(propMap?.DomainName ?? modelPropertyInfo.Name);
                if (propInfo == null)
                {
                    //Debug.WriteLine("Unmapped property ({0}).{1}", typeof(TDomain).Name, modelPropertyInfo.Name);
                    continue;
                }

                var originalValue = propInfo.GetValue(domainInstance);
                // Property info
                try
                {
                    if (originalValue == null)
                        continue;
                }
                catch(Exception e) // HACK: For some reason, some LINQ providers will return NULL on EntityReferences with no value
                {
                    Debug.WriteLine(e.ToString());
                }

                // Traversal stuff
                PropertyInfo modelProperty = null;
                object sourceObject = domainInstance;
                PropertyInfo sourceProperty = propInfo;

                // Set 
                if (propMap == null)
                    modelProperty = typeof(TModel).GetRuntimeProperty(propInfo.Name);
                else
                {
                    modelProperty = typeof(TModel).GetRuntimeProperty(propMap.ModelName);
                    // Go through the via elements in the object map. This code traces a path 
                    // through the domain class instantiating any necessary associative entity
                    // classes. Example when a model entity is really two or three tables in the DB..
                    // 🎶 Ah for just one time, I would take the northwest passage
                    // To find the hand of Franklin reaching for the Beaufort Sea.
                    // Tracing one warm line, through a land so wide and savage
                    // And make a northwest passage to the sea. 🎶
                    var via = propMap.Via;
                    List<PropertyMap> viaWalk = new List<PropertyMap>();
                    while(via?.DontLoad == false)
                    {
                        viaWalk.Add(via);
                        via = via.Via;
                    }

                    sourceProperty = propInfo;
                    foreach (var p in viaWalk.Select(o=>o))
                    {
                        if(!(sourceObject is IList))
                            sourceObject = sourceProperty.GetValue(sourceObject);
                        sourceProperty = this.ExtractDomainType(sourceProperty.PropertyType).GetRuntimeProperty(p.DomainName);
                    }
                }

                // validate property type
                if (propMap?.DontLoad == true)
                    continue;
                
                // Set value
                object pValue = null;
                if (modelProperty == null)
                    continue;
                //DebugWriteLine("Unmapped property ({0}).{1}", typeof(TDomain).Name, propInfo.Name);
                else if (modelProperty.GetCustomAttribute<DelayLoadAttribute>() != null &&
                    modelProperty.GetCustomAttribute<AutoLoadAttribute>() == null)
                    continue;
                else if (sourceProperty.PropertyType == typeof(byte[]) && modelProperty.PropertyType == typeof(Guid)) // Guid to BA
                    modelProperty.SetValue(retVal, new Guid((byte[])sourceProperty.GetValue(sourceObject)));
                else if (modelProperty.PropertyType.GetTypeInfo().IsAssignableFrom(sourceProperty.PropertyType.GetTypeInfo()))
                    modelProperty.SetValue(retVal, sourceProperty.GetValue(sourceObject));
                else if (sourceProperty.PropertyType == typeof(String) && modelProperty.PropertyType == typeof(Type))
                    modelProperty.SetValue(retVal, Type.GetType(sourceProperty.GetValue(sourceObject) as String));
                else if (MapUtil.TryConvert(originalValue, modelProperty.PropertyType, out pValue))
                    modelProperty.SetValue(retVal, pValue);
                else if (originalValue is IList)
                {
                    var modelInstance = Activator.CreateInstance(modelProperty.PropertyType) as IList;
                    modelProperty.SetValue(retVal, modelInstance);
                    var instanceMapFunction = typeof(ModelMapper).GetGenericMethod("MapDomainInstance", new Type[] { sourceProperty.PropertyType.GenericTypeArguments[0], modelProperty.PropertyType.GenericTypeArguments[0] },
                        new Type[] { sourceProperty.PropertyType.GenericTypeArguments[0] });
                    foreach (var itm in originalValue as IList)
                    {
                        // Traverse?
                        var instance = itm;
                        var via = propMap.Via;
                        while (via != null)
                        {
                            instance = instance?.GetType().GetRuntimeProperty(via.DomainName)?.GetValue(instance);
                            if (instance is IList)
                            {
                                var parm = Expression.Parameter(instance.GetType());
                                Expression aggregateExpr = null;
                                if (!String.IsNullOrEmpty(via.OrderBy))
                                    aggregateExpr = this.CreateSortExpression(parm, via);
                                aggregateExpr = this.CreateAggregateFunction(aggregateExpr, via);

                                // Get the generic method for LIST to be widdled down
                                instance = Expression.Lambda(aggregateExpr, parm).Compile().DynamicInvoke(instance);

                            }
                            via = via.Via;
                        }
                        modelInstance.Add(instanceMapFunction.Invoke(this, new object[] { instance }));
                    }
                }
                else if (originalValue is IIdentifiedEntity)
                {
                    var instanceMapFunction = typeof(ModelMapper).GetGenericMethod("MapDomainInstance", new Type[] { sourceProperty.PropertyType, modelProperty.PropertyType },
                       new Type[] { sourceProperty.PropertyType });
                    modelProperty.SetValue(retVal, instanceMapFunction.Invoke(this, new object[] { sourceProperty.GetValue(sourceObject) }));

                }
            }

            return retVal;
        }
    }
}
