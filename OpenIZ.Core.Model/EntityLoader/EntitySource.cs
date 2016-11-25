/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
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
 * User: justi
 * Date: 2016-6-14
 */
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace OpenIZ.Core.Model.EntityLoader
{
    /// <summary>
    /// Delay loader class
    /// </summary>
    public sealed class EntitySource
    {

        /// <summary>
        /// Dummy entity source
        /// </summary>
        public class DummyEntitySource : IEntitySourceProvider
        {
            /// <summary>
            /// Gets the specified object
            /// </summary>
            public TObject Get<TObject>(Guid? key) where TObject : IdentifiedData, new()
            {
                return new TObject() { Key = key };
            }

            /// <summary>
            /// Gets the specified object
            /// </summary>
            public TObject Get<TObject>(Guid? key, Guid? versionKey) where TObject : IdentifiedData, IVersionedEntity, new()
            {
                return new TObject() { Key = key, VersionKey = versionKey };
            }

            /// <summary>
            /// Gets the specified relations
            /// </summary>
            public IEnumerable<TObject> GetRelations<TObject>(Guid? sourceKey, decimal? sourceVersionSequence) where TObject : IdentifiedData, IVersionedAssociation, new()
            {
	            return new List<TObject>();
            }

            /// <summary>
            /// Gets the specified relations
            /// </summary>
            public IEnumerable<TObject> GetRelations<TObject>(Guid? sourceKey) where TObject : IdentifiedData, ISimpleAssociation, new()
            {
				return new List<TObject>();
			}

            /// <summary>
            /// Query 
            /// </summary>
            public IEnumerable<TObject> Query<TObject>(Expression<Func<TObject, bool>> query) where TObject : IdentifiedData, new()
            {
                return new List<TObject>();
            }
        }

        // Load object
        private static Object s_lockObject = new object();
  
        // Current instance
        private static EntitySource s_instance = new EntitySource(new DummyEntitySource());
        

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
                lock (s_lockObject)
                    s_instance = value;
            }
        }

        /// <summary>
        /// Get the specified object / version
        /// </summary>
        public TObject Get<TObject>(Guid? key, Guid? version) where TObject : IdentifiedData, IVersionedEntity, new()
        {
            if (key == null)
                return null;
            return this.m_provider.Get<TObject>(key, version);
        }

        /// <summary>
        /// Get the current version of the specified object
        /// </summary>
        public TObject Get<TObject>(Guid? key) where TObject : IdentifiedData, new()
        {
            if (key == null)
                return null;
            else
                return this.m_provider.Get<TObject>(key);
        }

        /// <summary>
        /// Gets the current entity source provider
        /// </summary>
        public IEntitySourceProvider Provider {  get { return this.m_provider; } }
    }
}
