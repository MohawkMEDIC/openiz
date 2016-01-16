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
        protected bool m_delayLoad = true;

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
        /// Clones this object member for member with delay load disabled
        /// </summary>
        public IdentifiedData AsFrozen() // Not the movie
        {
            IdentifiedData retVal = this.GetType().GetConstructor(Type.EmptyTypes).Invoke(null) as IdentifiedData; 
            List<FieldInfo> fields = new List<FieldInfo>();
            Type typ = this.GetType();
            while (typ != typeof(Object))
            {
                fields.AddRange(typ.GetFields(BindingFlags.NonPublic // Don't let them in, don't let them see ... 
                    | BindingFlags.Instance)); // ... Well now they know..
                typ = typ.BaseType;
            }

            foreach (FieldInfo fi in fields)
            {
                object value = fi.GetValue(this);
                if (value is IdentifiedData)
                    value = (value as IdentifiedData).AsFrozen(); // Let it go
                else if(value is IList && 
                    typeof(IdentifiedData).IsAssignableFrom(fi.FieldType.GetGenericArguments()[0]))
                {
                    var newList = fi.FieldType.GetConstructor(Type.EmptyTypes).Invoke(null) as IList;
                    foreach (IdentifiedData itm in value as IList)
                        newList.Add(itm.AsFrozen()); // Let it go
                    value = newList;
                }
                fi.SetValue(retVal, value); // Can't hold it back anymore... 
            }

            retVal.m_delayLoad = false; 
            return retVal;
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
