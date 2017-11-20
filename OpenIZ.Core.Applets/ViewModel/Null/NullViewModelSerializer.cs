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
 * Date: 2017-2-25
 */
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
using OpenIZ.Core.Diagnostics;
using System.Collections;
using OpenIZ.Core.Model.EntityLoader;
using System.Diagnostics;
using OpenIZ.Core.Model.Attributes;

namespace OpenIZ.Core.Applets.ViewModel.Null
{
    /// <summary>
    /// Represents a serializer that 
    /// </summary>
    public class NullViewModelSerializer : IViewModelSerializer
    {

        // Tracer for the class
        private Tracer m_tracer = Tracer.GetTracer(typeof(NullViewModelSerializer));
        // Sync lock
        private Object m_syncLock = new object();

        // Static ync lock
        private static object s_syncLock = new Object();

        // Related load methods
        private static Dictionary<Type, MethodInfo> m_relatedLoadMethods = new Dictionary<Type, MethodInfo>();
        // Classifiers
        private static Dictionary<Type, IViewModelClassifier> m_classifiers = new Dictionary<Type, IViewModelClassifier>();

        // Reloated load association
        private static Dictionary<Type, MethodInfo> m_relatedLoadAssociations = new Dictionary<Type, MethodInfo>();
        private Dictionary<Guid, IEnumerable> m_loadedAssociations = new Dictionary<Guid, IEnumerable>();

        // Formatters
        private  Dictionary<Type, INullTypeFormatter> m_formatters = new Dictionary<Type, INullTypeFormatter>();


        /// <summary>
        /// Gets or sets the view model definnition
        /// </summary>
        public ViewModelDescription ViewModel { get; set; }

        /// <summary>
        /// Deserialize a memory stream obbject
        /// </summary>
        [Obsolete("Deserialization is not supported on the null serializer", true)]
        public object DeSerialize(Stream s, Type t)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Deserialize the memory stream
        /// </summary>
        [Obsolete("Deserialization is not supported on the null serializer", true)]
        public TModel DeSerialize<TModel>(Stream s)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Load related object
        /// </summary>
        internal object LoadRelated(Type propertyType, Guid key)
        {
            MethodInfo methodInfo = null;
            if (!m_relatedLoadMethods.TryGetValue(propertyType, out methodInfo))
            {
                methodInfo = this.GetType().GetRuntimeMethod(nameof(LoadRelated), new Type[] { typeof(Guid) }).MakeGenericMethod(propertyType);
                lock (m_relatedLoadMethods)
                    if (!m_relatedLoadMethods.ContainsKey(propertyType))
                        m_relatedLoadMethods.Add(propertyType, methodInfo);

            }
            return methodInfo.Invoke(this, new object[] { key });
        }

        /// <summary>
        /// Load collection
        /// </summary>
        internal IList LoadCollection(Type propertyType, Guid key)
        {
            MethodInfo methodInfo = null;

            // Load
            if (!m_relatedLoadAssociations.TryGetValue(propertyType, out methodInfo))
            {
                methodInfo = this.GetType().GetRuntimeMethod(nameof(LoadCollection), new Type[] { typeof(Guid) }).MakeGenericMethod(propertyType.StripGeneric());
                lock (m_relatedLoadAssociations)
                    if (!m_relatedLoadAssociations.ContainsKey(propertyType))
                        m_relatedLoadAssociations.Add(propertyType, methodInfo);

            }
            var listValue = methodInfo.Invoke(this, new object[] { key }) as IList;
            if (propertyType.GetTypeInfo().IsAssignableFrom(listValue.GetType().GetTypeInfo()))
                return listValue;
            else
            {
                var retVal = Activator.CreateInstance(propertyType, listValue);
                return retVal as IList;
            }

        }

        /// <summary>
        /// Delay loads the specified collection association
        /// </summary>
        public IEnumerable<TAssociation> LoadCollection<TAssociation>(Guid sourceKey) where TAssociation : IdentifiedData, ISimpleAssociation, new()
        {
#if DEBUG
            this.m_tracer.TraceVerbose("Delay loading collection for object : {0}", sourceKey);
#endif
            // Have we already loaded this in the current serializer?
            IEnumerable association = null;
            if (!this.m_loadedAssociations.TryGetValue(sourceKey, out association))
            {
                association = EntitySource.Current.Provider.GetRelations<TAssociation>(sourceKey);
                if (this.m_loadedAssociations.ContainsKey(sourceKey))
                    this.m_loadedAssociations.Add(sourceKey, association);
            }
            return association as IEnumerable<TAssociation>;
        }

        /// <summary>
        /// Load the related information
        /// </summary>
        public TRelated LoadRelated<TRelated>(Guid? objectKey) where TRelated : IdentifiedData, new()
        {
#if DEBUG
            this.m_tracer.TraceVerbose("Delay loading related object : {0}", objectKey);
#endif
            if (objectKey.HasValue)
                return EntitySource.Current.Provider.Get<TRelated>(objectKey);
            else
                return default(TRelated);
        }

        /// <summary>
        /// Load a serializer assembly (not supported)
        /// </summary>
        public void LoadSerializerAssembly(Assembly asm)
        {
            var typeFormatters = asm.ExportedTypes.Where(o => typeof(INullTypeFormatter).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && o.GetTypeInfo().IsClass)
              .Select(o => Activator.CreateInstance(o) as INullTypeFormatter)
              .Where(o => !m_formatters.ContainsKey(o.HandlesType));
            foreach (var fmtr in typeFormatters)
                m_formatters.Add(fmtr.HandlesType, fmtr);
        }

        /// <summary>
        /// Get the specified formatter
        /// </summary>
        /// <param name="type">The type to retrieve the formatter for</param>
        public INullTypeFormatter GetFormatter(Type type)
        {
            INullTypeFormatter typeFormatter = null;
            if (!this.m_formatters.TryGetValue(type, out typeFormatter))
            {
                typeFormatter = new NullReflectionTypeFormatter(type);
                lock (this.m_syncLock)
                    if (!this.m_formatters.ContainsKey(type))
                        this.m_formatters.Add(type, typeFormatter);
            }
            return typeFormatter;
        }

        /// <summary>
        /// Gets the appropriate classifier for the specified type
        /// </summary>
        public IViewModelClassifier GetClassifier(Type type)
        {
            IViewModelClassifier retVal = null;
            if (!m_classifiers.TryGetValue(type, out retVal))
            {
                var classifierAtt = type.StripGeneric().GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                if (classifierAtt != null)
                    retVal = new Json.JsonReflectionClassifier(type);
                lock (m_classifiers)
                    if (!m_classifiers.ContainsKey(type))
                        m_classifiers.Add(type, retVal);
            }
            return retVal;
        }
        
        /// <summary>
        /// Write property utility
        /// </summary>
        public void WritePropertyUtil(String propertyName, Object instance, SerializationContext context, bool noSubContext = false)
        {

            if (instance == null) return;

            // first write the property
            if (!String.IsNullOrEmpty(propertyName) && context?.ShouldSerialize(propertyName) == false)
                return;

            // Instance data
            if (instance is IdentifiedData)
            {
                var identifiedData = instance as IdentifiedData;

                // Complex type .. allow the formatter to handle this
                INullTypeFormatter typeFormatter = this.GetFormatter(instance.GetType());

                var subContext = noSubContext ? context as NullSerializationContext : new NullSerializationContext(propertyName, this, instance, context);
                typeFormatter.Serialize(instance as IdentifiedData, subContext);
            }
            else if (instance is IList && !instance.GetType().IsArray)
            {
                // Classifications?
                var classifier = this.GetClassifier(instance.GetType().StripNullable());

                if (classifier == null) // no classifier
                {
                    foreach (var itm in instance as IList)
                        this.WritePropertyUtil(null, itm, new NullSerializationContext(propertyName, this, instance, context as NullSerializationContext), noSubContext);
                }
                else
                {
                    foreach (var cls in classifier.Classify(instance as IList))
                    {
                        Object value = new List<Object>(cls.Value as IEnumerable<Object>);
                        if (cls.Value.Count == 1)
                            value = cls.Value[0];
                        // Now write
                        this.WritePropertyUtil(cls.Key, value, new NullSerializationContext(propertyName, this, instance, context as NullSerializationContext), value is IList);
                    }
                }
            }

        }

        /// <summary>
        /// Serializer the object to the specified text writer
        /// </summary>
        public void Serialize(TextWriter s, IdentifiedData data)
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
            this.m_tracer.TraceVerbose("PERF >>> SERIALIZING {0}", data);
#endif
            try
            {
                this.WritePropertyUtil(null, data, null);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error serializing {0} : {1}", data, e);
                throw;
            }
            finally
            {
#if DEBUG
                sw.Stop();
                this.m_tracer.TraceVerbose("PERF >>> SERIALIZED {0} IN {1} ms", data, sw.ElapsedMilliseconds);
#endif
            }
        }

        /// <summary>
        /// Serialize the specified object to the stream (null writer doesn't do anything)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="data"></param>
        public void Serialize(Stream s, IdentifiedData data)
        {
            this.Serialize((TextWriter)null, data);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        public String Serialize(IdentifiedData data)
        {
            this.Serialize((TextWriter)null, data);
            return String.Empty;
        }
    }
}
