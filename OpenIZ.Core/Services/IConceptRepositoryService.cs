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
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a service which is responsible for the
    /// maintenance of concepts
    /// </summary>
    public interface IConceptRepositoryService
    {

        /// <summary>
        /// Perform an insert, throwing an error if duplicate exists
        /// </summary>
        Concept InsertConcept(Concept concept);

        /// <summary>
        /// Inserts or updates a concept into the persistence store
        /// </summary>
        Concept SaveConcept(Concept concept);

        /// <summary>
        /// Saves a reference term
        /// </summary>
        Concept SaveReferenceTerm(ReferenceTerm term);

        /// <summary>
        /// Saves a concept class
        /// </summary>
        Concept SaveConceptClass(ConceptClass clazz);

        /// <summary>
        /// Get the concept set by mnemonic
        /// </summary>
        ConceptSet GetConceptSet(string mnemonic);

        /// <summary>
        /// Get a concept by its mnemonic
        /// </summary>
        /// <param name="mnemonic">The concept mnemonic to get</param>
        Concept GetConcept(String mnemonic);

        /// <summary>
        /// Performs an arbirary query 
        /// </summary>
        /// <param name="query">The query to execute</param>
        IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query);

        /// <summary>
        /// Find concepts 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalResults);

        /// <summary>
        /// Finds a series of concepts by name
        /// </summary>
        IEnumerable<Concept> FindConceptsByName(String name, String language);

        /// <summary>
        /// Find a reference term by code system oid
        /// </summary>
        /// <param name="code">The code</param>
        /// <param name="codeSystemOid">The oid of the code system</param>
        IEnumerable<Concept> FindConceptsByReferenceTerm(String code, String codeSystemOid);
        
        /// <summary>
        /// Returns a value which indicates whether <paramref name="a"/> implies <paramref name="b"/>
        /// </summary>
        bool Implies(Concept a, Concept b);

        /// <summary>
        /// Returns true if the concept <paramref name="concept"/> is a member of set <paramref name="set"/>
        /// </summary>
        bool IsMember(ConceptSet set, Concept concept);

        /// <summary>
        /// Gets the specified reference term from <paramref name="concept"/> in <paramref name="codeSystemOid"/>
        /// </summary>
        ReferenceTerm GetReferenceTerm(Concept concept, String codeSystemOid);

        /// <summary>
        /// Gets the specified concept
        /// </summary>
        IdentifiedData GetConcept(Guid id, Guid versionId);

        /// <summary>
        /// Obsoletes the specified concept
        /// </summary>
        IdentifiedData ObsoleteConcept(Guid key);
    }
}
