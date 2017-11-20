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
 * Date: 2016-7-16
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;
using System.Collections;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;

namespace OpenIZ.Core.Model.Collection
{
    /// <summary>
    /// Represents a collection of model items 
    /// </summary>
    [XmlType(nameof(Bundle), Namespace = "http://openiz.org/model")]
    [XmlRoot(nameof(Bundle), Namespace = "http://openiz.org/model")]
    [JsonObject("Bundle")]
    [XmlInclude(typeof(Concept))]
    [XmlInclude(typeof(ReferenceTerm))]
    [XmlInclude(typeof(Act))]
    [XmlInclude(typeof(TextObservation))]
    [XmlInclude(typeof(ConceptSet))]
    [XmlInclude(typeof(CodedObservation))]
    [XmlInclude(typeof(QuantityObservation))]
    [XmlInclude(typeof(PatientEncounter))]
    [XmlInclude(typeof(ExtensionType))]
    [XmlInclude(typeof(SubstanceAdministration))]
    [XmlInclude(typeof(UserEntity))]
    [XmlInclude(typeof(ApplicationEntity))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(Entity))]
    [XmlInclude(typeof(Patient))]
    [XmlInclude(typeof(Provider))]
    [XmlInclude(typeof(Organization))]
    [XmlInclude(typeof(TemplateDefinition))]
    [XmlInclude(typeof(Protocol))]
    [XmlInclude(typeof(Place))]
    [XmlInclude(typeof(Material))]
    [XmlInclude(typeof(ManufacturedMaterial))]
    [XmlInclude(typeof(CarePlan))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(ApplicationEntity))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(PhoneticAlgorithm))]
    [XmlInclude(typeof(Bundle))]
    [XmlInclude(typeof(ConceptClass))]
    [XmlInclude(typeof(ConceptRelationship))]
    [XmlInclude(typeof(ConceptRelationshipType))]
    [XmlInclude(typeof(SecurityUser))]
	[XmlInclude(typeof(SecurityRole))]
	[XmlInclude(typeof(CodeSystem))]
	public class Bundle : IdentifiedData
    {
        /// <summary>
        /// Create new bundle
        /// </summary>
        public Bundle()
        {
            this.Item = new List<IdentifiedData>();
            this.ExpansionKeys = new List<Guid>();
        }

        // Lock object
        private object m_lockObject = new object();

        // Property cache
        private static Dictionary<Type, IEnumerable<PropertyInfo>> m_propertyCache = new Dictionary<System.Type, IEnumerable<PropertyInfo>>();
        private static object s_lockObject = new object();

        // Bundle contents
        private List<IdentifiedData> m_bundleContents = new List<IdentifiedData>();
        private HashSet<String> m_bundleTags = new HashSet<string>(); // hashset of all tags

        // Modified now
        private DateTimeOffset m_modifiedOn = DateTime.Now;

        /// <summary>
        /// Gets the time the bundle was modified
        /// </summary>
        public override DateTimeOffset ModifiedOn
        {
            get
            {
                return this.m_modifiedOn;
            }
        }

        /// <summary>
        /// Clean the bundle
        /// </summary>
        /// <returns></returns>
        public override IdentifiedData Clean()
        {
            for (int i = this.Item.Count - 1; i >= 0; i--)
                if (this.Item[i] == null)
                    this.Item.RemoveAt(i);
                else
                    this.Item[i].Clean() ;
            return this;
        }

        /// <summary>
        /// Gets or sets items in the bundle
        /// </summary>
        [XmlElement("item"), JsonProperty("item")]
        public List<IdentifiedData> Item
        {
            get { return this.m_bundleContents; }
            set { this.m_bundleContents = value; }
        }

        /// <summary>
        /// Entry into the bundle
        /// </summary>
        [XmlElement("entry"), JsonProperty("entry")]
        public Guid? EntryKey { get; set; }

        /// <summary>
        /// Gets the entry object
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IdentifiedData Entry
        {
            get { if (this.EntryKey.HasValue) return this.Item.Find(o => o.Key == this.EntryKey); else return null; }
        }

        /// <summary>
        /// Gets or sets the count in this bundle
        /// </summary>
        [XmlElement("offset"), JsonProperty("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the count in this bundle
        /// </summary>
        [XmlElement("count"), JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the total results
        /// </summary>
        [XmlElement("totalResults"), JsonProperty("totalResults")]
        public int TotalResults { get; set; }


        /// <summary>
        /// Gets or sets the keys of objects that aren't really in the bundle but are expansion items
        /// </summary>
        [XmlElement("result"), JsonProperty("result")]
        public List<Guid> ExpansionKeys { get; set; }

        /// <summary>
        /// Add item to the bundle
        /// </summary>
        public void Add(IdentifiedData data)
        {
            if (data == null) return;
            this.Item.Add(data);
            this.m_bundleTags.Add(data.Tag);
        }

        /// <summary>
        /// Insert data at the specified index
        /// </summary>
        public void Insert(int index, IdentifiedData data)
        {
            if (data == null) return;
            this.Item.Insert(index, data);
            this.m_bundleTags.Add(data.Tag);
        } 

        /// <summary>
        /// True if the bundle has a tag
        /// </summary>
        public bool HasTag(String tag)
        {
            return this.m_bundleTags.Contains(tag);
        }

        /// <summary>
        /// Create a bundle
        /// </summary>
        public static Bundle CreateBundle(IdentifiedData resourceRoot, bool followList = true)
        {
            if (resourceRoot is Bundle) return resourceRoot as Bundle;
            Bundle retVal = new Bundle();
            retVal.Key = Guid.NewGuid();
            retVal.Count = retVal.TotalResults = 1;
            if (resourceRoot == null)
                return retVal;
            retVal.EntryKey = resourceRoot.Key;
            retVal.Add(resourceRoot);
            ProcessModel(resourceRoot, retVal, followList);
            return retVal;
        }

        /// <summary>
        /// Create a bundle
        /// </summary>
        public static Bundle CreateBundle(IEnumerable<IdentifiedData> resourceRoot, int totalResults, int offset)
        {
            Bundle retVal = new Bundle();
            retVal.Key = Guid.NewGuid();
            retVal.Count = resourceRoot.Count();
            retVal.Offset = offset;
            retVal.TotalResults = totalResults;
            if (resourceRoot == null)
                return retVal;

            // Resource root
            foreach (var itm in resourceRoot)
            {
                if (itm == null)
                    continue;
                if (!retVal.Item.Exists(o => o.Tag == itm.Tag) && itm.Key.HasValue)
                {
                    retVal.Add(itm.GetLocked());
                    Bundle.ProcessModel(itm.GetLocked() as IdentifiedData, retVal);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Reconstitutes the bundle (takes the flat reference structures and fills them out into proper object references)
        /// </summary>
        public void Reconstitute()
        {
            HashSet<IdentifiedData> context = new HashSet<IdentifiedData>();
            foreach (var itm in this.Item)
            {
                this.Reconstitute(itm, context);
            }
        }

        /// <summary>
        /// Re-constitute the data
        /// </summary>
        /// <remarks>Basically this will find any refs and fill them in</remarks>
        private void Reconstitute(IdentifiedData data, HashSet<IdentifiedData> context)
        {
            if (context.Contains(data))
                return;
            context.Add(data);
            // Prevent delay loading from EntitySource (we're doing that right now)

            // Iterate over properties
            foreach (var pi in data.GetType().GetRuntimeProperties().Where(o => o.GetCustomAttribute<DataIgnoreAttribute>() == null))
            {

                // Is this property not null? If so, we want to iterate
                object value = pi.GetValue(data);
                if (value is IList)
                {
                    foreach (var itm in value as IList)
                        if (itm is IdentifiedData)
                            this.Reconstitute(itm as IdentifiedData, context);
                }
                else if (value is IdentifiedData)
                    this.Reconstitute(value as IdentifiedData, context);

                // Is the pi a delay load? if so then get the key property
                var keyName = pi.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty;
                if (keyName == null || pi.SetMethod == null)
                    continue; // Skip if there is no delay load or if we can't even set this property

                // Now we get the value of the key
                var keyPi = data.GetType().GetRuntimeProperty(keyName);
                if (keyPi == null || (keyPi.PropertyType != typeof(Guid) &&
                    keyPi.PropertyType != typeof(Guid?)))
                    continue; // Invalid key link name

                // Get the key and find a match
                var key = (Guid?)keyPi.GetValue(data);
                var bundleItem = this.Item.Find(o => o.Key == key);
                if(bundleItem != null)
                    pi.SetValue(data, bundleItem);
                
            }

            context.Remove(data);

        }

        /// <summary>
        /// Packages the objects into a bundle
        /// </summary>
        public static void ProcessModel(IdentifiedData model, Bundle currentBundle, bool followList = true)
        {
            try
            {

                // Iterate over properties
                IEnumerable<PropertyInfo> properties = null;
                if (!m_propertyCache.TryGetValue(model.GetType(), out properties))
                    lock (s_lockObject)
                    {
                        properties = model.GetType().GetRuntimeProperties().Where(p => p.GetCustomAttribute<SerializationReferenceAttribute>() != null ||
                            typeof(IList).GetTypeInfo().IsAssignableFrom(p.PropertyType.GetTypeInfo()) && p.GetCustomAttributes<XmlElementAttribute>().Count() > 0 && followList).ToList();

	                    if (!m_propertyCache.ContainsKey(model.GetType()))
	                    {
							m_propertyCache.Add(model.GetType(), properties);
						}
                    }
                
                currentBundle.m_modifiedOn = DateTimeOffset.Now;
                foreach (var pi in properties)
                {
                    try
                    {
                        object rawValue = pi.GetValue(model);
                        if (rawValue == null) continue;


                        if (rawValue is IList && followList)
                        {
                            foreach (var itm in rawValue as IList)
                            {

                                var iValue = itm as IdentifiedData;
                                if (iValue != null)
                                {
                                    if (currentBundle.Item.Exists(o => o?.Tag == iValue?.Tag))
                                        continue;

                                    if (pi.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                                        lock (currentBundle.m_lockObject)
                                            if (!currentBundle.HasTag(iValue.Tag) && iValue.Key.HasValue)
                                            {
                                                currentBundle.ExpansionKeys.Add(iValue.Key.Value);
                                                currentBundle.Insert(0, iValue);
                                            }
                                    ProcessModel(iValue, currentBundle, false);
                                }
                            }
                        }
                        else if (rawValue is IdentifiedData)
                        {
                            var iValue = rawValue as IdentifiedData;

                            // Check for existing item
                            if (iValue != null && !currentBundle.HasTag(iValue.Tag ))
                            {
                                if (pi.GetCustomAttribute<XmlIgnoreAttribute>() != null && iValue != null)
                                    lock (currentBundle.m_lockObject)
                                        if (!currentBundle.HasTag(iValue.Tag) && iValue.Key.HasValue)
                                        {
                                            currentBundle.ExpansionKeys.Add(iValue.Key.Value);
                                            currentBundle.Insert(0, iValue);
                                        }
                                ProcessModel(iValue, currentBundle, followList);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Instance error: {0}", e);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("Error: {0}", e);
            }
        }


        /// <summary>
        /// Gets from the item set only those items which are for expansion
        /// </summary>
        public IEnumerable<IdentifiedData> GetExpansionItems()
        {
            return this.Item.Where(o => this.ExpansionKeys.Contains(o.Key.Value));
        }

        /// <summary>
        /// Gets from the item set only those items which are results
        /// </summary>
        public IEnumerable<IdentifiedData> GetResultItems()
        {
            return this.Item.Where(o => !this.ExpansionKeys.Contains(o.Key.Value));
        }

    }
}
