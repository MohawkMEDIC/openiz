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
 * Date: 2016-6-19
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using OpenIZ.Core.Interfaces;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityName, Data.EntityName>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Entities.EntityName Insert(ModelDataContext context, Core.Model.Entities.EntityName data, IPrincipal principal)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context, principal);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;
            var retVal = base.Insert(context, data, principal);

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityNameComponent, Data.EntityNameComponent>(
                    data.Component, 
                    data, 
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        public override Core.Model.Entities.EntityName Update(ModelDataContext context, Core.Model.Entities.EntityName data, IPrincipal principal)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context, principal);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityNameComponent, Data.EntityNameComponent>(
                    data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Data load options
        /// </summary>
        /// <returns></returns>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Data.EntityName>(c => c.NameUseConcept);
            dlo.LoadWith<Data.EntityName>(c => c.EntityNameComponents);
            dlo.LoadWith<Data.EntityNameComponent>(c => c.ComponentTypeConcept);
            dlo.LoadWith<Data.EntityNameComponent>(c => c.PhoneticValue);

            return dlo;
        }
    }

    /// <summary>
    /// Represents an entity name component persistence service
    /// </summary>
    public class EntityNameComponentPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityNameComponent, Data.EntityNameComponent>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Entities.EntityNameComponent modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as Data.EntityNameComponent;

            // Duplicate name?
            var existing = context.PhoneticValues.FirstOrDefault(o => o.Value == modelInstance.Value);
            if (existing != null && existing.PhoneticValueId != retVal.PhoneticValueId)
                retVal.PhoneticValueId = existing.PhoneticValueId;
            else
            {
                var phoneticCoder = ApplicationContext.Current.GetService<IPhoneticAlgorithmHandler>();
                retVal.PhoneticValue = new PhoneticValue()
                {
                    PhoneticValueId = Guid.NewGuid(),
                    Value = modelInstance.Value,
                    PhoneticAlgorithmId = phoneticCoder?.AlgorithmId ?? PhoneticAlgorithmKeys.None,
                    PhoneticCode = phoneticCoder?.GenerateCode(modelInstance.Value)
                };
            }

            return retVal;
        }

        /// <summary>
        /// Insert context
        /// </summary>
        public override Core.Model.Entities.EntityNameComponent Insert(ModelDataContext context, Core.Model.Entities.EntityNameComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update
        /// </summary>
        public override Core.Model.Entities.EntityNameComponent Update(ModelDataContext context, Core.Model.Entities.EntityNameComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Update(context, data, principal);
        }
    }
}
