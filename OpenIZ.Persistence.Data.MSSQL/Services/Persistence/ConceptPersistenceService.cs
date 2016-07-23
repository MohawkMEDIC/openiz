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
 * Date: 2016-6-19
 */
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System.Linq.Expressions;
using System.Data.Linq;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Concept persistence service
    /// </summary>
    public class ConceptPersistenceService : VersionedDataPersistenceService<Core.Model.DataTypes.Concept, ConceptVersion, Data.Concept>
    {
        /// <summary>
        /// To morel instance
        /// </summary>
        public override Core.Model.DataTypes.Concept ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            var de = dataInstance as Data.ConceptVersion;
            //retVal.ConceptSetsXml = de.Concept.ConceptSetMembers.Select(o => o.ConceptSetId).ToList();
            return retVal; 
        }

        /// <summary>
        /// Insert concept 
        /// </summary>
        public override Core.Model.DataTypes.Concept Insert(ModelDataContext context, Core.Model.DataTypes.Concept data, IPrincipal principal)
        {
            data.StatusConceptKey = data.StatusConceptKey ?? StatusKeys.Active;
            data.ClassKey = data.ClassKey == Guid.Empty ? ConceptClassKeys.Other : data.ClassKey;
            
            // Ensure exists
            data.Class?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.ClassKey = data.Class?.Key ?? data.ClassKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;

            // Persist
            var retVal = base.Insert(context, data, principal);

            // Concept sets 
            if (retVal.ConceptSets != null)
                foreach (var i in retVal.ConceptSets)
                {
                    i.EnsureExists(context, principal);
                    context.ConceptSetMembers.InsertOnSubmit(new Data.ConceptSetMember() { ConceptId = retVal.Key.Value, ConceptSetId = i.Key.Value });
                }

            // Concept names
            if (retVal.ConceptNames != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ConceptName, Data.ConceptName>(
                    retVal.ConceptNames,
                    data,
                    context,
                    principal
                );

            if (retVal.ReferenceTerms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ConceptReferenceTerm, Data.ConceptReferenceTerm>(
                     retVal.ReferenceTerms,
                     data,
                     context,
                     principal
                 );

            return retVal;
        }

        /// <summary>
        /// Override update to handle associated items
        /// </summary>
        public override Core.Model.DataTypes.Concept Update(ModelDataContext context, Core.Model.DataTypes.Concept data, IPrincipal principal)
        {

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.Value.ToByteArray();
            if (retVal.ConceptNames != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ConceptName, Data.ConceptName>(
                     retVal.ConceptNames,
                     data,
                     context,
                     principal
                 );

            if(retVal.ReferenceTerms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ConceptReferenceTerm, Data.ConceptReferenceTerm>(
                     retVal.ReferenceTerms,
                     data,
                     context,
                     principal
                 );

            // Concept sets 
            if (retVal.ConceptSets != null)
            {
                // Special case m2m
                var existingConceptSets = context.ConceptSetMembers.Where(o => o.ConceptId == retVal.Key);
                // Any new?
                var newConcepts = retVal.ConceptSets.Where(o => !existingConceptSets.Select(e => e.ConceptSetId).ToList().Contains(o.Key.Value));
                foreach (var i in newConcepts)
                {
                    i.EnsureExists(context, principal);
                    context.ConceptSetMembers.InsertOnSubmit(new Data.ConceptSetMember() { ConceptId = retVal.Key.Value, ConceptSetId = i.Key.Value });
                }

                var delConcepts = existingConceptSets.Select(e => e.ConceptSetId).ToList().Where(o => !retVal.ConceptSets.Exists(c => c.Key == o));
                foreach (var i in delConcepts)
                    context.ConceptSetMembers.DeleteOnSubmit(existingConceptSets.FirstOrDefault(p => p.ConceptId == retVal.Key.Value && p.ConceptSetId == i));
            }

            return retVal;
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Core.Model.DataTypes.Concept Obsolete(ModelDataContext context, Core.Model.DataTypes.Concept data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Get data load options
        /// </summary>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = base.GetDataLoadOptions();
            loadOptions.LoadWith<Data.ConceptVersion>(c => c.Concept);
            loadOptions.LoadWith<Data.Concept>(c => c.ConceptNames);
            loadOptions.LoadWith<Data.ConceptVersion>(c => c.ConceptClass);

            return loadOptions;
        }
    }

    /// <summary>
    /// Persistence service for concept names
    /// </summary>
    public class ConceptNamePersistenceService : IdentifiedPersistenceService<Core.Model.DataTypes.ConceptName, Data.ConceptName>
    {
        /// <summary>
        /// Concept name service
        /// </summary>
        public override object FromModelInstance(Core.Model.DataTypes.ConceptName modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as Data.ConceptName;
            var phoneticCoder = ApplicationContext.Current.GetService<IPhoneticAlgorithmHandler>();
            retVal.PhoneticAlgorithmId = phoneticCoder?.AlgorithmId ?? PhoneticAlgorithmKeys.None;
            retVal.PhoneticCode = phoneticCoder?.GenerateCode(modelInstance.Name);
            return retVal;
        }
    }
}
