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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a service which is responsible for the
	/// maintenance of concepts.
	/// </summary>
	public class LocalConceptRepositoryService : IConceptRepositoryService
	{
		/// <summary>
		/// Searches for a concept class using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept class.</param>
		/// <returns>Returns a list of concept classes who match the specified query.</returns>
		public IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptClass>)} not found");
			}

			return persistenceService.Query(query, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Searches for a concept class using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept class.</param>
		/// <param name="count">The count of the concept classes to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concept classes who match the specified query.</returns>
		public IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptClass>)} not found");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

        /// <summary>
        /// Queries for concept names.
        /// </summary>
        /// <param name="query">The query to use to search for concept names.</param>
        /// <returns>Returns a list of concept names.</returns>
        public IEnumerable<ConceptName> FindConceptNames(Expression<Func<ConceptName, bool>> query)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptName>)} not found");
            }

            return persistenceService.Query(query, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Queries for concept names.
        /// </summary>
        /// <param name="query">The query to use to search for concept names.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of concept names.</returns>
        public IEnumerable<ConceptName> FindConceptNames(Expression<Func<ConceptName, bool>> query, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptName>)} not found");
            }

            return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Queries for concept reference terms.
        /// </summary>
        /// <param name="query">The query to use to search for concept reference terms.</param>
        /// <returns>Returns a list of concept reference terms.</returns>
        public IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptReferenceTerm>)} not found");
			}

			return persistenceService.Query(query, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Queries for concept reference terms.
		/// </summary>
		/// <param name="query">The query to use to search for concept reference terms.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of concept reference terms.</returns>
		public IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptReferenceTerm>)} not found");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

        /// <summary>
        /// Queries for reference terms.
        /// </summary>
        /// <param name="query">The query to use to search for reference terms.</param>
        /// <returns>Returns a list of reference terms.</returns>
        public IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTerm>)} not found");
            }

            return persistenceService.Query(query, AuthenticationContext.Current.Principal);
        }


        /// <summary>
        /// Queries for reference terms.
        /// </summary>
        /// <param name="query">The query to use to search for reference terms.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of reference terms.</returns>
        public IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTerm>)} not found");
            }

            return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Queries for reference term names.
        /// </summary>
        /// <param name="query">The query to use to search for reference term names.</param>
        /// <returns>Returns a list of reference term names.</returns>
        public IEnumerable<ReferenceTermName> FindReferenceTermNames(Expression<Func<ReferenceTermName, bool>> query)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTermName>)} not found");
            }

            return persistenceService.Query(query, AuthenticationContext.Current.Principal);
        }


        /// <summary>
        /// Queries for reference term names.
        /// </summary>
        /// <param name="query">The query to use to search for reference term names.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of reference term names.</returns>
        public IEnumerable<ReferenceTermName> FindReferenceTermNames(Expression<Func<ReferenceTermName, bool>> query, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTermName>)} not found");
            }

            return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Searches for a concept using a given query.
        /// </summary>
        /// <param name="query">The query to use for searching for the concept.</param>
        /// <returns>Returns a list of concepts who match the specified query.</returns>
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query)
		{
			int total = 0;
			return this.FindConcepts(query, 0, null, out total);
		}

		/// <summary>
		/// Searches for a concept using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept.</param>
		/// <param name="count">The count of the concepts to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concepts who match the specified query.</returns>
		public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Concept>)} not found");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Searches for a concept by name and language.
		/// </summary>
		/// <param name="name">The name of the concept.</param>
		/// <param name="language">The language of the concept.</param>
		/// <returns>Returns a list of concepts.</returns>
		public IEnumerable<Concept> FindConceptsByName(string name, string language)
		{
			return this.FindConcepts(o => o.ConceptNames.Any(n => n.Name == name && n.Language == language));
		}

		/// <summary>
		/// Finds a concept by reference term.
		/// </summary>
		/// <param name="code">The code of the reference term.</param>
		/// <param name="codeSystemOid">The code system OID of the reference term.</param>
		/// <returns>Returns a list of concepts.</returns>
		public IEnumerable<Concept> FindConceptsByReferenceTerm(string code, string codeSystemOid)
		{
			return this.FindConcepts(o => o.ReferenceTerms.Any(r => r.ReferenceTerm.CodeSystem.Oid == codeSystemOid && r.ReferenceTerm.Mnemonic == code));
		}

		/// <summary>
		/// Searches for a concept set using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept set.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
		public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query)
		{
			var total = 0;
			return this.FindConceptSets(query, 0, null, out total);
		}

		/// <summary>
		/// Searches for a concept sets using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept sets.</param>
		/// <param name="count">The count of the concept sets to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalResults">The total count of the search results.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
		public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

        /// <summary>
        /// Gets the specified concept.
        /// </summary>
        /// <param name="id">The id of the concept.</param>
        /// <param name="versionId">The version id of the concept.</param>
        /// <returns>Returns the specified concept.</returns>
        public IdentifiedData GetConcept(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Concept>)} not found");
			}

			return persistenceService.Get(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Get a concept by its mnemonic
		/// </summary>
		/// <param name="mnemonic">The concept mnemonic to get</param>
		public Concept GetConcept(string mnemonic)
		{
			return this.FindConcepts(o => o.Mnemonic == mnemonic).FirstOrDefault();
		}

		/// <summary>
		/// Gets a concept class.
		/// </summary>
		/// <param name="id">The id of the concept class to retrieve.</param>
		/// <returns>Returns the concept class.</returns>
		public ConceptClass GetConceptClass(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptClass>)} not found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
		}

        /// <summary>
        /// Gets a concept name.
        /// </summary>
        /// <param name="id">The id of the concept name to retrieve.</param>
        /// <returns>Returns the concept name.</returns>
        public ConceptName GetConceptName(Guid id)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptName>)} not found");
            }

            return persistenceService.Get<Guid>(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Gets a concept reference term by id.
        /// </summary>
        /// <param name="id">The id of the concept reference term.</param>
        /// <returns>Returns the concept reference term.</returns>
        public ConceptReferenceTerm GetConceptReferenceTerm(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptReferenceTerm>)} not found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Get the specified concept set by identifier
		/// </summary>
		public ConceptSet GetConceptSet(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
			}

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
		public ReferenceTerm GetReferenceTerm(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTerm>)} not found");
			}

			return persistenceService.Get(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
		}

        /// <summary>
        /// Get the specified reference term for the concept
        /// </summary>
        public ReferenceTermName GetReferenceTermName(Guid id)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTermName>)} not found");
            }

            return persistenceService.Get(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Returns a value which indicates whether <paramref name="a"/> implies <paramref name="b"/>
        /// </summary>
        public bool Implies(Concept a, Concept b)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts a concept.
		/// </summary>
		/// <param name="concept">The concept to be inserted.</param>
		/// <returns>Returns the inserted concept.</returns>
		public Concept InsertConcept(Concept concept)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Concept>)} not found");
			}

			return persistenceService.Insert(concept, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts a concept class.
		/// </summary>
		/// <param name="conceptClass">The concept class to be inserted.</param>
		/// <returns>Returns the newly inserted concept class.</returns>
		public ConceptClass InsertConceptClass(ConceptClass conceptClass)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptClass>)} not found");
			}

			return persistenceService.Insert(conceptClass, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Inserts a concept name.
        /// </summary>
        /// <param name="conceptName">The concept class to be inserted.</param>
        /// <returns>Returns the newly inserted concept class.</returns>
        public ConceptName InsertConceptName(ConceptName conceptName)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptName>)} not found");
            }

            return persistenceService.Insert(conceptName, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Inserts a concept reference term.
        /// </summary>
        /// <param name="conceptReferenceTerm">The concept reference term to be inserted.</param>
        /// <returns>Returns the inserted concept reference term.</returns>
        public ConceptReferenceTerm InsertConceptReferenceTerm(ConceptReferenceTerm conceptReferenceTerm)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptReferenceTerm>)} not found");
			}

			return persistenceService.Insert(conceptReferenceTerm, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts a concept set.
		/// </summary>
		/// <param name="set">The concept set to be inserted.</param>
		/// <returns>Returns the inserted concept set.</returns>
		public ConceptSet InsertConceptSet(ConceptSet set)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
			}

			return persistenceService.Insert(set, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Inserts a reference term.
        /// </summary>
        /// <param name="referenceTerm">The reference term to be inserted.</param>
        /// <returns>Returns the inserted reference term.</returns>
        public ReferenceTerm InsertReferenceTerm(ReferenceTerm referenceTerm)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
            }

            return persistenceService.Insert(referenceTerm, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Inserts a reference term name.
        /// </summary>
        /// <param name="referenceTermName">The reference term name to be inserted.</param>
        /// <returns>Returns the inserted reference term name.</returns>
        public ReferenceTermName InsertReferenceTermName(ReferenceTermName referenceTermName)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
            }

            return persistenceService.Insert(referenceTermName, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Determine if the concept set contains the specified concept
        /// </summary>
        public bool IsMember(ConceptSet set, Concept concept)
		{
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

			if (persistence == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
			}

			return persistence.Count(o => o.ConceptsXml.Any(c => c == concept.Key), AuthenticationContext.Current.Principal) > 0;
		}

		/// <summary>
		/// Obsoletes a concept.
		/// </summary>
		/// <param name="key">The key of the concept to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept.</returns>
		public Concept ObsoleteConcept(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Concept>)} not found");
			}
            
			return persistenceService.Obsolete(this.GetConcept(key, Guid.Empty) as Concept, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes a concept class.
		/// </summary>
		/// <param name="key">The key of the concept class to obsolete.</param>
		/// <returns>Returns the obsoleted concept class.</returns>
		public ConceptClass ObsoleteConceptClass(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptClass>)} not found");
			}

			return persistenceService.Obsolete(new ConceptClass { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Obsoletes a concept name.
        /// </summary>
        /// <param name="key">The key of the concept name to obsolete.</param>
        /// <returns>Returns the obsoleted concept name.</returns>
        public ConceptName ObsoleteConceptName(Guid key)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptName>)} not found");
            }

            return persistenceService.Obsolete(new ConceptName { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Obsoletes a concept reference term.
        /// </summary>
        /// <param name="key">The key of the concept reference term to obsolete.</param>
        /// <returns>Returns the obsoleted concept reference term.</returns>
        public ConceptReferenceTerm ObsoleteConceptReferenceTerm(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptReferenceTerm>)} not found");
			}

			return persistenceService.Obsolete(new ConceptReferenceTerm { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes a concept set.
		/// </summary>
		/// <param name="key">The key of the concept set to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept set.</returns>
		public ConceptSet ObsoleteConceptSet(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
			}

			return persistenceService.Obsolete(new ConceptSet { Key = key }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Insert or updates a concept.
		/// </summary>
		/// <param name="concept">The concept to be saved.</param>
		/// <returns>Returns the saved concept.</returns>
		public Concept SaveConcept(Concept concept)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Concept>)} not found");
			}

			if (concept.Key.HasValue && persistenceService.Get(new Identifier<Guid>(concept.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(concept, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(concept, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts or updates a concept class.
		/// </summary>
		/// <param name="conceptClass">The concept class to be saved.</param>
		/// <returns>Returns the saved concept class.</returns>
		public ConceptClass SaveConceptClass(ConceptClass conceptClass)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptClass>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptClass>)} not found");
			}

			if (conceptClass.Key.HasValue && persistenceService.Get(new Identifier<Guid>(conceptClass.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(conceptClass, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(conceptClass, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Inserts or updates a concept name.
        /// </summary>
        /// <param name="conceptName">The concept name to be saved.</param>
        /// <returns>Returns the saved concept name.</returns>
        public ConceptName SaveConceptName(ConceptName conceptName)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptName>)} not found");
            }

			if (conceptName.Key.HasValue && persistenceService.Get(new Identifier<Guid>(conceptName.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(conceptName, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(conceptName, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Inserts or updates a concept reference term.
        /// </summary>
        /// <param name="conceptReferenceTerm">The concept reference term to be saved.</param>
        /// <returns>Returns the saved concept reference term.</returns>
        public ConceptReferenceTerm SaveConceptReferenceTerm(ConceptReferenceTerm conceptReferenceTerm)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptReferenceTerm>)} not found");
			}

			if (conceptReferenceTerm.Key.HasValue && persistenceService.Get(new Identifier<Guid>(conceptReferenceTerm.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(conceptReferenceTerm, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(conceptReferenceTerm, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts or updates a concept set.
		/// </summary>
		/// <param name="set">The concept set to be saved.</param>
		/// <returns>Returns the saved concept set.</returns>
		public ConceptSet SaveConceptSet(ConceptSet set)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ConceptSet>)} not found");
			}

			if (set.Key.HasValue && persistenceService.Get(new Identifier<Guid>(set.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(set, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(set, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts or updates a concept reference term.
		/// </summary>
		/// <param name="term">The reference term to be saved.</param>
		/// <returns>Returns a reference name.</returns>
		public ReferenceTerm SaveReferenceTerm(ReferenceTerm term)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTerm>)} not found");
			}

			if (term.Key.HasValue && persistenceService.Get(new Identifier<Guid>(term.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(term, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(term, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Inserts or updates a concept reference term name.
        /// </summary>
        /// <param name="term">The reference term name to be saved.</param>
        /// <returns>Returns a reference term name.</returns>
        public ReferenceTermName SaveReferenceTermName(ReferenceTermName term)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();

            if (persistenceService == null)
            {
                throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTermName>)} not found");
            }

			if (term.Key.HasValue && persistenceService.Get(new Identifier<Guid>(term.Key.Value), AuthenticationContext.Current.Principal, true) != null)
			{
				return persistenceService.Update(term, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}

			return persistenceService.Insert(term, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}
    }
}