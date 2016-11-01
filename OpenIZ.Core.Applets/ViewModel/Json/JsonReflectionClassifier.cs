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

        /// <summary>
        /// Creates a new reflection based classifier
        /// </summary>
        public JsonReflectionClassifier(ClassifierAttribute classifierAtt)
        {
            this.m_classifierAttribute = classifierAtt;
        }

        /// <summary>
        /// Gets the type this handles
        /// </summary>
        public Type HandlesType
        {
            get
            {
                return typeof(IList<IdentifiedData>);
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