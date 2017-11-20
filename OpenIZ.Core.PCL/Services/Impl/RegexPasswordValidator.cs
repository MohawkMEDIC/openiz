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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a regular expression password validator
    /// </summary>
    public abstract class RegexPasswordValidator : IPasswordValidatorService
    {

        // Default password pattern
        public const string DefaultPasswordPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$";

        // Regex for password validation
        private readonly Regex m_passwordRegex;

        /// <summary>
        /// Create regex password validator with specified expression
        /// </summary>
        public RegexPasswordValidator(String passwordMatch)
        {
            this.m_passwordRegex = new Regex(passwordMatch);
        }

        /// <summary>
        /// Validate the specified password
        /// </summary>
        public bool Validate(string password)
        {
            return this.m_passwordRegex.IsMatch(password);
        }
    }
}
