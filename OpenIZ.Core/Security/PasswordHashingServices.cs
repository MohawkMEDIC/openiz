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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// SHA256 password generator service
    /// </summary>
    public class SHA256PasswordHashingService : IPasswordHashingService
    {
        /// <summary>
        /// Encode a password using the SHA256 encoding
        /// </summary>
        public string EncodePassword(string password)
        {
            SHA256 hasher = SHA256.Create();
            return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-","").ToLower();
        }
    }

    /// <summary>
    /// SHA1 password generator service
    /// </summary>
    public class SHA1PasswordHashingService : IPasswordHashingService
    {
        /// <summary>
        /// Encode a password using the SHA256 encoding
        /// </summary>
        public string EncodePassword(string password)
        {
            SHA1 hasher = SHA1.Create();
            return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
        }
    }

    /// <summary>
    /// SHA1 password generator service
    /// </summary>
    public class MD5PasswordHashingService : IPasswordHashingService
    {
        /// <summary>
        /// Encode a password using the SHA256 encoding
        /// </summary>
        public string EncodePassword(string password)
        {
            MD5 hasher = MD5.Create();
            return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
        }
    }

    /// <summary>
    /// Plaintext password generator service
    /// </summary>
    public class PlainPasswordHashingService : IPasswordHashingService
    {
        /// <summary>
        /// Encode a password using the SHA256 encoding
        /// </summary>
        public string EncodePassword(string password)
        {
            return password;
        }
    }
}
