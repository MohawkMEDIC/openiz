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
 * Date: 2016-11-30
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
            return JavascriptBusinessRulesEngine.Current.Invoke("AfterQuery", new Bundle() { Item = results.OfType<IdentifiedData>().ToList() }).Item.OfType<TBinding>();
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
