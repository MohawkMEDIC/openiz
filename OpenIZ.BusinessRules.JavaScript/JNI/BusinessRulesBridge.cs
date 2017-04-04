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
 * Date: 2016-11-8
 */
using Jint.Native;
using Newtonsoft.Json;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenIZ.Core.Model.Roles;
using System.Reflection;
using OpenIZ.Core;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using System.Collections;

namespace OpenIZ.BusinessRules.JavaScript.JNI
{
    /// <summary>
    /// Represents business rules bridge
    /// </summary>
    public class BusinessRulesBridge
    {
        
        // View model serializer
        private JsonViewModelSerializer m_modelSerializer = new JsonViewModelSerializer();

        // Map of view model names to type names
        private Dictionary<String, Type> m_modelMap = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes the business rules bridge
        /// </summary>
        public BusinessRulesBridge()
        {

            foreach(var t in typeof(IdentifiedData).GetTypeInfo().Assembly.ExportedTypes)
            {
                var jatt = t.GetTypeInfo().GetCustomAttribute<JsonObjectAttribute>();

                if (jatt?.Id != null && !this.m_modelMap.ContainsKey(jatt.Id))
                    this.m_modelMap.Add(jatt.Id, t);
            }
        }

        /// <summary>
        /// Gets the serializer
        /// </summary>
        public JsonViewModelSerializer Serializer { get { return this.m_modelSerializer; } }

        /// <summary>
        /// Add a business rule for the specified object
        /// </summary>
        public void AddBusinessRule(String target, String trigger, Func<Object, ExpandoObject> _delegate)
        {
            JavascriptBusinessRulesEngine.Current.RegisterRule(target, trigger, _delegate);
        }

        /// <summary>
        /// Adds validator
        /// </summary>
        public void AddValidator(String target, Func<Object, Object[]> _delegate)
        {
            JavascriptBusinessRulesEngine.Current.RegisterValidator(target, _delegate);
        }

        /// <summary>
        /// Executes the business rule
        /// </summary>
        public object ExecuteRule(String action, Object data)
        {
            var sData = this.ToModel(data);
            var retVal = JavascriptBusinessRulesEngine.Current.Invoke(action, sData);
            return this.ToViewModel(retVal);
        }

        /// <summary>
        /// Delete cache item
        /// </summary>
        public void DeleteCache(String type, String key) {
            Type dataType = null;
            if (!this.m_modelMap.TryGetValue(type, out dataType))
                throw new InvalidOperationException($"Cannot find type information for {type}");

            var idpInstance = ApplicationServiceContext.Current.GetService(typeof(IDataCachingService)) as IDataCachingService;
            if (idpInstance == null)
                throw new KeyNotFoundException($"The data caching service for {type} was not found");
            idpInstance.Remove(dataType, Guid.Parse(key));
        }

        /// <summary>
        /// Simplifies an IMSI object
        /// </summary>
        public ExpandoObject ToViewModel(IdentifiedData data)
        {
            // Serialize to a view model serializer
            using (MemoryStream ms = new MemoryStream())
            {
                using (TextWriter tw = new StreamWriter(ms, Encoding.UTF8, 2048, true))
                    this.m_modelSerializer.Serialize(tw, data);
                ms.Seek(0, SeekOrigin.Begin);

                // Parse
                Jint.Native.Json.JsonParser parser = new Jint.Native.Json.JsonParser(JavascriptBusinessRulesEngine.Current.Engine);
                JsonSerializer jsz = new JsonSerializer() { DateFormatHandling = DateFormatHandling.IsoDateFormat, TypeNameHandling = TypeNameHandling.None };
                using (JsonReader reader = new JsonTextReader(new StreamReader(ms)))
                {
                    var retVal = jsz.Deserialize<Newtonsoft.Json.Linq.JObject>(reader);
                    return this.ConvertToJint(retVal);
                    ///return retVal;
                }
            }
        }

        /// <summary>
        /// Execute bundle rules
        /// </summary>
        public Object ExecuteBundleRules(String trigger, Object bundle)
        {
            var bdl = this.ToModel(bundle) as Bundle;
            foreach (var itm in bdl.Item)
                JavascriptBusinessRulesEngine.Current.Invoke(trigger, itm);
            return bundle;

        }

        /// <summary>
        /// Convert to Jint object
        /// </summary>
        private ExpandoObject ConvertToJint(JObject source)
        {
            var retVal = new ExpandoObject();
            var expandoDic = (IDictionary<String, Object>)retVal;
            foreach(var kv in source)
            {
                if (kv.Value is JObject)
                    expandoDic.Add(kv.Key, ConvertToJint(kv.Value as JObject));
                else if (kv.Value is JArray)
                    expandoDic.Add(kv.Key, (kv.Value as JArray).Select(o => o is JValue ? (o as JValue).Value : ConvertToJint(o as JObject)).ToArray());
                else
                    expandoDic.Add(kv.Key, (kv.Value as JValue).Value);
            }
            return retVal;
        }

        /// <summary>
        /// Get service by name
        /// </summary>
        public object GetService(String serviceName)
        {
            var serviceType = typeof(IActRepositoryService).GetTypeInfo().Assembly.ExportedTypes.FirstOrDefault(o => o.Name == serviceName && o.GetTypeInfo().IsInterface);
            if (serviceType == null)
                return null;
            else
                return ApplicationServiceContext.Current.GetService(serviceType);
        }

        /// <summary>
        /// Generate new guid
        /// </summary>
        public String NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generate new guid
        /// </summary>
        public Guid ParseGuid(String guid)
        {
            return Guid.Parse(guid);
        }


        /// <summary>
        /// Get data asset
        /// </summary>
        public String GetDataAsset(String dataId)
        {
            using (Stream ins = (ApplicationServiceContext.Current.GetService(typeof(IDataReferenceResolver)) as IDataReferenceResolver).Resolve(dataId))
            using (MemoryStream ms = new MemoryStream())
            {
                ins.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Expand the view model object to an identified object 
        /// </summary>
        public IdentifiedData ToModel(Object data)
        {

            // Serialize to a view model serializer
            using (MemoryStream ms = new MemoryStream())
            {
                JsonSerializer jsz = new JsonSerializer();
                using (JsonWriter reader = new JsonTextWriter(new StreamWriter(ms, Encoding.UTF8, 2048, true)))
                    jsz.Serialize(reader, data);

                // De-serialize
                ms.Seek(0, SeekOrigin.Begin);
                var retVal = this.m_modelSerializer.DeSerialize<IdentifiedData>(ms);
                return retVal;
            }
        }

        /// <summary>
        /// Gets the specified data from the underlying data-store
        /// </summary>
        public object Obsolete(String type, Guid id)
        {

            Type dataType = null;
            if (!this.m_modelMap.TryGetValue(type, out dataType))
                throw new InvalidOperationException($"Cannot find type information for {type}");

            var idp = typeof(IRepositoryService<>).MakeGenericType(dataType);
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {type} was not found. Ensure an IRepositoryService<{type}> is registered");

            var mi = idp.GetRuntimeMethod("Obsolete", new Type[] { typeof(Guid) });
            return this.ToViewModel(mi.Invoke(idpInstance, new object[] { id }) as IdentifiedData);
        }

        /// <summary>
        /// Gets the specified data from the underlying data-store
        /// </summary>
        public object Get(String type, String id)
        {

            Type dataType = null;
            if (!this.m_modelMap.TryGetValue(type, out dataType))
                throw new InvalidOperationException($"Cannot find type information for {type}");

            var idp = typeof(IRepositoryService<>).MakeGenericType(dataType);
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {type} was not found. Ensure an IRepositoryService<{type}> is registered");

            var mi = idp.GetRuntimeMethod("Get", new Type[] { typeof(Guid) });
            return this.ToViewModel(mi.Invoke(idpInstance, new object[] { Guid.Parse(id) }) as IdentifiedData);
        }

        /// <summary>
        /// Finds the specified data 
        /// </summary>
        public object Find(String type, String query)
        {
            Type dataType = null;
            if (!this.m_modelMap.TryGetValue(type, out dataType))
                throw new InvalidOperationException($"Cannot find type information for {type}");

            var idp = typeof(IRepositoryService<>).MakeGenericType(dataType);
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {type} was not found. Ensure an IRepositoryService<{type}> is registered");

            MethodInfo builderMethod = (MethodInfo)typeof(QueryExpressionParser).GetGenericMethod("BuildLinqExpression", new Type[] { dataType }, new Type[] { typeof(NameValueCollection) });
            var mi = idp.GetRuntimeMethod("Find", new Type[] { builderMethod.ReturnType  });

            var nvc = NameValueCollection.ParseQueryString(query);
            var filter = builderMethod.Invoke(null, new Object[] { nvc });

            var results = mi.Invoke(idpInstance, new object[] { filter }) as IEnumerable;
            return this.ToViewModel(new Bundle()
            {
                Item = results.OfType<IdentifiedData>().ToList(),
            });

        }

        /// <summary>
        /// Saves the specified object
        /// </summary>
        public object Save(object value)
        {

            var data = this.ToModel(value);
            if (data == null) throw new ArgumentException("Could not parse value for save");

            var idp = typeof(IRepositoryService<>).MakeGenericType(data.GetType());
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {data.GetType()} was not found. Ensure an IRepositoryService<{data.GetType()}> is registered");

            var mi = idp.GetRuntimeMethod("Save", new Type[] { data.GetType() });
            return this.ToViewModel(mi.Invoke(idpInstance, new object[] { data }) as IdentifiedData);

        }

        /// <summary>
        /// Inserts the specified object
        /// </summary>
        public object Insert(object value)
        {

            var data = this.ToModel(value);
            if (data == null) throw new ArgumentException("Could not parse value for insert");

            var idp = typeof(IRepositoryService<>).MakeGenericType(data.GetType());
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {data.GetType()} was not found. Ensure an IRepositoryService<{data.GetType()}> is registered");

            var mi = idp.GetRuntimeMethod("Insert", new Type[] { data.GetType() });
            return this.ToViewModel(mi.Invoke(idpInstance, new object[] { data }) as IdentifiedData);
        }
    }
}
