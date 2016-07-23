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
 * Date: 2016-6-22
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
        /// Create the specified concept st
        /// </summary>
        public ConceptSet InsertConceptSet(ConceptSet set)
        {
            throw new NotImplementedException();
        }

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

        /// <summary>
        /// Locates the specified concepts by name
        /// </summary>
        public IEnumerable<Concept> FindConceptsByName(string name, string language)
        {
            return this.FindConcepts(o => o.ConceptNames.Any(n => n.Name == name && n.Language == language));
        }

        /// <summary>
        /// Find concepts by a reference term
        /// </summary>
        public IEnumerable<Concept> FindConceptsByReferenceTerm(string code, string codeSystemOid)
        {
            return this.FindConcepts(o => o.ReferenceTerms.Any(r => r.ReferenceTerm.CodeSystem.Oid == codeSystemOid && r.ReferenceTerm.Mnemonic == code));
        }

        /// <summary>
        /// Find concept sets that match the specified query
        /// </summary>
        public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query)
        {
            int total = 0;
            return this.FindConceptSets(query, 0, null, out total);
        }

        /// <summary>
        /// Find the specified concept sts
        /// </summary>
        public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept set persistence service found");

            return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
        }

        /// <summary>
        /// Gets the specified concept by mnemonic
        /// </summary>
        public Concept GetConcept(string mnemonic)
        {
            return this.FindConcepts(o => o.Mnemonic == mnemonic).FirstOrDefault();
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

        /// <summary>
        /// Get the specified concept set by identifier
        /// </summary>
        public ConceptSet GetConceptSet(Guid id)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept set persistence service found");

            return persistenceService.Get(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);

        }

        /// <summary>
        /// Get the specified concept set by mnemonic
        /// </summary>
        public ConceptSet GetConceptSet(string mnemonic)
        {
            return this.FindConceptSets(o => o.Mnemonic == mnemonic).FirstOrDefault();
        }

        /// <summary>
        /// Get the specified reference term for the concept
        /// </summary>
        public ReferenceTerm GetReferenceTerm(Concept concept, string codeSystemOid)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No reference term persistence service found");
            return persistenceService.Query(o => o.SourceEntityKey == concept.Key && o.ReferenceTerm.CodeSystem.Oid == codeSystemOid, AuthenticationContext.Current.Principal).FirstOrDefault()?.ReferenceTerm;
        }

        public bool Implies(Concept a, Concept b)
        {
            throw new NotImplementedException();
        }

        public Concept InsertConcept(Concept concept)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determine if the concept set contains the specified concept
        /// </summary>
        public bool IsMember(ConceptSet set, Concept concept)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
            if (persistence == null)
                throw new InvalidOperationException("Cannot locate concept set persistence service");
            return persistence.Count(o => o.Concepts.Any(c=>c.Key == concept.Key), AuthenticationContext.Current.Principal) > 0;
        }

        public IdentifiedData ObsoleteConcept(Guid key)
        {
            throw new NotImplementedException();
        }

        public ConceptSet ObsoleteConceptSet(Guid key)
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

        public ConceptSet SaveConceptSet(ConceptSet set)
        {
            throw new NotImplementedException();
        }

        public Concept SaveReferenceTerm(ReferenceTerm term)
        {
            throw new NotImplementedException();
        }

    }
}
