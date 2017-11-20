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
 * Date: 2017-4-10
 */
using System;
using System.Collections.Generic;

namespace OpenIZ.Core.Interfaces
{

    /// <summary>
    /// Event arguments for data disclosure
    /// </summary>
    public class AuditDataEventArgs : EventArgs
    {

        /// <summary>
        /// Objects which were impacted
        /// </summary>
        public IEnumerable<Object> Objects { get; private set; }

        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Creates a new data event args
        /// </summary>
        public AuditDataEventArgs(IEnumerable<object> objects)
        {
            this.Success = true;
            this.Objects = objects;
        }

        /// <summary>
        /// Creates a new data event args
        /// </summary>
        public AuditDataEventArgs(params object[] objects)
        {
            this.Objects = objects;
        }
    }

    /// <summary>
    /// Audit the fact that data was dislcosed
    /// </summary>
    public class AuditDataDisclosureEventArgs : AuditDataEventArgs
    {

        /// <summary>
        /// Gets the query that caused the data to be disclosed
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Audit disclosure 
        /// </summary>
        public AuditDataDisclosureEventArgs(string query, IEnumerable<object> objects) : base(objects)
        {
            this.Query = query;
        }
    }

    /// <summary>
    /// Represents a service which creates data event related audits
    /// </summary>
    public interface IAuditEventSource
    {

        /// <summary>
        /// Indicates that data was imported
        /// </summary>
        event EventHandler<AuditDataEventArgs> DataCreated;

        /// <summary>
        /// Data was updated
        /// </summary>
        event EventHandler<AuditDataEventArgs> DataUpdated;

        /// <summary>
        /// Data was obsoleted
        /// </summary>
        event EventHandler<AuditDataEventArgs> DataObsoleted;

        /// <summary>
        /// Indicates that data was disclosed
        /// </summary>
        event EventHandler<AuditDataDisclosureEventArgs> DataDisclosed;
    }
}