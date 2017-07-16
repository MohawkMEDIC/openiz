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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Interfaces
{
    /// <summary>
    /// Arguments that related to a security event
    /// </summary>
    public class SecurityAuditDataEventArgs : AuditDataEventArgs
    {
        /// <summary>
        /// Security properties that changed
        /// </summary>
        public IEnumerable<String> ChangedProperties { get; set; }

        /// <summary>
        /// Creates new security data event args
        /// </summary>
        public SecurityAuditDataEventArgs(Object obj, params string[] properties) : base(obj)
        {
            this.ChangedProperties = properties;
            this.Success = true;
        }
    }
    /// <summary>
    /// Security audit event source
    /// </summary>
    public interface ISecurityAuditEventSource : IAuditEventSource
    {

        /// <summary>
        /// Fired when security attributes related to an object change
        /// </summary>
        event EventHandler<SecurityAuditDataEventArgs> SecurityAttributesChanged;

        /// <summary>
        /// Fired when security attributes related to an object change
        /// </summary>
        event EventHandler<SecurityAuditDataEventArgs> SecurityResourceCreated;


        /// <summary>
        /// Fired when security attributes related to an object change
        /// </summary>
        event EventHandler<SecurityAuditDataEventArgs> SecurityResourceDeleted;
    }
}
