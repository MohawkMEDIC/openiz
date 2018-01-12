/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Model
{
    /// <summary>
    /// A token request
    /// </summary>
    public class OAuthTokenRequest
    {
        ///<summary>Grant type</summary> 
        public String Grant_Type { get; set; }
        ///<summary>Scope of the grant</summary>
        public String Scope { get; set; }
        ///<summary>User name</summary>
        public String UserName { get; set; }
        ///<summary>Password</summary>
        public String Password { get; set; }
        ///<summary>Auth code from authorization service</summary>
        public String Code { get; set; }
        ///<summary>Red</summary>
        public String Redirect_Uri { get; set; }
        /// <summary>
        /// Refreshing token
        /// </summary>
        public String Refresh_Token { get; set; }
        /// <summary>
        /// Assertion
        /// </summary>
        public String Assertion { get; set; }
    }
}
