using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents data that is identified by a key
    /// </summary>
    public abstract class IdentifiedData : IIdentified<Guid>
    {
        // True when the data class is locked for storage
        private bool m_delayLoad = true;

        /// <summary>
        /// True if the class is currently loading associations when accessed
        /// </summary>
        public bool DelayLoad
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
        public Guid Key { get; set; }


        /// <summary>
        /// Gets or sets the Id of the base data
        /// </summary>
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
        /// Validate the state of this object
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IResultDetail> Validate()
        {
            return new List<IResultDetail>();
        }
    }
}
