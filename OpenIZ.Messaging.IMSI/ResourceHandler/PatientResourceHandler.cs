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
 * Date: 2016-6-28
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Collection;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using System.Security.Permissions;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Resource handler for patients
    /// </summary>
    public class PatientResourceHandler : IResourceHandler
    {
        // repository
        private IPatientRepositoryService m_repository;

        public PatientResourceHandler()
        {
            ApplicationContext.Current.Started += (o,e) => this.m_repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
        }

        /// <summary>
        /// Gets the resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "Patient";
            }
        }

        /// <summary>
        /// Gets the type 
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(Patient);
            }
        }

        /// <summary>
        /// Create the specified patient
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Bundle bundleData = data as Bundle;
            bundleData?.Reconstitute();
            var processData = bundleData?.Entry ?? data;

            if (processData is Bundle)
                throw new InvalidOperationException("Bundle must have entry of type Patient");
            else if (processData is Patient)
            {
                var patientData = data as Patient;
                if (updateIfExists)
                    return this.m_repository.Save(patientData);
                else
                    return this.m_repository.Insert(patientData);
            }
            else
                throw new ArgumentException("Invalid persistence type");
        }

        /// <summary>
        /// Gets the specified patient data
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            return this.m_repository.Get(id, versionId);
        }

        /// <summary>
        /// Obsolete the specified patient
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteClinicalData)]
        public IdentifiedData Obsolete(Guid key)
        {
            return this.m_repository.Obsolete(key);
        }

        /// <summary>
        /// Query the specified patient
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryClinicalData)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            return this.m_repository.Find(QueryExpressionParser.BuildLinqExpression<Patient>(queryParameters));
        }

        /// <summary>
        /// Query specified patient
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.QueryClinicalData)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            return this.m_repository.Find(QueryExpressionParser.BuildLinqExpression<Patient>(queryParameters), offset, count, out totalCount);
        }

        /// <summary>
        /// Update the specified patient data
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public IdentifiedData Update(IdentifiedData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var bundleData = data as Bundle;
            bundleData?.Reconstitute();
            var saveData = bundleData?.Entry ?? data;

            if (saveData is Bundle)
                throw new InvalidOperationException("Bundle must have entry point of Patient");
            else if (saveData is Patient)
                return this.m_repository.Save(saveData as Patient);
            else
                throw new InvalidOperationException("Invalid storage type");
        }
    }
}
