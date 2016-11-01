using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Diagnostics;
using Newtonsoft.Json;
using System.Collections;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a JSON view model serializer
    /// </summary>
    public class JsonViewModelSerializer : IViewModelSerializer
    {

        // Formatters
        private Dictionary<Type, IJsonViewModelTypeFormatter> m_formatters = new Dictionary<Type, IJsonViewModelTypeFormatter>();

        // Classifiers
        private Dictionary<Type, IViewModelClassifier> m_classifiers = new Dictionary<Type, IViewModelClassifier>();

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(JsonViewModelSerializer));

        /// <summary>
        /// Creates a json view model serializer
        /// </summary>
        public JsonViewModelSerializer()
        {
            this.ViewModel = new ViewModelDescription()
            {
                Model = new List<TypeModelDescription>()
                {
                    new TypeModelDescription()
                    {
                        TypeName = "IdentifiedData",
                        All = true
                    }
                }
            };
        }

        /// <summary>
        /// Gets or sets the view model definition for this view model serializer
        /// </summary>
        public ViewModelDescription ViewModel
        {
            get; set;
        }

        /// <summary>
        /// De-serializes the specified object from the stream
        /// </summary>
        public TModel DeSerialize<TModel>(Stream s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delay loads the specified collection association
        /// </summary>
        public List<TAssociation> LoadCollection<TAssociation>(Guid sourceKey) where TAssociation : IdentifiedData, ISimpleAssociation, new()
        {
            return EntitySource.Current.Provider.GetRelations<TAssociation>(sourceKey);
        }

        /// <summary>
        /// Load the related information
        /// </summary>
        public TRelated LoadRelated<TRelated>(Guid objectKey) where TRelated : IdentifiedData, new()
        {
            return EntitySource.Current.Provider.Get<TRelated>(objectKey);
        }

        /// <summary>
        /// Write property utility
        /// </summary>
        public void WritePropertyUtil(JsonWriter w, String propertyName, Object instance, SerializationContext context)
        {

            // Are we to never serialize this?
            if (context?.ShouldSerialize(propertyName) == false)
                return;

            // first write the property
            if (!String.IsNullOrEmpty(propertyName))  // In an array so don't emit the property name
                w.WritePropertyName(propertyName);
            
            // Emit type
            if (instance == null)
                w.WriteNull();
            else if (instance is IdentifiedData)
            {
                var identifiedData = instance as IdentifiedData;

                // Complex type .. allow the formatter to handle this
                IJsonViewModelTypeFormatter typeFormatter = this.GetFormatter(instance.GetType());

                var simpleValue = typeFormatter.GetSimpleValue(instance);
                if (simpleValue != null && context.PropertyName != "$other") // Special case for $other
                    w.WriteValue(simpleValue);
                else
                {
                    w.WriteStartObject();

                    var subContext = new JsonSerializationContext(propertyName, this, instance, context as JsonSerializationContext);
                    this.WriteSimpleProperty(w, "$type", identifiedData.Type);
                    this.WriteSimpleProperty(w, "$id", String.Format("obj{0}", subContext.ObjectId));
                    
                    // Write ref
                    var parentObjectId = context?.GetParentObjectId(identifiedData);
                    if (parentObjectId.HasValue) // Recursive
                        this.WriteSimpleProperty(w, "$ref", String.Format("#obj{0}", parentObjectId.Value));
                    else
                        typeFormatter.Serialize(w, instance as IdentifiedData, subContext);

                    w.WriteEndObject();
                }
            }
            else if (instance is IList)
            {
                // Classifications?
                var classifier = this.GetClassifier(instance.GetType());

                if (classifier == null) // no classifier
                {
                    w.WriteStartArray();
                    foreach (var itm in instance as IList)
                        this.WritePropertyUtil(w, null, itm, new JsonSerializationContext(propertyName, this, instance, context as JsonSerializationContext));
                    w.WriteEndArray();
                }
                else
                {
                    w.WriteStartObject();
                    foreach(var cls in classifier.Classify(instance as IList))
                    {
                        Object value = new List<Object>(cls.Value as IEnumerable<Object>);
                        if (cls.Value.Count == 1)
                            value = cls.Value[0];
                        // Now write
                        this.WritePropertyUtil(w, cls.Key, value, new JsonSerializationContext(propertyName, this, instance, context as JsonSerializationContext));
                    }
                    w.WriteEndObject();
                }
            }
            else
                w.WriteValue(instance);

        }

        /// <summary>
        /// Gets the appropriate classifier for the specified type
        /// </summary>
        private IViewModelClassifier GetClassifier(Type type)
        {
            IViewModelClassifier retVal = null;
            if (!this.m_classifiers.TryGetValue(type, out retVal))
                retVal = JsonReflectionTypeFormatter.Current.GetClassifier(type);
            return retVal;
        }

        /// <summary>
        /// Write a simple property
        /// </summary>
        /// <param name="propertyName">The name of the JSON property</param>
        /// <param name="value">The value of the JSON property</param>
        /// <param name="w">The value of the property</param>
        private void WriteSimpleProperty(JsonWriter w, String propertyName, Object value)
        {
            w.WritePropertyName(propertyName);
            w.WriteValue(value);
        }

        /// <summary>
        /// Get the specified formatter
        /// </summary>
        /// <param name="type">The type to retrieve the formatter for</param>
        private IJsonViewModelTypeFormatter GetFormatter(Type type)
        {
            IJsonViewModelTypeFormatter typeFormatter = null;
            if (!this.m_formatters.TryGetValue(type, out typeFormatter))
                typeFormatter = JsonReflectionTypeFormatter.Current;
            return typeFormatter;
        }

        /// <summary>
        /// Load the specified serializer assembly
        /// </summary>
        public void LoadSerializerAssembly(Assembly asm)
        {
            var typeFormatters = asm.ExportedTypes.Where(o => typeof(IJsonViewModelTypeFormatter).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && o.GetTypeInfo().IsClass)
                .Select(o => Activator.CreateInstance(o) as IJsonViewModelTypeFormatter)
                .Where(o => !this.m_formatters.ContainsKey(o.HandlesType));
            var classifiers = asm.ExportedTypes.Where(o => typeof(IViewModelClassifier).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && o.GetTypeInfo().IsClass)
                .Select(o => Activator.CreateInstance(o) as IViewModelClassifier)
                .Where(o => !this.m_classifiers.ContainsKey(o.HandlesType));
            foreach (var fmtr in typeFormatters)
                this.m_formatters.Add(fmtr.HandlesType, fmtr);
            foreach (var cls in classifiers)
                this.m_classifiers.Add(cls.HandlesType, cls);
        }

        /// <summary>
        /// Serialize object to string
        /// </summary>
        public String Serialize(IdentifiedData data)
        {
            using (StringWriter sw = new StringWriter())
            {
                this.Serialize(sw, data);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Serialize the specified data
        /// </summary>
        public void Serialize(Stream s, IdentifiedData data)
        {

            using (StreamWriter tw = new StreamWriter(s))
                this.Serialize(tw, data);
        }


        /// <summary>
        /// Serialize to the specified text writer
        /// </summary>
        public void Serialize(TextWriter tw, IdentifiedData data)
        {
            try
            {
                using (JsonWriter jw = new JsonTextWriter(tw))
                {
                    this.WritePropertyUtil(jw, null, data, null);
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error serializing {0} : {1}", data, e);
                throw;
            }
        }
    }
}
