using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a business rules service with no behavior, but intended to help in the implementation of another 
    /// business rules service
    /// </summary>
    public abstract class BaseBusinessRulesService<TModel> : IBusinessRulesService<TModel> where TModel : IdentifiedData
    {
        /// <summary>
        /// After insert
        /// </summary>
        public virtual TModel AfterInsert(TModel data)
        {
            return data;
        }

        /// <summary>
        /// After obsolete
        /// </summary>
        public virtual TModel AfterObsolete(TModel data)
        {
            return data;
        }

        /// <summary>
        /// After query
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public virtual IEnumerable<TModel> AfterQuery(IEnumerable<TModel> results)
        {
            return results;
        }

        /// <summary>
        /// Fired after retrieve
        /// </summary>
        public virtual TModel AfterRetrieve(TModel result)
        {
            return result;
        }

        /// <summary>
        /// After update
        /// </summary>
        public virtual TModel AfterUpdate(TModel data)
        {
            return data;
        }

        /// <summary>
        /// Before insert complete
        /// </summary>
        public virtual TModel BeforeInsert(TModel data)
        {
            return data;
        }

        /// <summary>
        /// Before obselete
        /// </summary>
        public virtual TModel BeforeObsolete(TModel data)
        {
            return data;
        }

        /// <summary>
        /// Before update
        /// </summary>
        public virtual TModel BeforeUpdate(TModel data)
        {
            return data;
        }

        /// <summary>
        /// Validate the specified object
        /// </summary>
        public virtual List<DetectedIssue> Validate(TModel data)
        {
            return new List<DetectedIssue>();
        }
    }
}
