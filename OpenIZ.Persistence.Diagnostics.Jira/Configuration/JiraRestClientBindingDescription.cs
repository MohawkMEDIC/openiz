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
using System;
using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;

namespace OpenIZ.Persistence.Diagnostics.Jira.Configuration
{
    /// <summary>
    /// Represents a JIRA REST client binding description
    /// </summary>
    internal class JiraRestClientBindingDescription : IRestClientBindingDescription
    {
        /// <summary>
        /// Gets the content type mapper
        /// </summary>
        public IContentTypeMapper ContentTypeMapper
        {
            get
            {
                return new DefaultContentTypeMapper();
            }
        }

        /// <summary>
        /// Whether optimizations should be performed
        /// </summary>
        public bool Optimize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the security description
        /// </summary>
        public IRestClientSecurityDescription Security
        {
            get
            {
                return new JiraSecurityDescription();
            }
        }
    }
}