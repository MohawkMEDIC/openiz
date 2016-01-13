using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents a relational class which is bound on a version boundary
    /// </summary>
    public abstract class VersionBoundRelationData<TTargetType> : BaseEntityData where TTargetType : VersionedEntityData
    {

        // The identifier of the version where this data is effective
        private Guid m_effectiveVersionId;
        // The identifier of the version where this data is no longer effective
        private Guid? m_obsoleteVersionId;
        // The version where this data is effective
        private TTargetType m_effectiveVersion;
        // The version where this data is obsolete
        private TTargetType m_obsoleteVersion;

        /// <summary>
        /// Gets or sets the effective version of this type
        /// </summary>
        public Guid EffectiveVersionId
        {
            get { return this.m_effectiveVersionId; }
            set
            {
                this.m_effectiveVersionId = value;
                this.m_effectiveVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the obsoleted version identifier
        /// </summary>
        public Guid? ObsoleteVersionId
        {
            get { return this.m_obsoleteVersionId; }
            set
            {
                this.m_obsoleteVersionId = value;
                this.m_obsoleteVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the effective version
        /// </summary>
        public TTargetType EffectiveVersion
        {
            get
            {
                if(this.m_effectiveVersion == null &&
                    this.DelayLoad &&
                    this.m_effectiveVersionId != Guid.Empty)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<TTargetType>>();
                    this.m_effectiveVersion = dataPersistence.Query(t => t.VersionKey == this.m_effectiveVersionId, null).FirstOrDefault();
                }
                return this.m_effectiveVersion;
            }
            set
            {
                this.m_effectiveVersion = value;
                if (value == null)
                    this.m_effectiveVersionId = Guid.Empty;
                else
                    this.m_effectiveVersionId = value.Key;
            }
        }

        /// <summary>
        /// Gets the obsoletion version
        /// </summary>
        public TTargetType ObsoleteVersion
        {
            get
            {
                if(this.m_obsoleteVersion == null &&
                    this.DelayLoad &&
                    this.m_obsoleteVersionId.HasValue)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<TTargetType>>();
                    this.m_obsoleteVersion = dataPersistence.Query(t => t.VersionKey == this.m_obsoleteVersionId, null).FirstOrDefault();

                }
                return this.m_obsoleteVersion;
            }
            set
            {
                this.m_obsoleteVersion = value;
                if (value == null)
                    this.m_obsoleteVersionId = Guid.Empty;
                else
                    this.m_obsoleteVersionId = value.Key;
            }
        }

        /// <summary>
        /// Gets or sets the user that created this relationship
        /// </summary>
        public override SecurityUser CreatedBy
        {
            get
            {
                return this.EffectiveVersion?.CreatedBy;
            }
        }

        /// <summary>
        /// Gets the identifier of the user that created this relationship
        /// </summary>
        public override Guid CreatedById
        {
            get
            {
                return (Guid)this.EffectiveVersion?.CreatedById;
            }
            set
            {
                throw new NotSupportedException("CreatedById is based on EffectiveVersion property");
            }
        }

        /// <summary>
        /// Obsoleted by
        /// </summary>
        public override SecurityUser ObsoletedBy
        {
            get
            {
                return this.ObsoleteVersion?.CreatedBy;
            }
        }

        /// <summary>
        /// Gets the identifier of the user that obsoleted the relationship
        /// </summary>
        public override Guid? ObsoletedById
        {
            get
            {
                return this.ObsoleteVersion?.ObsoletedById;
            }
            set
            {
                throw new NotSupportedException("ObsoletedById is based on EffectiveVersion property");
            }
        }
    }
}
