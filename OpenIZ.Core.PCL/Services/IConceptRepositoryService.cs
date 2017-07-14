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
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a service which is responsible for the maintenance of concepts.
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

		///// <summary>
		///// Queries for concept names.
		///// </summary>
		///// <param name="query">The query to use to search for concept names.</param>
		///// <returns>Returns a list of concept names.</returns>
		//IEnumerable<ConceptName> FindConceptNames(Expression<Func<ConceptName, bool>> query);

		///// <summary>
		///// Queries for concept names.
		///// </summary>
		///// <param name="query">The query to use to search for concept names.</param>
		///// <param name="offset">The offset of the query.</param>
		///// <param name="count">The count of the query.</param>
		///// <param name="totalCount">The total count of the query.</param>
		///// <returns>Returns a list of concept names.</returns>
		//IEnumerable<ConceptName> FindConceptNames(Expression<Func<ConceptName, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Queries for concept reference terms.
		/// </summary>
		/// <param name="query">The query to use to search for concept reference terms.</param>
		/// <returns>Returns a list of concept reference terms.</returns>
		IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query);

		/// <summary>
		/// Queries for concept reference terms.
		/// </summary>
		/// <param name="query">The query to use to search for concept reference terms.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of concept reference terms.</returns>
		IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Searches for a concept using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the concept.</param>
		/// <returns>Returns a list of concepts who match the specified query.</returns>
		IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query);

        /// <summary>
        /// Find code systems
        /// </summary>
        IEnumerable<CodeSystem> FindCodeSystems(Expression<Func<CodeSystem, bool>> query);

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
		IEnumerable<Concept> FindConceptsByReferenceTerm(string code, Uri codeSystem);

        /// <summary>
        /// Finds a concept by reference term.
        /// </summary>
        /// <param name="code">The code of the reference term.</param>
        /// <param name="codeSystemDomain">The code system OID of the reference term.</param>
        /// <returns>Returns a list of concepts.</returns>
        IEnumerable<Concept> FindConceptsByReferenceTerm(string code, String codeSystemDomain);


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
		/// Searches for a reference term using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the reference term.</param>
		/// <returns>Returns a list of concept sets who match the specified query.</returns>
		IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query);

		/// <summary>
		/// Searches for a reference terms using a given query.
		/// </summary>
		/// <param name="query">The query to use for searching for the reference terms.</param>
		/// <param name="count">The count of the reference terms to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of reference terms who match the specified query.</returns>
		IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified concept.
		/// </summary>
		/// <param name="id">The id of the concept.</param>
		/// <param name="versionId">The version id of the concept.</param>
		/// <returns>Returns the specified concept.</returns>
		IdentifiedData GetConcept(Guid id, Guid versionId);

		/// <summary>
		/// Gets a concept by mnemonic.
		/// </summary>
		/// <param name="mnemonic">The mnemonic of the concept.</param>
		/// <returns>Returns the concept.</returns>
		Concept GetConcept(string mnemonic);

		/// <summary>
		/// Gets a concept class.
		/// </summary>
		/// <param name="id">The id of the concept class to retrieve.</param>
		/// <returns>Returns the concept class.</returns>
		ConceptClass GetConceptClass(Guid id);

		/// <summary>
		/// Gets a concept reference term by id.
		/// </summary>
		/// <param name="id">The id of the concept reference term.</param>
		/// <returns>Returns the concept reference term.</returns>
		ConceptReferenceTerm GetConceptReferenceTerm(Guid id);

		/// <summary>
		/// Gets a concept set.
		/// </summary>
		/// <param name="id">The id of the concept set to retrieve.</param>
		/// <returns>Returns the concept set.</returns>
		ConceptSet GetConceptSet(Guid id);

		/// <summary>
		/// Gets a concept set by mnemonic.
		/// </summary>
		/// <param name="mnemonic">The mnemonic of the concept set.</param>
		/// <returns>Returns the concept set.</returns>
		ConceptSet GetConceptSet(string mnemonic);

		/// <summary>
		/// Gets a reference term.
		/// </summary>
		/// <param name="id">The id of the reference term to retrieve.</param>
		/// <returns>Returns the reference term.</returns>
		ReferenceTerm GetReferenceTerm(Guid id);

		/// <summary>
		/// Returns a value which indicates whether <paramref name="a"/> implies <paramref name="b"/>
		/// </summary>
		/// <param name="a">The left hand concept.</param>
		/// <param name="b">The right hand concept.</param>
		/// <returns>Returns true if the first concept implies the second concept.</returns>
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
		/// Inserts a concept reference term.
		/// </summary>
		/// <param name="conceptReferenceTerm">The concept reference term to be inserted.</param>
		/// <returns>Returns the inserted concept reference term.</returns>
		ConceptReferenceTerm InsertConceptReferenceTerm(ConceptReferenceTerm conceptReferenceTerm);

		/// <summary>
		/// Inserts a concept set.
		/// </summary>
		/// <param name="set">The concept set to be inserted.</param>
		/// <returns>Returns the inserted concept set.</returns>
		ConceptSet InsertConceptSet(ConceptSet set);

		/// <summary>
		/// Inserts a reference term.
		/// </summary>
		/// <param name="referenceTerm">The reference term to be inserted.</param>
		/// <returns>Returns the inserted reference term.</returns>
		ReferenceTerm InsertReferenceTerm(ReferenceTerm referenceTerm);

		/// <summary>
		/// Returns true if the concept <paramref name="concept"/> is a member of set <paramref name="set"/>
		/// </summary>
		bool IsMember(ConceptSet set, Concept concept);

		/// <summary>
		/// Obsoletes a concept.
		/// </summary>
		/// <param name="key">The key of the concept to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept.</returns>
		Concept ObsoleteConcept(Guid key);

		/// <summary>
		/// Obsoletes a concept class.
		/// </summary>
		/// <param name="key">The key of the concept class to obsolete.</param>
		/// <returns>Returns the obsoleted concept class.</returns>
		ConceptClass ObsoleteConceptClass(Guid key);

		/// <summary>
		/// Obsoletes a concept reference term.
		/// </summary>
		/// <param name="key">The key of the concept reference term to obsolete.</param>
		/// <returns>Returns the obsoleted concept reference term.</returns>
		ConceptReferenceTerm ObsoleteConceptReferenceTerm(Guid key);

		/// <summary>
		/// Obsoletes a concept set.
		/// </summary>
		/// <param name="key">The key of the concept set to be obsoleted.</param>
		/// <returns>Returns the obsoleted concept set.</returns>
		ConceptSet ObsoleteConceptSet(Guid key);

		/// <summary>
		/// Obsoletes the reference term.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the obsoleted reference term.</returns>
		ReferenceTerm ObsoleteReferenceTerm(Guid key);

		/// <summary>
		/// Insert or updates a concept.
		/// </summary>
		/// <param name="concept">The concept to be saved.</param>
		/// <returns>Returns the saved concept.</returns>
		Concept SaveConcept(Concept concept);

		/// <summary>
		/// Inserts or updates a concept class.
		/// </summary>
		/// <param name="conceptClass">The concept class to be saved.</param>
		/// <returns>Returns the saved concept class.</returns>
		ConceptClass SaveConceptClass(ConceptClass conceptClass);

		/// <summary>
		/// Inserts or updates a concept reference term.
		/// </summary>
		/// <param name="conceptReferenceTerm">The concept reference term to be saved.</param>
		/// <returns>Returns the saved concept reference term.</returns>
		ConceptReferenceTerm SaveConceptReferenceTerm(ConceptReferenceTerm conceptReferenceTerm);

		/// <summary>
		/// Inserts or updates a concept set.
		/// </summary>
		/// <param name="set">The concept set to be saved.</param>
		/// <returns>Returns the saved concept set.</returns>
		ConceptSet SaveConceptSet(ConceptSet set);

		/// <summary>
		/// Inserts or updates a concept reference term.
		/// </summary>
		/// <param name="referenceTerm">The reference term to be saved.</param>
		/// <returns>Returns a reference term.</returns>
		ReferenceTerm SaveReferenceTerm(ReferenceTerm referenceTerm);

        /// <summary>
        /// Gets the concept reference term for the specified code system 
        /// </summary>
        /// <returns></returns>
        ReferenceTerm GetConceptReferenceTerm(Guid conceptId, String codeSystem);
    }
}