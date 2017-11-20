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
using System.Collections.ObjectModel;

namespace OpenIZ.Core.Configuration
{
    /// <summary>
    /// OpenIZ Security configuration
    /// </summary>
    public class OpenIzSecurityConfiguration 
    {

        /// <summary>
        /// Password regex
        /// </summary>
        public string PasswordRegex { get; set; }

        /// <summary>
        /// Allow unsigned applets to be installed
        /// </summary>
        public bool AllowUnsignedApplets { get; set; }

        /// <summary>
        /// Basic authentication configuration
        /// </summary>
        public OpenIzBasicAuthorization BasicAuth { get; set; }

        /// <summary>
        /// Gets or sets the claims auth
        /// </summary>
        public OpenIzClaimsAuthorization ClaimsAuth { get; set; }

        /// <summary>
        /// Trusted publishers
        /// </summary>
        public ObservableCollection<string> TrustedPublishers { get; set; }
    }
}