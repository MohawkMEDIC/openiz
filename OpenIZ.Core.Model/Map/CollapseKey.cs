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
 * Date: 2016-4-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Association map
    /// </summary>
    [XmlType( nameof(CollapseKey), Namespace = "http://openiz.org/model/map")]
    public class CollapseKey
    {
        /// <summary>
        /// Gets or sets the name of the property can be collapsed if a key is used
        /// </summary>
        [XmlAttribute("propertyName")]
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the key in the domain model which "PropertyName" can be collapsed
        /// </summary>
        [XmlAttribute("keyName")]
        public String KeyName { get; set; }


        /// <summary>
        /// Validate the collapse key
        /// </summary>
        public IEnumerable<ValidationResultDetail> Validate(Type domainClass)
        {

            List<ValidationResultDetail> retVal = new List<ValidationResultDetail>();
            if (String.IsNullOrEmpty(this.KeyName))
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("collapseKey missing @keyName"), null, null));
            if (String.IsNullOrEmpty(this.PropertyName))
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("collapseKey missing @propertyName"), null, null));
            if (!String.IsNullOrEmpty(this.PropertyName) && domainClass?.GetRuntimeProperty(this.PropertyName) == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", domainClass?.Name, this.PropertyName), null, null));
            if (!String.IsNullOrEmpty(this.KeyName) && domainClass?.GetRuntimeProperty(this.KeyName) == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", domainClass?.Name, this.KeyName), null, null));

            if (!String.IsNullOrEmpty(this.KeyName))
            {
                var runtimeProperty = domainClass?.GetRuntimeProperty(this.KeyName);
                if (runtimeProperty?.PropertyType != typeof(Guid) &&
                    runtimeProperty?.PropertyType != typeof(Guid?) &&
                    runtimeProperty?.PropertyType != typeof(Decimal) &&
                    runtimeProperty?.PropertyType != typeof(Decimal?))
                    retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} must be one of [Guid, Nullable<Guid>, Decimal, Nullable<Decimal>]", domainClass?.Name, this.KeyName), null, null));
            }
            
            return retVal;
        }
    }
}