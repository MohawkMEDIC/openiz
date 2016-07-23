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
    [XmlInclude(typeof(Concept))]
    [XmlInclude(typeof(ReferenceTerm))]
    [XmlInclude(typeof(Act))]
    [XmlInclude(typeof(TextObservation))]
    [XmlInclude(typeof(ConceptSet))]
    [XmlInclude(typeof(CodedObservation))]
    [XmlInclude(typeof(QuantityObservation))]
    [XmlInclude(typeof(PatientEncounter))]
    [XmlInclude(typeof(SubstanceAdministration))]
    [XmlInclude(typeof(Entity))]
    [XmlInclude(typeof(Patient))]
    [XmlInclude(typeof(Provider))]
    [XmlInclude(typeof(Organization))]
    [XmlInclude(typeof(Place))]
    [XmlInclude(typeof(Material))]
    [XmlInclude(typeof(ManufacturedMaterial))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(ApplicationEntity))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(PhoneticAlgorithm))]
    [XmlInclude(typeof(Bundle))]
    [XmlInclude(typeof(ConceptClass))]
    [XmlInclude(typeof(ConceptRelationship))]
    [XmlInclude(typeof(ConceptRelationshipType))]
    [XmlInclude(typeof(SecurityUser))]
    public class Bundle : IdentifiedData
    {

        // Lock object
        private object m_lockObject = new object();

        /// <summary>
        /// Represents bundle contents
        /// </summary>
        private List<IdentifiedData> m_bundleContents = new List<IdentifiedData>();

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
            get { return this.Item.Find(o => o.Key == this.EntryKey); }
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
        /// Create a bundle
        /// </summary>
        public static Bundle CreateBundle(IdentifiedData resourceRoot)
        {
            Bundle retVal = new Bundle();
            retVal.Key = Guid.NewGuid();
            retVal.Count = retVal.TotalResults = 1;
            if (resourceRoot == null)
                return retVal;
            retVal.EntryKey = resourceRoot.Key;
            retVal.Item.Add(resourceRoot);
            ProcessModel(resourceRoot, retVal);
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
                if (!retVal.Item.Exists(o => o.Key == itm.Key))
                {
                    retVal.Item.Add(itm.GetLocked());
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
            foreach(var itm in this.Item)
                this.Reconstitute(itm);
        }
        
        /// <summary>
        /// Re-constitute the data
        /// </summary>
        /// <remarks>Basically this will find any refs and fill them in</remarks>
        private void Reconstitute(IdentifiedData data)
        {
            // Prevent delay loading from EntitySource (we're doing that right now)
            bool originalDelayLoad = data.IsDelayLoadEnabled;
            data.SetDelayLoad(false);

            // Iterate over properties
            foreach (var pi in data.GetType().GetRuntimeProperties())
            {

                // Is this property not null? If so, we want to iterate
                object value = pi.GetValue(data);
                if (value is IList)
                {
                    foreach (var itm in value as IList)
                        if (itm is IdentifiedData)
                            this.Reconstitute(itm as IdentifiedData);
                }
                else if (value is IdentifiedData)
                    this.Reconstitute(value as IdentifiedData);

                // Is the pi a delay load? if so then get the key property
                var keyName = pi.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty;
                if (keyName == null || pi.SetMethod == null)
                    continue; // Skip if there is no delay load or if we can't even set this property

                // Now we get the value of the key
                var keyPi = data.GetType().GetRuntimeProperty(keyName);
                if (keyPi == null)
                    continue; // Invalid key link name

                // Get the key and find a match
                var key = (Guid?)keyPi.GetValue(data);
                var bundleItem = this.Item.Find(o => o.Key == key);
                if(bundleItem != null)
                    pi.SetValue(data, bundleItem);
                
            }

            data.SetDelayLoad(originalDelayLoad);

        }

        /// <summary>
        /// Packages the objects into a bundle
        /// </summary>
        public static void ProcessModel(IdentifiedData model, Bundle currentBundle, bool followList = true)
        {
            try
            {
                foreach (var pi in model.GetType().GetRuntimeProperties().Where(p => p.GetCustomAttribute<SerializationReferenceAttribute>() != null))
                {
                    try
                    {
                        object rawValue = pi.GetValue(model);
                        if (rawValue == null) continue;

                        if (rawValue is IList && followList)
                        {
                            foreach (var itm in rawValue as IList)
                            {

                                if (itm is IdentifiedData)
                                {
                                    if (currentBundle.Item.Exists(o => o.Key == (itm as IdentifiedData).Key))
                                        continue;

                                    if (pi.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                                        lock (currentBundle.m_lockObject)
                                            if (!currentBundle.Item.Exists(o => o.Key == (itm as IdentifiedData).Key))
                                                currentBundle.Item.Add(itm as IdentifiedData);

                                    ProcessModel(itm as IdentifiedData, currentBundle, true);
                                }
                            }
                        }
                        else if (rawValue is IdentifiedData)
                        {
                            var iValue = rawValue as IdentifiedData;
                            var versionedValue = rawValue as IVersionedEntity;

                            // Check for existing item
                            if (!currentBundle.Item.Exists(i => i.Key == iValue.Key && versionedValue?.VersionKey == (i as IVersionedEntity)?.VersionKey))
                            {
                                if (pi.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                                    lock (currentBundle.m_lockObject)
                                        if (!currentBundle.Item.Exists(o => o.Key == iValue.Key))
                                            currentBundle.Item.Add(iValue);
                                ProcessModel(iValue, currentBundle);
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

    }
}
