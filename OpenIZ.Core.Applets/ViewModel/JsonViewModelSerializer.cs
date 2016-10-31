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
using System.Diagnostics;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Applets.ViewModel.Description;
using System.Xml.Serialization;
using OpenIZ.Core;
using OpenIZ.Core.Services;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// View model class for Applets turns the core business model objects into
    /// a more sane, flat model to be given to applets as JSON
    /// </summary>
    /// <remarks>
    /// This file needs a bath because it stinks... AKA: Clean it up
    /// </remarks>
    public static class JsonViewModelSerializer
    {

        // Binder
        private static ModelSerializationBinder s_binder = new ModelSerializationBinder();

        // Property cache
        private static ViewModelDescription s_defaultViewModel = new ViewModelDescription()
        {
            Name = "__default"
        };

        private static Dictionary<Type, Dictionary<String, PropertyInfo>> s_readPropertyCache = new Dictionary<Type, Dictionary<String, PropertyInfo>>();

        // View model cache
        private static Dictionary<String, RootSerializationContext> s_rootSerializationContexts = new Dictionary<string, RootSerializationContext>();
        
        /// <summary>
        /// Serialize using the stream writer
        /// </summary>
        public static void Serialize(TextWriter writer, IdentifiedData data, ViewModelDescription viewModel = null)
        {
            JsonWriter jwriter = new JsonTextWriter(writer);

            if (viewModel == null)
                PrepareDefaultViewModel(data.GetType());
            // Iterate over properties
            SerializeInternal(data, jwriter, new RootSerializationContext(data?.GetType(), viewModel ?? s_defaultViewModel), new Stack<Guid>());

        }

        /// <summary>
        /// Serializes the specified internal data 
        /// </summary>
        /// <remarks>
        /// Rules: 
        ///     - Keys are serialized as JSON object property
        ///     - Delay load are also expanded to keynameModel which is the model
        ///     - Collections are expanded to classifier class model[classifier] = value
        /// </remarks>
        public static String Serialize(IdentifiedData data, ViewModelDescription viewModel = null)
        {
            if (data == null)
                return String.Empty;

            using (StringWriter sw = new StringWriter())
            {
                Serialize(sw, data, viewModel);
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
        /// Perform the work of de-serializing
        /// </summary>
        private static IdentifiedData DeSerializeInternal(JsonReader jreader, Type serializationType)
        {


            // Must be at start object
            if (jreader.TokenType != JsonToken.StartObject)
                throw new InvalidDataException();

            // Ctor object
            object retVal = null;

            if (serializationType.GetTypeInfo().IsAbstract)
                retVal = new object();
            else
                retVal = Activator.CreateInstance(serializationType);
            int depth = jreader.Depth;

            PreparePropertyCache(serializationType);
            var serializationProperties = s_readPropertyCache[serializationType];


            // Reader
            while (jreader.Read())
            {
                // Break condition
                if (jreader.TokenType == JsonToken.EndObject && jreader.Depth == depth)
                    break;

                switch (jreader.TokenType)
                {
                    case JsonToken.PropertyName:
                        // Find the property 
                        PropertyInfo propertyInfo = null;
                        serializationProperties.TryGetValue(jreader.Value.ToString(), out propertyInfo);
#if VERBOSE_DEBUG
                        Debug.WriteLine("< {0}", jreader.Path);
#endif
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
                                    var classifierName = elementType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty;
                                    PropertyInfo classifierProperty = null;
                                    if (classifierName != null)
                                        classifierProperty = elementType.GetRuntimeProperty(classifierName);
                                    else
                                        throw new InvalidOperationException(String.Format("Invalid state must have classifier at {0} on {1}", jreader.Path, elementType));

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
                                                // Might only be one object in the array 
                                                jreader.Read();

                                                if (jreader.TokenType == JsonToken.StartArray || jreader.TokenType == JsonToken.StartObject)
                                                {

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
                                                        else if (jreader.TokenType == JsonToken.Null) // null = skip
                                                            continue;
                                                        else // simple value
                                                        {
                                                            contained = Activator.CreateInstance(elementType);
                                                            // Get the simple value property
                                                            PropertyInfo simpleProperty = null;
                                                            if (elementType.GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>() != null)
                                                                simpleProperty = elementType.GetRuntimeProperty(elementType.GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>().ValueProperty);

                                                            if (simpleProperty != null && simpleProperty.PropertyType.StripNullable() == typeof(Guid))
                                                            {
                                                                if (!String.IsNullOrEmpty((String)jreader.Value))
                                                                    simpleProperty.SetValue(contained, Guid.Parse((String)jreader.Value));
                                                            }
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
                                                }
                                                else if (jreader.TokenType == JsonToken.Null)
                                                    ; // don't do anything
                                                else
                                                {
                                                    var klu = elementType.GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();
                                                    var satt = elementType.GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>();
                                                    object value = null;
                                                    if (elementType.StripNullable() == typeof(Guid))
                                                        value = Guid.Parse((String)jreader.Value);
                                                    else if (satt != null) // not a key
                                                    {
                                                        var sattInstance = Activator.CreateInstance(elementType);
                                                        var sattPropertyInfo = elementType.GetRuntimeProperty(satt.ValueProperty);
                                                        if (sattPropertyInfo.PropertyType == typeof(String))
                                                            sattPropertyInfo.SetValue(sattInstance, jreader.Value.ToString());
                                                        else if (sattPropertyInfo.PropertyType == typeof(byte[]))
                                                        {
                                                            try
                                                            {
                                                                sattPropertyInfo.SetValue(sattInstance, Convert.FromBase64String(jreader.Value?.ToString()));
                                                            }
                                                            catch
                                                            {
                                                                sattPropertyInfo.SetValue(sattInstance, Encoding.UTF8.GetBytes(jreader.Value?.ToString() ?? ""));
                                                            }
                                                        }
                                                        else
                                                            sattPropertyInfo.SetValue(sattInstance, jreader.Value);
                                                        value = sattInstance;
                                                    }
                                                    else if (jreader.Value != null)
                                                        value = JsonConvert.DeserializeObject((String)jreader.Value, elementType.StripNullable());

                                                    // Set the classifier
                                                    if (classifier != "$other")
                                                    {
                                                        var setProperty = classifierProperty;
                                                        String propertyName = classifierProperty.Name;
                                                        var target = value;
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
                                                    (modelInstance as IList).Add(value);

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

                                            if (typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(elementType.GetTypeInfo()))
                                                (modelInstance as IList).Add(DeSerializeInternal(jreader, elementType));
                                            else
                                            {
                                                // TODO: This file needs to be cleaned up desperately
                                                var klu = elementType.GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();
                                                if (elementType.StripNullable() == typeof(Guid))
                                                    (modelInstance as IList).Add(Guid.Parse((String)jreader.Value));
                                                else if (klu == null) // not a key
                                                    (modelInstance as IList).Add(JsonConvert.DeserializeObject((String)jreader.Value, elementType));
                                            }
                                        }


                                        if (propertyInfo.GetCustomAttribute<DataIgnoreAttribute>() != null &&
                                            propertyInfo.GetCustomAttribute<JsonPropertyAttribute>() == null)
                                            continue; // skip

                                        propertyInfo.SetValue(retVal, modelInstance);
                                    }
                                }
                                break;
                            default: // Simple values
                                if (propertyInfo == null)
                                    continue;
                                else if (propertyInfo.Name == nameof(IdentifiedData.Type)) // Type switch
                                {
                                    var xsiType = s_binder.BindToType((retVal is Object ? typeof(IdentifiedData) : retVal.GetType()).GetTypeInfo().Assembly.FullName, (String)jreader.Value);
                                    if (xsiType != retVal.GetType())
                                    {
                                        var nRetVal = Activator.CreateInstance(xsiType);
                                        nRetVal.CopyObjectData(retVal);
                                        retVal = nRetVal;
                                        serializationType = xsiType;
                                        PreparePropertyCache(xsiType);
                                        serializationProperties = s_readPropertyCache[xsiType];
                                    }
                                }
                                else if (jreader.TokenType == JsonToken.Null)
                                    break;
                                else if (propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(jreader.ValueType.GetTypeInfo()))
                                    propertyInfo.SetValue(retVal, jreader.Value);
                                else
                                    switch (jreader.TokenType)
                                    {
                                        case JsonToken.Integer:
                                            if (propertyInfo.PropertyType.StripNullable().GetTypeInfo().IsEnum)
                                                propertyInfo.SetValue(retVal, Enum.ToObject(propertyInfo.PropertyType.StripNullable(), jreader.Value));
                                            //else if (StripNullable(propertyInfo.PropertyType) == typeof(Int32) ||
                                            //    StripNullable(propertyInfo.PropertyType) == typeof(UInt32))
                                            else
                                                propertyInfo.SetValue(retVal, JsonConvert.DeserializeObject(jreader.Value.ToString(), propertyInfo.PropertyType));

                                            break;
                                        case JsonToken.Date:
                                            if (propertyInfo.PropertyType.StripNullable() == typeof(DateTime))
                                                propertyInfo.SetValue(retVal, (DateTime)jreader.Value);
                                            else if (propertyInfo.PropertyType == typeof(String))
                                                propertyInfo.SetValue(retVal, ((DateTime)jreader.Value).ToString("o"));
                                            else
                                            {
                                                // HACK: Mono is a scumbag
                                                var dto = new DateTimeOffset((DateTime)jreader.Value);
                                                propertyInfo.SetValue(retVal, dto);
                                            }
                                            break;
                                        case JsonToken.Float:
                                            if (propertyInfo.PropertyType.StripNullable() == typeof(Decimal))
                                                propertyInfo.SetValue(retVal, Convert.ToDecimal(jreader.Value));
                                            else
                                                propertyInfo.SetValue(retVal, (Double)jreader.Value);
                                            break;
                                        case JsonToken.String:

                                            var klu = propertyInfo.PropertyType.GetTypeInfo().GetCustomAttribute<KeyLookupAttribute>();
                                            if (propertyInfo.PropertyType.StripNullable() == typeof(Guid))
                                            {
                                                if (!String.IsNullOrEmpty((String)jreader.Value))
                                                    propertyInfo.SetValue(retVal, Guid.Parse((String)jreader.Value));
                                            }
                                            else if (klu == null) // not a key
                                                propertyInfo.SetValue(retVal, JsonConvert.DeserializeObject((String)jreader.Value, propertyInfo.PropertyType.StripNullable()));
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
        /// Prepares the property caches
        /// </summary>
        /// <param name="serialziationType"></param>
        private static void PreparePropertyCache(Type serializationType)
        {
            // Properties
            Dictionary<String, PropertyInfo> parseProperties = null;
            if (!s_readPropertyCache.TryGetValue(serializationType, out parseProperties))
            {
                parseProperties = new Dictionary<string, PropertyInfo>();
                foreach (var itm in serializationType.GetRuntimeProperties().Where(o => o.CanWrite))
                {
                    var jpa = itm.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                    if (jpa == null && itm.GetCustomAttribute<SerializationReferenceAttribute>() != null)
                        jpa = serializationType.GetRuntimeProperty(itm.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty)?.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName + "Model";
                    if (jpa == null)
                        jpa = itm.Name.ToLower() + "Model";
                    parseProperties.Add(jpa, itm);
                }
                lock (s_readPropertyCache)
                    if (!s_readPropertyCache.ContainsKey(serializationType))
                        s_readPropertyCache.Add(serializationType, parseProperties);
            }
        }

        /// <summary>
        /// Prepare default view model
        /// </summary>
        private static void PrepareDefaultViewModel(Type serializationType)
        {
            // Properties
            if (!s_defaultViewModel.Model.Any(o => o.TypeName == serializationType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName))
            {
                TypeModelDescription modelDescription = new TypeModelDescription()
                {
                    TypeName = serializationType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName
                };

                foreach (var itm in serializationType.GetRuntimeProperties().Where(o => o.CanRead))
                {
                    // Truly ignore this? No JsonProperty and a DataIgnore?
                    if (itm.GetCustomAttribute<DataIgnoreAttribute>() != null && itm.GetCustomAttribute<JsonPropertyAttribute>() == null)
                        continue;

                    var jpa = itm.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
                    if (jpa == null && itm.GetCustomAttribute<SerializationReferenceAttribute>() != null)
                        jpa = serializationType.GetRuntimeProperty(itm.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty)?.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName + "Model";
                    if (jpa == null)
                        jpa = itm.Name.ToLower() + "Model";

                    if (jpa == null || jpa == "$type")
                        continue;
                    modelDescription.Properties.Add(new PropertyModelDescription()
                    {
                        Name = jpa
                    });
                }
                lock (s_defaultViewModel)
                    s_defaultViewModel.Model.Add(modelDescription);
            }
        }

        /// <summary>
        /// Serialize the specified object
        /// </summary>
        private static void SerializeInternal(IdentifiedData data, JsonWriter jwriter, SerializationContext context, Stack<Guid> writeStack)
        {

            // True when this method has modified "data" attribute
            bool loadedProperties = false;

            if (data == null || data.Key.HasValue && writeStack.Contains(data.Key.Value))
            {
                jwriter.WriteNull();
                return;
            }

            // Prepare the default view model if not exist
            PrepareDefaultViewModel(data.GetType());

            // Prevent circular references
            if(data.Key.HasValue)
                writeStack.Push(data.Key.Value);

            // Write object data
            jwriter.WriteStartObject();
            jwriter.WritePropertyName("$type");
            jwriter.WriteValue(data.Type);
            var myClassifier = data.GetType().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();

            foreach (var propertyInfo in data.GetType().GetRuntimeProperties())
            {
                // Property info cannot read
                if (!propertyInfo.CanRead) continue;

                // Is there a serialization configured for this?
                var propertyContext = new PropertySerializationContext(propertyInfo, context);

                // We never serialize unless
                //  - Context is ROOT (all ROOT properties are serialized)
                //  - The definition says ALL properties
                //  - The definition is explicitly set
                if (!((context is RootSerializationContext) ||
                    (context as PropertySerializationContext).Description?.All == true ||
                    propertyContext.Description is PropertyModelDescription) &&
                    typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType.StripGeneric().GetTypeInfo()) ||
                    (propertyContext.Description as PropertyModelDescription)?.Action == SerializationBehaviorType.Never)
                    continue;

                // Do we skip this property?
                if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null && propertyInfo.GetCustomAttribute<SerializationReferenceAttribute>() == null ||
                    propertyInfo.GetCustomAttribute<DataIgnoreAttribute>() != null && propertyInfo.GetCustomAttribute<JsonPropertyAttribute>() == null)
                    continue;

                // Value is null
                var value = propertyInfo.GetValue(data);

                // First, is this explicitly defined!?
                if ((value == null || (value as IList)?.Count == 0) && (propertyContext.Description as PropertyModelDescription)?.Action == SerializationBehaviorType.Always)
                {
                    try
                    {
                        // Known miss targets
                        HashSet<String> missProp = null;
                        if (propertyContext.Root.Loaded.TryGetValue(data.Key.Value, out missProp))
                        {
                            if (missProp.Contains(propertyContext.Name))
                                continue; // skip known miss
                            else
                                missProp.Add(propertyContext.Name);
                        }
                        else
                            propertyContext.Root.Loaded.Add(data.Key.Value, new HashSet<string>() { propertyContext.Name });

                        // We are going to load this mofo , so let's make sure it is loaded
                        if (value is IList)
                        {
                            if (data.Key != null && data.ModifiedOn != default(DateTimeOffset))
                            {
                                var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];
                                var method = EntitySource.Current.Provider.GetType().GetRuntimeMethod("GetRelations", new Type[] { typeof(Guid?) }).MakeGenericMethod(elementType);
                                value = method.Invoke(EntitySource.Current.Provider, new Object[] { data.Key }) as IList;
                                propertyInfo.SetValue(data, value);
                                loadedProperties = true;

                            }
                        }
                        else if (propertyInfo.GetCustomAttribute<SerializationReferenceAttribute>() != null)
                        {
                            var keyPropertyRef = propertyInfo.GetCustomAttribute<SerializationReferenceAttribute>();
                            var keyProperty = data.GetType().GetRuntimeProperty(keyPropertyRef.RedirectProperty);
                            var key = keyProperty.GetValue(data);
                            if (key != null)
                            {
                                var method = EntitySource.Current.Provider.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid?) }).MakeGenericMethod(propertyInfo.PropertyType);
                                // Backing property
                                value = method.Invoke(EntitySource.Current.Provider, new Object[] { key }) as IList;
                                propertyInfo.SetValue(data, value);
                                loadedProperties = true;

                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }

                if (value == null || (propertyInfo.Name == myClassifier?.ClassifierProperty && value is IdentifiedData) ||
                    propertyInfo.PropertyType.GetTypeInfo().IsValueType && value == Activator.CreateInstance(propertyInfo.PropertyType))
                    continue;

                String propertyName = propertyContext.Name;
                if (String.IsNullOrEmpty(propertyName))
                    continue;

                if (value is IList && (value as IList).Count == 0)
                    continue;

#if VERBOSE_DEBUG
                Debug.WriteLine("> {0}", jwriter.Path);
#endif
                jwriter.WritePropertyName(propertyName);

                if (value is IList)
                {
                    var listValue = value as IList;
                    var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];

                    // Element type is an association? then we must have an explicit description
                    if (typeof(ISimpleAssociation).GetTypeInfo().IsAssignableFrom(elementType.GetTypeInfo()))
                    {
                        if (propertyContext.Description == null && context.Description?.All != true)
                        {
                            jwriter.WriteNull();
                            continue;
                        }
                    }

                    var classifierAttribute = elementType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                    if (classifierAttribute == null) // No classifier
                    {
                        jwriter.WriteStartArray();
                        foreach (var litm in listValue)
                        {
                            if (litm is IdentifiedData)
                            {
                                // Value ... hmm... Is there a more specific rendition based on actual value?
                                if (propertyContext.Description is TypeModelDescription)
                                    propertyContext.Description = context.ViewModelDefinition.FindDescription(litm.GetType().StripGeneric());

                                SerializeInternal(litm as IdentifiedData, jwriter, propertyContext, writeStack);
                            }
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
                            var classifierObj = GetClassifierObj(litm, classifierAttribute);

                            // Classifier obj becomes the dictionary object
                            List<Object> values = null;
                            String classKey = classifierObj?.ToString() ?? "$other";

                            // Classified object is not to be loaded
                            if (propertyContext.Parent.Description?.Properties.Any(o => (o.Classifier == classKey || o.Classifier == "*") && o.Name == propertyName) != true &&
                                context.Description?.All != true &&
                                !context.ViewModelDefinition.Model.Any(o => o.TypeName == elementType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName))
                                continue;

                            // Classified group add
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

                            if (kv.Value.Count() > 1)
                                jwriter.WriteStartArray();
                            foreach (var vitm in kv.Value)
                            {
                                // Does the list item expose a simple value property?
                                var simpleAttribute = vitm.GetType().GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>();
                                var svalue = vitm;
                                if (simpleAttribute != null && kv.Key != "$other")
                                    svalue = vitm.GetType().GetRuntimeProperty(simpleAttribute.ValueProperty).GetValue(vitm);

                                if (svalue is IdentifiedData)
                                {
                                    var newPropertyContext = new PropertySerializationContext(propertyInfo, context);
                                    newPropertyContext.Description = propertyContext.Parent.Description?.Properties.FirstOrDefault(o => o.Classifier == kv.Key && o.Name == propertyName) ??
                                            propertyContext.Parent.Description?.Properties.FirstOrDefault(o => o.Classifier == "*" && o.Name == propertyName) ??
                                            newPropertyContext.Description;
                                    SerializeInternal(vitm as IdentifiedData, jwriter, newPropertyContext, writeStack);
                                }
                                else
                                    jwriter.WriteValue(svalue);
                            }
                            if (kv.Value.Count() > 1)
                                jwriter.WriteEndArray();
                        }

                        jwriter.WriteEndObject();
                    }
                }
                else if (value is IdentifiedData)
                {
                    // Value ... hmm... Is there a more specific rendition based on actual value?
                    if (propertyContext.Description is TypeModelDescription)
                        propertyContext.Description = context.ViewModelDefinition.FindDescription(value.GetType().StripGeneric());

                    SerializeInternal(value as IdentifiedData, jwriter, propertyContext, writeStack);
                }
                else
                    jwriter.WriteValue(value);
            }

            if(data.Key.HasValue)
                writeStack.Pop();
            jwriter.WriteEndObject();

            // Loaded properties?
            if (loadedProperties)
                (ApplicationServiceContext.Current.GetService(typeof(IDataCachingService)) as IDataCachingService).Add(data);
        }

        /// <summary>
        /// Get classifier object
        /// </summary>
        private static object GetClassifierObj(object o, ClassifierAttribute classifierAttribute)
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
                return GetClassifierObj(classifierObj, classifierAttribute);
            return classifierObj;
        }
    }

}
