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
    internal class LocalConceptService : IConceptService
    {

        // Concept service
        private IDataPersistenceService<Concept> m_conceptService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
        private IDataPersistenceService<ConceptSet> m_conceptSetService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
        private IDataPersistenceService<ConceptReferenceTerm> m_referenceTermService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

        /// <summary>
        /// Find concepts
        /// </summary>
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query)
        {
            int r;
            return this.FindConcepts(query, 0, null, out r);
        }

        /// <summary>
        /// Find concepts
        /// </summary>
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalResults)
        {
            totalResults = 0;
            return this.m_conceptService?.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
        }

        /// <summary>
        /// Find concepts by name
        /// </summary>
        public IEnumerable<Concept> FindConceptsByName(string name, string language)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();
            return persistenceService.Query(o => o.Name == name && o.Language == language, AuthenticationContext.Current.Principal).Select(o=>o.SourceEntity);
        }

        /// <summary>
        /// Find concepts by reference term
        /// </summary>
        public IEnumerable<Concept> FindConceptsByReferenceTerm(string code, string codeSystemOid)
        {
            return this.m_referenceTermService.Query(o => o.ReferenceTerm.Mnemonic == code && o.ReferenceTerm.CodeSystem.Oid == codeSystemOid, AuthenticationContext.Current.Principal).Select(o=>o.SourceEntity);
        }

        /// <summary>
        /// Find concept sets
        /// </summary>
        public IEnumerable<ConceptSet> FindConceptSet(Expression<Func<ConceptSet, bool>> query)
        {
            int r;
            return this.FindConceptSet(query, 0, null, out r);
        }

        /// <summary>
        /// Find concept set by query
        /// </summary>
        public IEnumerable<ConceptSet> FindConceptSet(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults)
        {
            totalResults = 0;
            return this.m_conceptSetService?.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
        }

        /// <summary>
        /// Get a concept by the concept's core mnemonic
        /// </summary>
        public Concept GetConcept(string mnemonic)
        {
            return this.m_conceptService.Query(o => o.Mnemonic == mnemonic, AuthenticationContext.Current.Principal).FirstOrDefault();
        }

        /// <summary>
        /// Get the specified concept
        /// </summary>
        public IdentifiedData GetConcept(Guid id, Guid versionId)
        {
            return this.m_conceptService.Get(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Get the specified concept by name
        /// </summary>
        public ConceptSet GetConceptSet(string mnemonic)
        {
            return this.m_conceptSetService.Query(o => o.Mnemonic == mnemonic, AuthenticationContext.Current.Principal).FirstOrDefault();
        }

        /// <summary>
        /// Get the specified reference term
        /// </summary>
        public ReferenceTerm GetReferenceTerm(Concept concept, string codeSystemOid)
        {
            return this.m_referenceTermService.Query(o => o.SourceEntityKey == concept.Key && o.ReferenceTerm.CodeSystem.Oid == codeSystemOid, AuthenticationContext.Current.Principal).Select(o=>o.ReferenceTerm).FirstOrDefault();
        }

        /// <summary>
        /// Determine if concept <paramref name="a"/> implies the same meaning as <paramref name="b"/>
        /// </summary>
        public bool Implies(Concept a, Concept b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert the specified concept
        /// </summary>
        public Concept InsertConcept(Concept concept)
        {
            return this.m_conceptService.Insert(concept, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Determine if the code is a member of the specified set
        /// </summary>
        public bool IsMember(ConceptSet set, Concept concept)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsoletes the specified concept
        /// </summary>
        public IdentifiedData ObsoleteConcept(Guid key)
        {
            Concept c = this.m_conceptService.Get(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);
            return this.m_conceptService.Obsolete(c, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Save the specified concept
        /// </summary>
        public Concept SaveConcept(Concept concept)
        {
            return this.m_conceptService.Update(concept, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Save the concept class
        /// </summary>
        /// <param name="clazz"></param>
        public ConceptClass SaveConceptClass(ConceptClass clazz)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();
            return persistence.Update(clazz, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Save the reference term
        /// </summary>
        public ReferenceTerm SaveReferenceTerm(ReferenceTerm term)
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();
            return persistence.Update(term, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

    }
}
