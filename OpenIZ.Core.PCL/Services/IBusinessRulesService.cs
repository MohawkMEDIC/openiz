using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a service that executes business rules based on triggers which happen in the persistence layer
    /// </summary>
    /// <remarks>
    /// Note: This can be done, instead with events on the persistence layer on the OpenIZ datalayer, however there 
    /// may come a time when a rule is fired without persistence occurring
    /// </remarks>
    public interface IBusinessRulesService<TModel> where TModel : IdentifiedData
    {

        /// <summary>
        /// Called before an insert occurs
        /// </summary>
        TModel BeforeInsert(TModel data);

        /// <summary>
        /// Called after an insert occurs
        /// </summary>
        TModel AfterInsert(TModel data);

        /// <summary>
        /// Called to validate a specific object
        /// </summary>
        List<DetectedIssue> Validate(TModel data);

        /// <summary>
        /// Called before an update occurs
        /// </summary>
        TModel BeforeUpdate(TModel data);

        /// <summary>
        /// Called after update committed
        /// </summary>
        TModel AfterUpdate(TModel data);

        /// <summary>
        /// Called before obsolete
        /// </summary>
        TModel BeforeObsolete(TModel data);

        /// <summary>
        /// Called after obsolete committed
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        TModel AfterObsolete(TModel data);

        /// <summary>
        /// Called after retrieve
        /// </summary>
        TModel AfterRetrieve(TModel result);

        /// <summary>
        /// Called after query
        /// </summary>
        IEnumerable<TModel> AfterQuery(IEnumerable<TModel> results);

    }

    /// <summary>
    /// Detected issue priority
    /// </summary>
    public enum DetectedIssuePriorityType
    {
        Error = 1,
        Informational = 2,
        Warning = 4
    }

    /// <summary>
    /// Represents a detected issue
    /// </summary>
    public class DetectedIssue
    {
        /// <summary>
        /// Represents a detected issue priority
        /// </summary>
        public DetectedIssuePriorityType Priority { get; set; }

        /// <summary>
        /// Text related to the issue
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// The type of issue (a concept)
        /// </summary>
        public Guid TypeKey { get; set; }
    }
}
