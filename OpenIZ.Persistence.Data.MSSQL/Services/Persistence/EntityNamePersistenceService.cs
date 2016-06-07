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
 * Date: 2016-4-19
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Data.Linq;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Persistence of entity names
    /// </summary>
    public class EntityNamePersistenceService : VersionedAssociationPersistenceService<Core.Model.Entities.EntityName, Core.Model.Entities.Entity, Data.EntityName>
    {
        /// <summary>
        /// Convert to model
        /// </summary>
        internal override Core.Model.Entities.EntityName ConvertToModel(object data)
        {
            if (data == null)
                return null;

            var name = data as Data.EntityName;

            var retVal = DataCache.Current.Get(name.EntityNameId) as Core.Model.Entities.EntityName;
            if (retVal == null)
            {
                retVal = this.ConvertItem(name);

                ConceptPersistenceService cp = new ConceptPersistenceService();
                if (name.NameUseConcept != null)
                    retVal.NameUse = cp.ConvertItem(name.NameUseConcept.CurrentVersion());
                if (name.EntityNameComponents != null)
                {
                    retVal.Component = new List<Core.Model.Entities.EntityNameComponent>();
                    retVal.Component.AddRange(
                        name.EntityNameComponents.Select(o=>new Core.Model.Entities.EntityNameComponent()
                        {
                            ComponentTypeKey = o.ComponentTypeConceptId,
                            Key = o.EntityNameComponentId,
                            PhoneticAlgorithmKey = o.PhoneticValue.PhoneticAlgorithmId,
                            PhoneticCode = o.PhoneticValue.PhoneticCode,
                            Value = o.PhoneticValue.Value,
                            SourceEntityKey = retVal.Key,
                        }));
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the table that backs this particular persister
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Table<Data.EntityName> GetDataTable(ModelDataContext context)
        {
            return context.EntityNames;
        }

        /// <summary>
        /// Insert the entity name into the database, optionally creating a new version of the entity
        /// </summary>
        internal override Core.Model.Entities.EntityName Insert(Core.Model.Entities.EntityName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            var domainName = this.ConvertFromModel(storageData) as Data.EntityName;

            // Ensure that the use code exists
            if (storageData.NameUse != null)
                domainName.NameUseConceptId = storageData.NameUse.EnsureExists(principal, dataContext).Key;

            // Get the current version & create a new version if needed
            var currentEntityVersion = dataContext.EntityVersions.Single(o => o.EntityId == storageData.SourceEntityKey && o.ObsoletionTime == null);
            EntityVersion newEntityVersion = newVersion ? currentEntityVersion.NewVersion(principal, dataContext) : currentEntityVersion;
            domainName.EffectiveVersionSequenceId = newEntityVersion.VersionSequenceId;
            domainName.Entity = newEntityVersion.Entity;

            // Get phonetic service
            var phoneticAlgorithm = ApplicationContext.Current.GetService<IPhoneticAlgorithmHandler>();

            // Convert address components 
            foreach (var itm in storageData.Component)
            {
                var nameValue = dataContext.PhoneticValues.SingleOrDefault(o => o.Value == itm.Value);
                if (nameValue == null)
                {
                    if (itm.PhoneticAlgorithmKey == Guid.Empty && phoneticAlgorithm != null)
                    {
                        itm.PhoneticCode = phoneticAlgorithm.GenerateCode(itm.Value);
                        itm.PhoneticAlgorithmKey = phoneticAlgorithm.AlgorithmId;
                    }
                    else
                        itm.PhoneticAlgorithmKey = Core.Model.DataTypes.PhoneticAlgorithm.EmptyAlgorithm.Key;

                    // Set the phonetic value
                    nameValue = new PhoneticValue()
                    {
                        Value = itm.Value,
                        PhoneticAlgorithmId = itm.PhoneticAlgorithmKey,
                        PhoneticCode = itm.PhoneticCode
                    };
                }
                
                domainName.EntityNameComponents.Add(new Data.EntityNameComponent()
                {
                    ComponentTypeConceptId = itm.ComponentTypeKey == Guid.Empty ? null : (Guid?)itm.ComponentTypeKey,
                    PhoneticValue = nameValue,
                    EntityName = domainName
                });
            }
            dataContext.EntityNames.InsertOnSubmit(domainName);
            dataContext.SubmitChanges(); // Write and reload data from database

            // Copy properties
            storageData.Key = domainName.EntityNameId;
            storageData.EffectiveVersionSequenceId = domainName.EffectiveVersionSequenceId;
            storageData.SourceEntityKey = domainName.EntityId;
            return storageData;

        }

        internal override Core.Model.Entities.EntityName Obsolete(Core.Model.Entities.EntityName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            throw new NotImplementedException();
        }

        internal override Core.Model.Entities.EntityName Update(Core.Model.Entities.EntityName storageData, IPrincipal principal, ModelDataContext dataContext, bool newVersion)
        {
            throw new NotImplementedException();
        }
    }
}
