/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-24
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
    [XmlInclude(typeof(Bundle))]
    [XmlInclude(typeof(ConceptRelationship))]
    [XmlInclude(typeof(ConceptRelationshipType))]
    [XmlInclude(typeof(SecurityUser))]
    public class Bundle : IdentifiedData
    {

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
        /// Create a bundle
        /// </summary>
        public static Bundle CreateBundle(IdentifiedData resourceRoot)
        {
            Bundle retVal = new Bundle();
            if (resourceRoot == null)
                return retVal;

            retVal.Item.Add(resourceRoot);
            ProcessModel(resourceRoot, retVal);
            return retVal;
        }

        /// <summary>
        /// Packages the objects into a bundle
        /// </summary>
        private static void ProcessModel(IdentifiedData model, Bundle currentBundle, bool followList = true)
        {

            foreach(var pi in model.GetType().GetRuntimeProperties().Where(p => p.GetCustomAttribute<DelayLoadAttribute>() != null))
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
                                    currentBundle.Item.Add(itm as IdentifiedData);

                                ProcessModel(itm as IdentifiedData, currentBundle, false);
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
                                currentBundle.Item.Add(iValue);
                            ProcessModel(iValue, currentBundle);
                        }
                    }
                }
                catch(Exception e)
                {
                    // TODO: LOG
                }
            }
        }

    }
}
