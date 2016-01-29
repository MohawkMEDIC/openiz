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
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents data that is identified by a key
    /// </summary>
    [XmlType("IdentifiedData",  Namespace = "http://openiz.org/model"), JsonObject("IdentifiedData")]
    public abstract class IdentifiedData : IIdentifiedEntity
    {
        // True when the data class is locked for storage
        private bool m_delayLoad = true;

        /// <summary>
        /// True if the class is currently loading associations when accessed
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool IsDelayLoadEnabled
        {
            get
            {
                return this.m_delayLoad;
            }
        }

        /// <summary>
        /// Unlock the instance
        /// </summary>
        public void Unlock()
        {
            this.SetDelayLoad(true);
        }

        /// <summary>
        /// Lock the instnace
        /// </summary>
        public void Lock()
        {
            this.SetDelayLoad(false);
        }

        /// <summary>
        /// Set delay load
        /// </summary>
        private void SetDelayLoad(bool v)
        {

            List<FieldInfo> fields = new List<FieldInfo>();

            Type typ = this.GetType();
            
            while (typ != typeof(Object))
            {
                fields.AddRange(typ.GetRuntimeFields().Where(o=>!o.IsStatic && o.IsPrivate)); // ... Well now they know..
                typ = typ.GetTypeInfo().BaseType;
            }

            this.m_delayLoad = v;
            foreach (FieldInfo fi in fields)
            {
                object value = fi.GetValue(this);
                if (value is IdentifiedData)
                {
                    if (!v)
                        (value as IdentifiedData).Lock(); // Let it go
                    else
                        (value as IdentifiedData).Unlock();
                }
                else if (value is IList &&
                    typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(fi.FieldType.GenericTypeArguments[0].GetTypeInfo()))
                {
                    foreach (IdentifiedData itm in value as IList)
                        if (!v)
                            itm.Lock(); // Let it go
                        else
                            itm.Unlock();
                }
            }
        }

        /// <summary>
        /// The internal primary key value of the entity
        /// </summary>
        [XmlElement("id"), JsonProperty("id")]
        public Guid Key { get; set; }

        /// <summary>
        /// Get associated entity
        /// </summary>
        protected TEntity DelayLoad<TEntity>(Guid? keyReference, TEntity currentInstance) where TEntity : IdentifiedData
        {
            if (this.m_delayLoad &&
                keyReference.HasValue)
                currentInstance = EntitySource.Current.Get(keyReference.Value, currentInstance);
            return currentInstance;
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public virtual void Refresh() { }
    }
}
