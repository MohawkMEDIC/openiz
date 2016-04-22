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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Security;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Concept service
    /// </summary>
    internal class LocalConceptRepositoryService : IConceptRepositoryService
    {

        /// <summary>
        /// Find concepts
        /// </summary>
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query)
        {
            int total = 0;
            return this.FindConcepts(query, 0, null, out total);
        }

        /// <summary>
        /// Find concepts
        /// </summary>
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalResults)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);

        }

        public IEnumerable<Concept> FindConceptsByName(string name, string language)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Concept> FindConceptsByReferenceTerm(string code, string codeSystemOid)
        {
            throw new NotImplementedException();
        }

        public Concept GetConcept(string mnemonic)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified concept
        /// </summary>
        public IdentifiedData GetConcept(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            return persistenceService.Get(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
            
        }

        public ConceptSet GetConceptSet(string mnemonic)
        {
            throw new NotImplementedException();
        }

        public ReferenceTerm GetReferenceTerm(Concept concept, string codeSystemOid)
        {
            throw new NotImplementedException();
        }

        public bool Implies(Concept a, Concept b)
        {
            throw new NotImplementedException();
        }

        public Concept InsertConcept(Concept concept)
        {
            throw new NotImplementedException();
        }

        public bool IsMember(ConceptSet set, Concept concept)
        {
            throw new NotImplementedException();
        }

        public IdentifiedData ObsoleteConcept(Guid key)
        {
            throw new NotImplementedException();
        }

        public Concept SaveConcept(Concept concept)
        {
            throw new NotImplementedException();
        }

        public Concept SaveConceptClass(ConceptClass clazz)
        {
            throw new NotImplementedException();
        }

        public Concept SaveReferenceTerm(ReferenceTerm term)
        {
            throw new NotImplementedException();
        }

    }
}
