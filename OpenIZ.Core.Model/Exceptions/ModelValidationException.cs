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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Model.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Model validation exception
    /// </summary>
    public class ModelValidationException : Exception
    {
        /// <summary>
        /// Creates a new model validation exception
        /// </summary>
        public ModelValidationException(IEnumerable<ValidationResultDetail> errors) : this(null, errors)
        {
        }

        /// <summary>
        /// Creates a new model validation exception
        /// </summary>
        public ModelValidationException(String message, IEnumerable<ValidationResultDetail> errors) : base(message)
        {
            this.ValidationDetails = errors;
        }


        /// <summary>
        /// The errors from validation
        /// </summary>
        public IEnumerable<ValidationResultDetail> ValidationDetails { get; private set; }

        /// <summary>
        /// Output model map exception as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Model Validation Exception:\r\n");
            foreach (var itm in this.ValidationDetails)
                sb.AppendFormat("{0}: {1} @ {2}\r\n", itm.Level, itm.Message, itm.Location);
            return sb.ToString();
        }
    }
}
