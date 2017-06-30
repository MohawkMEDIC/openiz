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
using MARC.Everest.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Security utility functions
    /// </summary>
    public static class SecurityUtils
    {

        /// <summary>
        /// Find the specified certificate
        /// </summary>
        public static X509Certificate2 FindCertificate(
            String storeLocation, String storeName, String x509FindType, String findValue)
        {
            
            X509Store store = new X509Store((StoreName)Enum.Parse(typeof(StoreName), storeName ?? "My"),
                (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocation ?? "LocalMachine")
            );

            try
            {
                store.Open(OpenFlags.ReadOnly);
                // Now find the certificate
                var matches = store.Certificates.Find((X509FindType)Enum.Parse(typeof(X509FindType), x509FindType ?? "FindByThumbprint"), findValue, false);
                if (matches.Count > 1)
                    throw new DuplicateItemException("More than one candidate certificate found");
                else if (matches.Count == 0)
                    throw new KeyNotFoundException("No matching certificates found");
                else
                    return matches[0];
            }
            finally
            {
                store.Close();
            }

        }
    }
}
