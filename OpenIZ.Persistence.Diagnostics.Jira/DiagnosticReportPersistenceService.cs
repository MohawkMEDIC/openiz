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
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.AMI.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Diagnostics;
using OpenIZ.Core.Http;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Persistence.Diagnostics.Jira.Configuration;
using OpenIZ.Core.Model.Constants;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Core.Security.Attribute;
using System.Security.Permissions;
using OpenIZ.Core.Security;

namespace OpenIZ.Persistence.Diagnostics.Jira
{
    /// <summary>
    /// Diagnostic report persistence service.
    /// </summary>
    public class DiagnosticReportPersistenceService : IDataPersistenceService<DiagnosticReport>
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Persistence.Diagnostics.Jira");

        // Configuration
        private JiraServiceConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.diagnostics.jira") as JiraServiceConfiguration;

		/// <summary>
		/// Initializes a new instance of the <see cref="DiagnosticReportPersistenceService"/> class.
		/// </summary>
		public DiagnosticReportPersistenceService()
        {
        }

        /// <summary>
        /// Fired when an issue is being inserted
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<DiagnosticReport>> Inserted;
        /// <summary>
        /// Fired when the issue is being inserted
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<DiagnosticReport>> Inserting;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<DiagnosticReport>> Obsoleted;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<DiagnosticReport>> Obsoleting;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PostQueryEventArgs<DiagnosticReport>> Queried;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PreQueryEventArgs<DiagnosticReport>> Querying;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PostRetrievalEventArgs<DiagnosticReport>> Retrieved;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PreRetrievalEventArgs> Retrieving;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<DiagnosticReport>> Updated;
        /// <summary>
        /// Not supported
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<DiagnosticReport>> Updating;

        /// <summary>
        /// Not supported
        /// </summary>
        public int Count(Expression<Func<DiagnosticReport, bool>> query, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public DiagnosticReport Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts the specified diagnostic report
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.Login)]
        public DiagnosticReport Insert(DiagnosticReport storageData, IPrincipal principal, TransactionMode mode)
        {
            var persistenceArgs = new PrePersistenceEventArgs<DiagnosticReport>(storageData, principal);
            this.Inserting?.Invoke(this, persistenceArgs);
            if (persistenceArgs.Cancel)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Warning, 0, "Pre-persistence event cancelled the insertion");
                return persistenceArgs.Data;
            }

            try
            {
                // Send 
                var serviceClient = new JiraServiceClient(new RestClient(this.m_configuration));

                serviceClient.Authenticate(new Model.JiraAuthenticationRequest(this.m_configuration.UserName, this.m_configuration.Password));
                var issue = serviceClient.CreateIssue(new Model.JiraIssueRequest()
                {
                    Fields = new Model.JiraIssueFields()
                    {
                        Description = storageData.Note,
                        Summary = String.Format("OpenIZ-DIAG: Issue from {0}", storageData?.Submitter?.Names?.FirstOrDefault()?.Component?.FirstOrDefault(n => n.ComponentTypeKey == NameComponentKeys.Given)?.Value),
                        IssueType = new Model.JiraIdentifier("Bug"),
                        Priority = new Model.JiraIdentifier("High"),
                        Project = new Model.JiraKey(this.m_configuration.Project),
                        Labels = new string[] { "OpenIZMobile" }
                    }
                });

                serviceClient.Client.Requesting += (o, e) =>
                {
                    e.AdditionalHeaders.Add("X-Atlassian-Token", "nocheck");
                };
                // Attachments
                List<MultipartAttachment> attachments = new List<MultipartAttachment>();

                foreach (var itm in storageData.Attachments)
                {
                    if (itm is DiagnosticBinaryAttachment)
                    {
                        var bin = itm as DiagnosticBinaryAttachment;
                        attachments.Add(new MultipartAttachment(bin.Content, "application/x-gzip", bin.FileDescription, true));
                    }
                    else
                    {
                        var txt = itm as DiagnosticTextAttachment;
                        attachments.Add(new MultipartAttachment(Encoding.UTF8.GetBytes(txt.Content), "text/plain", txt.FileName, true));
                    }
                }

                // Attach the application information
                using(var ms = new MemoryStream())
                {
                    XmlSerializer xsz = new XmlSerializer(typeof(DiagnosticApplicationInfo));
                    xsz.Serialize(ms, storageData.ApplicationInfo);
                    attachments.Add(new MultipartAttachment(ms.ToArray(), "text/xml", "appinfo.xml", true));
                }
                serviceClient.CreateAttachment(issue, attachments);
                storageData.CorrelationId = issue.Key;
                storageData.Key = Guid.NewGuid();

                // Invoke
                this.Inserted?.Invoke(this, new PostPersistenceEventArgs<DiagnosticReport>(storageData, principal));

                return storageData;
            }
            catch (Exception ex)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, ex.HResult, "Error sending to JIRA: {0}", ex);
                throw;
            }

        }

        /// <summary>
        /// Not supported
        /// </summary>
        public DiagnosticReport Obsolete(DiagnosticReport storageData, IPrincipal principal, TransactionMode mode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public IEnumerable<DiagnosticReport> Query(Expression<Func<DiagnosticReport, bool>> query, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public IEnumerable<DiagnosticReport> Query(Expression<Func<DiagnosticReport, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public DiagnosticReport Update(DiagnosticReport storageData, IPrincipal principal, TransactionMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
