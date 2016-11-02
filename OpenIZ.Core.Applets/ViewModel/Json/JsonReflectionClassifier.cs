using System;
using System.Collections;
using System.Collections.Generic;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model;
using System.Reflection;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Reflection;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a simple reflection based classifier
    /// </summary>
    internal class JsonReflectionClassifier : IViewModelClassifier
    {
        // Classifier attribute
        private ClassifierAttribute m_classifierAttribute;

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
            foreach(var itm in data)
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
        public IList Compose(Dictionary<string, object> values)
        {
            var retValType = typeof(List<>).MakeGenericType(this.m_type);
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
                            Activator.CreateInstance(setProperty.PropertyType) :
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

            classifierAttribute = classifierObj?.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
            if (classifierAttribute != null)
                return this.GetClassifierObj(classifierObj, classifierAttribute);
            return classifierObj;
        }

    }
}