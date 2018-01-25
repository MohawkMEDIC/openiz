/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
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
        /// Invokes the specified trigger if one is registered
        /// </summary>
        /// <param name="triggerName">The name of the trigger to run</param>
        /// <param name="data">The data to be used in the trigger</param>
        /// <returns>The result of the trigger</returns>
        private TBinding InvokeTrigger(String triggerName, TBinding data)
        {
            if (JavascriptBusinessRulesEngine.Current.HasRule<TBinding>(triggerName, data?.GetType()))
                using (var instance = JavascriptBusinessRulesEngine.GetThreadInstance())
                    return instance.Invoke(triggerName, data);
            else
                return data;
        }

        /// <summary>
        /// Fire after insert on the type
        /// </summary>
        public TBinding AfterInsert(TBinding data)
        {
            return this.InvokeTrigger("AfterInsert", data);
        }

        /// <summary>
        /// After obsoletion
        /// </summary>
        public TBinding AfterObsolete(TBinding data)
        {
            return this.InvokeTrigger("AfterObsolete", data);
        }

        /// <summary>
        /// After query is complete
        /// </summary>
        public IEnumerable<TBinding> AfterQuery(IEnumerable<TBinding> results)
        {
            // Invoke the business rule
            if (results.Any() && JavascriptBusinessRulesEngine.Current.HasRule<TBinding>("AfterQuery", typeof(Bundle)))
                using (var instance = JavascriptBusinessRulesEngine.GetThreadInstance())
                    return instance.Invoke("AfterQuery", new Bundle() { Item = results.OfType<IdentifiedData>().ToList() }).Item.OfType<TBinding>();
            else
                return results;

        }

        /// <summary>
        /// After retrieve
        /// </summary>
        public TBinding AfterRetrieve(TBinding result)
        {
            return this.InvokeTrigger("AfterRetrieve", result);
        }

        /// <summary>
        /// After update
        /// </summary>
        public TBinding AfterUpdate(TBinding data)
        {
            return this.InvokeTrigger("AfterUpdate", data);

        }

        /// <summary>
        /// Before insert
        /// </summary>
        public TBinding BeforeInsert(TBinding data)
        {
            return this.InvokeTrigger("BeforeInsert", data);

        }

        /// <summary>
        /// Before an obsoletion
        /// </summary>
        public TBinding BeforeObsolete(TBinding data)
        {
            return this.InvokeTrigger("BeforeObsolete", data);

        }

        /// <summary>
        /// Before Update
        /// </summary>
        public TBinding BeforeUpdate(TBinding data)
        {
            return this.InvokeTrigger("BeforeUpdate", data);
        }

        /// <summary>
        /// Validate the object
        /// </summary>
        public List<DetectedIssue> Validate(TBinding data)
        {
            using (var instance = JavascriptBusinessRulesEngine.GetThreadInstance())
                return instance.Validate(data);
        }

    }
}
