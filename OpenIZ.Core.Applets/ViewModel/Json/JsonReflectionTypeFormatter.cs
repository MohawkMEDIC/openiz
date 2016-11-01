using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model;
using System.Reflection;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model.Reflection;
using System.Collections;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents the JSON Reflection fallback serializer
    /// </summary>
    public class JsonReflectionTypeFormatter : IJsonViewModelTypeFormatter
    {
        // Current reflection formatter
        private static JsonReflectionTypeFormatter s_current;

        /// <summary>
        /// Gets the current reflection formatter
        /// </summary>
        public static JsonReflectionTypeFormatter Current
        {
            get
            {
                if (s_current == null)
                    s_current = new JsonReflectionTypeFormatter();
                return s_current;
            }
        }

        /// <summary>
        /// Gets the type that this formatter handles
        /// </summary>
        public Type HandlesType
        {
            get
            {
                return typeof(IdentifiedData);
            }
        }

        /// <summary>
        /// JSON reflection type formatter
        /// </summary>
        private JsonReflectionTypeFormatter()
        {

        }

        /// <summary>
        /// Get property name
        /// </summary>
        public String GetPropertyName(PropertyInfo info)
        {
            JsonPropertyAttribute jpa = info.GetCustomAttribute<JsonPropertyAttribute>();
            String retVal = null;
            if (jpa != null)
                retVal = jpa.PropertyName;
            else
            {
                SerializationReferenceAttribute sra = info.GetCustomAttribute<SerializationReferenceAttribute>();
                if(sra != null)
                {
                    jpa = info.DeclaringType.GetRuntimeProperty(sra.RedirectProperty).GetCustomAttribute<JsonPropertyAttribute>();
                    if (jpa != null)
                        retVal = jpa.PropertyName + "Model";
                }
            }

            if (retVal == null)
                retVal = info.Name.ToLower() + "Model";
            return retVal;
        }

        /// <summary>
        /// Serialize the specified object to the wire
        /// </summary>
        public void Serialize(JsonWriter w, IdentifiedData o, JsonSerializationContext context)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));

            // For each item in the property ... 
            foreach(var propertyInfo in o.GetType().GetRuntimeProperties())
            {
                // Get the property name
                var propertyName = this.GetPropertyName(propertyInfo);
                if (propertyName.StartsWith("$")) // Skip internal property names
                    continue;

                // Serialization decision
                if (!context.ShouldSerialize(propertyName))
                    continue;

                // Get the property 
                var value = propertyInfo.GetValue(o);
                if (value == null || (value as IList)?.Count == 0)
                    continue;

                // TODO: Classifier
                context.JsonContext.WritePropertyUtil(w, propertyName, value, context);
            }
        }

        public TModel Deserialize<TModel>(JsonReader r, JsonSerializationContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the appriopriate reflection based classifier
        /// </summary>
        internal IViewModelClassifier GetClassifier(Type type)
        {
            var classifierAtt = type.StripGeneric().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
            if (classifierAtt != null)
                return new JsonReflectionClassifier(classifierAtt);
            return null;
        }

        /// <summary>
        /// Get the simple value of the object
        /// </summary>
        public object GetSimpleValue(object value)
        {
            var simpleValueAttribute = value.GetType().GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>();
            if (simpleValueAttribute != null)
                return value.GetType().GetRuntimeProperty(simpleValueAttribute.ValueProperty).GetValue(value);
            return null;
        }
    }
}
