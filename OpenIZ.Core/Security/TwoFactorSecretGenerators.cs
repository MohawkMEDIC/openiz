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
using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents a TFA secret generator which uses the server's clock
    /// </summary>
    public class SimpleTfaSecretGenerator : ITwoFactorSecretGenerator
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name
        {
            get
            {
                return "Simple TFA generator";
            }
        }

        /// <summary>
        /// Generate the TFA secret
        /// </summary>
        public string GenerateTfaSecret()
        {
            var secretInt = DateTime.Now.Ticks % 9999;
            return String.Format("{0:0000}", secretInt);
        }

        /// <summary>
        /// Validate the secret
        /// </summary>
        public bool Validate(string secret)
        {
            int toss;
            return secret.Length == 4 && Int32.TryParse(secret, out toss);
        }
    }
}
