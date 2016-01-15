using MARC.Everest.Connectors;
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
    public abstract class BaseEntityData : IdentifiedData
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
        public virtual SecurityUser CreatedBy {
            get
            {
                if (this.m_delayLoad && this.m_createdById != Guid.Empty && this.m_createdBy == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_createdBy = dataLayer.Get(new Identifier<Guid>() { Id = this.m_createdById }, null, true);
                }
                return this.m_createdBy;
            }
         }
        
        /// <summary>
        /// Gets or sets the user that obsoleted this base data
        /// </summary>
        public virtual SecurityUser ObsoletedBy {
            get
            {
                if (this.m_delayLoad && this.m_obsoletedById.HasValue && this.m_obsoletedBy == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_obsoletedBy = dataLayer.Get(new Identifier<Guid>() { Id = this.m_obsoletedById.Value }, null, true);
                }
                return this.m_obsoletedBy;
            }
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Guid CreatedById
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
        public virtual Guid? ObsoletedById
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
