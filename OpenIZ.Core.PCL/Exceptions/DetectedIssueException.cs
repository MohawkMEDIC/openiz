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
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Represents an exception which contains a series of detected issue events
    /// </summary>
    public class DetectedIssueException : Exception
    {

        /// <summary>
        /// Gets the list of issues set by the BRE 
        /// </summary>
        public List<DetectedIssue> Issues { get; private set; }

        /// <summary>
        /// Creates a new detected issue exception
        /// </summary>
        public DetectedIssueException()
        {

        }

        /// <summary>
        /// Creates a new detected issue exception with the specified <paramref name="issues"/> and <paramref name="message"/>
        /// </summary>
        public DetectedIssueException(List<DetectedIssue> issues, String message) : this(issues, message, null)
        {

        }

        /// <summary>
        /// Creates a new detected issue exception with the specified <paramref name="issues"/> <paramref name="message"/> and causal exception (<paramref name="innerException"/>)
        /// </summary>
        public DetectedIssueException(List<DetectedIssue> issues, String message, Exception innerException) : base(message, innerException)
        {
            this.Issues = issues;
        }

        /// <summary>
        /// Creates a new detected issue exception with the specified issue list
        /// </summary>
        public DetectedIssueException(List<DetectedIssue> issues) : this(issues, null, null)
        {
        }

        /// <summary>
        /// Write to string
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder("BRE Violations:");
            foreach (var i in this.Issues)
#if DEBUG
                sb.AppendFormat("\r\n{0}- {1}", i.Priority, i.Text);
#else
                sb.AppendFormat("\r\n{0}- {1}", i.Priority, i.Text);
#endif
            return sb.ToString();
        }

    }
}
