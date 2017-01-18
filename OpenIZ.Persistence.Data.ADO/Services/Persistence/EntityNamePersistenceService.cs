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
 * Date: 2016-6-18
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data.Model.DataType;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Entity name persistence service
    /// </summary>
    public class EntityNamePersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityName, DbEntityName>
    {

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override Core.Model.Entities.EntityName Insert(DataContext context, Core.Model.Entities.EntityName data, IPrincipal principal)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context, principal);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;
            var retVal = base.Insert(context, data, principal);

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityNameComponent, DbEntityNameComponent>(
                   data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the entity name
        /// </summary>
        public override Core.Model.Entities.EntityName Update(DataContext context, Core.Model.Entities.EntityName data, IPrincipal principal)
        {
            // Ensure exists
            data.NameUse?.EnsureExists(context, principal);
            data.NameUseKey = data.NameUse?.Key ?? data.NameUseKey;

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.Value.ToByteArray();

            // Data component
            if (data.Component != null)
                base.UpdateAssociatedItems<Core.Model.Entities.EntityNameComponent, DbEntityNameComponent>(
                   data.Component,
                    data,
                    context,
                    principal);

            return retVal;
        }
        
    }

    /// <summary>
    /// Represents an entity name component persistence service
    /// </summary>
    public class EntityNameComponentPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.EntityNameComponent, DbEntityNameComponent>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Entities.EntityNameComponent modelInstance, DataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as DbEntityNameComponent;

            // Duplicate name?
            var existing = context.FirstOrDefault<DbPhoneticValue>(o => o.Value == modelInstance.Value);
            if (existing != null && existing.Key != retVal.ValueKey)
                retVal.ValueKey = existing.Key;
            else
            {
                var phoneticCoder = ApplicationContext.Current.GetService<IPhoneticAlgorithmHandler>();
                retVal.ValueKey = context.Insert(new DbPhoneticValue()
                {
                    Value = modelInstance.Value,
                    PhoneticAlgorithmKey = phoneticCoder?.AlgorithmId ?? PhoneticAlgorithmKeys.None,
                    PhoneticCode = phoneticCoder?.GenerateCode(modelInstance.Value)
                }).Key;
            }

            return retVal;
        }

        public override EntityNameComponent ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            return base.ToModelInstance(dataInstance, context, principal);
        }

        /// <summary>
        /// Insert context
        /// </summary>
        public override Core.Model.Entities.EntityNameComponent Insert(DataContext context, Core.Model.Entities.EntityNameComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update
        /// </summary>
        public override Core.Model.Entities.EntityNameComponent Update(DataContext context, Core.Model.Entities.EntityNameComponent data, IPrincipal principal)
        {
            data.ComponentType?.EnsureExists(context, principal);
            data.ComponentTypeKey = data.ComponentType?.Key ?? data.ComponentTypeKey;
            return base.Update(context, data, principal);
        }
    }
}
