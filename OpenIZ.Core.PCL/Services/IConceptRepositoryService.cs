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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a service which is responsible for the
	/// maintenance of concepts
	/// </summary>
	public interface IConceptRepositoryService
	{
		/// <summary>
		/// Searches for a concept class using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept class.</param>
		/// <returns>Returns a list of concept classes who match the specified query.</returns>
		IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query);

		/// <summary>
		/// Searches for a concept class using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept class.</param>
		/// <param name="count">The count of the concept classes to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concept classes who match the specified query.</returns>
		IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Searches for a concept using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept.</param>
		/// <returns>Returns a list of concepts who match the specified query.</returns>
		IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query);

		/// <summary>
		/// Searches for a concept using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept.</param>
		/// <param name="count">The count of the concepts to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concepts who match the specified query.</returns>
		IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Searches for a concept by name and language.
		/// </summary>
		/// <param name="name">The name of the concept.</param>
		/// <param name="language">The language of the concept.</param>
		/// <returns>Returns a list of concepts.</returns>
		IEnumerable<Concept> FindConceptsByName(string name, string language);

		/// <summary>
		/// Finds a concept by reference term.
		/// </summary>
		/// <param name="code">The code of the reference term.</param>
		/// <param name="codeSystemOid">The code system OID of the reference term.</param>
		/// <returns>Returns a list of concepts.</returns>
		IEnumerable<Concept> FindConceptsByReferenceTerm(string code, string codeSystemOid);

		/// <summary>
		/// Searches for a concept set using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept set.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
		IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query);

		/// <summary>
		/// Searches for a concept sets using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept sets.</param>
		/// <param name="count">The count of the concept sets to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
		IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified concept.
		/// </summary>
		/// <param name="id">The id of the concept.</param>
		/// <param name="versionId">The version id of the concept.</param>
		/// <returns>Returns the specified concept.</returns>
		IdentifiedData GetConcept(Guid id, Guid versionId);

		/// <summary>
		/// Get a concept by its mnemonic
		/// </summary>
		/// <param name="mnemonic">The concept mnemonic to get</param>
		Concept GetConcept(string mnemonic);

		/// <summary>
		/// Gets a concept class.
		/// </summary>
		/// <param name="id">The id of the concept class to retrieve.</param>
		/// <returns>Returns the concept class.</returns>
		ConceptClass GetConceptClass(Guid id);

		/// <summary>
		/// Gets the specified concept set.
		/// </summary>
		ConceptSet GetConceptSet(Guid id);

		/// <summary>
		/// Get the concept set by mnemonic
		/// </summary>
		ConceptSet GetConceptSet(string mnemonic);

		/// <summary>
		/// Gets the specified reference term from <paramref name="concept"/> in <paramref name="codeSystemOid"/>
		/// </summary>
		ReferenceTerm GetReferenceTerm(Concept concept, string codeSystemOid);

		/// <summary>
		/// Returns a value which indicates whether <paramref name="a"/> implies <paramref name="b"/>
		/// </summary>
		bool Implies(Concept a, Concept b);

		/// <summary>
		/// Inserts a concept.
		/// </summary>
		/// <param name="concept">The concept to be inserted.</param>
		/// <returns>Returns the inserted concept.</returns>
		Concept InsertConcept(Concept concept);

		/// <summary>
		/// Inserts a concept class.
		/// </summary>
		/// <param name="conceptClass">The concept class to be inserted.</param>
		/// <returns>Returns the newly inserted concept class.</returns>
		ConceptClass InsertConceptClass(ConceptClass conceptClass);

		/// <summary>
		/// Inserts a concept set.
		/// </summary>
		/// <param name="set">The concept set to be inserted.</param>
		/// <returns>Returns the inserted concept set.</returns>
		ConceptSet InsertConceptSet(ConceptSet set);

		/// <summary>
		/// Returns true if the concept <paramref name="concept"/> is a member of set <paramref name="set"/>
		/// </summary>
		bool IsMember(ConceptSet set, Concept concept);

		/// <summary>
		/// Obsoletes a concept.
		/// </summary>
		/// <param name="key">The key of the concept to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept.</returns>
		IdentifiedData ObsoleteConcept(Guid key);

		/// <summary>
		/// Obsoletes a concept class.
		/// </summary>
		/// <param name="key">The key of the concept class to obsolete.</param>
		/// <returns>Returns the obsoleted concept class.</returns>
		ConceptClass ObsoleteConceptClass(Guid key);

		/// <summary>
		/// Obsoletes a concept set.
		/// </summary>
		/// <param name="key">The key of the concept set to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept set.</returns>
		ConceptSet ObsoleteConceptSet(Guid key);

		/// <summary>
		/// Insert or updates a concept.
		/// </summary>
		/// <param name="concept">The concept to be saved.</param>
		/// <returns>Returns the saved concept.</returns>
		Concept SaveConcept(Concept concept);

		/// <summary>
		/// Inserts or updates a concept class.
		/// </summary>
		/// <param name="clazz">The concept class to be saved.</param>
		/// <returns>Returns the saved concept class.</returns>
		Concept SaveConceptClass(ConceptClass clazz);

		/// <summary>
		/// Inserts or updates a concept set.
		/// </summary>
		/// <param name="set">The concept set to be saved.</param>
		/// <returns>Returns the saved concept set.</returns>
		ConceptSet SaveConceptSet(ConceptSet set);

		/// <summary>
		/// Inserts or updates a concept reference term.
		/// </summary>
		/// <param name="term">The reference term to be saved.</param>
		/// <returns>Returns a concept.</returns>
		Concept SaveReferenceTerm(ReferenceTerm term);
	}
}