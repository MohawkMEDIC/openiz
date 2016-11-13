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
using System;
using System.Linq;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;
using System.Data.Linq;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
	/// <summary>
	/// Persistence service for ConceptSets
	/// </summary>
	public class ConceptSetPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.ConceptSet, Data.ConceptSet>
	{

        /// <summary>
        /// Get data load options
        /// </summary>
        /// <returns></returns>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = base.GetDataLoadOptions();
            loadOptions.LoadWith<Data.ConceptSet>(cs => cs.ConceptSetMembers);
            loadOptions.LoadWith<Data.ConceptSetMember>(cs => cs.Concept);
            loadOptions.LoadWith<Data.ConceptVersion>(c => c.Concept);
            loadOptions.LoadWith<Data.Concept>(c => c.ConceptNames);
            loadOptions.LoadWith<Data.ConceptVersion>(c => c.ConceptClass);
            //loadOptions.LoadWith<Data.Concept>(c => c.ConceptVersions);

            return loadOptions;
        }

        /// <summary>
        /// Return value
        /// </summary>
        public override Core.Model.DataTypes.ConceptSet ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            var de = dataInstance as Data.ConceptSet;
            retVal.ConceptsXml = de.ConceptSetMembers.Select(o => o.ConceptId).ToList();
            return retVal;
        }

        /// <summary>
        /// Inser the specified concept set
        /// </summary>
        public override Core.Model.DataTypes.ConceptSet Insert(ModelDataContext context, Core.Model.DataTypes.ConceptSet data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            // Concept sets 
            if (retVal.Concepts != null)
                foreach (var i in retVal.Concepts)
                {
                    i.EnsureExists(context, principal);
                    context.ConceptSetMembers.InsertOnSubmit(new Data.ConceptSetMember() { ConceptId = i.Key.Value, ConceptSetId = retVal.Key.Value });
                }
            return retVal;
        }

        /// <summary>
        /// Update the specified conceptset
        /// </summary>
        public override Core.Model.DataTypes.ConceptSet Update(ModelDataContext context, Core.Model.DataTypes.ConceptSet data, IPrincipal principal)
        {
            var retVal = base.Update(context, data, principal);

            // Concept sets 
            if (retVal.Concepts != null)
            {
                // Special case m2m
                var existingConceptSets = context.ConceptSetMembers.Where(o => o.ConceptSetId == retVal.Key);
                // Any new?
                var newConcepts = retVal.Concepts.Where(o => !existingConceptSets.Select(e => e.ConceptId).ToList().Contains(o.Key.Value));
                foreach (var i in newConcepts)
                {
                    i.EnsureExists(context, principal);
                    context.ConceptSetMembers.InsertOnSubmit(new Data.ConceptSetMember() { ConceptId = i.Key.Value, ConceptSetId = retVal.Key.Value });
                }

                var delConcepts = existingConceptSets.Select(e => e.ConceptId).ToList().Where(o => !retVal.Concepts.Exists(c => c.Key == o));
                foreach (var i in delConcepts)
                    context.ConceptSetMembers.DeleteOnSubmit(existingConceptSets.FirstOrDefault(p => p.ConceptId == i && p.ConceptSetId == retVal.Key.Value));
            }

            return retVal;
        }
    }
}

