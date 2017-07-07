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
 * Date: 2017-3-24
 */
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Security;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Data.ADO.Configuration;
using MARC.HI.EHRS.SVC.Core;
using System.Diagnostics;
using OpenIZ.Persistence.Data.ADO.Data.Model.DataType;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Core.Services;

namespace OpenIZ.Persistence.Data.ADO.Services
{
    /// <summary>
    /// ADO OID Registrar
    /// </summary>
    public class AdoOidRegistrarService : IOidRegistrarService
    {
        /// <summary>
        /// Configuration 
        /// </summary>
        protected AdoConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(AdoDataConstants.ConfigurationSectionName) as AdoConfiguration;

        // Tracer
        private TraceSource m_tracer = new TraceSource(AdoDataConstants.TraceSourceName);

        /// <summary>
        /// Extended attributes
        /// </summary>
        private static readonly Dictionary<String, Type> m_extendedAttributes = new Dictionary<string, Type>()
        {
            { "ValidationRegex", typeof(String) }
        };

        /// <summary>
        /// Gets or sets the list of extended attributes this registrar can store
        /// </summary>
        public Dictionary<string, Type> ExtendedAttributes
        {
            get
            {
                return m_extendedAttributes;
            }
        }

        /// <summary>
        /// Delete an oid from the registrar
        /// </summary>
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.UnrestrictedMetadata)]
        public void DeleteOid(string oidName)
        {
            using (DataContext dataContext = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    dataContext.Open();
                    using (var tx = dataContext.BeginTransaction())
                    {
                        try
                        {

                            // First attempt to find the oid
                            var oid = dataContext.FirstOrDefault<DbAssigningAuthority>(o => o.DomainName == oidName);
                            if (oid == null) throw new KeyNotFoundException(oidName);

                            oid.ObsoletedByKey = AuthenticationContext.Current.Principal.GetUserKey(dataContext);
                            oid.ObsoletionTime = DateTimeOffset.Now;
                            dataContext.Update(oid);

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error deleting OID {0} : {1}", oidName, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Find oid by URI
        /// </summary>
        public OidData FindData(Uri reference)
        {
            using (DataContext dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    dataContext.Open();

                    // First attempt to find the oid
                    String referenceStr = reference.ToString();
                    var oid = dataContext.FirstOrDefault<DbAssigningAuthority>(o => o.Url == referenceStr);
                    if (oid == null) return null;
                    else
                        return this.ParseOidData(oid);

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error finding OID {0} : {1}", reference, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Parse oid data
        /// </summary>
        private OidData ParseOidData(DbAssigningAuthority oid)
        {
            var retVal = new OidData()
            {
                Description = oid.Description,
                Mnemonic = oid.DomainName,
                Name = oid.Name,
                Oid = oid.Oid,
                Ref = !String.IsNullOrEmpty(oid.Url) ? new Uri(oid.Url) : null
            };

            if (!String.IsNullOrEmpty(oid.ValidationRegex))
                retVal.Attributes.Add(new KeyValuePair<string, string>("ValidationRegex", oid.ValidationRegex));
            return retVal;
        }

        /// <summary>
        /// Find data based on OID
        /// </summary>
        public OidData FindData(string oid)
        {
            using (DataContext dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    dataContext.Open();

                    // First attempt to find the oid
                    var find = dataContext.FirstOrDefault<DbAssigningAuthority>(o => o.Oid == oid);
                    if (find == null) return null;
                    else
                        return this.ParseOidData(find);

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error finding OID {0} : {1}", oid, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Find based on attribute
        /// </summary>
        public OidData FindData(string attributeName, string attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an oid by domain name
        /// </summary>
        public OidData GetOid(string domainName)
        {
            using (DataContext dataContext = this.m_configuration.Provider.GetReadonlyConnection())
            {
                try
                {
                    dataContext.Open();

                    // First attempt to find the oid
                    var find = dataContext.FirstOrDefault<DbAssigningAuthority>(o => o.DomainName == domainName);
                    if (find == null) return null;
                    else
                        return this.ParseOidData(find);

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error finding OID {0} : {1}", domainName, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Register a new oid
        /// </summary>
        public void Register(OidData oidData)
        {
            using (DataContext dataContext = this.m_configuration.Provider.GetWriteConnection())
            {
                try
                {
                    dataContext.Open();
                    using (var tx = dataContext.BeginTransaction())
                    {
                        try
                        {

                            // First attempt to find the oid
                            var dba = new DbAssigningAuthority()
                            {
                                Description = oidData.Description,
                                Name = oidData.Name,
                                Oid = oidData.Oid,
                                Url = oidData.Ref.ToString(),
                                DomainName = oidData.Mnemonic
                            };

                            // Add attributes 
                            var regex = oidData.Attributes.FirstOrDefault(o => o.Key == "ValidationRegex");
                            var asgnDev = oidData.Attributes.FirstOrDefault(o => o.Key == "AssigningDevice");
                            dba.ValidationRegex = regex.Value;
                            if(!String.IsNullOrEmpty(asgnDev.Value))
                            {
                                var device = ApplicationContext.Current.GetService<ISecurityRepositoryService>().FindDevices(o => o.Name == asgnDev.Value).FirstOrDefault();
                                dba.AssigningDeviceKey = device?.Key;
                            }

                            dataContext.Insert(dba);

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error inserting OID {0} : {1}", oidData, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Register a new oid
        /// </summary>
        public OidData Register(string name, string oid, string desc, string uri)
        {
            var oidData = new OidData()
            {
                Name = name,
                Oid = oid,
                Description = desc,
                Ref = new Uri(uri),
                Mnemonic = name
            };
            this.Register(oidData);
            return oidData;
        }

        /// <summary>
        /// Remove the specified oid
        /// </summary>
        public void Remove(OidData oidData)
        {
            this.DeleteOid(oidData.Mnemonic);
        }
    }
}
