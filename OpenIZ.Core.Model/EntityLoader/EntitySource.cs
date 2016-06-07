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
 * Date: 2016-2-1
 */
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
        public TObject Get<TObject>(Guid key, Guid version, TObject currentInstance) where TObject : IdentifiedData, IVersionedEntity
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
        public List<TObject> GetRelations<TObject>(Guid sourceKey, Decimal sourceVersionSequence, List<TObject> currentInstance) where TObject : IdentifiedData, IVersionedAssociation
        {
            if (currentInstance == null)
                return this.m_provider.Query<TObject>(o => sourceKey == o.SourceEntityKey && sourceVersionSequence >= o.EffectiveVersionSequenceId && (o.ObsoleteVersionSequenceId == null || sourceVersionSequence < o.ObsoleteVersionSequenceId)).ToList();
            return currentInstance;

        }

        /// <summary>
        /// Get bound relations
        /// </summary>
        public List<TObject> GetRelations<TObject>(Guid sourceKey, List<TObject> currentInstance) where TObject : IdentifiedData, ISimpleAssociation
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
