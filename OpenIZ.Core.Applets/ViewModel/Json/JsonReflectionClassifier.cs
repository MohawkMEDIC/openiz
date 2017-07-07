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
using System;
using System.Collections;
using System.Collections.Generic;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model;
using System.Reflection;
using OpenIZ.Core.Model.EntityLoader;
using System.Linq.Expressions;
using System.Linq;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a simple reflection based classifier
    /// </summary>
    internal class JsonReflectionClassifier : IViewModelClassifier
    {
        // Classifier attribute
        private ClassifierAttribute m_classifierAttribute;

        // Classifier hash map
        private static Dictionary<Type, ClassifierAttribute> m_classifierCache = new Dictionary<Type, ClassifierAttribute>();

        private static Dictionary<Type, Dictionary<String, Object>> m_classifierObjectCache = new Dictionary<Type, Dictionary<string, object>>();

        // Type
        private Type m_type;

        /// <summary>
        /// Creates a new reflection based classifier
        /// </summary>
        public JsonReflectionClassifier(Type type)
        {
            this.m_type = type;
            var classifierAtt = type.StripGeneric().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
            this.m_classifierAttribute = classifierAtt;
        }

        /// <summary>
        /// Gets the type this handles
        /// </summary>
        public Type HandlesType
        {
            get
            {
                return this.m_type;
            }
        }

        /// <summary>
        /// Classify the specified data element
        /// </summary>
        public Dictionary<string, IList> Classify(IList data)
        {
            Dictionary<String, IList> retVal = new Dictionary<string, IList>();

			// copy for the enumeration check
	        var copy = new object[data.Count];

			data.CopyTo(copy, 0);

            foreach(var itm in copy)
            {
                var classifier = this.GetClassifierObj(itm, this.m_classifierAttribute);
                String classKey = classifier?.ToString() ?? "$other";

                IList group = null;
                if (!retVal.TryGetValue(classKey, out group))
                {
                    group = new List<Object>();
                    retVal.Add(classKey, group);
                }
                group.Add(itm);
            }
            return retVal;
        }

        /// <summary>
        /// Perform a re-classification of values
        /// </summary>
        public IList Compose(Dictionary<string, object> values, Type retValType)
        {
            //var retValType = typeof(List<>).MakeGenericType(this.m_type);
            var retVal = Activator.CreateInstance(retValType) as IList;

            foreach (var itm in values)
            {
                PropertyInfo classifierProperty = this.m_type.GetRuntimeProperty(this.m_classifierAttribute.ClassifierProperty),
                    setProperty = classifierProperty;

                String propertyName = setProperty.Name;
                Object itmClassifier = null, target = itmClassifier;

                // Construct the classifier
                if (itm.Key != "$other")
                {
                    while (propertyName != null)
                    {
                        var classifierValue = typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(setProperty.PropertyType.GetTypeInfo()) ?
                            this.LoadClassifier(setProperty.PropertyType, itm.Key) :
                            itm.Key;

                        if (target != null)
                            setProperty.SetValue(target, classifierValue);
                        else
                            itmClassifier = target = classifierValue;

                        propertyName = setProperty.PropertyType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty;
                        if (propertyName != null)
                        {
                            setProperty = setProperty.PropertyType.GetRuntimeProperty(propertyName);
                            target = classifierValue;
                        }
                    }
                }

                // Now set the classifiers
                foreach(var inst in itm.Value as IList ?? new List<Object>() { itm.Value })
                {
                    if (inst == null) continue;

                    if(itm.Key != "$other" )
                        classifierProperty.SetValue(inst, itmClassifier);
                    retVal.Add(inst);
                }

            }

            return retVal;
        }

        /// <summary>
        /// Load classifier
        /// </summary>
        private Object LoadClassifier(Type type, string classifierValue)
        {
            Dictionary<String, Object> classValue = new Dictionary<string, object>();
            if (!m_classifierObjectCache.TryGetValue(type, out classValue))
            {
                classValue = new Dictionary<string, object>();
                lock (m_classifierObjectCache)
                    if (!m_classifierObjectCache.ContainsKey(type))
                        m_classifierObjectCache.Add(type, classValue);
            }

            Object retVal = null;
            if(!classValue.TryGetValue(classifierValue, out retVal)) { 
                var funcType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
                var exprType = typeof(Expression<>).MakeGenericType(funcType);
                var mi = typeof(IEntitySourceProvider).GetGenericMethod(nameof(IEntitySourceProvider.Query), new Type[] { type }, new Type[] { exprType });
                var classPropertyName = type.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty;
                if (classPropertyName != null)
                {
                    var rtp = type.GetRuntimeProperty(classPropertyName);
                    if (rtp != null && typeof(String) == rtp.PropertyType)
                    {
                        var parm = Expression.Parameter(type);
                        var exprBody = Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(parm, rtp), Expression.Constant(classifierValue));
                        var builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { funcType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
                        var funcExpr = builderMethod.Invoke(null, new object[] { exprBody, new ParameterExpression[] { parm } });
                        retVal = (mi.Invoke(EntitySource.Current.Provider, new object[] { funcExpr }) as IEnumerable).OfType<Object>().FirstOrDefault();
                    }
                }
                retVal = retVal ?? Activator.CreateInstance(type);
                lock (classValue)
                    if (!classValue.ContainsKey(classifierValue))
                        classValue.Add(classifierValue, retVal);
            }

            return retVal;
        }

        /// <summary>
        /// Get classifier object
        /// </summary>
        private object GetClassifierObj(object o, ClassifierAttribute classifierAttribute)
        {
            if (o == null) return null;

            var classProperty = o.GetType().GetRuntimeProperty(classifierAttribute.ClassifierProperty);
            var classifierObj = classProperty.GetValue(o);
            if (classifierObj == null)
            {
                // Force load
                var keyPropertyName = classProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty;
                if (keyPropertyName == null)
                    return null;
                var keyPropertyValue = o.GetType().GetRuntimeProperty(keyPropertyName).GetValue(o);

                // Now we want to force load!!!!
                var getValueMethod = typeof(EntitySource).GetGenericMethod("Get", new Type[] { classProperty.PropertyType }, new Type[] { typeof(Guid?) });
                classifierObj = getValueMethod.Invoke(EntitySource.Current, new object[] { keyPropertyValue });
                classProperty.SetValue(o, classifierObj);
            }

            if (classifierObj != null)
            {
                if (!m_classifierCache.TryGetValue(classifierObj.GetType(), out classifierAttribute))
                    lock (m_classifierCache)
                        if (!m_classifierCache.ContainsKey(classifierObj.GetType()))
                        {
                            classifierAttribute = classifierObj?.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                            m_classifierCache.Add(classifierObj.GetType(), classifierObj.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>());
                        }
            }

            if (classifierAttribute != null)
                return this.GetClassifierObj(classifierObj, classifierAttribute);
            return classifierObj;
        }

    }
}