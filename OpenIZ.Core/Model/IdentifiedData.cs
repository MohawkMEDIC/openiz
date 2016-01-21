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
 * Date: 2016-1-19
 */
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents data that is identified by a key
    /// </summary>
    [Serializable]
    [DataContract(Name = "IdentifiedData", Namespace = "http://openiz.org/model")]
    public abstract class IdentifiedData : IIdentified<Guid>
    {
        // True when the data class is locked for storage
        [NonSerialized]
        private bool m_delayLoad = true;

        /// <summary>
        /// True if the class is currently loading associations when accessed
        /// </summary>
        [IgnoreDataMember]
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
                fields.AddRange(typ.GetFields(BindingFlags.NonPublic // Don't let them in, don't let them see ... 
                    | BindingFlags.Instance)); // ... Well now they know..
                typ = typ.BaseType;
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
                    typeof(IdentifiedData).IsAssignableFrom(fi.FieldType.GetGenericArguments()[0]))
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
        [DataMember(Name = "id")]
        public Guid Key { get; set; }


        /// <summary>
        /// Gets or sets the Id of the base data
        /// </summary>
        [IgnoreDataMember]
        public virtual Identifier<Guid> Id
        {
            get
            {
                return new Identifier<Guid>()
                {
                    Id = this.Key
                };
            }
            set
            {
                // TODO: Compare the AA to configuration
                this.Key = value.Id;
            }
        }

        /// <summary>
        /// Get associated entity
        /// </summary>
        protected TEntity DelayLoad<TEntity>(Guid? keyReference, TEntity currentInstance)
        {
            if(currentInstance == null &&
                this.m_delayLoad &&
                keyReference.HasValue &&
                keyReference.Value != default(Guid))
            {
                var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();
                currentInstance = persistenceService.Get(new Identifier<Guid>(keyReference.Value), null, true);
            }
            return currentInstance;
        }

        /// <summary>
        /// Validate the state of this object
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IResultDetail> Validate()
        {
            return new List<IResultDetail>();
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public virtual void Refresh() { }
    }
}
