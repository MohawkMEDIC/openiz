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
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Security.Attribute;
using System.Security.Permissions;
using OpenIZ.Core.Security;
using System.Net.Mail;
using OpenIZ.Persistence.Diagnostics.Email.Configuration;
using OpenIZ.Core.Model.Constants;
using System.Net;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Persistence.Diagnostics.Email
{
    /// <summary>
    /// Persistence service for diagnostics
    /// </summary>
    public class DiagnosticReportPersistenceService : IDataPersistenceService<DiagnosticReport>
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Persistence.Diagnostics.Email");

        // Configuration
        private DiagnosticEmailServiceConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.diagnostics.email") as DiagnosticEmailServiceConfiguration;

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
                var issueId = Guid.NewGuid().ToString().Substring(0, 4);
                // Setup message

                MailMessage bugMessage = new MailMessage();
                bugMessage.From = new MailAddress(this.m_configuration.Smtp.From, $"OpenIZ Bug on behalf of {storageData?.Submitter?.Names?.FirstOrDefault()?.Component?.FirstOrDefault(n => n.ComponentTypeKey == NameComponentKeys.Given)?.Value}");
                foreach (var itm in this.m_configuration.Recipients)
                    bugMessage.To.Add(itm);
                bugMessage.Subject = $"ISSUE #{issueId}";
                bugMessage.Body = storageData.Note;

                // Add attachments
                foreach (var itm in storageData.Attachments)
                {
                    if (itm is DiagnosticBinaryAttachment)
                    {
                        var bin = itm as DiagnosticBinaryAttachment;
                        bugMessage.Attachments.Add(new Attachment(new MemoryStream(bin.Content), itm.FileDescription ?? itm.FileName, "application/x-gzip"));
                    }
                    else
                    {
                        var txt = itm as DiagnosticTextAttachment;
                        bugMessage.Attachments.Add(new Attachment(new MemoryStream(Encoding.UTF8.GetBytes(txt.Content)), itm.FileDescription ?? itm.FileName, "text/plain"));
                    }
                }

                // Attach the application information
                using (var ms = new MemoryStream())
                {
                    XmlSerializer xsz = new XmlSerializer(typeof(DiagnosticApplicationInfo));
                    xsz.Serialize(ms, storageData.ApplicationInfo);
                    bugMessage.Attachments.Add(new Attachment(new MemoryStream(ms.ToArray()), "appinfo.xml", "text/xml"));
                }

                SmtpClient smtpClient = new SmtpClient(this.m_configuration.Smtp.Server.Host, this.m_configuration.Smtp.Server.Port);
                smtpClient.UseDefaultCredentials = String.IsNullOrEmpty(this.m_configuration.Smtp.Username);
                smtpClient.EnableSsl = this.m_configuration.Smtp.Ssl;
                if (!(smtpClient.UseDefaultCredentials))
                    smtpClient.Credentials = new NetworkCredential(this.m_configuration.Smtp.Username, this.m_configuration.Smtp.Password);
                smtpClient.SendCompleted += (o, e) =>
                {
                    this.m_traceSource.TraceInformation("Successfully sent message to {0}", bugMessage.To);
                    if (e.Error != null)
                        this.m_traceSource.TraceEvent(TraceEventType.Error, 0, e.Error.ToString());
                    (o as IDisposable).Dispose();
                };
                this.m_traceSource.TraceInformation("Sending bug email message to {0}", bugMessage.To);
                smtpClient.Send(bugMessage);

                // Invoke
                this.Inserted?.Invoke(this, new PostPersistenceEventArgs<DiagnosticReport>(storageData, principal));
                storageData.CorrelationId = issueId;
                storageData.Key = Guid.NewGuid();
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
