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
 * Date: 2017-5-13
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents a local regex password validator
    /// </summary>
    public class LocalPasswordValidationService : RegexPasswordValidator
    {
        /// <summary>
        /// Local password validation service
        /// </summary>
        public LocalPasswordValidationService() : base((ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(OpenIzConstants.OpenIZConfigurationName) as Configuration.OpenIzConfiguration).Security.PasswordRegex ?? RegexPasswordValidator.DefaultPasswordPattern)
        {
            
        }
    }
}
