using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents the root of all model classes in the OpenIZ Core
    /// </summary>
    public abstract class BaseData : IIdentified<Guid>
    {

        // Created by identifier
        private Guid m_createdById;
        // Created by
        private SecurityUser m_createdBy;
        // Obsoleted by
        private Guid? m_obsoletedById;
        // Obsoleted by user
        private SecurityUser m_obsoletedBy;

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
                if (this.m_createdBy == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_createdBy = dataLayer.Get(new Identifier<Guid>(this.m_createdById), null, true);
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
                if (this.m_obsoletedById.HasValue && this.m_obsoletedBy == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_obsoletedBy = dataLayer.Get(new Identifier<Guid>(this.m_obsoletedById.Value), null, true);
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
    }
}
