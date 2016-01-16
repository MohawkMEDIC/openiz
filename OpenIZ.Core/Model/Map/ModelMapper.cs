using MARC.Everest.Connectors;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Model mapper
    /// </summary>
    public sealed class ModelMapper
    {

        // Model map source
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Core.Model.Map");

        // The map file
        private ModelMap m_mapFile;

        /// <summary>
        /// Creates a new mapper from a file
        /// </summary>
        public ModelMapper(String mapFile)
        {
            using (FileStream fs = File.OpenRead(mapFile))
            {
                this.Load(fs);
            }
        }

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
            XmlSerializer xsz = new XmlSerializer(typeof(ModelMap));
            this.m_mapFile = xsz.Deserialize(sourceStream) as ModelMap;
            var validation = this.Validate(this.m_mapFile);
            if (validation.Any(o => o.Type == ResultDetailType.Error))
                throw new ModelMapValidationException(validation);
        }

        /// <summary>
        /// Validate the map
        /// </summary>
        public IEnumerable<IResultDetail> Validate(ModelMap map)
        {
            return map.Validate();
        }
        /// <summary>
        /// Map member 
        /// </summary>
        public MemberExpression MapModelMember(MemberExpression memberExpression, Expression accessExpression)
        {
            
            ClassMap classMap = this.m_mapFile.GetModelClassMap(memberExpression.Expression.Type);

            if (classMap == null)
                return memberExpression;

            // Expression is the same class? Collapse if it is a key
            MemberExpression accessExpressionAsMember = accessExpression as MemberExpression;
            CollapseKey collapseKey = null;
            PropertyMap propertyMap = null;

            if (memberExpression.Member.Name == "Key" && classMap.TryGetCollapseKey(accessExpressionAsMember?.Member.Name, out collapseKey))
                return Expression.MakeMemberAccess(accessExpressionAsMember.Expression, accessExpressionAsMember.Expression.Type.GetProperty(collapseKey.KeyName));
            else if (classMap.TryGetModelProperty(memberExpression.Member.Name, out propertyMap))
            {
                // We have to map through an associative table
                if(propertyMap.Via != null && accessExpressionAsMember != null)
                {
                    MemberExpression viaExpression = Expression.MakeMemberAccess(accessExpression, accessExpression.Type.GetProperty(propertyMap.DomainName));
                    foreach(var via in propertyMap.Via)
                    {
                        
                        MemberInfo viaMember = this.ExtractDomainType(viaExpression.Type).GetProperty(via.DomainName);
                        if (viaMember == null)
                            throw new MissingMemberException(via.DomainName);
                        viaExpression = Expression.MakeMemberAccess(viaExpression, viaMember);
                    }
                    return viaExpression;
                }
                else
                    return Expression.MakeMemberAccess(accessExpression, this.ExtractDomainType(accessExpression.Type).GetProperty(propertyMap.DomainName));
            }
            else
            {
                // look for idenical named property
                Type domainType = this.MapModelType(memberExpression.Expression.Type);

                // Get domain member and map
                MemberInfo domainMember = domainType.GetProperty(memberExpression.Member.Name);
                if (domainMember != null)
                    return Expression.MakeMemberAccess(accessExpression, domainMember);
                else
                    throw new NotSupportedException(String.Format("Cannot find property information for {0}", memberExpression.Member.Name));
            }
        }

        /// <summary>
        /// Extracts a domain type from a generic if needed
        /// </summary>
        public Type ExtractDomainType(Type domainType)
        {
            if (!domainType.IsGenericType) return domainType;
            else if (domainType.GetGenericArguments().Length == 1)
                return this.ExtractDomainType(domainType.GetGenericArguments()[0]);
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
            ClassMap classMap = this.m_mapFile.GetModelClassMap(rootExpression.Expression.Type);

            if (classMap == null)
                return lambdaParameterExpression;

            // Expression is the same class? Collapse if it is a key
            PropertyMap propertyMap = null;
            classMap.TryGetModelProperty(rootExpression.Member.Name, out propertyMap);

            // Is there a VIA that we need to express?
            if (propertyMap.Via != null)
            {
                Expression viaExpression = lambdaParameterExpression;
                foreach (var via in propertyMap.Via)
                {

                    MemberInfo viaMember = this.ExtractDomainType(viaExpression.Type).GetProperty(via.DomainName);
                    if (viaMember == null)
                        throw new MissingMemberException(via.DomainName);
                    viaExpression = Expression.MakeMemberAccess(viaExpression, viaMember);
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
            var parameter = Expression.Parameter(typeof(TTo), expression.Parameters[0].Name);
            Expression expr = new ModelExpressionVisitor(this, parameter).Visit(expression.Body);
            var retVal = Expression.Lambda<Func<TTo, bool>>(expr, parameter); 
            this.m_traceSource.TraceInformation("Map Expression: {0} > {1}", expression, retVal);
            return retVal;
        }

        /// <summary>
        /// Map model instance
        /// </summary>
        public TDomain MapModelInstance<TModel, TDomain>(TModel modelInstance) where TDomain : new()
        {
            ClassMap classMap = this.m_mapFile.GetModelClassMap(typeof(TModel));

            if (classMap == null)
                return default(TDomain);

            // Now the property maps
            TDomain retVal = new TDomain();
            foreach(var propInfo in typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {

                // Property info
                if (propInfo.GetCustomAttribute<DelayLoadAttribute>() != null ||
                    propInfo.GetValue(modelInstance) == null)
                    continue;

                if (!propInfo.PropertyType.IsPrimitive && propInfo.PropertyType != typeof(Guid) &&
                    (!propInfo.PropertyType.IsGenericType || propInfo.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) &&
                    propInfo.PropertyType != typeof(String) &&
                    propInfo.PropertyType != typeof(DateTime) &&
                    propInfo.PropertyType != typeof(DateTimeOffset) &&
                    propInfo.PropertyType != typeof(Type) &&
                    propInfo.PropertyType != typeof(Decimal))
                    continue;

                // Map property
                PropertyMap propMap = null;
                classMap.TryGetModelProperty(propInfo.Name, out propMap);
                PropertyInfo domainProperty = null;
                Object targetObject = retVal;

                // Set 
                if (propMap == null)
                    domainProperty = typeof(TDomain).GetProperty(propInfo.Name);
                else
                {
                    domainProperty = typeof(TDomain).GetProperty(propMap.DomainName);
                    if (propMap.Via != null)
                        foreach (var prop in propMap.Via)
                        {
                            // Is the domain property null
                            var propValue = domainProperty.GetValue(targetObject);
                            if (propValue == null)
                            {
                                propValue = Activator.CreateInstance(domainProperty.PropertyType);
                                domainProperty.SetValue(targetObject, propValue);
                            }
                            targetObject = propValue;

                            domainProperty = domainProperty.PropertyType.GetProperty(propInfo.Name);
                        }
                }

                    // Set value
                if (domainProperty == null)
                    this.m_traceSource.TraceInformation("Unmapped property ({0}).{1}", typeof(TModel).Name, propInfo.Name);
                else if (domainProperty.PropertyType.IsAssignableFrom(propInfo.PropertyType))
                    domainProperty.SetValue(targetObject, propInfo.GetValue(modelInstance));
                else if (propInfo.PropertyType == typeof(Type) && domainProperty.PropertyType == typeof(String))
                    domainProperty.SetValue(targetObject, (propInfo.GetValue(modelInstance) as Type).AssemblyQualifiedName);
                else
                    domainProperty.SetValue(targetObject, MARC.Everest.Connectors.Util.FromWireFormat(propInfo.GetValue(modelInstance), domainProperty.PropertyType));

            }

            return retVal;
        }

        /// <summary>
        /// Map model instance
        /// </summary>
        public TModel MapDomainInstance<TDomain, TModel>(TDomain domainInstance) where TModel : new()
        {
            ClassMap classMap = this.m_mapFile.GetModelClassMap(typeof(TModel));

            if (classMap == null)
                return default(TModel);

            // Now the property maps
            TModel retVal = new TModel();
            foreach (var propInfo in typeof(TDomain).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {

                // Property info
                try {
                    if (propInfo.GetValue(domainInstance) == null)
                        continue;
                }
                catch(Exception e) // HACK: For some reason, some LINQ providers will return NULL on EntityReferences with no value
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                }

                // Map property
                PropertyMap propMap = null;
                classMap.TryGetDomainProperty(propInfo.Name, out propMap);
                PropertyInfo modelProperty = null;
                object sourceObject = domainInstance;
                PropertyInfo sourceProperty = propInfo;

                // Set 
                if (propMap == null)
                    modelProperty = typeof(TModel).GetProperty(propInfo.Name);
                else
                {
                    modelProperty = typeof(TModel).GetProperty(propMap.ModelName);
                    foreach(var p in propMap.Via.Select(o=>o).Reverse())
                    {
                        sourceObject = sourceProperty.GetValue(sourceObject);
                        sourceProperty = sourceProperty.PropertyType.GetProperty(p.DomainName);
                    }
                }

                // validate property type
                if (!sourceProperty.PropertyType.IsPrimitive && sourceProperty.PropertyType != typeof(Guid) &&
                    (!sourceProperty.PropertyType.IsGenericType || sourceProperty.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) &&
                    sourceProperty.PropertyType != typeof(String) &&
                    sourceProperty.PropertyType != typeof(DateTime) &&
                    sourceProperty.PropertyType != typeof(DateTimeOffset) &&
                    sourceProperty.PropertyType != typeof(Decimal))
                    continue;

                // Skip delay load props
                if (modelProperty.GetCustomAttribute<DelayLoadAttribute>() != null)
                    continue;

                // Set value
                if (modelProperty == null)
                    Trace.TraceInformation("Unmapped property ({0}).{1}", typeof(TDomain).Name, propInfo.Name);
                else if (modelProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    modelProperty.SetValue(retVal, sourceProperty.GetValue(sourceObject));
                else if (sourceProperty.PropertyType == typeof(String) && modelProperty.PropertyType == typeof(Type))
                    modelProperty.SetValue(retVal, Type.GetType(sourceProperty.GetValue(sourceObject) as String));
                else
                    modelProperty.SetValue(retVal, MARC.Everest.Connectors.Util.FromWireFormat(sourceProperty.GetValue(sourceObject), modelProperty.PropertyType));

            }

            return retVal;
        }
    }
}
