using System;
using System.Collections.Generic;
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
                    throw new NotSupportedException(String.Format("Cannot find property information for {0}"));
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
            Expression expr = new ModelExpressionVisitor(this).Visit(expression.Body);
            return Expression.Lambda<Func<TTo, bool>>(expr, Expression.Parameter(typeof(TTo), expression.Parameters[0].Name));
        }
    }
}
