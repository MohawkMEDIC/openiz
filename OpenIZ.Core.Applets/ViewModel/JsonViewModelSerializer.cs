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
 * Date: 2016-7-18
 */
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
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Model.Serialization;
using OpenIZ.Core.Model.Reflection;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// View model class for Applets turns the core business model objects into
    /// a more sane, flat model to be given to applets as JSON
    /// </summary>
    public static class JsonViewModelSerializer
    {

        // Binder
        private static ModelSerializationBinder s_binder = new ModelSerializationBinder();

        /// <summary>
        /// Serializes the specified internal data 
        /// </summary>
        /// <remarks>
        /// Rules: 
        ///     - Keys are serialized as JSON object property
        ///     - Delay load are also expanded to keynameModel which is the model
        ///     - Collections are expanded to classifier class model[classifier] = value
        /// </remarks>
        public static String Serialize(IdentifiedData data)
        {
            using (StringWriter sw = new StringWriter())
            {
                JsonWriter jwriter = new JsonTextWriter(sw);
                // Iterate over properties
                SerializeInternal(data, jwriter);

                return sw.ToString();
            }
        }

        /// <summary>
        /// Parses the specified json string into <typeparamref name="TModel"/>
        /// </summary>
        public static TModel DeSerialize<TModel>(String jsonString) where TModel : IdentifiedData, new()
        {

            using (StringReader sr = new StringReader(jsonString))
            {
                JsonReader jreader = new JsonTextReader(sr);

                // Iterate over the object properties
                while (jreader.TokenType != JsonToken.StartObject && jreader.Read()) ;

                return DeSerializeInternal(jreader, typeof(TModel)) as TModel;
            }

        }

        /// <summary>
        /// Strips any nullable typing
        /// </summary>
        private static Type StripNullable(Type t)
        {
            if (t.GetTypeInfo().IsGenericType &&
                t.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                return t.GetTypeInfo().GenericTypeArguments[0];
            return t;
        }

        /// <summary>
        /// Perform the work of de-serializing
        /// </summary>
        private static IdentifiedData DeSerializeInternal(JsonReader jreader, Type serializationType)
        {
            // Must be at start object
            if (jreader.TokenType != JsonToken.StartObject)
                throw new InvalidDataException();

            // Ctor object
            object retVal = Activator.CreateInstance(serializationType);
            int depth = jreader.Depth;

            // Reader
            while (jreader.Read())
            {
                // Break condition
                if (jreader.TokenType == JsonToken.EndObject && jreader.Depth == depth) break;

                switch (jreader.TokenType)
                {
                    case JsonToken.PropertyName:
                        // Find the property 
                        var propertyInfo = serializationType.GetRuntimeProperties().FirstOrDefault(o => o.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == (String)jreader.Value);
                        if (propertyInfo == null)
                            propertyInfo = serializationType.GetRuntimeProperties().FirstOrDefault(o =>o.GetCustomAttribute<SerializationReferenceAttribute>() != null && serializationType.GetRuntimeProperty(o.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty)?.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName + "Model" == (String)jreader.Value);
                        if (propertyInfo == null )
                            propertyInfo = serializationType.GetRuntimeProperties().FirstOrDefault(o => o.Name.ToLower() + "Model" == (String)jreader.Value);
                        jreader.Read();


                        // Token type switch
                        switch (jreader.TokenType)
                        {
                            case JsonToken.StartObject:
                                // This may be a plain object or 
                                if (propertyInfo == null)
                                {
                                    int cDepth = jreader.Depth;
                                    while (jreader.Read()) // Read object;
                                        if (cDepth == jreader.Depth && jreader.TokenType == JsonToken.EndObject)
                                            break;
                                }
                                else if (typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType.GetTypeInfo()))
                                {
                                    var value = DeSerializeInternal(jreader, propertyInfo.PropertyType);

                                    if (propertyInfo.GetCustomAttribute<DataIgnoreAttribute>() != null &&
                                        propertyInfo.GetCustomAttribute<JsonPropertyAttribute>() == null)
                                        continue; // skip

                                    propertyInfo.SetValue(retVal, value);
                                }
                                else if (typeof(IList).GetTypeInfo().IsAssignableFrom(propertyInfo?.PropertyType.GetTypeInfo()))
                                {
                                    int cDepth = jreader.Depth;
                                    var modelInstance = Activator.CreateInstance(propertyInfo.PropertyType);
                                    var elementType = propertyInfo.PropertyType.GetTypeInfo().GenericTypeArguments[0];
                                    // Construct the classifier type
                                    var classifierProperty = elementType.GetRuntimeProperty(elementType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>().ClassifierProperty);

                                    while (jreader.Read()) // Read object;
                                    {
                                        if (cDepth == jreader.Depth && jreader.TokenType == JsonToken.EndObject)
                                            break;

                                        switch (jreader.TokenType)
                                        {
                                            case JsonToken.PropertyName:
                                                String classifier = (String)jreader.Value;

                                                // Consume the start array
                                                //
                                                while (jreader.Read() && jreader.TokenType != JsonToken.StartArray && jreader.TokenType != JsonToken.StartObject) ;

                                                bool isArray = jreader.TokenType == JsonToken.StartArray;
                                                // Read inner array
                                                int iDepth = jreader.Depth;

                                                while (isArray && jreader.Read() || !isArray)
                                                {
                                                    
                                                    if (iDepth == jreader.Depth && (jreader.TokenType == JsonToken.EndArray || jreader.TokenType == JsonToken.EndObject || jreader.TokenType == JsonToken.None))
                                                        break;

                                                    // Construct the object
                                                    Object contained = null;
                                                    if (jreader.TokenType == JsonToken.StartObject)
                                                        contained = DeSerializeInternal(jreader, elementType);
                                                    else // simple value
                                                    {
                                                        contained = Activator.CreateInstance(elementType);
                                                        // Get the simple value property
                                                        PropertyInfo simpleProperty = null;
                                                        if(elementType.GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>() != null)
                                                            simpleProperty = elementType.GetRuntimeProperty(elementType.GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>().ValueProperty);

                                                        if (simpleProperty != null && StripNullable(simpleProperty.PropertyType) == typeof(Guid))
                                                            simpleProperty.SetValue(contained, Guid.Parse((String)jreader.Value));
                                                        else if (simpleProperty != null && simpleProperty.PropertyType == typeof(byte[]))
                                                        {
                                                            switch (jreader.TokenType)
                                                            {
                                                                case JsonToken.Boolean:
                                                                    simpleProperty.SetValue(contained, new BooleanExtensionHandler().Serialize(jreader.Value));
                                                                    break;
                                                                case JsonToken.String:
                                                                    simpleProperty.SetValue(contained, new StringExtensionHandler().Serialize(jreader.Value));
                                                                    break;
                                                                case JsonToken.Integer:
                                                                case JsonToken.Float:
                                                                    simpleProperty.SetValue(contained, new DecimalExtensionHandler().Serialize(Convert.ToDecimal(jreader.Value)));
                                                                    break;
                                                                default:
                                                                    simpleProperty.SetValue(contained, jreader.Value);
                                                                    break;

                                                            }
                                                        }
                                                        else if (simpleProperty != null)
                                                            simpleProperty.SetValue(contained, jreader.Value);
                                                        else
                                                            throw new InvalidOperationException(String.Format("Cannot set value of {0} as simple property", elementType));
                                                    }

                                                    // Set the classifier
                                                    if (classifier != "$other")
                                                    {
                                                        var setProperty = classifierProperty;
                                                        var target = contained;
                                                        String propertyName = classifierProperty.Name;
                                                        while (propertyName != null)
                                                        {
                                                            var classifierValue = typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(setProperty.PropertyType.GetTypeInfo()) ?
                                                                Activator.CreateInstance(setProperty.PropertyType) :
                                                                classifier;
                                                            setProperty.SetValue(target, classifierValue);
                                                            propertyName = setProperty.PropertyType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty;
                                                            if (propertyName != null)
                                                            {
                                                                setProperty = setProperty.PropertyType.GetRuntimeProperty(propertyName);
                                                                target = classifierValue;
                                                            }
                                                        }
                                                    }

                                                // Add the container to the object
                                                (modelInstance as IList).Add(contained);


                                                }
                                                break;
                                        }
                                    }

                                    if (propertyInfo.GetCustomAttribute<DataIgnoreAttribute>() != null &&
                                        propertyInfo.GetCustomAttribute<JsonPropertyAttribute>() == null)
                                        continue; // skip

                                    propertyInfo.SetValue(retVal, modelInstance);
                                }
                                break;
                            case JsonToken.StartArray:
                                {
                                    if (propertyInfo == null) // Skip
                                    {
                                        int cDepth = jreader.Depth;
                                        while (jreader.Read()) // Read object;
                                            if (cDepth == jreader.Depth && jreader.TokenType == JsonToken.EndArray)
                                                break;
                                    }
                                    else
                                    { // Iterate over the array and add each element to the instance.
                                        var modelInstance = Activator.CreateInstance(propertyInfo.PropertyType);
                                        var elementType = propertyInfo.PropertyType.GetTypeInfo().GenericTypeArguments[0];
                                        int cDepth = jreader.Depth;
                                        while (jreader.Read()) // Read object;
                                        {
                                            if (cDepth == jreader.Depth && jreader.TokenType == JsonToken.EndArray)
                                                break;
                                            (modelInstance as IList).Add(DeSerializeInternal(jreader, elementType));
                                        }


                                        if (propertyInfo.GetCustomAttribute<DataIgnoreAttribute>() != null &&
                                            propertyInfo.GetCustomAttribute<JsonPropertyAttribute>() == null)
                                            continue; // skip

                                        propertyInfo.SetValue(retVal, modelInstance);
                                    }
                                }
                                break;
                            default: // Simple values
                                if(propertyInfo == null)
                                    continue;
                                else if (propertyInfo.Name == nameof(IdentifiedData.Type)) // Type switch
                                {
                                    var xsiType = s_binder.BindToType(retVal.GetType().GetTypeInfo().Assembly.FullName, (String)jreader.Value);
                                    if (xsiType != retVal.GetType())
                                    {
                                        var nRetVal = Activator.CreateInstance(xsiType);
                                        nRetVal.CopyObjectData(retVal);
                                        retVal = nRetVal;
                                        serializationType = xsiType;
                                    }
                                }
                                else if (propertyInfo == null)
                                    continue;
                                else if (jreader.TokenType == JsonToken.Null)
                                    break;
                                else if (propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(jreader.ValueType.GetTypeInfo()))
                                    propertyInfo.SetValue(retVal, jreader.Value);
                                else
                                    switch (jreader.TokenType)
                                    {
                                        case JsonToken.Integer:
                                            if (StripNullable(propertyInfo.PropertyType).GetTypeInfo().IsEnum)
                                                propertyInfo.SetValue(retVal, Enum.ToObject(StripNullable(propertyInfo.PropertyType), jreader.Value));
                                            //else if (StripNullable(propertyInfo.PropertyType) == typeof(Int32) ||
                                            //    StripNullable(propertyInfo.PropertyType) == typeof(UInt32))
                                            else
                                                propertyInfo.SetValue(retVal, JsonConvert.DeserializeObject(jreader.Value.ToString(), propertyInfo.PropertyType));

                                            break;
                                        case JsonToken.Date:
                                            if (StripNullable(propertyInfo.PropertyType) == typeof(DateTime))
                                                propertyInfo.SetValue(retVal, (DateTime)jreader.Value);
                                            else if (propertyInfo.PropertyType == typeof(String))
                                                propertyInfo.SetValue(retVal, ((DateTime)jreader.Value).ToString("o"));
                                            else
                                                propertyInfo.SetValue(retVal, (DateTimeOffset)jreader.Value);
                                            break;
                                        case JsonToken.Float:
                                            if (StripNullable(propertyInfo.PropertyType) == typeof(Decimal))
                                                propertyInfo.SetValue(retVal, Convert.ToDecimal(jreader.Value));
                                            else
                                                propertyInfo.SetValue(retVal, (Double)jreader.Value);
                                            break;
                                        case JsonToken.String:

                                            var klu = propertyInfo.PropertyType.GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();
                                            if (StripNullable(propertyInfo.PropertyType) == typeof(Guid))
                                                propertyInfo.SetValue(retVal, Guid.Parse((String)jreader.Value));
                                            else if (klu == null) // not a key
                                              propertyInfo.SetValue(retVal, JsonConvert.DeserializeObject((String)jreader.Value, propertyInfo.PropertyType));
                                            else
                                            {
                                                var scopedInstance = Activator.CreateInstance(propertyInfo.PropertyType);
                                                // Set the propoerty
                                                propertyInfo.PropertyType.GetRuntimeProperty(klu.UniqueProperty).SetValue(scopedInstance, jreader.Value);
                                                propertyInfo.SetValue(retVal, scopedInstance);
                                            }
                                            
                                            break;

                                    }

                                break;
                        }

                        break; // property name
                }
            }

            // Return parsed and expanded object
            return retVal as IdentifiedData;
        }

        /// <summary>
        /// Serialize the specified object
        /// </summary>
        private static void SerializeInternal(IdentifiedData data, JsonWriter jwriter, HashSet<Guid> writeStack = null)
        {
            // Prevent infinite loop
            IVersionedEntity ver = data as IVersionedEntity;
            if (writeStack == null)
                writeStack = new HashSet<Guid>();
            else if (data.Key.HasValue && writeStack.Contains(ver?.VersionKey ?? data.Key.Value))
            {
                jwriter.WriteNull();
                return;
            }

            if (data.Key.HasValue || ver?.VersionKey.HasValue == true)
                writeStack.Add(ver?.VersionKey ?? data.Key.Value);

            // Write object data
            jwriter.WriteStartObject();
            jwriter.WritePropertyName("$type");
            jwriter.WriteValue(data.Type);
            var myClassifier = data.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
            foreach (var itm in data.GetType().GetRuntimeProperties().Where(p => p.CanRead && p.CanWrite))
            {
                // Truly ignore this? No JsonProperty and a DataIgnore?
                if (itm.GetCustomAttribute<DataIgnoreAttribute>() != null && itm.GetCustomAttribute<JsonPropertyAttribute>() == null)
                    continue; 
                // Value is null
                var value = itm.GetValue(data);
                if (value == null || (itm.Name == myClassifier?.ClassifierProperty && value is IdentifiedData))
                    continue;

                String propertyName = itm.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                var delayLoadProperty = itm.GetCustomAttribute<SerializationReferenceAttribute>();
                if (!String.IsNullOrEmpty(delayLoadProperty?.RedirectProperty))
                    propertyName = data.GetType().GetRuntimeProperty(delayLoadProperty?.RedirectProperty).GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName + "Model";
                else if (propertyName == null && value is IdentifiedData)
                    propertyName = String.Format("{0}Model", itm.Name.ToLower());
                else if (propertyName == null || propertyName == "$type")
                    continue;
                else if (value is IList && (value as IList).Count == 0)
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
                                SerializeInternal(litm as IdentifiedData, jwriter, writeStack);
                            else
                                jwriter.WriteValue(litm);
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
                            while (classifierPath != null)
                            {
                                classifierObj = classifierObj.GetType().GetRuntimeProperty(classifierPath.ClassifierProperty).GetValue(classifierObj);
                                classifierPath = classifierObj?.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                            }

                            // Classifier obj becomes the dictionary object
                            List<Object> values = null;
                            String classKey = classifierObj?.ToString() ?? "$other";
                            if (!classifiedGroups.TryGetValue(classKey, out values))
                            {
                                values = new List<object>();
                                classifiedGroups.Add(classKey, values);
                            }

                            values.Add(litm);
                        }

                        // Write classified groups
                        foreach (var kv in classifiedGroups)
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
                                    SerializeInternal(vitm as IdentifiedData, jwriter, writeStack);
                                else
                                    jwriter.WriteValue(svalue);
                            }
                            jwriter.WriteEndArray();
                        }

                        jwriter.WriteEndObject();
                    }
                }
                else if (value is IdentifiedData)
                    SerializeInternal(value as IdentifiedData, jwriter, writeStack);
                else
                    jwriter.WriteValue(value);
            }

            if (ver?.VersionKey.HasValue == true || data.Key.HasValue)
                writeStack.Remove(ver?.VersionKey ?? data.Key.Value);
            jwriter.WriteEndObject();

        }
    }

}
