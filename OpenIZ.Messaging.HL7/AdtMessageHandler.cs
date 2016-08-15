using MARC.HI.EHRS.SVC.Messaging.HAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Messaging.HAPI.TransportProtocol;
using NHapi.Base.Model;
using OpenIZ.Core.Diagnostics;
using System.Diagnostics;
using NHapi.Base.Util;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core;
using NHapi.Base.Parser;
using OpenIZ.Core.Services;
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Auditing.Services;
using MARC.HI.EHRS.SVC.Auditing.Data;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Roles;
using NHapi.Model.V25.Message;

namespace OpenIZ.Messaging.HL7
{
    /// <summary>
    /// Represents a message handler that can handle ADT messages from remote systems
    /// </summary>
    [Description("ADT Message Handler")]
    public class AdtMessageHandler : IHL7MessageHandler
    {

        // Tracer
        private TraceSource m_tracer = new TraceSource("OpenIZ.Messaging.HL7");

        /// <summary>
        /// Handle a received message on the LLP interface
        /// </summary>
        public IMessage HandleMessage(Hl7MessageReceivedEventArgs e)
        {
            IMessage response = null;
            try
            {
                if (e.Message.Version == "2.5")
                {
                    // Get the MSH segment
                    var terser = new Terser(e.Message);
                    var trigger = terser.Get("/MSH-9-2");
                    Trace.TraceInformation("Message is of type {0} {1}", e.Message.GetType().FullName, trigger);

                    switch (trigger)
                    {
                        case "Q23":
                            if (e.Message is NHapi.Model.V25.Message.QBP_Q21)
                                response = HandleIDQuery(e.Message as NHapi.Model.V25.Message.QBP_Q21, e);
                            else
                                response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
                            break;
                        case "A01":
                        case "A04":
                        case "A05":
                            if (e.Message is NHapi.Model.V25.Message.ADT_A01)
                                response = HandleAdmit(e.Message as NHapi.Model.V25.Message.ADT_A01, e);
                            else
                                response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
                            break;
                        case "A08":
                            if (e.Message is NHapi.Model.V25.Message.ADT_A01)
                                response = HandleUpdate(e.Message as NHapi.Model.V25.Message.ADT_A01, e);
                            //else if (e.Message is NHapi.Model.V25.Message.ADT_A08)
                            //    response = HandleUpdate(e.Message as NHapi.Model.V25.Message.ADT_A08, e);
                            else
                                response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
                            break;
                        case "A40":
                            if (e.Message is NHapi.Model.V25.Message.ADT_A39)
                                response = HandleMerge(e.Message as NHapi.Model.V25.Message.ADT_A39, e);
                            //else if (e.Message is NHapi.Model.V25.Message.ADT_A40)
                            //    response = HandleMerge(e.Message as NHapi.Model.V25.Message.ADT_A40, e);
                            else
                                response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
                            break;
                        default:
                            response = MessageUtil.CreateNack(e.Message, "AR", "201", ApplicationContext.Current.GetLocaleString("HL7201"));
                            Trace.TraceError("{0} is not a supported trigger", trigger);
                            break;
                    }
                }

                // response still null?
                if (response == null)
                    response = MessageUtil.CreateNack(e.Message, "AR", "203", ApplicationContext.Current.GetLocaleString("HL7203"));

            }
            catch (Exception ex)
            {
                this.m_tracer.TraceEvent(TraceEventType.Error, ex.HResult, "Error processing message {0} : {1}", e.Message?.Version, ex);
                response = MessageUtil.CreateNack(e.Message, "AR", "207", ex.Message);

            }

            return response;
        }

        //private IMessage HandleMerge(NHapi.Model.V25.Message.ADT_A40 aDT_A40, Hl7MessageReceivedEventArgs e)
        //{
        //    PipeParser parser = new PipeParser();
        //    aDT_A40.MSH.MessageType.MessageStructure.Value = "ADT_A39";
        //    var message = parser.Parse(parser.Encode(aDT_A40));
        //    if (message is NHapi.Model.V25.Message.ADT_A39)
        //        return this.HandleMerge(message as NHapi.Model.V25.Message.ADT_A39, e);
        //    else
        //        return MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
        //}

        /// <summary>
        /// Handle PIX Update
        /// </summary>
        private IMessage HandleUpdate(NHapi.Model.V25.Message.ADT_A09 aDT_A08, Hl7MessageReceivedEventArgs e)
        {
            PipeParser parser = new PipeParser();
            aDT_A08.MSH.MessageType.MessageStructure.Value = "ADT_A01";
            var message = parser.Parse(parser.Encode(aDT_A08));
            if (message is NHapi.Model.V25.Message.ADT_A01)
                return this.HandleUpdate(message as NHapi.Model.V25.Message.ADT_A01, e);
            else
                return MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
        }

        /// <summary>
        /// Handle the PIX merge request
        /// </summary>
        private IMessage HandleMerge(NHapi.Model.V25.Message.ADT_A39 request, Hl7MessageReceivedEventArgs evt)
        {
            // Get config
            var dataService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

            // Create a details array
            List<IResultDetail> dtls = new List<IResultDetail>();

            // Validate the inbound message
            MessageUtil.Validate((IMessage)request, dtls);

            IMessage response = null;

            // Control 
            if (request == null)
                return null;

            // Data controller
            //DataUtil dataUtil = new DataUtil() { Context = this.Context };
            //AuditUtil auditUtil = new AuditUtil() { Context = this.Context };

            // Construct appropriate audit
            List<AuditData> audit = new List<AuditData>();
            try
            {

                // Create Data
                for (var i = 0; i < request.PATIENTRepetitionsUsed; i++)
                {

                    var patientGroup = request.GetPATIENT(i);
                    var survivor = MessageUtil.CreatePatient(request.MSH, request.EVN, patientGroup.PID, patientGroup.PD1);
                    if (survivor == null)
                        throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));
                    var victim = dataService.Find(o => o.Identifiers.Any(id => id.Authority.DomainName == patientGroup.MRG.GetPriorAlternatePatientID(0).AssigningAuthority.NamespaceID.Value && id.Value == patientGroup.MRG.GetPriorAlternatePatientID(0).IDNumber.Value)).FirstOrDefault();
                    if (victim == null)
                        throw new KeyNotFoundException();

                    // Merge
                    var result = dataService.Merge(survivor, victim);
                }

                // Now audit
                //audit.Add(auditUtil.CreateAuditData("ITI-8", ActionType.Delete, OutcomeIndicator.Success, evt, deletedRecordIds));
                //audit.Add(auditUtil.CreateAuditData("ITI-8", ActionType.Update, OutcomeIndicator.Success, evt, updatedRecordIds));
                // Now process the result
                response = MessageUtil.CreateNack(request, dtls, typeof(NHapi.Model.V25.Message.ACK));
                MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
                (response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
                (response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                if (!dtls.Exists(o => o.Message == e.Message || o.Exception == e))
                    dtls.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
                response = MessageUtil.CreateNack(request, dtls, typeof(NHapi.Model.V25.Message.ACK));
                //audit.Add(auditUtil.CreateAuditData("ITI-8", ActionType.Delete, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>()));
            }
            finally
            {
                IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();
                if (auditSvc != null)
                    foreach (var aud in audit)
                        auditSvc.SendAudit(aud);
            }
            return response;
        }

        /// <summary>
        /// Handle PIX update
        /// </summary>
        private IMessage HandleUpdate(NHapi.Model.V25.Message.ADT_A01 request, Hl7MessageReceivedEventArgs evt)
        {
            // Get config
            var dataService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

            // Create a details array
            List<IResultDetail> dtls = new List<IResultDetail>();

            // Validate the inbound message
            MessageUtil.Validate((IMessage)request, dtls);

            IMessage response = null;

            // Control 
            if (request == null)
                return null;

            // Data controller
            //DataUtil dataUtil = new DataUtil() { Context = this.Context };
            //AuditUtil auditUtil = new AuditUtil() { Context = this.Context };

            // Construct appropriate audit
            AuditData audit = null;
            try
            {

                // Create Query Data
                var data = MessageUtil.CreatePatient(request.MSH, request.EVN, request.PID, request.PD1);
                if (data == null)
                    throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));

                var result = dataService.Save(data);

                if (result == null || result.VersionKey == null)
                    throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("DTPE001"));

                //audit = auditUtil.CreateAuditData("ITI-8", result.VersionId.UpdateMode == UpdateModeType.Update ? ActionType.Update : ActionType.Create, OutcomeIndicator.Success, evt, new List<VersionedDomainIdentifier>() { result.VersionId });
                // Now process the result
                response = MessageUtil.CreateNack(request, dtls, typeof(NHapi.Model.V25.Message.ACK));
                MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
                (response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
                (response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                if (!dtls.Exists(o => o.Message == e.Message || o.Exception == e))
                    dtls.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
                response = MessageUtil.CreateNack(request, dtls, typeof(NHapi.Model.V25.Message.ACK));
                //audit = auditUtil.CreateAuditData("ITI-8", ActionType.Create, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>());
            }
            finally
            {
                IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();
                if (auditSvc != null)
                    auditSvc.SendAudit(audit);
            }
            return response;
        }

        /// <summary>
        /// Handle a PIX admission
        /// </summary>
        private IMessage HandleAdmit(NHapi.Model.V25.Message.ADT_A01 request, Hl7MessageReceivedEventArgs evt)
        {
            // Get config
            var dataService = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            // Create a details array
            List<IResultDetail> dtls = new List<IResultDetail>();

            // Validate the inbound message
            MessageUtil.Validate((IMessage)request, dtls);

            IMessage response = null;

            // Control 
            if (request == null)
                return null;

            // Data controller
            //AuditUtil auditUtil = new AuditUtil() { Context = this.Context };
            //DataUtil dataUtil = new DataUtil() { Context = this.Context };

            // Construct appropriate audit
            AuditData audit = null;
            try
            {

                // Create Query Data
                var data = MessageUtil.CreatePatient(request.MSH, request.EVN, request.PID, request.PD1);
                if (data == null)
                    throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));

                var result = dataService.Insert(data);
                if (result == null || result.VersionKey == null)
                    throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("DTPE001"));

                //audit = auditUtil.CreateAuditData("ITI-8", result.VersionId.UpdateMode == UpdateModeType.Update ? ActionType.Update : ActionType.Create, OutcomeIndicator.Success, evt, new List<VersionedDomainIdentifier>() { result.VersionId });

                // Now process the result
                response = MessageUtil.CreateNack(request, dtls, typeof(NHapi.Model.V25.Message.ACK));
                MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
                (response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
                (response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                if (!dtls.Exists(o => o.Message == e.Message || o.Exception == e))
                    dtls.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
                response = MessageUtil.CreateNack(request, dtls, typeof(NHapi.Model.V25.Message.ACK));
                //audit = auditUtil.CreateAuditData("ITI-8", ActionType.Create, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>());
            }
            finally
            {
                IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();
                if (auditSvc != null)
                    auditSvc.SendAudit(audit);
            }
            return response;
        }


        /// <summary>
        /// Handle a PIX query
        /// </summary>
        private IMessage HandleIDQuery(NHapi.Model.V25.Message.QBP_Q21 request, Hl7MessageReceivedEventArgs evt)
        {
            // Get config
            var dataService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

            // Create a details array
            List<IResultDetail> dtls = new List<IResultDetail>();

            // Validate the inbound message
            MessageUtil.Validate((IMessage)request, dtls);

            IMessage response = null;

            // Control 
            if (request == null)
                return null;

            // Construct appropriate audit
            AuditData audit = null;

            // Data controller
            //AuditUtil auditUtil = new AuditUtil() { Context = this.Context };
            //DataUtil dataUtil = new DataUtil() { Context = this.Context };

            try
            {

                // Create Query Data
                Expression<Func<Patient, bool>> query = MessageUtil.CreateIDQuery(request.QPD);

                if (query == null)
                    throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));

                int count = Int32.Parse(request?.RCP?.QuantityLimitedRequest?.Quantity?.Value ?? "0");
                int offset = 0;
                int totalCount = 0;
                var result = dataService.Find(query, offset, count, out totalCount);

                // Update locations?
                /*
                foreach (var dtl in dtls)
                    if (dtl is PatientNotFoundResultDetail)
                        dtl.Location = "QPD^1^3^1^1";
                    else if (dtl is UnrecognizedPatientDomainResultDetail)
                        dtl.Location = "QPD^1^3^1^4";
                    else if (dtl is UnrecognizedTargetDomainResultDetail)
                        dtl.Location = "QPD^1^4^";

                
                audit = auditUtil.CreateAuditData("ITI-9", ActionType.Execute, OutcomeIndicator.Success, evt, result);
                */

                // Now process the result
                response = MessageUtil.CreateRSP_K23(result, dtls);
                //var r = dcu.CreateRSP_K23(null, null);
                // Copy QPD
                try
                {
                    (response as NHapi.Model.V25.Message.RSP_K23).QPD.MessageQueryName.Identifier.Value = request.QPD.MessageQueryName.Identifier.Value;
                    Terser reqTerser = new Terser(request),
                        rspTerser = new Terser(response);
                    rspTerser.Set("/QPD-1", reqTerser.Get("/QPD-1"));
                    rspTerser.Set("/QPD-2", reqTerser.Get("/QPD-2"));
                    rspTerser.Set("/QPD-3-1", reqTerser.Get("/QPD-3-1"));
                    rspTerser.Set("/QPD-3-4-1", reqTerser.Get("/QPD-3-4-1"));
                    rspTerser.Set("/QPD-3-4-2", reqTerser.Get("/QPD-3-4-2"));
                    rspTerser.Set("/QPD-3-4-3", reqTerser.Get("/QPD-3-4-3"));
                    rspTerser.Set("/QPD-4-1", reqTerser.Get("/QPD-4-1"));
                    rspTerser.Set("/QPD-4-4-1", reqTerser.Get("/QPD-4-4-1"));
                    rspTerser.Set("/QPD-4-4-2", reqTerser.Get("/QPD-4-4-2"));
                    rspTerser.Set("/QPD-4-4-3", reqTerser.Get("/QPD-4-4-3"));

                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
                //MessageUtil.((response as NHapi.Model.V25.Message.RSP_K23).QPD, request.QPD);

                MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                response = MessageUtil.CreateNack(request, dtls, typeof(RSP_K23));
                Terser errTerser = new Terser(response);
                // HACK: Fix the generic ACK with a real ACK for this message
                errTerser.Set("/MSH-9-2", "K23");
                errTerser.Set("/MSH-9-3", "RSP_K23");
                errTerser.Set("/QAK-2", "AE");
                errTerser.Set("/MSA-1", "AE");
                errTerser.Set("/QAK-1", request.QPD.QueryTag.Value);
                //audit = auditUtil.CreateAuditData("ITI-9", ActionType.Execute, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>());
            }
            finally
            {
                IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();
                if (auditSvc != null)
                    auditSvc.SendAudit(audit);
            }

            return response;

        }
    }
}
