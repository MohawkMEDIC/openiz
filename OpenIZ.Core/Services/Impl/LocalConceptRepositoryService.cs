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
				throw new InvalidOperationException(string.Format("{0} persistence service not found", nameof(IDataPersistenceService<ConceptClass>)));
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
				throw new InvalidOperationException(string.Format("{0} persistence service not found", nameof(IDataPersistenceService<ConceptClass>)));
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
				throw new InvalidOperationException("No concept persistence service found");

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
			int total = 0;
			return this.FindConceptSets(query, 0, null, out total);
		}

		/// <summary>
		/// Searches for a concept sets using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept sets.</param>
		/// <param name="count">The count of the concept sets to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
		public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept set persistence service found");

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
				throw new InvalidOperationException("No concept persistence service found");

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
			throw new NotImplementedException();
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
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Concept>)));
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts a concept set.
		/// </summary>
		/// <param name="set">The concept set to be inserted.</param>
		/// <returns>Returns the inserted concept set.</returns>
		public ConceptSet InsertConceptSet(ConceptSet set)
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
			return persistence.Count(o => o.Concepts.Any(c => c.Key == concept.Key), AuthenticationContext.Current.Principal) > 0;
		}

		/// <summary>
		/// Obsoletes a concept.
		/// </summary>
		/// <param name="key">The key of the concept to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept.</returns>
		public IdentifiedData ObsoleteConcept(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes a concept class.
		/// </summary>
		/// <param name="key">The key of the concept class to obsolete.</param>
		/// <returns>Returns the obsoleted concept class.</returns>
		public ConceptClass ObsoleteConceptClass(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes a concept set.
		/// </summary>
		/// <param name="key">The key of the concept set to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept set.</returns>
		public ConceptSet ObsoleteConceptSet(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Insert or updates a concept.
		/// </summary>
		/// <param name="concept">The concept to be saved.</param>
		/// <returns>Returns the saved concept.</returns>
		public Concept SaveConcept(Concept concept)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts or updates a concept class.
		/// </summary>
		/// <param name="clazz">The concept class to be saved.</param>
		/// <returns>Returns the saved concept class.</returns>
		public Concept SaveConceptClass(ConceptClass clazz)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts or updates a concept set.
		/// </summary>
		/// <param name="set">The concept set to be saved.</param>
		/// <returns>Returns the saved concept set.</returns>
		public ConceptSet SaveConceptSet(ConceptSet set)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts or updates a concept reference term.
		/// </summary>
		/// <param name="term">The reference term to be saved.</param>
		/// <returns>Returns a concept.</returns>
		public Concept SaveReferenceTerm(ReferenceTerm term)
		{
			throw new NotImplementedException();
		}
	}
}