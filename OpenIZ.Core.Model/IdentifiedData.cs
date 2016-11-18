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
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    [XmlType("IdentifiedData", Namespace = "http://openiz.org/model"), JsonObject("IdentifiedData")]
    public abstract class IdentifiedData : IIdentifiedEntity
    {

        // True when the data class is locked for storage
        private bool m_delayLoad = false;

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
        /// Set delay load
        /// </summary>
        public virtual void SetDelayLoad(bool v)
        {

            List<FieldInfo> fields = new List<FieldInfo>();

            Type typ = this.GetType();

            while (typ != typeof(Object))
            {
                fields.AddRange(typ.GetRuntimeFields().Where(o => !o.IsStatic)); // ... Well now they know..
                typ = typ.GetTypeInfo().BaseType;
            }

            this.m_delayLoad = v;
            foreach (FieldInfo fi in fields)
            {
                object value = fi.GetValue(this);
                if (value is IdentifiedData &&
                    (value as IdentifiedData).IsDelayLoadEnabled != v)
                    (value as IdentifiedData).SetDelayLoad(v); // Let it go
                else if (value is IList &&
                    fi.FieldType.GenericTypeArguments.Length > 0 &&
                    typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(fi.FieldType.GenericTypeArguments[0].GetTypeInfo()))
                {
                    foreach (IdentifiedData itm in value as IList)
                        if (itm.IsDelayLoadEnabled != v)
                            itm?.SetDelayLoad(v);
                }
            }
        }

        /// <summary>
        /// The internal primary key value of the entity
        /// </summary>
        [XmlElement("id"), JsonProperty("id")]
        public Guid? Key { get; set; }

        /// <summary>
        /// True if key should be serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeKey()
        {
            return this.Key.HasValue;
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        [XmlIgnore, JsonProperty("$type")]
        public virtual String Type
        {
            get
            {
                return this.GetType().GetTypeInfo().GetCustomAttribute<JsonObjectAttribute>().Id;
            }
            set { }
        }

        /// <summary>
        /// Get associated entity
        /// </summary>
        protected TEntity DelayLoad<TEntity>(Guid? keyReference, TEntity currentInstance) where TEntity : IdentifiedData, new()
        {
            if (currentInstance == null &&
                this.m_delayLoad &&
                keyReference.HasValue)
            {
                //Debug.WriteLine("Delay loading key reference: {0}>{1}", this.Key, keyReference);
                currentInstance = EntitySource.Current.Get<TEntity>(keyReference.Value);
            }
            //currentInstance?.SetDelayLoad(this.IsDelayLoadEnabled);
            return currentInstance;
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public virtual void Refresh() { }

        /// <summary>
        /// Gets or sets the modified on time
        /// </summary>
        [XmlElement("modifiedOn"), JsonProperty("modifiedOn"), DataIgnore]
        public abstract DateTimeOffset ModifiedOn { get; }

        /// <summary>
        /// Gets a tag which changes whenever the object is updated
        /// </summary>
        [XmlElement("etag"), JsonProperty("etag"), DataIgnore]
        public virtual String Tag
        {
            get
            {
                if (this.Key.HasValue)
                    return BitConverter.ToString(this.Key?.ToByteArray()).Replace("-", "");
                return null;
            }
        }

        /// <summary>
        /// Cleans the identified data of any "empty" stuff
        /// </summary>
        public virtual IdentifiedData Clean() { return this; }

        /// <summary>
        /// True if the object is empty
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEmpty() { return false; }

        /// <summary>
        /// Clone the specified data
        /// </summary>
        public IdentifiedData Clone()
        {
            var retVal = this.MemberwiseClone() as IdentifiedData;
            retVal.m_delayLoad = true;
            return retVal;
        }

        /// <summary>
        /// Clone the specified data
        /// </summary>
        public IdentifiedData GetLocked()
        {
            var retVal = this.MemberwiseClone() as IdentifiedData;
            retVal.SetDelayLoad(false);
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool SemanticEquals(object obj)
        {
            var other = obj as IdentifiedData;
            if (other == null)
                return false;
            return this.Key == other.Key && this.Type == other.Type;
        }
    }
}
