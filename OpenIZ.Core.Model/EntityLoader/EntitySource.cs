using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.EntityLoader
{
    /// <summary>
    /// Delay loader class
    /// </summary>
    public sealed class EntitySource
    {

        // Load object
        private static Object s_lockObject = new object();
        // Current instance
        private static EntitySource s_instance;

        /// <summary>
        /// Delay load provider
        /// </summary>
        private IEntitySourceProvider m_provider;

        /// <summary>
        /// Delay loader ctor
        /// </summary>
        public EntitySource(IEntitySourceProvider provider)
        {
            m_provider = provider;
        }

        /// <summary>
        /// Gets the current delay loader
        /// </summary>
        public static EntitySource Current {
            get
            {
                return s_instance;
            }
            set
            {
                if (s_instance == null)
                    lock (s_lockObject)
                        s_instance = value;
                else
                    throw new InvalidOperationException("Current context already set");
            }
        }

        /// <summary>
        /// Get the specified object / version
        /// </summary>
        public TObject Get<TObject>(Guid key, Guid version, TObject currentInstance) where TObject : IdentifiedData
        {
            if (currentInstance == null &&
                version != Guid.Empty)
                return this.m_provider.Get<TObject>(key, version);
            return currentInstance;
        }

        /// <summary>
        /// Get the current version of the specified object
        /// </summary>
        public TObject Get<TObject>(Guid key, TObject currentInstance) where TObject : IdentifiedData
        {
            if (currentInstance == null)
                return this.m_provider.Get<TObject>(key);
            return currentInstance;
        }
         
        /// <summary>
        /// Get version bound relations
        /// </summary>
        public List<TObject> GetRelations<TObject>(Guid sourceKey, Decimal sourceVersionSequence, List<TObject> currentInstance) where TObject : IVersionedAssociation
        {
            if (currentInstance == null)
                return this.m_provider.Query<TObject>(o => sourceKey == o.SourceEntityKey && sourceVersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || sourceVersionSequence < o.ObsoleteVersionSequenceId)).ToList();
            return currentInstance;

        }

        /// <summary>
        /// Get bound relations
        /// </summary>
        public List<TObject> GetRelations<TObject>(Guid sourceKey, List<TObject> currentInstance) where TObject : ISimpleAssociation
        {
            if (currentInstance == null)
                return this.m_provider.Query<TObject>(o => sourceKey == o.SourceEntityKey).ToList();
            return currentInstance;
        }

        /// <summary>
        /// Gets the current entity source provider
        /// </summary>
        public IEntitySourceProvider Provider {  get { return this.m_provider; } }
    }
}
