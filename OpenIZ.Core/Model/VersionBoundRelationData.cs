using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using MARC.Everest.Connectors;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents a relational class which is bound on a version boundary
    /// </summary>
    public abstract class VersionBoundRelationData<TTargetType> : BaseEntityData where TTargetType : VersionedEntityData<TTargetType>
    {

        // The identifier of the version where this data is effective
        private Decimal m_effectiveVersionSequenceId;
        // The identifier of the version where this data is no longer effective
        private Decimal? m_obsoleteVersionSequenceId;
        // The version where this data is effective
        private TTargetType m_effectiveVersion;
        // The version where this data is obsolete
        private TTargetType m_obsoleteVersion;
        // Target entity key
        private Guid m_targetEntityKey;
        // The target entity
        private TTargetType m_targetEntity;

        /// <summary>
        /// Gets or sets the target entity's key
        /// </summary>
        public Guid TargetEntityKey
        {
            get
            {
                return this.m_targetEntityKey;
            }
            set
            {
                this.m_targetEntityKey = value;
                this.m_targetEntity = null;
            }
        }

        /// <summary>
        /// The entity that this relationship targets
        /// </summary>
        public TTargetType TargetEntity
        {
            get
            {
                if (this.m_targetEntity == null &&
                   this.DelayLoad &&
                   this.m_targetEntityKey != default(Guid))
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<TTargetType>>();
                    this.m_targetEntity = dataPersistence.Query(t => t.Key == this.m_targetEntityKey, null).FirstOrDefault();
                }
                return this.m_targetEntity;
            }
            set
            {
                this.m_targetEntity = value;
                if (value == null)
                    this.m_targetEntityKey = default(Guid);
                else
                    this.m_targetEntityKey = value.Key;
            }
        }

        /// <summary>
        /// Gets or sets the effective version of this type
        /// </summary>
        public Decimal EffectiveVersionSequenceId
        {
            get { return this.m_effectiveVersionSequenceId; }
            set
            {
                this.m_effectiveVersionSequenceId = value;
                this.m_effectiveVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the obsoleted version identifier
        /// </summary>
        public Decimal? ObsoleteVersionSequenceId
        {
            get { return this.m_obsoleteVersionSequenceId; }
            set
            {
                this.m_obsoleteVersionSequenceId = value;
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
                    this.m_effectiveVersionSequenceId != default(Decimal))
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<TTargetType>>();
                    this.m_effectiveVersion = dataPersistence.Query(t => t.VersionSequence == this.m_effectiveVersionSequenceId, null).FirstOrDefault();
                }
                return this.m_effectiveVersion;
            }
            set
            {
                this.m_effectiveVersion = value;
                if (value == null)
                    this.m_effectiveVersionSequenceId = default(Decimal);
                else
                    this.m_effectiveVersionSequenceId = value.VersionSequence;
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
                    this.m_obsoleteVersionSequenceId.HasValue)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<TTargetType>>();
                    this.m_obsoleteVersion = dataPersistence.Query(t => t.VersionSequence == this.m_obsoleteVersionSequenceId, null).FirstOrDefault();

                }
                return this.m_obsoleteVersion;
            }
            set
            {
                this.m_obsoleteVersion = value;
                if (value == null)
                    this.m_obsoleteVersionSequenceId = null;
                else
                    this.m_obsoleteVersionSequenceId = value.VersionSequence;
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

        /// <summary>
        /// Validates that this object has a target entity
        /// </summary>
        public override IEnumerable<IResultDetail> Validate()
        {
            var validResults = base.Validate() as List<IResultDetail>;
            if (this.TargetEntityKey == Guid.Empty)
                validResults.Add(new RequiredElementMissingResultDetail(ResultDetailType.Error, String.Format("({0}).{1} required", this.GetType().Name, nameof(TargetEntityKey)), null));
            return validResults;
        }
    }
}
