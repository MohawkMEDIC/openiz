using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript
{
    /// <summary>
    /// Represents a rule service base binding
    /// </summary>
    public class RuleServiceBase<TBinding> : IBusinessRulesService<TBinding> where TBinding : IdentifiedData
    {

        /// <summary>
        /// Fire after insert on the type
        /// </summary>
        public TBinding AfterInsert(TBinding data)
        {
            // Invoke the business rule
            return JavascriptBusinessRulesEngine.Current.Invoke("AfterInsert", data);
        }

        /// <summary>
        /// After obsoletion
        /// </summary>
        public TBinding AfterObsolete(TBinding data)
        {
            // Invoke the business rule
            return JavascriptBusinessRulesEngine.Current.Invoke("AfterObsolete", data);
        }

        /// <summary>
        /// After query is complete
        /// </summary>
        public IEnumerable<TBinding> AfterQuery(IEnumerable<TBinding> results)
        {
            // Invoke the business rule
            return JavascriptBusinessRulesEngine.Current.Invoke("AfterQuery", results);
        }

        /// <summary>
        /// After retrieve
        /// </summary>
        public TBinding AfterRetrieve(TBinding result)
        {
            // Invoke the business rule
            return JavascriptBusinessRulesEngine.Current.Invoke("AfterRetrieve", result);
        }

        /// <summary>
        /// After update
        /// </summary>
        public TBinding AfterUpdate(TBinding data)
        {
            return JavascriptBusinessRulesEngine.Current.Invoke("AfterUpdate", data);
        }

        /// <summary>
        /// Before insert
        /// </summary>
        public TBinding BeforeInsert(TBinding data)
        {
            return JavascriptBusinessRulesEngine.Current.Invoke("BeforeInsert", data);
        }

        /// <summary>
        /// Before an obsoletion
        /// </summary>
        public TBinding BeforeObsolete(TBinding data)
        {
            return JavascriptBusinessRulesEngine.Current.Invoke("BeforeObsolete", data);
        }

        /// <summary>
        /// Before Update
        /// </summary>
        public TBinding BeforeUpdate(TBinding data)
        {
            return JavascriptBusinessRulesEngine.Current.Invoke("BeforeUpdate", data);
        }

        /// <summary>
        /// Validate the object
        /// </summary>
        public List<DetectedIssue> Validate(TBinding data)
        {
            return JavascriptBusinessRulesEngine.Current.Validate(data);
        }
    }
}
