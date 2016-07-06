using Newtonsoft.Json;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// View model class for Applets turns the core business model objects into
    /// a more sane, flat model to be given to applets as JSON
    /// </summary>
    public class ViewModelSerializer 
    {

        /// <summary>
        /// Serializes the specified internal data 
        /// </summary>
        /// <remarks>
        /// Rules: 
        ///     - Keys are serialized as JSON object property
        ///     - Delay load are also expanded to keynameModel which is the model
        ///     - Collections are expanded to classifier class model[classifier] = value
        /// </remarks>
        public String Serialize(IdentifiedData data)
        {
            data.SetDelayLoad(false);
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    JsonWriter jwriter = new JsonTextWriter(sw);
                    // Iterate over properties
                    this.SerializeInternal(data, jwriter);

                    return sw.ToString();
                }
            }
            finally { 
                data.SetDelayLoad(true);
            }
        }

        /// <summary>
        /// Serialize the specified object
        /// </summary>
        private void SerializeInternal(IdentifiedData data, JsonWriter jwriter, HashSet<Guid> writeStack = null)
        {
            // Prevent infinite loop
            IVersionedEntity ver = data as IVersionedEntity;
            if (writeStack == null)
                writeStack = new HashSet<Guid>();
            else if (writeStack.Contains(ver?.VersionKey ?? data.Key))
            {
                jwriter.WriteNull();
                return;
            }
            writeStack.Add(ver?.VersionKey ?? data.Key);

            // Write object data
            jwriter.WriteStartObject();
            var myClassifier = data.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();

            foreach (var itm in data.GetType().GetRuntimeProperties())
            {
                // Value is null
                var value = itm.GetValue(data);
                if (value == null || itm.Name == myClassifier?.ClassifierProperty)
                    continue;

                String propertyName = itm.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                var delayLoadProperty = itm.GetCustomAttribute<DelayLoadAttribute>();
                if (!String.IsNullOrEmpty(delayLoadProperty?.KeyPropertyName))
                    propertyName = data.GetType().GetRuntimeProperty(delayLoadProperty?.KeyPropertyName).GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName + "Model";
                else if (propertyName == null && value is IdentifiedData)
                    propertyName = String.Format("{0}Model", itm.Name.ToLower());
                else if (propertyName == null)
                    continue;

                jwriter.WritePropertyName(propertyName);

                if (value is IList)
                {
                    // TODO: What if the object has classifiers?
                    var elementType = itm.PropertyType.GenericTypeArguments[0];
                    var classifierAttribute = elementType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                    if (classifierAttribute == null) // No classifier
                    {
                        jwriter.WriteStartArray();
                        foreach (var litm in value as IList)
                        {
                            if (litm is IdentifiedData)
                                this.SerializeInternal(litm as IdentifiedData, jwriter, writeStack);
                            else
                                jwriter.WriteValue(value);
                        }
                        jwriter.WriteEndArray();
                    }
                    else
                    {
                        // Classifier value 
                        jwriter.WriteStartObject();
                        
                        // Group the clasisfied groups 
                        Dictionary<String, List<Object>> classifiedGroups = new Dictionary<String, List<Object>>();
                        foreach (var litm in value as IList)
                        {
                            var classifierObj = elementType.GetRuntimeProperty(classifierAttribute.ClassifierProperty).GetValue(litm);
                            var classifierPath = classifierObj?.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                            while(classifierPath != null)
                            {
                                classifierObj = classifierObj.GetType().GetRuntimeProperty(classifierPath.ClassifierProperty).GetValue(classifierObj);
                                classifierPath = classifierObj?.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                            }

                            // Classifier obj becomes the dictionary object
                            List<Object> values = null;
                            String classKey = classifierObj?.ToString() ?? "Other";
                            if(!classifiedGroups.TryGetValue(classKey, out values))
                            {
                                values = new List<object>();
                                classifiedGroups.Add(classKey , values);
                            }

                            values.Add(litm);
                        }

                        // Write classified groups
                        foreach(var kv in classifiedGroups)
                        {
                            jwriter.WritePropertyName(kv.Key);
                            jwriter.WriteStartArray();
                            foreach (var vitm in kv.Value)
                            {
                                // Does the list item expose a simple value property?
                                var simpleAttribute = vitm.GetType().GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>();
                                var svalue = vitm;
                                if (simpleAttribute != null)
                                    svalue = vitm.GetType().GetRuntimeProperty(simpleAttribute.ValueProperty).GetValue(vitm);

                                if (svalue is IdentifiedData)
                                    this.SerializeInternal(vitm as IdentifiedData, jwriter, writeStack);
                                else
                                    jwriter.WriteValue(svalue);
                            }
                            jwriter.WriteEndArray();
                        }

                        jwriter.WriteEndObject();
                    }
                }
                else if (value is IdentifiedData)
                    this.SerializeInternal(value as IdentifiedData, jwriter, writeStack);
                else
                    jwriter.WriteValue(value);
            }

            writeStack.Remove(ver?.VersionKey ?? data.Key);
            jwriter.WriteEndObject();

        }
    }
}
