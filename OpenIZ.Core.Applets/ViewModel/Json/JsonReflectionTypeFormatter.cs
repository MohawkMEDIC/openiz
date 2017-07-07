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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model;
using System.Reflection;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Applets.ViewModel.Description;
using System.Collections;
using OpenIZ.Core.Model.Serialization;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents the JSON Reflection fallback serializer
    /// </summary>
    public class JsonReflectionTypeFormatter : IJsonViewModelTypeFormatter
    {
        // Current reflection formatter
        private Object m_syncLock = new object();

        // Serialization binder
        private static ModelSerializationBinder s_binder = new ModelSerializationBinder();

        // JSON property information
        private Dictionary<Type, Dictionary<String, PropertyInfo>> m_jsonPropertyInfo = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        // JSON Property names
        private Dictionary<PropertyInfo, String> m_jsonPropertyNames = new Dictionary<PropertyInfo, string>();

        /// <summary>
        /// Type of alue
        /// </summary>
        private Type m_type = null;

        /// <summary>
        /// Represents a JSON reflection formatter
        /// </summary>
        public JsonReflectionTypeFormatter(Type t)
        {
            this.m_type = t;
        }
       
        /// <summary>
        /// Gets the type that this formatter handles
        /// </summary>
        public Type HandlesType
        {
            get
            {
                return this.m_type;
            }
        }

        /// <summary>
        /// JSON reflection type formatter
        /// </summary>
        private JsonReflectionTypeFormatter()
        {

        }

        /// <summary>
        /// Get property information for the specified type
        /// </summary>
        private Dictionary<String, PropertyInfo> GetPropertyInfo(Type propertyType)
        {

            Dictionary<String, PropertyInfo> retVal = null;
            lock(this.m_syncLock)
                if (!this.m_jsonPropertyInfo.TryGetValue(propertyType, out retVal))
                {
                    retVal = new Dictionary<string, PropertyInfo>();
                    foreach (var pi in propertyType.GetRuntimeProperties().Where(o => o.CanWrite))
                    {
                        var propertyName = GetPropertyName(pi);
                        if (propertyName != null && !propertyName.StartsWith("$"))
                            retVal.Add(propertyName, pi);
                    }
                    if (!this.m_jsonPropertyInfo.ContainsKey(propertyType))
                            this.m_jsonPropertyInfo.Add(propertyType, retVal);
                }
            return retVal;
        }

        /// <summary>
        /// Get property name
        /// </summary>
        protected String GetPropertyName(PropertyInfo info, bool includeIgnored = false)
        {

            String retVal = null;
            if (!this.m_jsonPropertyNames.TryGetValue(info, out retVal) || includeIgnored)
            {
                if (!includeIgnored && info.GetCustomAttribute<DataIgnoreAttribute>() != null && info.GetCustomAttribute<JsonPropertyAttribute>() == null ||
                    info.GetCustomAttribute<JsonIgnoreAttribute>() != null && info.GetCustomAttribute<SerializationReferenceAttribute>() == null)
                    retVal = null;
                else
                {
                    // Property info
                    JsonPropertyAttribute jpa = info.GetCustomAttribute<JsonPropertyAttribute>();
                    if (jpa != null)
                        retVal = jpa.PropertyName;
                    else
                    {
                        SerializationReferenceAttribute sra = info.GetCustomAttribute<SerializationReferenceAttribute>();
                        if (sra != null)
                        {
                            jpa = info.DeclaringType.GetRuntimeProperty(sra.RedirectProperty).GetCustomAttribute<JsonPropertyAttribute>();
                            if (jpa != null)
                                retVal = jpa.PropertyName + "Model";
                        }
                    }

                    if (retVal == null)
                        retVal = info.Name.ToLower() + "Model";
                }

                lock(this.m_syncLock)
                    if(!this.m_jsonPropertyNames.ContainsKey(info))
                        this.m_jsonPropertyNames.Add(info, retVal);
            }
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
            bool loadedProperties = false;
            
            // Iterate properties 
            foreach (var propertyInfo in o.GetType().GetRuntimeProperties())
            {
                // Get the property name
                var propertyName = GetPropertyName(propertyInfo);
                if (propertyName == null || propertyName.StartsWith("$")) // Skip internal property names
                    continue;

                // Serialization decision
                if (!context.ShouldSerialize(propertyName))
                    continue;

                // Get the property 
                var value = propertyInfo.GetValue(o);

                // Null ,do we want to force load?
                if (value == null || (value as IList)?.Count == 0)
                {
                    var tkey = o.Key.HasValue ? o.Key.Value : Guid.NewGuid();
                    if (context.ShouldForceLoad(propertyName, tkey))
                    {
                        if (value is IList && !propertyInfo.PropertyType.IsArray)
                        {
                            if(o.Key.HasValue) 
                                value = context.JsonContext.LoadCollection(propertyInfo.PropertyType, (Guid)o.Key);
                            propertyInfo.SetValue(o, value);
                            loadedProperties = (value as IList).Count > 0;
                        }
                        else
                        {
                            var keyPropertyRef = propertyInfo.GetCustomAttribute<SerializationReferenceAttribute>();
	                        if (keyPropertyRef != null)
	                        {
		                        var keyProperty = o.GetType().GetRuntimeProperty(keyPropertyRef.RedirectProperty);
		                        var key = keyProperty.GetValue(o);
		                        if (key != null)
		                        {
			                        value = context.JsonContext.LoadRelated(propertyInfo.PropertyType, (Guid)key);
			                        propertyInfo.SetValue(o, value);
			                        loadedProperties = value != null;
		                        }

	                        }
                        }

                    }
                    else
                        continue;
                }

                // TODO: Classifier
                context.JsonContext.WritePropertyUtil(w, propertyName, value, context);

            }

            // Loaded something, let's cache it
            if (loadedProperties && o.Key.HasValue && ((o as IVersionedEntity) == null || (o as IVersionedEntity)?.VersionKey.HasValue == true) &&
                o.LoadState != LoadState.New)
                (ApplicationServiceContext.Current.GetService(typeof(IDataCachingService)) as IDataCachingService).Add(o);
        }

        /// <summary>
        /// De-serialize the specified object from the class
        /// </summary>
        public Object Deserialize(JsonReader r, Type asType, JsonSerializationContext context)
        {
            var retVal = new Object();
            if (!asType.GetTypeInfo().IsAbstract)
                retVal = Activator.CreateInstance(asType);

            int depth = r.TokenType == JsonToken.StartObject ? r.Depth : r.Depth - 1 ; // current depth
            var properties = GetPropertyInfo(asType);
            
            // We will parse this until we can no longer parse
            while (r.Read())
            {
                switch (r.TokenType)
                {
                    // The current reader is at an end of object
                    case JsonToken.EndObject:
                        if (depth == r.Depth)
                            return retVal;
                        else
                            throw new JsonSerializationException("JSON in invalid state");
                    // Current reader is at a property name
                    case JsonToken.PropertyName:
                        switch ((String)r.Value)
                        {
                            case "$type":
                                var xsiType = s_binder.BindToType(asType.GetTypeInfo().Assembly.FullName, r.ReadAsString());
                                if (xsiType != asType)
                                {
                                    var fmtr = context.JsonContext.GetFormatter(xsiType);
                                    var nRetVal = fmtr.Deserialize(r, xsiType, context);
                                    nRetVal.CopyObjectData(retVal);
                                    return nRetVal;
                                }
                                break;
                            case "$ref": // TODO: lookup reference
                                break;
                            default:
                                string propertyName = r.Value as String;
                                PropertyInfo propertyInfo = null;
                                if (properties.TryGetValue(propertyName, out propertyInfo))
                                {
                                    // Read next token
                                    r.Read();
                                    var instance = context.JsonContext.ReadElementUtil(r, propertyInfo.PropertyType, new JsonSerializationContext(propertyName, context.JsonContext, retVal, context));
                                    if (!(instance == null || (instance as IList)?.Count == 0))
                                        propertyInfo.SetValue(retVal, instance);
                                }
                                else
                                    r.Skip();
                                break;
                        }
                        break;
                }
            }
            return retVal;
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

        /// <summary>
        /// Get an object value from a simple value
        /// </summary>
        public object FromSimpleValue(object simpleValue)
        {
            // Simple value is null
            if (simpleValue == null)
                return null;

            var simpleValueAttribute = this.m_type.GetTypeInfo().GetCustomAttribute<SimpleValueAttribute>();
            object retVal = null;

            // Value attribute is null
            if (simpleValueAttribute != null)
            {
                retVal = Activator.CreateInstance(this.m_type); 
                var simpleProperty = this.m_type.GetRuntimeProperty(simpleValueAttribute.ValueProperty);
                var propertyType = simpleProperty.PropertyType.StripNullable();
                if (propertyType == typeof(Guid))
                    return Guid.Parse((String)simpleValue);
                else if (propertyType == typeof(byte[]))
                {
                    if (simpleValue is Boolean)
                        simpleProperty.SetValue(retVal, new BooleanExtensionHandler().Serialize(simpleValue));
                    else if (simpleValue is String)
                        simpleProperty.SetValue(retVal, new StringExtensionHandler().Serialize(simpleValue));
                    else if (simpleValue is Int32 || simpleValue is float || simpleValue is double || simpleValue is decimal)
                        simpleProperty.SetValue(retVal, new DecimalExtensionHandler().Serialize(simpleValue));
                    else
                        simpleProperty.SetValue(retVal, simpleValue);
                }
                else
                    simpleProperty.SetValue(retVal, simpleValue);
            }
            return retVal;
        }
    }
}
