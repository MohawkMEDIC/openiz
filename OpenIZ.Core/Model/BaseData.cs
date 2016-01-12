using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents the root of all model classes in the OpenIZ Core
    /// </summary>
    public abstract class BaseData : IIdentified<Guid>
    {

        // True when the data class is locked for storage
        protected bool m_delayLoad = true;
        
        // Created by identifier
        private Guid m_createdById;
        // Created by
        private SecurityUser m_createdBy;
        // Obsoleted by
        private Guid? m_obsoletedById;
        // Obsoleted by user
        private SecurityUser m_obsoletedBy;

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
        public BaseData AsFrozen()
        {
            BaseData retVal = Activator.CreateInstance(this.GetType()) as BaseData;
            List<FieldInfo> fields = new List<FieldInfo>();
            Type typ = this.GetType();
            while (typ != typeof(Object))
            {
                fields.AddRange(typ.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                typ = typ.BaseType;
            }

            foreach (FieldInfo fi in fields)
            {
                object value = fi.GetValue(this);
                if(value is BaseData)
                    value = (value as BaseData).AsFrozen();
                fi.SetValue(retVal, value);
            }
            retVal.m_delayLoad = false;
            return retVal;
        }


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
        /// The internal primary key value of the entity
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Creation Time
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Obsoletion time
        /// </summary>
        public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the user that created this base data
        /// </summary>
        public SecurityUser CreatedBy {
            get
            {
                if (this.m_delayLoad && this.m_createdById != Guid.Empty && this.m_createdBy == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_createdBy = dataLayer.Get(new Identifier<Guid>() { Id = this.m_createdById }, null, true);
                }
                return this.m_createdBy;
            }
            set
            {
                if(value == null)
                {
                    this.m_createdBy = null;
                    this.m_createdById = Guid.Empty;
                }
                else
                {
                    this.m_createdBy = value;
                    this.m_createdById = value.Key;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the user that obsoleted this base data
        /// </summary>
        public SecurityUser ObsoletedBy {
            get
            {
                if (this.m_delayLoad && this.m_obsoletedById.HasValue && this.m_obsoletedBy == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_obsoletedBy = dataLayer.Get(new Identifier<Guid>() { Id = this.m_obsoletedById.Value }, null, true);
                }
                return this.m_obsoletedBy;
            }
            set
            {
                if(value == null)
                {
                    this.m_obsoletedBy = null;
                    this.m_obsoletedById = null;
                }
                else
                {
                    this.m_obsoletedBy = value;
                    this.m_obsoletedById = value.Key;
                }
            }
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid CreatedById
        {
            get { return this.m_createdById; }
            set
            {
                if (this.m_createdById != value)
                    this.m_createdBy = null;
                this.m_createdById = value;
            }
        }

        /// <summary>
        /// Gets or sets the obsoleted by identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? ObsoletedById
        {
            get { return this.m_obsoletedById; }
            set
            {
                if (this.m_obsoletedById != value)
                    this.m_obsoletedBy = null;
                this.m_obsoletedById = value;
            }
        }

        /// <summary>
        /// Represent the data as a string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} (K:{1})", this.GetType().Name, this.Key);
        }
    }
}
