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
 * User: khannan
 * Date: 2017-7-7
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Services;

namespace OizDevTool
{
    /// <summary>
    /// Represents a CSD import utility.
    /// </summary>
    public partial class CsdImport
    {
        /// <summary>
        /// Maps the assigning authority.
        /// </summary>
        /// <param name="otherId">The other identifier.</param>
        /// <returns>AssigningAuthority.</returns>
        /// <exception cref="System.InvalidOperationException">If the assigning authority is not found.</exception>
        private static AssigningAuthority MapAssigningAuthority(otherID otherId)
        {
            var metadataService = ApplicationContext.Current.GetService<IMetadataRepositoryService>();

            String assigningAuthorityName = otherId.assigningAuthorityName;
            // sometimes codes are used underneath the authority name we have to differentiate between these as they are different identifiers
            if (!String.IsNullOrEmpty(otherId.code))
            {
                Uri r = null;
                if (Uri.TryCreate(assigningAuthorityName, UriKind.Absolute, out r))
                    assigningAuthorityName += "/" + otherId.code;
                else
                    assigningAuthorityName += "." + otherId.code;
            }

            // lookup by NSID
            var assigningAuthority = metadataService.FindAssigningAuthority(a => a.DomainName == assigningAuthorityName).FirstOrDefault();

            if (assigningAuthority != null)
            {
                return assigningAuthority;
            }

            ShowWarningMessage($"Warning, unable to locate assigning authority by NSID using value: {assigningAuthorityName}, will attempt to lookup by URL");

            // lookup by URL
            assigningAuthority = metadataService.FindAssigningAuthority(a => a.Url == assigningAuthorityName).FirstOrDefault();

            if (assigningAuthority != null)
            {
                return assigningAuthority;
            }

            ShowWarningMessage($"Warning, unable to locate assigning authority by URL using value: {otherId.assigningAuthorityName}, will attempt to lookup by OID");

            // lookup by OID
            assigningAuthority = metadataService.FindAssigningAuthority(a => a.Oid == assigningAuthorityName).FirstOrDefault();

            if (assigningAuthority == null)
            {
                ShowErrorOnNotFound($"Error, {emergencyMessage} Unable to locate assigning authority using NSID, URL, or OID. Has {otherId.assigningAuthorityName} been added to the OpenIZ assigning authority list?");
            }

            return assigningAuthority;
        }

        /// <summary>
        /// Maps the coded type.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="codingScheme">The coding scheme.</param>
        /// <returns>Concept.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// code - Value cannot be null
        /// or
        /// codingScheme - Value cannot be null
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Unable to locate service</exception>
        private static Concept MapCodedType(string code, string codingScheme)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), "Value cannot be null");
            }

            if (codingScheme == null)
            {
                throw new ArgumentNullException(nameof(codingScheme), "Value cannot be null");
            }

            // since CSD coded types are oid based, we want to make sure that the coding scheme starts with "oid"
            if (!codingScheme.StartsWith("oid:") && !codingScheme.StartsWith("http://") && !codingScheme.StartsWith("urn:"))
            {
                codingScheme = "urn:oid:" + codingScheme;
            }

            var compositeKey = new CompositeKey(code, codingScheme);

            Concept concept;

            if (conceptKeys.All(c => c.Key != compositeKey))
            {
                var conceptRepositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

                if (conceptRepositoryService == null)
                {
                    throw new InvalidOperationException($"Unable to locate service: {nameof(IConceptRepositoryService)}");
                }

                concept = conceptRepositoryService.FindConceptsByReferenceTerm(code, new Uri(codingScheme)).FirstOrDefault();

                if (concept == null)
                {
                    ShowErrorOnNotFound($"Error, {emergencyMessage} Unable to locate concept using code: {code} and coding scheme: {codingScheme}");
                }
                else
                {
                    conceptKeys.Add(compositeKey, concept.Key.Value);
                }
            }
            else
            {
                Guid key;
                conceptKeys.TryGetValue(compositeKey, out key);
                concept = new Concept
                {
                    Key = key
                };
            }

            return concept;
        }

        /// <summary>
        /// Maps the status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="codeSystem">The code system.</param>
        /// <returns>Returns a status key.</returns>
        /// <exception cref="System.InvalidOperationException">IConceptRepositoryService</exception>
        private static Guid? MapStatusCode(string code, string codeSystem)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), "Value cannot be null");
            }

            if (codeSystem == null)
            {
                throw new ArgumentNullException(nameof(codeSystem), "Value cannot be null");
            }

            var conceptService = ApplicationContext.Current.GetConceptService();

            return conceptService.FindConceptsByReferenceTerm(code, new Uri(codeSystem)).FirstOrDefault()?.Key;
        }
    }
}
