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
using Jint.Runtime.Debugger;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace OpenIZ.BusinessRules.JavaScript.JNI
{
    /// <summary>
    /// Represents business rules bridge
    /// </summary>
    public class BusinessRulesBridge
    {

        private Tracer m_tracer = Tracer.GetTracer(typeof(BusinessRulesBridge));

        private Regex date_regex = new Regex(@"(\d{4})-(\d{2})-(\d{2})");
        // View model serializer
        private JsonViewModelSerializer m_modelSerializer = new JsonViewModelSerializer();

        // Map of view model names to type names
        private Dictionary<String, Type> m_modelMap = new Dictionary<string, Type>();

        // Cache objects for find and get
        private Dictionary<Guid, ExpandoObject> m_cacheObject = new Dictionary<Guid, ExpandoObject>(10);

        /// <summary>
        /// Initializes the business rules bridge
        /// </summary>
        public BusinessRulesBridge()
        {

            foreach (var t in typeof(IdentifiedData).GetTypeInfo().Assembly.ExportedTypes)
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
        /// Break current execution
        /// </summary>
        public void Break()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
            else
                new JsConsoleProvider().warn("Break was requested however no debugger is attached.");
        }

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
        /// True if the system is operating on the OpenIZ Front end
        /// </summary>
        public bool IsInFrontEnd
        {
            get
            {
                return ApplicationServiceContext.HostType != OpenIZHostType.Server;
            }
        }

        /// <summary>
        /// Saves tags associated with the specified object
        /// </summary>
        public object SaveTags(ExpandoObject obj)
        {
            try
            {
                var modelObj = this.ToModel(obj) as ITaggable;
                if (modelObj != null)
                {
                    var tags = modelObj.Tags;
                    if (tags.Count() > 0)
                    {
                        var tpi = ApplicationServiceContext.Current.GetService(typeof(ITagPersistenceService)) as ITagPersistenceService;
                        if (tpi == null)
                            return obj;
                        foreach (var t in tags)
                        {
                            t.SourceEntityKey = (modelObj as IIdentifiedEntity).Key;
                            tpi.Save(t.SourceEntityKey.Value, t);
                        }
                    }
                }
                return obj;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error saving tags: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Delete cache item
        /// </summary>
        public void DeleteCache(String type, String key)
        {
            Type dataType = null;
            if (!this.m_modelMap.TryGetValue(type, out dataType))
                throw new InvalidOperationException($"Cannot find type information for {type}");

            var idpInstance = ApplicationServiceContext.Current.GetService(typeof(IDataCachingService)) as IDataCachingService;
            if (idpInstance == null)
                throw new KeyNotFoundException($"The data caching service for {type} was not found");
            idpInstance.Remove(Guid.Parse(key));
        }

        /// <summary>
        /// Simplifies an IMSI object
        /// </summary>
        public ExpandoObject ToViewModel(IdentifiedData data)
        {
            try
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
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error converting to view model: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Produce a literal representation of the data
        /// </summary>
        private object ProduceLiteral(object data)
        {
            StringBuilder sb = new StringBuilder();
            var dict = data as IDictionary<String, Object>;
            if (dict != null)
                foreach (var kv in dict)
                    sb.AppendFormat("{0}={{{1}}}", kv.Key, this.ProduceLiteral(kv.Value));
            else
                sb.Append(data?.ToString() ?? "null");
            return sb.ToString();
        }

        /// <summary>
        /// Execute bundle rules
        /// </summary>
        public Object ExecuteBundleRules(String trigger, Object bundle)
        {
            try
            {
                var thdPool = ApplicationServiceContext.Current.GetService(typeof(IThreadPoolService)) as IThreadPoolService;
                IDictionary<String, Object> bdl = bundle as IDictionary<String, Object>;

                this.m_cacheObject.Clear();

                object rawItems = null;
                if (!bdl.TryGetValue("$item", out rawItems) && !bdl.TryGetValue("item", out rawItems))
                {
                    this.m_tracer.TraceVerbose("Bundle contains no items: {0}", this.ProduceLiteral(bdl));
                    return bundle;
                }

                Object[] itms = rawItems as object[];

                for (int i = 0; i < itms.Length; i++)
                {
                    try
                    {
                        itms[i] = JavascriptBusinessRulesEngine.Current.InvokeRaw(trigger, itms[i]);
                    }
                    catch (Exception e)
                    {
                        //if (System.Diagnostics.Debugger.IsAttached)
                        throw;
                        //else
                        //    Tracer.GetTracer(typeof(BusinessRulesBridge)).TraceError("Error applying rule for {0}: {1}", itms[i], e);
                    }
                }

                bdl.Remove("item");
                bdl.Remove("$item");
                bdl.Add("$item", itms);
                return bdl;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing bundle rules: {0}", e);
                throw;
            }

        }

        /// <summary>
        /// Convert to Jint object
        /// </summary>
        private ExpandoObject ConvertToJint(JObject source)
        {
            try { 
            var retVal = new ExpandoObject();

            if (source == null)
                return retVal;

            var expandoDic = (IDictionary<String, Object>)retVal;
            foreach (var kv in source)
            {
                if (kv.Value is JObject)
                    expandoDic.Add(kv.Key, ConvertToJint(kv.Value as JObject));
                else if (kv.Value is JArray)
                    expandoDic.Add(kv.Key == "item" ? "$item" : kv.Key, (kv.Value as JArray).Select(o => o is JValue ? (o as JValue).Value : ConvertToJint(o as JObject)).ToArray());
                else
                {
                    object jValue = (kv.Value as JValue).Value;
                    if (jValue is String && date_regex.IsMatch(jValue.ToString())) // Correct dates
                    {
                        var dValue = date_regex.Match(jValue.ToString());
                        expandoDic.Add(kv.Key, new DateTime(Int32.Parse(dValue.Groups[1].Value), Int32.Parse(dValue.Groups[2].Value), Int32.Parse(dValue.Groups[3].Value)));
                    }
                    else
                        expandoDic.Add(kv.Key, (kv.Value as JValue).Value);
                }
            }
            return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error converting to JINT : {0}", e);
                throw;
            }
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
                return Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
            }
        }

        /// <summary>
        /// Expand the view model object to an identified object 
        /// </summary>
        public IdentifiedData ToModel(Object data)
        {
            try { 
            var dictData = data as IDictionary<String, object>;
            if (dictData?.ContainsKey("$item") == true) // HACK: JInt does not like Item property on ExpandoObject
            {
                dictData.Add("item", dictData["$item"]);
                dictData.Remove("$item");
            }

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
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error converting to model instance : {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Gets the specified data from the underlying data-store
        /// </summary>
        public object Obsolete(String type, Guid id)
        {
            try { 
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
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error obsoleting object : {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Gets the specified data from the underlying data-store
        /// </summary>
        public object Get(String type, String id)
        {
            try
            {
                var guidId = Guid.Parse(id);

                // First, does the object existing the cache
                ExpandoObject retVal = null;
                if (!this.m_cacheObject.TryGetValue(guidId, out retVal))
                {
                    Type dataType = null;
                    if (!this.m_modelMap.TryGetValue(type, out dataType))
                        throw new InvalidOperationException($"Cannot find type information for {type}");

                    var idp = typeof(IRepositoryService<>).MakeGenericType(dataType);
                    var idpInstance = ApplicationServiceContext.Current.GetService(idp);
                    if (idpInstance == null)
                        throw new KeyNotFoundException($"The repository service for {type} was not found. Ensure an IRepositoryService<{type}> is registered");

                    var mi = idp.GetRuntimeMethod("Get", new Type[] { typeof(Guid) });
                    retVal = this.ToViewModel(mi.Invoke(idpInstance, new object[] { guidId }) as IdentifiedData);
                    if (this.m_cacheObject.Count >= 10)
                        this.m_cacheObject.Clear();
                    if (!this.m_cacheObject.ContainsKey(guidId))
                        this.m_cacheObject.Add(guidId, retVal);
                }
                return retVal;
             }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error getting object: {0}", e);
                throw;
            }
}

        /// <summary>
        /// Find object
        /// </summary>
        public object Find(String type, ExpandoObject query)
        {
            var queryStr = new NameValueCollection((query as IDictionary<String, object>).ToArray()).ToString();
            return Find(type, queryStr);
        }

        /// <summary>
        /// Finds the specified data 
        /// </summary>
        public object Find(String type, String query)
        {
            try { 
            Type dataType = null;
            if (!this.m_modelMap.TryGetValue(type, out dataType))
                throw new InvalidOperationException($"Cannot find type information for {type}");

            var idp = typeof(IRepositoryService<>).MakeGenericType(dataType);
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {type} was not found. Ensure an IRepositoryService<{type}> is registered");

            MethodInfo builderMethod = (MethodInfo)typeof(QueryExpressionParser).GetGenericMethod("BuildLinqExpression", new Type[] { dataType }, new Type[] { typeof(NameValueCollection) });
            var mi = idp.GetRuntimeMethod("Find", new Type[] { builderMethod.ReturnType });

            var nvc = NameValueCollection.ParseQueryString(query);
            var filter = builderMethod.Invoke(null, new Object[] { nvc });

            var results = (mi.Invoke(idpInstance, new object[] { filter }) as IEnumerable).OfType<IdentifiedData>();
            return this.ToViewModel(new Bundle()
            {
                Item = results.ToList(),
                TotalResults = results.Count()
            });
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing search : {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Saves the specified object
        /// </summary>
        public object Save(object value)
        {
            try { 
            var data = this.ToModel(value);

            if (data.Key.HasValue && this.m_cacheObject.ContainsKey(data.Key.Value))
                this.m_cacheObject.Remove(data.Key.Value);

            if (data == null) throw new ArgumentException("Could not parse value for save");

            var idp = typeof(IRepositoryService<>).MakeGenericType(data.GetType());
            var idpInstance = ApplicationServiceContext.Current.GetService(idp);
            if (idpInstance == null)
                throw new KeyNotFoundException($"The repository service for {data.GetType()} was not found. Ensure an IRepositoryService<{data.GetType()}> is registered");

            var mi = idp.GetRuntimeMethod("Save", new Type[] { data.GetType() });
            return this.ToViewModel(mi.Invoke(idpInstance, new object[] { data }) as IdentifiedData);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error saving object: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Inserts the specified object
        /// </summary>
        public object Insert(object value)
        {
            try
            {
                var data = this.ToModel(value);
                if (data == null) throw new ArgumentException("Could not parse value for insert");

                if (data.Key.HasValue && this.m_cacheObject.ContainsKey(data.Key.Value))
                    this.m_cacheObject.Remove(data.Key.Value);

                var idp = typeof(IRepositoryService<>).MakeGenericType(data.GetType());
                var idpInstance = ApplicationServiceContext.Current.GetService(idp);
                if (idpInstance == null)
                    throw new KeyNotFoundException($"The repository service for {data.GetType()} was not found. Ensure an IRepositoryService<{data.GetType()}> is registered");

                var mi = idp.GetRuntimeMethod("Insert", new Type[] { data.GetType() });
                return this.ToViewModel(mi.Invoke(idpInstance, new object[] { data }) as IdentifiedData);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error inserting BRE object: {0}", e);
                throw;
            }
        }
    }
}
