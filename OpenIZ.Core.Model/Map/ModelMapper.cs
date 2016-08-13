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
 * Date: 2016-6-14
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
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Map
{

    /// <summary>
    /// Represents model mapping event arguments
    /// </summary>
    public class ModelMapEventArgs : EventArgs
    {
        /// <summary>
        /// Domain object
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Identified data model object
        /// </summary>
        public IdentifiedData ModelObject { get; set; }

        /// <summary>
        /// Gets or sets the domain object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Gets or sets a cancel comand
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Model mapper
    /// </summary>
    public sealed class ModelMapper
    {

        /// <summary>
        /// Primitive types
        /// </summary>
        private static readonly HashSet<Type> primitives = new HashSet<Type>()
        {
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?),
            typeof(String),
            typeof(Guid),
            typeof(Guid?),
            typeof(Type),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(UInt32),
            typeof(UInt32?)
        };

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
        /// Fired anytime any model mapper maps to a model
        /// </summary>
        public static event EventHandler<ModelMapEventArgs> MappingToModel;
        /// <summary>
        /// Fired anytime any model mapper maps finished
        /// </summary>
        public static event EventHandler<ModelMapEventArgs> MappedToModel;

        /// <summary>
        /// Fires the pre map returning whether cancellation is necessary
        /// </summary>
        private static object FireMappingToModel(object sender, Guid key, IdentifiedData modelInstance)
        {
            ModelMapEventArgs e = new ModelMapEventArgs() { ObjectType = modelInstance.GetType(), ModelObject = modelInstance, Key = key };
            MappingToModel?.Invoke(sender, e);
            if (e.Cancel)
                return e.ModelObject;
            else
                return null;
        }

        /// <summary>
        /// Fires that a map has occurred
        /// </summary>
        private static void FireMappedToModel(object sender, Guid key, IdentifiedData modelInstance)
        {
            ModelMapEventArgs e = new ModelMapEventArgs() { ObjectType = modelInstance.GetType(), Key = key, ModelObject = modelInstance };
            MappedToModel?.BeginInvoke(sender, e, null, null);
        }

        /// <summary>
        /// Maps a cast to appropriate path
        /// </summary>
        public Expression MapTypeCast(UnaryExpression sourceExpression, Expression accessExpression)
        {

            // First we find the map for the specified type
            ClassMap classMap = this.m_mapFile.GetModelClassMap(sourceExpression.Operand.Type);

            PropertyMap castMap = classMap.Cast?.Find(o => o.ModelType == sourceExpression.Type);
            if (castMap == null) throw new InvalidCastException();
            Expression accessExpr = Expression.MakeMemberAccess(accessExpression, accessExpression.Type.GetRuntimeProperty(castMap.DomainName));
            while(castMap.Via != null)
            {
                castMap = castMap.Via;
                accessExpr = Expression.MakeMemberAccess(accessExpr, accessExpr.Type.GetRuntimeProperty(castMap.DomainName));
            }
            return accessExpr;

        }

        /// <summary>
        /// Map member 
        /// </summary>
        public Expression MapModelMember(MemberExpression memberExpression, Expression accessExpression, Type modelType = null)
        {

            ClassMap classMap = this.m_mapFile.GetModelClassMap(modelType ?? memberExpression.Expression.Type);

            if (classMap == null)
                return accessExpression;

            // Expression is the same class? Collapse if it is a key
            MemberExpression accessExpressionAsMember = accessExpression as MemberExpression;
            CollapseKey collapseKey = null;
            PropertyMap propertyMap = null;

            if (memberExpression.Member.Name == "Key" && classMap.TryGetCollapseKey(accessExpressionAsMember?.Member.Name, out collapseKey))
                return Expression.MakeMemberAccess(accessExpressionAsMember.Expression, accessExpressionAsMember.Expression.Type.GetRuntimeProperty(collapseKey.KeyName));
            else if (classMap.TryGetModelProperty(memberExpression.Member.Name, out propertyMap))
            {
                // We have to map through an associative table
                if (propertyMap.Via != null)
                {
                    Expression viaExpression = Expression.MakeMemberAccess(accessExpression, accessExpression.Type.GetRuntimeProperty(propertyMap.DomainName));
                    var via = propertyMap.Via;
                    while (via != null)
                    {

                        MemberInfo viaMember = viaExpression.Type.GetRuntimeProperty(via.DomainName);
                        if (viaMember == null)
                            break;
                        viaExpression = Expression.MakeMemberAccess(viaExpression, viaMember);

                        if (via.OrderBy != null && viaExpression.Type.GetTypeInfo().ImplementedInterfaces.Any(o => o == typeof(IEnumerable)))
                            viaExpression = viaExpression.Sort(via.OrderBy, via.SortOrder);
                        if (via.Aggregate != AggregationFunctionType.None)
                            viaExpression = viaExpression.Aggregate(via.Aggregate);
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
                Type domainType = this.MapModelType(modelType ?? memberExpression.Expression.Type);

                // Get domain member and map
                MemberInfo domainMember = accessExpression.Type.GetRuntimeProperty(memberExpression.Member.Name);
                if (domainMember != null)
                    return Expression.MakeMemberAccess(accessExpression, domainMember);
                else
                {
                    // Try on the base? 
                    if (classMap.ParentDomainProperty != null)
                    {
                        domainMember = domainType.GetRuntimeProperty(classMap.ParentDomainProperty.DomainName);
                        return MapModelMember(memberExpression, Expression.MakeMemberAccess(accessExpression, domainMember), (modelType ?? memberExpression.Expression.Type).GetTypeInfo().BaseType);
                    }
                    else
                        throw new NotSupportedException(String.Format("Cannot find property information for {0}({1}).{2}", memberExpression.Expression, memberExpression.Expression.Type.Name, memberExpression.Member.Name));
                }
            }
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
            while (propertyMap == null && classMap != null)
            {
                classMap.TryGetModelProperty(rootExpression.Member.Name, out propertyMap);
                if (propertyMap == null)
                {
                    classMap = this.m_mapFile.GetModelClassMap(classMap.ModelType.GetTypeInfo().BaseType);
                    //                    var tDomain = rootExpression.Expression.Type.GetRuntimeProperty(classMap.ParentDomainProperty.DomainName);

                }
            }

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
#if VERBOSE_DEBUG
                Debug.WriteLine("Map Expression: {0} > {1}", expression, retVal);
#endif
                return retVal;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error converting {0}. {1}", expression, e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Map model instance
        /// </summary>
        public TDomain MapModelInstance<TModel, TDomain>(TModel modelInstance) where TDomain : new()
        {

            // Set the identity source
            IEntitySourceProvider currentProvider = null;


            ClassMap classMap = this.m_mapFile.GetModelClassMap(typeof(TModel), typeof(TDomain));
            if (classMap == null)
                classMap = this.m_mapFile.GetModelClassMap(typeof(TModel));

            if (classMap == null || modelInstance == null)
                return default(TDomain);

            // Now the property maps
            TDomain retVal = new TDomain();
            foreach (var propInfo in typeof(TModel).GetRuntimeProperties())
            {

                // no need to load?
                if (propInfo == null)
                    continue;
                else if (!primitives.Contains(propInfo.PropertyType) && !propInfo.PropertyType.GetTypeInfo().IsEnum)
                    continue;
                else if (propInfo.GetCustomAttribute<DataIgnoreAttribute>() != null 
                    || !propInfo.CanWrite)
                    continue;


                // Property info
                if (propInfo.GetValue(modelInstance) == null)
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
                    domainProperty.SetValue(targetObject, ((Guid)propInfo.GetValue(modelInstance)).ToByteArray());
                else if (
                    (domainProperty.PropertyType == typeof(DateTime) || domainProperty.PropertyType == typeof(DateTime?))
                    && (propInfo.PropertyType == typeof(DateTimeOffset) || propInfo.PropertyType == typeof(DateTimeOffset?)))
                {
                    domainProperty.SetValue(targetObject, ((DateTimeOffset)propInfo.GetValue(modelInstance)).DateTime);
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
        public TModel MapDomainInstance<TDomain, TModel>(TDomain domainInstance, HashSet<Guid> keyStack = null) where TModel : new()
        {

            ClassMap classMap = this.m_mapFile.GetModelClassMap(typeof(TModel), typeof(TDomain));

            if (domainInstance == null)
                return default(TModel);
            else
            {
                var cType = typeof(TModel);
                while (cType != null && classMap == null || !typeof(TDomain).GetTypeInfo().IsAssignableFrom(Type.GetType(classMap.DomainClass).GetTypeInfo()))
                {
                    cType = cType.GetTypeInfo().BaseType;
                    classMap = this.m_mapFile.GetModelClassMap(cType);
                } // work up the tree
            }

            // Now the property maps
            TModel retVal = new TModel();

            // Key?
            if (classMap == null)
                return retVal;

            // Cache lookup
            var idEnt = retVal as IIdentifiedEntity;
            PropertyMap iKeyMap = null;
            if (idEnt != null)
                classMap.TryGetModelProperty("Key", out iKeyMap);
            if (iKeyMap != null)
            {
                object keyValue = typeof(TDomain).GetRuntimeProperty(iKeyMap.DomainName).GetValue(domainInstance);
                while (iKeyMap.Via != null)
                {
                    keyValue = keyValue.GetType().GetRuntimeProperty(iKeyMap.Via.DomainName).GetValue(keyValue);
                    iKeyMap = iKeyMap.Via;
                }
                if (keyValue is byte[])
                    keyValue = new Guid(keyValue as byte[]);

                // Set key vaue
                idEnt.Key = (Guid)keyValue;

                var cache = FireMappingToModel(this, (Guid)keyValue, retVal as IdentifiedData);
                if (cache != null)
                    return (TModel)cache;
            }

            // Are we currently processing this?
            if (idEnt != null)
            {
                if (keyStack == null)
                    keyStack = new HashSet<Guid>();
                if (keyStack.Contains(idEnt.Key.Value))
                    return default(TModel);
                else
                    keyStack.Add(idEnt.Key.Value);
            }

            // Iterate the properties and map
            foreach (var modelPropertyInfo in typeof(TModel).GetRuntimeProperties())
            {

                // no need to load?
                if (modelPropertyInfo == null)
                    continue;
                else if (modelPropertyInfo.GetCustomAttribute<DataIgnoreAttribute>() != null ||
                    modelPropertyInfo.GetCustomAttribute<AutoLoadAttribute>() == null &&
                    !primitives.Contains(modelPropertyInfo.PropertyType) && !modelPropertyInfo.PropertyType.GetTypeInfo().IsEnum 
                    || !modelPropertyInfo.CanWrite)
                    continue;

                // Map property
                PropertyMap propMap = null;
                classMap.TryGetModelProperty(modelPropertyInfo.Name, out propMap);

                if (propMap?.DontLoad == true)
                    continue;
                var propInfo = typeof(TDomain).GetRuntimeProperty(propMap?.DomainName ?? modelPropertyInfo.Name);
                if (propInfo == null)
                {
#if VERBOSE_DEBUG
                    Debug.WriteLine("Unmapped property ({0}).{1}", typeof(TDomain).Name, modelPropertyInfo.Name);
#endif
                    continue;
                }

                var originalValue = propInfo.GetValue(domainInstance);
                // Property info
                try
                {
                    if (originalValue == null)
                        continue;
                }
                catch (Exception e) // HACK: For some reason, some LINQ providers will return NULL on EntityReferences with no value
                {
                    Debug.WriteLine(e.ToString());
                }

                // Traversal stuff
                PropertyInfo modelProperty = propMap == null ? modelPropertyInfo : typeof(TModel).GetRuntimeProperty(propMap.ModelName); ;
                object sourceObject = domainInstance;
                PropertyInfo sourceProperty = propInfo;

                // Go through the via elements in the object map. This code traces a path 
                // through the domain class instantiating any necessary associative entity
                // classes. Example when a model entity is really two or three tables in the DB..
                // 🎶 Ah for just one time, I would take the northwest passage
                // To find the hand of Franklin reaching for the Beaufort Sea.
                // Tracing one warm line, through a land so wide and savage
                // And make a northwest passage to the sea. 🎶
                if (propMap != null)
                {
                    var via = propMap.Via;
                    List<PropertyMap> viaWalk = new List<PropertyMap>();
                    while (via?.DontLoad == false)
                    {
                        viaWalk.Add(via);
                        via = via.Via;
                    }

                    sourceProperty = propInfo;
                    foreach (var p in viaWalk.Select(o => o))
                    {
                        if (!(sourceObject is IList))
                            sourceObject = sourceProperty.GetValue(sourceObject);
                        sourceProperty = this.ExtractDomainType(sourceProperty.PropertyType).GetRuntimeProperty(p.DomainName);
                    }
                }

                // validate property type
                if (propMap?.DontLoad == true)
                    continue;

                // Set value
                object pValue = null;

                //DebugWriteLine("Unmapped property ({0}).{1}", typeof(TDomain).Name, propInfo.Name);
                if (sourceProperty.PropertyType == typeof(byte[]) && modelProperty.PropertyType == typeof(Guid)) // Guid to BA
                    modelProperty.SetValue(retVal, new Guid((byte[])sourceProperty.GetValue(sourceObject)));
                else if (modelProperty.PropertyType.GetTypeInfo().IsAssignableFrom(sourceProperty.PropertyType.GetTypeInfo()))
                    modelProperty.SetValue(retVal, sourceProperty.GetValue(sourceObject));
                else if (sourceProperty.PropertyType == typeof(String) && modelProperty.PropertyType == typeof(Type))
                    modelProperty.SetValue(retVal, Type.GetType(sourceProperty.GetValue(sourceObject) as String));
                else if (MapUtil.TryConvert(originalValue, modelProperty.PropertyType, out pValue))
                    modelProperty.SetValue(retVal, pValue);
                // Handles when a map is a list for example doing a VIA over a version relationship
                else if (originalValue is IList)
                {
                    var modelInstance = Activator.CreateInstance(modelProperty.PropertyType) as IList;
                    modelProperty.SetValue(retVal, modelInstance);
                    var instanceMapFunction = typeof(ModelMapper).GetGenericMethod("MapDomainInstance", new Type[] { sourceProperty.PropertyType.GetTypeInfo().GenericTypeArguments[0], modelProperty.PropertyType.GetTypeInfo().GenericTypeArguments[0] },
                        new Type[] { sourceProperty.PropertyType.GetTypeInfo().GenericTypeArguments[0], typeof(HashSet<Guid>) });
                    foreach (var itm in originalValue as IList)
                    {
                        // Traverse?
                        var instance = itm;
                        var via = propMap?.Via;
                        while (via != null)
                        {
                            instance = instance?.GetType().GetRuntimeProperty(via.DomainName)?.GetValue(instance);
                            if (instance is IList)
                            {
                                var parm = Expression.Parameter(instance.GetType());
                                Expression aggregateExpr = parm;
                                if (!String.IsNullOrEmpty(via.OrderBy))
                                    aggregateExpr = parm.Sort(via.OrderBy, via.SortOrder);
                                aggregateExpr = aggregateExpr.Aggregate(via.Aggregate);

                                // Get the generic method for LIST to be widdled down
                                instance = Expression.Lambda(aggregateExpr, parm).Compile().DynamicInvoke(instance);
                                
                            }
                            via = via.Via;
                        }
                        modelInstance.Add(instanceMapFunction.Invoke(this, new object[] { instance, keyStack }));
                    }
                }
                // Flat map list 1..1
                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(modelProperty.PropertyType.GetTypeInfo()) &&
                    typeof(IList).GetTypeInfo().IsAssignableFrom(sourceProperty.PropertyType.GetTypeInfo()))
                {
                    var modelInstance = Activator.CreateInstance(modelProperty.PropertyType) as IList;
                    modelProperty.SetValue(retVal, modelInstance);
                    var instanceMapFunction = typeof(ModelMapper).GetGenericMethod("MapDomainInstance", new Type[] { sourceProperty.PropertyType.GenericTypeArguments[0], modelProperty.PropertyType.GenericTypeArguments[0] },
                        new Type[] { sourceProperty.PropertyType.GenericTypeArguments[0], typeof(HashSet<Guid>) });
                    var listValue = sourceProperty.GetValue(sourceObject);

                    // Is this list a versioned association??
                    if (typeof(TDomain).GetRuntimeProperty("VersionSequenceId") != null &&
                        sourceProperty.PropertyType.GenericTypeArguments[0].GetRuntimeProperty("EffectiveVersionSequenceId") != null) // Filter!!! Yay!
                    {
                        var parm = Expression.Parameter(listValue.GetType());
                        Expression aggregateExpr = null;
                        aggregateExpr = parm.IsActive(domainInstance);
                        listValue = Expression.Lambda(aggregateExpr, parm).Compile().DynamicInvoke(listValue);
                    }

                    foreach (var itm in listValue as IEnumerable)
                        modelInstance.Add(instanceMapFunction.Invoke(this, new object[] { itm, keyStack }));
                }
                else if (m_mapFile.GetModelClassMap(modelProperty.PropertyType) != null)
                {
                    // TODO: Clean this up
                    var instance = originalValue; //sourceProperty.GetValue(sourceObject);
                    var via = propMap?.Via;
                    while (via != null)
                    {
                        instance = instance?.GetType().GetRuntimeProperty(via.DomainName)?.GetValue(instance);
                        if (instance is IList)
                        {
                            var parm = Expression.Parameter(instance.GetType());
                            Expression aggregateExpr = parm;
                            if (!String.IsNullOrEmpty(via.OrderBy))
                                aggregateExpr = parm.Sort(via.OrderBy, via.SortOrder);
                            aggregateExpr = aggregateExpr.Aggregate(via.Aggregate);

                            // Get the generic method for LIST to be widdled down
                            instance = Expression.Lambda(aggregateExpr, parm).Compile().DynamicInvoke(instance);

                        }
                        via = via.Via;
                    }
                    var instanceMapFunction = typeof(ModelMapper).GetGenericMethod("MapDomainInstance", new Type[] { instance?.GetType(), modelProperty.PropertyType },
                       new Type[] { instance.GetType(), typeof(HashSet<Guid>) });
                    modelProperty.SetValue(retVal, instanceMapFunction.Invoke(this, new object[] { instance, keyStack }));

                }
            }

#if VERBOSE_DEBUG
            Debug.WriteLine("Leaving: {0}>{1}", typeof(TDomain).FullName, typeof(TModel).FullName);
#endif
            if (idEnt != null)
            {
                keyStack.Remove(idEnt.Key.Value);
                FireMappedToModel(this, idEnt.Key ?? Guid.Empty, retVal as IdentifiedData);
            }
           // (retVal as IdentifiedData).SetDelayLoad(true);

            return retVal;
        }

    }
}
