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
using OpenIZ.Core.Security.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a service which is responsible for the
	/// maintenance of concepts.
	/// </summary>
	public class LocalConceptRepositoryService : IConceptRepositoryService, IPersistableQueryRepositoryService
	{
		/// <summary>
		/// Searches for a concept class using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept class.</param>
		/// <returns>Returns a list of concept classes who match the specified query.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query)
		{
            int tr = 0;
            return this.FindConceptClasses(query, 0, null, out tr);
		}

		/// <summary>
		/// Searches for a concept class using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept class.</param>
		/// <param name="count">The count of the concept classes to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concept classes who match the specified query.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query, int offset, int? count, out int totalCount)
		{
            return this.Find<ConceptClass>(query, offset, count, out totalCount, Guid.Empty);
		}

        /// <summary>
        /// Queries for concept names.
        /// </summary>
        /// <param name="query">The query to use to search for concept names.</param>
        /// <returns>Returns a list of concept names.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptName> FindConceptNames(Expression<Func<ConceptName, bool>> query)
        {
            int tr = 0;
            return this.FindConceptNames(query, 0, null, out tr);
        }

        /// <summary>
        /// Queries for concept names.
        /// </summary>
        /// <param name="query">The query to use to search for concept names.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of concept names.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptName> FindConceptNames(Expression<Func<ConceptName, bool>> query, int offset, int? count, out int totalCount)
        {
            return this.Find<ConceptName>(query, offset, count, out totalCount, Guid.Empty);
        }

        /// <summary>
        /// Queries for concept reference terms.
        /// </summary>
        /// <param name="query">The query to use to search for concept reference terms.</param>
        /// <returns>Returns a list of concept reference terms.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query)
		{
            int t = 0;
            return this.FindConceptReferenceTerms(query, 0, null, out t);
		}

		/// <summary>
		/// Queries for concept reference terms.
		/// </summary>
		/// <param name="query">The query to use to search for concept reference terms.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of concept reference terms.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query, int offset, int? count, out int totalCount)
		{
            return this.Find<ConceptReferenceTerm>(query, 0, count, out totalCount, Guid.Empty);
		}

        /// <summary>
        /// Queries for reference terms.
        /// </summary>
        /// <param name="query">The query to use to search for reference terms.</param>
        /// <returns>Returns a list of reference terms.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query)
        {
            int t = 0;
            return this.FindReferenceTerms(query, 0, null, out t);
        }


        /// <summary>
        /// Queries for reference terms.
        /// </summary>
        /// <param name="query">The query to use to search for reference terms.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of reference terms.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query, int offset, int? count, out int totalCount)
        {
            return this.Find<ReferenceTerm>(query, offset, count, out totalCount, Guid.Empty);
        }

        /// <summary>
        /// Queries for reference term names.
        /// </summary>
        /// <param name="query">The query to use to search for reference term names.</param>
        /// <returns>Returns a list of reference term names.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ReferenceTermName> FindReferenceTermNames(Expression<Func<ReferenceTermName, bool>> query)
        {
            int t = 0;
            return this.FindReferenceTermNames(query, 0, null, out t);
        }


        /// <summary>
        /// Queries for reference term names.
        /// </summary>
        /// <param name="query">The query to use to search for reference term names.</param>
        /// <param name="offset">The offset of the query.</param>
        /// <param name="count">The count of the query.</param>
        /// <param name="totalCount">The total count of the query.</param>
        /// <returns>Returns a list of reference term names.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ReferenceTermName> FindReferenceTermNames(Expression<Func<ReferenceTermName, bool>> query, int offset, int? count, out int totalCount)
        {
            return this.Find<ReferenceTermName>(query, offset, count, out totalCount, Guid.Empty);
        }

        /// <summary>
        /// Searches for a concept using a given query.
        /// </summary>
        /// <param name="query">The query to use for searching for the concept.</param>
        /// <returns>Returns a list of concepts who match the specified query.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalCount)
		{
            return this.Find<Concept>(query, offset, count, out totalCount, Guid.Empty);
		}

		/// <summary>
		/// Searches for a concept by name and language.
		/// </summary>
		/// <param name="name">The name of the concept.</param>
		/// <param name="language">The language of the concept.</param>
		/// <returns>Returns a list of concepts.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<Concept> FindConceptsByName(string name, string language)
		{
			return this.FindConcepts(o => o.ConceptNames.Any(n => n.Name == name && n.Language == language));
		}

		/// <summary>
		/// Finds a concept by reference term.
		/// </summary>
		/// <param name="code">The code of the reference term.</param>
		/// <param name="codeSystem">The code system OID of the reference term.</param>
		/// <returns>Returns a list of concepts.</returns>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<Concept> FindConceptsByReferenceTerm(string code, Uri codeSystem)
		{
            if (codeSystem.Scheme == "urn" && codeSystem.LocalPath.StartsWith("oid"))
            {
                string csOid = codeSystem.LocalPath.Substring(4);
                return this.FindConcepts(o => o.ReferenceTerms.Any(r => r.ReferenceTerm.CodeSystem.Oid == csOid && r.ReferenceTerm.Mnemonic == code));
            }
            else 
                return this.FindConcepts(o => o.ReferenceTerms.Any(r => r.ReferenceTerm.CodeSystem.Url == codeSystem.OriginalString && r.ReferenceTerm.Mnemonic == code));

		}

		/// <summary>
		/// Searches for a concept set using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept set.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults)
		{
            return this.Find<ConceptSet>(query, offset, count, out totalResults, Guid.Empty);
		}

        /// <summary>
        /// Gets the specified concept.
        /// </summary>
        /// <param name="id">The id of the concept.</param>
        /// <param name="versionId">The version id of the concept.</param>
        /// <returns>Returns the specified concept.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public Concept GetConcept(string mnemonic)
		{
			return this.FindConcepts(o => o.Mnemonic == mnemonic).FirstOrDefault();
		}

        /// <summary>
        /// Gets a concept class.
        /// </summary>
        /// <param name="id">The id of the concept class to retrieve.</param>
        /// <returns>Returns the concept class.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public ConceptSet GetConceptSet(string mnemonic)
		{
			return this.FindConceptSets(o => o.Mnemonic == mnemonic).FirstOrDefault();
		}

        /// <summary>
        /// Get the specified reference term for the concept
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
		/// Obsoletes the reference term.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the obsoleted reference term.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public ReferenceTerm ObsoleteReferenceTerm(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTerm>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<ReferenceTerm>)} not found");
			}

			return persistenceService.Obsolete(this.GetReferenceTerm(key), AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
        /// Insert or updates a concept.
        /// </summary>
        /// <param name="concept">The concept to be saved.</param>
        /// <returns>Returns the saved concept.</returns>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
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

        /// <summary>
        /// Find using query continuation
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> query, int offset, int? count, out int totalResults, Guid queryId) where TEntity : IdentifiedData
        {
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<TEntity>>();
            if (persistence == null)
                throw new InvalidOperationException($"Cannot find entity persister for {typeof(TEntity).FullName}");

            if (queryId != Guid.Empty && persistence is IStoredQueryDataPersistenceService<TEntity>)
                return (persistence as IStoredQueryDataPersistenceService<TEntity>).Query(query, queryId, offset, count, AuthenticationContext.Current.Principal, out totalResults);
            else
                return persistence.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
        }

        /// <summary>
        /// Get concept reference term for the specified code system
        /// </summary>
        public ReferenceTerm GetConceptReferenceTerm(Guid conceptId, String codeSystem)
        {
            // Concept is loaded
            var refTermService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();
            if (refTermService == null)
                throw new InvalidOperationException("Cannot find concept service");
            int tr = 0;
            var refTermEnt = refTermService.Query(o => (o.ReferenceTerm.CodeSystem.Url == codeSystem || o.ReferenceTerm.CodeSystem.Oid == codeSystem) && o.SourceEntityKey == conceptId, 0, 1, AuthenticationContext.Current.Principal, out tr).FirstOrDefault();
            if (refTermEnt != null)
                return refTermEnt.ReferenceTerm ?? this.GetReferenceTerm(refTermEnt.ReferenceTermKey.Value);
            else
                return null;
        }
    }
}