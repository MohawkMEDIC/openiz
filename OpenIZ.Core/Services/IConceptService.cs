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
    public interface IConceptService
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
        ReferenceTerm SaveReferenceTerm(ReferenceTerm term);

        /// <summary>
        /// Saves a concept class
        /// </summary>
        ConceptClass SaveConceptClass(ConceptClass clazz);

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
        /// Find concepts 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query);

        /// <summary>
        /// Find concept set
        /// </summary>
        IEnumerable<ConceptSet> FindConceptSet(Expression<Func<ConceptSet, bool>> query);

        /// <summary>
        /// Find concepts 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalResults);

        /// <summary>
        /// Find concept set
        /// </summary>
        IEnumerable<ConceptSet> FindConceptSet(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults);

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
