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
 * Date: 2016-1-24
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Determiner codes
    /// </summary>
    public static class DeterminerKeys
    {
        /// <summary>
        /// QUALIFIEDKIND
        /// </summary>
        public static readonly Guid DescribedQualified = Guid.Parse("604CF1B7-8891-49FB-B95F-3E4E875691BC");
        /// <summary>
        /// instance
        /// </summary>
        public static readonly Guid Specific = Guid.Parse("F29F08DE-78A7-4A5E-AEAF-7B545BA19A09");
        /// <summary>
        /// Described
        /// </summary>
        public static readonly Guid Described = Guid.Parse("AD28A7AC-A66B-42C4-91B4-DE40A2B11980");
    }
}
