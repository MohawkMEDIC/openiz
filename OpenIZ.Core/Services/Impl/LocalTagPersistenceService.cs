/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-4-24
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Tag persistence service for act
    /// </summary>
    public class LocalTagPersistenceService : ITagPersistenceService
    {
        /// <summary>
        /// Save tag
        /// </summary>
        public void Save(Guid sourceKey, ITag tag)
        {
            var cache = ApplicationContext.Current.GetService<IDataCachingService>();

            if (tag is EntityTag)
            {
                var idp = ApplicationContext.Current.GetService<IDataPersistenceService<EntityTag>>();
                var existing = idp.Query(o => o.SourceEntityKey == sourceKey && o.TagKey == tag.TagKey, AuthenticationContext.Current.Principal).FirstOrDefault();
                if (existing != null)
                {
                    existing.Value = tag.Value;
                    if (existing.Value == null)
                        idp.Obsolete(existing, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                    else
                        idp.Update(existing as EntityTag, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                }
                else
                    idp.Insert(tag as EntityTag, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            else if (tag is ActTag)
            {
                var idp = ApplicationContext.Current.GetService<IDataPersistenceService<ActTag>>();
                var existing = idp.Query(o => o.SourceEntityKey == sourceKey && o.TagKey == tag.TagKey, AuthenticationContext.Current.Principal).FirstOrDefault();
                if (existing != null)
                {
                    existing.Value = tag.Value;
                    if (existing.Value == null)
                        idp.Obsolete(existing, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                    else
                        idp.Update(existing as ActTag, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                }
                else
                    idp.Insert(tag as ActTag, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }

            cache.Remove(sourceKey);
            
        }
    }


}
