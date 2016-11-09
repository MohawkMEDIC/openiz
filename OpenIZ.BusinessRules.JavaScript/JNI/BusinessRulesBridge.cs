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

namespace OpenIZ.BusinessRules.JavaScript.JNI
{
    /// <summary>
    /// Represents business rules bridge
    /// </summary>
    public class BusinessRulesBridge
    {

        // View model serializer
        private IViewModelSerializer m_modelSerializer = new JsonViewModelSerializer();

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
        /// Simplifies an IMSI object
        /// </summary>
        public Object SimplifyObject(IdentifiedData data)
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
        /// Convert to Jint object
        /// </summary>
        private Dictionary<String, Object> ConvertToJint(JObject source)
        {
            Dictionary<String, Object> retVal = new Dictionary<string, object>();
            foreach(var kv in source)
            {
                if (kv.Value is JObject)
                    retVal.Add(kv.Key, ConvertToJint(kv.Value as JObject));
                else if (kv.Value is JArray)
                    retVal.Add(kv.Key, (kv.Value as JArray).Select(o => (o as JValue).Value));
                else
                    retVal.Add(kv.Key, (kv.Value as JValue).Value);
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
        /// Expand the view model object to an identified object 
        /// </summary>
        public Patient ExpandObject(Object data)
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
                return retVal as Patient;
            }

        }


    }
}
