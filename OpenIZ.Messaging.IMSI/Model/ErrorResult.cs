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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Model
{
    /// <summary>
    /// Identified data
    /// </summary>
    [XmlType(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
    [XmlRoot(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
    public class ErrorResult : IdentifiedData
    {

        /// <summary>
        /// Gets the date this was modified
        /// </summary>
        public override DateTimeOffset ModifiedOn
        {
            get
            {
                return DateTimeOffset.Now;
            }
        }

        /// <summary>
        /// Represents an error result
        /// </summary>
        public ErrorResult()
        {
            this.Details = new List<ResultDetail>();
        }

        /// <summary>
        /// Gets or sets the details of the result
        /// </summary>
        [XmlElement("detail")]
        public List<ResultDetail> Details { get; set; }

    }
}
