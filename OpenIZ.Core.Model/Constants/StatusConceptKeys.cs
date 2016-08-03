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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Concept identifiers
    /// </summary>
    public static class StatusKeys
    {

        /// <summary>
        /// Status - New
        /// </summary>
        public static readonly Guid New = Guid.Parse("C34FCBF1-E0FE-4989-90FD-0DC49E1B9685");
        /// <summary>
        /// Status - Obsolete
        /// </summary>
        public static readonly Guid Obsolete = Guid.Parse("BDEF5F90-5497-4F26-956C-8F818CCE2BD2");
        /// <summary>
        /// Status - Nullified
        /// </summary>
        public static readonly Guid Nullfied = Guid.Parse("CD4AA3C4-02D5-4CC9-9088-EF8F31E321C5");
        /// <summary>
        /// Status - Active
        /// </summary>
        public static readonly Guid Active = Guid.Parse("C8064CBD-FA06-4530-B430-1A52F1530C27");
    }
}
