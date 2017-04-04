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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Represents a clinical protocol
    /// </summary>
    public interface IClinicalProtocol
    {

        /// <summary>
        /// Load the specified protocol data
        /// </summary>
        void Load(Core.Model.Acts.Protocol protocolData);

        /// <summary>
        /// Get the protocol data
        /// </summary>
        Core.Model.Acts.Protocol GetProtocolData();

        /// <summary>
        /// Gets the identifier for the protocol
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the protocol
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Calculate the clinical protocol for the given patient
        /// </summary>
        List<Act> Calculate(Patient p, IDictionary<String, Object> parameters);

        /// <summary>
        /// Update the care plan based on new data
        /// </summary>
        List<Act> Update(Patient p, List<Act> existingPlan);

        /// <summary>
        /// Called prior to performing calculation of the care protocol allowing the object to prepare the object for whatever 
        /// pre-requisite data is needed for the protocol
        /// </summary>
        void Initialize(Patient p, IDictionary<String, Object> parameters);
    }
}
